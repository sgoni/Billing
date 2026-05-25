import requests


class ConsulManager:

    def __init__(self, host="localhost", port=8500):
        self.base_url = f"http://{host}:{port}"

    def register_service(self, name, service_id, address, port, check=None, tags=None):
        payload = {
            "Name": name,
            "ID": service_id,
            "Address": address,
            "Port": port,
        }

        if check:
            payload["Check"] = check

        if tags:
            payload["Tags"] = tags

        response = requests.put(
            f"{self.base_url}/v1/agent/service/register",
            json=payload
        )

        print(f"{response.status_code} → {name}")
