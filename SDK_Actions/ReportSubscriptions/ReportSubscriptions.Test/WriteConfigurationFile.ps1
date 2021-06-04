$filename = "$($env:TEMP)\report.subscription\configuration"

$files = get-childitem -path "..\ReportSubscriptions\bin\Debug\*.dll"
$files | foreach { [System.Reflection.Assembly]::LoadFile($_.FullName)}
$configuration = New-Object ReportSubscriptions.Model.Configuration
$configuration.ClientId = Read-Host -Prompt "Provide the ClientId"
$configuration.ClientSecret  = Read-Host -Prompt "Provide the ClientSecret"
$configuration.ImpersonationLogin  = Read-Host -Prompt "Provide the ImpersonationLogin"

$serializer= New-Object System.Xml.Serialization.XmlSerializer( $configuration.GetType() )
New-Item -Path $filename -ItemType File -Force
$writer = new-object System.IO.StreamWriter($filename)
$serializer.Serialize($writer,$configuration)
$writer.Close()