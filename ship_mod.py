import os
import shutil
from pathlib import Path

def copy_directory(source_dir: str, destination_dir: str):
    source_path = Path(source_dir)
    destination_path = Path(destination_dir)
        
    destination_path.mkdir(parents=True, exist_ok=True)
        
    for item in source_path.iterdir():
        destination_filepath = destination_path / item.name
        if item.is_dir():
            if destination_filepath.exists():
                shutil.rmtree(destination_filepath)
            shutil.copytree(item, destination_filepath)
        else:
            shutil.copy2(item, destination_filepath)
            
    print(f"Directory Copied: {dir_to_ship}")

def zip_mods():
    download_dir = os.path.expanduser("~/Downloads")
    output_name = f"{download_dir}/Mod Zips/{mod_dir} Beta"

    print("zipping to Mod Zips")
    zip_dir = f"{script_dir}/{mod_dir}"
    shutil.make_archive(output_name, 'zip', zip_dir)

    print("zipping to local")
    output_name = f"{script_dir}/{mod_dir} Beta"
    shutil.make_archive(output_name, 'zip', zip_dir)


if __name__ == "__main__":  
    dir_to_ship = "Unofficial AtO Balance Patch"  
    mod_dir = "Unofficial AtO Balance Patch"

    script_dir = os.path.dirname(os.path.abspath(__file__))
    source = f"{script_dir}/{dir_to_ship}/BepInEx/config/Obeliskial_importing"
    destination = f"../../config/Obeliskial_importing"
    
    copy_directory(source, destination)

    output_name = dir_to_ship
    zip_dir = dir_to_ship
    # shutil.make_archive(output_name, 'zip', zip_dir)
    zip_mods()
