param(
    [string] $IsoListOnePath = "data/source/upstream/six/list-one.xml",
    [string] $CldrSupplementalDataPath = "data/source/upstream/cldr/supplementalData.xml",
    [string] $OutputPath = "data/source/currency-data.snapshot.json"
)

$ErrorActionPreference = "Stop"

function Get-RequiredText {
    param(
        [object] $Value,
        [string] $FieldName
    )

    $text = if ($Value -is [System.Xml.XmlNode]) {
        [string]$Value.InnerText
    }
    else {
        [string]$Value
    }

    if ([string]::IsNullOrWhiteSpace($text)) {
        throw "Missing required field '$FieldName'."
    }

    return $text.Trim()
}

function Convert-MinorUnit {
    param(
        [string] $Value,
        [string] $Code
    )

    $normalized = $Value.Trim()

    if ($normalized -eq "N.A.") {
        return $null
    }

    $parsed = 0
    if (-not [int]::TryParse($normalized, [ref]$parsed) -or $parsed -lt 0) {
        throw "Invalid minor unit '$Value' for '$Code'."
    }

    return $parsed
}

function Convert-CashRoundingIncrement {
    param(
        [int] $CashRounding,
        [int] $CashDigits
    )

    if ($CashRounding -le 0) {
        return $null
    }

    $scale = [decimal][Math]::Pow(10, $CashDigits)
    return [decimal]$CashRounding / $scale
}

function Get-CurrencyKind {
    param(
        [string] $Code,
        [string] $Name
    )

    $preciousMetalCodes = @{
        XAG = $true
        XAU = $true
        XPD = $true
        XPT = $true
    }

    $fundCodes = @{
        BOV = $true
        CHE = $true
        CHW = $true
        CLF = $true
        COU = $true
        MXV = $true
        USN = $true
        UYI = $true
        UYW = $true
        XAD = $true
        XBA = $true
        XBB = $true
        XBC = $true
        XBD = $true
        XDR = $true
        XSU = $true
        XUA = $true
    }

    if ($Code -eq "XXX" -or $Name -eq "No universal currency") {
        return "NoCurrency"
    }

    if ($Code -eq "XTS") {
        return "Testing"
    }

    if ($preciousMetalCodes.ContainsKey($Code)) {
        return "PreciousMetal"
    }

    if ($fundCodes.ContainsKey($Code)) {
        return "Fund"
    }

    return "NationalCurrency"
}

function Add-Territory {
    param(
        [hashtable] $TerritoriesByCurrency,
        [string] $CurrencyCode,
        [string] $Territory
    )

    if (-not $TerritoriesByCurrency.ContainsKey($CurrencyCode)) {
        $TerritoriesByCurrency[$CurrencyCode] = [Collections.Generic.SortedSet[string]]::new([StringComparer]::Ordinal)
    }

    [void]$TerritoriesByCurrency[$CurrencyCode].Add($Territory)
}

function Load-XmlDocument {
    param([string] $Path)

    $settings = [System.Xml.XmlReaderSettings]::new()
    $settings.DtdProcessing = [System.Xml.DtdProcessing]::Ignore
    $reader = [System.Xml.XmlReader]::Create($Path, $settings)

    try {
        $document = [System.Xml.XmlDocument]::new()
        $document.Load($reader)
        return $document
    }
    finally {
        $reader.Dispose()
    }
}

$isoPath = (Resolve-Path $IsoListOnePath).Path
$cldrPath = (Resolve-Path $CldrSupplementalDataPath).Path

$iso = Load-XmlDocument $isoPath
$cldr = Load-XmlDocument $cldrPath

$territoriesByCurrency = @{}
$currencyData = $cldr.supplementalData.currencyData

foreach ($region in @($currencyData.region)) {
    $territory = [string]$region.iso3166

    if ($territory -notmatch '^[A-Z]{2}$') {
        continue
    }

    foreach ($currency in @($region.currency)) {
        $currencyCode = [string]$currency.iso4217

        if ($currencyCode -notmatch '^[A-Z]{3}$') {
            continue
        }

        if (-not [string]::IsNullOrWhiteSpace([string]$currency.to)) {
            continue
        }

        if ([string]$currency.tender -eq "false") {
            continue
        }

        Add-Territory $territoriesByCurrency $currencyCode $territory
    }
}

$fractionsByCurrency = @{}

foreach ($fraction in @($currencyData.fractions.info)) {
    $currencyCode = [string]$fraction.iso4217

    if ($currencyCode -match '^[A-Z]{3}$') {
        $fractionsByCurrency[$currencyCode] = $fraction
    }
}

$defaultFraction = $fractionsByCurrency["DEFAULT"]
$entriesByCode = [ordered]@{}

foreach ($entry in @($iso.ISO_4217.CcyTbl.CcyNtry)) {
    $code = [string]$entry.Ccy

    if ([string]::IsNullOrWhiteSpace($code)) {
        continue
    }

    if ($code -notmatch '^[A-Z]{3}$') {
        throw "Invalid ISO currency code '$code'."
    }

    $name = Get-RequiredText $entry.CcyNm "CcyNm"
    $numericCode = Get-RequiredText $entry.CcyNbr "CcyNbr"
    $minorUnit = Convert-MinorUnit (Get-RequiredText $entry.CcyMnrUnts "CcyMnrUnts") $code

    if ($numericCode -notmatch '^[0-9]{3}$') {
        throw "Invalid numeric code '$numericCode' for '$code'."
    }

    if (-not $entriesByCode.Contains($code)) {
        $entriesByCode[$code] = [ordered]@{
            code = $code
            numericCode = $numericCode
            englishName = $name
            decimalPlaces = $minorUnit
        }

        continue
    }

    $existing = $entriesByCode[$code]

    if ($existing.numericCode -ne $numericCode -or $existing.englishName -ne $name -or $existing.decimalPlaces -ne $minorUnit) {
        throw "Inconsistent ISO data for '$code'."
    }
}

$snapshotEntries = [Collections.Generic.List[object]]::new()

foreach ($code in ($entriesByCode.Keys | Sort-Object)) {
    $entry = $entriesByCode[$code]
    $fraction = if ($fractionsByCurrency.ContainsKey($code)) { $fractionsByCurrency[$code] } else { $defaultFraction }
    $fractionDigits = [int]$fraction.digits
    $cashDigits = if ($null -ne $fraction.cashDigits) { [int]$fraction.cashDigits } else { $fractionDigits }
    $cashRounding = if ($null -ne $fraction.cashRounding) { [int]$fraction.cashRounding } else { 0 }
    if ($territoriesByCurrency.ContainsKey($code)) {
        $territories = [string[]]@($territoriesByCurrency[$code] | ForEach-Object { [string]$_ })
    }
    else {
        $territories = [string[]]::new(0)
    }
    $kind = Get-CurrencyKind $code $entry.englishName

    $snapshotEntry = [ordered]@{
        code = $code
        numericCode = $entry.numericCode
        englishName = $entry.englishName
        decimalPlaces = $entry.decimalPlaces
        kind = $kind
        isTender = ($kind -eq "NationalCurrency" -and $territories.Count -gt 0)
        territories = [string[]]$territories
    }

    if ($null -ne $fraction.cashDigits -or ($cashRounding -gt 0 -and $null -ne $entry.decimalPlaces -and $cashDigits -ne [int]$entry.decimalPlaces)) {
        $snapshotEntry.cashDecimalPlaces = $cashDigits
    }

    $cashRoundingIncrement = Convert-CashRoundingIncrement $cashRounding $cashDigits
    if ($null -ne $cashRoundingIncrement) {
        $snapshotEntry.cashRoundingIncrement = $cashRoundingIncrement
    }

    $snapshotEntries.Add($snapshotEntry)
}

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $OutputPath) | Out-Null

$json = $snapshotEntries | ConvertTo-Json -Depth 10
$utf8NoBom = [Text.UTF8Encoding]::new($false)
[IO.File]::WriteAllText((Join-Path (Get-Location) $OutputPath), $json + [Environment]::NewLine, $utf8NoBom)

Write-Host "Generated $OutputPath from $IsoListOnePath and $CldrSupplementalDataPath ($($snapshotEntries.Count) entries)."
