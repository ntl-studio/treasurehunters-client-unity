Here's how to build and upload an iOS app
```powershell
pwsh ./scripts/build-ios-client.ps1 
```

The script uses several environment variables to configure the build process:

- `NTL_TH_DEVTEAMID` - the apple development program team id
- `NTL_TH_STORAGEACCOUNTCONNECTIONSTRING` - the connection string to the azure storage account

---
`pre-build.sh` is a script that is run before the build process starts. It is used to configure the build environment and preapre the build process to download custom packages. In fact this script is necessary only for a cloud build. For a local build, the script is not necessary.