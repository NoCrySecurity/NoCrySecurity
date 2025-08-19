using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureGuard.Api.Data;
using SecureGuard.Api.Models;

namespace SecureGuard.Api.Controllers
{
    public class LogsController : Controller
    {
        private readonly AppDbContext _db;

        public LogsController(AppDbContext db)
        {
            _db = db;
        }

        // ======== 🌐 ROTA HTML (MVC View) ============
        // GET: /Logs/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Retorna a página HTML com filtros, tabela e JS
        }

        // ======== 🔄 API JSON (usada pelo JavaScript) ========
        // GET: /api/logs
        [HttpGet("/api/logs")]
        public async Task<IActionResult> GetLogs(
            string? filtro,
            string? ordenarPor = "data",
            int pagina = 1,
            int tamanho = 10)
        {
            var query = _db.Logs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.ToLower();
                query = query.Where(l =>
                    (l.Mensagem ?? "").ToLower().Contains(f) ||
                    l.Tipo.ToString().ToLower().Contains(f));
            }

            query = ordenarPor?.ToLower() switch
            {
                "risco" => query.OrderByDescending(l => l.Risco),
                "nome" => query.OrderBy(l => l.Mensagem),
                _ => query.OrderByDescending(l => l.CriadoEmUtc)
            };

            int totalLogs = await query.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalLogs / (double)tamanho);

            var logs = await query
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync();

            return Json(new
            {
                logs,
                totalPaginas
            });
        }

        // POST: /api/logs
        [HttpPost("/api/logs")]
        public async Task<IActionResult> Criar([FromBody] Log log)
        {
            if (log == null || string.IsNullOrWhiteSpace(log.Mensagem))
                return BadRequest("Dados inválidos");

            // Garantia dos valores mínimos
            log.CriadoEmUtc = DateTime.UtcNow;

            if (!Enum.IsDefined(typeof(LogType), log.Tipo))
                log.Tipo = LogType.NovoArquivo;

            log.Risco = Enum.IsDefined(typeof(RiskLevel), log.Risco)
                ? log.Risco
                : RiskLevel.Baixo;

            _db.Logs.Add(log);
            await _db.SaveChangesAsync();

            return Ok(log);
        }
    }
}
