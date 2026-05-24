from typing import List, Optional, Dict, Any
from pydantic import BaseModel, Field, model_validator


# -----------------------------
# CONNECTION (RESPETA TU YAML)
# -----------------------------
class Connection(BaseModel):
    internal_host: str
    external_host: str

    port: Optional[int] = None
    management_port: Optional[int] = None

    database: Optional[str] = None

    admin_user: Optional[str] = None
    admin_password: Optional[str] = None


# -----------------------------
# VAULT ROLE (COMPATIBLE PG + RABBIT)
# -----------------------------
class VaultRole(BaseModel):
    name: str

    # Postgres
    ttl: Optional[str] = None
    max_ttl: Optional[str] = None
    permissions: Optional[List[str]] = None

    # RabbitMQ
    tags: Optional[str] = None
    vhosts: Optional[Dict[str, Any]] = None


# -----------------------------
# VAULT CONFIG
# -----------------------------
class VaultConfig(BaseModel):
    enabled: bool = False
    engine: Optional[str] = None
    connection_name: Optional[str] = None

    roles: List[VaultRole] = []


# -----------------------------
# SERVICE
# -----------------------------
class Service(BaseModel):
    name: str
    type: str

    connection: Connection
    vault: Optional[VaultConfig] = None

    # 🔥 VALIDACIÓN CENTRAL (SIN ROMPER TU MODELO)
    @model_validator(mode="after")
    def validate_service(self):
        conn = self.connection

        # -------------------------
        # POSTGRES
        # -------------------------
        if self.type == "postgres":
            if conn.port is None:
                raise ValueError(f"{self.name}: postgres requires connection.port")

            if conn.database is None:
                raise ValueError(f"{self.name}: postgres requires connection.database")

            if not conn.admin_user or not conn.admin_password:
                raise ValueError(f"{self.name}: postgres requires admin credentials")

        # -------------------------
        # RABBITMQ
        # -------------------------
        if self.type == "rabbitmq":
            if conn.port is None:
                raise ValueError(f"{self.name}: rabbitmq requires connection.port")

            if conn.management_port is None:
                raise ValueError(f"{self.name}: rabbitmq requires connection.management_port")

            if not conn.admin_user or not conn.admin_password:
                raise ValueError(f"{self.name}: rabbitmq requires admin credentials")

        # -------------------------
        # VAULT (SI ESTÁ HABILITADO)
        # -------------------------
        if self.vault and self.vault.enabled:
            if not self.vault.engine:
                raise ValueError(f"{self.name}: vault.engine is required when enabled")

            if not self.vault.roles:
                raise ValueError(f"{self.name}: vault.roles must not be empty")

        return self


# -----------------------------
# ROOT
# -----------------------------
class ServicesConfig(BaseModel):
    services: List[Service]