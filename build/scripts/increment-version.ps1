git fetch --tags
$searchTags = git tag --list 1.0.0-alpha.* --sort=-v:refname
if ($searchTags -eq $null) {
    $version = "1.0.0-alpha.1"
} else {
    $version = $searchTags -split '\r?\n' | Select-Object -First 1
    $version = $version.Replace("1.0.0-alpha.", "")
    $version = ([int]$version) + 1
    $version = "1.0.0-alpha." + $version
}

$projectSettingsPath = "../treasurehunters/ProjectSettings/ProjectSettings.asset"

$projectSettings = get-content $projectSettingsPath -Raw
$projectSettings = $projectSettings -replace "bundleVersion: .*", "bundleVersion: $version" | Set-Content ../treasurehunters/ProjectSettings/ProjectSettings.asset

git add $projectSettingsPath
git commit -m"Update version to $version"
git tag $version
git push
git push origin $version