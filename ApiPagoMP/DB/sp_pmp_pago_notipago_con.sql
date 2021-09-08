USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_notipago_con]    Script Date: 07/09/2021 09:10:38 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_pmp_pago_notipago_con] 
-- ALTER PROC sp_pmp_pago_notipago_con
	@REFERENCIA VARCHAR(20),
	@COMERCIO VARCHAR(12)
AS

SELECT id 
	FROM pagomp_pago 
	WHERE referencia = @REFERENCIA 
	AND comercio = @COMERCIO
	AND estatus = 1