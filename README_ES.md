# SOLTEC.PreBuildValidator

## 📚 Resumen

**SOLTEC.PreBuildValidator** es una aplicación de consola ligera que valida reglas esenciales del proyecto **antes de permitir que la compilación tenga éxito**.

Garantiza que la base de código permanezca limpia, documentada, probada y configurada correctamente según los estándares del proyecto.

---

## 🚀 Características principales

- **Validación de versión de lenguaje**: Asegura que el proyecto defina un `<LangVersion>` válido en el archivo `.csproj`.
- **Validación de cobertura de pruebas**: Garantiza que cada clase pública/protegida tenga una clase de prueba correspondiente.
- **Validación de presencia de métodos de prueba**: Verifica que cada método público/protegido tenga al menos un método de prueba.
- **Validación de TODO/FIXME**: Detecta comentarios TODO o FIXME que hayan quedado en el código de producción.
- **Validación de documentación XML**: Asegura que todas las clases, métodos y propiedades públicas/protegidas tengan comentarios de documentación XML (`///`).

---

## 🛠️ Tecnologías utilizadas

- **.NET 8.0**
- **C# 12**
- **Top-Level Statements** para el punto de entrada del programa
- **Excepción personalizada ValidationException** para un manejo de errores claro
- **Motores de validación optimizados con Regex** para el análisis de código

---

## 📋 Funcionamiento

Al compilar el proyecto principal (por ejemplo, `SOLTEC.Core`),  
**este validador se ejecuta automáticamente como un evento Post-Build** y:

1. Valida que se cumplan todos los estándares de codificación y documentación.
2. Lanza un error claro y específico si alguna validación falla.
3. Detiene el proceso de compilación si se detectan errores.

Si todas las validaciones pasan, la compilación tiene éxito.

---

## ⚙️ Uso

### 🎯 Comando para ejecutar

El validador **debe recibir** el nombre del proyecto/solución como argumento en la línea de comandos.

Ejemplo:

```bash
SOLTEC.PreBuildValidator.exe SOLTEC.Core
```

Donde:
- `"SOLTEC.Core"` es el nombre del proyecto que se desea validar (el nombre de la carpeta y el archivo `.csproj` deben coincidir).

---

### 📄 Integración Post-Build (SOLTEC.Core.csproj)

Asegúrate de que tu archivo `.csproj` (por ejemplo, `SOLTEC.Core.csproj`) incluya:

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
  <Exec Command="&quot;$(SolutionDir)Tools\SOLTEC.PreBuildValidator\bin\Debug\net8.0\SOLTEC.PreBuildValidator.exe&quot; SOLTEC.Core" />
</Target>
```

✅ Esto garantiza que cada vez que se construya el proyecto, se ejecutará automáticamente la validación.

---

## 📑 Flujo de validación

| Paso de validación | Descripción |
|:---|:---|
| LangVersionValidator | Verifica la existencia y el contenido de `<LangVersion>` en el `.csproj`. |
| TestCoverageValidator | Asegura que cada clase pública/protegida tenga una clase de prueba correspondiente. |
| TestMethodPresenceValidator | Verifica que cada método público/protegido tenga al menos un método de prueba. |
| TodoFixmeValidator | Detecta cualquier comentario TODO o FIXME en el código de producción. |
| XmlDocValidator | Valida la documentación XML para todos los miembros públicos/protegidos. |

---

## 🛑 Manejo de errores

Si falla alguna validación:

- Se lanzará una **ValidationException**.
- Se mostrará un mensaje claro y detallado en la consola.
- El proceso de compilación se detendrá inmediatamente.
- Deberás corregir el problema indicado antes de compilar nuevamente.

Ejemplo de salida de error:

```plaintext
Validation failed: Test coverage validation failed: The following classes are missing corresponding test classes: CustomerService, OrderManager.
```

---

## 📢 Notas

- Los archivos generados automáticamente como `.Designer.cs`, `.g.cs` y `.AssemblyInfo.cs` son excluidos de las validaciones.
- El validador es extensible: se pueden añadir nuevas reglas de validación fácilmente si se requiere.

---

## 👨‍💻 Autor

Desarrollado y mantenido por JuanMa.

---
