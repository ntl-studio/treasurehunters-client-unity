git fetch --tags
$searchTags = git tag --list v0.0.* --sort=-v:refname
if ($searchTags -eq $null) {
    $version = "0.0.1"
} else {
    $version = $searchTags -split '\r?\n' | Select-Object -First 1
    $version = $version.Replace("v0.0.", "")
    $version = ([int]$version) + 1
    $version = "0.0." + $version
}

$projectSettingsPath = "../treasurehunters/ProjectSettings/ProjectSettings.asset"

$projectSettings = get-content $projectSettingsPath -Raw
$projectSettings = $projectSettings -replace "bundleVersion: .*", "bundleVersion: $version" | Set-Content ../treasurehunters/ProjectSettings/ProjectSettings.asset

git add $projectSettingsPath
git commit -m"Update version to v$version"
git tag v$version
git push
git push origin v$version