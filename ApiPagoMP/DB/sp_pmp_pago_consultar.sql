USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_consultar]    Script Date: 08/09/2021 12:47:16 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_pmp_pago_consultar] 
-- ALTER PROC sp_pmp_pago_consultar
	@CLAVE_REFERENCIA VARCHAR(20)
AS

SELECT referencia, 
	monto_min, 
	monto_max, 
	estatus, 
	cliente 
FROM pagomp_referencias 
WHERE referencia = @CLAVE_REFERENCIA AND 
	estatus = 1

--SELECT 
--	referencia
--	,monto_minimo AS monto_min
--	,monto_maximo AS monto_max
--	,'Juan Francisco Flores García' as cliente
--FROM mpf_ventas_referencia_unica 
--WHERE 
--	referencia = @CLAVE_REFERENCIA
--	AND plataforma ='WILLYS'
--	AND estatus = 1