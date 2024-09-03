## Requisitos Previos

Para ejecutar este proyecto, necesitarás tener instalados los siguientes programas y herramientas:

- **.NET SDK [8]**: [Descargar .NET SDK](https://dotnet.microsoft.com/download)
- **Visual Studio o Visual Studio Code**: Para editar y ejecutar el código.
- **PostgreSQL**: Como base de datos. [Descargar PostgreSQL](https://www.postgresql.org/download/)
- **Git**: Para clonar el repositorio. [Descargar Git](https://git-scm.com/downloads)

## Configuración del Proyecto

Sigue estos pasos para configurar y ejecutar el proyecto localmente.

### 1. Clonar el Repositorio

Primero, clona el repositorio en tu máquina local.
Una vez que hayas clonado el repositorio, navega al directorio del proyecto y restaura las dependencias necesarias utilizando .NET:

dotnet restore

### 2. Configuración base de datos
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=todoappdb;Username=postgres;Password=postgres;"
}
Colocar su Database, su nombre de usuario y password.

### 3. Migración de datos
dotnet ef database update

### 4. Correr proyecto
dotnet run

### 5. Probar Websocket
npm install -g wscat

corre el servidor de Wesocket:
wscat -c ws://localhost:<puerto>/ws

wscat -c ws://localhost:<puerto>/ws


