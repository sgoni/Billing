import requests


class ConsulManager:

    def __init__(self, host="localhost", port=8500):
        self.base_url = f"http://{host}:{port}"  # ✅ FIX

    def register_service(self, name, service_id, address, port):
        payload = {
            "Name": name,
            "ID": service_id,
            "Address": address,
            "Port": port,
            "Check": {
                "TCP": f"{address}:{port}",
                "Interval": "10s",
                "Timeout": "5s"
            }
        }

        response = requests.put(
            f"{self.base_url}/v1/agent/service/register",
            json=payload
        )

        print(response.status_code, response.text)
