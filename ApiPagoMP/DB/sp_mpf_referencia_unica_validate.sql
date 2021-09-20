USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_mpf_referencia_unica_validate]    Script Date: 17/09/2021 11:04:18 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		pescalante
-- Create date: 26/08/2021
-- Description:	metodo para mostrar ventas_referencias_unicas
-- =============================================
--CREATE PROCEDURE [dbo].[sp_mpf_referencia_unica_validate]
ALTER PROCEDURE [dbo].[sp_mpf_referencia_unica_validate]
     @plataforma       varchar(64)
    ,@referencia       varchar(64)
    ,@monto_referencia float = 0.0
AS
BEGIN
   SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

   DECLARE @mensaje        VARCHAR(512) 
   DECLARE @estatus        INT = -1
   DECLARE @saldo          FLOAT
   DECLARE @pago_semanal   FLOAT
   DECLARE @pago_minimo    FLOAT
   DECLARE @pago_maximo    FLOAT
   DECLARE @cve_solicitud  VARCHAR(24) 
   DECLARE @nombre_cliente VARCHAR(512) 
   DECLARE @pagable        INT

   IF ISNULL(@referencia, '') <> '' AND ISNULL(@plataforma, '') <> ''
   BEGIN 
	   SELECT @cve_solicitud = referencia.cve_solicitud
           , @pago_minimo   = referencia.monto_minimo
           , @pago_minimo   = referencia.monto_minimo
           , @estatus       = saldo.estatus
           , @saldo         = saldo.pago_para_liquidar
           , @pago_semanal  = saldo.pago_semanal
	     FROM dbo.mpf_ventas_referencia_unica referencia
        JOIN dbo.vw_mpf_saldos_credito saldo on saldo.clave_solicitud = referencia.cve_solicitud
       WHERE referencia.estatus = 1
         AND referencia.vigencia >= CONVERT(DATE, GETDATE())
         AND referencia.referencia = @referencia
         AND referencia.plataforma = @plataforma
       ORDER BY fecha_alta DESC

   END

   IF ISNULL(@cve_solicitud, '') = ''
        SET @pagable = 0
   ELSE IF @estatus = 0
        SET @pagable = 0
   ELSE BEGIN
      SET @pagable = 1
      
      IF @plataforma = 'WILLYS'
      BEGIN
         select @nombre_cliente = Corporativo.dbo.udf_gm_concatenar_nombre(cto.primer_nombre, cto.segundo_nombre, cto.apellido_paterno, cto.apellido_paterno) 
           from mpf_solicitud_macropay sol
           join mpf_solicitud_macropay_contrato cto on cto.cve_contrato = sol.cve_contrato
           where sol.cve_solicitud = @cve_solicitud
      END

   END

   IF @monto_referencia <> 0 
   BEGIN
      IF @monto_referencia > @saldo 
         SET @pagable = 0
      ELSE IF @monto_referencia < @pago_minimo
         SET @pagable = 0
      ELSE IF @monto_referencia > @pago_maximo
         SET @pagable = 0

      SELECT @pagable AS payable
   END
   ELSE 
   BEGIN
      IF @plataforma <> 'WILLYS'
         SELECT @pagable AS payable
              , CAST(ISNULL(CASE WHEN @saldo >= @pago_minimo THEN @pago_minimo ELSE @saldo END, 0.0) AS float) AS min_amount
              , CAST(ISNULL(CASE WHEN @saldo >= @pago_maximo THEN @pago_maximo ELSE @saldo END, 0.0) AS float) AS max_amount
      ELSE
         SELECT @pagable AS payable
              , CAST(ISNULL(CASE WHEN @saldo >= @pago_minimo THEN @pago_minimo ELSE @saldo END, 0.0) AS float) AS min_amount
              , CAST(ISNULL(CASE WHEN @saldo >= @pago_maximo THEN @pago_maximo ELSE @saldo END, 0.0) AS float) AS max_amount
              , CAST(ISNULL(@nombre_cliente, 'N/E') AS varchar(512)) AS cliente
   END

END 
