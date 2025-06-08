# Ścieżka do folderu Views/Employee
$folder = "Views/Employee"

# Upewnij się, że folder istnieje
if (!(Test-Path -Path $folder)) {
    New-Item -ItemType Directory -Path $folder | Out-Null
}

# Lista widoków do utworzenia
$views = @(
    "AddMovie",
    "EditMovies",
    "DeleteMovie",
    "BrowseMovies",
    "AddHall",
    "EditHalls",
    "DeleteHall",
    "BrowseHalls",
    "AddScreening",
    "EditScreenings",
    "DeleteScreening",
    "BrowseScreenings",
    "TicketSalesReport"
)

# Szablon zawartości placeholdera
$generateContent = {
    param($viewName)
    return "@{
    ViewData[""Title""] = ""$viewName"";
}
<h2>$viewName</h2>
<p>To jest widok $viewName.cshtml</p>"
}

# Tworzenie plików .cshtml
foreach ($view in $views) {
    $path = "$folder/$view.cshtml"
    $content = & $generateContent $view

    Set-Content -Path $path -Value $content
    Write-Host " Utworzono: $path"
}