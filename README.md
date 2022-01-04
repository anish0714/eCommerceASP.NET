INSTALL DOT NET VERSION 5.0.12
dotnet tool install --global dotnet-ef --version 5.0.12

In appsettings.json file, change DefaultConnection value as mentioned below
Replace "DESKTOP-C1B8BGT" to your local database connection name


ADD MIGRATIONS TO CONNECT DATABASE:
dotnet ef migrations add InitialCreate --project ShoppingKart
dotnet ef database update --project ShoppingKart