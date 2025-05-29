# »спользуем официальный образ watchtower как базовый
FROM containrrr/watchtower:latest

#  оманда по умолчанию (уже задана в оригинальном образе, но можно переопределить)
ENTRYPOINT ["/watchtower"]