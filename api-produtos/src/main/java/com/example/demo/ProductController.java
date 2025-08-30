package com.example.demo;

import org.springframework.web.bind.annotation.*;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

@RestController
@RequestMapping("/products")
public class ProductController {

    private final Map<String, Product> db = new ConcurrentHashMap<>();

    @GetMapping
    public List<Product> getAll() {
        return new ArrayList<>(db.values());
    }

    @GetMapping("/{id}")
    public Product getById(@PathVariable String id) {
        Product product = db.get(id);
        if (product == null) throw new RuntimeException("Produto não encontrado");
        return product;
    }

    @PostMapping
    @ResponseStatus(org.springframework.http.HttpStatus.CREATED)
    public Product create(@RequestBody Product product) {
        String id = UUID.randomUUID().toString();
        product.setId(id);
        db.put(id, product);
        return product;
    }

    @PutMapping("/{id}")
    public Product update(@PathVariable String id, @RequestBody Product update) {
        Product product = db.get(id);
        if (product == null) throw new RuntimeException("Produto não encontrado");

        if (update.getName() != null) product.setName(update.getName());
        if (update.getPrice() != 0.0) product.setPrice(update.getPrice());
        if (update.getQuantity() != 0) product.setQuantity(update.getQuantity());

        db.put(id, product);
        return product;
    }

    @DeleteMapping("/{id}")
    @ResponseStatus(org.springframework.http.HttpStatus.NO_CONTENT)
    public void delete(@PathVariable String id) {
        if (!db.containsKey(id)) throw new RuntimeException("Produto não encontrado");
        db.remove(id);
    }
}