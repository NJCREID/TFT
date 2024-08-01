import os

# Directory path
directory = "../../../TFT API/TFT_API/assets/images/traits/"

# Function to rename files
def prepend_to_filenames(directory_path, prefix):
    # Ensure directory path is absolute and normalized
    directory_path = os.path.abspath(os.path.normpath(directory_path))
    
    # List all files in the directory
    try:
        files = os.listdir(directory_path)
    except FileNotFoundError:
        print(f"Directory not found: {directory_path}")
        return
    
    # Iterate through files and prepend prefix
    for filename in files:
        # Construct full path to the file
        filepath = os.path.join(directory_path, filename)
        
        # Skip directories if any
        if os.path.isdir(filepath):
            continue
        
        # Construct new filename with prefix
        new_filename = f"{prefix}{filename}"
        new_filepath = os.path.join(directory_path, new_filename)
        
        try:
            # Rename the file
            os.rename(filepath, new_filepath)
            print(f"Renamed: {filename} -> {new_filename}")
        except Exception as e:
            print(f"Error renaming {filename}: {e}")

# Prepend "TFT11_" to all filenames in the directory
prepend_to_filenames(directory, "TFT11_")
