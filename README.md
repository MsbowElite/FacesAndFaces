# FacesAndFaces
Facial recognition

# Plans

Add UnitOfWork
Add Mappers
Add FluentValidations 3 layers
Apply DDD
Apply CQRS
Apply EventSourcing
Refactory to clean code
Create DOC and swagger with versioning
Organize AppServicies and Servicies
Encapsulate Successful and unsuccessful responses

docker pull mcr.microsoft.com/mssql/server:2019-latest

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Insecure!12345" `
   -p 1433:1433 --name sql1 -h sql1 `
   -d mcr.microsoft.com/mssql/server:2019-latest

Scaffold-DbContext "Server=localhost;Database=FACEORDER;User Id=sa;password=Insecure!12345;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Persistence\Entities -ContextDir Persistence -Context OrdersDbContext -Force
