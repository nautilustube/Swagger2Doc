
try {
    # cd 到
    $rootPath = $PWD;
    
    # 編譯 net core web
    $project = "$rootPath\Swagger2Doc.csproj"
    $output = "$rootPath\publish"

    if (-not (Test-Path -Path $output)) {
        New-Item -ItemType Directory -Path $output
        Write-Host "Folder created successfully."
    } else {
        Write-Host "Folder already exists."
    }

    # clear previous releases
    Remove-Item "$output\*" -Recurse -Force
    # dotnet publish $project -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true -o $output
    dotnet publish $project -p:PublishSingleFile=true -c Release -r win-x64 --no-self-contained -o $output
    # dotnet publish $project -c Release -o $output
    # dotnet publish $project -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true -o $output
    # dotnet publish $project -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true -o $output
    # dotnet build $project -c Release -r linux-x64 -o $output
    Read-Host -Prompt "Publish Success!"
}
catch {
    Write-Output $Error[0]
}
Read-Host -Prompt "Press Enter to exit"


