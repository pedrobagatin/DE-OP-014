LABEL org.opencontainers.image.source="https://github.com/luigicfilho/luigicfilho"
# Usa imagem oficial do Node.js
FROM node:18-alpine

# Define diretório de trabalho
WORKDIR /usr/src/app

# Copia package.json e package-lock.json
COPY package*.json ./

# Instala dependências
RUN npm install

# Copia os arquivos da aplicação
COPY . .

# Expõe a porta
EXPOSE 3000

# Comando para rodar a aplicação
CMD ["node", "app.js"]