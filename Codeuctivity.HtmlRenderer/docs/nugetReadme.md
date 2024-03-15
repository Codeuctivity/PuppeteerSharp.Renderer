# Render HTML to PDF

```c#
await using var chromiumRenderer = await Renderer.CreateAsync();
await chromiumRenderer.ConvertHtmlToPdf(sourceHtmlFilePath, desitinationPdf);
```

## Render HTML to PNG

```c#
await using var chromiumRenderer = await Renderer.CreateAsync();
await chromiumRenderer.ConvertHtmlToPng(actualFilePath, pathRasterizedHtml);
```
