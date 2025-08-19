using System;

namespace SecureGuard.Api.Models
{
    public enum LogType
    {
        NovoArquivo = 0,
        CriptografiaSuspeita = 1,
        ArquivoSuspeito = 2,
        ArquivoRemovido = 3,
        ProcessoSuspeito = 4,

        // Adicionado para compatibilizar com o serviço:
        AcessoNaoAutorizado = 5
    }

    public enum RiskLevel
    {
        // Adicionado para compatibilizar com o serviço:
        Info = 0,

        Baixo = 1,
        Medio = 2,
        Alto = 3,
        Critico = 4,
        Maximo = 5
    }

    public class Log
    {
        public int Id { get; set; }
        public LogType Tipo { get; set; }
        public RiskLevel Risco { get; set; }
        public string? Mensagem { get; set; }
        public string? Caminho { get; set; }
        public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    }
}
