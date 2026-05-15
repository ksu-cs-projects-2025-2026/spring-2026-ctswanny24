Recip-EZ README / Setup Guide
=============================

Recip-EZ is a full-stack recipe and inventory web application.

The project uses:

- ASP.NET Core / .NET 8 for the backend
- React + Vite for the frontend
- SQL Server LocalDB for the default development database
- Optional Docker SQL Server support

Every time the Server part is started, the database is purged, recreated, and reseeded with the data in Data/Dataset_CSV
If you want to see a seeded inventory, use the login 
Username: Ctswanny24@gmail.com
Password: 1234z

If you want to test the register feature (newer and a little more finicky)
- Insert names into the slots for them
- Insert your email
- Insert your password and confirm it
- Click log-in.

Due to the fact that the application is purged and seeded on startup, a new registered account isn't tracked if the server is stopped and run again.
I intended to make sure that each time the project was started and stopped, it could be tested and controlled. 
in the future, this could be changed, and new things could be added to the CSV files before closing.

There is a bug in the ingredient search that doesn't fully narrow down an item until a typo is made, then the ingredients are more filtered. 


1. Required Software
--------------------

This project is easiest to run on Windows with Visual Studio 2022 and SQL Server LocalDB.

Required:

- Git
- .NET 8 SDK
- Node.js LTS
- npm, included with Node.js
- SQL Server Express LocalDB

Recommended:

- Visual Studio 2022 Community

2. Install Required Software from PowerShell
-------------------------------------------

Open PowerShell as a normal user and run these commands.

Install Visual Studio for LocalDb to work.

Install Git:

winget install Git.Git

Install .NET 8 SDK:

winget install Microsoft.DotNet.SDK.8

Install Node.js LTS:

winget install OpenJS.NodeJS.LTS

Install SQL Server Express:
https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver17

Using Command line and winget to get SQLServer Express is finicky. Try using this website.
winget install Microsoft.SQLServer.2022.SqlLocalDb


After installing software, close PowerShell and open a new PowerShell window.

If winget is not recognized, install "App Installer" from the Microsoft Store, then try again.


3. Verify Software Installation
-------------------------------

Run these commands:

git --version
dotnet --version
node --version
npm --version
sqllocaldb info

Expected results:

- git should print a version number.
- dotnet should print a version number.
- node should print a version number.
- npm should print a version number.
- sqllocaldb info should list available LocalDB instances.

If sqllocaldb info lists MSSQLLocalDB, LocalDB is ready.

4. Database Setup
-----------------

Important:

- The computer must have SQL Server LocalDB installed.
- The Recip-EZ_Database database does not need to already exist.
- The backend creates the database automatically when it starts.
- The backend also seeds starter data automatically.
- In development, the backend deletes and recreates the database each time it starts.

Because of this, do not point the app at a database containing data you want to keep.

To create the MSSQLLocalDB instance if it does not already exist:

sqllocaldb create MSSQLLocalDB

If the command says the instance already exists, that is okay.

Start LocalDB:

sqllocaldb start MSSQLLocalDB


5. Restore Backend Packages
---------------------------

From the repository root, run:

dotnet restore Recip-EZ.sln


6. Install Frontend Packages
----------------------------

From the repository root, run:

cd recip-ez.client
npm install
cd ..


7. Run the Project from the Terminal
------------------------------------

The easiest terminal setup is to run the backend and frontend in two separate PowerShell windows.

Terminal 1: start the backend with the HTTPS profile:

dotnet run --project Recip-EZ.Server --launch-profile https

Terminal 2: start the frontend:

cd recip-ez.client
npm run dev

Open the frontend URL shown by Vite, usually:

http://localhost:5173

Swagger may be available at:

https://localhost:7111/swagger


8. Run the Project from Visual Studio
-------------------------------------

Open the solution file:

Recip-EZ.sln

Make sure Recip-EZ.Server is selected as the startup project.

Run the project


9. Fresh Computer Quick Start
------------------------------

Run these commands on a fresh Windows computer.

Check versions of required software:
git --version
dotnet --version
node --version
npm --version
sqllocaldb info

If the version is behind or not recognized,
Install required software:

winget install Git.Git
winget install Microsoft.DotNet.SDK.8
winget install OpenJS.NodeJS.LTS

Do this from the link above. The winget is a little finicky
Might also require a full restart of the computer.
winget install Microsoft.SQLServer.2022.Express

Close and reopen PowerShell.

Clone the repository:

git clone <repository-url>
cd spring-2026-ctswanny24

Create and start LocalDB:

sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB

Restore backend packages:

dotnet restore Recip-EZ.sln

Install frontend packages:

cd recip-ez.client
npm install
cd ..

Start the backend:

dotnet run --project Recip-EZ.Server --launch-profile https

Open the link given in the client terminal that opens up after the server starts.

The application should create and seed the development database automatically.
