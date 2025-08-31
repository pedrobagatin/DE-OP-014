package com.example.pedidos;

public class Pedido {
    private String id;
    private String customer;
    private String[] items;
    private double total;

    public Pedido() {}

    public Pedido(String id, String customer, String[] items, double total) {
        this.id = id;
        this.customer = customer;
        this.items = items;
        this.total = total;
    }

    // Getters e Setters
    public String getId() { return id; }
    public void setId(String id) { this.id = id; }
    public String getCustomer() { return customer; }
    public void setCustomer(String customer) { this.customer = customer; }
    public String[] getItems() { return items; }
    public void setItems(String[] items) { this.items = items; }
    public double getTotal() { return total; }
    public void setTotal(double total) { this.total = total; }
}