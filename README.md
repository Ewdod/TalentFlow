# TalenFlow
A Sample N-layered .NET Core Project Demonstrating Clean Architecture and the Generic Repository Pattern.

## Packages

### Infrastructure
```
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

## Migrations
Before running the following commands, make sure that Web is set as startup project. Run the follwoing commands on the project "Infrastructure".

### Infrastructure
```
Add-Migration InitialCreate -Context TalentFlowDbContext -OutputDir Data/Migrations
Update-Database -Context TalentFlowDbContext

Add-Migration InitialIdentity -Context ApplicationDbContext -OutputDir Identity/Migrations
Update-Database -Context ApplicationDbContext
```
