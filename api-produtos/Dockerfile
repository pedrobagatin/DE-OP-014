# Etapa 1: Compilação (multi-stage)
FROM rust:1.75 AS builder

WORKDIR /app

# Copia o Cargo.toml e Cargo.lock
COPY Cargo.toml .

# Cria um arquivo vazio para compilar dependências primeiro
RUN mkdir src && echo 'fn main(){}' > src/main.rs
RUN cargo build --release

# Substitui o src e compila o projeto real
COPY src ./src
RUN rm target/release/deps/api_produtos*
RUN cargo build --release

# Etapa 2: Imagem final pequena
FROM debian:bookworm-slim

# Instala dependências mínimas (opcional, só se precisar de ferramentas)
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copia o binário compilado do estágio anterior
COPY --from=builder /app/target/release/api-produtos .

# Expõe a porta 8080
EXPOSE 8080

# Comando para rodar o binário
CMD ["./api-produtos"]