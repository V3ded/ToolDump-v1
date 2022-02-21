# Disclaimer
This repository and the code in it is provided as is. I won't respond to any pull requests or issues. Be responsible and don't use the provided code / tools in a malicious way. I do **not** condone or endorse malicious behavior.

# Introduction
This repository contains a dump of few tools I wrote over the past 2-3 years. The main reason behind this dump is that most of the methods used in these tools are now considered outdated or obsolete (to an extent). While said tools might not be as useful in real life engagements, I hope someone can at least use them as a learning resource instead.

# Tools

### [Ballista](Ballista/)
**Ballista** is a C# tool responsible for x64 cross-process injection. The tool relies on `NtCreateSection()` and `NtMapViewOfSection()` syscalls in order to evade user level API hooking.

###### Keywords
- C#
- Syscalls
- Process Injection
- Sections

***

### [Marlowe](Marlowe/)
**Marlowe** is a PowerShell script which uses WMI to query local and remote systems with the aim of revealing whether said systems are virtual machines. 

###### Keywords
- PowerShell
- WMI

***

### [RainCaller](RainCaller/)
**RainCaller** is a shellcode dropper suite consisting of a *Python3* webserver (server) and a *C#* dropper (client).

###### Keywords
- C#
- DInvoke
- Python
- Dropper

***

### [SharpSectionJect](SharpSectionJect/)
**SharpSectionJect** is a collection of C# process injectors in various formats. More specifically *EXE*, *DLL*, *MsBuild* and *InstallUtil* formats.

###### Keywords
- C#
- PInvoke
- Sections
- AppLocker bypass

***

### [UAC_Bypasses](UAC_Bypasses/)
**UAC_Bypasses** is a collection of PowerShell UAC bypasses. These bypasses utilize the *CurVer* trick to bypass registry monitoring. More about this technique can be found on my blog [here](https://v3ded.github.io/redteam/utilizing-programmatic-identifiers-progids-for-uac-bypasses
).

###### Keywords
- PowerShell
- CurVer
- UAC Bypass