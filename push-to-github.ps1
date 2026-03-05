# Mailer repo - GitHub এ push করার স্ক্রিপ্ট
# ডাবল-ক্লিক করুন অথবা PowerShell এ চালান: .\push-to-github.ps1

Set-Location $PSScriptRoot

Write-Host "Remote: $(git remote get-url origin)" -ForegroundColor Cyan
Write-Host "Branch: $(git branch --show-current)" -ForegroundColor Cyan
Write-Host ""

# বড় রিপো/স্লো নেটের জন্য (যদি আগে সেট না থাকে)
git config --global http.postBuffer 524288000 2>$null

Write-Host "Pushing to origin main..." -ForegroundColor Yellow
$err = $null
try {
    git push -u origin main 2>&1 | Tee-Object -Variable pushOut
    if ($LASTEXITCODE -ne 0) { $err = $pushOut }
} catch {
    $err = $_.Exception.Message
}

if ($err) {
    Write-Host "`nPush FAILED. Common fixes:" -ForegroundColor Red
    Write-Host "1. Check internet connection."
    Write-Host "2. Try SSH: git remote set-url origin git@github.com:adnanahamed66772ndpc/MALER.git"
    Write-Host "3. Run in a normal PowerShell (not from IDE) as Administrator."
    Write-Host "4. Disable VPN temporarily."
    exit 1
}

Write-Host "`nPush succeeded." -ForegroundColor Green
pause
