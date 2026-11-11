using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;

namespace Konekto.API.Validation;

/// <summary>
/// Shared JobPosting rules for create and update operations.
/// </summary>
public static class JobPostingValidation
{
    public static async Task<string?> ValidateAsync(KonektoDbContext db, string? title, int? companyId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "Başlık (Title) alanı zorunludur.";

        if (title.Length > 120)
            return "Başlık (Title) en fazla 120 karakter olabilir.";

        if (!companyId.HasValue || companyId.Value <= 0)
            return "Şirket (CompanyId) alanı zorunludur.";

        if (!await db.Companies.AnyAsync(c => c.Id == companyId.Value))
            return "Belirtilen şirket (CompanyId) bulunamadı.";

        return null;
    }
}
