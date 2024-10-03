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

There is unfortunately a bug with EF that I haven't solved for timeseries. EF tries to insert an existing timestamp in the _Dates_ table, where timestamps are unique. The repetable scenario is for the second timeserie for a second stock with same period as first stock, EF fails to store the facts, because Postgres throws an exception stating insertion of a duplicate timestamp in _Dates_ table.
I've designed the timeseries as a dimension entity model. The table _TimeSerieFacts_ has the facts prize and volume and includes two foreign keys to the dimensions tables _Dates and _Timeseries_. The shadowed primary key of _TimeSerieFacts_ is the combination of the foreign keys DateId and TimeSerieId, because a there must only be one fact per date and timeserie. The _Dates_ table holds an id as primary key and a timestamp with timezone. There must only be unique timestamps in the table. The _TimeSeries_ table holds metadata for a timeserie as name and a foreign key to the _Stock_ table. The reason for having a _Dates_ table is that it can save a little amount storage, when many stocks have facts for the same day thus same timestamp. And it may be useful in calculation like moving averages or getting the prize for the end of a week. The _Dates_ table can hold the week number or month as well.
The EF TimeSerieFacts insertion method AddOrUpdateTimeSerieAddFacts in Gateways.Repositories.Stock looks up and sets the foreing keys for timestamp, DateId, and timeserie, TimeSerieId, in _Dates_ and _TimeSeries_ tables for the _TimeSerieFactsDao_ to insert and clears _Date_ and _TimeSerie_ navigation properties of the _TimeSerieFactsDao_. This works for all other EF insertion methods that have to refer to en exising related entry. And, EF doesn't insert a new entry. I've added an dbContext.Attach(existingDate) call, so EF should mark it _Unchanged_ in tracking of the Date entry. How ever it doesn't prevent the bug. I've to program a unit test that fails due to the problem, and solve it.
