The purpose of this project is to learn and combine technologies:
* GraphQL (GQL) - to have one and only one endpoint that expose advanced services,
* Entity Framework Core 8 (EF) - to code less SQL and make it easier to handle database migrations,
* MediatR - to decouple GQL resolvers call from application behaviour,
* AutoMapper - to make it easier to map betweeen dto's and dao's crossing layers,   
* Swagger - for GQL and a health endpoint,
* Postgres SQL database - to use a SQL database for free with npgsql as ODBC and EF Core driver,
* Angular - to learn how to integrate a GQL backend,
* Python - to learn how a python script intefaces to a GQL backend,
* FluentMigrator - to be able to version control the database, when functions or stored procedures are inevitable,
* Dimensional modelling - to learn and test how EF handle the model,
* Clean code architecture - to learn it,
* Docker with postgress database - to learn how to handle containerized database and make it easier to unit test with a database.

The domain is stocks and time series for day prizes from Nasdaq Nordic, where the application stores stock's day prices as timeseries for as long back data is available. In case I miss data then I can code calculated timeseries like moving average, Bollinger bands etc.

I would like to see how the stack performs with GQL and EF, learn how to build GQL resolvers and code effective EF linq.
The questions for EF are:
* Can it search and read fast with advanced filters?
* Can it store a huge timeseries fast?
* What about bulk insert?
* How is it to program unit test with EF and the database?

**Status**

I've programmed:
* The backend with GQL that exposes query and mutation of stocks, exchanges, currencies and only add timeseries of day prizes.
* A python script that gets timeseries of day prizes from Nasdag Nordic and stores the timeseries using GQL,
* EF handling of stocks, currencies, exchanges and timeseries,
* A simple Angular project that can show the stocks,
* An advanced filter search for the stock queury with a PLSQL function called from EF,
* Compose.yaml file for a ubuntu/postgres image based container.

I've launched the application in debug in vs2022 and the python script in debug in vsc. It works!

My experience so far is:
* GraphQL - works very well as a presenter in the clean code architecture,
* MediatR - works out of the box,
* AutoMapper - works well, when unit testing is set up of all mappings and configurations,
* Entity Framework Core 8 (EF) - gives me issues see below,
* Swagger - works out of the box,
* Postgres SQL database - Postgres and npgsql works with out problems,
* Angular - I did it and the page works. It's not easy as backend developer,
* Python - it was fast and easy to code towards GQL,
* FluentMigrator - I've had to code the apply migrations. It works with embedded plpgsql scripts,
* Dimensional modelling - I've problems with it and EF,
* Clean code architecture - no comment,
* Docker Postgres database - works fine with a steep learning curve.

EF have given me some issues
1) With docker is unit testing solveed. The tests don't have to be set up for particular situations. There is no longer code for seeding the database, so the it's clear and small. Otherwise unit testing with EF became long, unmainaintable and complex, because the unit test arrangement often became very long and cumbersome. The acting were typical short. And the following assertions of results are hard to understand. The database had to be seeded with a lot of data in arrangement section of the test for a particular situation. The same EF db context may not be used in arrangement for the acting, because EF caches the seed. A Docker image helps, because you can seed and prepare all situations in the database, so one can focus on testing.
2) I'm used to write SQL functions or stored procedures that do the handling of data and relations. With EF you have to program the handling of existing related entries to a new entry for insertion. This implies to set the foreign keys and clear navigational properties in the entity inserted, otherwise related entries are inserted as duplicates.
3) I've to think carefully which changes go together before calling EF SaveChanges(). With a SQL stored procedure I handed over these decision to the function or procedure.
4) EF version 8 don't have any bulk insertion. Functions and store procedure do.

**How to set up the programming environment**

You have to have:
* Cloned my Elis repository.
* An IDE like Visual Studio, VS, or Visual Studio Code, VSC.
* Docker and docker-compose to run the postgres database image in a container. I use Docker Desktop that includes docker-compose,
* PgAdmin to inspect and look data up in database tables (note you may choose to have PgAdmin running in the container, if you know how),
* Optional how ever it's very convenient with an text editor like Notepad++.

The procedure is:
1. Get and install IDE, Docker, PgAdmin and text-editor applications missing.
2. Start up Docker and get the image ubuntu/postgres, https://hub.docker.com/_/postgres/.
3. Edit the compose.yaml file in ./Elis/Docker to set up the password and persistent folder for database files in the docker container.
4. In a terminal like Windows Powershell you spin up the container with the command _docker-compose up --detach_.
5. Start PgAdmin and inspect the database is a live and clean.
6. Run the EF-Core migrations. In VS you issue the command _Update-Database_ in the _Package Manager Console_, and while you've selected _ElisBackend_ as the active project including in the field Default project of the console.
7. Inspect the database _Elis_ with PgAdmin. You should have and Elis database with tables.
8. In order to run _ApplyMigrations_ that applies the FluentMigrator migrations you need a connectionstring like _"Host=localhost;Port=5432;Username=postgres;Password=secret;Database=Elis" for the database. You find the connectionstring in the appsettings.json file for Elisbackend. Set the correct password, in case you have changed it in step 3.
9. Run ApplyMigrations with arguments: _<connectionsstring> up_, where _up_ means migrate up. In VS you can set the arguments under _Properties_ | _Debug_ | _Open debug launch profiles UI_, and run ApplyMigrations in debug mode with VS.  
11. Inspect in PgAdmin that _Stocks_, _TimeSeries_ and _TimeSeriesFacts_ tables have data.
13. Build the backend and unit tests projects.
14. Run unit tests

**TODOs**
* Program GQL subscriptions for time series,
* Investigate GQL filtering and possibilities,
* Insert tons of timeseries and facts to measure performance,
* Organize docker different database volumes for unit testing and production.
* Add security and roles, so user has to log on. Superusers only can administrate the application.
