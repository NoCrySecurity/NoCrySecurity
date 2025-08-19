const sql = require('mssql');

const dbConfig = {
  user: 'sqlserver',
  password: 'sqlserver1',
  server: 'localhost',
  port: 1433,
  database: 'master',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

async function getAllLogs() {
  try {
    await sql.connect(dbConfig);
    const result = await sql.query`SELECT * FROM dbo.Logs ORDER BY Timestamp DESC`;
    return result.recordset;
  } catch (err) {
    console.error('Erro ao buscar logs:', err.message);
    throw err;
  }
}

async function insertLog({ Tipo, Mensagem, Origem, NivelRisco }) {
  try {
    await sql.connect(dbConfig);
    await sql.query`
      INSERT INTO dbo.Logs (Timestamp, Tipo, Mensagem, Origem, NivelRisco)
      VALUES (GETDATE(), ${Tipo}, ${Mensagem}, ${Origem}, ${NivelRisco})
    `;
  } catch (err) {
    console.error('Erro ao inserir log:', err.message);
    throw err;
  }
}

async function logArquivoSuspeito(nomeArquivo) {
  await insertLog({
    Tipo: 'Arquivo Suspeito',
    Mensagem: `O arquivo "${nomeArquivo}" apresentou comportamento incomum.`,
    Origem: 'Monitoramento de Arquivos',
    NivelRisco: 2
  });
}

async function logCriptografiaSuspeita(processName) {
  await insertLog({
    Tipo: 'Criptografia Suspeita',
    Mensagem: `Possível atividade de criptografia por "${processName}".`,
    Origem: 'Analisador de Processos',
    NivelRisco: 3
  });
}

async function logNovoArquivoDetectado(caminhoArquivo) {
  await insertLog({
    Tipo: 'Novo Arquivo',
    Mensagem: `Novo arquivo detectado: ${caminhoArquivo}`,
    Origem: 'Sistema de Arquivos',
    NivelRisco: 1
  });
}

async function logAcessoNaoAutorizado(usuario, recurso) {
  await insertLog({
    Tipo: 'Acesso Não Autorizado',
    Mensagem: `Usuário "${usuario}" tentou acessar "${recurso}" sem permissão.`,
    Origem: 'Firewall Inteligente',
    NivelRisco: 2
  });
}

async function logAlteracaoRegistro(chave) {
  await insertLog({
    Tipo: 'Alteração no Registro',
    Mensagem: `Chave de registro modificada: "${chave}"`,
    Origem: 'Registro do Sistema',
    NivelRisco: 2
  });
}

async function logProcessoDesconhecido(processName) {
  await insertLog({
    Tipo: 'Processo Desconhecido',
    Mensagem: `Processo não identificado: "${processName}"`,
    Origem: 'Monitor de Processos',
    NivelRisco: 2
  });
}

async function logBloqueioRansomware(processName) {
  await insertLog({
    Tipo: 'Bloqueio de Ransomware',
    Mensagem: `Processo "${processName}" foi bloqueado preventivamente.`,
    Origem: 'IA Anti-Ransomware',
    NivelRisco: 3
  });
}

async function logArquivoRemovido(filePath) {
  await insertLog({
    Tipo: 'Arquivo Removido',
    Mensagem: `O arquivo "${filePath}" foi removido.`,
    Origem: 'Monitoramento de Arquivos',
    NivelRisco: 2
  });
}

module.exports = {
  insertLog,
  getAllLogs,
  logArquivoSuspeito,
  logCriptografiaSuspeita,
  logNovoArquivoDetectado,
  logAcessoNaoAutorizado,
  logAlteracaoRegistro,
  logProcessoDesconhecido,
  logBloqueioRansomware,
  logArquivoRemovido   
};