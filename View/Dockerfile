# ���������� ����������� ����� .NET ��� ������
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# �������� ����� ������� � ��������������� �����������
COPY View.csproj .
RUN dotnet restore

# �������� ���� ��� � �������� ������
COPY . .
RUN dotnet publish -c Release -o /app

# ���������� ����� ��� �������
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# ��������� ���� 5000
EXPOSE 5000

# ��������� ����������
ENTRYPOINT ["dotnet", "View.dll"]