import hvac


class VaultClient:
    def __init__(self, url, token):
        self._client = hvac.Client(
            url=url,
            token=token
        )

    # 🔥 acceso directo al cliente real (para casos avanzados)
    @property
    def raw(self):
        return self._client

    # ✅ lo que necesitas para bootstrap
    @property
    def sys(self):
        return self._client.sys

    @property
    def secrets(self):
        return self._client.secrets