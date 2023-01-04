[CmdletBinding()]
param(
    [Parameter()]
    [string]$logFile = '../../logs/build.logs',
    [Parameter()]
    [string]$projectPath = '../treasurehunters',
    # the build path is relative to the project path
    [Parameter()]
    [string]$buildPath = '../../ios',
    [Parameter()]
    [string]$devTeamId
)

pwsh ./scripts/increment-version.ps1

rm -r $buildPath

/Applications/Unity/Hub/Editor/2021.3.4f1/Unity.app/Contents/MacOS/Unity -quit -accept-apiupdate -batchmode -logFile "$logFile" -projectPath "$projectPath" -executeMethod BuildScript.PerformBuild -buildPath "$buildPath"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Unity build failed"
    exit
}

$buildFolderToRemove = $projectPath + '/' + $buildPath + "_BurstDebugInformation_DoNotShip"
Remove-Item -Path "$buildFolderToRemove" -Recurse -Force

$xcodeProj = $projectPath + '/' + $buildPath + '/' + 'Unity-iPhone.xcodeproj'
# open -W "$xcodeProj"


# get development team id from environment variable if not provided
if (!$devTeamId) {
    $devTeamId = $env:NTL_TH_DEVTEAMID
}

$projectFileContent = Get-Content "$xcodeProj/project.pbxproj" -Raw

$projectFileContent = $projectFileContent -replace 'Unity-Target-New.app', 'TreasureHunters.app'
$projectFileContent | Set-Content "$xcodeProj/project.pbxproj"

[System.Collections.Generic.List[string]]$projectFileContent = get-content "$xcodeProj/project.pbxproj"

Function Get-Index($projectFileContent, $startIndex, $searchString) {
    for ($i = $startIndex; $i -lt $projectFileContent.Count; $i++) {
        if ($projectFileContent[$i].Contains($searchString, [System.StringComparison]::OrdinalIgnoreCase)) {
            return $i
        }
    }
    return -1
}

$pbxSectionIndex = Get-Index $projectFileContent 0 'Begin PBXBuildFile section'
if ($pbxSectionIndex -eq -1) {
    Write-Error "Could not find PBXBuildFile section"
    exit
}

$targetAttributesIndex = Get-Index $projectFileContent $pbxSectionIndex 'TargetAttributes = {'
if ($targetAttributesIndex -eq -1) {
    Write-Error "Could not find TargetAttributes section"
    exit
}

$projectFileContent.Insert($targetAttributesIndex + 2, "						DevelopmentTeam = $devTeamId;");

$createdToolsVersionIndex = Get-Index $projectFileContent $targetAttributesIndex 'CreatedOnToolsVersion = 9.2;'
if ($createdToolsVersionIndex -eq -1) {
    Write-Error "Could not find CreatedOnToolsVersion item"
    exit
}

$provisioningStyleIndex = Get-Index $projectFileContent $createdToolsVersionIndex 'ProvisioningStyle = '
if ($provisioningStyleIndex -eq -1) {
    Write-Error "Could not find ProvisioningStyle section"
    exit
}

$projectFileContent[$provisioningStyleIndex] = '				ProvisioningStyle = Automatic;'

$xcBuildConfigurationIndex = Get-Index $projectFileContent $provisioningStyleIndex 'Begin XCBuildConfiguration section'
if ($xcBuildConfigurationIndex -eq -1) {
    Write-Error "Could not find XCBuildConfiguration section"
    exit
}

$developmentTeamIndex = Get-Index $projectFileContent $xcBuildConfigurationIndex 'DEVELOPMENT_TEAM = "";'
if ($developmentTeamIndex -eq -1) {
    Write-Error "Could not find DEVELOPMENT_TEAM item"
    exit
}

$projectFileContent.Insert($developmentTeamIndex + 1, "				`"DEVELOPMENT_TEAM[sdk=iphoneos*]`" = $devTeamId;");

$provisioningProfileSpecifierIndex = Get-Index $projectFileContent $developmentTeamIndex 'PROVISIONING_PROFILE_SPECIFIER = "$(PROVISIONING_PROFILE_SPECIFIER_APP)";'
if ($provisioningProfileSpecifierIndex -eq -1) {
    Write-Error "Could not find PROVISIONING_PROFILE_SPECIFIER item after line $developmentTeamIndex"
    exit
}

$projectFileContent.Insert($provisioningProfileSpecifierIndex + 1, '				"PROVISIONING_PROFILE_SPECIFIER[sdk=iphoneos*]" = "Treasure Hunters AdHoc";');

$developmentTeamIndex = Get-Index $projectFileContent $provisioningProfileSpecifierIndex 'DEVELOPMENT_TEAM = "";'
if ($developmentTeamIndex -eq -1) {
    Write-Error "Could not find DEVELOPMENT_TEAM item"
    exit
}

$projectFileContent.Insert($developmentTeamIndex + 1, "				`"DEVELOPMENT_TEAM[sdk=iphoneos*]`" = $devTeamId;");

$provisioningProfileSpecifierIndex = Get-Index $projectFileContent $developmentTeamIndex 'PROVISIONING_PROFILE_SPECIFIER = "$(PROVISIONING_PROFILE_SPECIFIER_APP)";'
if ($provisioningProfileSpecifierIndex -eq -1) {
    Write-Error "Could not find PROVISIONING_PROFILE_SPECIFIER item after line $developmentTeamIndex"
    exit
}

$projectFileContent.Insert($provisioningProfileSpecifierIndex + 1, '				"PROVISIONING_PROFILE_SPECIFIER[sdk=iphoneos*]" = "Treasure Hunters AdHoc";');

$releaseForRunningIndex = Get-Index $projectFileContent $provisioningProfileSpecifierIndex '/* ReleaseForRunning */'
if ($releaseForRunningIndex -eq -1) {
    Write-Error "Could not find ReleaseForRunning item after line $provisioningProfileSpecifierIndex"
    exit
}

$developmentTeamIndex = Get-Index $projectFileContent $releaseForRunningIndex 'DEVELOPMENT_TEAM = "";'
if ($developmentTeamIndex -eq -1) {
    Write-Error "Could not find DEVELOPMENT_TEAM item"
    exit
}

$projectFileContent.Insert($developmentTeamIndex + 1, "				`"DEVELOPMENT_TEAM[sdk=iphoneos*]`" = $devTeamId;");

$provisioningProfileSpecifierIndex = Get-Index $projectFileContent $developmentTeamIndex 'PROVISIONING_PROFILE_SPECIFIER = "$(PROVISIONING_PROFILE_SPECIFIER_APP)";'
if ($provisioningProfileSpecifierIndex -eq -1) {
    Write-Error "Could not find PROVISIONING_PROFILE_SPECIFIER item after line $developmentTeamIndex"
    exit
}

$projectFileContent.Insert($provisioningProfileSpecifierIndex + 1, '				"PROVISIONING_PROFILE_SPECIFIER[sdk=iphoneos*]" = "Treasure Hunters AdHoc";');

$releaseForProfilingIndex = Get-Index $projectFileContent $provisioningProfileSpecifierIndex '/* ReleaseForProfiling */'
if ($releaseForProfilingIndex -eq -1) {
    Write-Error "Could not find ReleaseForProfiling item after line $provisioningProfileSpecifierIndex"
    exit
}

$developmentTeamIndex = Get-Index $projectFileContent $releaseForProfilingIndex 'DEVELOPMENT_TEAM = "";'
if ($developmentTeamIndex -eq -1) {
    Write-Error "Could not find DEVELOPMENT_TEAM item"
    exit
}

$projectFileContent.Insert($developmentTeamIndex + 1, "				`"DEVELOPMENT_TEAM[sdk=iphoneos*]`" = $devTeamId;");

$provisioningProfileSpecifierIndex = Get-Index $projectFileContent $developmentTeamIndex 'PROVISIONING_PROFILE_SPECIFIER = "$(PROVISIONING_PROFILE_SPECIFIER_APP)";'
if ($provisioningProfileSpecifierIndex -eq -1) {
    Write-Error "Could not find PROVISIONING_PROFILE_SPECIFIER item after line $developmentTeamIndex"
    exit
}

$projectFileContent.Insert($provisioningProfileSpecifierIndex + 1, '				"PROVISIONING_PROFILE_SPECIFIER[sdk=iphoneos*]" = "Treasure Hunters AdHoc";');

Set-Content "$xcodeProj/project.pbxproj" $projectFileContent

# open -W "$xcodeProj"

Set-Location "$xcodeProj/.."
xcodebuild 
xcodebuild clean -workspace Unity-iPhone.xcodeproj/project.xcworkspace -scheme Unity-iPhone -configuration Debug
$archivePath = "../archives/TreasureHunters-" + (Get-Date -Format "yyyy-MM-dd-HH-mm-ss") + ".xcarchive" 
xcodebuild archive -workspace Unity-iPhone.xcodeproj/project.xcworkspace -scheme Unity-iPhone -archivePath "$archivePath"
$appsPath = "../apps/TreasureHunters-" + (Get-Date -Format "yyyy-MM-dd-HH-mm-ss")
xcodebuild -exportArchive -archivePath "$archivePath" -exportPath "$appsPath" -exportOptionsPlist "../archives/exportOptions.plist"

Write-Information "Update manifest.plist"

$manifest = Get-Content "$appsPath/manifest.plist" -Raw

$manifest = $manifest -replace "treasurehunters.ipa", "TreasureHunters.ipa"
$manifest = $manifest -replace "treasurehunters-", "TreasureHunters-"
Set-Content "$appsPath/manifest.plist" $manifest

Set-Location ../treasurehunters-client-unity/build
pwsh ./scripts/upload-build.ps1 -archivePath "../$appsPath" -connectionString "$env:NTL_TH_STORAGEACCOUNTCONNECTIONSTRING"
