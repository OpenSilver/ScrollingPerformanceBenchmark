# Scrolling Performance Benchmark

The same page built with three .NET UI frameworks that run in the browser, to compare how smoothly each one scrolls.

| Framework | Live demo | Source |
|---|---|---|
| OpenSilver | https://opensilver-scrolling-test.azurewebsites.net/ | [OpenSilver](OpenSilver) |
| Uno Platform | https://uno-scrolling-test.azurewebsites.net/ | [Uno](Uno) |
| Avalonia UI | https://avalonia-scrolling-test.azurewebsites.net/ | [Avalonia](Avalonia) |

## The test page

Each app shows the same long, magazine-style article inside a single vertical `ScrollViewer`. The page has photos, drop shadows, gradients, vector paths, a chart, a timeline, a photo gallery and a comment section. The content isn't virtualized, so the whole page stays in the visual tree.

The three pages contain the same sections and elements, written for each framework's XAML dialect. The differences are:

- The OpenSilver version sets `NativeScrolling.UseNativeScrollBars`, so it shows the platform's scrollbars where they overlay the content (on mobile devices and macOS).
- Drop shadows use each framework's own mechanism: `DropShadowEffect` in OpenSilver, `BoxShadow` in Avalonia and `ThemeShadow` in Uno.
- No font is specified, so each framework uses its default: a system font stack in OpenSilver (Segoe UI on Windows, other installed fonts elsewhere), Inter in Avalonia and Open Sans in Uno. Text therefore wraps slightly differently, and the page height varies a little.

## How the frameworks scroll

- **OpenSilver** renders the UI as HTML elements, and since version 3.4 a `ScrollViewer` is scrolled by the browser itself (CSS overflow). The browser typically moves the content on its compositor thread, and the .NET code is told about the new offset afterwards.
- **Uno Platform** and **Avalonia UI** draw the UI into a canvas with Skia, and the GPU renders it through WebGL. Each scrolled frame is still prepared by the framework itself, in .NET code and Skia compiled to WebAssembly. In the configuration used here (no WebAssembly multithreading), that work runs on the browser's main thread.

As a result, scrolling in OpenSilver relies mostly on the browser's own scrolling, while in the other two each frame also requires work in WebAssembly. One exception is the mouse wheel and the touchpad: OpenSilver currently handles `wheel` events with a non-passive listener on its root element, so the browser waits for the main thread, where the event is handled in .NET code, before it starts scrolling. Touch scrolling isn't affected.

**The difference is easier to see on devices with slower CPUs, such as low-end phones.** On desktop computers, all three versions can scroll smoothly.

## Other effects of native scrolling

These aren't measured by this benchmark. In OpenSilver, because the browser scrolls the content:

- The browser's find (Ctrl+F) scrolls to the results, including inside scrollable areas.
- Touch and momentum scrolling behave as on any web page on the device, including the inertia and the overscroll effect (bounce on iOS, stretch or glow on Android).
- Selecting text and dragging past the edge of a scrollable area scrolls it automatically.
- Scrollable areas are real browser scroll containers, which assistive tools such as screen readers can recognize and scroll.
- The platform's scrollbars can optionally be used on mobile devices and macOS (`NativeScrolling.UseNativeScrollBars`, enabled in this benchmark).

## How to compare

1. Open the three live demos on the same device and browser, preferably a phone.
2. Scroll the page with slow drags and with fast flicks, and watch for dropped frames, delayed starts and blank areas. The first scroll after loading can stutter in all three, while code is compiled and images are decoded, so scroll up and down for a few seconds before judging.
3. For measurements, record a trace while scrolling on the device itself, for example with Chrome's remote debugging for Android (`chrome://inspect`) and the DevTools *Performance* panel, and compare frame rates and main-thread activity.

CPU throttling in desktop developer tools isn't a good substitute for a phone: it slows down only the page's main thread, not the GPU or the browser's other threads, so it doesn't reproduce the behavior of a low-end device.

## Versions

| Framework | Version |
|---|---|
| OpenSilver | 3.4.0 preview (`OpenSilver` 3.4.0-preview-2026-11-17) |
| Uno Platform | Uno.Sdk 7.1.0-dev.23 (Uno.WinUI 7.1.0-dev.286), Skia renderer |
| Avalonia UI | 12.2 nightly build (12.2.999-cibuild0101409-alpha) |

All three apps target .NET 11 and use each framework's default Release settings. AOT compilation isn't enabled, so the .NET code runs on the WebAssembly interpreter. AOT can make the frameworks that draw every frame in .NET noticeably faster, at the cost of a larger download and much longer builds. The OpenSilver project also keeps its template's `WasmEnableSIMD=false` setting. The build settings are in each project file.

## Running locally

Each folder contains a solution. The browser versions can be started with:

```
dotnet run -c Release --project OpenSilver/TestScrollingPerformance.Browser
dotnet run -c Release --project Uno/TestScrollingPerformance -f net11.0-browserwasm
dotnet run -c Release --project Avalonia/TestScrollingPerformance.Browser
```

Use the Release configuration for any comparison: Debug builds are much slower.

The OpenSilver previews and the Avalonia nightly packages come from the feeds configured in `OpenSilver/NuGet.Config` and `Avalonia/NuGet.Config`. The Uno development builds are on nuget.org, and the Uno.Sdk version is set in `Uno/global.json`.

## Limitations

- This benchmark only measures scrolling of a single, non-virtualized page. It says nothing about startup time, download size or other features.
- Results depend on the device, the browser and its version.
- Each framework may offer settings or techniques that improve its result. Suggestions and pull requests that make any of the three versions faster are welcome.

## License

The code is licensed under the [MIT](LICENSE) license.

The photos aren't covered by the MIT license. They come from [Unsplash](https://unsplash.com) (through [Lorem Picsum](https://picsum.photos)) and are used under the [Unsplash License](https://unsplash.com/license). Photos by Alexey Topolyanskiy, Andrew Coelho, Kevin Young, Linh Nguyen, Christian Joudrey, Roberto Nickson, Luke Chesser, Alexander Shustov, Andrew Ridley, Wolfgang Lutz, Samuel Zeller, Galymzhan Abdugalimov, petradr and Charlie Foster.
