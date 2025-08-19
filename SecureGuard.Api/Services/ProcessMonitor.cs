using System.Management;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SecureGuard.Api.Data;
using SecureGuard.Api.Models;

namespace SecureGuard.Api.Services
{
    public class ProcessMonitorService : BackgroundService
    {
        private readonly AppDbContext _db;
        private ManagementEventWatcher? _watcher;

        public ProcessMonitorService(AppDbContext db)
        {
            _db = db;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var query = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace");
            _watcher = new ManagementEventWatcher(query);
            _watcher.EventArrived += (s, e) =>
            {
                var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();

              var log = new Log
{
    Tipo = LogType.ProcessoSuspeito,
    Caminho = processName,
    Risco = RiskLevel.Alto,
    Mensagem = "Novo processo detectado"
    // CriadoEmUtc será preenchido automaticamente
};

                _db.Logs.Add(log);
                _db.SaveChanges();
            };

            _watcher.Start();
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _watcher?.Stop();
            _watcher?.Dispose();
            base.Dispose();
        }
    }
}
