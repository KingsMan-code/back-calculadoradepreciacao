using ClosedXML.Excel;
using CalculadoraDepreciacao.Models;

namespace CalculadoraDepreciacao.Services;

public class ExcelExporter
{
    public byte[] CreateWorkbookWithFormulas(CalculationRequest req)
    {
        using var wb = new XLWorkbook();
        var sheetName = "Calculadora Aluguel Equipamentos"; // ensure <= 31 chars
        if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
        var ws = wb.Worksheets.Add(sheetName);

        // Headers and labels (minimal for clarity)
        ws.Cell("A1").Value = "Parâmetros de Entrada";
        ws.Cell("A2").Value = "Preço de Compra (por unidade)";
        ws.Cell("A3").Value = "Vida Útil (anos)";
        ws.Cell("A4").Value = "% Custos Adicionais Anuais";
        ws.Cell("A5").Value = "% Margem de Lucro Anual";
        ws.Cell("A6").Value = "Dias de Utilização Anual (estimado)";
        ws.Cell("A7").Value = "Quantidade de Equipamentos";
        ws.Cell("A8").Value = "Tipo de Período";
        ws.Cell("A9").Value = "Número de Períodos (dias ou meses)";
        ws.Cell("A10").Value = "% Desconto para Períodos Longos (opcional)";

        // Input values
        ws.Cell("B2").Value = req.PrecoCompra;
        ws.Cell("B3").Value = req.VidaUtilAnos;
        ws.Cell("B4").Value = req.PercentCustos;
        ws.Cell("B5").Value = req.PercentMargem;
        ws.Cell("B6").Value = req.DiasUtilizacaoAnual;
        ws.Cell("B7").Value = req.Quantidade;
        ws.Cell("B8").Value = req.TipoPeriodo;
        ws.Cell("B9").Value = req.NumeroPeriodos;
        ws.Cell("B10").Value = req.PercentDesconto;

        // Apply styles similar to the Python script
        // Header rows (1,12,20) - bold and centered across a few columns
        ws.Range("A1:F1").Style.Font.SetBold();
        ws.Range("A1:F1").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
        ws.Range("A12:F12").Style.Font.SetBold();
        ws.Range("A12:F12").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
        ws.Range("A20:F20").Style.Font.SetBold();
        ws.Range("A20:F20").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

        // Input cell styles
        ws.Cell("B2").Style.NumberFormat.Format = "\"R$\" #,##0.00"; // currency
        ws.Cell("B2").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;

        foreach (var addr in new[] { "B3", "B6", "B7", "B9" })
        {
            ws.Cell(addr).Style.NumberFormat.Format = "#,#0"; // integers
            ws.Cell(addr).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
        }

        foreach (var addr in new[] { "B4", "B5", "B10" })
        {
            ws.Cell(addr).Style.NumberFormat.Format = "0.00%"; // percent
            ws.Cell(addr).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
        }

        // Dropdown for Tipo de Período (B8)
        ws.Cell("B8").Value = req.TipoPeriodo;
        try
        {
            var dv = ws.Range("B8").CreateDataValidation();
            dv.AllowedValues = ClosedXML.Excel.XLAllowedValues.List;
            dv.InCellDropdown = true;
            dv.List("Dias,Meses");
            dv.ShowInputMessage = false;
            dv.ShowErrorMessage = false;
        }
        catch
        {
            // If data validation API differs, ignore; cell will still contain the value.
        }

        // Formulas (keep formulas so Excel calculates)
        ws.Cell("B13").FormulaA1 = "=B2 / B3"; // Depreciação Anual
        ws.Cell("B14").FormulaA1 = "=B2 * B4"; // Custos
        ws.Cell("B15").FormulaA1 = "=B13 + B14"; // Custo total
        ws.Cell("B16").FormulaA1 = "=B2 * B5"; // Margem
        ws.Cell("B17").FormulaA1 = "=B15 + B16"; // Receita anual
        ws.Cell("B18").FormulaA1 = "=B17 / B6"; // Taxa diária base
        ws.Cell("B21").FormulaA1 = "=IF(B8=\"Dias\", B9, B9*30)";
        ws.Cell("B22").FormulaA1 = "=B18 * (1 - B10)";
        ws.Cell("B23").FormulaA1 = "=B22 * B21";
        ws.Cell("B24").FormulaA1 = "=B23 * B7";

        // Style formulas results as currency where appropriate
        foreach (var addr in new[] { "B13", "B14", "B15", "B16", "B17", "B18", "B22", "B23", "B24" })
        {
            ws.Cell(addr).Style.NumberFormat.Format = "\"R$\" #,##0.00";
            ws.Cell(addr).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
        }

        // Styles
        ws.Column("A").Width = 40;
        ws.Column("B").Width = 25;
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
