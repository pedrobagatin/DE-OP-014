# Etapa 1: Compila com Maven
FROM maven:3.8-openjdk-17 AS builder

WORKDIR /app
COPY pom.xml .
COPY src ./src

# Compila o projeto
RUN mvn clean package -DskipTests

# Etapa 2: Imagem final leve
FROM openjdk:17-jre-slim

WORKDIR /app
COPY --from=builder /app/target/*.jar app.jar

EXPOSE 8080

ENTRYPOINT ["java", "-jar", "app.jar"]