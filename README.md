# net-travis-service

I am an AWS SAM stack that sends an alert whenever a handheld device travels more than 50 meters from its associated vehicle

## Functions

### Handheld events processor
This function processes handheld events from a queue in batches of up to 10 events at a time and stores them in a database. This function is triggered by a queue.

![Handheld-processor](/docs/handheld.png)

### Vehicle event processor
This function processes vehicle events from a queue in batches of up to 10 events at a time. It checks the lookup table to find the handheld device paired with the vehicle and gets the relevant events for the handheld device from the database. If the distance between them is >= 50m, an alert is sent to an SNS topic. This function is triggered by a queue.

![Vehicle-processor](/docs/vehicle.png)

## Run locally
The functions can be run locally with the following requirements:
- Docker (compose file can be found in \docker)
- [Lambda test tool](https://github.com/aws/aws-lambda-dotnet/tree/master/Tools/LambdaTestTool)
- Dev AWS environment with all resources created (Only applicable to Vehicl event processor)

Test payload for each function is included in /src/Functions/{functionName}/.lambda-test-tool