This Roulette project uses includes: ASP.NET Core 2.2 API, ASP.NET Core 2.2 Web Application and some references for Entities, Db configurations, 

Technologies:
SDK - Microsoft.NETCore, ASP.NET Core 2.2, EnfityFramework.Core, Swagger - NSwag, FluentValidation, Json, Razor Pages, Javascript, jQuery, HTML, CSS.

1) Clone github project to your local machine.
2) Run it with Visual Studio .
3) Create local db with name "Roulette" by SQL Server Object Explorer "(localdb)\MSSQLLocalDB".
		ConnectionString: "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Roulette;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

4) Run command: update-database with EntityFramework to apply all migrations.
5) Build solution with two projects "Roulette.Api" and "Roulette.Web".
6) In web application register user and play roulette.

P.S.
Sorry for the delay, it's my fault, i could not find enough time to finish the client side application correctly, but in my opinion the main idea is clear.

Respectfully

David Chkhitunidze