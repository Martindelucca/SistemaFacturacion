USE [FacturacionDB]
GO
/****** Object:  Table [dbo].[T_ARTICULOS]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_ARTICULOS](
	[id_articulo] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](100) NOT NULL,
	[precio_unitario] [decimal](10, 2) NOT NULL,
	[stock] [int] NULL,
	[activo] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_articulo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[T_DETALLES_FACTURA]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_DETALLES_FACTURA](
	[id_detalle] [int] IDENTITY(1,1) NOT NULL,
	[nro_factura] [int] NOT NULL,
	[id_articulo] [int] NOT NULL,
	[cantidad] [int] NOT NULL,
	[precio_unitario] [decimal](10, 2) NOT NULL,
	[subtotal]  AS ([cantidad]*[precio_unitario]),
PRIMARY KEY CLUSTERED 
(
	[id_detalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[T_FACTURAS]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_FACTURAS](
	[nro_factura] [int] IDENTITY(1,1) NOT NULL,
	[fecha] [datetime] NOT NULL,
	[id_forma_pago] [int] NOT NULL,
	[cliente] [nvarchar](100) NOT NULL,
	[total] [decimal](12, 2) NULL,
	[activa] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[nro_factura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[T_FORMAS_PAGO]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_FORMAS_PAGO](
	[id_forma_pago] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_forma_pago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[T_ARTICULOS] ADD  DEFAULT ((0)) FOR [stock]
GO
ALTER TABLE [dbo].[T_ARTICULOS] ADD  DEFAULT ((1)) FOR [activo]
GO
ALTER TABLE [dbo].[T_FACTURAS] ADD  DEFAULT (getdate()) FOR [fecha]
GO
ALTER TABLE [dbo].[T_FACTURAS] ADD  DEFAULT ((0)) FOR [total]
GO
ALTER TABLE [dbo].[T_FACTURAS] ADD  DEFAULT ((1)) FOR [activa]
GO
ALTER TABLE [dbo].[T_DETALLES_FACTURA]  WITH CHECK ADD FOREIGN KEY([id_articulo])
REFERENCES [dbo].[T_ARTICULOS] ([id_articulo])
GO
ALTER TABLE [dbo].[T_DETALLES_FACTURA]  WITH CHECK ADD FOREIGN KEY([nro_factura])
REFERENCES [dbo].[T_FACTURAS] ([nro_factura])
GO
ALTER TABLE [dbo].[T_FACTURAS]  WITH CHECK ADD FOREIGN KEY([id_forma_pago])
REFERENCES [dbo].[T_FORMAS_PAGO] ([id_forma_pago])
GO
/****** Object:  StoredProcedure [dbo].[SP_ACTUALIZAR_FACTURA]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para actualizar factura
CREATE   PROCEDURE [dbo].[SP_ACTUALIZAR_FACTURA]
    @nro_factura INT,
    @fecha DATETIME,
    @id_forma_pago INT,
    @cliente NVARCHAR(100)
AS
BEGIN
    BEGIN TRY
        UPDATE T_FACTURAS 
        SET fecha = @fecha,
            id_forma_pago = @id_forma_pago,
            cliente = @cliente
        WHERE nro_factura = @nro_factura AND activa = 1
        
        SELECT @@ROWCOUNT as FilasAfectadas
    END TRY
    BEGIN CATCH
        THROW
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_ELIMINAR_FACTURA]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para eliminar factura (eliminación lógica)
CREATE   PROCEDURE [dbo].[SP_ELIMINAR_FACTURA]
    @nro_factura INT
AS
BEGIN
    BEGIN TRY
        UPDATE T_FACTURAS 
        SET activa = 0
        WHERE nro_factura = @nro_factura
        
        SELECT @@ROWCOUNT as FilasAfectadas
    END TRY
    BEGIN CATCH
        THROW
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_INSERTAR_DETALLE_FACTURA]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para insertar detalle de factura
CREATE   PROCEDURE [dbo].[SP_INSERTAR_DETALLE_FACTURA]
    @nro_factura INT,
    @id_articulo INT,
    @cantidad INT,
    @precio_unitario DECIMAL(10,2)
AS
BEGIN
    BEGIN TRY
        -- Verificar si ya existe el artículo en la factura
        IF EXISTS (SELECT 1 FROM T_DETALLES_FACTURA 
                  WHERE nro_factura = @nro_factura AND id_articulo = @id_articulo)
        BEGIN
            -- Actualizar cantidad existente
            UPDATE T_DETALLES_FACTURA 
            SET cantidad = cantidad + @cantidad,
                precio_unitario = @precio_unitario
            WHERE nro_factura = @nro_factura AND id_articulo = @id_articulo
        END
        ELSE
        BEGIN
            -- Insertar nuevo detalle
            INSERT INTO T_DETALLES_FACTURA (nro_factura, id_articulo, cantidad, precio_unitario)
            VALUES (@nro_factura, @id_articulo, @cantidad, @precio_unitario)
        END
        
        -- Actualizar total de la factura
        UPDATE T_FACTURAS 
        SET total = (
            SELECT SUM(subtotal) 
            FROM T_DETALLES_FACTURA 
            WHERE nro_factura = @nro_factura
        )
        WHERE nro_factura = @nro_factura
        
    END TRY
    BEGIN CATCH
        THROW
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_INSERTAR_FACTURA]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS - DML
-- =============================================

-- SP para insertar factura (maestro)
CREATE   PROCEDURE [dbo].[SP_INSERTAR_FACTURA]
    @fecha DATETIME,
    @id_forma_pago INT,
    @cliente NVARCHAR(100),
    @nro_factura INT OUTPUT
AS
BEGIN
    BEGIN TRY
        INSERT INTO T_FACTURAS (fecha, id_forma_pago, cliente)
        VALUES (@fecha, @id_forma_pago, @cliente)
        
        SET @nro_factura = SCOPE_IDENTITY()
    END TRY
    BEGIN CATCH
        SET @nro_factura = -1
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_ARTICULO_POR_ID]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para obtener artículo por ID
CREATE   PROCEDURE [dbo].[SP_OBTENER_ARTICULO_POR_ID]
    @id_articulo INT
AS
BEGIN
    SELECT id_articulo, nombre, precio_unitario, stock, activo
    FROM T_ARTICULOS 
    WHERE id_articulo = @id_articulo AND activo = 1
END
GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_ARTICULOS]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para obtener todos los artículos
CREATE   PROCEDURE [dbo].[SP_OBTENER_ARTICULOS]
AS
BEGIN
    SELECT id_articulo, nombre, precio_unitario, stock, activo
    FROM T_ARTICULOS 
    WHERE activo = 1
    ORDER BY nombre
END
GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_FACTURA_COMPLETA]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para obtener factura completa por número
CREATE   PROCEDURE [dbo].[SP_OBTENER_FACTURA_COMPLETA]
    @nro_factura INT
AS
BEGIN
    -- Datos de la factura
    SELECT f.nro_factura, f.fecha, f.cliente, f.total, f.id_forma_pago,
           fp.nombre as forma_pago
    FROM T_FACTURAS f
    INNER JOIN T_FORMAS_PAGO fp ON f.id_forma_pago = fp.id_forma_pago
    WHERE f.nro_factura = @nro_factura AND f.activa = 1
    
    -- Detalles de la factura
    SELECT df.id_detalle, df.id_articulo, df.cantidad, df.precio_unitario, df.subtotal,
           a.nombre as nombre_articulo
    FROM T_DETALLES_FACTURA df
    INNER JOIN T_ARTICULOS a ON df.id_articulo = a.id_articulo
    WHERE df.nro_factura = @nro_factura
    ORDER BY df.id_detalle
END
GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_FACTURAS]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para obtener todas las facturas
CREATE   PROCEDURE [dbo].[SP_OBTENER_FACTURAS]
AS
BEGIN
    SELECT f.nro_factura, f.fecha, f.cliente, f.total,
           fp.nombre as forma_pago
    FROM T_FACTURAS f
    INNER JOIN T_FORMAS_PAGO fp ON f.id_forma_pago = fp.id_forma_pago
    WHERE f.activa = 1
    ORDER BY f.fecha DESC
END
GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_FORMAS_PAGO]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS - CONSULTAS
-- =============================================

-- SP para obtener todas las formas de pago
CREATE   PROCEDURE [dbo].[SP_OBTENER_FORMAS_PAGO]
AS
BEGIN
    SELECT id_forma_pago, nombre 
    FROM T_FORMAS_PAGO 
    ORDER BY nombre
END
GO
/****** Object:  StoredProcedure [dbo].[SP_VERIFICAR_DETALLE_EXISTENTE]    Script Date: 9/9/2025 12:04:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP para verificar existencia de detalle específico
CREATE   PROCEDURE [dbo].[SP_VERIFICAR_DETALLE_EXISTENTE]
    @nro_factura INT,
    @id_articulo INT
AS
BEGIN
    SELECT COUNT(*) as Existe
    FROM T_DETALLES_FACTURA 
    WHERE nro_factura = @nro_factura AND id_articulo = @id_articulo
END
GO
