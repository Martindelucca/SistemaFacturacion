using Practica01.Servicios;
using System;
using System.Collections.Generic;
using Practica01.Dominio;
using Practica01.Servicios;

namespace Practica01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🧾 SISTEMA DE FACTURACIÓN - PRUEBA COMPLETA");
            Console.WriteLine("===========================================\n");

            try
            {
                // Crear instancias de servicios
                var articuloService = new ArticuloService();
                var formaPagoService = new FormaPagoService();
                var facturaService = new FacturaService();
                var manager = new FacturacionManager();

                Console.WriteLine("✅ Servicios creados exitosamente\n");

                // ====================================
                // PRUEBA 1: OBTENER ARTÍCULOS
                // ====================================
                Console.WriteLine("📦 PRUEBA 1: Obteniendo artículos...");
                var articulos = articuloService.ObtenerArticulos();

                Console.WriteLine($"   Artículos encontrados: {articulos.Count}");
                foreach (var articulo in articulos.Take(3)) // Solo los primeros 3
                {
                    Console.WriteLine($"   - ID: {articulo.IdArticulo} | {articulo.Nombre} | ${articulo.PrecioUnitario:F2}");
                }
                Console.WriteLine();

                // ====================================
                // PRUEBA 2: OBTENER FORMAS DE PAGO
                // ====================================
                Console.WriteLine("💳 PRUEBA 2: Obteniendo formas de pago...");
                var formasPago = formaPagoService.ObtenerFormasPago();

                Console.WriteLine($"   Formas de pago encontradas: {formasPago.Count}");
                foreach (var forma in formasPago)
                {
                    Console.WriteLine($"   - ID: {forma.IdFormaPago} | {forma.Nombre}");
                }
                Console.WriteLine();

                // ====================================
                // PRUEBA 3: CREAR FACTURA SIMPLE
                // ====================================
                Console.WriteLine("📄 PRUEBA 3: Creando factura simple...");

                if (articulos.Count > 0 && formasPago.Count > 0)
                {
                    // Crear factura
                    var factura = facturaService.CrearFactura("Cliente de Prueba", formasPago[0].IdFormaPago);
                    Console.WriteLine($"   Factura creada para: {factura.Cliente}");
                    Console.WriteLine($"   Forma de pago: {factura.FormaPago.Nombre}");

                    // Agregar algunos artículos
                    bool resultado1 = facturaService.AgregarArticuloAFactura(factura, articulos[0].IdArticulo, 2);
                    bool resultado2 = facturaService.AgregarArticuloAFactura(factura, articulos[1].IdArticulo, 1);

                    Console.WriteLine($"   Artículo 1 agregado: {(resultado1 ? "✅" : "❌")}");
                    Console.WriteLine($"   Artículo 2 agregado: {(resultado2 ? "✅" : "❌")}");
                    Console.WriteLine($"   Total detalles: {factura.Detalles.Count}");
                    Console.WriteLine($"   Total factura: ${factura.Total:F2}");
                    Console.WriteLine();

                    // ====================================
                    // PRUEBA 4: REGLA DE NEGOCIO (ARTÍCULOS DUPLICADOS)
                    // ====================================
                    Console.WriteLine("🔄 PRUEBA 4: Probando artículos duplicados...");
                    Console.WriteLine($"   Cantidad inicial del primer artículo: {factura.Detalles[0].Cantidad}");

                    // Agregar el mismo artículo otra vez
                    bool resultado3 = facturaService.AgregarArticuloAFactura(factura, articulos[0].IdArticulo, 1);
                    Console.WriteLine($"   Mismo artículo agregado nuevamente: {(resultado3 ? "✅" : "❌")}");
                    Console.WriteLine($"   Cantidad después: {factura.Detalles[0].Cantidad}");
                    Console.WriteLine($"   Total detalles (debe seguir siendo 2): {factura.Detalles.Count}");
                    Console.WriteLine($"   Nuevo total: ${factura.Total:F2}");
                    Console.WriteLine();

                    // ====================================
                    // PRUEBA 5: GUARDAR FACTURA (OPCIONAL - SOLO SI TIENES BD)
                    // ====================================
                    Console.WriteLine("💾 PRUEBA 5: Guardando factura en BD...");
                    try
                    {
                        bool guardado = facturaService.GuardarFactura(factura);
                        if (guardado)
                        {
                            Console.WriteLine($"   ✅ Factura guardada exitosamente. Nº: {factura.NroFactura}");
                        }
                        else
                        {
                            Console.WriteLine("   ❌ Error al guardar factura");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ⚠️  Error de BD (normal si no está configurada): {ex.Message}");
                    }
                    Console.WriteLine();

                    // ====================================
                    // PRUEBA 6: OBTENER FACTURAS EXISTENTES (OPCIONAL)
                    // ====================================
                    Console.WriteLine("📋 PRUEBA 6: Consultando facturas existentes...");
                    try
                    {
                        var facturasExistentes = facturaService.ObtenerFacturas();
                        Console.WriteLine($"   Facturas en BD: {facturasExistentes.Count}");

                        foreach (var f in facturasExistentes.Take(3))
                        {
                            Console.WriteLine($"   - Nº: {f.NroFactura} | {f.Cliente} | ${f.Total:F2} | {f.Fecha:dd/MM/yyyy}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ⚠️  Error de BD: {ex.Message}");
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("   ❌ No hay datos suficientes para crear factura");
                }

                // ====================================
                // PRUEBA 7: VENTA COMPLETA CON MANAGER
                // ====================================
                Console.WriteLine("🏪 PRUEBA 7: Venta completa con FacturacionManager...");
                try
                {
                    if (articulos.Count >= 2 && formasPago.Count > 0)
                    {
                        var articulosVenta = new List<(int idArticulo, int cantidad, decimal? precio)>
                        {
                            (articulos[0].IdArticulo, 1, null),  // Primer artículo
                            (articulos[1].IdArticulo, 2, null),  // Segundo artículo  
                            (articulos[0].IdArticulo, 1, null)   // Primer artículo otra vez (debe sumar)
                        };

                        var ventaCompleta = manager.ProcesarVentaCompleta(
                            "Cliente Manager Test",
                            formasPago[0].IdFormaPago,
                            articulosVenta
                        );

                        if (ventaCompleta != null)
                        {
                            Console.WriteLine($"   ✅ Venta procesada. Nº: {ventaCompleta.NroFactura}");
                            Console.WriteLine($"   Cliente: {ventaCompleta.Cliente}");
                            Console.WriteLine($"   Detalles: {ventaCompleta.Detalles.Count}");
                            Console.WriteLine($"   Total: ${ventaCompleta.Total:F2}");
                        }
                        else
                        {
                            Console.WriteLine("   ❌ Error en venta completa");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  Error en venta completa: {ex.Message}");
                }

                Console.WriteLine("\n===========================================");
                Console.WriteLine("🎉 PRUEBAS COMPLETADAS");
                Console.WriteLine("===========================================");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR GENERAL: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}

// ====================================
// EXTENSIÓN PARA LINQ (si no tienes .NET 6+)
// ====================================
#if !NET6_0_OR_GREATER
public static class LinqExtensions
{
    public static IEnumerable<T> Take<T>(this IEnumerable<T> source, int count)
    {
        if (source == null) yield break;
        
        int taken = 0;
        foreach (var item in source)
        {
            if (taken >= count) yield break;
            yield return item;
            taken++;
        }
    }
}
#endif