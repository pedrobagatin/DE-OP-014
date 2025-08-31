const express = require("express");
const app = express();
app.use(express.json());

// "Banco de dados" em memória
let orders = [];
let nextId = 1;

// Criar pedido (Create)
app.post("/orders", (req, res) => {
  const { customer, items, total } = req.body;

  if (!customer || !items || !total) {
    return res.status(400).json({ error: "Dados inválidos" });
  }

  const order = {
    id: nextId++,
    customer,
    items,
    total,
    createdAt: new Date(),
  };

  orders.push(order);
  res.status(201).json(order);
});

// Listar pedidos (Read All)
app.get("/orders", (req, res) => {
  res.json(orders);
});

// Obter pedido por ID (Read One)
app.get("/orders/:id", (req, res) => {
  const id = parseInt(req.params.id);
  const order = orders.find(o => o.id === id);

  if (!order) {
    return res.status(404).json({ error: "Pedido não encontrado" });
  }

  res.json(order);
});

// Atualizar pedido (Update)
app.put("/orders/:id", (req, res) => {
  const id = parseInt(req.params.id);
  const order = orders.find(o => o.id === id);

  if (!order) {
    return res.status(404).json({ error: "Pedido não encontrado" });
  }

  const { customer, items, total } = req.body;

  if (customer) order.customer = customer;
  if (items) order.items = items;
  if (total) order.total = total;

  res.json(order);
});

// Remover pedido (Delete)
app.delete("/orders/:id", (req, res) => {
  const id = parseInt(req.params.id);
  const index = orders.findIndex(o => o.id === id);

  if (index === -1) {
    return res.status(404).json({ error: "Pedido não encontrado" });
  }

  const removed = orders.splice(index, 1);
  res.json(removed[0]);
});

// Inicializar servidor
const PORT = 3000;
app.listen(PORT, () => {
  console.log(`🚀 Servidor rodando em http://localhost:${PORT}`);
});