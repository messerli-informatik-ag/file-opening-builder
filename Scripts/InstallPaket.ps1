$dotnet = 'dotnet.exe'
$params = 'tool', 'install', '--global', 'Paket'

try {
    & $dotnet $params
}
catch {
}
finally {
    exit 0
}
