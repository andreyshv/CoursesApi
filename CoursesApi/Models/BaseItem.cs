using System.ComponentModel.DataAnnotations;

namespace CoursesApi.Models
{
    public abstract class BaseItem
    {
        public long Id { get; set; }

        [Timestamp]
        public byte[] Version { get; set; } = null!;
    }

    public abstract class BaseItemDTO
    {
        public long Id { get; set; }
        
        public byte[] Version { get; set; } = null!;
    }
}
