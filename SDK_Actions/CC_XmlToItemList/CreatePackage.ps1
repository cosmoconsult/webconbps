param($targetDir,$projectDir,$projectName,$copyDestinationFolder)
Write-Output "targetDir:'$targetDir'"
Write-Output "projectDir:'$projectDir'"
Write-Output "projectName:'$projectName'"
Write-Output "copyDestinationFolder:'$copyDestinationFolder'"
New-Item $projectDir\Publish -ItemType Directory -Force -ErrorAction SilentlyContinue
$files = Get-ChildItem -Path "$targetDir\*.dll", "$TargetDir\*.json", "$TargetDir\*.pdb"  -Exclude "WebCon.*"
Compress-Archive -Path $files -DestinationPath "$projectDir\Publish\$projectName.zip" -Force;
if ($copyDestinationFolder -ne $null){
    Copy-Item "$projectDir\Publish\$projectName.zip" $copyDestinationFolder -Force -Verbose;
}

