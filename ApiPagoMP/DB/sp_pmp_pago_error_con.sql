USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_pmp_pago_error_con]    Script Date: 07/09/2021 10:35:47 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_pmp_pago_error_con] 
-- ALTER PROC sp_pmp_pago_error_con
	@CODIGO VARCHAR(4)
AS

SELECT codigo, 
	descripcion 
FROM pagomp_errores 
WHERE codigo = @CODIGO  