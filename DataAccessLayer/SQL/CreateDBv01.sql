-- Create Elis DB version 02 from scratch
-- Log
-- 2016-10-21 Jan: Created script.
-- 2017-04-12 Jan: Added index on timeserie.
-- 2018-02-03 Jan: v02 Redesigned the database for dimension modelling and simplified it.
-- ----------------------------------------------------------------------------

-- The table drops is removed when database got real data
 --drop tables in inverse order
 --DROP TABLE IF EXISTS Version;
 --DROP TABLE IF EXISTS Timeserie;
 --DROP TABLE IF EXISTS StockSerie;   
 --DROP TABLE IF EXISTS Serie;
 --DROP TABLE IF EXISTS Serieattribute;
 --DROP TABLE IF EXISTS Datasource;
 --DROP TABLE IF EXISTS Valuetype;
 --DROP TABLE IF EXISTS Stock;
 --DROP TABLE IF EXISTS Market;
 --DROP TABLE IF EXISTS Country;
 --DROP TABLE IF EXISTS Currency;

-- CREATE FUNCTION getVersion() RETURNS integer[] AS $$
-- DECLARE
    -- version integer[] := {0,0};
    -- ma integer := 0;
    -- mi interger == 0;
-- BEGIN
    -- if exists( select * from information_schema.tables 
              -- where 
              -- table_catalog = CURRENT_CATALOG and table_schema = CURRENT_SCHEMA
              -- and table_name = 'Version' ) 
    -- then	    
       -- SELECT Major, Minor INTO ma, mi FROM Version ORDER BY Major, Minor DESC LIMIT 1       
    -- end if;
    -- RETURN ;
-- END;
-- $$ LANGUAGE plpgsql;

-- create tables
--CREATE TABLE version (
--		id SERIAL
--	,	major int NOT NULL
--	,	minor int NOT NULL
--	,	CONSTRAINT pkversion PRIMARY KEY (id)
--);
--INSERT INTO Version ( major, minor) VALUES ( 0, 1);

-- ISO 4217
CREATE TABLE CurrencyDim (
		id SERIAL
	,	name VARCHAR NOT NULL
	,	shortname VARCHAR NOT NULL	
	,	CONSTRAINT pkcurrency PRIMARY  KEY (Id)
);
INSERT INTO CurrencyDim (name, shortname) VALUES ( 'Euro', 'EUR' );
INSERT INTO CurrencyDim (name, shortname) VALUES ( 'Denmark Krone', 'DKK' );
INSERT INTO CurrencyDim (name, shortname) VALUES ( 'Norway krone', 'NOK' );
INSERT INTO CurrencyDim (name, shortname) VALUES ( 'Sweden krona', 'SEK' );
INSERT INTO CurrencyDim (name, shortname) VALUES ( 'US Dollar', 'USD' );
INSERT INTO CurrencyDim (name, shortname) VALUES ( 'Australia Dollar', 'AUD' );
INSERT INTO CurrencyDim (name, shortname) VALUES ( 'United Kingdom Pound', 'GBP' );

-- ISO 3166 codes
CREATE TABLE CountryDim (
		id SERIAL
	,	name VARCHAR NOT NULL
	,	shortname2letter VARCHAR NOT NULL	
	,	shortname3letter VARCHAR NOT NULL	
	,	currencyId int NOT NULL
	,	CONSTRAINT pkcontry PRIMARY  KEY (Id)
);
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Denmark', 'DK', 'DNK' 
	,	(SELECT id FROM CurrencyDim WHERE shortname='DKK'));
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'United Kingdom', 'GB', 'GBR'
	,	(SELECT id FROM CurrencyDim WHERE shortname='GBP'));
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'United States', 'US', 'US'
	,	(SELECT id FROM CurrencyDim WHERE shortname='USD'));
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Sweden', 'SE', 'SWE'
	,	(SELECT id FROM CurrencyDim WHERE shortname='SEK'));
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Norway', 'NO', 'NOR'
	,	(SELECT id FROM CurrencyDim WHERE shortname='NOK'));
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Finland', 'FI', 'FIN'
	,	(SELECT id FROM CurrencyDim WHERE shortname='EUR'));
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'France', 'FR', 'FRA'
	,	(SELECT id FROM CurrencyDim WHERE shortname='EUR'));
INSERT INTO CountryDim (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Australia', 'AU', 'AUS'
	,	(SELECT id FROM CurrencyDim WHERE shortname='AUD'));
	
CREATE TABLE MarketDim (
		id SERIAL
	,	name VARCHAR NOT NULL
	,	shortname VARCHAR
	,	provider VARCHAR NOT NULL
	,	countryId int
	,	CONSTRAINT pkmarket PRIMARY  KEY (Id)
	, 	CONSTRAINT fkcountry FOREIGN KEY ( countryId ) REFERENCES  CountryDim ( id )
);
INSERT INTO MarketDim (name, shortname, provider, countryId) VALUES 
	( 'Copenhagen Stock Exchange', 'Copenhagen', 'Nasdaq OMX', (SELECT id FROM countryDim WHERE name = 'Denmark' ));
INSERT INTO MarketDim (Name, shortname, provider, countryId) VALUES 
	( 'New York Stock Exchange', 'NYSE', 'Intercontinental Exchange', (SELECT id FROM countryDim WHERE name = 'United States' ));
INSERT INTO MarketDim (name, shortname, provider, countryId) VALUES 
	( 'Nasdaq OMX U.S.', 'Nasdaq', 'Nasdaq OMX U.S.', (SELECT id FROM countryDim WHERE name = 'United States' ));

-- instrument as a stock, obligation, ETN, ETF, CFD etc.
CREATE TABLE StockDim (   
		id SERIAL
	,	name VARCHAR NOT NULL
	,	shortname VARCHAR NOT NULL -- 
	,	marketId int NOT NULL
	,	CONSTRAINT pkstock PRIMARY KEY (Id)
	, 	CONSTRAINT fkstockmarket FOREIGN KEY ( marketId ) REFERENCES  MarketDim ( id )
);

INSERT INTO StockDim (Name, shortname, MarketId) VALUES ( 
		'Novo Nordisk B'
	,	'NOVO-B'
	,	(SELECT id FROM MarketDim WHERE name = 'Copenhagen Stock Exchange' )	
);

CREATE TABLE ValueDim (
		id SERIAL
	,	name VARCHAR NOT NULL
	,	valueShift INT NOT NULL
	,	CONSTRAINT pkdatatype PRIMARY KEY (Id)
);
INSERT INTO ValueDim (name, valueShift) VALUES ( '2 decimals', 2);
INSERT INTO ValueDim (name, valueShift) VALUES ( 'whole number', 0);
INSERT INTO ValueDim (name, valueShift) VALUES ( '4 decimals',4);
INSERT INTO ValueDim (name, valueShift) VALUES ( '6 decimals',6);

CREATE TABLE SerieAttributeDim (
		id SERIAL
	,	name VARCHAR NOT NULL
	,	description VARCHAR NOT NULL
	,	CONSTRAINT pkserieattribute PRIMARY KEY (Id)
);
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'open', 'The opening price for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'close', 'The closing price for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'high', 'The high price for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'low', 'The low price for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'volume', 'The volume for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'avg', 'The average price for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'turnover', 'The turnover for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'calculated', 'The calculated value for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'index', 'The index for the date (day)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'dividend', 'The dividend for the date' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'split', 'The split of the stock from that date)' );
INSERT INTO SerieAttributeDim (name, description) VALUES ( 'currency exchange', 'The currency exchange of one currency to another' );

CREATE TABLE SerieDim (
		id SERIAL
	,	name VARCHAR NOT NULL
	,	valueId int NOT NULL
	,	currencyId int NOT NULL
	,	serieAttributeId int NOT NULL
	,	CONSTRAINT pkserie PRIMARY KEY (Id)
	, 	CONSTRAINT fkserievaluetype FOREIGN KEY ( valueId ) REFERENCES  ValueDim ( id )
	, 	CONSTRAINT fkseriecurrency FOREIGN KEY ( currencyId ) REFERENCES CurrencyDim ( id )
	, 	CONSTRAINT fkserieserieattribute FOREIGN KEY ( serieAttributeId ) REFERENCES SerieAttributeDim ( id )
);
INSERT INTO SerieDim ( name, valueId, currencyId, serieAttributeId) VALUES (
		'NOVO-B daily close price'
	,	(SELECT id FROM ValueDim WHERE name = '2 decimals' )
	,	(SELECT id FROM CurrencyDim WHERE shortname = 'DKK' )
	,	(SELECT id FROM SerieAttributeDim WHERE name = 'close' )		
);
INSERT INTO SerieDim ( name, valueId, currencyId, serieAttributeId) VALUES ( 
		'NOVO-B daily volume'
	,	(SELECT id FROM ValueDim WHERE name = 'whole number' )
	,	(SELECT id FROM CurrencyDim WHERE shortname = 'DKK' )
	,	(SELECT id FROM SerieAttributeDim WHERE name = 'volume' )		
);

CREATE TABLE StockSerieRel (
		stockId int NOT NULL
	,	serieId int NOT NULL
	,	CONSTRAINT pkstockserie PRIMARY KEY (stockId, serieId)
	, 	CONSTRAINT fkstockseriestock FOREIGN KEY ( stockId ) REFERENCES  StockDim ( id )
	, 	CONSTRAINT fkstockserieserie FOREIGN KEY ( serieId ) REFERENCES  SerieDim ( id )	
);
INSERT INTO StockSerieRel ( stockId, serieId) VALUES ( 
		(SELECT id FROM StockDim WHERE shortname = 'NOVO-B' )
	,	(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price' )
);
INSERT INTO StockSerieRel ( stockId, serieId) VALUES ( 
		(SELECT id FROM StockDim WHERE shortname = 'NOVO-B' )
	,	(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily volume' )
);

CREATE TABLE TimeDim (
		id SERIAL
	,	time TIMESTAMPTZ NOT NULL   
	,	CONSTRAINT pktime PRIMARY KEY ( id )
);
INSERT INTO TimeDim (time) VALUES ('2016-10-20');
INSERT INTO TimeDim (time) VALUES ('2016-10-19');
INSERT INTO TimeDim (time) VALUES ('2016-10-18');
INSERT INTO TimeDim (time) VALUES ('2016-10-17');
INSERT INTO TimeDim (time) VALUES ('2016-10-14');
INSERT INTO TimeDim (time) VALUES ('2016-10-13');
INSERT INTO TimeDim (time) VALUES ('2016-10-12');
INSERT INTO TimeDim (time) VALUES ('2016-10-11');
INSERT INTO TimeDim (time) VALUES ('2016-10-10');
INSERT INTO TimeDim (time) VALUES ('2016-10-07');
INSERT INTO TimeDim (time) VALUES ('2016-10-06');
INSERT INTO TimeDim (time) VALUES ('2016-10-05');
INSERT INTO TimeDim (time) VALUES ('2016-10-04');
INSERT INTO TimeDim (time) VALUES ('2016-10-03');
INSERT INTO TimeDim (time) VALUES ('2016-09-30');
INSERT INTO TimeDim (time) VALUES ('2016-09-29');
INSERT INTO TimeDim (time) VALUES ('2016-09-28');
INSERT INTO TimeDim (time) VALUES ('2016-09-27');
INSERT INTO TimeDim (time) VALUES ('2016-09-26');
INSERT INTO TimeDim (time) VALUES ('2016-09-23');
INSERT INTO TimeDim (time) VALUES ('2016-09-22');
INSERT INTO TimeDim (time) VALUES ('2016-09-21');

CREATE TABLE SerieFact (
		id SERIAL
	,	timeId int NOT NULL   
	,	serieId int NOT NULL
	,	value int NOT NULL       
	,	CONSTRAINT pkseriedata PRIMARY KEY (Id)
	, 	CONSTRAINT fkseriedataserie FOREIGN KEY ( SerieId ) REFERENCES  SerieDim ( id )
	, 	CONSTRAINT fktime FOREIGN KEY ( timeId ) REFERENCES  TimeDim ( id )
);

-- Index: timeandserieidix
-- DROP INDEX public.timeandserieidix;
CREATE UNIQUE INDEX timeandserieidix
    ON public.SerieFact USING btree
    (serieId, timeId, value)
    TABLESPACE pg_default;

-- closing prices for Novo, test
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-20'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27820);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-19'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27630);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-18'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27530);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-17'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27260);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-14'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27050);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-13'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 26570);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-12'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 26740);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-11'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 26890);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-10'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27070);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-07'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27030);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-06'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27350);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-05'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27770);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-04'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27170);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-10-03'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27150);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-30'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 27540);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-29'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 28000);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-28'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 29000);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-27'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 28870);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-26'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 29620);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-23'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 29680);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-22'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 30400);
INSERT INTO SerieFact (timeId, serieId, value) VALUES ((SELECT id FROM TimeDim WHERE time ='2016-09-21'),(SELECT id FROM SerieDim WHERE name = 'NOVO-B daily close price'), 30770);

select * from StockDim;


