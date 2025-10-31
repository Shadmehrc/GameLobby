param(
  [string]$Namespace = 'game',
  [string]$Label = 'app=gamelobby',
  [int]$StartPort = 5144,
  [int]$ContainerPort = 8080
)

$ErrorActionPreference = 'Stop'

$pods = kubectl -n $Namespace get pods -l $Label --no-headers | ForEach-Object {
  $cols = ($_ -split '\s+') 
  if ($cols[1] -eq '1/1' -and $cols[2] -eq 'Running') { $cols[0] }
}

if (-not $pods) {
  Write-Host "[x] No Running/Ready pods found for label '$Label'." -ForegroundColor Red
  Write-Host "    Try: kubectl -n $Namespace get pods -l $Label --show-labels"
  exit 1
}

$i = 0
foreach ($p in $pods) {
  $local = $StartPort + $i
  Write-Host "[*] Forwarding pod '$p'  =>  http://localhost:$local  (->$ContainerPort)" -ForegroundColor Cyan
  Start-Process -FilePath "cmd.exe" -ArgumentList "/k kubectl -n $Namespace port-forward pod/$p $local`:$ContainerPort"
  $i++
}
