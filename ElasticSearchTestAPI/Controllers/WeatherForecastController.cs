using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchTestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];
        private readonly ElasticsearchClient _elasticsearchClient;

        public WeatherForecastController(ElasticsearchClient elasticsearchClient)
        {
            _elasticsearchClient = elasticsearchClient;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "PostWeatherForecast")]
        public async Task<IActionResult> Post([FromBody] WeatherForecast weatherForecast)
        {
            var response = await _elasticsearchClient.IndexAsync(weatherForecast, idx => idx.Index("weather"));
            if (response.IsValidResponse)
            {
                return CreatedAtAction(nameof(Post), new { id = weatherForecast.Date }, weatherForecast);
            }
            return BadRequest("Failed to index the weather forecast in Elasticsearch.");
        }
        [HttpPost("bulk", Name = "CreateMultiIndex")]
        public async Task<IActionResult> CreateMultiIndex([FromBody] IEnumerable<WeatherForecast> weatherForecasts)
        {
            var bulkResponse = await _elasticsearchClient.BulkAsync(b => b.Index("weather").IndexMany(weatherForecasts));
            if (bulkResponse.IsValidResponse)
            {
                return CreatedAtAction(nameof(CreateMultiIndex), new { count = weatherForecasts.Count() }, weatherForecasts);
            }
            return BadRequest("Failed to index the weather forecasts in Elasticsearch.");
        }
    }
}
