namespace Assignment.Infrastructure;

public class UserRepository : IUserRepository
{
    private KanbanContext _context;

    public UserRepository(KanbanContext context)
    {
        this._context = context;
    }

    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = _context.Users.FirstOrDefault(c => c.Name == user.Name);
        Response status;

        if (entity is null)
        {
            entity = new User
            {
                Name = user.Name,
                Email = user.Email,
            };

            _context.Users.Add(entity);
            _context.SaveChanges();

            status = Created;
        }
        else
        {
            status = Conflict;
        }

        var created = new UserDTO(
            entity.Id,
            entity.Name ?? "",
            entity.Email ?? ""
        );

        return (status, created.Id);
    }

    public Response Delete(int userId, bool force = false)
    {
        var user = _context.Users.FirstOrDefault(c => c.Id == userId);
        Response response;

        if (user is null)
        {
            response = NotFound;
        }
        else
        {
            var activeTasks = from c in user.Items
                              where (new[] { State.Active, State.New }).Contains(c.State)
                              select c;

            if (activeTasks.Count() != 0 && !force)
            {
                response = Conflict;
            }
            else
            {
                _context.Users.Remove(user);
                _context.SaveChanges();

                response = Deleted;
            }
        }

        return response;
    }

    public UserDTO Find(int userId)
    {
        var users = from c in _context.Users
                    where c.Id == userId
                    select new UserDTO(c.Id, c.Name ?? "", c.Email ?? "");

        return users.FirstOrDefault() ?? new UserDTO(-1, "", "");
    }

    public IReadOnlyCollection<UserDTO> Read()
    {
        var users = from c in _context.Users
                    orderby c.Name
                    select new UserDTO(c.Id, c.Name ?? "", c.Email ?? "");

        return users.ToArray();
    }

    public Response Update(UserUpdateDTO user)
    {
        var entity = _context.Users.Find(user.Id);

        Response response;

        if (entity is null)
        {
            response = NotFound;
        }
        else if (_context.Users.FirstOrDefault(c => c.Id != user.Id && c.Name == user.Name) != null)
        {
            response = Conflict;
        }
        else
        {
            entity.Name = user.Name;
            entity.Email = user.Email;
            _context.SaveChanges();
            response = Updated;
        }

        return response;
    }
}
