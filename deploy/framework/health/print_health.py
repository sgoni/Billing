def print_health(summary):
    print("\n📊 SYSTEM HEALTH\n")

    for s in summary:
        print(f"{s['name']} [{s['type']}] → {s['status']}")
