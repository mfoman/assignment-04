using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.Infrastructure;

public class WorkItem
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Title { get; set; }

    public int? AssignedToId { get; set; }

    public User? AssignedTo { get; set; }

    public string? Description { get; set; }

    [Required]
    public State State { get; set; }

    public ICollection<Tag> Tags { get; set; }

    public WorkItem(string title)
    {
        Title = title;
        Tags = new HashSet<Tag>();
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedDate { get; set; }
}
