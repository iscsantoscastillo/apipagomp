USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_comercio_con]    Script Date: 24/09/2021 05:34:46 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- CREATE PROC [dbo].[sp_pmp_pago_comercio_con] 
ALTER PROC [dbo].[sp_pmp_pago_comercio_con]
	@COMERCIO VARCHAR(12)
AS

SELECT 
	descripcion
	,caja_mp
	,referencia_mp
	,cve_forma_pago_mp
	,plataforma_mp
	,sucursal_mp
FROM pagomp_comercios 
WHERE comercio = @COMERCIO  