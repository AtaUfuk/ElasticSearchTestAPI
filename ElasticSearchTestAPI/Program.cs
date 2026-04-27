using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using ElasticSearchTestAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var elasticSearchConfig = builder.Configuration.GetSection("ElasticSearch").Get<ElasticSearch>();
var settings=new ElasticsearchClientSettings(new Uri(elasticSearchConfig.Uri)).ClientCertificate(elasticSearchConfig.ClientSertificatePath).CertificateFingerprint(elasticSearchConfig.CertificateFingerprint)
    .DefaultIndex(elasticSearchConfig.DefaultIndex).Authentication(new BasicAuthentication(elasticSearchConfig.Username, elasticSearchConfig.Password));
var client=new ElasticsearchClient(settings);
builder.Services.AddSingleton(client);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
