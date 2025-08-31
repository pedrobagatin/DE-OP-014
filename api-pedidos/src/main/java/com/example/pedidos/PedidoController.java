package com.example.pedidos;

import org.springframework.web.bind.annotation.*;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

@RestController
@RequestMapping("/orders")
public class PedidoController {

    private Map<String, Pedido> pedidos = new ConcurrentHashMap<>();

    @PostMapping
    public Pedido create(@RequestBody Pedido pedido) {
        String id = UUID.randomUUID().toString();
        pedido.setId(id);
        pedidos.put(id, pedido);
        return pedido;
    }

    @GetMapping
    public Collection<Pedido> list() {
        return pedidos.values();
    }

    @GetMapping("/{id}")
    public Pedido get(@PathVariable String id) {
        Pedido pedido = pedidos.get(id);
        if (pedido == null) throw new NoSuchElementException("Pedido não encontrado");
        return pedido;
    }

    @PutMapping("/{id}")
    public Pedido update(@PathVariable String id, @RequestBody Pedido updated) {
        Pedido pedido = pedidos.get(id);
        if (pedido == null) throw new NoSuchElementException("Pedido não encontrado");
        pedido.setCustomer(updated.getCustomer());
        pedido.setItems(updated.getItems());
        pedido.setTotal(updated.getTotal());
        return pedido;
    }

    @DeleteMapping("/{id}")
    public Pedido delete(@PathVariable String id) {
        Pedido pedido = pedidos.remove(id);
        if (pedido == null) throw new NoSuchElementException("Pedido não encontrado");
        return pedido;
    }
}