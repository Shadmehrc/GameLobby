$ports      = @(5144,5146,5145)
$baseUrl    = "http://localhost"
$path       = "/Lobby"
$lobbyId    = 7
$N          = 100
$Concurrency= 20  

$numbers = 1..$N
$chunks = [System.Collections.ArrayList]::new()
for ($i=0; $i -lt $numbers.Count; $i += $Concurrency) {
    $chunks.Add($numbers[$i..([Math]::Min($i+$Concurrency-1, $numbers.Count-1))]) | Out-Null
}

$results = @()

foreach ($batch in $chunks) {
    $jobs = @()
    foreach ($i in $batch) {
        $jobs += Start-Job -ArgumentList $ports,$baseUrl,$path,$lobbyId,$i -ScriptBlock {
            param($ports,$baseUrl,$path,$lobbyId,$i)
            $p   = Get-Random -InputObject $ports
            $url = "{0}:{1}{2}/{3}/join?playerID=u{4}" -f $baseUrl, $p, $path, $lobbyId, $i
            try {
                $resp = Invoke-RestMethod -Method Post -Uri $url -TimeoutSec 5 -ErrorAction Stop
                [PSCustomObject]@{ ok=$true; port=$p; code=200; msg=$resp.message }
            } catch {
                $code = -1
                if ($_.Exception.Response) { $code = [int]$_.Exception.Response.StatusCode }
                $body = ""
                try { 
                    $stream = $_.Exception.Response.GetResponseStream()
                    if ($stream) {
                        $reader = New-Object System.IO.StreamReader($stream)
                        $body = $reader.ReadToEnd()
                        $reader.Dispose()
                    }
                } catch {}
                [PSCustomObject]@{ ok=$false; port=$p; code=$code; msg=$body }
            }
        }
    }

    $batchResults = Receive-Job -Job $jobs -Wait
    $results += $batchResults
    Remove-Job $jobs | Out-Null
    Start-Sleep -Milliseconds 300
}

"Success=" + ($results | ? ok).Count + "  Fail=" + ($results | ? { -not $_.ok }).Count
$results | Group-Object code | Select Name,Count
$results | Where-Object { -not $_.ok } | Select-Object -First 5

try {
  $stateUrl = "{0}:{1}{2}/{3}" -f $baseUrl, $ports[0], $path, $lobbyId
  $state    = Invoke-RestMethod -Uri $stateUrl -TimeoutSec 5
  "Final MemberCount=$($state.memberCount)  Status=$($state.status)"
} catch {
  "GET state failed: $($_.Exception.Message)"
}
