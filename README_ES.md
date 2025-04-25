# SOLTEC.PreBuildValidator

**SOLTEC.PreBuildValidator** es una herramienta de consola para .NET 8 diseñada para ejecutar validaciones automáticas antes de compilar el proyecto principal `SOLTEC.Core`.

## 🔍 Propósito

- Validar estructura, convenciones de nombres y documentación XML.
- Prevenir despliegues con componentes incompletos o sin documentar.
- Asegurar consistencia antes de subir a GitHub o regenerar la wiki.

## 🛠️ Funcionalidades

- Detecta automáticamente el proyecto a validar a partir del archivo `.sln`.
- Escanea en busca de:
  - Comentarios XML faltantes
  - Bloques `example` incompletos
  - Violaciones de nomenclatura (prefijos incorrectos)
  - Enumeraciones sin documentar

## 🚀 Uso

Utiliza los scripts incluidos para compilar y ejecutar el validador:
```
./run-validator.sh       # Linux/macOS
run-validator.bat        # Windows
```

> La herramienta devuelve un estado claro de éxito o fallo, y muestra los errores directamente en consola.
