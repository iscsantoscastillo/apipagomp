USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_notipago_con_trx]    Script Date: 27/09/2021 02:05:38 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_pmp_pago_notipago_con_trx] 
-- ALTER PROC sp_pmp_pago_notipago_con_trx
	@TRANSACCION VARCHAR(20),
	@COMERCIO VARCHAR(12),
	@CLAVE_SUCURSAL VARCHAR(30),
	@REFERENCIA VARCHAR(20)
AS

SELECT id 
	FROM pagomp_pago 
	WHERE transaccion = @TRANSACCION 
	AND comercio = @COMERCIO
	AND clave_sucursal = @CLAVE_SUCURSAL
	AND referencia = @REFERENCIA
	AND estatus = 1