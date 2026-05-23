import os

import hvac
from dotenv import load_dotenv


def get_vault_role(svc):
    return svc.get("vault", {}).get("role_name")


class VaultManager:

    def __init__(self):
        load_dotenv()

        url = os.getenv("VAULT_URL")
        token = os.getenv("VAULT_TOKEN")

        if not url or not token:
            raise Exception("Missing VAULT config")

        self.client = hvac.Client(
            url=url,
            token=token
        )

        if not self.client.is_authenticated():
            raise Exception("Vault authentication failed")

    def inject_postgres(self, svc, context):
        creds = self.client.generate_db_creds(svc["name"])

        svc["user"] = creds["username"]
        svc["password"] = creds["password"]

        print(f"🔐 Vault injected Postgres creds for {svc['name']}")

    def inject_rabbitmq(self, svc, context):
        role = get_vault_role(svc)
        if not role:
            return

        creds = self.client.read(f"rabbitmq/creds/{role}")

        svc["user"] = creds["data"]["username"]
        svc["password"] = creds["data"]["password"]

        print(f"🔐 Vault injected RabbitMQ creds for {svc['name']}")