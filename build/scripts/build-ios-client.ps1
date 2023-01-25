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

$version = pwsh ./scripts/increment-version.ps1

rm -r $buildPath

/Applications/Unity/Hub/Editor/2022.1.23f1/Unity.app/Contents/MacOS/Unity -quit -accept-apiupdate -batchmode -logFile "$logFile" -projectPath "$projectPath" -executeMethod BuildScript.PerformBuild -buildPath "$buildPath"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Unity build failed"
    exit
}

$buildFolderToRemove = $projectPath + '/' + $buildPath + "_BurstDebugInformation_DoNotShip"
Remove-Item -Path "$buildFolderToRemove" -Recurse -Force

$xcodeProj = $projectPath + '/' + $buildPath + '/' + 'Unity-iPhone.xcodeproj'
$infoPlist = $projectPath + '/' + $buildPath + '/' + 'Info.plist'
# open -W "$xcodeProj"


# get development team id from environment variable if not provided
if (!$devTeamId) {
    $devTeamId = $env:NTL_TH_DEVTEAMID
}

$projectFileContent = Get-Content "$xcodeProj/project.pbxproj" -Raw

$projectFileContent = $projectFileContent -replace 'Unity-Target-New.app', 'Treasure Hunters.app'
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

$projectFileContent.Insert($targetAttributesIndex + 2, "						DevelopmentTeam = $devTeamId;")

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


Function Write-BuildConfiguration($projectFileContent, $startIndex, $searchTemplate) {
    $buildConfigurationSectionIndex = Get-Index $projectFileContent $startIndex $searchTemplate
    if ($buildConfigurationSectionIndex -eq -1) {
        Write-Error "Could not find $searchTemplate section"
        exit
    }

    $developmentTeamIndex = Get-Index $projectFileContent $buildConfigurationSectionIndex 'DEVELOPMENT_TEAM = "";'
    if ($developmentTeamIndex -eq -1) {
        Write-Error "Could not find DEVELOPMENT_TEAM item"
        exit
    }

    $projectFileContent.Insert($developmentTeamIndex + 1, "				`"DEVELOPMENT_TEAM[sdk=iphoneos*]`" = $devTeamId;")

    $infoPlistFileIndex = Get-Index $projectFileContent $developmentTeamIndex 'INFOPLIST_FILE = Info.plist;'
    if ($infoPlistFileIndex -eq -1) {
        Write-Error "Could not find INFOPLIST_FILE item"
        exit
    }

    $projectFileContent.Insert($infoPlistFileIndex + 1, "				INFOPLIST_KEY_LSApplicationCategoryType = `"public.app-category.games`";")

    $provisioningProfileSpecifierIndex = Get-Index $projectFileContent $developmentTeamIndex 'PROVISIONING_PROFILE_SPECIFIER = "$(PROVISIONING_PROFILE_SPECIFIER_APP)";'
    if ($provisioningProfileSpecifierIndex -eq -1) {
        Write-Error "Could not find PROVISIONING_PROFILE_SPECIFIER item after line $developmentTeamIndex"
        exit
    }

    $projectFileContent.Insert($provisioningProfileSpecifierIndex + 1, '				"PROVISIONING_PROFILE_SPECIFIER[sdk=iphoneos*]" = "NTL-Studio";');

    $targetedDeviceFamilyIndex = Get-Index $projectFileContent $developmentTeamIndex 'TARGETED_DEVICE_FAMILY = '
    if ($targetedDeviceFamilyIndex -eq -1) {
        Write-Error "Could not find TARGETED_DEVICE_FAMILTY item"
        exit
    }

    $projectFileContent[$targetedDeviceFamilyIndex] = '				TARGETED_DEVICE_FAMILY = 1;'
    $projectFileContent.Insert($targetedDeviceFamilyIndex + 1, "				SUPPORTS_MACCATALYST = NO;")
    $projectFileContent.Insert($targetedDeviceFamilyIndex + 2, "                SUPPORTS_MAC_DESIGNED_FOR_IPHONE_IPAD = NO;")

}

Write-BuildConfiguration $projectFileContent $provisioningStyleIndex '1D6058940D05DD3E006BFB54 /* Debug */ ='
Write-BuildConfiguration $projectFileContent $provisioningStyleIndex '1D6058950D05DD3E006BFB54 /* Release */ ='
Write-BuildConfiguration $projectFileContent $provisioningStyleIndex '56E860811D6757FF00A1AB2B /* ReleaseForRunning */ ='
Write-BuildConfiguration $projectFileContent $provisioningStyleIndex '56E860841D67581C00A1AB2B /* ReleaseForProfiling */ ='

Set-Content "$xcodeProj/project.pbxproj" $projectFileContent
# open -W "$xcodeProj"

$plist = Get-Content $infoPlist
$plist = $plist | Select-String -pattern 'UIInterfaceOrientationPortraitUpsideDown' -NotMatch
$plist = $plist | Select-String -pattern 'UIInterfaceOrientationLandscapeLeft' -NotMatch
$plist = $plist | Select-String -pattern 'UIInterfaceOrientationLandscapeRight' -NotMatch
Set-Content $infoPlist $plist

$exportOptions = Resolve-Path ./ExportOptions.plist

Set-Location "$xcodeProj/.."
xcodebuild 
xcodebuild clean -workspace Unity-iPhone.xcodeproj/project.xcworkspace -scheme Unity-iPhone -configuration Debug
$archivePath = "../archives/TreasureHunters-" + (Get-Date -Format "yyyy-MM-dd-HH-mm-ss") + ".xcarchive" 
xcodebuild archive -workspace Unity-iPhone.xcodeproj/project.xcworkspace -scheme Unity-iPhone -archivePath "$archivePath"
$appsPath = "../apps/TreasureHunters-" + (Get-Date -Format "yyyy-MM-dd-HH-mm-ss")
xcodebuild -exportArchive -archivePath "$archivePath" -exportPath "$appsPath" -exportOptionsPlist "$exportOptions"

Write-Information "Update manifest.plist"

$manifest = Get-Content "$appsPath/manifest.plist" -Raw

$manifest = $manifest -replace "treasurehunters.ipa", "Treasure%20Hunters.ipa"
$manifest = $manifest -replace "treasurehunters-", "Treasure%20Hunters-"
Set-Content "$appsPath/manifest.plist" $manifest

Set-Location ../treasurehunters-client-unity/build
pwsh ./scripts/upload-build.ps1 -archivePath "../$appsPath" -connectionString "$env:NTL_TH_STORAGEACCOUNTCONNECTIONSTRING"

$projectSettingsPath = "../treasurehunters/ProjectSettings/ProjectSettings.asset"
git add $projectSettingsPath
git commit -m"Update version to v$version"
git tag v$version
git push
git push origin v$version
