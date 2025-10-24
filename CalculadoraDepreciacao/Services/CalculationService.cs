using CalculadoraDepreciacao.Models;

namespace CalculadoraDepreciacao.Services;

public class CalculationService
{
    public CalculationResult Calculate(CalculationRequest req)
    {
        var result = new CalculationResult
        {
            DepreciacaoAnualPorUnidade = req.PrecoCompra / Math.Max(1, req.VidaUtilAnos),
            CustosAdicionaisAnualPorUnidade = req.PrecoCompra * req.PercentCustos
        };

        result.CustoTotalAnualPorUnidade = result.DepreciacaoAnualPorUnidade + result.CustosAdicionaisAnualPorUnidade;
        result.MargemLucroAnualPorUnidade = req.PrecoCompra * req.PercentMargem;
        result.ReceitaAnualPorUnidade = result.CustoTotalAnualPorUnidade + result.MargemLucroAnualPorUnidade;
        result.TaxaDiariaBasePorUnidade = result.ReceitaAnualPorUnidade / Math.Max(1, req.DiasUtilizacaoAnual);
        result.DiasTotaisLocacao = req.TipoPeriodo.Equals("Meses", StringComparison.OrdinalIgnoreCase)
            ? req.NumeroPeriodos * 30
            : req.NumeroPeriodos;
        result.PrecoDiarioAjustado = result.TaxaDiariaBasePorUnidade * (1 - req.PercentDesconto);
        result.PrecoTotalPorUnidade = result.PrecoDiarioAjustado * result.DiasTotaisLocacao;
        result.PrecoTotalParaTodos = result.PrecoTotalPorUnidade * req.Quantidade;

        return result;
    }
}
