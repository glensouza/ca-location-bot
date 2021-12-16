namespace CALocationBot.Api.CityCounty;

public class SearchService
{
    private SearchClient searchClient;
    private readonly SearchIndexClient adminClient;
    private readonly ILogger logger;

    public SearchService(string serviceName, string indexName, string apiKey, ExecutionContext executionContext, ILogger logger)
    {
        this.logger = logger;

        // Create a SearchIndexClient to send create/delete index commands
        Uri serviceEndpoint = new($"https://{serviceName}.search.windows.net/");
        AzureKeyCredential credential = new(apiKey);
        this.adminClient = new SearchIndexClient(serviceEndpoint, credential);
        if (this.adminClient.GetIndexNames().All(s => s != indexName)) //.GetIndexes().All(index => index.Name != indexName))
        {
            CreateIndex(indexName, serviceEndpoint, credential, executionContext);
        }
        else
        {
            // Create a SearchClient to load and query documents
            this.searchClient = new SearchClient(serviceEndpoint, indexName, credential);
            if (this.searchClient.GetDocumentCount() <= 0)
            {
                CreateIndex(indexName, serviceEndpoint, credential, executionContext);
            }
        }
    }

    private void CreateIndex(string indexName, Uri serviceEndpoint, AzureKeyCredential credential, ExecutionContext executionContext)
    {
        // Create index
        FieldBuilder fieldBuilder = new();
        IList<SearchField> searchFields = fieldBuilder.Build(typeof(CityCountySearchIndex));

        SearchIndex definition = new(indexName, searchFields);

        SearchSuggester suggester = new("sg", new[] { "City", "County" });
        definition.Suggesters.Add(suggester);

        try
        {
            this.adminClient.CreateOrUpdateIndex(definition);
            System.Threading.Thread.Sleep(2000);
        }
        catch (Exception e)
        {
            this.logger.LogError($"Error ");
            throw;
        }

        // Upload documents in a single Upload request.
        CityCountyService cityCountyService = new(executionContext);
        IEnumerable<CityCountyModel> cityCounties = cityCountyService.AllCityCounties;
        List<CityCountySearchIndex> newIndexes = cityCounties.Select(s => new CityCountySearchIndex { City = s.City, County = s.County }).ToList();
        int counter = 0;
        foreach (CityCountySearchIndex cityCountySearchIndex in newIndexes)
        {
            counter++;
            cityCountySearchIndex.CityId = counter.ToString();
        }
        IndexDocumentsBatch<CityCountySearchIndex> batch = IndexDocumentsBatch.MergeOrUpload(newIndexes);

        // Create a SearchClient to load and query documents
        this.searchClient = new SearchClient(serviceEndpoint, indexName, credential);

        try
        {
            IndexDocumentsResult result = this.searchClient.IndexDocuments(batch);
        }
        catch (Exception ex)
        {
            // If for some reason any documents are dropped during indexing, you can compensate by delaying and retrying.
            // This logs the failed document keys and continues.
            this.logger.LogError($"Failed to index some of the documents: {ex.Message}");
        }
    }

    public CityCountyModel Search(string text = "", string filter = "")
    {
        if (string.IsNullOrEmpty(text))
        {
            text = "*";
        }

        SearchOptions options = new()
        {
            IncludeTotalCount = true,
            Filter = filter,
            OrderBy = { "City", "County" }
        };

        options.Select.Add("City");
        options.Select.Add("County");

        SearchResults<CityCountySearchIndex> indexes = this.searchClient.Search<CityCountySearchIndex>(text, options);
        List<SearchResult<CityCountySearchIndex>> cityCounties = indexes.GetResults().ToList();
        CityCountySearchIndex cityCounty = cityCounties.OrderByDescending(s => s.Score).FirstOrDefault()?.Document;
        return new CityCountyModel
        {
            County = cityCounty?.County,
            City = cityCounty?.City
        };
    }
}
