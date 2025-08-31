FROM rust:1.82 as builder

WORKDIR /app

# Copiar e compilar
COPY Cargo.toml Cargo.lock ./
COPY src ./src

RUN cargo build --release

# Imagem final
FROM debian:bookworm-slim
WORKDIR /app
COPY --from=builder /app/target/release/api-estoque /app/api

EXPOSE 3000

CMD ["./api"]