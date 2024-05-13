# ms-inventory
Para correr el microservicio por primera vez:
1. Descargar .NET Core SDK
2. Instalar las dependencias necesarias:
```
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

```
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

```
dotnet add package RabbitMQ.Client
```

```
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
```

3. Generar la base de datos y su réplica.
```
docker-compose up -d
```

4. Agregar la base de datos.
```
dotnet ef database update
```

```
dotnet add package DotNetEnv
```

5. Ejecutar el microservicio.
```
dotnet run Program.cs
```
