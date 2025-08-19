const express = require('express');
const router = express.Router();
const {
  getAllLogs,
  insertLog,
  logArquivoSuspeito,
  logCriptografiaSuspeita,
  logNovoArquivoDetectado,
  logAcessoNaoAutorizado,
  logAlteracaoRegistro,
  logProcessoDesconhecido,
  logBloqueioRansomware
} = require('../Models/LogModel'); // Ajuste o caminho se necessário

// 🔹 GET - Buscar logs com filtros, ordenação e paginação
router.get('/logs', async (req, res) => {
  try {
    const { filtro, ordenacao, page = 1, limit = 10 } = req.query;
    let logs = await getAllLogs();

    // Filtro
    if (filtro) {
      const filtroLower = filtro.toLowerCase();
      logs = logs.filter(log =>
        (log.Tipo && log.Tipo.toLowerCase().includes(filtroLower)) ||
        (log.Mensagem && log.Mensagem.toLowerCase().includes(filtroLower)) ||
        (log.Origem && log.Origem.toLowerCase().includes(filtroLower))
      );
    }

    // Ordenação
    if (ordenacao === 'data') {
      logs.sort((a, b) => new Date(b.Timestamp) - new Date(a.Timestamp));
    } else if (ordenacao === 'risco') {
      logs.sort((a, b) => b.NivelRisco - a.NivelRisco);
    } else if (ordenacao === 'nome') {
      logs.sort((a, b) => (a.Mensagem || '').localeCompare(b.Mensagem || ''));
    }

    // Paginação
    const pageNum = Number(page) || 1;
    const lim = Number(limit) || 10;
    const start = (pageNum - 1) * lim;
    const pagedLogs = logs.slice(start, start + lim);

    res.json({
      total: logs.length,
      page: pageNum,
      limit: lim,
      logs: pagedLogs
    });
  } catch (err) {
    console.error('Erro ao buscar logs:', err.message);
    res.status(500).send('Erro ao buscar logs.');
  }
});

// 🔸 POST - Inserir novo log
router.post('/logs/inserir', async (req, res) => {
  try {
    await insertLog(req.body);
    res.status(201).send('Log adicionado com sucesso!');
  } catch (err) {
    console.error('Erro ao inserir log:', err.message);
    res.status(500).send('Erro ao inserir log.');
  }
});

module.exports = router;
