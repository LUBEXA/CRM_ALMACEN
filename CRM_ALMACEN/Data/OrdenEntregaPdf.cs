using CRM_ALMACEN.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRM_ALMACEN.Data;

/// <summary>
/// Genera la "Orden de Entrega" en PDF para un pedido (SolicitudPedido).
/// Diseño en blanco y negro (pensado para impresión), con el desglose de
/// servicios por producto: Tarimas, Maniobras, Paletizados, Insumo tarima.
/// </summary>
public static class OrdenEntregaPdf
{
    // Datos del emisor (el almacén que entrega). Ajusta aquí si cambian.
    private const string EmisorNombre = "LUBEXA IMPORT & SUPPLY";
    private const string EmisorCorto = "LUBEXA";
    private const string Almacen = "Matriz LUBEXA";

    // Tonos de gris (impresión en blanco y negro)
    private static readonly Color Negro = Color.FromHex("#111111");
    private static readonly Color Gris = Color.FromHex("#555555");
    private static readonly Color GrisFila = Color.FromHex("#f2f2f2");
    private static readonly Color GrisEnc = Color.FromHex("#dddddd");
    private static readonly Color Linea = Color.FromHex("#888888");

    /// <param name="servicios">Cobros/servicios del pedido (maniobras, tarimas, etc.) a reflejar en la orden.</param>
    /// <param name="logoPng">Bytes del logo (PNG/JPG). Puede ser null si aún no hay logo.</param>
    public static byte[] Generar(SolicitudPedido s, IEnumerable<Cargo>? servicios = null, byte[]? logoPng = null)
    {
        var folio = $"OR-{s.Id:00000}";
        var listaServicios = servicios?.ToList() ?? [];

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.4f, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontSize(9).FontColor(Negro).FontFamily("Helvetica"));

                page.Content().Column(col =>
                {
                    col.Spacing(0);

                    // ====== Encabezado ======
                    col.Item().PaddingBottom(8).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(EmisorNombre).FontSize(16).Bold();
                            c.Item().Text("Almacenaje y distribución").FontSize(8.5f).FontColor(Gris);
                        });
                        row.ConstantItem(150).AlignRight().AlignMiddle().Element(c =>
                        {
                            if (logoPng is { Length: > 0 })
                                c.Height(44).Image(logoPng).FitHeight();
                            else
                                c.Text("").FontSize(8);
                        });
                    });

                    // Barra de título + folio
                    col.Item().BorderTop(1.5f).BorderBottom(1.5f).BorderColor(Negro)
                        .PaddingVertical(6).Row(row =>
                    {
                        row.RelativeItem().AlignMiddle().Text("ORDEN DE ENTREGA")
                            .FontSize(13).Bold().LetterSpacing(0.06f);
                        row.ConstantItem(180).AlignRight().AlignMiddle().Text(t =>
                        {
                            t.Span("FOLIO: ").FontSize(9).FontColor(Gris);
                            t.Span(folio).FontSize(12).Bold();
                        });
                    });

                    // ====== Cuadro de datos ======
                    col.Item().PaddingTop(10).Border(1).BorderColor(Linea).Column(box =>
                    {
                        FilaDoble(box, "Cliente:", s.Cliente?.Nombre ?? "—",
                                       "RFC:", string.IsNullOrWhiteSpace(s.Cliente?.Rfc) ? "—" : s.Cliente!.Rfc!);
                        FilaDoble(box, "Fecha:", $"{s.Fecha:dd/MM/yyyy}    Horario: {s.Fecha:HH:mm} hrs",
                                       "Almacén de salida:", Almacen);
                        FilaDoble(box, "Unidad de transporte:", "______________________",
                                       "No. de piezas:", s.Detalles.Sum(d => d.Cantidad).ToString());
                        FilaLarga(box, "Domicilio destino:",
                                  string.IsNullOrWhiteSpace(s.Cliente?.Direccion) ? "—" : s.Cliente!.Direccion!);
                        FilaLarga(box, "Observaciones:",
                                  string.IsNullOrWhiteSpace(s.Notas) ? "—" : s.Notas!, ultima: true);
                    });

                    // ====== Tabla de mercancía ======
                    col.Item().PaddingTop(14).Text("Detalle de la mercancía").FontSize(11).Bold();
                    col.Item().PaddingTop(4).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(26);   // #
                            c.RelativeColumn(4);    // Modelo
                            c.RelativeColumn(1.6f);  // Ubicación
                            c.RelativeColumn(1.2f);  // Piezas
                        });

                        void Th(string t) => tabla.Cell().Element(CeldaEnc).Text(t).Bold().FontSize(8.5f);
                        Th("#"); Th("Modelo / Producto"); Th("Ubicación"); Th("Piezas");

                        var totalPiezas = 0;
                        var i = 0;
                        foreach (var d in s.Detalles)
                        {
                            i++;
                            totalPiezas += d.Cantidad;
                            var fondo = i % 2 == 0 ? GrisFila : Colors.White;
                            var modelo = $"{d.Producto?.Codigo} — {d.Producto?.Nombre}".Trim(' ', '—');

                            tabla.Cell().Background(fondo).Element(Celda).AlignCenter().Text(i.ToString());
                            tabla.Cell().Background(fondo).Element(Celda).Text(modelo);
                            tabla.Cell().Background(fondo).Element(Celda).AlignCenter()
                                .Text(string.IsNullOrWhiteSpace(d.Producto?.Ubicacion) ? "—" : d.Producto!.Ubicacion!);
                            tabla.Cell().Background(fondo).Element(Celda).AlignCenter().Text(d.Cantidad.ToString()).Bold();
                        }

                        tabla.Cell().ColumnSpan(3).Element(CeldaTotal).AlignRight().Text("TOTAL DE PIEZAS").Bold();
                        tabla.Cell().Element(CeldaTotal).AlignCenter().Text(totalPiezas.ToString()).Bold();
                    });

                    // ====== Tabla de servicios (maniobras, tarimas, etc.) ======
                    col.Item().PaddingTop(14).Text("Servicios aplicados").FontSize(11).Bold();
                    col.Item().PaddingTop(4).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(26);   // #
                            c.RelativeColumn(5);    // Servicio
                            c.RelativeColumn(1.4f);  // Cantidad
                        });

                        void Th(string t) => tabla.Cell().Element(CeldaEnc).Text(t).Bold().FontSize(8.5f);
                        Th("#"); Th("Tipo de servicio"); Th("Cantidad");

                        var cobrables = listaServicios.Where(c => c.Tipo == TipoCargo.Servicio).ToList();
                        if (cobrables.Count == 0)
                        {
                            tabla.Cell().ColumnSpan(3).Element(Celda).AlignCenter()
                                .Text("Sin servicios capturados").FontColor(Gris).Italic();
                        }
                        else
                        {
                            var i = 0;
                            var totalServ = 0;
                            foreach (var c in cobrables)
                            {
                                i++;
                                totalServ += c.Cantidad;
                                var fondo = i % 2 == 0 ? GrisFila : Colors.White;
                                tabla.Cell().Background(fondo).Element(Celda).AlignCenter().Text(i.ToString());
                                tabla.Cell().Background(fondo).Element(Celda).Text(c.Concepto);
                                tabla.Cell().Background(fondo).Element(Celda).AlignCenter().Text(c.Cantidad.ToString()).Bold();
                            }
                            tabla.Cell().ColumnSpan(2).Element(CeldaTotal).AlignRight().Text("TOTAL DE SERVICIOS").Bold();
                            tabla.Cell().Element(CeldaTotal).AlignCenter().Text(totalServ.ToString()).Bold();
                        }
                    });

                    // ====== Condiciones ======
                    col.Item().PaddingTop(14).Text("Condiciones de entrega / recepción").FontSize(11).Bold();
                    col.Item().Text("Estatus físico, daños visibles, empaques y validaciones:").FontSize(8.5f).FontColor(Gris);
                    col.Item().PaddingTop(4).Border(1).BorderColor(Linea).Padding(6).Column(c =>
                    {
                        for (var k = 0; k < 3; k++)
                            c.Item().PaddingVertical(9).LineHorizontal(0.6f).LineColor(Linea);
                    });

                    col.Item().PaddingTop(6).Text(t =>
                    {
                        t.Span("Nota: ").Bold().FontSize(8);
                        t.Span("Toda mercancía se recibe y/o entrega tras revisión física visible. " +
                               "Daños no visibles por embalaje quedan sujetos a validación posterior.").FontSize(8).FontColor(Gris);
                    });

                    // ====== Firmas ======
                    col.Item().PaddingTop(34).Row(row =>
                    {
                        row.RelativeItem().Element(c => Firma(c, "Operador (Cliente)"));
                        row.ConstantItem(40);
                        row.RelativeItem().Element(c => Firma(c, $"Revisado por ({EmisorCorto})"));
                    });
                });

                // ====== Pie de página ======
                page.Footer().BorderTop(0.8f).BorderColor(Linea).PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text($"{EmisorNombre}  ·  {Almacen}").FontSize(7.5f).FontColor(Gris);
                    row.RelativeItem().AlignRight().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(7.5f).FontColor(Gris);
                });
            });
        }).GeneratePdf();
    }

    // -------- filas del cuadro de datos --------
    private static void FilaDoble(ColumnDescriptor box, string e1, string v1, string e2, string v2)
        => box.Item().BorderBottom(1).BorderColor(Linea).Row(r =>
        {
            r.RelativeItem().Padding(5).Text(t => { t.Span($"{e1} ").Bold(); t.Span(v1); });
            r.RelativeItem().BorderLeft(1).BorderColor(Linea).Padding(5).Text(t => { t.Span($"{e2} ").Bold(); t.Span(v2); });
        });

    private static void FilaLarga(ColumnDescriptor box, string etiqueta, string valor, bool ultima = false)
    {
        var item = ultima ? box.Item() : box.Item().BorderBottom(1).BorderColor(Linea);
        item.Padding(5).Text(t => { t.Span($"{etiqueta} ").Bold(); t.Span(valor); });
    }

    // -------- bloque de firma --------
    private static void Firma(IContainer c, string rol)
        => c.Column(col =>
        {
            col.Item().PaddingTop(22).LineHorizontal(0.8f).LineColor(Negro);
            col.Item().PaddingTop(3).Text(rol).FontSize(8.5f).Bold();
            col.Item().Text("Nombre y firma").FontSize(7.5f).FontColor(Gris);
            col.Item().PaddingTop(6).Text("Fecha: ______ / ______ / __________").FontSize(8.5f);
        });

    // -------- celdas de la tabla --------
    private static IContainer CeldaEnc(IContainer c)
        => c.Background(GrisEnc).Border(0.6f).BorderColor(Linea).PaddingVertical(5).PaddingHorizontal(5).AlignCenter().AlignMiddle();

    private static IContainer Celda(IContainer c)
        => c.Border(0.6f).BorderColor(Linea).PaddingVertical(5).PaddingHorizontal(5).AlignMiddle();

    private static IContainer CeldaTotal(IContainer c)
        => c.Background(GrisEnc).Border(0.6f).BorderColor(Linea).PaddingVertical(5).PaddingHorizontal(5).AlignMiddle();
}
