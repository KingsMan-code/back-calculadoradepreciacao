namespace CalculadoraDepreciacao.Models;

public class CalculationResult
{
    public decimal DepreciacaoAnualPorUnidade { get; set; }
    public decimal CustosAdicionaisAnualPorUnidade { get; set; }
    public decimal CustoTotalAnualPorUnidade { get; set; }
    public decimal MargemLucroAnualPorUnidade { get; set; }
    public decimal ReceitaAnualPorUnidade { get; set; }
    public decimal TaxaDiariaBasePorUnidade { get; set; }
    public int DiasTotaisLocacao { get; set; }
    public decimal PrecoDiarioAjustado { get; set; }
    public decimal PrecoTotalPorUnidade { get; set; }
    public decimal PrecoTotalParaTodos { get; set; }
}
