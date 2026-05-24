import time
import requests

from framework.runtime.health_factory import get_health_checker


def wait_for_service(svc, timeout=30):
    print(f"⏳ Waiting for {svc.type} {svc.name}...")

    checker = get_health_checker(svc)

    start = time.time()

    while time.time() - start < timeout:
        if checker.check(svc):
            print(f"✅ {svc.name} ready")
            return True

        time.sleep(2)

    raise TimeoutError(f"{svc.name} not ready after {timeout}s")


def wait_for_rabbitmq_management(host, port, user, password, timeout=60):
    print(f"⏳ Waiting for RabbitMQ management API...")

    url = f"http://{host}:{port}/api/overview"

    start = time.time()

    while True:
        try:
            r = requests.get(url, auth=(user, password), timeout=3)

            if r.status_code == 200:
                print("✅ RabbitMQ management ready")
                return

        except Exception:
            pass

        if time.time() - start > timeout:
            raise TimeoutError("RabbitMQ management API not ready")

        time.sleep(2)
