@description('Name of the Cosmos DB account')
param cosmosDBAccountName string

@description('Location to deploy the Cosmos DB account')
param location string

@description('Name for the Database')
param databaseName string

@description('Name for the Container')
param containerName string

resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-10-15' = {
  name: cosmosDBAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    enableFreeTier: true
    databaseAccountOfferType: 'Standard' 
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
  }
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-10-15' = {
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-10-15' = {
  name: containerName
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/id'
        ]
      }
    }
  }
}
