function Execute-MyQuery {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,  
        [Parameter(Mandatory = $true)]
        [int]$Port,  
        [Parameter(Mandatory = $true)]
        [string]$IP  
    )

    validateFile -QueryFile $QueryFile

    try {
        $ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)
    } catch {
        Write-Host -ForegroundColor Red "!Error : Could make the connection"
        exit 1
    }

    Process-QueryFile -QueryFile $QueryFile -Port $Port -IP $IP
}

function validateFile {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile 
    )

    if ([System.IO.Path]::GetExtension($QueryFile) -ne ".tinysql") {
        Write-Host -ForegroundColor Red "!Error : The file does not count with the required extension (.tinysql)"
        exit 1
    }

    if (-not (Test-Path $QueryFile)) {
        Write-Host -ForegroundColor Red "!Error: The file $QueryFile does not exist"
        exit 1
    }
}

function Process-QueryFile {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,
        [int]$Port,  
        [string]$IP  
    )

    Write-Host -ForegroundColor Magenta "[ LOADING $QueryFile ... ]"
    $lines = Get-Content $QueryFile
    foreach ($line in $lines) {
        if (-not [string]::IsNullOrWhiteSpace($line)) {
            Send-SQLCommand -command $line
        }
    }
}

function Send-Message {
    param (
        [Parameter(Mandatory=$true)]
        [pscustomobject]$message,
        [Parameter(Mandatory=$true)]
        [System.Net.Sockets.Socket]$client
    )

    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $writer = New-Object System.IO.StreamWriter($stream)
    try {
        $writer.WriteLine($message)
    }
    finally {
        $writer.Close()
        $stream.Close()
    }
}

function Receive-Message {
    param (
        [System.Net.Sockets.Socket]$client
    )
    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $reader = New-Object System.IO.StreamReader($stream)
    try {
        $line = $reader.ReadLine()
        if ($null -ne $line) {
            return $line
        } else {
            return ""
        }
    }
    finally {
        $reader.Close()
        $stream.Close()
    }
}

function Send-SQLCommand {
    param (
        [string]$command
    )
    $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
    $client.Connect($ipEndPoint)

    $startTime = Get-Date
    
    $requestObject = [PSCustomObject]@{
        RequestType  = 0;
        RequestBody  = $command
    }
    Write-Host " "
    Write-Host "< $command >"

    $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
    Send-Message -client $client -message $jsonMessage
    
    $response = Receive-Message -client $client
    
    $responseObject = ConvertFrom-Json -InputObject $response

    $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
    $client.Close()

    $endTime = Get-Date
    $duration = $endTime - $startTime
    Write-Host -ForegroundColor Yellow "EXECUTION : $($duration.TotalSeconds)s"

    if ($responseObject.Status -ne 0) {
        Write-Host -ForegroundColor Red "!ERROR : Something went wrong :("
    } else {
        Write-Host -ForegroundColor Green "SUCCESS : $($responseObject.ResponseBody)"
        
        if ($command -like "SELECT*") {
            Write-Host -ForegroundColor Blue "[ EXECUTING TABLE ]"
            Write-Host " "
    
            $filePath = "C:\Users\ejcan\Desktop\U\FSC\Proyecto 2\TinySQLDb\SavedTables\DataToPrint.txt"
    
            Print-SQLTable -filePath $filePath
        }
    }
}

function Print-SQLTable {
    param (
        [Parameter(Mandatory = $true)]
        [string]$filePath
    )

    if (-Not (Test-Path -Path $filePath)) {
        Write-Host "!Error : $filePath is not found" -ForegroundColor Red
        return
    }

    $fileContent = Get-Content -Path $filePath

    foreach ($line in $fileContent) {
        Write-Host $line
    }
}