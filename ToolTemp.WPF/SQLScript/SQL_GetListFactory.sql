CREATE PROCEDURE GetListFactory
	@FactoryCode NVARCHAR(50)
AS
BEGIN
	SELECT *
	FROM dv_Factory_Configs
	WHERE @FactoryCode = FactoryCode
END;

