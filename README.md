# Codeuctivity.HtmlRenderer

[![Nuget](https://img.shields.io/nuget/v/Codeuctivity.HtmlRenderer.svg)](https://www.nuget.org/packages/Codeuctivity.HtmlRenderer/) [![Build](https://github.com/Codeuctivity/PuppeteerSharp.Renderer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Codeuctivity/PuppeteerSharp.Renderer/actions/workflows/dotnet.yml) [![Codacy Badge](https://app.codacy.com/project/badge/Grade/d550bcfac3374735bb98fbe9a63842d3)](https://www.codacy.com/gh/Codeuctivity/PuppeteerSharp.Renderer/dashboard?utm_source=github.com&utm_medium=referral&utm_content=Codeuctivity/PuppeteerSharp.Renderer&utm_campaign=Badge_Grade) [![Donate](https://img.shields.io/static/v1?label=Paypal&message=Donate&color=informational)](https://www.paypal.com/donate?hosted_button_id=7M7UFMMRTS7UE)

Renders HTML to PDF or PNGs

- Based on PuppeteerSharp
- Focused on Windows and Linux support
- Give the cli version a try - [Codeuctivity.HtmlRendererCli.exe](https://github.com/Codeuctivity/PuppeteerSharp.Renderer/releases)

## Render HTML to PDF

```c#
await using var chromiumRenderer = await Renderer.CreateAsync();
await chromiumRenderer.ConvertHtmlToPdf(sourceHtmlFilePath, desitinationPdf);
```

## Render HTML to PNG

```c#
await using var chromiumRenderer = await Renderer.CreateAsync();
await chromiumRenderer.ConvertHtmlToPng(actualFilePath, pathRasterizedHtml);
```

## Development

### Linux

```bash
sudo apt install libgbm-dev
```
