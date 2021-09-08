USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_bitacora_ins]    Script Date: 07/09/2021 09:36:26 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_pmp_pago_bitacora_ins] 
-- ALTER PROC sp_pmp_pago_bitacora_ins
	@JSON TEXT,
	@USUARIOALTA VARCHAR(30)
AS

INSERT INTO pagomp_bitacora
	(json, 
	fecha_alta,
	estatus,
	usuario_alta) 
VALUES
	(@JSON, 
	GETDATE(),
	1,
	@USUARIOALTA)