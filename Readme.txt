Roulette project includes: ASP.NET Core 2.2 API, ASP.NET Core 2.2 Web Application, some references for Entities, Db configurations, Dtos and related logics for API.

Technologies:
SDK - Microsoft.NETCore, ASP.NET Core 2.2, EnfityFramework.Core, Swagger - NSwag, FluentValidation, Json, Razor Pages, Javascript, jQuery, HTML, CSS.

1) Clone github project to your local machine.
2) Run it with Visual Studio .
3) Create local db with name "Roulette" by SQL Server Object Explorer "(localdb)\MSSQLLocalDB".
		ConnectionString: "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Roulette;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

4) Run command: update-database with EntityFramework to apply all migrations.
5) Build solution with two projects "Roulette.Api" and "Roulette.Web".
6) In web application register user and play roulette.

Respectfully

David Chkhitunidze
