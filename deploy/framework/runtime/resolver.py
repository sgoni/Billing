import os


def running_in_docker() -> bool:
    return os.path.exists("/.dockerenv")


def resolve_host(svc):
    conn = svc.connection

    if running_in_docker():
        return conn.internal_host
    return conn.external_host
