# MásPelículas API - Backend de Gestión de Películas

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10.0"/>
  <img src="https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core Web API"/>
  <img src="https://img.shields.io/badge/Entity%20Framework-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core"/>
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server"/>
  <img src="https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C# 12"/>
  <img src="https://img.shields.io/badge/JWT-Bearer-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT"/>
  <img src="https://img.shields.io/badge/AutoMapper-12.0-BE161D?style=for-the-badge" alt="AutoMapper"/>
  <img src="https://img.shields.io/badge/MSTest-Tests-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white" alt="MSTest"/>
</p>

---

## Descripción del Proyecto

**MásPelículas API** es una API RESTful desarrollada en **ASP.NET Core** para la gestión completa de un catálogo de películas. El proyecto implementa todas las operaciones CRUD para películas, géneros, actores y salas de cine, junto con un sistema de autenticación basado en JWT y funcionalidades avanzadas como geolocalización, filtrado dinámico y sistema de reviews.

Este proyecto forma parte del aprendizaje de desarrollo de APIs profesionales con .NET, implementando patrones y prácticas modernas de desarrollo backend.

---

## Características Principales

### Gestión de Películas
- **CRUD completo** de películas
- **Filtrado avanzado** por título, género, estado (en cines, próximos estrenos)
- **Paginación** con headers de respuesta
- **Ordenamiento dinámico** por cualquier campo
- **Subida de posters** con almacenamiento local
- **Relaciones** con géneros, actores y salas de cine
- **JsonPatch** para actualizaciones parciales

### Gestión de Actores
- **CRUD completo** con paginación
- **Subida de fotos** con validación de tipo y tamaño
- **Personajes** en películas con orden de aparición
- **JsonPatch** para actualizaciones parciales

### Gestión de Géneros
- **CRUD completo** de géneros cinematográficos
- Asociación múltiple con películas

### Salas de Cine
- **CRUD completo** de salas de cine
- **Geolocalización** con NetTopologySuite
- **Búsqueda por proximidad** (salas cercanas en un radio)
- Coordenadas de ubicación (latitud/longitud)

### Sistema de Reviews
- **Crear reviews** de películas (usuarios autenticados)
- **Puntuación** de 1 a 5 estrellas
- **Comentarios** de usuarios
- **Protección de autoría** (solo el dueño puede editar/eliminar)
- **Una review por usuario** por película

### Autenticación y Autorización
- **JWT Bearer** para autenticación
- **Registro** de usuarios
- **Login** con generación de token
- **Roles** (Admin, Usuario)
- **Claims** para autorización granular
- Tokens con expiración de 1 año

### Almacenamiento de Archivos
- **Almacenamiento local** en wwwroot
- **Validación** de tipo de archivo (imágenes)
- **Validación** de peso máximo (4-5 MB)
- **URLs públicas** para acceso a archivos

---

## Tecnologías Utilizadas

### Backend
| Tecnología | Versión | Descripción |
|------------|---------|-------------|
| **.NET** | 10.0 | Framework de desarrollo |
| **ASP.NET Core Web API** | 10.0 | Framework para APIs REST |
| **Entity Framework Core** | 10.0.3 | ORM para acceso a datos |
| **ASP.NET Core Identity** | 10.0.3 | Sistema de usuarios |
| **C#** | 12.0 | Lenguaje de programación |

### Base de Datos
| Tecnología | Descripción |
|------------|-------------|
| **SQL Server** | SGBD relacional |
| **NetTopologySuite** | Tipos espaciales/geográficos |
| **EF Core Migrations** | Gestión de esquema |

### Paquetes NuGet
```
AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
Microsoft.AspNetCore.Authentication.JwtBearer (10.0.3)
Microsoft.AspNetCore.Identity.EntityFrameworkCore (10.0.3)
Microsoft.AspNetCore.Mvc.NewtonsoftJson (10.0.3)
Microsoft.EntityFrameworkCore.SqlServer (10.0.3)
Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite (10.0.3)
System.Linq.Dynamic.Core (1.7.1)
DotNetEnv (3.1.1)
```

### Testing
| Tecnología | Versión | Descripción |
|------------|---------|-------------|
| **MSTest** | 4.1.0 | Framework de testing |
| **Microsoft.AspNetCore.Mvc.Testing** | 10.0.3 | Testing de integración |
| **Microsoft.EntityFrameworkCore.InMemory** | 10.0.3 | BD en memoria para tests |
| **Moq** | 4.20.72 | Mocking framework |

---

## Arquitectura del Proyecto

```
MásPelículasAPI/
│
├── 📂 Controllers/
│   ├── ActoresController.cs       # CRUD de actores + fotos
│   ├── CuentasController.cs       # Auth: registro, login, roles
│   ├── CustomBaseController.cs    # Controller base genérico
│   ├── GenerosController.cs       # CRUD de géneros
│   ├── PeliculasController.cs     # CRUD + filtros de películas
│   ├── ReviewController.cs        # Reviews de películas
│   └── SalasDeCineController.cs   # CRUD + geolocalización
│
├── 📂 DTOs/
│   ├── Actor*.cs                  # DTOs de actores
│   ├── Genero*.cs                 # DTOs de géneros
│   ├── Pelicula*.cs               # DTOs de películas
│   ├── Review*.cs                 # DTOs de reviews
│   ├── SalaDeCine*.cs             # DTOs de salas
│   ├── PaginacionDTO.cs           # Paginación
│   └── UserInfo.cs                # Auth DTOs
│
├── 📂 Entidades/
│   ├── Actor.cs
│   ├── Genero.cs
│   ├── Pelicula.cs
│   ├── PeliculasActores.cs        # Relación M:N
│   ├── PeliculasGeneros.cs        # Relación M:N
│   ├── PeliculasSalasDeCine.cs    # Relación M:N
│   ├── Review.cs
│   ├── SalaDeCine.cs
│   └── IId.cs                     # Interfaz común
│
├── 📂 Helpers/
│   ├── AutoMapperProfiles.cs      # Configuración de mapeos
│   ├── HttpContextExtensions.cs   # Extensiones para paginación
│   ├── QueryableExtensions.cs     # Extensiones para IQueryable
│   ├── TypeBinder.cs              # Model binder para JSON
│   └── PeliculaExisteAttribute.cs # Filtro de validación
│
├── 📂 Servicios/
│   ├── IAlmacenadorArchivos.cs    # Interfaz de archivos
│   └── AlmacenadorArchivosLocal.cs # Implementación local
│
├── 📂 Validaciones/
│   ├── PesoArchivoValidacion.cs   # Validación de tamaño
│   └── TipoArchivoValidacion.cs   # Validación de tipo
│
├── 📂 Migrations/
│
├── ApplicationDbContext.cs         # DbContext + Seed data
├── Startup.cs                      # Configuración de servicios
├── Program.cs                      # Punto de entrada
├── appsettings.json
└── .env.example
│
MasPelículas.Tests/
│
├── 📂 PruebasUnitarias/
│   ├── ActoresControllerTests.cs
│   ├── CuentasControllerTests.cs
│   ├── GeneroControllerTests.cs
│   ├── PeliculasControllerTests.cs
│   ├── ReviewControllerTests.cs
│   └── SalasDeCineControllerTests.cs
│
├── 📂 PruebasDeIntegración/
│   ├── GenerosControllerTests.cs
│   └── ReviewControllerTests.cs
│
├── BasePruebas.cs                  # Clase base para tests
├── UsuarioFalsoFiltro.cs           # Filtro de usuario mock
└── AllowAnonymousHandler.cs        # Handler para bypass auth
```

---

## 🔌 Endpoints de la API

### Películas
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/peliculas` | Índice (en cines + próximos estrenos) |
| `GET` | `/api/peliculas/filtro` | Filtrar con paginación |
| `GET` | `/api/peliculas/{id}` | Obtener por ID (con detalles) |
| `POST` | `/api/peliculas` | Crear película |
| `PUT` | `/api/peliculas/{id}` | Actualizar película |
| `PATCH` | `/api/peliculas/{id}` | Actualización parcial |
| `DELETE` | `/api/peliculas/{id}` | Eliminar película |

### Actores
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/actores` | Listar con paginación |
| `GET` | `/api/actores/{id}` | Obtener por ID |
| `POST` | `/api/actores` | Crear actor (con foto) |
| `PUT` | `/api/actores/{id}` | Actualizar actor |
| `PATCH` | `/api/actores/{id}` | Actualización parcial |
| `DELETE` | `/api/actores/{id}` | Eliminar actor |

### Géneros
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/generos` | Listar todos |
| `GET` | `/api/generos/{id}` | Obtener por ID |
| `POST` | `/api/generos` | Crear género |
| `PUT` | `/api/generos/{id}` | Actualizar género |
| `DELETE` | `/api/generos/{id}` | Eliminar género |

### Salas de Cine
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/salasdecine` | Listar todas |
| `GET` | `/api/salasdecine/{id}` | Obtener por ID |
| `GET` | `/api/salasdecine/cercanos` | Buscar cercanas |
| `POST` | `/api/salasdecine` | Crear sala |
| `PUT` | `/api/salasdecine/{id}` | Actualizar sala |
| `DELETE` | `/api/salasdecine/{id}` | Eliminar sala |

### Reviews
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/peliculas/{id}/reviews/{reviewId}` | Obtener review |
| `POST` | `/api/peliculas/{id}/reviews` | Crear review 🔐 |
| `PUT` | `/api/peliculas/{id}/reviews/{reviewId}` | Editar review 🔐 |
| `DELETE` | `/api/peliculas/{id}/reviews/{reviewId}` | Eliminar review 🔐 |

### Cuentas
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/cuentas/registro` | Registrar usuario |
| `POST` | `/api/cuentas/login` | Iniciar sesión |
| `GET` | `/api/cuentas/listado` | Listar usuarios 🔐 |
| `POST` | `/api/cuentas/asignar-rol` | Asignar rol 🔐👑 |
| `POST` | `/api/cuentas/remover-rol` | Remover rol 🔐👑 |

> 🔐 = Requiere autenticación | 👑 = Requiere rol Admin

---

## Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/MasPeliculasAPI.git
cd MasPeliculasAPI
```

### 2. Configurar Variables de Entorno

Crea un archivo `.env` basándote en `.env.example`:

```env
DB_SERVER=localhost
DB_NAME=MasPeliculasDB
JWT_SECRET=ClaveSecretaMuySeguraParaJWT_DebeSerLargaYCompleja
```

### 3. Restaurar Paquetes

```bash
dotnet restore
```

### 4. Aplicar Migraciones

```bash
cd MasPelículasAPI
dotnet ef database update
```

### 5. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- **HTTP:** `http://localhost:5210`
- **HTTPS:** `https://localhost:7261`

---

## Autenticación

### Registro de Usuario
```http
POST /api/cuentas/registro
Content-Type: application/json

{
  "email": "usuario@ejemplo.com",
  "password": "Password123!"
}
```

### Login
```http
POST /api/cuentas/login
Content-Type: application/json

{
  "email": "usuario@ejemplo.com",
  "password": "Password123!"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiracion": "2027-03-03T00:00:00Z"
}
```

### Usar el Token
```http
GET /api/cuentas/listado
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## Tests

El proyecto incluye pruebas unitarias y de integración:

```bash
cd MasPelículas.Test
dotnet test
```

### Cobertura de Tests
- Controllers de Actores
- Controllers de Géneros
- Controllers de Películas
- Controllers de Salas de Cine
- Controllers de Reviews
- Controllers de Cuentas
- Pruebas de integración con WebApplicationFactory

---

## Modelo de Datos

```
┌─────────────┐     ┌──────────────────┐     ┌─────────────┐
│   Generos   │────<│ PeliculasGeneros │>────│  Peliculas  │
└─────────────┘     └──────────────────┘     └──────┬──────┘
                                                    │
┌─────────────┐     ┌──────────────────┐            │
│   Actores   │────<│ PeliculasActores │>───────────┤
└─────────────┘     └──────────────────┘            │
                                                    │
┌─────────────┐     ┌────────────────────┐          │
│ SalasDeCine │────<│PeliculasSalasDeCine│>─────────┤
└─────────────┘     └────────────────────┘          │
                                                    │
┌─────────────┐                                     │
│   Reviews   │>────────────────────────────────────┘
└──────┬──────┘
       │
       v
┌─────────────┐
│AspNetUsers  │
└─────────────┘
```

---

## Usuario Admin por Defecto

El sistema crea un usuario administrador mediante seed data:

| Campo | Valor |
|-------|-------|
| **Email** | admin@maspeliculas.com |
| **Password** | Aa123456! |
| **Rol** | Admin |

---

## Autor

**Mateo Pavoni**
- 📧 Email: mateopavoni905@gmail.com
- 🐙 GitHub: [@mateopavoni](https://github.com/mateopavoni)
