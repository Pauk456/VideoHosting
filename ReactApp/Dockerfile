# 1) Базовый образ с Node.js
FROM node:18-alpine

# 2) Рабочая папка
WORKDIR /app

# 3) Копируем package.json и lock-файл
COPY package.json package-lock.json ./

# 4) Устанавливаем зависимости
RUN npm ci

# 5) Копируем остальной код
COPY . .

# 6) Экспортируем порт, на котором работает Vite Dev Server
EXPOSE 3000

# 7) По умолчанию Vite слушает только localhost — принудительно слушаем все интерфейсы
ENV HOST=0.0.0.0
ENV PORT=3000

# 8) Запускаем ваш скрипт из package.json ("start": "vite --host 0.0.0.0 --port 3000")
CMD ["npm", "start"]
