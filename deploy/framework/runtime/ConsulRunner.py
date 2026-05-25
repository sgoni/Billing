import os


class ConsulRunner:

    def __init__(self, consul_manager):
        self.consul = consul_manager

    def register(self, services):
        print("🧭 Consul registration starting...")

        for svc in services:
            print(f"DEBUG → {svc.name} | consul: {getattr(svc, 'consul', None)}")

            if not getattr(svc, "consul", None) or not svc.consul.enabled:
                continue

            print(f"🧭 Registering service in Consul: {svc.name}")

            address = self._resolve_address(svc)
            check = self._build_check(svc, address)

            self.consul.register_service(
                name=svc.consul.service_name or svc.name,
                service_id=svc.consul.service_id or f"{svc.name}-1",
                address=address,
                port=svc.connection.port,
                check=check,
                tags=self._build_tags(svc)
            )

        print("✅ Consul registration completed")

    # =========================
    # ADDRESS RESOLUTION (portable)
    # =========================
    def _resolve_address(self, svc):
        mode = os.getenv("DEPLOY_MODE", "docker")  # docker | host | hybrid

        if mode == "docker":
            return svc.connection.internal_host

        elif mode == "host":
            return svc.connection.external_host

        elif mode == "hybrid":
            return "host.docker.internal"

        return svc.connection.external_host

    # =========================
    # HEALTH CHECK BUILDER
    # =========================
    def _build_check(self, svc, address):
        svc_type = svc.type.lower()

        if svc_type == "postgres":
            return {
                "TCP": f"{address}:{svc.connection.port}",
                "Interval": "10s",
                "Timeout": "5s"
            }

        elif svc_type == "rabbitmq":
            # usa management port
            if svc.connection.management_port:
                return {
                    "HTTP": f"http://{address}:{svc.connection.management_port}/api/overview",
                    "Interval": "10s",
                    "Timeout": "5s"
                }

        # fallback genérico
        return {
            "TCP": f"{address}:{svc.connection.port}",
            "Interval": "15s"
        }

    # =========================
    # TAGS (observabilidad)
    # =========================
    def _build_tags(self, svc):
        tags = [svc.type]

        if svc.type == "postgres":
            tags.append("db")

        if svc.type == "rabbitmq":
            tags.append("mq")

        return tags
