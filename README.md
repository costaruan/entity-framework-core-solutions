# Entity Framework Core Solutions

A comprehensive collection of solutions, best practices, and examples for working with Entity Framework Core.

## EFCorePerformance

The Entity Framework Core is the preferred ORM for most .NET developers due to its excellent out-of-the-box performance, but we'll explore how to optimize database updates with EF and when SQL might be a better choice.

### Initial Setup

To test if we’re in the correct folder, run ``dotnet ef``. If we see an Entity Framework unicorn in ASCII art, we’re ready to go.

```console
dotnet ef
```

If you see an error “No executable found matching command dotnet-ef”, you may have to run a manual ``dotnet restore`` from the command line first, or explicitly run ``dotnet tool install --global dotnet-ef`` to install the command line tools.

We can now create an initial database migration. Let’s call it “InitialMigration”.

```console
dotnet ef migrations add InitialMigration
```

Applying the database migration and bringing the database up-to-date with our model.

```console
dotnet ef database update InitialMigration
```
