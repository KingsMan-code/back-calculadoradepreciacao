namespace CalculadoraDepreciacao.Models;

public class CalculationRequest
{
    public decimal PrecoCompra { get; set; }
    public int VidaUtilAnos { get; set; }
    public decimal PercentCustos { get; set; }
    public decimal PercentMargem { get; set; }
    public int DiasUtilizacaoAnual { get; set; }
    public int Quantidade { get; set; }
    public string TipoPeriodo { get; set; } = "Dias"; // "Dias" ou "Meses"
    public int NumeroPeriodos { get; set; }
    public decimal PercentDesconto { get; set; }
}
