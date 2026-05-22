import requests

class HttpProvider:

    def deploy(self, config, context):
        print(f"🌐 Checking {config['name']}")

        try:
            r = requests.get(config["url"], timeout=5)
            if r.status_code == 200:
                print("✅ Healthy")
            else:
                print("❌ Unhealthy")
        except Exception as e:
            print("❌ Error:", e)