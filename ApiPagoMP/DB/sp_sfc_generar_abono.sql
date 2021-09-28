USE [SoftCredito]
GO
/****** Object:  StoredProcedure [dbo].[sp_sfc_generar_abono]    Script Date: 27/09/2021 02:07:40 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_sfc_generar_abono]
	--Parametros
	@clave_solicitud nvarchar(20),
	@cve_sucursal nvarchar(4),
	@cve_vendedor nvarchar(10),
	@caja nvarchar(6),
	@total_pagado float,
	@cve_forma_pago nvarchar(4) = '',
	@folio_recibo nvarchar(14) = '',
	@referencia nvarchar(max) = '',
	@fecha_envio datetime = NULL,
	@fg_envio_softcredito smallint = 0,
   @id_abono int = -1 out
AS 
BEGIN 
	--Variables
	DECLARE @id_registro AS INTEGER = 0 --jesus varguez
	DECLARE @msg as NVARCHAR(500) = ''
	DECLARE @cve_contrato varchar(20)
	DECLARE @cve_cliente varchar(20)
	DECLARE @centro_sap nvarchar(4)
	DECLARE @capital_pagado float = 0
	DECLARE @interes_pagado float = 0
	DECLARE @plazo int = 0
	DECLARE @fecha_actual datetime = GETDATE()
	DECLARE @fecha_abono datetime 
	DECLARE @diferencia int = 0
	DECLARE @abono_registrado float = 0
	DECLARE @canal_pago nvarchar(10) = 'SUCURSAL'
	DECLARE @nb_vendedor nvarchar(50)
   DECLARE @cve_tienda nvarchar(50)   = @cve_sucursal
   DECLARE @cve_asociado nvarchar(50) = @cve_vendedor
	DECLARE @fg_requiere_corte int = 0
   DECLARE @fg_esparcial int = 1 
   DECLARE @fg_abono_consola int = 1 

	DECLARE @tbl_amortizacion table(
		clave_solicitud varchar(20)
		,capital_pagado float
		,interes_pagado float
		,total_abonado float
		,amortizaciones_pagadas int
	)

	/******Validaciones*******/
   Select @cve_contrato = cve_contrato
        , @cve_cliente = cve_cliente
     FROM sfc_ventas_softcredito 
	 WHERE vs_status = 1 
      AND clave_solicitud = @clave_solicitud	
   
   IF isnull(@cve_contrato, '') = ''
	BEGIN
		SET @msg= 'La solicitud de crédito no éxiste.'
	END
  
	--IF NOT EXISTS(
	--	SELECT
	--		1
	--	FROM sfc_ventas_softcredito 
	--	WHERE 
	--		vs_status = 1 AND
	--		clave_solicitud = @clave_solicitud	
	--) AND @msg = ''
	--BEGIN
	--	SET @msg= 'La solicitud de crédito no éxiste.'
	--END	

   ------- VALIDACION CASHDRO --------------------
   IF @cve_asociado = '9999999999'
   BEGIN
      SET @cve_tienda = '7003'
   END
   ----------------------------------------------

	IF EXISTS(
		SELECT
			1 
		FROM sfc_tipo_financiamiento
		WHERE 
			estatus = 1 AND
			tf_cve_sucursal = @cve_tienda
	)
	BEGIN
		SELECT
			  @canal_pago = tf_canal
			, @centro_sap = tf_cve_sucursal
			, @nb_vendedor = 'CONTROL PAGOS'
         , @cve_tienda  = tf_cve_sucursal
		FROM sfc_tipo_financiamiento
		WHERE 
			estatus = 1 AND
			tf_cve_sucursal = @cve_tienda
	END

	IF @canal_pago = 'SUCURSAL' 
	BEGIN

	--Validación corte--
   Select @fg_abono_consola = ISNULL(fg_abonos_consola_macropay, 1)
     from Corporativo.dbo.gm_cat_tiendas_configuracion
    where cve_tienda = @cve_sucursal 

      IF @fg_abono_consola  = 1 AND 
         @canal_pago = 'SUCURSAL' AND 
         ((@cve_vendedor <> '0000000000' AND @caja <> '000004') OR (@cve_vendedor = '0000000000' AND @caja =  '000004'))
	   BEGIN
		   SET @fg_requiere_corte = 1
	   END

	--IF EXISTS(
	--	SELECT
	--		1
	--	FROM Corporativo..gm_cat_tiendas ti with(nolock)
	--	WHERE 
	--		ti.cve_tienda = @cve_sucursal AND
	--		(
	--			ti.fg_abonos_consola_macropay = 1
	--		)
	--) AND @canal_pago = 'SUCURSAL' AND ((@cve_vendedor <> '0000000000' AND @caja <> '000004') OR (@cve_vendedor = '0000000000' AND @caja =  '000004'))
	--BEGIN
	--	SET @fg_requiere_corte = 1
	--END

      SELECT @fecha_abono = max(as_fecha_alta)
  	     FROM sfc_abonos_softcredito 
	    WHERE clave_solicitud = @clave_solicitud

		SET @diferencia = DATEDIFF(SECOND, convert(datetime, @fecha_abono, 20), @fecha_actual)
		IF @diferencia < 30
		BEGIN
			SET @msg= 'Ya se ha registrado un abono de la solicitud ' + @clave_solicitud
		END
  

		--SELECT TOP 1 
		--	@abono_registrado = as_total_pagado,
		--	@fecha_abono = as_fecha_alta 
		--FROM sfc_abonos_softcredito 
		--WHERE clave_solicitud = @clave_solicitud
		--ORDER BY as_fecha_alta DESC

		--SELECT @diferencia = DATEDIFF(SECOND, convert(datetime, @fecha_abono, 20), @fecha_actual)
		--IF @diferencia < 30
		--BEGIN
		--	SET @msg= 'Ya se ha registrado un abono de ' + ' $ ' + CONVERT(nvarchar, @abono_registrado) + '.00 de la solicitud ' + @clave_solicitud
		--END

	END


	/*Algoritmo*/
	--SELECT --TOP 1 
	--	@cve_contrato = Cve_Contrato,
	--	@cve_cliente = Cve_cliente
	--FROM SoftCredito..sfc_ventas_softcredito 
	--WHERE 
	--	vs_status = 1 AND
	--	clave_solicitud = @clave_solicitud	

	IF @canal_pago = 'SUCURSAL'
	BEGIN
      -- SUCURSAL ANTERIOR -----------------------------------------------
--      IF EXISTS(SELECT 1 
--                  FROM GRUPOMACRO.dbo.opc_SUCURSAL
--                 WHERE Sc_Cve_Sucursal = @cve_sucursal)
      
      IF EXISTS(SELECT 1 
                  FROM Corporativo.dbo.gm_cat_tiendas
		           WHERE centro_sap = @cve_sucursal and @caja = '000004') -- interfaz abonos sap
      BEGIN
         -- si existe el centro sap 
		   SELECT TOP 1
		   	 @centro_sap = centro_sap
           , @cve_tienda = cve_tienda
		   FROM Corporativo.dbo.gm_cat_tiendas
		   WHERE centro_sap = @cve_sucursal

         SET @cve_asociado = CONCAT(@centro_sap, '-', right(concat('000', @cve_vendedor), 3))

      END
      ELSE IF EXISTS(SELECT 1 
                  FROM Corporativo.dbo.gm_cat_tiendas
		           WHERE cve_tienda = @cve_sucursal) -- si se esta mandando tienda
      BEGIN
         SELECT TOP 1
		          @centro_sap  = centro_sap
              , @cve_tienda = cve_tienda
		     FROM Corporativo.dbo.gm_cat_tiendas
		    WHERE cve_tienda = @cve_sucursal
      END
      ELSE BEGIN -- si se esta mandando un centro sap

		   SELECT TOP 1
		   	 @centro_sap = centro_sap
           , @cve_tienda = cve_tienda
		   FROM Corporativo.dbo.gm_cat_tiendas
		   WHERE centro_sap = @cve_sucursal
      END


      IF ISNULL(@centro_sap, '') = ''
      BEGIN
         SET @centro_sap = CONCAT('0000',@centro_sap) 
         SET @cve_tienda = @cve_sucursal
      END

      -- EMPLEADO ANTERIOR -----------------------------------------------
      IF EXISTS(SELECT 1 
                  FROM GRUPOMACRO.dbo.vendedor
		     WHERE Vn_Cve_Vendedor = @cve_asociado)
      BEGIN
         SELECT TOP 1
		          @nb_vendedor = Vn_Descripcion
		     FROM GRUPOMACRO.dbo.vendedor
		     WHERE Vn_Cve_Vendedor = @cve_asociado
      END
	  ------HP------
	  ELSE IF EXISTS(
			SELECT 1 
                  FROM Corporativo.dbo.gm_tc_usuarios
		     WHERE nombre_usuario = @cve_asociado
	  )
	  BEGIN
	           SELECT TOP 1
		          @nb_vendedor = pe.nombre_completo
		     FROM Corporativo.dbo.gm_tc_usuarios us
			 INNER JOIN Corporativo..gm_tc_personas pe ON pe.id_persona = us.id_persona
		     WHERE us.nombre_usuario = @cve_asociado
	  END
	  --------------
      ELSE BEGIN 
		   SELECT TOP 1
		         @nb_vendedor = nombre
		   FROM Corporativo.dbo.gm_vendedores_sap
		   WHERE id_pos = @cve_asociado

         IF ISNULL(@nb_vendedor, '') = ''
         BEGIN
            SET @nb_vendedor = 'VENDEDOR POS SAP'
         END

      END
	END

   ------- VALIDACION CASHDRO --------------------
   IF @cve_asociado = '9999999999'
   BEGIN
      SET @nb_vendedor = CONCAT('VENDEDOR ', ISNULL(@canal_pago, 'NA'))
   END
   ----------------------------------------------

   ------- VALIDACION REFERENCIA UNICA ---------------------
   IF @referencia = 'RUCNTA' or @referencia = 'RUOPAY'  or @referencia = 'RUWILLYS' 
   BEGIN
      SET @fg_esparcial = 2
   END

	IF @msg = ''
	BEGIN
      -- insertamos la amortizacion [2021-07-06]
      INSERT @tbl_amortizacion execute sp_sfc_agregar_amortizacion @clave_solicitud, @total_pagado, @fg_esparcial

   	-- refrescamos el saldo
      SELECT TOP 1
   		@capital_pagado = ta.capital_pagado
   		,@interes_pagado = ta.interes_pagado
   		,@plazo = ta.amortizaciones_pagadas
   	FROM @tbl_amortizacion ta

      -- aplicamos el abono
		INSERT INTO sfc_abonos_softcredito
		(
			clave_solicitud
			,cve_contrato
			,cve_cliente
			,as_fecha_abono
			,as_vendedor
			,as_nombre_vendedor
			,sc_cve_sucursal
			,centro_sap
			,as_capital_pagado
			,as_interes_pagado
			,as_total_pagado
			,as_plazos_pagados
			,as_canal_pago
			,as_forma_pago
			,as_recibo_pago
			,as_status
			,as_usuario_alta
			,as_fecha_alta
			,as_referencia_pago
			,as_fecha_envio_softcredito
			,as_enviado_softcredito
			, fg_requiere_corte
		) VALUES (
			@clave_solicitud,
			@cve_contrato,
			@cve_cliente,
			GETDATE(),
			@cve_asociado,
			@nb_vendedor,
			@cve_tienda, --@cve_sucursal, se sustituye para recuperar la tienda del catalogo
			@centro_sap,
			@capital_pagado,
			@interes_pagado,
			@total_pagado,
			@plazo,
			@canal_pago, --SUCURSAL, OXXO, OPENPAY, TRANSFERENCIA
			@cve_forma_pago,
			@folio_recibo,
			1, -- 0 Cancelado, 1 Activo
			@caja,
			GETDATE(),
			@referencia,
			@fecha_envio,
			@fg_envio_softcredito,
			@fg_requiere_corte
		)

      IF @@ROWCOUNT <> 1
      BEGIN
         SET @msg = CONCAT('ERROR. No se pudo agregar el abono de la solicitud "',@clave_solicitud, '".');
         THROW 51001, @msg, 1;  
      END

		SET @id_registro= SCOPE_IDENTITY()

		SELECT  @id_registro AS id_registro 

		SET  @id_abono = @id_registro 
		
		return @id_registro
	END
	ELSE
	BEGIN
		RAISERROR(@msg,11,1)WITH SETERROR
	END
END




select * from sfc_abonos_softcredito where clave_solicitud = 'SL202100000010' order by as_fecha_abono desc