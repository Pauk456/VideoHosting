# ���������� ����������� ����� .NET ��� ������
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# �������� ����� ������� � ��������������� �����������
COPY VideoAndImgService.csproj .
RUN dotnet restore

# �������� ���� ��� � �������� ������
COPY . .
RUN dotnet publish -c Release -o /app

# ���������� ����� ��� �������
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# ��������� ���� 5001
EXPOSE 5001

# ��������� ����������
ENTRYPOINT ["dotnet", "VideoAndImgService.dll"]