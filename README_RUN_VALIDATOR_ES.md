# Instrucciones para los Scripts - SOLTEC.PreBuildValidator

Esta carpeta incluye dos scripts para compilar y ejecutar la herramienta de validación previa a la compilación.

Están diseñados para funcionar **sin importar desde qué carpeta se ejecuten**.

## 🚀 Windows

Para ejecutar el validador:
```
run-validator.bat
```

Este script:
1. Cambia a la carpeta del propio script
2. Compila el proyecto con `dotnet build`
3. Ejecuta el DLL resultante del validador

## 🐧 Linux/macOS

Haz que el script sea ejecutable:
```bash
chmod +x run-validator.sh
```

Luego ejecuta:
```bash
./run-validator.sh
```

> El validador comprobará automáticamente y reportará cualquier problema en el proyecto antes de compilar.
