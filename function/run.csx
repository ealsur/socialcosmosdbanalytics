#r "Microsoft.Azure.Documents.Client"
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

public static async Task Run(IReadOnlyList<Document> input, IAsyncCollector<dynamic> results)
{
    // Create a client.
    ITextAnalyticsAPI client = new TextAnalyticsAPI();
    client.AzureRegion = AzureRegions.Westus; // Change if it's in another region
    client.SubscriptionKey = "<your-text-analytics-api-key>";

    string languageToAnalyze = "en";
    foreach(var doc in input){
         SentimentBatchResult sentiment = client.Sentiment(
                    new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput(languageToAnalyze, "0", doc.GetPropertyValue<string>("message"))
                        }));


        var result = new {
            id = doc.Id,
            message = doc.GetPropertyValue<string>("message"),
            score = sentiment.Documents[0].Score
        };

        await results.AddAsync(result);
    }
}