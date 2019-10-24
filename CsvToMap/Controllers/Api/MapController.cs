using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using CsvToMap.Models;
using MarkEmbling.PostcodesIO;
using Newtonsoft.Json;

namespace CsvToMap.Controllers.Api
{
    public class MapController : ApiController
    {
        private const string Path = "C:\\Users\\lorna.watson\\Documents\\Projects\\CsvToMap";

        [System.Web.Mvc.HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var list = new List<Map>();

                var directory = new DirectoryInfo(Path);
                if (directory.Exists)
                {
                    var files = directory.GetFiles().OrderByDescending(f => f.LastWriteTime);
                    if (files.Any())
                    {
                        var file = files.FirstOrDefault().FullName;

                        var json = File.ReadAllText(file);
                        list = JsonConvert.DeserializeObject<List<Map>>(json);
                    }
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}