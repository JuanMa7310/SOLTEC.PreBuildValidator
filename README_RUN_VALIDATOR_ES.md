# SOLTEC.PreBuildValidator - Guía de Ejecución Manual

## 📚 Propósito

Este documento explica cómo ejecutar manualmente el **SOLTEC.PreBuildValidator** sin depender del proceso de compilación.

Es útil para realizar validaciones independientes durante el desarrollo, en pipelines de CI/CD, o antes de hacer un merge en la rama principal.

---

## 🚀 Cómo Ejecutarlo

Para ejecutar el validador manualmente, debes ejecutar el ejecutable **y proporcionar el nombre del proyecto como argumento**.

### 🎯 Ejemplo de Comando

```bash
SOLTEC.PreBuildValidator.exe SOLTEC.Core
```

Donde:
- `SOLTEC.Core` es el nombre de la carpeta del proyecto y del archivo `.csproj` que deseas validar.

✅ **Proporcionar el nombre del proyecto es obligatorio**.  
✅ **Si no se proporciona este argumento, el validador mostrará un error y finalizará.**

---

## 📋 Qué Sucede Durante la Ejecución

Cuando ejecutas el validador:

1. Validará la estructura y el código del proyecto.
2. Validará:
   - La presencia de `<LangVersion>` en el `.csproj`.
   - La cobertura de pruebas para las clases públicas/protegidas.
   - La existencia de métodos de prueba para métodos públicos/protegidos.
   - La ausencia de comentarios TODO/FIXME.
   - La documentación XML de clases, métodos y propiedades públicas/protegidas.

---

## 🛑 Manejo de Fallos

Si alguna validación falla:

- Se lanzará una **ValidationException**.
- El error describirá claramente el problema encontrado.
- La aplicación terminará con un código de error (`Environment.Exit(1)`).

Ejemplo de salida si una clase no tiene cobertura de prueba:

```plaintext
Validation failed: Test coverage validation failed: The following classes are missing corresponding test classes: CustomerService, OrderManager.
```

---

## 📢 Notas

- Solo existe un único archivo ejecutable con declaraciones de nivel superior (Top-Level Program.cs), garantizando una ejecución simple y eficiente.
- Archivos generados como `.Designer.cs`, `.g.cs` y `.AssemblyInfo.cs` son excluidos automáticamente de la validación.
- Todos los errores de validación son precisos y agrupados por tipo de error.

---

## 👨‍💻 Autor

Desarrollado y mantenido por JuanMa.

---
