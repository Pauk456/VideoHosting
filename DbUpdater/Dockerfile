# ���������� ����������� ����� .NET ��� ������
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# �������� ����� ������� � ��������������� �����������
COPY DbUpdater.csproj .
RUN dotnet restore

# �������� ���� ��� � �������� ������
COPY . .
RUN dotnet publish -c Release -o /app

# ���������� ����� ��� �������
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# ��������� ���� 5008
EXPOSE 5008

# ��������� ����������
ENTRYPOINT ["dotnet", "DbUpdater.dll"]