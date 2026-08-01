# IA.md — Uso de Inteligencia Artificial

## Prompts de Jorge (ítems 1–3: Implementación de CQRS)

**Herramienta utilizada:** ChatGPT (OpenAI), vía chat web.
**Alcance:** Orientación para el patrón CQRS en Shortly, definición de la estructura del proyecto, creación de modelos Command/Query, resolución de errores de compilación y validación de la arquitectura.

## 1. Ítem 1 — Command & Query Models

* "¿Cómo separo los modelos de Command y Query sin modificar la entidad Link?"
* "¿Qué diferencia existe entre una entidad de dominio, un Command y un Query?"
* "¿Qué información debería contener un CreateUrlCommand?"

## 2. Ítem 2 — Command Handlers

* "¿Qué responsabilidades debe tener un Command Handler?"
* "¿Cómo mover la lógica de creación desde LinkService hacia un Command Handler?"
* "¿Cómo registrar los Command Handlers en Program.cs mediante Dependency Injection?"
* "¿Cómo resolver errores de compilación relacionados con Command Handlers y namespaces?"
* "¿Cómo validar que la creación de URLs se realiza mediante el Command Handler?"

## 3. Ítem 3 — Query Handlers

* "¿Qué diferencia existe entre un Query Handler y un Command Handler?"
* "¿Cómo registrar los Query Handlers en Dependency Injection?"
* "¿Cómo resolver errores de compilación relacionados con Query Handlers y namespaces?"
* "¿Cómo validar que las operaciones de lectura utilizan exclusivamente los Query Handlers?"

---

## Prompts de Felipe (ítems 4, 5, 3-GetStatsQuery)

**Herramienta utilizada:** Claude (Anthropic), vía chat web.
**Alcance:** Implementación de la separación de repositorios read/write (ítem 4), el schema de lectura optimizado con sincronización (ítem 5), y `GetStatsQuery` (parte pendiente del ítem 3).

## 1. Análisis inicial del proyecto

* "Tengo que hacer el taller2, me podrias dar una idea de que es lo que falta" (adjuntando el proyecto)

## 2. Ítem 4 — Repositorios read/write separados

* "Esta bien o falta algo mas?"
* Troubleshooting de errores de compilación: contenido de `DeleteUrlCommandHandler.cs` mezclado por error dentro de `GetUrlQueryHandler.cs`.
* "Como puedo probar esta parte"
* "Creame el commit para subirlo"

## 3. Ítem 5 — Schema de lectura optimizado

* "Como empiezo la siguiente parte"
* Troubleshooting de compilación: `LinkReadRepository.cs` (implementación) no se había reemplazado, solo la interfaz `ILinkReadRepository.cs`.
* "como lo pruebo?"

## 4. Ítem 3 — GetStatsQuery (parte pendiente)

* "que sigue"
* "sisi" (confirmando la propuesta de qué estadísticas incluir: total de links, total de clicks, link más clickeado)
* "Esta bien esto?"
* "como lo pruebo?"

---

### Resumen de uso

La IA se utilizó como asistente de pair-programming para:

* **Diseño de arquitectura CQRS:** Definir cómo separar los repositorios de lectura/escritura y diseñar la tabla de lectura desnormalizada (`LinkReadModel`) con su lógica de sincronización.
* **Troubleshooting de compilación:** Diagnosticar errores por archivos mal reemplazados o con contenido mezclado durante la integración manual del código.
* **Validación funcional:** Guiar pruebas paso a paso en el navegador para confirmar que cada ítem funcionaba de punta a punta (crear, listar, redirigir, ver estadísticas).
* **Manejo de Git:** Redactar los mensajes de commit siguiendo el estándar del proyecto.