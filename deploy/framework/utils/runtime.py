import os

def is_running_in_docker():
    return os.path.exists("/.dockerenv")