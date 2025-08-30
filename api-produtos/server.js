const express = require('express');
const { v4: uuidv4 } = require('uuid');
const app = express();

app.use(express.json());

const db = new Map();

// Listar todos
app.get('/products', (req, res) => {
  res.json(Array.from(db.values()));
});

// Buscar por ID
app.get('/products/:id', (req, res) => {
  const product = db.get(req.params.id);
  if (!product) return res.status(404).json({ error: 'Produto não encontrado' });
  res.json(product);
});

// Criar
app.post('/products', (req, res) => {
  const { name, price, quantity } = req.body;
  const id = uuidv4();
  const product = { id, name, price, quantity };
  db.set(id, product);
  res.status(201).json(product);
});

// Atualizar
app.put('/products/:id', (req, res) => {
  const product = db.get(req.params.id);
  if (!product) return res.status(404).json({ error: 'Produto não encontrado' });

  const { name, price, quantity } = req.body;
  if (name) product.name = name;
  if (price !== undefined) product.price = price;
  if (quantity !== undefined) product.quantity = quantity;

  db.set(req.params.id, product);
  res.json(product);
});

// Deletar
app.delete('/products/:id', (req, res) => {
  if (!db.delete(req.params.id)) {
    return res.status(404).json({ error: 'Produto não encontrado' });
  }
  res.status(204).send();
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`API Node.js rodando na porta ${PORT}`);
});