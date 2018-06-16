-- FUNCTION: public."GetTimeSeries"()

DROP FUNCTION public."GetTimeSeries"();

CREATE OR REPLACE FUNCTION GetTimeSeries ( serieId INT, begin TIMESTAMPTZ, end TIMESTAMPTZ ) 
 RETURNS TABLE (
  ???,
  INT
) 
AS $$
BEGIN
 RETURN QUERY SELECT
 title,
 cast( release_year as integer)
 FROM
 film
 WHERE
 title ILIKE p_pattern ;
END; $$ 

$BODY$;

ALTER FUNCTION public."GetTimeSeries"()
    OWNER TO "ElisStockWriter";

GRANT EXECUTE ON FUNCTION public."GetTimeSeries"() TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."GetTimeSeries"() TO "ElisStockWriter";

COMMENT ON FUNCTION public."GetTimeSeries"()
    IS 'Gets time serie for a serieId for a period with a start and end time';

