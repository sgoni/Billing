# ✅ Checklist Stage Environment (Axenta)

## 1. Docker Compose
- Quitar `env_file:` y usar solo `--env-file` desde tu `deploy.py`.
- Usar volúmenes persistentes para:
  - Postgres (`/var/lib/postgresql/data`)
  - Vault (`/vault/file`, `/vault/logs`)
  - Consul (`/consul/data`)
- Mantener solo los puertos que realmente necesites expuestos.

## 2. Environments (`.env.stage`)
- Solo dejar valores no sensibles (ej. nombres de contenedor, hosts internos).
- **No guardar tokens o contraseñas en `.env.stage`.**
- Los secrets se deben inyectar desde el pipeline (Jenkins, GitHub Actions, etc.).

## 3. Vault
- Usar `vault.json` con storage `file` y listener `tcp`.
- Ejecutar `init_vault.py` después de `up`.
- Guardar las claves/tokens generados en un gestor de secretos (**no en git**).
- Configurar policies y tokens por servicio (ej. `postgres-reader`, `consul-admin`, `rabbitmq-publisher`).

## 4. Consul
- Persistencia con `/consul/data`.
- Si tienes varios servicios, configurar **service definitions** en Consul para registrarlos automáticamente.
- Usar healthchecks internos para validar que los servicios estén vivos.

## 5. Postgres
- Volumen persistente para no perder datos.
- Backups automáticos en Stage (aunque sea un `pg_dump` programado).
- Credenciales tomadas de Vault (cuando lo integres).

## 6. Logging y Observabilidad
- Redirigir logs de Vault/Consul/Postgres a volúmenes (`/vault/logs`, etc.).
- Integrar con tu stack de observabilidad (Grafana, Prometheus, Graylog).
- Revisar `healthcheck` en todos los servicios críticos.

## 7. Scripts Python
- `deploy.py` debe poder levantar/bajar por ambiente (`dev`, `stage`, `prod`).
- Validar estado de contenedores (salida clara en logs).
- Integrarse con `init_vault.py` solo en Stage/Prod.
- `init_vault.py` debe generar los secrets en `deploy/secrets/` (que está en `.gitignore`).

## 8. Seguridad
- Asegurar que `deploy/secrets/` esté ignorado en `.gitignore`.
- Nunca exponer el `root_token` de Vault en logs del pipeline.
- En Stage/Prod, usar **TLS** en Vault y Consul si los expones a redes externas.
