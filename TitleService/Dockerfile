# ���������� ����������� ����� .NET ��� ������
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# �������� ����� ������� � ��������������� �����������
COPY TitleService.csproj .
RUN dotnet restore

# �������� ���� ��� � �������� ������
COPY . .
RUN dotnet publish -c Release -o /app

# ���������� ����� ��� �������
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# ��������� ���� 5006
EXPOSE 5006

# ��������� ����������
ENTRYPOINT ["dotnet", "TitleService.dll"]