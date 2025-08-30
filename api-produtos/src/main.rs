use actix_web::{web, App, HttpResponse, HttpServer, Responder, Result};
use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::sync::{Arc, Mutex};

// Estrutura do Produto
#[derive(Serialize, Deserialize, Clone)]
struct Product {
    id: String,
    name: String,
    price: f64,
    quantity: i32,
}

// Dados de entrada para criar/atualizar
#[derive(Deserialize)]
struct CreateProduct {
    name: String,
    price: f64,
    quantity: i32,
}

#[derive(Deserialize)]
struct UpdateProduct {
    name: Option<String>,
    price: Option<f64>,
    quantity: Option<i32>,
}

// Tipo para armazenar produtos em memória
type Db = Arc<Mutex<HashMap<String, Product>>>;

// Rota: GET /products
async fn get_products(db: web::Data<Db>) -> Result<impl Responder> {
    let products = db.lock().unwrap();
    let list: Vec<Product> = products.values().cloned().collect();
    Ok(HttpResponse::Ok().json(list))
}

// Rota: GET /products/{id}
async fn get_product(db: web::Data<Db>, path: web::Path<String>) -> Result<impl Responder> {
    let id = path.into_inner();
    let products = db.lock().unwrap();
    match products.get(&id) {
        Some(product) => Ok(HttpResponse::Ok().json(product)),
        None => Ok(HttpResponse::NotFound().json("Produto não encontrado")),
    }
}

// Rota: POST /products
async fn create_product(
    db: web::Data<Db>,
    new_product: web::Json<CreateProduct>,
) -> Result<impl Responder> {
    let product = Product {
        id: uuid::Uuid::new_v4().to_string(),
        name: new_product.name.clone(),
        price: new_product.price,
        quantity: new_product.quantity,
    };

    {
        let mut products = db.lock().unwrap();
        products.insert(product.id.clone(), product.clone());
    }

    Ok(HttpResponse::Created().json(product))
}

// Rota: PUT /products/{id}
async fn update_product(
    db: web::Data<Db>,
    path: web::Path<String>,
    update_data: web::Json<UpdateProduct>,
) -> Result<impl Responder> {
    let id = path.into_inner();
    let mut products = db.lock().unwrap();

    if let Some(product) = products.get_mut(&id) {
        if let Some(name) = &update_data.name {
            product.name = name.clone();
        }
        if let Some(price) = update_data.price {
            product.price = price;
        }
        if let Some(quantity) = update_data.quantity {
            product.quantity = quantity;
        }

        Ok(HttpResponse::Ok().json(product.clone()))
    } else {
        Ok(HttpResponse::NotFound().json("Produto não encontrado"))
    }
}

// Rota: DELETE /products/{id}
async fn delete_product(db: web::Data<Db>, path: web::Path<String>) -> Result<impl Responder> {
    let id = path.into_inner();
    let mut products = db.lock().unwrap();

    if products.remove(&id).is_some() {
        Ok(HttpResponse::NoContent().finish())
    } else {
        Ok(HttpResponse::NotFound().json("Produto não encontrado"))
    }
}

// Função principal
#[actix_web::main]
async fn main() -> std::io::Result<()> {
    // Banco de dados em memória compartilhada
    let db = Arc::new(Mutex::new(HashMap::<String, Product>::new()));
    // Inicia o servidor
    HttpServer::new(move || {
        App::new()
            .app_data(web::Data::new(db.clone()))
            .route("/products", web::get().to(get_products))
            .route("/products", web::post().to(create_product))
            .route("/products/{id}", web::get().to(get_product))
            .route("/products/{id}", web::put().to(update_product))
            .route("/products/{id}", web::delete().to(delete_product))
    })
    .bind("127.0.0.1:8080")?
    .run()
    .await
}