
[![Build and Test](https://github.com/davidmarco12/challenge-twitter/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/davidmarco12/challenge-twitter/actions/workflows/ci-cd.yml)

[![imagen](https://github.com/user-attachments/assets/c93c011c-8f64-4676-9d6b-b377380c13e4)](https://challenge-twitter-production.up.railway.app/swagger/index.html)


# TwitterAPI

API REST desarrollada en .NET 8.

## 🚀 Tecnologías

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para base de datos
- **PostgreSQL** - Base de datos
- **Docker & Docker Compose** - Containerización
- **Swagger/OpenAPI** - Documentación de API
- **Railway** - Hosting
- **Github Actions** - CI/CD


## 📋 Prerrequisitos

Antes de comenzar, asegúrate de tener instalado:

- [Docker](https://docs.docker.com/get-docker/) (versión 20.10 o superior)
- [Docker Compose](https://docs.docker.com/compose/install/) (versión 2.0 o superior)
- [Git](https://git-scm.com/downloads)

## 🛠️ Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/davidmarco12/challenge-twitter.git
cd challenge-twitter
```

### 2. Configurar variables de entorno (Opcional)

El proyecto viene con configuración por defecto. Si deseas personalizar:

```bash
# Crear archivo en src/WebAPI/ appsetting.Developer.json (opcional)
```

Variables disponibles:
``` "ConnectionStrings": {
  "DefaultConnection": "Host=database.server;Port=5432;Database=TwitterAPI;Username=postgres;Password=YourStrong@Passw0rd;Trust Server Certificate=true"
}
```

### 3. Levantar el proyecto

```bash
# Construir y levantar todos los servicios
docker-compose up --build

# O en segundo plano
docker-compose up --build -d
```

### 4. Verificar que esté funcionando

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
  - username: `postgres`
  - Password: `YourStrong@Passw0rd`
- **Adminer**: http://localhost:8080/

## 🗄️ Servicios Incluidos

| Servicio | Puerto | Descripción |
|----------|--------|-------------|
| **twitterapi-app** | 5000 | API principal de TwitterAPI |
| **database.server** | 5432 | Base de datos PostgreSQL |

Tambien se incluyo un adminer papra la visualizacion de las tablas.

Hay un scripts en -> src/scripts/init-database.sql para popular las tablas.


## 📚 Uso de la API

### Swagger

Una vez que la aplicación esté corriendo, puedes acceder a:

- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health


