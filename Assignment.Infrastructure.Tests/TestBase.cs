using Assignment.Core;
using Assignment.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public abstract class TestBase : IDisposable
{
    private SqliteConnection _connection;
    private DbContextOptions<KanbanContext> _contextOptions;
    protected KanbanContext _context;

    public TestBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<KanbanContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        var context = new KanbanContext(_contextOptions);

        context.Database.EnsureCreated();

        var user = new User()
        {
            Name = "Frederik Raisa",
            Email = "frai@itu.dk"
        };

        var tagInUse = new Tag("InUse")
        {
            Id = 1
        };

        var tasks = new List<WorkItem>
        {
            new WorkItem("task 1") {
                Id = 10,
                Title = "task 1",
                AssignedTo = user,
                State = State.Active,
                Tags = new List<Tag> {tagInUse}
            }
        };

        tagInUse.WorkItems.Add(tasks[0]);

        context.Users.Add(user);
        context.Tags.AddRange(tagInUse);
        context.Items.AddRange(tasks);

        context.SaveChanges();

        _context = context;
    }

    public void Dispose() => _connection.Dispose();
}
