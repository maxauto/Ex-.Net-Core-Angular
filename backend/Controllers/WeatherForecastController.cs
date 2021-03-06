﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api")]

    public class WeatherForecastController : ControllerBase
    {

        private readonly DataContext _databaseContext;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,DataContext context)
        {
            _databaseContext = context;
            _logger = logger;
        }
        #region  MSSQL database
        // [HttpGet("getdata")]
        // public async Task<IActionResult> GetData()
        // {
        //     try
        //     {
        //         var tagValueDatas = await _context.TagValue.ToListAsync();
        //         return Ok(tagValueDatas);
        //     }
        //     catch (Exception ex)
        //     {

        //         return NotFound( new{result=ex, message="fail"});
        //     }

        // }
    
        // [HttpGet("{id}")]

        // public async Task<IActionResult> GetDataByID(int id)
        // {
        //     try
        //     {
        //         var tagValueData = await _context.TagValue.FirstOrDefaultAsync(data => data.Id ==id);
        //         return Ok(new{result=tagValueData, message="success"});
        //     }
        //     catch (Exception ex)
        //     {
                
        //         return NotFound( new{result=ex, message="fail"});
        //     }
        // }

        // [HttpPut("updatedata")]
        // public async Task<IActionResult> UpdateData(TagValue result)
        // {
        //     try
        //     {
        //         var results =  await _context.TagValue.SingleOrDefaultAsync(data => data.Id == result.Id);
        //         if(result != null)
        //         {
        //             // update value
        //             results.Value = result.Value;
        //             results.Tagname = result.Tagname;

        //             _context.Update(results);
        //             _context.SaveChanges();
        //         }
        //         return Ok( new{result=result, message="sucess"});
        //     }
        //     catch (Exception ex)
        //     {
        //         return NotFound( new{result=ex, message="fail"});
        //     }
        // }
        #endregion
        #region PIWEBAPI
         [HttpGet("getTT01Value")]
        public async Task<IActionResult> getTT01Value()
        {
            try
            {
                // HttpClientHandler clientHandler = ( new HttpClientHandler() { UseDefaultCredentials = true });
                // clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; //access to https
                // HttpClient client = new HttpClient(clientHandler);
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                string TT01url = @"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwArAGOnh16hGVqQAMKaRzTQCemkhmq4GUS67Jlev1VN-gMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNlxGQUNUT1JZXEFSTTF8VFQtMDE/recorded?starttime=*-40d&endtime=*";

                HttpResponseMessage response = await client.GetAsync(TT01url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="sucees"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }


        [HttpGet("DIESELdailyFillVolVal/{selDate}")]
        public async Task<IActionResult> DIESELdailyFillVolVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                //DateTime seletedDate = new DateTime();
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string DDFVurl = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQmlrltdQkjUKd9XFOXW_NVgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfERBSUxZIEFNT1VOVCBGSUxMSU5HIFZPTFVNRSAoRElFU0VMKQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(DDFVurl);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95dailyFillVolVal/{selDate}")]
        public async Task<IActionResult> GAS95dailyFillVolVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQeEkyS9gKbkm7tuK-b_9ZmgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfERBSUxZIEFNT1VOVCBGSUxMSU5HIFZPTFVNRSAoR0FTR0hPTDk1KQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }
        
        [HttpGet("DIESELavgCycleTVal/{selDate}")]
        public async Task<IActionResult> DIESELavgCycleTVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQDxYI2MtknUekXOTzxhWuSAMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBEQUlMWSBBVkVSQUdFIENZQ0xFIFRJTUU/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95avgCycleTVal/{selDate}")]
        public async Task<IActionResult> GAS95avgCycleTVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQqoYYOAfpfEiVOWL4oRoFxAMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBEQUlMWSBBVkVSQUdFIENZQ0xFIFRJTUU/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("SOdailyavgWaitTVal/{selDate}")]
        public async Task<IActionResult> SOdailyAvgWaitTVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQtMZrpKeMDke_MQ-2GaHVHgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfFNBTEUgT0ZGSUNFIERBSUxZIEFWRVJBR0UgV0FJVElORyBUSU1F/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("IWBdailyAvgWaitTVal/{selDate}")]
        public async Task<IActionResult> IWBdailyAvgWaitTVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw3QL6Mjt_6hG4hVTudaRgyQvWLBAFJ2TUWlf-WK9vpFHwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXElOQk9VTkQgV0VJR0hUQlJJREdFfElOQk9VTkQgV0VJR0hCUklER0UgREFJTFkgQVZFUkFHRSBXQUlUSU5HIFRJTUU/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELdailyAvgWaitTVal/{selDate}")]
        public async Task<IActionResult> DIESELdailyAvgWaitTVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQEgikTfxvRESnesr4fjxtxwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBEQUlMWSBBVkVSQUdFIFdBSVRJTkcgVElNRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95dailyAvgWaitTVal/{selDate}")]
        public async Task<IActionResult> GAS95dailyAvgWaitTVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQEUq7d1pTYkeLV6ga4uj7VQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBEQUlMWSBBVkVSQUdFIFdBSVRJTkcgVElNRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("OWBdailyAvgWaitTVal/{selDate}")]
        public async Task<IActionResult> OWBdailyAvgWaitTVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwpSJFnjt_6hG4hVTudaRgyQqbtERL5W3U-gwc7iicbzjwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXE9VVEJPVU5EIFdFSUdIVEJSSURHRXxPVVRCT1VORCBXRUlHSEJSSURHRSBEQUlMWSBBVkVSQUdFIFdBSVRJTkcgVElNRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("dailyTruckInVal/{selDate}")]
        public async Task<IActionResult> dailyTruckInVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQL_OscsyLX0eWtxZ14Ua6xwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfERBSUxZIE5VTUJFUiBPRiBUUlVDS1MgSU4/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("dailyTruckOutVal/{selDate}")]
        public async Task<IActionResult> dailyTruckOutVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw0f0eqzt_6hG4hVTudaRgyQakbfH4mtk0K7bfkI2WqtEgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEVYSVQgR0FURXxEQUlMWSBOVU1CRVIgT0YgVFJVQ0tTIE9VVA/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("SOdailyAvgQVal/{selDate}")]
        public async Task<IActionResult> SOdailyAvgQVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQX_mCw5ZM70q7toyh3At2mwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfFNBTEUgT0ZGSUNFIERBSUxZIEFWRVJBR0UgTlVNQkVSIE9GIFFVRVVF/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("IWBdailyAvgQVal/{selDate}")]
        public async Task<IActionResult> IWBdailyAvgQVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw3QL6Mjt_6hG4hVTudaRgyQvBfMURpMYkaVbKFKIpaIuQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXElOQk9VTkQgV0VJR0hUQlJJREdFfElOQk9VTkQgV0VJR0hCUklER0UgREFJTFkgQVZFUkFHRSBOVU1CRVIgT0YgUVVFVUU/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELdailyAvgQVal/{selDate}")]
        public async Task<IActionResult> DIESELdailyAvgQVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQyyGzd5Z_B0Sf7C0YpVl5sAMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBEQUlMWSBBVkVSQUdFIE5VTUJFUiBPRiBRVUVVRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95dailyAvgQVal/{selDate}")]
        public async Task<IActionResult> GAS95dailyAvgQVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);

                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQ_-itW4CBpECG0I1jd--QLAMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBEQUlMWSBBVkVSQUdFIE5VTUJFUiBPRiBRVUVVRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
            } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("OWBdailyAvgQVal/{selDate}")]
        public async Task<IActionResult> OWBdailyAvgQVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwpSJFnjt_6hG4hVTudaRgyQDQepp_AFMU6McMb3i-j7dgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXE9VVEJPVU5EIFdFSUdIVEJSSURHRXxPVVRCT1VORCBXRUlHSEJSSURHRSBEQUlMWSBBVkVSQUdFIE5VTUJFUiBPRiBRVUVVRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELdailyAvgWIPVal/{selDate}")]
        public async Task<IActionResult> DIESELdailyAvgWIPVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQ3g2In2D_20mTm96BjY_jwwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBEQUlMWSBBVkVSQUdFIFdJUA/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95dailyAvgWIPVal/{selDate}")]
        public async Task<IActionResult> GAS95dailyAvgWIPVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQSBstWfe-f0qNBPRMLUJ0wQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBEQUlMWSBBVkVSQUdFIFdJUA/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

         [HttpGet("SOdailyAvgSchUVal/{selDate}")]
        public async Task<IActionResult> SOdailyAvgSchUVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQgwkApjFuLEm5dHSOTpMV4gMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfFNBTEUgT0ZGSUNFIERBSUxZIEFWRVJBR0UgU0NIX1VUSUxJWkFUSU9O/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("IWBdailyAvgSchUVal/{selDate}")]
        public async Task<IActionResult> IWBdailyAvgSchUVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw3QL6Mjt_6hG4hVTudaRgyQuGl69bRgKkGm2_WTBtwKkgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXElOQk9VTkQgV0VJR0hUQlJJREdFfElOQk9VTkQgV0VJR0hCUklER0UgREFJTFkgQVZFUkFHRSBTQ0hfVVRJTElaQVRJT04/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELdailyAvgSchUVal/{selDate}")]
        public async Task<IActionResult> DIESELdailyAvgSchUVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQHT_vByThs0uqV9drwYFHNwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBEQUlMWSBBVkVSQUdFIFNDSF9VVElMSVpBVElPTg/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95dailyAvgSchUVal/{selDate}")]
        public async Task<IActionResult> GAS95dailyAvgSchUVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQwVFMF7crK0-2lxA8HObuFAMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBEQUlMWSBBVkVSQUdFIFNDSF9VVElMSVpBVElPTg/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("OWBdailyAvgSchUVal/{selDate}")]
        public async Task<IActionResult> OWBdailyAvgSchUVal(DateTime selDate)
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Now;
                TimeSpan value =  today.Subtract(selDate);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwpSJFnjt_6hG4hVTudaRgyQNGiQoZDhcES7qpjbxLW-fQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXE9VVEJPVU5EIFdFSUdIVEJSSURHRXxPVVRCT1VORCBXRUlHSEJSSURHRSBEQUlMWSBBVkVSQUdFIFNDSF9VVElMSVpBVElPTg/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELmoFillVolVal")]
        public async Task<IActionResult> DIESELmoFillVolVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQSq6-ynwuZ027YrESOARmHgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfE1PTlRITFkgQU1PVU5UIEZJTExJTkcgVk9MVU1FIChESUVTRUwp/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95moFillVolVal")]
        public async Task<IActionResult> GAS95moFillVolVall()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQp_Ugjr0CHUO8q8GrKmCpWgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfE1PTlRITFkgQU1PVU5UIEZJTExJTkcgVk9MVU1FIChHQVNHSE9MOTUp/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELmoAvgCycleTVal")]
        public async Task<IActionResult> DIESELmoAvgCycleTVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQ78fculoS7EenUeUoxErJ6wMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBNT05USExZIEFWRVJBR0UgQ1lDTEUgVElNRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

         [HttpGet("GAS95moAvgCycleTVal")]
        public async Task<IActionResult> GAS95moAvgCycleTVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQi259JiczK0-PUMCwxPGVJgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBNT05USExZIEFWRVJBR0UgQ1lDTEUgVElNRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("SOmoAvgWaitTVal")]
        public async Task<IActionResult> SOmoAvgWaitTVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQb8aCx9Xoskm752qdOuaAywMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfFNBTEUgT0ZGSUNFIE1PTlRITFkgQVZFUkFHRSBXQUlUSU5HIFRJTUU/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("IWBmoAvgWaitTVal")]
        public async Task<IActionResult> IWBmoAvgWaitTVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw3QL6Mjt_6hG4hVTudaRgyQq9Fn6jKFZEWE7n0mFLXfmgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXElOQk9VTkQgV0VJR0hUQlJJREdFfElOQk9VTkQgV0VJR0hCUklER0UgTU9OVEhMWSBBVkVSQUdFIFdBSVRJTkcgVElNRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELmoAvgWaitTVal")]
        public async Task<IActionResult> DIESELmoAvgWaitTVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQ1MvOamkwMkaxPr0VdzCL6AMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBNT05USExZIEFWRVJBR0UgV0FJVElORyBUSU1F/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95moAvgWaitTVal")]
        public async Task<IActionResult> GAS95moAvgWaitTVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQh7g9kA2zpkyT3RBHoL7bTwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBNT05USExZIEFWRVJBR0UgV0FJVElORyBUSU1F/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("OWBmoAvgWaitTVal")]
        public async Task<IActionResult> OWBmoAvgWaitTVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwpSJFnjt_6hG4hVTudaRgyQtOhLixf8X0OM5y52k23ZHQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXE9VVEJPVU5EIFdFSUdIVEJSSURHRXxPVVRCT1VORCBXRUlHSEJSSURHRSBNT05USExZIEFWRVJBR0UgV0FJVElORyBUSU1F/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("moTruckInVal")]
        public async Task<IActionResult> moTruckInVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQcM7SdYlB306OjK2wizzSaQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfE1PTlRITFkgTlVNQkVSIE9GIFRSVUNLUyBJTg/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("moTruckOutVal")]
        public async Task<IActionResult> moTruckOutVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw0f0eqzt_6hG4hVTudaRgyQt7urOVHmr02o78fIOzV5pQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEVYSVQgR0FURXxNT05USExZIE5VTUJFUiBPRiBUUlVDS1MgT1VU/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("SOmoAvgQVal")]
        public async Task<IActionResult> SOmoAvgQVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQT-o9C6HmqESzrDhU-sn4jAMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfFNBTEUgT0ZGSUNFIE1PTlRITFkgQVZFUkFHRSBOVU1CRVIgT0YgUVVFVUU/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("IWBmoAvgQVal")]
        public async Task<IActionResult> IWBmoAvgQVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw3QL6Mjt_6hG4hVTudaRgyQ153hLBU4LUm7jKvEcnUyYwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXElOQk9VTkQgV0VJR0hUQlJJREdFfElOQk9VTkQgV0VJR0hCUklER0UgTU9OVEhMWSBBVkVSQUdFIE5VTUJFUiBPRiBRVUVVRQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELmoAvgQVal")]
        public async Task<IActionResult> DIESELmoAvgQVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQ8Ad9yfsPVECdTrMR_fh2igMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBNT05USExZIEFWRVJBR0UgTlVNQkVSIE9GIFFVRVVF/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95moAvgQVal")]
        public async Task<IActionResult> GAS95moAvgQVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQPmQL7nKeDket1wvLG4iZfQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBNT05USExZIEFWRVJBR0UgTlVNQkVSIE9GIFFVRVVF/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("OWBmoAvgQVal")]
        public async Task<IActionResult> OWBmoAvgQVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwpSJFnjt_6hG4hVTudaRgyQSAoahRteUE-GUXyrjQQm-AMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXE9VVEJPVU5EIFdFSUdIVEJSSURHRXxPVVRCT1VORCBXRUlHSEJSSURHRSBNT05USExZIEFWRVJBR0UgTlVNQkVSIE9GIFFVRVVF/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELmoAvgWIPVal")]
        public async Task<IActionResult> DIESELmoAvgWIPVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQrN-naxcEfUyRsddgiG_2lQMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBNT05USExZIEFWRVJBR0UgV0lQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95moAvgWIPVal")]
        public async Task<IActionResult> GAS95moAvgWIPVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQ01s0YgHk-0epu4FdGCqyHwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBNT05USExZIEFWRVJBR0UgV0lQ/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("SOmoAvgSchUVal")]
        public async Task<IActionResult> SOmoAvgSchUVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwOw4HFTt_6hG4hVTudaRgyQc0OiOk1YG0ee1ZP_tVrJIgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXFNBTEUgT0ZGSUNFfFNBTEUgT0ZGSUNFIE1PTlRITFkgQVZFUkFHRSBTQ0hfVVRJTElaQVRJT04/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("IWBmoAvgSchUVal")]
        public async Task<IActionResult> IWBmoAvgSchUVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhw3QL6Mjt_6hG4hVTudaRgyQnS1cS_MHy0uxgjBrHggIdwMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXElOQk9VTkQgV0VJR0hUQlJJREdFfElOQk9VTkQgV0VJR0hCUklER0UgTU9OVEhMWSBBVkVSQUdFIFNDSF9VVElMSVpBVElPTg/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("DIESELmoAvgSchUVal")]
        public async Task<IActionResult> DIESELmoAvgSchUVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwny3Vcjt_6hG4hVTudaRgyQKRLFA-aRWUOhjvnrPmkdtAMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXERJRVNFTCBCQVl8RElFU0VMIEJBWSBNT05USExZIEFWRVJBR0UgU0NIX1VUSUxJWkFUSU9O/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("GAS95moAvgSchUVal")]
        public async Task<IActionResult> GAS95moAvgSchUVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwwBQnlDt_6hG4hVTudaRgyQTJ0BtnvOVU-tkNUf7ZAvCgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXEdBU09IT0w5NSBCQVl8R0FTT0hPTDk1IEJBWSBNT05USExZIEFWRVJBR0UgU0NIX1VUSUxJWkFUSU9O/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("OWBmoAvgSchUVal")]
        public async Task<IActionResult> OWBmoAvgSchUVal()
        {
            try
            {
                var credentrials = new NetworkCredential("group2","inc.382");  // using username & password for login 
                HttpClientHandler clientHandler = new HttpClientHandler { Credentials = credentrials };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                DateTime today = DateTime.Today;
                DateTime month = new DateTime(2018,3,31);
                TimeSpan value =  today.Subtract(month);
                string starttime = Convert.ToString(Convert.ToInt32(value.TotalDays));
                string endtime = Convert.ToString(Convert.ToInt32(value.TotalDays) - 1);

                string url = $@"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwpSJFnjt_6hG4hVTudaRgyQ_hnEZFqSBkWFjAhEszbEFgMjAyLjQ0LjEyLjE0NlxHUk9VUDJfNFxGQUNUT1JZXE9VVEJPVU5EIFdFSUdIVEJSSURHRXxPVVRCT1VORCBXRUlHSEJSSURHRSBNT05USExZIEFWRVJBR0UgU0NIX1VUSUxJWkFUSU9O/recorded?starttime=*-{starttime}d&endtime=*-{endtime}d";

                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp",item["Timestamp"].Value<string>());
                        dataPair.Add("value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok(new {result=result, message="success"});
           } 
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("getSOData/Din/{SelDate}")]
        public IActionResult getSODataDateIn(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.SaleOfficeData.FromSqlRaw("SELECT * FROM _SaleOfficeData WHERE Date_In={0}",SelDate).ToList();
                var _result = (from s in _databaseContext.SaleOfficeData
                               join p in _databaseContext.Popaper
                               on s.PoNo equals p.PoNo
                               select new {
                                   s.DateIn,
                                   s.DateOut,
                                   s.TimeIn,
                                   s.TimeOut,
                                   s.PoNo,
                                   s.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.DateIn == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getSOData/Dout/{SelDate}")]
        public IActionResult getSODataDateOut(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.SaleOfficeData.FromSqlRaw("SELECT * FROM _SaleOfficeData WHERE Date_Out={0}",SelDate).ToList();
                var _result = (from s in _databaseContext.SaleOfficeData
                               join p in _databaseContext.Popaper
                               on s.PoNo equals p.PoNo
                               select new {
                                   s.DateIn,
                                   s.DateOut,
                                   s.TimeIn,
                                   s.TimeOut,
                                   s.PoNo,
                                   s.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.DateOut == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getSOData/Po/{POno}")]
        public IActionResult getSODataPO(string POno)
        {
            try
            {
                // var _result = _databaseContext.SaleOfficeData.FromSqlRaw($"SELECT* FROM _SaleOfficeData WHERE PO_no={POno}").ToList();
                var _result = (from s in _databaseContext.SaleOfficeData
                               join p in _databaseContext.Popaper
                               on s.PoNo equals p.PoNo
                               select new {
                                   s.DateIn,
                                   s.DateOut,
                                   s.TimeIn,
                                   s.TimeOut,
                                   s.PoNo,
                                   s.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.PoNo == POno).ToList();

                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getSOData/Tid/{id}")]
        public IActionResult getSODataTruckID(string id)
        {
            try
            {
                // var _result = _databaseContext.SaleOfficeData.FromSqlRaw("SELECT* FROM _SaleOfficeData WHERE Truck_ID={0}",id).ToList();
                var _result = (from s in _databaseContext.SaleOfficeData
                               join p in _databaseContext.Popaper
                               on s.PoNo equals p.PoNo
                               select new {
                                   s.DateIn,
                                   s.DateOut,
                                   s.TimeIn,
                                   s.TimeOut,
                                   s.PoNo,
                                   s.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.TruckId == id).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getIWBData/Din/{SelDate}")]
        public IActionResult getIWBDataDateIn(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.InboundWbdata.FromSqlRaw("SELECT * FROM _InboundWBData WHERE Date_In={0}",SelDate).ToList();
                var _result = (from i in _databaseContext.InboundWbdata
                               join p in _databaseContext.Popaper
                               on i.PoNo equals p.PoNo
                               select new {
                                   i.DateIn,
                                   i.DateOut,
                                   i.TimeIn,
                                   i.TimeOut,
                                   i.PoNo,
                                   i.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.DateIn == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getIWBData/Dout/{SelDate}")]
        public IActionResult getIWBDataDateOut(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.InboundWbdata.FromSqlRaw("SELECT * FROM _InboundWBData WHERE Date_Out={0}",SelDate).ToList();
                var _result = (from i in _databaseContext.InboundWbdata
                               join p in _databaseContext.Popaper
                               on i.PoNo equals p.PoNo
                               select new {
                                   i.DateIn,
                                   i.DateOut,
                                   i.TimeIn,
                                   i.TimeOut,
                                   i.PoNo,
                                   i.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.DateOut == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getIWBData/Po/{POno}")]
        public IActionResult getIWBDataPO(string POno)
        {
            try
            {
                // var _result = _databaseContext.InboundWbdata.FromSqlRaw("SELECT * FROM _InboundWBData WHERE PO_no={0}",POno).ToList();
                var _result = (from i in _databaseContext.InboundWbdata
                               join p in _databaseContext.Popaper
                               on i.PoNo equals p.PoNo
                               select new {
                                   i.DateIn,
                                   i.DateOut,
                                   i.TimeIn,
                                   i.TimeOut,
                                   i.PoNo,
                                   i.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.PoNo == POno).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getIWBData/Tid/{id}")]
        public IActionResult getIWBDataTruckID(string id)
        {
            try
            {
                // var _result = _databaseContext.InboundWbdata.FromSqlRaw("SELECT * FROM _InboundWBData WHERE Truck_ID={0}",id).ToList();
                var _result = (from i in _databaseContext.InboundWbdata
                               join p in _databaseContext.Popaper
                               on i.PoNo equals p.PoNo
                               select new {
                                   i.DateIn,
                                   i.DateOut,
                                   i.TimeIn,
                                   i.TimeOut,
                                   i.PoNo,
                                   i.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.TruckId == id).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getBayData/Din/{SelDate}")]
        public IActionResult getBayDataDateIn(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.BayData.FromSqlRaw("SELECT * FROM BayData WHERE Date_In={0}",SelDate).ToList();
                var _result = (from b in _databaseContext.BayData
                               join p in _databaseContext.Popaper on b.PoNo equals p.PoNo
                               join g in _databaseContext.Gas on p.Item equals g.GasId
                               select new {
                                   b.DateIn,
                                   b.DateOut,
                                   b.TimeIn,
                                   b.TimeOut,
                                   b.PoNo,
                                   b.ServiceTime,
                                   p.TruckId,
                                   g.GasType,
                                   p.Quantity
                               }).Where( o => o.DateIn == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getBayData/Dout/{SelDate}")]
        public IActionResult getBayDataDateOut(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.BayData.FromSqlRaw("SELECT * FROM BayData WHERE Date_Out={0}",SelDate).ToList();
                var _result = (from b in _databaseContext.BayData
                               join p in _databaseContext.Popaper on b.PoNo equals p.PoNo
                               join g in _databaseContext.Gas on p.Item equals g.GasId
                               select new {
                                   b.DateIn,
                                   b.DateOut,
                                   b.TimeIn,
                                   b.TimeOut,
                                   b.PoNo,
                                   b.ServiceTime,
                                   p.TruckId,
                                   g.GasType,
                                   p.Quantity
                               }).Where( o => o.DateOut == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getBayData/Po/{POno}")]
        public IActionResult getBayDataDatePO(string POno)
        {
            try
            {
                // var _result = _databaseContext.BayData.FromSqlRaw("SELECT * FROM _BayData WHERE PO_no={0}",POno).ToList();
                var _result = (from b in _databaseContext.BayData
                               join p in _databaseContext.Popaper on b.PoNo equals p.PoNo
                               join g in _databaseContext.Gas on p.Item equals g.GasId
                               select new {
                                   b.DateIn,
                                   b.DateOut,
                                   b.TimeIn,
                                   b.TimeOut,
                                   b.PoNo,
                                   b.ServiceTime,
                                   p.TruckId,
                                   g.GasType,
                                   p.Quantity
                               }).Where( o => o.PoNo == POno).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getBayData/Tid/{id}")]
        public IActionResult getBayDataTruckID(string id)
        {
            try
            {
                // var _result = _databaseContext.BayData.FromSqlRaw("SELECT * FROM _BayData WHERE Truck_ID={0}",id).ToList();
                var _result = (from b in _databaseContext.BayData
                               join p in _databaseContext.Popaper on b.PoNo equals p.PoNo
                               join g in _databaseContext.Gas on p.Item equals g.GasId
                               select new {
                                   b.DateIn,
                                   b.DateOut,
                                   b.TimeIn,
                                   b.TimeOut,
                                   b.PoNo,
                                   b.ServiceTime,
                                   p.TruckId,
                                   g.GasType,
                                   p.Quantity   
                               }).Where( o => o.TruckId == id).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getOWBData/Din/{SelDate}")]
        public IActionResult getOWBDataDateIn(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.OutboundWbdata.FromSqlRaw("SELECT * FROM _OutboundWbdata WHERE Date_In={0}",SelDate).ToList();
                var _result = (from o in _databaseContext.OutboundWbdata
                               join p in _databaseContext.Popaper
                               on o.PoNo equals p.PoNo
                               select new {
                                   o.DateIn,
                                   o.DateOut,
                                   o.TimeIn,
                                   o.TimeOut,
                                   o.PoNo,
                                   o.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.DateIn == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getOWBData/Dout/{SelDate}")]
        public IActionResult getOWBDataDateOut(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.OutboundWbdata.FromSqlRaw("SELECT * FROM _OutboundWbdata WHERE Date_Out={0}",SelDate).ToList();
                var _result = (from o in _databaseContext.OutboundWbdata
                               join p in _databaseContext.Popaper
                               on o.PoNo equals p.PoNo
                               select new {
                                   o.DateIn,
                                   o.DateOut,
                                   o.TimeIn,
                                   o.TimeOut,
                                   o.PoNo,
                                   o.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.DateOut == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getOWBData/Po/{POno}")]
        public IActionResult getOWBDataPO(string POno)
        {
            try
            {
                // var _result = _databaseContext.OutboundWbdata.FromSqlRaw("SELECT * FROM _OutboundWBData WHERE PO_no={0}",POno).ToList();
                var _result = (from o in _databaseContext.OutboundWbdata
                               join p in _databaseContext.Popaper
                               on o.PoNo equals p.PoNo
                               select new {
                                   o.DateIn,
                                   o.DateOut,
                                   o.TimeIn,
                                   o.TimeOut,
                                   o.PoNo,
                                   o.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.PoNo == POno).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getOWBData/Tid/{id}")]
        public IActionResult getOWBDataTruckID(string id)
        {
            try
            {
                // var _result = _databaseContext.OutboundWbdata.FromSqlRaw("SELECT * FROM _OutboundWbdata WHERE Truck_ID={0}",id).ToList();
                var _result = (from o in _databaseContext.OutboundWbdata
                               join p in _databaseContext.Popaper
                               on o.PoNo equals p.PoNo
                               select new {
                                   o.DateIn,
                                   o.DateOut,
                                   o.TimeIn,
                                   o.TimeOut,
                                   o.PoNo,
                                   o.ServiceTime,
                                   p.TruckId
                               }).Where( o => o.TruckId == id).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getEGData/Din/{SelDate}")]
        public IActionResult getEGDataDateIn(DateTime SelDate)
        {
            try
            {
                // var _result = _databaseContext.ExitGateData.FromSqlRaw("SELECT * FROM _ExitGateData WHERE Date_In={0}",SelDate).ToList();
                var _result = (from e in _databaseContext.ExitGateData
                               join p in _databaseContext.Popaper
                               on e.PoNo equals p.PoNo
                               select new {
                                   e.DateIn,
                                   e.TimeIn,
                                   e.PoNo,
                                   p.TruckId
                               }).Where( o => o.DateIn == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getEGData/Po/{POno}")]
        public IActionResult getEGDataPO(string POno)
        {
            try
            {
                // var _result = _databaseContext.ExitGateData.FromSqlRaw("SELECT * FROM _ExitGateData WHERE PO_no={0}",POno).ToList();
                var _result = (from e in _databaseContext.ExitGateData
                               join p in _databaseContext.Popaper
                               on e.PoNo equals p.PoNo
                               select new {
                                   e.DateIn,
                                   e.TimeIn,
                                   e.PoNo,
                                   p.TruckId
                               }).Where( o => o.PoNo == POno).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getEGData/Tid/{id}")]
        public IActionResult getEGDataTruckID(string id)
        {
            try
            {
                // var _result = _databaseContext.ExitGateData.FromSqlRaw("SELECT * FROM _ExitGateData WHERE Truck_ID={0}",id).ToList();
                var _result = (from e in _databaseContext.ExitGateData
                               join p in _databaseContext.Popaper
                               on e.PoNo equals p.PoNo
                               select new {
                                   e.DateIn,
                                   e.TimeIn,
                                   e.PoNo,
                                   p.TruckId
                               }).Where( o => o.TruckId == id).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        // [HttpGet("getPOData")]
        // public IActionResult getPOData()
        // {
        //     try
        //     {
        //         var _result = _databaseContext.Popaper.FromSqlRaw("SELECT * FROM POPaper").Select(c => new _Popaper{
        //             PoNo = c.PoNo,
        //             PaymentNo = c.PaymentNo,
        //             Date = c.Date,
        //             Time = c.Time,
        //             CustomerId = c.CustomerId,
        //             Item = c.Item,
        //             UnitPriceId = c.UnitPriceId,
        //             Quantity = c.Quantity,
        //             Amount = c.Amount,
        //             TruckId = c.TruckId
        //         }).ToList();
        //         return Ok( new{result=_result, message="sucess"});
        //     }
        //     catch (Exception ex)
        //     {
        //         return NotFound( new{result=ex, message="fail"});
        //     }
        // }

        [HttpGet("getReconShtData")]
        public IActionResult getReconShtData()
        {
            try
            {
                // var _result = _databaseContext.Popaper.FromSqlRaw("SELECT * FROM _POPaper").ToList();
                var _result = (from p in _databaseContext.Popaper
                               join g in _databaseContext.Gas on p.Item equals g.GasId
                               join c in _databaseContext.CustomerInfo on p.CustomerId equals c.CustomerId
                               select new {
                                   p.Date,
                                   p.PoNo,
                                   p.PaymentNo,
                                   p.InvoiceNo,
                                   p.TruckId,
                                   g.GasType,
                                   p.Quantity,
                                   p.Amount,
                                   c.Name
                               }).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getPOData/{POon}")]
        public IActionResult getPOData(String POon)
        {
            try
            {
                // var _result = _databaseContext.Popaper.FromSqlRaw("SELECT * FROM _POPaper WHERE Date={0}",SelDate).ToList();
                var _result = (from p in _databaseContext.Popaper
                               join g in _databaseContext.Gas on p.Item equals g.GasId
                               join c in _databaseContext.CustomerInfo on p.CustomerId equals c.CustomerId
                               join cp in _databaseContext.CostPriceGas on p.UnitPriceId equals cp.GasPriceId
                               join t in _databaseContext.Truck on p.TruckId equals t.TruckId
                               select new {
                                   p.Date,
                                   p.PoNo,
                                   p.PaymentNo,
                                   p.InvoiceNo,
                                   g.GasType,
                                   cp.Price,
                                   p.Quantity,
                                   p.Amount,
                                   p.CustomerId,
                                   c.Name,
                                   c.TaxPayerId,
                                   c.PhoneNo,
                                   p.TruckId,
                                   t.TruckDriverName
                               }).Where( o => o.PoNo == POon).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getInvoiceData/{IVno}")]
        public IActionResult getInvoiceData(String IVno)
        {
            try
            {
                // var _result = _databaseContext.Popaper.FromSqlRaw("SELECT * FROM _POPaper WHERE Date={0}",SelDate).ToList();
                var _result = (from p in _databaseContext.Popaper
                               join g in _databaseContext.Gas on p.Item equals g.GasId
                               join c in _databaseContext.CustomerInfo on p.CustomerId equals c.CustomerId
                               join cp in _databaseContext.CostPriceGas on p.UnitPriceId equals cp.GasPriceId
                               join b in _databaseContext.BayData on p.PoNo equals b.PoNo
                               select new {
                                   p.Date,
                                   p.InvoiceNo,
                                   p.PoNo,
                                   p.PaymentNo,
                                   b.ServiceTime,
                                   g.GasType,
                                   cp.Price,
                                   p.Quantity,
                                   p.Amount,
                                   p.CustomerId,
                                   c.Name,
                                   c.Address,
                                   c.TaxPayerId
                               }).Where( o => o.InvoiceNo == IVno).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getJournalData/{SelDate}")]
        public IActionResult getJournalTransData(DateTime SelDate)
        {
            try
            {
                var _result = (from j in _databaseContext.Transactions
                               select new {
                                   j.Date,
                                   j.Description,
                                   j.RefNo,
                                   j.Amount,
                                   j.Type
                               }).Where( o => o.Date == SelDate).ToList();
                return Ok( new{result=_result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("GetLedger/{account}")]
        public IActionResult GetLedger(string account)
        {
            try
            {
                List<Ledger> result = new List<Ledger>();
                var values = (from j in _databaseContext.Transactions
                               select new {
                                   j.Date,
                                   j.Description,
                                   j.RefNo,
                                   j.Amount,
                                   j.Type
                               }).ToList();
                
                if( account == "101" || account == "157" || account == "112" || account == "201" )
                {
                    int i = 0;
                    for(int a = 0;a < values.Count();a++)
                    {
                        if( account == "101" || account == "157" )
                        {
                            if("Credit" == values[a].Type && account == values[a].RefNo )
                            {
                                result.Add(new Ledger());
                                result[i].Date = values[a].Date;
                                result[i].Description = values[a-1].Description;
                                result[i].Amount = values[a].Amount;
                                result[i].RefNo = values[a].RefNo;
                                result[i].Type = values[a].Type;
                                result[i].JRefNo = "J"+Convert.ToString(Convert.ToDateTime(values[a].Date).Day);
                                ++i;
                            }else if("Debit" == values[a].Type && account == values[a].RefNo )
                            {
                                result.Add(new Ledger());
                                result[i].Date = values[a].Date;
                                result[i].Description = values[a].Description;
                                result[i].Amount = values[a].Amount;
                                result[i].RefNo = values[a].RefNo;
                                result[i].Type = values[a].Type;
                                result[i].JRefNo = "J"+Convert.ToString(Convert.ToDateTime(values[a].Date).Day);
                                ++i;
                            }
                        }else
                        {
                            if( account == values[a].RefNo )
                            {
                                result.Add(new Ledger());
                                result[i].Date = values[a].Date;
                                result[i].Description = values[a].Description;
                                result[i].Amount = values[a].Amount;
                                result[i].RefNo = values[a].RefNo;
                                result[i].Type = values[a].Type;
                                result[i].JRefNo = "J"+Convert.ToString(Convert.ToDateTime(values[a].Date).Day);
                                ++i;
                            }
                            
                        }
                            
                    }

                    int n = 0;
                    double? b = 0;
                    foreach(var item in result)
                    {
                        if(item.RefNo == "101" || item.RefNo == "112" || item.RefNo == "157")
                        {
                            b = item.Type == "Debit" ? b + item.Amount : b - item.Amount;
                            result[n].Date = item.Date;
                            result[n].Description = item.Description;
                            result[n].Amount = item.Amount;
                            result[n].RefNo = item.RefNo;
                            result[n].Type = item.Type;
                            result[n].JRefNo = item.JRefNo;
                            result[n].Balance = b;
                        }
                        else if(item.RefNo == "201")
                        {
                            b = item.Type == "Credit" ? b + item.Amount : b - item.Amount;

                            result[n].Date = item.Date;
                            result[n].Description = item.Description;
                            result[n].Amount = item.Amount;
                            result[n].RefNo = item.RefNo;
                            result[n].Type = item.Type;
                            result[n].JRefNo = item.JRefNo;
                            result[n].Balance = b;
                        }
                        ++n;
                    }
                }
                return Ok( new{result=result, message="success"});
                
            }
            catch(Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        [HttpGet("getIncomeStm")]
        public IActionResult getIncomeStm()
        {
            try
            {
                List<IncomeStatement> result = new List<IncomeStatement>();
                var values = (from j in _databaseContext.Transactions
                              join p in _databaseContext.Popaper on j.PoNo equals p.PoNo
                              join g in _databaseContext.Gas on p.Item equals g.GasId
                              select new
                              {
                                  j.Date,
                                  j.Description,
                                  j.RefNo,
                                  j.Amount,
                                  j.Type,
                                  g.GasType
                              }).ToList();
                var values1 = (from j in _databaseContext.Transactions
                               select new
                               {
                                   j.Date,
                                   j.Description,
                                   j.RefNo,
                                   j.PoNo,
                                   j.Amount,
                                   j.Type,
                               }).Where(o => o.PoNo == null).ToList();
                double? SG = 0, SD = 0, COGSG = 0, COGSD = 0, SSOS = 0, SGC = 0, UE = 0, DP = 0, PG = 0, PD = 0;
                foreach (var item in values)
                {
                    if (item.RefNo == "400" && item.GasType == "GASOHOL95" && item.Type == "Credit")
                    {
                        SG = SG + item.Amount;
                    }
                    else if (item.RefNo == "400" && item.GasType == "DIESEL" && item.Type == "Credit")
                    {
                        SD = SD + item.Amount;
                    }
                    else if (item.RefNo == "730" && item.GasType == "GASOHOL95" && item.Type == "Debit")
                    {
                        COGSG = COGSG + item.Amount;
                    }
                    else if (item.RefNo == "730" && item.GasType == "DIESEL" && item.Type == "Debit")
                    {
                        COGSD = COGSD + item.Amount;
                    }
                }
                foreach (var item1 in values1)
                {
                    if (item1.RefNo == "728")
                    {
                        UE = UE + item1.Amount;
                    }
                    else if (item1.Description == "Salaries and Wages Expense - Sale Office Staffs")
                    {
                        SSOS = SSOS + item1.Amount;
                    }
                    else if (item1.Description == "Salaries and Wages Expense - Gate Controller")
                    {
                        SGC = SGC + item1.Amount;
                    }
                    else if (item1.RefNo == "840")
                    {
                        DP = DP + item1.Amount;
                    }
                    else if (item1.RefNo == "157" && item1.Description == "Inventory - GASOHOL95" && item1.Type == "Debit")
                    {
                        PG = PG + item1.Amount;
                    }
                    else if (item1.RefNo == "157" && item1.Description == "Inventory - DIESEL" && item1.Type == "Debit")
                    {
                        PD = PD + item1.Amount;
                    }
                }

                var Dcost = (from c in _databaseContext.CostPriceGas
                            select new
                            {
                                c.Date,
                                c.GasId,
                                c.Cost
                            }).Where(o => o.GasId == "A01").ToList();
                var Gcost = (from c in _databaseContext.CostPriceGas
                            select new
                            {
                                c.Date,
                                c.GasId,
                                c.Cost
                            }).Where(o => o.GasId == "B01").ToList();

                double? Ds4avg = 0, Gs4avg = 0;
                foreach(var i in Dcost)
                {
                    Ds4avg += i.Cost;
                }
                foreach(var j in Gcost)
                {
                    Gs4avg += j.Cost;
                }

                result.Add(new IncomeStatement());
                result[0].SaleGAS95 = SG;
                result[0].SaleDIESEL = SD;
                result[0].TotalSale = SG + SD;
                result[0].COGSGAS95 = COGSG;
                result[0].COGSDIESEL = COGSD;
                result[0].TotalCOGS = COGSG + COGSD;
                result[0].GrossProfit = result[0].TotalSale - result[0].TotalCOGS;
                result[0].SalarySOS = SSOS;
                result[0].SalaryGC = SGC;
                result[0].TotalSalary = SSOS + SGC;
                result[0].UtilityExp = UE;
                result[0].Depreciation = DP;
                result[0].NetIncome = result[0].GrossProfit - SSOS - SGC - UE - DP;
                result[0].BeginGAS95 = 40000 * Gcost[0].Cost;
                result[0].BeginDIESEL = 30000 * Dcost[0].Cost;
                result[0].PurchaseGAS95 = PG;
                result[0].PurchaseDIESEL = PD;
                result[0].EndGAS95 = 23332.8 * (Gs4avg/Dcost.Count());
                result[0].EndDIESEL = 35060.56 * (Ds4avg/Gcost.Count());

                return Ok(new { result = result, message = "success" });
            }
            catch (Exception ex)
            {
                return NotFound(new { result = ex, message = "fail" });
            }
        }


        #endregion

    }
}
