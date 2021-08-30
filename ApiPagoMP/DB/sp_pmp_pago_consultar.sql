USE [SoftCredito]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC sp_pmp_pago_consultar 
-- ALTER PROC sp_pmp_pago_consultar
	@CLAVE_REFERENCIA VARCHAR(20)
AS

DECLARE @TABLE TABLE (
	cliente VARCHAR(50),
	monto_min DECIMAL(18,2),
	monto_max DECIMAL(18,2)
)

INSERT INTO @TABLE VALUES('Juan Francisco Flores García', 150,10000)

SELECT cliente, monto_min, monto_max FROM @TABLE;