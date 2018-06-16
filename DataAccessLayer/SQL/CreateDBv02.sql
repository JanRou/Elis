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
CREATE TABLE Currency (
		id SERIAL PRIMARY KEY
	,	name VARCHAR NOT NULL
	,	shortname VARCHAR NOT NULL	
);
INSERT INTO Currency (name, shortname) VALUES ( 'Euro', 'EUR' );
INSERT INTO Currency (name, shortname) VALUES ( 'Denmark Krone', 'DKK' );
INSERT INTO Currency (name, shortname) VALUES ( 'Norway krone', 'NOK' );
INSERT INTO Currency (name, shortname) VALUES ( 'Sweden krona', 'SEK' );
INSERT INTO Currency (name, shortname) VALUES ( 'US Dollar', 'USD' );
INSERT INTO Currency (name, shortname) VALUES ( 'Australia Dollar', 'AUD' );
INSERT INTO Currency (name, shortname) VALUES ( 'United Kingdom Pound', 'GBP' );

-- ISO 3166 codes
CREATE TABLE Country (
		id SERIAL PRIMARY KEY
	,	name VARCHAR NOT NULL
	,	shortname2letter VARCHAR NOT NULL	
	,	shortname3letter VARCHAR NOT NULL	
	,	currencyId int NOT NULL
);
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Denmark', 'DK', 'DNK' 
	,	(SELECT id FROM Currency WHERE shortname='DKK'));
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'United Kingdom', 'GB', 'GBR'
	,	(SELECT id FROM Currency WHERE shortname='GBP'));
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'United States', 'US', 'US'
	,	(SELECT id FROM Currency WHERE shortname='USD'));
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Sweden', 'SE', 'SWE'
	,	(SELECT id FROM Currency WHERE shortname='SEK'));
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Norway', 'NO', 'NOR'
	,	(SELECT id FROM Currency WHERE shortname='NOK'));
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Finland', 'FI', 'FIN'
	,	(SELECT id FROM Currency WHERE shortname='EUR'));
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'France', 'FR', 'FRA'
	,	(SELECT id FROM Currency WHERE shortname='EUR'));
INSERT INTO Country (name, shortname2letter, shortname3letter, currencyId) VALUES ( 'Australia', 'AU', 'AUS'
	,	(SELECT id FROM Currency WHERE shortname='AUD'));
	
CREATE TABLE Market (
		id SERIAL  PRIMARY KEY
	,	name VARCHAR NOT NULL
	,	shortname VARCHAR
	,	provider VARCHAR NOT NULL
	,	countryid SERIAL REFERENCES  Country ( id )
);
INSERT INTO Market (name, shortname, provider, countryId) VALUES 
	( 'Copenhagen Stock Exchange', 'Copenhagen', 'Nasdaq OMX', (SELECT id FROM Country WHERE name = 'Denmark' ));
INSERT INTO Market (Name, shortname, provider, countryId) VALUES 
	( 'New York Stock Exchange', 'NYSE', 'Intercontinental Exchange', (SELECT id FROM Country WHERE name = 'United States' ));
INSERT INTO Market (name, shortname, provider, countryId) VALUES 
	( 'Nasdaq OMX U.S.', 'Nasdaq', 'Nasdaq OMX U.S.', (SELECT id FROM Country WHERE name = 'United States' ));

-- instrument as a stock, obligation, ETN, ETF, CFD etc.
CREATE TABLE Stock (   
		id SERIAL PRIMARY KEY
	,	name VARCHAR NOT NULL
	,	shortname VARCHAR NOT NULL -- 
	,	marketid SERIAL NOT NULL REFERENCES  Market ( id )
);

INSERT INTO Stock (Name, shortname, MarketId) VALUES ( 
		'Novo Nordisk B'
	,	'NOVO-B'
	,	(SELECT id FROM Market WHERE name = 'Copenhagen Stock Exchange' )	
);

CREATE TABLE ValueDescription (
		id SERIAL PRIMARY KEY
	,	name VARCHAR NOT NULL
	,	valueshift INT NOT NULL
	,	unit varchar not null
	,	maxvalue bigint null
	,	minvalue bigint null	
);

	-- ,	currencyid SERIAL REFERENCES  Currency ( id )
-- 	,	(SELECT id FROM Currency WHERE shortname = 'DKK' )
INSERT INTO ValueDescription (name, valueshift, unit, maxvalue, minvalue) VALUES ( 'currency 2 decimals', 2, 'currency', 1000000000000, 0);
INSERT INTO ValueDescription (name, valueshift, unit, maxvalue, minvalue) VALUES ( 'whole number', 0, 'currency', 1000000000000, 0);

CREATE TABLE SeriesDescription (
		id SERIAL PRIMARY KEY
	,	name VARCHAR NOT NULL
	,	description VARCHAR NOT NULL
);
INSERT INTO SeriesDescription (name, description) VALUES ( 'open', 'The opening price for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'close', 'The closing price for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'high', 'The high price for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'low', 'The low price for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'volume', 'The volume for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'avg', 'The average price for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'turnover', 'The turnover for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'calculated', 'The calculated value for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'index', 'The index for the date (day)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'dividend', 'The dividend for the date' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'split', 'The split of the stock from that date)' );
INSERT INTO SeriesDescription (name, description) VALUES ( 'currency exchange', 'The currency exchange of one currency to another' );

CREATE TABLE SeriesDim (
		id SERIAL PRIMARY KEY
	,	name VARCHAR NOT NULL
	,	currencyid SERIAL REFERENCES Currency (id)
	,	valuedescriptionid SERIAL NOT NULL REFERENCES  ValueDescription ( id )
	,	seriesdescriptionid SERIAL NOT NULL REFERENCES SeriesDescription ( id )
);

INSERT INTO SeriesDim ( name, currencyId, valuedescriptionid, seriesdescriptionid) VALUES (
		'NOVO-B daily close price'
	,	(SELECT id FROM Currency WHERE shortname = 'DKK' )		
	,	(SELECT id FROM ValueDescription WHERE name = 'currency 2 decimals' )
	,	(SELECT id FROM SeriesDescription WHERE name = 'close')
);
	
INSERT INTO SeriesDim ( name, currencyId, valuedescriptionid, seriesdescriptionid) VALUES ( 
		'NOVO-B daily volume'
	,	(SELECT id FROM Currency WHERE shortname = 'DKK' )		
	,	(SELECT id FROM ValueDescription WHERE name = 'whole number' )
	,	(SELECT id FROM SeriesDescription WHERE name = 'volume' )		
);

CREATE TABLE StockSeriesRelation (
		stockid int NOT NULL REFERENCES  Stock ( id )
	,   seriesdimid int NOT NULL REFERENCES  SeriesDim ( id )
	,	CONSTRAINT pkstockserie PRIMARY KEY (stockId, seriesdimid)
);
INSERT INTO StockSeriesRelation ( stockid, seriesdimid) VALUES ( 
		(SELECT id FROM Stock WHERE shortname = 'NOVO-B' )
	,	(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price' )
);
INSERT INTO StockSeriesRelation ( stockid, seriesdimid) VALUES ( 
		(SELECT id FROM Stock WHERE shortname = 'NOVO-B' )
	,	(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily volume' )
);

CREATE TABLE TimeSeriesFact (
		id SERIAL PRIMARY KEY
	,	timelocalsource TIMESTAMP NOT NULL  -- no timezone
	,	timeutc TIMESTAMP NOT NULL			-- no timezone
	,	v INT NOT NULL
	,	seriesdimid SERIAL NOT NULL REFERENCES  SeriesDim ( id )
);

-- Index: timeandseriesidix
-- DROP INDEX public.timeandseriesdimidix;
-- index has also value, so a lookup is prevented
CREATE UNIQUE INDEX timeandseriesdimidix 
    ON public.TimeSeriesFact USING btree
    (seriesdimid, timeutc, v)
    TABLESPACE pg_default;

-- closing prices for Novo, test
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-20','2016-10-20 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27820);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-19','2016-10-19 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27630);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-18','2016-10-18 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27530);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-17','2016-10-17 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27260);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-14','2016-10-14 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27050);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-13','2016-10-13 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 26570);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-12','2016-10-12 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 26740);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-11','2016-10-11 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 26890);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-10','2016-10-10 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27070);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-07','2016-10-07 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27030);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-06','2016-10-06 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27350);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-05','2016-10-05 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27770);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-04','2016-10-04 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27170);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-10-03','2016-10-03 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27150);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-30','2016-09-30 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 27540);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-29','2016-09-29 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 28000);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-28','2016-09-28 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 29000);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-27','2016-09-27 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 28870);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-26','2016-09-26 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 29620);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-23','2016-09-23 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 29680);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-22','2016-09-22 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 30400);
INSERT INTO TimeSeriesFact (timelocalsource, timeutc, seriesdimid, v) VALUES ('2016-09-21','2016-09-21 00:00:00-2',(SELECT id FROM SeriesDim WHERE name = 'NOVO-B daily close price'), 30770);

select * from Stock;


