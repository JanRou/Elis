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
WHERE 
	-- st."Isin" = 'SE0009268717' -- Acarix
	-- st."Isin" = 'DK0061113511' -- Konsolidator
	-- st."Isin" = 'DK0062498333'-- Novo
	-- st."Isin" = 'DK0010219153' -- Rockwool
	st."Isin" = 'DK0010253921' -- Schouw & Co
	-- AND
	-- da."DateTimeUtc" > '2025-01-01'
ORDER BY da."DateTimeUtc"
;

/*
DELETE FROM public."TimeSerieFacts";
DELETE FROM public."TimeSeries" ts;
ALTER TABLE public."TimeSeries" ALTER COLUMN "Id" RESTART WITH 1;
*/