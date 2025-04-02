-- TODO may be this should be a view called from unit tests inserting or updating timeseriesfacts
-- to ensure no duplet entries in Dates
; WITH
DateTimeUniqCount AS (
SELECT 
	"DateTimeUtc",
	COUNT(*) AS "count"
	FROM public."Dates"
GROUP BY "DateTimeUtc"
)

SELECT *
FROM DateTimeUniqCount
WHERE "count" > 1
;