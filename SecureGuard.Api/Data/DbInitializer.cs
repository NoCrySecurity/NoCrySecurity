using System;
using System.Linq;
using SecureGuard.Api.Models;

namespace SecureGuard.Api.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            // se já tem seed, sai
            if (db.Logs.Any()) return;

            db.Logs.AddRange(
                new Log
                {
                    Tipo = LogType.NovoArquivo,
                    Risco = RiskLevel.Baixo,
                    Mensagem = "file.txt criado",
                    Caminho = @"C:\Users\annun\Downloads\file.txt",
                    CriadoEmUtc = DateTime.UtcNow.AddMinutes(-15)
                },
                new Log
                {
                    Tipo = LogType.CriptografiaSuspeita,
                    Risco = RiskLevel.Alto,
                    Mensagem = "*.enc detectado",
                    Caminho = @"C:\Users\annun\Downloads\secreto.enc",
                    CriadoEmUtc = DateTime.UtcNow.AddMinutes(-5)
                }
            );
            db.SaveChanges();
        }
    }
}
