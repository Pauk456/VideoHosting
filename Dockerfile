# ���������� ����������� ����� watchtower ��� �������
FROM containrrr/watchtower:latest

# ������� �� ��������� (��� ������ � ������������ ������, �� ����� ��������������)
ENTRYPOINT ["/watchtower"]