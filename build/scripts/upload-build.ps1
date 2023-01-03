[CmdletBinding()]
param(
    [Parameter()]
    [string]$ConnectionString, 

    [Parameter(Mandatory = $true)]
    [string]$archivePath
)

az storage blob delete-batch -s '$web' --pattern 'apps/*.ipa' --connection-string "$ConnectionString"

az storage blob upload -f "$archivePath/manifest.plist" -c '$web' -n 'treasurehunters/manifest.plist' --overwrite true --connection-string "$ConnectionString"

az storage blob upload-batch -d '$web/apps' -s "$archivePath/Apps" --pattern *.ipa --connection-string "$ConnectionString" 