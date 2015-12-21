



































IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_FlightStatus_UpsertBulk')
	exec('CREATE PROCEDURE [dbo].[usp_FlightStatus_UpsertBulk] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[usp_FlightStatus_UpsertBulk]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @OutputHolder TABLE ([Id] BIGINT, [BulkSaveId] BIGINT, [UpsertAction] VARCHAR(32));
	MERGE INTO [dbo].[FlightStatus] trg
	USING (
		SELECT * FROM #BulkSaveFlightStatus
	) as src
	ON trg.Id = src.Id AND scr.Id > 0
	WHEN MATCHED THEN
		UPDATE SET
			[FlightStatusRequestId] = src.[FlightStatusRequestId]
			, [FlightId] = src.[FlightId]
			, [CarrierFsCode] = src.[CarrierFsCode]
			, [FlightNumber] = src.[FlightNumber]
			, [DepAirportFsCode] = src.[DepAirportFsCode]
			, [ArrAirportFsCode] = src.[ArrAirportFsCode]
			, [DepDateL] = src.[DepDateL]
			, [DepDateUtc] = src.[DepDateUtc]
			, [ArrDateL] = src.[ArrDateL]
			, [ArrDateUtc] = src.[ArrDateUtc]
			, [Status] = src.[Status]
			, [FlightType] = src.[FlightType]
			, [ServiceClasses] = src.[ServiceClasses]
			, [Restrictions] = src.[Restrictions]
			, [SchedEqiupIataCode] = src.[SchedEqiupIataCode]
			, [ActEquipIataCode] = src.[ActEquipIataCode]
	WHEN NOT MATCHED THEN
		INSERT (
			[FlightStatusRequestId]
			, [FlightId]
			, [CarrierFsCode]
			, [FlightNumber]
			, [DepAirportFsCode]
			, [ArrAirportFsCode]
			, [DepDateL]
			, [DepDateUtc]
			, [ArrDateL]
			, [ArrDateUtc]
			, [Status]
			, [FlightType]
			, [ServiceClasses]
			, [Restrictions]
			, [SchedEqiupIataCode]
			, [ActEquipIataCode]
		) VALUES (
			src.[FlightStatusRequestId]
			, src.[FlightId]
			, src.[CarrierFsCode]
			, src.[FlightNumber]
			, src.[DepAirportFsCode]
			, src.[ArrAirportFsCode]
			, src.[DepDateL]
			, src.[DepDateUtc]
			, src.[ArrDateL]
			, src.[ArrDateUtc]
			, src.[Status]
			, src.[FlightType]
			, src.[ServiceClasses]
			, src.[Restrictions]
			, src.[SchedEqiupIataCode]
			, src.[ActEquipIataCode]
		)
	OUTPUT inserted.Id, src.BulkSaveId, $Action
	INTO @OutputHolder([Id], [BulkSaveId], [UpsertAction]);
	SELECT * FROM @OutputHolder
END

GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_FlightStatusData_UpsertBulk')
	exec('CREATE PROCEDURE [dbo].[usp_FlightStatusData_UpsertBulk] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[usp_FlightStatusData_UpsertBulk]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @OutputHolder TABLE ([Id] BIGINT, [BulkSaveId] BIGINT, [UpsertAction] VARCHAR(32));
	MERGE INTO [dbo].[FlightStatusData] trg
	USING (
		SELECT * FROM #BulkSaveFlightStatusData
	) as src
	ON trg.Id = src.Id AND scr.Id > 0
	WHEN MATCHED THEN
		UPDATE SET
			[FlightStatusId] = src.[FlightStatusId]
			, [FlightId] = src.[FlightId]
			, [PubDepLocal] = src.[PubDepLocal]
			, [PubDepUtc] = src.[PubDepUtc]
			, [PubArrLocal] = src.[PubArrLocal]
			, [PubArrUtc] = src.[PubArrUtc]
			, [FlightPlanPlannedDepLocal] = src.[FlightPlanPlannedDepLocal]
			, [FlightPlanPlannedDepUtc] = src.[FlightPlanPlannedDepUtc]
			, [FlightPlanPlannedArrLocal] = src.[FlightPlanPlannedArrLocal]
			, [FlightPlanPlannedArrUtc] = src.[FlightPlanPlannedArrUtc]
			, [SchedGateDepLocal] = src.[SchedGateDepLocal]
			, [SchedGateDepUtc] = src.[SchedGateDepUtc]
			, [EstGateDepLocal] = src.[EstGateDepLocal]
			, [EstGateDepUtc] = src.[EstGateDepUtc]
			, [ActGateDepLocal] = src.[ActGateDepLocal]
			, [ActGateDepUtc] = src.[ActGateDepUtc]
			, [SchedGateArrLocal] = src.[SchedGateArrLocal]
			, [SchedGateArrUtc] = src.[SchedGateArrUtc]
			, [EstGateArrLocal] = src.[EstGateArrLocal]
			, [EstGateArrUtc] = src.[EstGateArrUtc]
			, [EstRunwayDepLocal] = src.[EstRunwayDepLocal]
			, [EstRunwayDepUtc] = src.[EstRunwayDepUtc]
			, [ActRunwayDepLocal] = src.[ActRunwayDepLocal]
			, [ActRunwayDepUtc] = src.[ActRunwayDepUtc]
			, [EstRunwayArrLocal] = src.[EstRunwayArrLocal]
			, [EstRunwayArrUtc] = src.[EstRunwayArrUtc]
			, [ActRunwayArrLocal] = src.[ActRunwayArrLocal]
			, [ActRunwayArrUtc] = src.[ActRunwayArrUtc]
			, [DepGateDelayMins] = src.[DepGateDelayMins]
			, [DepRunwayDelayMins] = src.[DepRunwayDelayMins]
			, [ArrGateDelayMins] = src.[ArrGateDelayMins]
			, [ArrRunwayDelayMins] = src.[ArrRunwayDelayMins]
			, [SchedBlockMins] = src.[SchedBlockMins]
			, [BlockMins] = src.[BlockMins]
			, [SchedAirMins] = src.[SchedAirMins]
			, [AirMins] = src.[AirMins]
			, [SchedTaxiOutMins] = src.[SchedTaxiOutMins]
			, [TaxiOutMins] = src.[TaxiOutMins]
			, [SchedTaxiInMins] = src.[SchedTaxiInMins]
			, [TaxiInMins] = src.[TaxiInMins]
			, [DepTerminal] = src.[DepTerminal]
			, [DepGate] = src.[DepGate]
			, [ArrTerminal] = src.[ArrTerminal]
			, [ArrGate] = src.[ArrGate]
			, [IrregularOperationsJson] = src.[IrregularOperationsJson]
	WHEN NOT MATCHED THEN
		INSERT (
			[FlightStatusId]
			, [FlightId]
			, [PubDepLocal]
			, [PubDepUtc]
			, [PubArrLocal]
			, [PubArrUtc]
			, [FlightPlanPlannedDepLocal]
			, [FlightPlanPlannedDepUtc]
			, [FlightPlanPlannedArrLocal]
			, [FlightPlanPlannedArrUtc]
			, [SchedGateDepLocal]
			, [SchedGateDepUtc]
			, [EstGateDepLocal]
			, [EstGateDepUtc]
			, [ActGateDepLocal]
			, [ActGateDepUtc]
			, [SchedGateArrLocal]
			, [SchedGateArrUtc]
			, [EstGateArrLocal]
			, [EstGateArrUtc]
			, [EstRunwayDepLocal]
			, [EstRunwayDepUtc]
			, [ActRunwayDepLocal]
			, [ActRunwayDepUtc]
			, [EstRunwayArrLocal]
			, [EstRunwayArrUtc]
			, [ActRunwayArrLocal]
			, [ActRunwayArrUtc]
			, [DepGateDelayMins]
			, [DepRunwayDelayMins]
			, [ArrGateDelayMins]
			, [ArrRunwayDelayMins]
			, [SchedBlockMins]
			, [BlockMins]
			, [SchedAirMins]
			, [AirMins]
			, [SchedTaxiOutMins]
			, [TaxiOutMins]
			, [SchedTaxiInMins]
			, [TaxiInMins]
			, [DepTerminal]
			, [DepGate]
			, [ArrTerminal]
			, [ArrGate]
			, [IrregularOperationsJson]
		) VALUES (
			src.[FlightStatusId]
			, src.[FlightId]
			, src.[PubDepLocal]
			, src.[PubDepUtc]
			, src.[PubArrLocal]
			, src.[PubArrUtc]
			, src.[FlightPlanPlannedDepLocal]
			, src.[FlightPlanPlannedDepUtc]
			, src.[FlightPlanPlannedArrLocal]
			, src.[FlightPlanPlannedArrUtc]
			, src.[SchedGateDepLocal]
			, src.[SchedGateDepUtc]
			, src.[EstGateDepLocal]
			, src.[EstGateDepUtc]
			, src.[ActGateDepLocal]
			, src.[ActGateDepUtc]
			, src.[SchedGateArrLocal]
			, src.[SchedGateArrUtc]
			, src.[EstGateArrLocal]
			, src.[EstGateArrUtc]
			, src.[EstRunwayDepLocal]
			, src.[EstRunwayDepUtc]
			, src.[ActRunwayDepLocal]
			, src.[ActRunwayDepUtc]
			, src.[EstRunwayArrLocal]
			, src.[EstRunwayArrUtc]
			, src.[ActRunwayArrLocal]
			, src.[ActRunwayArrUtc]
			, src.[DepGateDelayMins]
			, src.[DepRunwayDelayMins]
			, src.[ArrGateDelayMins]
			, src.[ArrRunwayDelayMins]
			, src.[SchedBlockMins]
			, src.[BlockMins]
			, src.[SchedAirMins]
			, src.[AirMins]
			, src.[SchedTaxiOutMins]
			, src.[TaxiOutMins]
			, src.[SchedTaxiInMins]
			, src.[TaxiInMins]
			, src.[DepTerminal]
			, src.[DepGate]
			, src.[ArrTerminal]
			, src.[ArrGate]
			, src.[IrregularOperationsJson]
		)
	OUTPUT inserted.Id, src.BulkSaveId, $Action
	INTO @OutputHolder([Id], [BulkSaveId], [UpsertAction]);
	SELECT * FROM @OutputHolder
END

GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_FlightStatusUpdate_UpsertBulk')
	exec('CREATE PROCEDURE [dbo].[usp_FlightStatusUpdate_UpsertBulk] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[usp_FlightStatusUpdate_UpsertBulk]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @OutputHolder TABLE ([Id] BIGINT, [BulkSaveId] BIGINT, [UpsertAction] VARCHAR(32));
	MERGE INTO [dbo].[FlightStatusUpdate] trg
	USING (
		SELECT * FROM #BulkSaveFlightStatusUpdate
	) as src
	ON trg.Id = src.Id AND scr.Id > 0
	WHEN MATCHED THEN
		UPDATE SET
			[FlightStatusId] = src.[FlightStatusId]
			, [FlightId] = src.[FlightId]
			, [UpdatedAtLocal] = src.[UpdatedAtLocal]
			, [UpdatedAtUtc] = src.[UpdatedAtUtc]
			, [Source] = src.[Source]
			, [UpdatedTextFieldsJson] = src.[UpdatedTextFieldsJson]
			, [UpdatedDataFieldsJson] = src.[UpdatedDataFieldsJson]
	WHEN NOT MATCHED THEN
		INSERT (
			[FlightStatusId]
			, [FlightId]
			, [UpdatedAtLocal]
			, [UpdatedAtUtc]
			, [Source]
			, [UpdatedTextFieldsJson]
			, [UpdatedDataFieldsJson]
		) VALUES (
			src.[FlightStatusId]
			, src.[FlightId]
			, src.[UpdatedAtLocal]
			, src.[UpdatedAtUtc]
			, src.[Source]
			, src.[UpdatedTextFieldsJson]
			, src.[UpdatedDataFieldsJson]
		)
	OUTPUT inserted.Id, src.BulkSaveId, $Action
	INTO @OutputHolder([Id], [BulkSaveId], [UpsertAction]);
	SELECT * FROM @OutputHolder
END

GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_FlightStatusRequest_UpsertBulk')
	exec('CREATE PROCEDURE [dbo].[usp_FlightStatusRequest_UpsertBulk] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[usp_FlightStatusRequest_UpsertBulk]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @OutputHolder TABLE ([Id] BIGINT, [BulkSaveId] BIGINT, [UpsertAction] VARCHAR(32));
	MERGE INTO [dbo].[FlightStatusRequest] trg
	USING (
		SELECT * FROM #BulkSaveFlightStatusRequest
	) as src
	ON trg.Id = src.Id AND scr.Id > 0
	WHEN MATCHED THEN
		UPDATE SET
			[RequestUri] = src.[RequestUri]
			, [RequestDateUtc] = src.[RequestDateUtc]
			, [ResponseData] = src.[ResponseData]
			, [ProcessedDateUtc] = src.[ProcessedDateUtc]
	WHEN NOT MATCHED THEN
		INSERT (
			[RequestUri]
			, [RequestDateUtc]
			, [ResponseData]
			, [ProcessedDateUtc]
		) VALUES (
			src.[RequestUri]
			, src.[RequestDateUtc]
			, src.[ResponseData]
			, src.[ProcessedDateUtc]
		)
	OUTPUT inserted.Id, src.BulkSaveId, $Action
	INTO @OutputHolder([Id], [BulkSaveId], [UpsertAction]);
	SELECT * FROM @OutputHolder
END

GO


