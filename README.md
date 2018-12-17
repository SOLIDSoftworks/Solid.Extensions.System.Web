# Solid.Extensions.System.Web [![License](https://img.shields.io/github/license/mashape/apistatus.svg)](https://en.wikipedia.org/wiki/MIT_License) [![solidsoftworks MyGet Build Status](https://www.myget.org/BuildSource/Badge/solidsoftworks?identifier=39fc5c41-0af2-4d99-af28-48a6a67b55d7)](https://www.myget.org/)

## Disclaimer
We do not recommended using this library in any new applications. You should always try to have your stack be fully async instead of using Task.Run or any other variation of that within sync methods.

If you are, however, up against the wall and have a sync stack already and need to use async methods, you can try this library out and see if it works for you.

## Requirements
This is written for ASP.Net applications. This is not for AspNetCore applications.

## How to use
### Installation
Just reference the library. When the ASP.Net application starts, it will patch HttpContext.Current for you.

### Usage
```csharp
public string GetString()
{
    return HttpContext.Current.Run(() => GetStringAsync());
}

private async Task<string> GetStringAsync()
{
    return "Hello world";
}
```