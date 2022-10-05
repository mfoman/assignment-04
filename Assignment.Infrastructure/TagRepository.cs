namespace Assignment.Infrastructure;

public class TagRepository : ITagRepository
{
    private KanbanContext context;

    public TagRepository(KanbanContext context)
    {
        this.context = context;
    }

    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        throw new NotImplementedException();
    }

    public Response Delete(int tagId, bool force = false)
    {
        throw new NotImplementedException();
    }

    public TagDTO Find(int tagId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TagDTO> Read()
    {
        throw new NotImplementedException();
    }

    public Response Update(TagUpdateDTO tag)
    {
        throw new NotImplementedException();
    }
}
