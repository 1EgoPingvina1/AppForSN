# Angular 17 Dockerfile
FROM node:18-alpine as build

WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

# Stage 2 - Production with nginx
FROM nginx:alpine

# Копируем Angular приложение
COPY --from=build /app/dist/dtc.admin-test-client/browser /usr/share/nginx/html

# Копируем SSL сертификаты
COPY ssl/ /etc/nginx/ssl/

# Копируем nginx config
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 443

CMD ["nginx", "-g", "daemon off;"]