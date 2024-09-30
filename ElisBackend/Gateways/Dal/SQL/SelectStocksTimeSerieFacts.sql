SELECT 
		st."Name" AS "StockName"
	,	st."Isin"
	,	ts."Name" AS "TimeSeriesName"
	,	da."DateTimeUtc"
	,	fa."Price"
	,	fa."Volume"
FROM public."TimeSeries" ts
JOIN public."Stocks" st ON st."Id" = ts."StockId"
JOIN public."TimeSerieFacts" fa ON fa."TimeSerieId" = ts."Id"
JOIN public."Dates" da ON da."Id" = fa."DateId"
;

/*
DELETE FROM public."TimeSerieFacts";
DELETE FROM public."TimeSeries" ts;
ALTER TABLE public."TimeSeries" ALTER COLUMN "Id" RESTART WITH 1;
*/