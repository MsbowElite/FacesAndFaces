# FacesAndFaces
Facial recognition

docker pull mcr.microsoft.com/mssql/server:2019-latest

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Insecure!12345" `
   -p 1433:1433 --name sql1 -h sql1 `
   -d mcr.microsoft.com/mssql/server:2019-latest

Scaffold-DbContext "Server=localhost;Database=FACEORDER;User Id=sa;password=Insecure!12345;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Persistence\Entities -ContextDir Persistence -Context OrdersDbContext -Force
