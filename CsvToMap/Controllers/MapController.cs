using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using CsvToMap.Models;
using Newtonsoft.Json;

namespace CsvToMap.Controllers
{
    [Route("api/Map")]
    public class MapController : ApiController
    { 
        [System.Web.Mvc.HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var list = new List<Map>();

                var directory = new DirectoryInfo(PathConstant.Path);
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