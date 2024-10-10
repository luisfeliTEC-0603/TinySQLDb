param (
    [Parameter(Mandatory = $true)]
    [string]$IP,
    [Parameter(Mandatory = $true)]
    [int]$Port
)

$ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse("127.0.0.1"), 40404)

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
        Write-Host -ForegroundColor Red "!ERROR : $($responseObject.ResponseBody)"
    } else {
        Write-Host -ForegroundColor Green "SUCCESS : $($responseObject.ResponseBody)"
        Write-Host " "
        
        if ($command -like "SELECT*") {
            Write-Host -ForegroundColor Yellow "[ EXECUTING TABLE ]"
    
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

    # Verificar si el archivo existe
    if (-Not (Test-Path -Path $filePath)) {
        Write-Host "El archivo $filePath no existe." -ForegroundColor Red
        return
    }

    # Leer todo el contenido del archivo
    $fileContent = Get-Content -Path $filePath

    # Imprimir el contenido tal como está, respetando el formato del archivo
    foreach ($line in $fileContent) {
        Write-Host $line
    }
}