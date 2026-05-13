import os
import psycopg2
import logging

# Configuración de logs
logging.basicConfig(level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s")

def validate_role_permissions(db_name: str, role_name: str):
    """Valida si el rol tiene permisos sobre el esquema public y las tablas."""
    try:
        conn = psycopg2.connect(
            host=os.getenv("POSTGRES_HOST", "localhost"),
            port=os.getenv("POSTGRES_PORT", "5432"),
            dbname=db_name,
            user=os.getenv("POSTGRES_USER", "postgres"),
            password=os.getenv("POSTGRES_PASSWORD", "postgres")
        )

        with conn:
            with conn.cursor() as cur:
                # Verifica permiso de CREATE sobre el esquema public
                cur.execute(f"SELECT has_schema_privilege('{role_name}', 'public', 'CREATE');")
                create_priv = cur.fetchone()[0]

                # Verifica privilegios sobre tablas
                cur.execute(f"""
                    SELECT DISTINCT privilege_type
                    FROM information_schema.role_table_grants
                    WHERE grantee = %s AND table_schema = 'public';
                """, (role_name,))
                table_privs = [row[0] for row in cur.fetchall()]

                logging.info(f"\n🔍 Validación de rol: {role_name}")
                logging.info(f"✅ CREATE en esquema public: {'Sí' if create_priv else 'No'}")
                logging.info(f"✅ Privilegios sobre tablas: {', '.join(table_privs) if table_privs else 'Ninguno'}")

    except Exception as e:
        logging.error(f"❌ Error al validar el rol {role_name}: {e}")

if __name__ == "__main__":
    # Parámetros para ejecución manual
    db_name = os.getenv("POSTGRES_DB", "AccountingDb")
    role_name = "db-role"  # O el nombre del usuario dinámico si querés validar uno específico

    validate_role_permissions(db_name, role_name)