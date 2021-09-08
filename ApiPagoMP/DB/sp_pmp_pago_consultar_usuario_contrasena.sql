USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_consultar_usuario_contrasena]    Script Date: 07/09/2021 10:28:12 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- CREATE PROC sp_pmp_pago_consultar_usuario_contrasena 
ALTER PROC [dbo].[sp_pmp_pago_consultar_usuario_contrasena]
	@USUARIO VARCHAR(50),
	@CONTRASENA VARCHAR(50)
AS

SELECT id, 
	comercio, 
	usuario, 
	contrasena, 
	estatus 
FROM pagomp_comercios 
WHERE usuario = @USUARIO AND 
contrasena = @CONTRASENA