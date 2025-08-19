const express = require('express');
const path = require('path');
const cors = require('cors');

// ====== MONITORAMENTO AVANÇADO (CHOKIDAR) ======
const chokidar = require('chokidar');
const {
  logNovoArquivoDetectado,
  logArquivoSuspeito,
  logCriptografiaSuspeita,
  logArquivoRemovido  
} = require('./Models/LogModel');

// Pastas que você quer monitorar
const pathParaMonitorar = [
  'C:/Users/annun/Downloads',
];

// Extensões que indicam possível criptografia/ransomware
const EXT_SUSPEITAS = ['.encrypted', '.locked', '.cry', '.crypto', '.enc', '.ftcode', '.xor'];
const EXT_EXECUTAVEIS = ['.exe', '.bat', '.scr', '.js', '.vbs', '.ps1'];

// Controle de eventos para detectar criptografia em massa
let eventosRecentes = [];
const LIMITE_EVENTOS = 40;      // Altere a sensibilidade
const INTERVALO_MS = 12000;     // 12 segundos

const watcher = chokidar.watch(pathParaMonitorar, {
  ignored: /^\./,
  persistent: true,
  ignoreInitial: true
});

// Novo arquivo criado
watcher.on('add', async (filePath) => {
  try {
    if (EXT_SUSPEITAS.some(ext => filePath.endsWith(ext))) {
      await logCriptografiaSuspeita(`Arquivo suspeito: ${filePath}`);
      console.log('[Monitoramento] Criptografia suspeita detectada:', filePath);
    } else if (EXT_EXECUTAVEIS.some(ext => filePath.endsWith(ext))) {
      await logArquivoSuspeito(filePath);
      console.log('[Monitoramento] Arquivo executável suspeito:', filePath);
    } else {
      await logNovoArquivoDetectado(filePath);
      console.log('[Monitoramento] Novo arquivo detectado:', filePath);
    }

    eventosRecentes.push(Date.now());
    eventosRecentes = eventosRecentes.filter(t => t > Date.now() - INTERVALO_MS);
    if (eventosRecentes.length > LIMITE_EVENTOS) {
      await logCriptografiaSuspeita(`Muitos arquivos alterados em sequência: ${eventosRecentes.length} em ${INTERVALO_MS / 1000}s!`);
      console.log('[Monitoramento] ALERTA: Possível criptografia em massa detectada!');
      eventosRecentes = [];
    }
  } catch (e) {
    console.error('[Monitoramento] Erro ao inserir log:', e.message);
  }
});

// Nova pasta criada
watcher.on('addDir', async (dirPath) => {
  try {
    await logNovoArquivoDetectado(`[PASTA] ${dirPath}`);
    console.log('[Monitoramento] Nova pasta detectada:', dirPath);
  } catch (e) {
    console.error('[Monitoramento] Erro ao inserir log de pasta:', e.message);
  }
});

// Modificação em arquivos
watcher.on('change', async (filePath) => {
  eventosRecentes.push(Date.now());
  eventosRecentes = eventosRecentes.filter(t => t > Date.now() - INTERVALO_MS);
  if (eventosRecentes.length > LIMITE_EVENTOS) {
    await logCriptografiaSuspeita(`Muitos arquivos modificados em sequência: ${eventosRecentes.length} em ${INTERVALO_MS / 1000}s!`);
    console.log('[Monitoramento] ALERTA: Alterações suspeitas detectadas!');
    eventosRecentes = [];
  }
});

watcher.on('unlink', async (filePath) => {
  try {
    await logArquivoRemovido(filePath);
    console.log('[Monitoramento] Arquivo removido detectado:', filePath);
  } catch (e) {
    console.error('[Monitoramento] Erro ao inserir log de remoção:', e.message);
  }
});
// watcher.on('unlink', ...); // Opcional: monitorar deleção

watcher.on('error', error => {
  console.error('[MONITORAMENTO] Erro:', error);
});

// ====== EXPRESS & API REST ======
const app = express();
app.use(cors());
app.use(express.json());
app.use(express.static(path.join(__dirname, 'Views')));

// Rotas da API (backend)
const logsRouter = require('./Controllers/LogsController');
app.use('/api', logsRouter);  // Prefixo '/api' para backend

// Rota principal para index.html
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'Views', 'Home', 'index.html'));
});

// Rota para logs.html
app.get('/logs-page', (req, res) => {
  res.sendFile(path.join(__dirname, 'Views', 'Logs', 'logs.html'));
});

app.listen(3000, () => {
  console.log('✅ Servidor rodando em http://localhost:3000');
});
