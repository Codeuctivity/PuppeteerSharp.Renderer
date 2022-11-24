# Codeuctivity.HtmlRenderer

[![Nuget](https://img.shields.io/nuget/v/Codeuctivity.HtmlRenderer.svg)](https://www.nuget.org/packages/Codeuctivity.HtmlRenderer/) [![Build](https://github.com/Codeuctivity/PuppeteerSharp.Renderer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Codeuctivity/PuppeteerSharp.Renderer/actions/workflows/dotnet.yml) [![Donate](https://img.shields.io/static/v1?label=Paypal&message=Donate&color=informational)](https://www.paypal.com/donate?hosted_button_id=7M7UFMMRTS7UE)

Renders HTML to PDF or PNGs

- Based on PuppeteerSharp
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
sudo apt install libgbm-dev libatk-bridge2.0-0
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.1/install.sh | bash
exit
nvm install 18
```

... and if that fails you can either find out which dependency is missing on your system or you take a shortcut

```bash
sudo apt install -y chromium-browser
```
