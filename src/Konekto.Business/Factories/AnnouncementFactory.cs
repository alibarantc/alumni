using Konekto.Domain.Entities;

namespace Konekto.Business.Factories;

public static class AnnouncementFactory
{
    public static Announcement Create(string title, string content, string category = "Genel", string author = "Rektörlük / Mezunlar Ofisi")
    {
        return new Announcement
        {
            Title = title,
            Content = content,
            Category = category,
            Author = author,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static List<Announcement> GetPredefinedAnnouncements()
    {
        return new List<Announcement>
        {
            Create(
                "2026 Mezunlar Buluşması Tarihleri Belli Oldu!",
                "Değerli mezunlarımız, geleneksel Konekto Mezunlar Günü bu yıl 15 Kasım Cumartesi günü merkez kampüste gerçekleştirilecektir. Tüm mezun ve öğrencilerimiz davetlidir.",
                "Etkinlik",
                "Mezunlar İletişim Ofisi"
            ),
            Create(
                "Yazılım ve Teknoloji Zirvesi Kayıtları Başladı",
                "Sektör liderleri ve mezunlarımızın konuşmacı olarak katılacağı Kariyer Zirvesi 2026 için başvurular açılmıştır. Katılım ücretsizdir.",
                "Kariyer",
                "Kariyer Geliştirme Merkezi"
            ),
            Create(
                "Öğrenci-Mezun Mentorluk Programı 4. Dönem Başvuruları",
                "Son sınıf öğrencilerimiz ile tecrübeli mezunlarımızı bir araya getiren mentorluk programı için mentor ve mentee kayıtları başladı.",
                "Akademik",
                "Mentorluk Koordinatörlüğü"
            )
        };
    }
}

