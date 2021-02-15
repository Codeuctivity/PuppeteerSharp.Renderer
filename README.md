# PuppeteerSharp.Renderer

Renders HTML to PDF or PNGs

[![Nuget](https://img.shields.io/nuget/v/PuppeteerSharp.Renderer.svg)](https://www.nuget.org/packages/PuppeteerSharp.Renderer/) [![Codacy Badge](https://app.codacy.com/project/badge/Grade/d550bcfac3374735bb98fbe9a63842d3)](https://www.codacy.com/gh/Codeuctivity/PuppeteerSharp.Renderer/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Codeuctivity/PuppeteerSharp.Renderer&amp;utm_campaign=Badge_Grade) [![Build status](https://ci.appveyor.com/api/projects/status/6hnwbecpssn8j379/branch/main?svg=true)](https://ci.appveyor.com/project/stesee/puppeteersharp-renderer/branch/main) [![Donate](https://img.shields.io/static/v1?label=Paypal&message=Donate&color=informational)](https://www.paypal.com/donate?hosted_button_id=7M7UFMMRTS7UE)

- Based on PuppeteerSharp
- Focused on Windows and Linux support
- Give the cli version a try - [PuppeteerSharp.RendererCli.exe](https://github.com/Codeuctivity/PuppeteerSharp.Renderer/releases)

## Howto render HTML to PDF

```c#
await using var chromiumRenderer = await Renderer.CreateAsync();
await chromiumRenderer.ConvertHtmlToPdf(sourceHtmlFilePath, desitinationPdf);
```

## Howto render HTML to PNG

```c#
await using var chromiumRenderer = await Renderer.CreateAsync();
await chromiumRenderer.ConvertHtmlToPng(actualFilePath, pathRasterizedHtml);
```