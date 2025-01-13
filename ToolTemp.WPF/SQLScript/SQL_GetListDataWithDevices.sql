CREATE PROCEDURE GetListDataWithDevices
    @Port NVARCHAR(50),
    @Factory NVARCHAR(100),
    @Address INT,
    @Baudrate INT,
    @Language NVARCHAR(10) -- Tham số xác định ngôn ngữ
AS
BEGIN
    -- Xác định typeid dựa trên ngôn ngữ
    DECLARE @TypeId INT;
    IF @Language = 'vi'  -- Tiếng Việt
        SET @TypeId = 2;
    ELSE IF @Language = 'en'  -- Tiếng Anh
        SET @TypeId = 3;
    ELSE IF @Language = 'cn'  -- Tiếng Trung
        SET @TypeId = 1;
    ELSE
        SET @TypeId = NULL; -- Giá trị mặc định nếu ngôn ngữ không hợp lệ

    -- Lọc dữ liệu từ dv_BusDataTemp và kết hợp với bảng devices
    WITH tempt AS (
        SELECT *,
               ROW_NUMBER() OVER (PARTITION BY Channel, Factory, AddressMachine, Port, Baudrate ORDER BY UploadDate DESC) AS rn
        FROM dv_BusDataTemp
        WHERE Factory = @Factory 
          AND AddressMachine = @Address 
          AND Port = @Port 
          AND Baudrate = @Baudrate
    )
    SELECT t.Id,
           t.IdMachine,
           t.IdStyle,
           t.Channel,
           t.Factory,
           t.Line,
           t.AddressMachine,
           t.LineCode,
           t.Port,
           t.Temp,
           t.Max,
           t.Min,
           t.Baudrate,
           t.UploadDate,
           t.IsWarning,
           t.Sensor_Typeid,
           t.Sensor_kind,
           t.Sensor_ant,
           d.name AS DeviceName -- Tên thiết bị từ bảng devices
    FROM tempt t
    LEFT JOIN devices d ON t.Channel = d.devid
    WHERE t.rn = 1
      AND (d.typeid = @TypeId OR @TypeId IS NULL); -- Lọc theo typeid, cho phép NULL
END;
