#pip install pycryptodome
import sys, base64
from Crypto.Cipher import ARC4

def RC4(data, key):
    cipher = ARC4.new(key.encode('utf-8'))
    return cipher.encrypt(data)

def B64Enc(text):
    return base64.b64encode(text).decode('utf-8')

def B64Dec(text):
    return base64.b64decode(text)

if __name__ == '__main__':
    if len(sys.argv) != 3:
        sys.exit(f'{sys.argv[0]} <b64_sc> <key>')
    
    print('\n' + B64Enc(RC4(B64Dec(sys.argv[1]), sys.argv[2])))