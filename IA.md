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
