-- *****************
-- use WorkforceTest
-- *****************


DECLARE @dbName VARCHAR(128)
SELECT @dbName = (SELECT DB_NAME() AS DatabaseName)

IF @dbName != 'master' AND
   @dbName != 'model' AND
   @dbName != 'msdb' AND
   @dbName != 'tempdb' AND
   @dbName != 'Nop' AND
   SUBSTRING(@dbName, 1, 12) != 'ReportServer'
BEGIN


    declare @n char(1)
    set @n = char(10)

    declare @stmt nvarchar(max)

    -- procedures
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop procedure [' + schema_name(schema_id) + '].[' + name + ']'
    from sys.procedures


    -- check constraints
    select @stmt = isnull( @stmt + @n, '' ) +
    'alter table [' + schema_name(schema_id) + '].[' + object_name( parent_object_id ) + ']    drop constraint [' + name + ']'
    from sys.check_constraints

    -- functions
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop function [' + schema_name(schema_id) + '].[' + name + ']'
    from sys.objects
    where type in ( 'FN', 'IF', 'TF' )

    -- views
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop view [' + schema_name(schema_id) + '].[' + name + ']'
    from sys.views

    -- foreign keys
    select @stmt = isnull( @stmt + @n, '' ) +
        'alter table [' + schema_name(schema_id) + '].[' + object_name( parent_object_id ) + '] drop constraint [' + name + ']'
    from sys.foreign_keys

    -- tables
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop table [' + schema_name(schema_id) + '].[' + name + ']'
    from sys.tables

    -- user defined types
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop type [' + schema_name(schema_id) + '].[' + name + ']'
    from sys.types
    where is_user_defined = 1


    exec sp_executesql @stmt


END
ELSE
BEGIN
    PRINT 'Drop tables in database [' + @dbName + '] is not allowed'
END
GO

