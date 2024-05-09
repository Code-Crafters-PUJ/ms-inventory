# ms-inventory
Para correr el microservicio por primera vez:
1. Tener descargado .NET Core SDK
2. Instalar las dependencias necesarias, estas son:
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools
3. Hacer el docker compose: docker-compose up -d
4. Para agregar la base de datos: dotnet ef database update
5. Para correr el microservicio y realizar las peticiones: dotnet run Program.cs

Para correr el programa: dotnet run Program.cs
