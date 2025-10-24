using System.Globalization;
using CalculadoraDepreciacao.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CalculadoraDepreciacao.Services;

public class PdfExporter
{
    static PdfExporter()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] CreatePdf(CalculationRequest req, CalculationResult? result = null)
    {
        // Ensure QuestPDF license is configured (Community license) to avoid runtime exception
        try
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }
        catch
        {
            // If the property is not present in the version, ignore
        }
        // Generate simple PDF text with the inputs and results
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                // simple content without advanced chaining to remain compatible
                page.Content().Column(column =>
                {
                    column.Item().Text("Calculadora Aluguel Equipamentos");
                    column.Item().Text($"Preço de Compra: R$ {req.PrecoCompra:N2}");
                    column.Item().Text($"Vida útil (anos): {req.VidaUtilAnos}");
                    column.Item().Text($"% Custos: {req.PercentCustos:P2}");
                    column.Item().Text($"% Margem: {req.PercentMargem:P2}");
                    column.Item().Text($"Dias utilização anual: {req.DiasUtilizacaoAnual}");
                    column.Item().Text($"Quantidade: {req.Quantidade}");
                    column.Item().Text($"Tipo período: {req.TipoPeriodo}");
                    column.Item().Text($"Número períodos: {req.NumeroPeriodos}");
                    column.Item().Text($"% Desconto: {req.PercentDesconto:P2}");

                    if (result is not null)
                    {
                        column.Item().Text("Resultados:");
                        column.Item().Text($"Depreciação anual/unidade: R$ {result.DepreciacaoAnualPorUnidade:N2}");
                        column.Item().Text($"Custo total anual/unidade: R$ {result.CustoTotalAnualPorUnidade:N2}");
                        column.Item().Text($"Taxa diária base/unidade: R$ {result.TaxaDiariaBasePorUnidade:N2}");
                        column.Item().Text($"Preço total para todos: R$ {result.PrecoTotalParaTodos:N2}");
                    }
                });
            });
        });

        using var ms = new MemoryStream();
        doc.GeneratePdf(ms);
        return ms.ToArray();
    }
}
