import sys

def switch(command):

    if command == "Load":
        return "Success"
    
    elif command == "Evaluate":
        return "1"
    
    else:
        return "Fail"
    
for input in sys.stdin:
    command = input.strip()
    
    responce = switch(command)

    print(responce)
    sys.stdout.flush()