USE [SoftCredito]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- CREATE PROC [dbo].[sp_pmp_pago_comercio_con] 
ALTER PROC sp_pmp_pago_comercio_con
	@COMERCIO VARCHAR(12)
AS

SELECT 
	descripcion 
FROM pagomp_comercios 
WHERE comercio = @COMERCIO