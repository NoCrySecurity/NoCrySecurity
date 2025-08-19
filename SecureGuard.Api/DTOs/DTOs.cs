using SecureGuard.Api.Models;

namespace SecureGuard.Api.DTOs;

public class LogsQuery
{
    public LogType? Tipo { get; set; }
    public RiskLevel? Risco { get; set; }
    public DateTime? DeUtc { get; set; }
    public DateTime? AteUtc { get; set; }
    public string? Busca { get; set; }
    public string? OrdenarPor { get; set; } = "data_desc";
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 50;
}

public class PagedResult<T>
{
    public required IEnumerable<T> Itens { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
}
