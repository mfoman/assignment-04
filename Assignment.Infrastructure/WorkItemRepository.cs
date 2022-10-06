namespace Assignment.Infrastructure;

public class WorkItemRepository : IWorkItemRepository
{
    private KanbanContext _context;

    public WorkItemRepository(KanbanContext context)
    {
        this._context = context;
    }

    public (Response Response, int ItemId) Create(WorkItemCreateDTO item)
    {
        var entity = _context.Items.FirstOrDefault(c => c.Title == item.Title);
        var user = _context.Users.Find(item.AssignedToId);

        Response status;

        if (user is null && item.AssignedToId != null)
        {
            entity = new WorkItem(item.Title)
            {
                AssignedTo = null,
                State = State.Removed,
                Tags = new List<Tag> { }
            };

            status = BadRequest;
        }
        else if (entity is null)
        {
            var tags = from c in _context.Tags
                       where item.Tags.Contains(c.Name)
                       select c;

            entity = new WorkItem(item.Title)
            {
                AssignedTo = user,
                Description = item.Description,
                State = State.New,
                Tags = tags.ToList()
            };

            _context.Items.Add(entity);
            _context.SaveChanges();

            status = Created;
        }
        else
        {
            status = Conflict;
        }

        var created = new WorkItemDTO(
            entity.Id,
            entity.Title ?? "",
            entity.AssignedTo?.Name ?? "",
            entity.Tags as IReadOnlyCollection<string> ?? new List<string> { },
            entity.State
        );

        return (status, created.Id);
    }

    public Response Delete(int itemId)
    {
        var task = _context.Items.FirstOrDefault(c => c.Id == itemId);
        Response response;

        if (task is null)
        {
            response = NotFound;
        }
        else if ((new[] { State.Closed, State.Removed, State.Resolved }).Contains(task.State))
        {
            response = Conflict;
        }
        else if (task.State == State.Active)
        {
            task.State = State.Removed;
            _context.Items.Update(task);

            response = Updated;
        }
        else
        {
            _context.Items.Remove(task);
            _context.SaveChanges();

            response = Deleted;
        }

        return response;
    }

    public WorkItemDetailsDTO Find(int itemId)
    {
        var tasks = from c in _context.Items
                    where c.Id == itemId
                    select c;

        var t = tasks.FirstOrDefault();

        if (t is null)
        {
            return new WorkItemDetailsDTO(-1, "", "", DateTime.Now, "", new List<string> { }, State.Closed, DateTime.Now);
        }

        return new WorkItemDetailsDTO(
            t.Id,
            t.Title ?? "",
            t.Description ?? "",
            t.CreatedDate,
            t.AssignedTo?.Name ?? "",
            t.Tags as IReadOnlyCollection<string> ?? new List<string> { },
            t.State,
            t.UpdatedDate ?? DateTime.UtcNow
        );
    }

    public IReadOnlyCollection<WorkItemDTO> Read()
    {
        var Tasks = from c in _context.Items
                    orderby c.Title
                    select new WorkItemDTO(
                        c.Id,
                        c.Title ?? "",
                        (c.AssignedTo == null ? "" : c.AssignedTo.Name ?? ""),
                        c.Tags as IReadOnlyCollection<string> ?? new List<string> { },
                        c.State
                    );

        return Tasks.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByState(State state)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByTag(string tag)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByUser(int userId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadRemoved()
    {
        throw new NotImplementedException();
    }

    public Response Update(WorkItemUpdateDTO item)
    {
        var entity = _context.Items.Find(item.Id);

        Response response;

        if (entity is null)
        {
            response = NotFound;
        }
        else if (_context.Items.FirstOrDefault(c => c.Id != item.Id && c.Title == item.Title) != null)
        {
            response = Conflict;
        }
        else
        {
            entity.Title = item.Title;
            entity.Description = item.Description;
            entity.State = item.State;
            entity.Tags = item.Tags as List<Tag> ?? new List<Tag> { };

            if (entity.AssignedTo?.Id != item.AssignedToId)
            {
                var user = _context.Users.Find(item.AssignedToId);
                entity.AssignedTo = user;
            }

            _context.SaveChanges();
            response = Updated;
        }

        return response;
    }
}
