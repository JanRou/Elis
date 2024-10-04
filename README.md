The purpose of this project is to learn and combine technologies:
   * GraphQL (GQL) - to have one and only one endpoint that expose advanced services,
   * Entity Framework Core 8 (EF) - to code less SQL and make it easier to handle database migrations,
   * MediatR - to decouple GQL resolvers call from application behaviour,
   * AutoMapper - to make it easier to map betweeen dto's and dao's crossing layers,   
   * Swagger - for CQL and a health endpoint,
   * Postgres SQL database - to use a SQL database for free with npgsql as ODBC and EF Core driver,
   * Angular - to learn how to integrate a GQL backend,
   * Python - to learn how a python script can inteface to a GQL backend,
   * FluentMigrator - to be able to version control the database, when functions or stored procedures are inevitable,
   * Dimensional modelling - to learn and test how EF handle the model,
   * Clean code architecture - to learn it,
   * Optional: Docker for the database.

The domain is stocks and time series for day prizes from Nasdaq Nordic, where the application stores stock's day prices as timeseries for as long back data is available. In case I miss data then I can code calculated timeseries like moving average, Bollinger bands etc.

I would like to see how the stack performs with GQL and EF, learn how to build CQL resolvers and code effective EF linq.
The questions for EF are:
* Can it search and read fast with advanced filters?
*  Can it store a huge timeseries fast?
*  What about bulk insert?
*  How is it to program unit test with EF and the database?
  
Status is that I've programmed:
  * The backend with CQL that exposes query and mutation of stocks, exchanges, currencies and only add timeseries of day prizes.
  * A python script that gets timeseries of day prizes from Nasdag Nordic and stores the timeseries using CQL,
  * EF handling of stocks, currencies, exchanges and timeseries,
  * A simple Angular project that can show the stocks,
  * An advanced filter search for the stock queury with a PLSQL function called from EF.

Experience so far is that:
   * GraphQL (GQL) - CQL works very good as a presenter in the architecture,
   * MediatR - works out of the box,
   * AutoMapper - when unit testing all mappings set up then there are no surprises,
   * Entity Framework Core 8 (EF) - EF gives me issues see below,
   * Swagger - Works out of the box,
   * Postgres SQL database - Postgres and npgsql works with out problems,
   * Angular - I did it and the page works. It's not easy as backend developer,
   * Python - That was fast and easy to code towards CQL,
   * FluentMigrator - I've had to code the migrator then it works with embedded scripts,
   * Dimensional modelling - I've problems with it and EF,
   * Clean code architecture - no comment,
   * Optional: Docker for the database - haven't tried it.

EF gives me some issues:
1) The unit tests with the database becomes long, unmainaintable and complex, because the arrangment often becomes very complex before activation and assertion. The database has to be seeded with data. A particular situation has to bet present. The same EF db context may not be used for the activation. I believe a Docker image could help, because you can seed and prepare the situations in the image.
2) I'm used to write SQL functions or stored procedures that do the handling of data and relations. With EF you have to program that handling like look up existing related entries to the entry for insertion, set the foreign keys, clear navigational properties in the entity inserted, otherwise related entry is inserted as duplicates.
3) I've to think carefully which changes go together before calling EF SaveChanges(). With a SQL stored procedure I handed over these decision to the procedure.
4) EF version 8 don't have any bulk insertion.
