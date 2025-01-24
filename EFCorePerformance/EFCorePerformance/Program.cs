using Dapper;
using EFCorePerformance;
using EFCorePerformance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPut("increase-salaries", async (int companyId, DatabaseContext context) =>
{
    var company = await context
        .Set<Company>()
        .Include(c => c.Employees)
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company == null)
    {
        return Results.NotFound($"Company with id {companyId} not found");
    }

    foreach (var employee in company.Employees)
    {
        employee.Salary *= 1.1m;
    }

    company.LastSalaryUpdate = DateTime.UtcNow;

    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("increase-salaries-sql", async (int companyId, DatabaseContext context) =>
{
    var company = await context
        .Set<Company>()
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company == null)
    {
        return Results.NotFound($"Company with id {companyId} not found");
    }

    // !IMPORTANT
    await context.Database.BeginTransactionAsync();

    await context.Database.ExecuteSqlInterpolatedAsync(
        $"UPDATE Employees SET Salary = Salary * 1.1 WHERE CompanyId = {company.Id}");

    company.LastSalaryUpdate = DateTime.UtcNow;

    await context.SaveChangesAsync();

    // !IMPORTANT
    await context.Database.CommitTransactionAsync();

    return Results.NoContent();
});

app.MapPut("increase-salaries-sql-dapper", async (int companyId, DatabaseContext context) =>
{
    var company = await context
        .Set<Company>()
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company == null)
    {
        return Results.NotFound($"Company with id {companyId} not found");
    }

    // !IMPORTANT
    var transaction = await context.Database.BeginTransactionAsync();

    await context.Database.GetDbConnection().ExecuteAsync(
        "UPDATE Employees SET Salary = Salary * 1.1 WHERE CompanyId = @CompanyId",
        new { CompanyId = company.Id },
        transaction.GetDbTransaction());

    company.LastSalaryUpdate = DateTime.UtcNow;

    await context.SaveChangesAsync();

    // !IMPORTANT
    await context.Database.CommitTransactionAsync();

    return Results.NoContent();
});

app.Run();