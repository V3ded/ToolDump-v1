import sys
import base64

def XOR(text, key):
    output = ""
    for i, character in enumerate(text):
        output += chr(ord(character) ^ ord(key[i % len(key)]))
    return output

def B64Enc(text):
    return base64.b64encode(text.encode('ascii')).decode('ascii')

def Debug(text, key):
    print(XOR(base64.b64decode(text.encode('ascii')).decode('ascii'), key))

if __name__ == '__main__':
    if len(sys.argv) != 5:
        sys.exit(f'{sys.argv[0]} <key> <release> <x64_pid/processName> <b64_x64sc>')
    
    print('\n' + B64Enc(XOR(f'{sys.argv[2]};{sys.argv[3]};{sys.argv[4]}\n', sys.argv[1])) + '\n')