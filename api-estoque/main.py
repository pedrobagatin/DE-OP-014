from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
from uuid import uuid4

app = FastAPI()

class Product(BaseModel):
    id: str = None
    name: str
    quantity: int
    price: float

products = []

@app.post("/products", response_model=Product, status_code=201)
def create_product(product: Product):
    product.id = str(uuid4())
    products.append(product)
    return product

@app.get("/products", response_model=List[Product])
def list_products():
    return products

@app.get("/products/{product_id}", response_model=Product)
def get_product(product_id: str):
    for product in products:
        if product.id == product_id:
            return product
    raise HTTPException(status_code=404, detail="Produto não encontrado")

@app.put("/products/{product_id}", response_model=Product)
def update_product(product_id: str, updated: Product):
    for product in products:
        if product.id == product_id:
            product.name = updated.name
            product.quantity = updated.quantity
            product.price = updated.price
            return product
    raise HTTPException(status_code=404, detail="Produto não encontrado")

@app.delete("/products/{product_id}", response_model=Product)
def delete_product(product_id: str):
    for i, product in enumerate(products):
        if product.id == product_id:
            return products.pop(i)
    raise HTTPException(status_code=404, detail="Produto não encontrado")