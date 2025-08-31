package com.example.estoque;

import org.springframework.web.bind.annotation.*;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

@RestController
@RequestMapping("/products")
public class ProdutoController {

    private Map<String, Produto> produtos = new ConcurrentHashMap<>();

    @PostMapping
    public Produto create(@RequestBody Produto produto) {
        String id = UUID.randomUUID().toString();
        produto.setId(id);
        produtos.put(id, produto);
        return produto;
    }

    @GetMapping
    public Collection<Produto> list() {
        return produtos.values();
    }

    @GetMapping("/{id}")
    public Produto get(@PathVariable String id) {
        Produto produto = produtos.get(id);
        if (produto == null) throw new NoSuchElementException("Produto não encontrado");
        return produto;
    }

    @PutMapping("/{id}")
    public Produto update(@PathVariable String id, @RequestBody Produto updated) {
        Produto produto = produtos.get(id);
        if (produto == null) throw new NoSuchElementException("Produto não encontrado");
        produto.setName(updated.getName());
        produto.setQuantity(updated.getQuantity());
        produto.setPrice(updated.getPrice());
        return produto;
    }

    @DeleteMapping("/{id}")
    public Produto delete(@PathVariable String id) {
        Produto produto = produtos.remove(id);
        if (produto == null) throw new NoSuchElementException("Produto não encontrado");
        return produto;
    }
}