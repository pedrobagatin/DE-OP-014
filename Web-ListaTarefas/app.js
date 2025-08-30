require('dotenv').config();
const express = require('express');
const path = require('path');
const sql = require('mssql');

const app = express();
const PORT = process.env.PORT || 3000;

// ==============================
// Configuração do Banco: Tenta Azure SQL ou fallback para memória
// ==============================

let useSql = true; // Controla qual banco usar
let todos = [];   // Armazenamento em memória (usado no fallback)

// Configuração do Azure SQL
const dbConfig = {
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  server: process.env.DB_SERVER,
  database: process.env.DB_NAME,
  port: 1433,
  options: {
    encrypt: true,
    trustServerCertificate: false,
    enableArithAbort: true
  }
};
// Middleware
app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));
app.use(express.static(path.join(__dirname, 'public')));
app.use(express.urlencoded({ extended: true })); // Para receber dados do formulário

// Conexão com o banco
async function connectToDB() {
  try {
    await sql.connect(dbConfig);
    console.log('✅ Conectado ao Azure SQL Database');
  } catch (err) {
    console.error('❌ Erro ao conectar ao banco:', err.message);
    process.exit(1);
  }
}

// Função: Inicializa o banco (SQL ou fallback)
async function initializeDatabase() {
  if (!dbConfig.user || !dbConfig.password || !dbConfig.server || !dbConfig.database) {
    console.warn('⚠️ Variáveis de ambiente do banco ausentes. Usando armazenamento em memória.');
    setupInMemoryDB();
    return;
  }

  try {
    await sql.connect(dbConfig);
    console.log('✅ Conectado ao Azure SQL Database');

    // Cria tabela se não existir
    await sql.query(`
      IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Todos' AND xtype='U')
      CREATE TABLE Todos (
        id INT IDENTITY(1,1) PRIMARY KEY,
        task NVARCHAR(255) NOT NULL,
        done BIT NOT NULL DEFAULT 0
      );
    `);
    console.log('✅ Tabela "Todos" pronta no Azure SQL');
  } catch (err) {
    console.error('❌ Falha ao conectar ao Azure SQL:', err.message);
    console.log('🔁 Mudando para armazenamento em memória...\n');
    useSql = false;
    setupInMemoryDB();
  }
}

// Configura o banco em memória com dados iniciais
function setupInMemoryDB() {
  todos = [
    { id: 1, task: 'Estudar Azure', done: false },
    { id: 2, task: 'Fazer o projeto de To-Do', done: true }
  ];
  console.log('💡 Usando armazenamento em memória (fallback). Dados iniciais carregados.');
}

// ==============================
// Funções de acesso ao banco (genéricas)
// ==============================

async function getAllTodos() {
  if (useSql) {
    const result = await sql.query`SELECT * FROM Todos ORDER BY id`;
    return result.recordset;
  } else {
    return todos;
  }
}

async function addTodo(task) {
  if (useSql) {
    await sql.query`INSERT INTO Todos (task, done) VALUES (${task}, 0)`;
  } else {
    const id = todos.length > 0 ? Math.max(...todos.map(t => t.id)) + 1 : 1;
    todos.push({ id, task, done: false });
  }
}

async function toggleTodo(id) {
  if (useSql) {
    const result = await sql.query`SELECT done FROM Todos WHERE id = ${id}`;
    if (result.recordset.length > 0) {
      const current = result.recordset[0].done;
      await sql.query`UPDATE Todos SET done = ${current ? 0 : 1} WHERE id = ${id}`;
    }
  } else {
    const todo = todos.find(t => t.id === id);
    if (todo) todo.done = !todo.done;
  }
}

async function deleteTodo(id) {
  if (useSql) {
    await sql.query`DELETE FROM Todos WHERE id = ${id}`;
  } else {
    todos = todos.filter(t => t.id !== id);
  }
}

// Rota principal
app.get('/', async (req, res) => {
  try {
    const todos = await getAllTodos();
    const total = todos.length;
    const completed = todos.filter(t => t.done).length;
    const progress = total > 0 ? Math.round((completed / total) * 100) : 0;

    res.render('index', { todos, progress, completed, total });
  } catch (err) {
    console.error('Erro ao carregar tarefas:', err);
    res.status(500).send('Erro ao carregar tarefas');
  }
});

// Adicionar nova tarefa
app.post('/add', async (req, res) => {
  const task = req.body.task.trim();
  if (task) {
    await addTodo(task);
  }
  res.redirect('/');
});

// Marcar como concluída
app.post('/toggle/:id', async (req, res) => {
  const id = parseInt(req.params.id);
  await toggleTodo(id);
  res.redirect('/');
});

// Deletar tarefa
app.post('/delete/:id', async (req, res) => {
  const id = parseInt(req.params.id);
  await deleteTodo(id);
  res.redirect('/');
});

// ==============================
// Inicialização
// ==============================

initializeDatabase();

// Iniciar servidor
app.listen(PORT, () => {
  console.log(`✅ Servidor rodando em http://localhost:${PORT}`);
  console.log(`🔧 Modo de armazenamento: ${useSql ? 'Azure SQL' : 'Memória (fallback)'}`);
});

module.exports = app;