const express = require("express");
const app = express();
app.use(express.json());

// "Banco" em memória
let products = [];
let nextId = 1;

// Criar produto
app.post("/products", (req, res) => {
  const { name, quantity, price } = req.body;

  if (!name || quantity === undefined || price === undefined) {
    return res.status(400).json({ error: "Dados inválidos" });
  }

  const product = {
    id: nextId++,
    name,
    quantity,
    price,
    createdAt: new Date(),
  };

  products.push(product);
  res.status(201).json(product);
});

// Listar todos
app.get("/products", (req, res) => {
  res.json(products);
});

// Buscar por ID
app.get("/products/:id", (req, res) => {
  const id = parseInt(req.params.id);
  const product = products.find(p => p.id === id);

  if (!product) {
    return res.status(404).json({ error: "Produto não encontrado" });
  }

  res.json(product);
});

// Atualizar
app.put("/products/:id", (req, res) => {
  const id = parseInt(req.params.id);
  const product = products.find(p => p.id === id);

  if (!product) {
    return res.status(404).json({ error: "Produto não encontrado" });
  }

  const { name, quantity, price } = req.body;

  if (name) product.name = name;
  if (quantity !== undefined) product.quantity = quantity;
  if (price !== undefined) product.price = price;

  res.json(product);
});

// Remover
app.delete("/products/:id", (req, res) => {
  const id = parseInt(req.params.id);
  const index = products.findIndex(p => p.id === id);

  if (index === -1) {
    return res.status(404).json({ error: "Produto não encontrado" });
  }

  const removed = products.splice(index, 1);
  res.json(removed[0]);
});

// Iniciar servidor
const PORT = 3000;
app.listen(PORT, () => {
  console.log(`📦 API de Estoque rodando em http://localhost:${PORT}`);
});