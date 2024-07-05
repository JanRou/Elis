DO $$
DECLARE 
	nameIn varchar(200) := '' ;
	isinIn varchar(200) := '';
	exchangeNameIn varchar(200) := '';
	currencyCodeIn varchar(200) := '';
	orderByColumnIn varchar(200) := '';
	takeIn integer := 0;
	skipIn integer := 0;
BEGIN
 	drop table if exists _x;	
	create temporary table _x as
	SELECT 
		st."Id"
	FROM public."Stocks" st
	JOIN public."Exchanges" ex ON ex."Id" = st."ExchangeId"
	JOIN public."Currencies" cu ON cu."Id" = st."CurrencyId"
	WHERE
			( (nameIn = '' IS NOT FALSE) OR (st."Name" like nameIn) )
		AND ( (isinIn = '' IS NOT FALSE) OR (st."Isin" like isinIn) )
		AND ( (exchangeNameIn = '' IS NOT FALSE) OR (ex."Name" like exchangeNameIn) )
		AND ( (currencyCodeIn = '' IS NOT FALSE) OR (cu."Code" like currencyCodeIn) )
	ORDER BY 
	CASE 
		WHEN OrderByColumnIn='Isin' THEN st."Isin"
		ELSE st."Name"
	END		
	LIMIT CASE takeIn WHEN 0 THEN NULL ELSE takeIn END
	OFFSET skipIn
	;		

END;
$$ LANGUAGE plpgsql;

select * from _x;