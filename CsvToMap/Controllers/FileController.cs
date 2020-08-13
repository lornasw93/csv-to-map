using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using CsvToMap.Models;
using MarkEmbling.PostcodesIO;
using Newtonsoft.Json;

namespace CsvToMap.Controllers
{
    public class FileController : ApiController
    {
        [Route("api/File")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var list = new List<Map>();
                var data = File.ReadAllLines($"{PathConstant.Path}\\LatitudeLongitude.csv");

                list = data.Skip(1)
                    .Select(t => t.Split(','))
                    .Select(split => new Map
                    {
                        Latitude = Convert.ToDouble(split[0]),
                        Longitude = Convert.ToDouble(split[1])
                    }).ToList();

                var postcodeList = GetPostcodes();
                var count = postcodeList.Count() / 100;
                var chunks = ChunkBy(postcodeList.ToList(), count);
                var client = new PostcodesIOClient();

                foreach (var chunk in chunks)
                {
                    var bulkResult = client.BulkLookup(chunk);
                    list.AddRange(from item in bulkResult
                                  where item.Result != null
                                  select new Map
                                  {
                                      Latitude = Convert.ToDouble(item.Result.Latitude),
                                      Longitude = Convert.ToDouble(item.Result.Longitude)
                                  });
                }

                // write to file - results.json
                SerializeToFile(list);

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static List<string> GetPostcodes()
        {
            var data = File.ReadAllLines($"{PathConstant.Path}\\Postcodes.csv");
            var postcodes = data.Skip(1)
                .Select(c => !string.IsNullOrWhiteSpace(c.Split(',')[0])
                    ? c.Split(',')[0]
                    : string.Empty);
            return postcodes.ToList();
        }

        private static void SerializeToFile(List<Map> list)
        {
            using (var stream = File.CreateText($"{PathConstant.Path}\\Results.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(stream, list);
            }
        }

        private static List<List<T>> ChunkBy<T>(IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}