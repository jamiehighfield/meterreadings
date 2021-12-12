DO
$func$
BEGIN
	IF NOT EXISTS (select * from pg_tables where schemaname = 'public') THEN
		RETURN;
	END IF;

	EXECUTE (SELECT 'DROP TABLE IF EXISTS '
		|| string_agg(quote_ident(schemaname) || '.' || quote_ident(tablename), ', ')
		|| ' CASCADE'
	FROM   pg_tables
	WHERE  (schemaname = 'public')
	);
END
$func$;
