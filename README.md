# Transla

Transla is a centralized localization service, for maintaining and providing dictionary items and their translations your services and applications.

### Goal

Is to provide easy setup and maximal flexibility to fit different needs for different projects. At the same time not forcing or coupling to use particular technology. 

### Requirements

Currently it is built with multitarget framework in mind, compatible with netcoreapp2.1 and netcoreapp2.2.

### Storage options

Currently only package with Redis storage implementation is available, but there are no specific constraints on what kind of storage must be used, as we have abstraction level in the middle.

### Installation of service

Before this step you must have (existing) or (create a new empty .net core api project).

Following packages installed from NuGet:

- Transla.Storage.Redis - Redis storage of your settings and dictionaries
- Transla.Service - providing with logic and  core setup of your Transla
- Transla.Contracts - contracts that are used for Transla API endpoints

During startup of your application it is only a matter of adding Transla dependencies through extension method on top of `IServicesCollection`:

```csharp
 services.AddTransla();
```

To configure what kind of storage to use, each of packages implements their own extension method that you have to call. F.x. Redis storage expects  you to pass Redis connection string and database id as parameters, it is up to you where to get them from:

```csharp
  services.AddTranslaRedisStorage(Configuration.GetConnectionString("RedisConnection"), int.Parse(Configuration["RedisStorageDatabaseId"]));
```

Last piece of code, once app dependencies and services are registered, during `Configure(IApplicationBuilder)` you need to bootstrap Transla, by executing next line:

```csharp
app.UseTransla("your-administration-api-key");
```

### Example of client setup in .NET Core

Before this step you must have (existing) or (create a new empty .net core project).

Following packages installed from NuGet:

- Transla.Client - Client library providing developers usage of dictionary service and communication between your app and Transla dictionary service
- Transla.Contracts - contracts that are used for Transla API endpoints

During startup of your application it is only a matter of adding Transla client dependencies through extension method on top of `IServicesCollection`, again it is totally up to you what parameter values to pass, and where they come from.

```csharp
 services.AddTranslaConfiguration(new TranslaConfiguration()
            {
                ApplicationAlias = Configuration["Transla:ApplicationAlias"],
                BaseAddress = Configuration["Transla:BaseUrl"],
                CacheExpirationInMinutes = int.Parse(Configuration["Transla:ExpirationInMinutes"]),
                DefaultCulture = Configuration["Transla:DefaultCultureName"],
            });
```

A little extra configuration is needed, Transla client has to know about current http context, to get culture information, using inbuilt .NET Core http context accessor:

```csharp
services.AddHttpContextAccessor();
```

Also, as you noticed during configuration of Transla it expects value for cache expiration in minutes, it is based on default abstractions of Net Core to inject caching service, for this example we use Memory Cache:

```csharp
services.AddMemoryCache();
```

Thats it! :) 

#### How to get dictionary value

In your constructor you can inject `IDictionaryService`, that provides your with methods for getting value for specific or automatically discovered culture.

#### How automatic culture discovery 

It looks at query string parameter named `culture` as a priority, if not present then it looks at header named `Culture` if not present either then falls back to default culture defined during startup of project.

### API endpoints

![alt endpoints](https://i.ibb.co/BqdPxvg/screencapture-dictionary-goshop-staging-novicell-dk-swagger-index-html-2019-03-07-10-39-17.png)
