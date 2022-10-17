using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace TravisService.Adapters.DynamoDB.Extensions
{
    internal static class ItemExtensions
    {
        public static string GetHandheldId(this Dictionary<string, AttributeValue> Item) =>
            Item["HandheldId"].S;
    }
}
