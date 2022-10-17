namespace TravisService.Adapters.DynamoDB.Infrastructure
{
    public class DynamoDBSettings
    {
        public string Profile { get; set; }

        public string AwsRegion { get; set; }

        public string LookupTableName { get; set; }
    }
}
