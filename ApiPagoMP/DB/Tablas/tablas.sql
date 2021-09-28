USE [SoftCredito]
GO

/****** Object:  Table [dbo].[pagomp_bitacora]    Script Date: 27/09/2021 04:16:27 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[pagomp_bitacora](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[json] [text] NULL,
	[fecha_alta] [datetime] NULL,
	[estatus] [int] NULL,
	[usuario_alta] [varchar](30) NULL,
 CONSTRAINT [PK_pagomp_bitacora] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- ============================================================================================================================================

USE [SoftCredito]
GO

/****** Object:  Table [dbo].[pagomp_comercios]    Script Date: 27/09/2021 04:17:30 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[pagomp_comercios](
	[id] [int] NOT NULL,
	[comercio] [varchar](12) NULL,
	[usuario] [varchar](50) NULL,
	[contrasena] [varchar](50) NULL,
	[estatus] [int] NULL,
	[fecha_alta] [datetime] NULL,
	[usuario_alta] [varchar](30) NULL,
	[descripcion] [varchar](50) NULL,
	[caja_mp] [varchar](10) NULL,
	[referencia_mp] [varchar](10) NULL,
	[cve_forma_pago_mp] [varchar](10) NULL,
	[plataforma_mp] [varchar](10) NULL,
	[sucursal_mp] [varchar](10) NULL,
 CONSTRAINT [PK_pagomp_comercios] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ============================================================================================================================================

USE [SoftCredito]
GO

/****** Object:  Table [dbo].[pagomp_errores]    Script Date: 27/09/2021 04:18:02 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[pagomp_errores](
	[codigo] [varchar](4) NULL,
	[descripcion] [varchar](50) NULL
) ON [PRIMARY]
GO

--codigo	descripcion
--01	Referencia inválida.
--02	Error general
--03	Comercio no autorizado
--04	Servicio en mantenimiento
--05	Error no definido
--06	Método inválido
-- ============================================================================================================================================

USE [SoftCredito]
GO

/****** Object:  Table [dbo].[pagomp_pago]    Script Date: 27/09/2021 04:19:37 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[pagomp_pago](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[referencia] [varchar](20) NULL,
	[comercio] [varchar](12) NULL,
	[montopago] [decimal](18, 2) NULL,
	[transaccion] [varchar](20) NULL,
	[clave_sucursal] [varchar](30) NULL,
	[fecha_alta] [datetime] NULL,
	[estatus] [int] NULL,
	[usuario_alta] [varchar](30) NULL,
	[enviado_softcredito] [smallint] NULL,
	[fecha_enviado_softcredito] [datetime] NULL,
	[as_id_abono] [bigint] NULL,
	[fecha_unix] [bigint] NULL,
	[usuario_enviado_softcredito] [varchar](30) NULL,
 CONSTRAINT [PK_pagomp_notipago] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_fecha_enviado_softcredito]  DEFAULT ((0)) FOR [enviado_softcredito]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_fecha_enviado_softcredito_1]  DEFAULT (NULL) FOR [fecha_enviado_softcredito]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_as_id_abono]  DEFAULT (NULL) FOR [as_id_abono]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_usuario_enviado_softcredito]  DEFAULT (NULL) FOR [usuario_enviado_softcredito]
GO
-- ============================================================================================================================================
USE [SoftCredito]
GO

/****** Object:  Table [dbo].[pagomp_pago]    Script Date: 27/09/2021 04:19:37 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[pagomp_pago](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[referencia] [varchar](20) NULL,
	[comercio] [varchar](12) NULL,
	[montopago] [decimal](18, 2) NULL,
	[transaccion] [varchar](20) NULL,
	[clave_sucursal] [varchar](30) NULL,
	[fecha_alta] [datetime] NULL,
	[estatus] [int] NULL,
	[usuario_alta] [varchar](30) NULL,
	[enviado_softcredito] [smallint] NULL,
	[fecha_enviado_softcredito] [datetime] NULL,
	[as_id_abono] [bigint] NULL,
	[fecha_unix] [bigint] NULL,
	[usuario_enviado_softcredito] [varchar](30) NULL,
 CONSTRAINT [PK_pagomp_notipago] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_fecha_enviado_softcredito]  DEFAULT ((0)) FOR [enviado_softcredito]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_fecha_enviado_softcredito_1]  DEFAULT (NULL) FOR [fecha_enviado_softcredito]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_as_id_abono]  DEFAULT (NULL) FOR [as_id_abono]
GO

ALTER TABLE [dbo].[pagomp_pago] ADD  CONSTRAINT [DF_pagomp_pago_usuario_enviado_softcredito]  DEFAULT (NULL) FOR [usuario_enviado_softcredito]
GO

