![Build All](https://github.com/benchiverton/TweetVisualiser/workflows/Build%20All/badge.svg)

# Tweet Visualiser

This project visualises tweets related to specific topics. It's main purpose is to experiment with Azure's [Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction).

![](docs/TweetVisualiser.gif)

## This project

This project contains the following apps:

| App                      | Purpose                                                      |
| ------------------------ | ------------------------------------------------------------ |
| TweetVisualiser.Listener | Console App that streams tweets related to specific topics and pushes them to Cosmos DB. |
| TweetVisualiser.UI       | Blazor Web App that uses [Cosmos DB's Change Feed Processor](https://docs.microsoft.com/en-us/azure/cosmos-db/change-feed-processor) to stream tweets and calculate metrics related to them on the fly. |

### Getting started

All of the code is in the `src` folder. It requires the dotnet 5 SDK to run.

#### Cosmos DB

For this project, it's recommended to use the Cosmos DB Emulator instead of an Azure Cosmos DB Container, as Azure Cosmos DB Containers aren't free to run. You can either install the emulator locally, or run a docker image containing the emulator following [these steps](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator).

Once you've set up a local emulator, set the following environment variables:

`CosmosDB__Uri` (set to `https://localhost:8081` if the emulator has been installed locally)

`CosmosDB__PrimaryKey`

**Note** - Databases/Containers are created when the TweetVisualiser.Listener app is started, **not** when the UI is started. This is because Databases/Containers cannot be created in the blazor DI lifecycle.

#### Twitter API

The `TweetVisualiser.Listener` app required a Twitter Developer Account. If you don't have one, you can apply for access [here](https://developer.twitter.com/en/apply-for-access).

Create a new app from the [developer portal](https://developer.twitter.com/en/portal/dashboard). As the app is only be reading data we only need the Consumer Key and Consumer Secret. Once you have these, set the following environment variables to the values for this application:

`TweetListener__ConsumerKey`

`TweetListener__ConsumerSecret`