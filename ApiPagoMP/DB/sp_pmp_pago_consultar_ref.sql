USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_consultar_ref]    Script Date: 24/09/2021 06:28:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- CREATE PROC sp_pmp_pago_consultar_ref 
ALTER PROC [dbo].[sp_pmp_pago_consultar_ref]
	@COMERCIOID VARCHAR(20)
AS

SELECT id 
	,comercio 
	,usuario 
	,contrasena 
	,estatus
	,caja_mp
	,referencia_mp
	,cve_forma_pago_mp
	,plataforma_mp
	,sucursal_mp
FROM pagomp_comercios 
WHERE comercio = @COMERCIOID AND 
estatus = 1