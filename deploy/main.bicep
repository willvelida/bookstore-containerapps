@description('Name of the log analytics workspace')
param logAnalyticsName string

@description('Name of the connected Container Registry')
param containerRegistryName string

@description('Name of the Container App Environment')
param containerAppEnvName string

@description('Name of the Book Store Container App')
param bookApiContainerName string

@description('Name of the Cosmos DB account')
param cosmosDBAccountName string

@description('Name of the Database inside Cosmos DB')
param databaseName string

@description('Name of the Container inside Cosmos DB')
param containerName string

@description('Name of the APIM instance')
param apimName string

@description('Username for the Container Registry')
param acrUserName string

@description('Password for the Container Registry')
@secure()
param acrPassword string

param location string = resourceGroup().location

var publisherEmail = 'willvelida@hotmail.co.uk'
var publisherName = 'Will Velida'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-12-01-preview' = {
  name: containerRegistryName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource containerAppEnvironment 'Microsoft.Web/kubeEnvironments@2021-03-01' = {
  name: containerAppEnvName
  location: location 
  kind: 'containerenvironment'
  properties: {
    environmentType: 'managed'
    internalLoadBalancerEnabled: false
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

resource bookApiContainerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: bookApiContainerName
  location: location
  properties: {
    kubeEnvironmentId: containerAppEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 80
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: [
        {
          server: containerRegistry.properties.loginServer
          username: acrUserName
          passwordSecretRef: 'container-registry-password'
        }
      ]
      secrets: [
        {
          name: 'container-registry-password'
          value: acrPassword
        }
        {
          name: 'cosmosdbconnectionstring'
          value: cosmosDBAccount.listConnectionStrings().connectionStrings[0].connectionString
        }
        {
          name: 'databasename'
          value: cosmosDatabase.name
        }
        {
          name: 'containername'
          value: cosmosContainer.name
        }
      ]
    }
    template: {
      containers: [
        {
          name: bookApiContainerName
          image: 'velidaacr.azurecr.io/bookstoreapi:109d5a42286730a3038d73925ad75326e2d517c6'
          resources: {
            cpu: '0.5'
            memory: '1Gi'
          }         
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

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
  parent: cosmosDBAccount
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-10-15' = {
  name: containerName
  parent: cosmosDatabase
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

resource apim 'Microsoft.ApiManagement/service@2021-08-01' = {
  name: apimName
  location: location
  sku: {
    capacity: 1
    name: 'Developer'
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
  }
}
