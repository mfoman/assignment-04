namespace Assignment.Infrastructure;

public class Tag
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }
    public ICollection<WorkItem> WorkItems { get; set; }

    public Tag(string name)
    {
        Name = name;
        WorkItems = new HashSet<WorkItem>();
    }
}
