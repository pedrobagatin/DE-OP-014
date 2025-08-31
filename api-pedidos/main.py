from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
from uuid import uuid4

app = FastAPI()

class Order(BaseModel):
    id: str = None
    customer: str
    items: List[str]
    total: float

orders = []

@app.post("/orders", response_model=Order, status_code=201)
def create_order(order: Order):
    order.id = str(uuid4())
    orders.append(order)
    return order

@app.get("/orders", response_model=List[Order])
def list_orders():
    return orders

@app.get("/orders/{order_id}", response_model=Order)
def get_order(order_id: str):
    for order in orders:
        if order.id == order_id:
            return order
    raise HTTPException(status_code=404, detail="Pedido não encontrado")

@app.put("/orders/{order_id}", response_model=Order)
def update_order(order_id: str, updated: Order):
    for order in orders:
        if order.id == order_id:
            order.customer = updated.customer
            order.items = updated.items
            order.total = updated.total
            return order
    raise HTTPException(status_code=404, detail="Pedido não encontrado")

@app.delete("/orders/{order_id}", response_model=Order)
def delete_order(order_id: str):
    for i, order in enumerate(orders):
        if order.id == order_id:
            return orders.pop(i)
    raise HTTPException(status_code=404, detail="Pedido não encontrado")