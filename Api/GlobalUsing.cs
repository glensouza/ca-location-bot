global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Threading.Tasks;

global using Azure;
global using Azure.Search.Documents;
global using Azure.Search.Documents.Indexes;
global using Azure.Search.Documents.Indexes.Models;
global using Azure.Search.Documents.Models;

global using ICSharpCode.SharpZipLib.GZip;
global using ICSharpCode.SharpZipLib.Tar;

global using MaxMind.GeoIP2;
global using MaxMind.GeoIP2.Responses;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Azure.WebJobs;
global using Microsoft.Azure.WebJobs.Extensions.Http;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging;

global using Newtonsoft.Json;

global using CALocationBot.Api.CityCounty;
global using CALocationBot.Api.GeoLite2_City;
global using CALocationBot.Api.Models;
