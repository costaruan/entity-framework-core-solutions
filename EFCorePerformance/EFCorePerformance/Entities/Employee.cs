namespace EFCorePerformance.Entities;

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Salary { get; set; } = 0.0m;

    public int CompanyId { get; set; }
}