use axum::{
    extract::{Path, State},
    routing::{get, post, put, delete},
    Json, Router,
};
use serde::{Deserialize, Serialize};
use std::{net::SocketAddr, sync::{Arc, Mutex}};
use uuid::Uuid;

#[derive(Serialize, Deserialize, Clone)]
struct Product {
    id: String,
    name: String,
    quantity: i32,
    price: f64,
}

#[derive(Default)]
struct AppState {
    products: Mutex<Vec<Product>>,
}

#[tokio::main]
async fn main() {
    let state = Arc::new(AppState::default());

    let app = Router::new()
        .route("/products", post(create_product).get(list_products))
        .route("/products/:id", get(get_product).put(update_product).delete(delete_product))
        .with_state(state);

    let addr = SocketAddr::from(([0, 0, 0, 0], 3000));
    println!("📦 API de Estoque rodando em http://{}", addr);
    axum::serve(tokio::net::TcpListener::bind(addr).await.unwrap(), app).await.unwrap();
}

async fn create_product(
    State(state): State<Arc<AppState>>,
    Json(payload): Json<Product>,
) -> Json<Product> {
    let mut products = state.products.lock().unwrap();
    let mut product = payload;
    product.id = Uuid::new_v4().to_string();
    products.push(product.clone());
    Json(product)
}

async fn list_products(State(state): State<Arc<AppState>>) -> Json<Vec<Product>> {
    let products = state.products.lock().unwrap();
    Json(products.clone())
}

async fn get_product(Path(id): Path<String>, State(state): State<Arc<AppState>>) -> Option<Json<Product>> {
    let products = state.products.lock().unwrap();
    products.iter().find(|p| p.id == id).cloned().map(Json)
}

async fn update_product(
    Path(id): Path<String>,
    State(state): State<Arc<AppState>>,
    Json(payload): Json<Product>,
) -> Option<Json<Product>> {
    let mut products = state.products.lock().unwrap();
    if let Some(product) = products.iter_mut().find(|p| p.id == id) {
        product.name = payload.name;
        product.quantity = payload.quantity;
        product.price = payload.price;
        return Some(Json(product.clone()));
    }
    None
}

async fn delete_product(Path(id): Path<String>, State(state): State<Arc<AppState>>) -> Option<Json<Product>> {
    let mut products = state.products.lock().unwrap();
    if let Some(pos) = products.iter().position(|p| p.id == id) {
        return Some(Json(products.remove(pos)));
    }
    None
}