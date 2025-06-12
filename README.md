# TwitterAPI

API REST desarrollada en .NET 8.

## 🚀 Tecnologías

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para base de datos
- **PostgreSQL** - Base de datos
- **Docker & Docker Compose** - Containerización
- **Swagger/OpenAPI** - Documentación de API

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
  "DefaultConnection": "Host=localhost;Port=5432;Database=TwitterAPI;Username=postgres;Password=YourStrong@Passw0rd;Trust Server Certificate=true"
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
- **pgAdmin** (admin DB): http://localhost:8080
  - username: `postgres`
  - Password: `YourStrong@Passw0rd`

## 🗄️ Servicios Incluidos

| Servicio | Puerto | Descripción |
|----------|--------|-------------|
| **twitterapi-app** | 5000 | API principal de TwitterAPI |
| **database.server** | 5432 | Base de datos PostgreSQL |
| **pgAdmin** | 8080 | Interfaz web para administrar PostgreSQL |

## 📝 Comandos Útiles

### Gestión de contenedores

```bash
# Ver logs de todos los servicios
docker-compose logs -f

# Ver logs solo de la API
docker-compose logs -f twitterapi-app

# Ver logs solo de la base de datos
docker-compose logs -f database.server

# Parar todos los servicios
docker-compose down

# Parar y eliminar volúmenes (⚠️ elimina datos de DB)
docker-compose down -v

# Reconstruir solo un servicio
docker-compose up --build twitterapi-app
```

### Desarrollo

```bash
# Reiniciar solo la aplicación (útil durante desarrollo)
docker-compose restart twitterapi-app

# Ver el estado de los servicios
docker-compose ps

# Acceder al contenedor de la aplicación
docker exec -it twitterapi-app bash
```

## 📚 Uso de la API

### Endpoints principales

Una vez que la aplicación esté corriendo, puedes acceder a:

- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

### Ejemplos con curl

```bash
# Health check
curl http://localhost:5000/health


### Desarrollo local
```bash
docker-compose up --build
```
