import requests

class HttpHealth:

    def check(self, config):
        try:
            r = requests.get(config["url"], timeout=3)
            return r.status_code == 200
        except:
            return False