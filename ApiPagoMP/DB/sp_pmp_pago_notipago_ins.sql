USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_notipago_ins]    Script Date: 07/09/2021 09:07:37 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_pmp_pago_notipago_ins] 
-- ALTER PROC sp_pmp_pago_notipago_ins
	@REFERENCIA VARCHAR(20),
	@COMERCIO VARCHAR(12),
	@MONTOPAGO DECIMAL(18,2),
	@TRANSACCION VARCHAR(20),
	@FECHAHORATRX DATETIME,
	@CLAVESUCURSAL VARCHAR(30),
	@USUARIOALTA VARCHAR(30)
AS

INSERT INTO pagomp_pago
	(referencia, 
	comercio, 
	montopago, 
	transaccion, 
	fecha_hora_transaccion, 
	clave_sucursal, 
	fecha_alta,
	estatus,
	usuario_alta) 
VALUES
	(@REFERENCIA, 
	@COMERCIO, 
	@MONTOPAGO, 
	@TRANSACCION, 
	@FECHAHORATRX, 
	@CLAVESUCURSAL, 
	GETDATE(),
	1,
	@USUARIOALTA)