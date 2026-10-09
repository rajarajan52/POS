$replacements = @(
	@{ Old = 'namespace AspireApp2.ProductService'; New = 'namespace POSSystem.Services.Product' },
	@{ Old = 'namespace AspireApp2.InventoryService'; New = 'namespace POSSystem.Services.Inventory' },
	@{ Old = 'namespace AspireApp2.OrderService'; New = 'namespace POSSystem.Services.Order' },
	@{ Old = 'namespace AspireApp2.CustomerService'; New = 'namespace POSSystem.Services.Customer' },
	@{ Old = 'namespace AspireApp2.PaymentService'; New = 'namespace POSSystem.Services.Payment' },
	@{ Old = 'namespace AspireApp2.Web'; New = 'namespace POSSystem.UI.Web' },
	@{ Old = 'namespace AspireApp2.Core'; New = 'namespace POSSystem.Domain' },
	@{ Old = 'namespace AspireApp2.ServiceDefaults'; New = 'namespace POSSystem.Infrastructure' }
)

$files = Get-ChildItem -Path "C:\Users\Rajarajan\source\repos\AspireApp2" -Recurse -Filter "*.cs" -ErrorAction SilentlyContinue
$count = 0
foreach ($file in $files) {
	$content = Get-Content $file.FullName -Raw
	$originalContent = $content
	foreach ($replacement in $replacements) {
		$content = $content -replace [regex]::Escape($replacement.Old), $replacement.New
	}
	if ($content -ne $originalContent) {
		Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -Force
		$count++
	}
}
Write-Host "Updated $count files"
