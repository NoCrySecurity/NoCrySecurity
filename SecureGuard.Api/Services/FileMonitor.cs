using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SecureGuard.Api.Data;
using SecureGuard.Api.Models;

namespace SecureGuard.Api.Services
{
    public class FileMonitorService : BackgroundService
    {
        private readonly AppDbContext _db;
        private FileSystemWatcher? _watcher;

        public FileMonitorService(AppDbContext db)
        {
            _db = db;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _watcher = new FileSystemWatcher(@"C:\Users\annun\Downloads")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            _watcher.Created += (s, e) =>
    Log(LogType.NovoArquivo, e.FullPath, RiskLevel.Baixo);

_watcher.Changed += (s, e) =>
    Log(LogType.CriptografiaSuspeita, e.FullPath, RiskLevel.Critico, "Arquivo alterado");

_watcher.Deleted += (s, e) =>
    Log(LogType.ArquivoRemovido, e.FullPath, RiskLevel.Medio);

_watcher.Renamed += (s, e) =>
{
    var args = (RenamedEventArgs)e;
    Log(LogType.ArquivoSuspeito, args.FullPath, RiskLevel.Baixo, $"Renomeado de {args.OldFullPath}");
};

            return Task.CompletedTask;
        }

        private void Log(LogType tipo, string caminho, RiskLevel risco, string? mensagem = null)
{
    var log = new Log
    {
        Tipo = tipo,
        Caminho = caminho,
        Mensagem = mensagem,
        Risco = risco
        // ✅ Não precisa setar CriadoEmUtc — já tem valor padrão
    };

    _db.Logs.Add(log);
    _db.SaveChanges();
}

        public override void Dispose()
        {
            _watcher?.Dispose();
            base.Dispose();
        }
    }
}
