import sys

for input in sys.stdin:
    input = input.strip()
    
    if input != "":
        print(f"{input} recived")
        sys.stdout.flush()
        break

