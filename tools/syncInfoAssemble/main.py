from __future__ import annotations

import json
from pathlib import Path


def build_sync_info() -> None:
    """Assemble sync_info.json from base.json and localized change logs."""
    root = Path(__file__).parent
    input_dir = root / "input"
    output_dir = root / "output"

    base_path = input_dir / "base.json"
    with base_path.open("r", encoding="utf-8") as fp:
        data = json.load(fp)

    descriptions = data.setdefault("shortDescription", {})

    for txt_file in sorted(input_dir.glob("*.txt")):
        lang = txt_file.stem
        content = txt_file.read_text(encoding="utf-8").strip()
        descriptions[lang] = content

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / "sync_info.json"
    with output_path.open("w", encoding="utf-8") as fp:
        json.dump(data, fp, ensure_ascii=False, indent=4)
    print(f"Файл {output_path.name} успешно собран в {output_dir}")


if __name__ == "__main__":
    build_sync_info()
