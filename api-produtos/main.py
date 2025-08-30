from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import Optional
import uuid

# Modelos
class Product(BaseModel):
    name: str
    price: float
    quantity: int

class CreateProduct(Product):
    pass

class UpdateProduct(BaseModel):
    name: Optional[str] = None
    price: Optional[float] = None
    quantity: Optional[int] = None

# App FastAPI
app = FastAPI(title="API de Produtos", version="1.0")

# Banco de dados em memória
db = {}

# Rotas

@app.get("/products")
def list_products():
    return list(db.values())

@app.get("/products/{id}")
def get_product(id: str):
    product = db.get(id)
    if not product:
        raise HTTPException(status_code=404, detail="Produto não encontrado")
    return product

@app.post("/products", status_code=201)
def create_product(product: CreateProduct):
    id = str(uuid.uuid4())
    new_product = {"id": id, **product.dict()}
    db[id] = new_product
    return new_product

@app.put("/products/{id}")
def update_product(id: str, data: UpdateProduct):
    product = db.get(id)
    if not product:
        raise HTTPException(status_code=404, detail="Produto não encontrado")

    updated = data.dict(exclude_unset=True)  # Apenas campos enviados
    for key, value in updated.items():
        product[key] = value

    db[id] = product
    return product

@app.delete("/products/{id}", status_code=204)
def delete_product(id: str):
    if db.pop(id, None) is None:
        raise HTTPException(status_code=404, detail="Produto não encontrado")
    return None  # 204 No Content