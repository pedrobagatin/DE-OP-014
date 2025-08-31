use axum::{
    extract::{Path, State},
    routing::{get, post, put, delete},
    Json, Router,
};
use axum::response::IntoResponse;
use axum::http::StatusCode;
use serde::{Deserialize, Serialize};
use std::{net::SocketAddr, sync::{Arc, Mutex}};
use uuid::Uuid;

#[derive(Serialize, Deserialize, Clone)]
struct Order {
    id: String,
    customer: String,
    items: Vec<String>,
    total: f64,
}

#[derive(Default)]
struct AppState {
    orders: Mutex<Vec<Order>>,
}

#[tokio::main]
async fn main() {
    let state = Arc::new(AppState::default());

    let app = Router::new()
    .route("/orders", post(create_order).get(list_orders))
    .route("/orders/:id", get(get_order))
    .route("/orders/:id", put(update_order))
    .route("/orders/:id", delete(delete_order))
    .with_state(state);

    let addr = SocketAddr::from(([0, 0, 0, 0], 3000));
    println!("🚀 API de Pedidos rodando em http://{}", addr);
    axum::serve(tokio::net::TcpListener::bind(addr).await.unwrap(), app).await.unwrap();
}

async fn create_order(
    State(state): State<Arc<AppState>>,
    Json(payload): Json<Order>,
) -> Json<Order> {
    let mut orders = state.orders.lock().unwrap();
    let mut order = payload;
    order.id = Uuid::new_v4().to_string();
    orders.push(order.clone());
    Json(order)
}

async fn list_orders(State(state): State<Arc<AppState>>) -> Json<Vec<Order>> {
    let orders = state.orders.lock().unwrap();
    Json(orders.clone())
}

async fn get_order(
    Path(id): Path<String>,
    State(state): State<Arc<AppState>>,
) -> impl IntoResponse {
    let orders = state.orders.lock().unwrap();

    if let Some(order) = orders.iter().find(|o| o.id == id) {
        (axum::http::StatusCode::OK, Json(order.clone())).into_response()
    } else {
        (axum::http::StatusCode::NOT_FOUND, "Pedido não encontrado").into_response()
    }
}

async fn update_order(
    Path(id): Path<String>,
    State(state): State<Arc<AppState>>,
    Json(payload): Json<Order>,
) -> impl IntoResponse {
    let mut orders = state.orders.lock().unwrap();
    if let Some(order) = orders.iter_mut().find(|o| o.id == id) {
        order.customer = payload.customer;
        order.items = payload.items;
        order.total = payload.total;
        (StatusCode::OK, Json(order.clone())).into_response()
    } else {
        (StatusCode::NOT_FOUND, "Pedido não encontrado").into_response()
    }
}

async fn delete_order(
    Path(id): Path<String>,
    State(state): State<Arc<AppState>>,
) -> impl IntoResponse {
    let mut orders = state.orders.lock().unwrap();
    if let Some(pos) = orders.iter().position(|o| o.id == id) {
        let removed = orders.remove(pos);
        (StatusCode::OK, Json(removed)).into_response()
    } else {
        (StatusCode::NOT_FOUND, "Pedido não encontrado").into_response()
    }
}