import requests
import socket
from framework.runtime.resolver import resolve_host


class PostgresHealth:

    def check(self, svc):
        try:
            host = resolve_host(svc)
            port = svc.connection.port

            print(f"🔌 Checking Postgres: {host}:{port}")

            with socket.create_connection((host, port), timeout=3):
                return True

        except Exception as e:
            print(f"❌ Postgres error: {e}")
            return False


class RabbitHealth:

    def check(self, svc):
        try:
            host = resolve_host(svc)
            port = svc.connection.management_port
            user = svc.connection.admin_user
            password = svc.connection.admin_password

            url = f"http://{host}:{port}/api/users/"

            r = requests.get(url, auth=(user, password), timeout=3)

            return r.status_code == 200

        except Exception:
            return False
