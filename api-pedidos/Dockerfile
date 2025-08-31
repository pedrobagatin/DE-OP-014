FROM eclipse-temurin:17-jdk-alpine

WORKDIR /app

COPY pom.xml .
COPY src ./src

# Build do jar
RUN ./mvnw clean package -DskipTests || mvn clean package -DskipTests

EXPOSE 8080

CMD ["java", "-jar", "target/pedidos-api-1.0.0.jar"]