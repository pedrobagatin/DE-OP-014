# Imagem base
FROM node:18-alpine

# Diretório de trabalho no container
WORKDIR /app

# Copiar arquivos de dependências
COPY package*.json ./

# Instalar dependências
RUN npm install --production

# Copiar código da aplicação
COPY . .

# Expor a porta que a API usa
EXPOSE 3000

# Comando para iniciar o servidor
CMD ["node", "server.js"]