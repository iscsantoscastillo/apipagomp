USE [SoftCredito]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- CREATE PROC [dbo].[sp_pmp_pago_notipago_update] 
ALTER PROC sp_pmp_pago_notipago_update
	@ID_PAGO INT,
	@AS_ID_ABONO BIGINT,
	@USUARIO_ENV_SC VARCHAR(30),
	@FILAS_AFECTADAS INT OUT
AS

UPDATE pagomp_pago
	SET 
	enviado_softcredito = 1,
	fecha_enviado_softcredito = GETDATE(),
	as_id_abono = @AS_ID_ABONO,
	usuario_enviado_softcredito = @USUARIO_ENV_SC
WHERE id = @ID_PAGO

SET @FILAS_AFECTADAS = @@ROWCOUNT