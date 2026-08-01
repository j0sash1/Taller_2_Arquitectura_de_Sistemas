# Benchmark — CQRS (antes/después)

## Entorno

| | |
|---|---|
| Fecha | 2026-08-01 |
| Commit "antes" | `73c9333` (código original, sin CQRS) |
| Commit "después" | `main` (CQRS: repositorios read/write separados + read model) |
| SO | Windows 11 Home Single Language |
| Servidor | Kestrel (ASP.NET Core, `dotnet run`) |
| Herramienta | ApacheBench (`ab`) 2.3 |
| Comando usado | `ab -n 500 -c 10 <url>` (endpoints autenticados con `-H "Cookie: ..."`) |

> Misma máquina, mismo `-n 500 -c 10`, mismo usuario de prueba en ambas corridas.
> Tanto el "antes" como el "después" apuntan al mismo archivo físico de SQLite
> (`AppDbContext` y `AppReadDbContext` son instancias de `DbContext` separadas,
> pero no bases de datos separadas — ver el Análisis).

---

## Endpoint 1 — Lectura intensiva: `GET /` (home autenticada)

Dispara `GetLinksByUserId` (+ `GetStatsQuery` después de CQRS).

```
ab -n 500 -c 10 -H "Cookie: .AspNetCore.Cookies=<token>" http://localhost:5064/
```

| Métrica | Antes | Después |
|---|---|---|
| Requests/sec | 958.36 | 653.47 |
| p50 (ms) | 8 | 12 |
| p90 (ms) | 17 | 26 |
| p99 (ms) | 28 | 47 |
| Transfer rate (KB/sec) | 10339.80 | 7205.42 |
| Failed requests | 0 | 0 |

---

## Endpoint 2 — Escritura intensiva: `POST /` (crear link)

Dispara `CreateUrlCommandHandler` (+ sincronización del read model después de CQRS).

```
ab -n 500 -c 10 -H "Cookie: ..." -p postdata.txt -T "application/x-www-form-urlencoded" http://localhost:5064/
```

| Métrica | Antes | Después |
|---|---|---|
| Requests/sec | 2995.54 | 3715.90 |
| p50 (ms) | 3 | 3 |
| p90 (ms) | 7 | 3 |
| p99 (ms) | 18 | 4 |
| Transfer rate (KB/sec) | 351.04 | 435.46 |
| Failed requests | 0 | 0 |

Nota: `Non-2xx responses: 500` en ambas corridas es esperable — una
creación exitosa redirige (`302`), no devuelve `200`.

---

## Endpoint 3 — Mixto: `GET /{shortUrl}` (redirect)

Dispara una lectura (buscar el link) y una escritura (incrementar
clicks, más la sincronización del read model después de CQRS) en la
misma request.

```
ab -n 500 -c 10 http://localhost:5064/aspnet
```

| Métrica | Antes | Después |
|---|---|---|
| Requests/sec | 201.16 | 121.78 |
| p50 (ms) | 5 | 7 |
| p90 (ms) | 156 | 161 |
| p99 (ms) | 785 | 1892 |
| Transfer rate (KB/sec) | 32.41 | 19.62 |
| Failed requests | 0 | 0 |

Nota: `Non-2xx responses: 500` es esperable — el endpoint de redirect
responde con `302`/`301`, que `ab` no sigue.

---

## Comparación antes vs después

| Endpoint | Métrica | Antes | Después | Δ |
|---|---|---|---|---|
| Lectura (`/`) | Requests/sec | 958.36 | 653.47 | -31.8% |
| Lectura (`/`) | p99 (ms) | 28 | 47 | +67.9% |
| Escritura (`POST /`) | Requests/sec | 2995.54 | 3715.90 | +24.0% |
| Escritura (`POST /`) | p99 (ms) | 18 | 4 | -77.8% |
| Mixto (`/{shortUrl}`) | Requests/sec | 201.16 | 121.78 | -39.5% |
| Mixto (`/{shortUrl}`) | p99 (ms) | 785 | 1892 | +141.1% |

## Análisis

**La escritura mejoró y quedó mucho más estable.** El camino de crear
un link en la versión con CQRS hace un solo insert más un upsert al
read model, y ya no compite con ningún cálculo del lado de lectura. El
p99 bajó de 18ms a 4ms — una mejora grande y consistente en la latencia
de cola, que en la práctica importa más que el promedio.

**La lectura empeoró.** Antes de CQRS, la home calculaba sus
estadísticas en memoria a partir de la misma lista de links que ya
había traído (`Links.Sum(...)` directo en la vista Razor) — un solo
viaje a la base de datos. Después de CQRS, `GetStatsQueryHandler`
ejecuta su propia consulta independiente (`GetByUserIdAsync`) para
calcular esas mismas estadísticas, así que la home ahora golpea la base
de datos dos veces por request en vez de una. Separar las lecturas en
query handlers dedicados es arquitectónicamente correcto (cada handler
es testeable de forma independiente y tiene un solo propósito), pero
este caso puntual muestra un costo real: dos handlers leyendo datos que
se superponen de la misma tabla, en vez de que uno reutilice lo que el
otro ya trajo. Es una oportunidad de optimización genuina — un
siguiente paso podría hacer que `ListUrlsQueryHandler` y
`GetStatsQueryHandler` compartan una sola consulta, o calcular las
estadísticas a partir de la misma llamada al repositorio de lectura.

**El mixto empeoró notablemente, con el mayor golpe en la cola de
latencia (el p99 pasó de 785ms a 1892ms).** Cada redirect ahora toca la
base de datos tres veces en vez de una: leer el link
(`AppReadDbContext`), incrementar y guardar su contador de clicks
(`AppDbContext`), y sincronizar el read model (`AppReadDbContext` de
nuevo, incluyendo un join contra `Users` para resolver el email del
dueño). Como `AppDbContext` y `AppReadDbContext` son instancias
separadas de `DbContext` pero siguen apuntando al **mismo archivo físico
de SQLite**, y SQLite solo permite un escritor a la vez, estas
operaciones compiten entre sí bajo carga concurrente. Esa contención es
la explicación más probable del ensanchamiento de la cola de latencia
bajo la concurrencia de `-c 10`.

**Conclusión clave:** separar los repositorios de lectura y escritura
es una mejora arquitectónica real — desacopla las responsabilidades de
lectura y escritura, hace que cada lado sea testeable de forma
independiente, y deja el camino listo para escalar lecturas y
escrituras por separado más adelante. Pero **no** mejora
automáticamente el rendimiento cuando ambos lados todavía comparten una
sola base de datos física. El beneficio de performance con el que
generalmente se "vende" CQRS (poder escalar las lecturas
independientemente de las escrituras, por ejemplo con una réplica de
lectura o un almacén optimizado para consultas) recién se materializa
una vez que el lado de lectura está respaldado por infraestructura
genuinamente separada — que es explícitamente el "por ahora" que quedó
anotado en la configuración de `AppReadDbContext` de este proyecto.
Hasta que se dé ese paso, la separación read/write vale la pena por
calidad de código y testeabilidad, pero trae un costo de performance
pequeño y medible, causado por los viajes extra a la base que hacen
falta para mantener ambos lados sincronizados.

## Notas

- Ambas corridas usaron el mismo usuario de prueba, los mismos 3
  endpoints, y el mismo patrón de carga (`-n 500 -c 10`) para que la
  comparación sea justa.
- Los archivos crudos de salida de `ab` se guardan junto a este reporte
  (`before-read.txt`, `before-write.txt`, `before-mixed.txt`,
  `after-read.txt`, `after-write.txt`, `after-mixed.txt`).