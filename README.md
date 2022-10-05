# fh-Identity-Server-Host

API for adding, editing and deleting Local Authority, Voluntary, Charitable and Faith Organisations, their Users and their Roles.

API Login:
https://localhost:7108/api/Authenticate/login


{
  "username": "your username",
  "password": "your password"
}


Migrations Commands

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add CreateIdentitySchema -c ApplicationDbContext --output-dir C:\Research\fh-Identity-Server-Host\src\FamilyHub.IdentityServerHost\Persistence\Data\CreateIdentitySchema
dotnet ef migrations add Organisations -c ApplicationDbContext --output-dir C:\Research\fh-Identity-Server-Host\src\FamilyHub.IdentityServerHost\Persistence\Data\AddOrganisation

dotnet ef database update -c ApplicationDbContext
