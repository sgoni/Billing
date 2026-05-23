import time
import requests

def wait_for_vault(url, timeout=30):
    start = time.time()

    while time.time() - start < timeout:
        try:
            res = requests.get(f"{url}/v1/sys/health", timeout=2)
            if res.status_code in [200, 429, 472, 473]:
                print("🔐 Vault is ready")
                return True
        except:
            pass

        print("⏳ Waiting for Vault...")
        time.sleep(2)

    raise Exception("Vault not ready")

def wait_for_postgres(host, port, timeout=30):
    import socket
    import time

    print(f"🔌 Trying {host}:{port}")

    start = time.time()

    while time.time() - start < timeout:
        try:
            with socket.create_connection((host, port), timeout=3):
                print("✅ Postgres ready")
                return
        except Exception as e:
            print(f"❌ {e}")
            time.sleep(2)

    raise Exception(f"Postgres {host}:{port} not ready")

def wait_rabbit(host, port, user, password, timeout=30):
    import time, requests

    for _ in range(timeout):
        try:
            r = requests.get(
                f"http://{host}:{port}/api/overview",
                auth=(user, password),
                timeout=2
            )

            if r.status_code in (200, 401):
                print("✅ RabbitMQ ready")
                return True
            else:
                print(f"⚠️ status: {r.status_code}")

        except Exception as e:
            print(f"❌ {e}")

        time.sleep(1)

    raise Exception("RabbitMQ management not ready")