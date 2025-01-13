CREATE PROCEDURE GetLineByAddressAndFactory
    @Address INT, @Factory NVARCHAR(100)
AS
BEGIN
    SELECT 
        Line
    FROM 
        dv_Factory_Configs
    WHERE 
        Address = @Address AND FactoryCode = @Factory
END;
