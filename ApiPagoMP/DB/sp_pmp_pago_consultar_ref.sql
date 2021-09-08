USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_consultar]    Script Date: 07/09/2021 10:58:42 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- CREATE PROC sp_pmp_pago_consultar_ref 
ALTER PROC [dbo].[sp_pmp_pago_consultar_ref]
	@COMERCIOID VARCHAR(20)
AS

SELECT id, 
	comercio, 
	usuario, 
	contrasena, 
	estatus 
FROM pagomp_comercios 
WHERE comercio = @COMERCIOID AND 
estatus = 1