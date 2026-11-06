# G1 · Ropero Solidario – Fundación Adecco

Repositorio del **Grupo 1** del curso de .NET: aplicación web con **C#, ASP.NET Core MVC, Razor y Entity Framework Core**, desarrollada en equipo con Git y GitHub.

## Temática del grupo: Ropero Solidario – Fundación Adecco

Aplicación web para gestionar un **ropero solidario** con ropa donada, destinada a personas que necesitan vestimenta adecuada para entrevistas de trabajo y no disponen de recursos para adquirirla.

### Funcionalidades principales

- Catálogo de prendas (trajes, americanas, vestidos, camisas, zapatos, etc.) con fotos, tallas y disponibilidad.
- Búsqueda y filtros por categoría, talla y tipo de prenda.
- Registro e inicio de sesión de usuarios.
- Reserva de una o varias prendas para una entrevista de trabajo.
- Gestión de préstamos y devoluciones, con fechas previstas.
- Historial de reservas y préstamos de cada usuario.
- Gestión del inventario: añadir, editar y retirar prendas donadas.
- Panel de administración para gestionar usuarios, reservas y préstamos.

La idea es desarrollar una aplicación **sencilla pero funcional**, con **dos roles** (usuarios y administradores), que permita trabajar con C#, ASP.NET Core MVC, Razor y Entity Framework Core.

### Qué viene hecho y qué falta

Ya viene en el esqueleto: registro e inicio de sesión, los roles `User` y `Admin`, la administración de usuarios y la subida de imágenes (`ImageStorage`, reutilizable para las fotos de las prendas). Todo el dominio del ropero (catálogo de prendas, reservas, préstamos y devoluciones, inventario y panel de administración) está por construir.

## Qué incluye el esqueleto

Punto de partida común de todos los grupos: **solo la entidad `ApplicationUser` (usuario)** y la infraestructura para que todo arranque a la primera. Las entidades del dominio, sus relaciones, servicios, controladores y vistas las construye el equipo.

| Área | Contenido |
| --- | --- |
| Usuarios | `ApplicationUser` (ASP.NET Core Identity) con nombre visible, avatar, fecha de alta y estado activo; roles `Admin` y `User`. |
| Cuenta | Registro, login por usuario o email, logout y página de acceso denegado. |
| Perfil | Consulta y edición del perfil, avatar y cambio de contraseña. |
| Administración | Panel `/Users` (solo `Admin`): alta, edición, activación y borrado de usuarios. |
| Datos | Entity Framework Core + SQLite (`App_Data/g1.db`), base recreada y datos demo al arrancar. |
| Imágenes | `ImageStorage` para guardar imágenes subidas; hoy solo avatares (añadid vuestras carpetas en `ImageFolder`). |
| Interfaz | Razor + Bootstrap 5 en local, con tema claro/oscuro. |
| Calidad y despliegue | Proyecto de tests xUnit, GitHub Actions (compilar y testear) y `Dockerfile`. |

## Requisitos

- SDK de .NET 10. `global.json` acepta cualquier SDK de la versión 10.0.
- Visual Studio Code y C# Dev Kit. Es el IDE estándar del curso en Windows, macOS y
  Linux.
- Git. Docker es opcional y solo se usa al explicar despliegue.

No instales ASP.NET, C#, SQLite ni Entity Framework por separado: los aporta el SDK o
se restauran como dependencias del proyecto.

## Instalación inicial

Instala .NET 10 SDK, Visual Studio Code y C# Dev Kit una sola vez en el ordenador.

### Windows (PowerShell)

```powershell
winget install --id Microsoft.DotNet.SDK.10 --exact
winget install --id Microsoft.VisualStudioCode --exact
winget install --id Git.Git --exact
```

Cierra y abre una terminal nueva; después instala la extensión:

```powershell
code --install-extension ms-dotnettools.csdevkit
```

### macOS (Terminal, con Homebrew)

```bash
brew install dotnet
brew install --cask visual-studio-code
brew install git
code --install-extension ms-dotnettools.csdevkit
```

Si no utilizas Homebrew, instala el SDK de .NET 10 con el instalador oficial de
[macOS](https://dotnet.microsoft.com/download/dotnet/10.0) y añade C# Dev Kit desde
el panel Extensions de VS Code.

### Ubuntu 26.04 (Terminal)

```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0 git
sudo snap install code --classic
code --install-extension ms-dotnettools.csdevkit
```

Para otra distribución Linux, sigue el instalador oficial de
[.NET para Linux](https://learn.microsoft.com/dotnet/core/install/linux) y, una vez
instalado VS Code, ejecuta el último comando.

Comprueba la instalación desde esta carpeta:

```bash
dotnet --version
```

Debe mostrar una versión que empiece por `10.0`.
Si el comando `code` no se reconoce, abre VS Code e instala C# Dev Kit desde
**Extensions**.

## Primer arranque

Clona el repositorio y abre **su carpeta** (no una carpeta superior) en VS Code. C# Dev Kit carga automáticamente `G1AspNet.sln`.

```bash
git clone https://github.com/certidevs/g1_aspnet.git
cd g1_aspnet
code .
dotnet restore G1AspNet.sln
dotnet tool restore
dotnet run --project G1AspNet.csproj --launch-profile http
```

Abre `http://localhost:5107`. El primer arranque crea `App_Data/g1.db`, crea las tablas y añade los usuarios demo:

- `admin` / `Admin123!`
- `user` / `User123!`

## Entidades y quién las hace

Las entidades del dominio ya están creadas y cada una tiene su carpeta de vistas, su controlador con `Index` y `Details`, y un enlace en el menú. Lo que falta por entidad (`Create`, `Edit`, `Save` y `Delete` con su vista `Form`) es el trabajo de las issues de la tabla, marcado con un `TODO` en cada controlador.

| Entidad | Controlador | Vistas |
| --- | --- | --- |
| `Category` · issue #5 · jmachin-learning | `CategoriesController` · issue #11 · cgaq100 | `Views/Categories` · issue #12 · cgaq100 |
| `Location` · issue #6 · cgaq100 | `LocationController` · issue #13 · cgaq100 | `Views/Location` · issue #14 · cgaq100 |
| `ClothingItem` · issue #1 · jmachin-learning | `ClothingItemsController` · issue #15 · jmachin-learning | `Views/ClothingItems` · issue #16 · jmachin-learning |
| `Reservation` · issue #7 · Jowey92 | `ReservationsController` · issue #17 · Jowey92 | `Views/Reservations` · issue #18 · Jowey92 |
| `Donation` · issue #8 · gonzaloriveroantonio-droid | `DonationController` · issue #19 · gonzaloriveroantonio-droid | `Views/Donation` · issue #20 · gonzaloriveroantonio-droid |

**Ejemplo completo:** `Location` ya tiene crear, editar y desactivar funcionando (`LocationController` y `Views/Location`): copiad su código. En el resto de entidades, `Create`, `Edit`, `Save`, `Delete`, `Form.cshtml` y los botones de `Index` y `Details` están escritos pero comentados con el marcador `DESCOMENTA`: descomentadlos, ajustadlos y completadlos.

## Trabajo en equipo en GitHub

Todo el grupo tiene permiso de escritura en este repositorio y **se trabaja directamente en `main`**. Para no pisaros el trabajo:

1. **Siempre `git pull` antes de `git push`** (y también antes de empezar a trabajar):

   ```bash
   git pull
   # ... trabajáis, probáis y hacéis commit ...
   git pull
   git push
   ```

2. **Un commit pequeño por cada cambio**, con mensaje claro (`feat: ...`, `fix: ...`, `docs: ...`).
3. **No toquéis la entidad, el controlador o las vistas de otra persona sin avisarle antes.** Mirad en la tabla de arriba quién se encarga de cada cosa.
4. **No hagáis `git push --force`.**
5. Quien ya sepa usar ramas puede usarlas; no es obligatorio.
6. Repartid el trabajo con **Issues** (una persona por issue) y, si queréis, un tablero de **Projects**.

### Base de datos en desarrollo

Este proyecto **no usa migraciones**. Al arrancar, la aplicación borra `App_Data/g1.db`, la vuelve a crear a partir de las entidades y carga los datos demo de `Data/DbInitializer.cs` (es el equivalente a `create-drop` con H2 en Spring Boot). Por eso:

- Si cambias una entidad o una relación, **solo tienes que volver a ejecutar la aplicación**.
- Si haces `git pull` con cambios de otras personas, **solo tienes que volver a ejecutarla**.
- No hay que ejecutar `dotnet ef` ni borrar la base a mano.
- Los datos que crees a mano al probar se pierden al reiniciar. Lo que deba existir siempre se añade a `DbInitializer`.

## Comandos de trabajo diario

Todos se ejecutan desde la raíz de este repositorio.

```bash
# Restaurar dependencias NuGet y herramientas locales tras clonar o actualizar
dotnet restore G1AspNet.sln
dotnet tool restore

# Ejecutar la aplicación
dotnet run --project G1AspNet.csproj --launch-profile http

# Ejecutar con recarga al guardar archivos
dotnet watch --project G1AspNet.csproj run --launch-profile http

# Compilar y ejecutar los tests
dotnet build G1AspNet.sln
dotnet test G1AspNet.sln
```

### Entity Framework y SQLite

La base de datos se recrea al arrancar (ver *Base de datos en desarrollo*): no hay migraciones que crear ni aplicar.

### CI y Docker

[GitHub Actions](.github/workflows/build-and-test.yml) ejecuta restauración, compilación Release y tests en cada `push` a `main` y en cada *pull request* hacia `main`.

Docker es opcional:

```bash
docker build -t g1_aspnet:local .
docker run --rm -p 10000:10000 g1_aspnet:local
```

El contenedor queda disponible en `http://localhost:10000`.

## IDEs

En VS Code, selecciona el perfil `http` y pulsa F5 para depurar. Las tareas `G1: compilar` y `G1: ejecutar tests` están disponibles en **Terminal → Run Task**. En Windows, Visual Studio 2026 también abre `G1AspNet.sln`; consulta [la guía de IDEs](docs/IDE-SETUP.md).
