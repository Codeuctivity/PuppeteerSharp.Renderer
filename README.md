# PuppeteerSharp.Renderer

Renders HTML to PDF or PNGs

[![Nuget](https://img.shields.io/nuget/v/PuppeteerSharp.Renderer.svg)](https://www.nuget.org/packages/PuppeteerSharp.Renderer/) [![Codacy Badge](https://app.codacy.com/project/badge/Grade/7ba69957e12f4348a25e14e7db124cd6)](https://www.codacy.com/gh/Codeuctivity/PuppeteerSharp.Renderer/dashboard?utm_source=github.com&utm_medium=referral&utm_content=Codeuctivity/PuppeteerSharp.Renderer&utm_campaign=Badge_Grade)
[![Build status](https://ci.appveyor.com/api/projects/status/6hnwbecpssn8j379/branch/main?svg=true)](https://ci.appveyor.com/project/stesee/puppeteersharp-renderer/branch/main) [![Donate](https://img.shields.io/static/v1?label=Paypal&message=Donate&color=informational)](https://www.paypal.com/donate?hosted_button_id=7M7UFMMRTS7UE)

- Based on PuppeteerSharp
- Focused on Windows and Linux support

```c#
await using var chromiumRenderer = await Renderer.CreateAsync();
await chromiumRenderer.ConvertHtmlToPdf(sourceHtmlFilePath, desitinationPdf);
```
