using Konekto.Domain.Entities;

namespace Konekto.Business.Factories;

public static class CompanyFactory
{
    public static Company Create(string name, string industry, string website, string location)
    {
        return new Company
        {
            Name = name,
            Industry = industry,
            Website = website,
            Location = location
        };
    }

    public static List<Company> GetPredefinedCompanies()
    {
        return new List<Company>
        {
            Create("Google", "Technology", "https://about.google", "Mountain View, CA / Istanbul"),
            Create("Microsoft", "Technology", "https://microsoft.com", "Redmond, WA / Istanbul"),
            Create("Trendyol", "E-Commerce", "https://trendyol.com", "Istanbul, Turkey"),
            Create("Getir", "Delivery & Tech", "https://getir.com", "Istanbul, Turkey")
        };
    }
}

