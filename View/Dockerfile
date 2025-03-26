# Используем официальный образ .NET для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY View.csproj .
RUN dotnet restore

# Копируем весь код и собираем проект
COPY . .
RUN dotnet publish -c Release -o /app

# Используем образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Открываем порт 5000
EXPOSE 5000

# Запускаем приложение
ENTRYPOINT ["dotnet", "View.dll"]