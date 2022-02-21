function Check-PortConnector {
    Param([string]$Machine = "localhost")
    
    return !(Get-WmiObject Win32_PortConnector -ComputerName $Machine -ErrorAction Stop)
}

function Check-VMProcesses {
    Param([string]$Machine = "localhost")

    return (Get-WmiObject Win32_Process -ComputerName $Machine -ErrorAction Stop | Select ProcessName | Select-String "vboxtray|vboxservice|vmtoolsd|vm3dservice" -Quiet) -ne $null
}

function Check-CPUInfo {
    Param([string]$Machine = "localhost")

    $info = Get-WmiObject Win32_Processor -ComputerName $Machine -ErrorAction Stop | Select-Object NumberOfLogicalProcessors, Name
    return (($info | Select-Object -ExpandProperty NumberOfLogicalProcessors) -ge 32) + (($info | Select-Object -ExpandProperty Name | Select-String "epyc|xeon" -Quiet) -ne $null)
}

function Check-BiosSerial {
    Param([string]$Machine = "localhost")

    return (Get-WmiObject Win32_Bios -ComputerName $Machine -ErrorAction Stop | Select-Object -ExpandProperty SerialNumber) -eq 0
}

function Check-SystemMakerInfo {
    Param([string]$Machine = "localhost")

    $info = Get-WmiObject Win32_ComputerSystem -ComputerName $Machine -ErrorAction Stop | Select-Object Model, Manufacturer
    return (($info | Select-Object -ExpandProperty Model | Select-String "virtualbox|vmware" -Quiet) -ne $null) + (($info | Select-Object -ExpandProperty Manufacturer | Select-String "innotek|vmware" -Quiet) -ne $null)
}

function Check-DriveSize {
    Param([string]$Machine = "localhost")

    return (Get-WmiObject Win32_LogicalDisk -Filter "DeviceID='C:'" -ComputerName $Machine -ErrorAction Stop | Select-Object -ExpandProperty Size) -le 121111111111
}

function Invoke-Marlowe {
    Param([string]$Machine = "localhost")
    
    $count = 0
    $count += Check-PortConnector   -Machine $Machine
    $count += Check-VMProcesses     -Machine $Machine
    $count += Check-CPUInfo         -Machine $Machine
    $count += Check-BiosSerial      -Machine $Machine
    $count += Check-SystemMakerInfo -Machine $Machine
    $count += Check-DriveSize       -Machine $Machine

    If($count -ge 3) {
        Write-Host "VM detected! ($count/8)"
    }
    else {
        Write-Host "Not a VM! ($count/8)"
    }
}