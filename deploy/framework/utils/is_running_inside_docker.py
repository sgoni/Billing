import os

def is_running_inside_docker():
    return os.path.exists("/.dockerenv")