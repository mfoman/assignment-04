using Assignment.Core;
using Assignment.Infrastructure;

namespace Assignment3.Entities.Tests;

public class WorkItemRepositoryTests : TestBase
{
    private WorkItemRepository _repo;

    public WorkItemRepositoryTests()
    {
        // Given
        _repo = new WorkItemRepository(_context);
    }

    // 1. Trying to update or delete a non-existing entity should return NotFound
    [Fact]
    public void update_delete_on_nonexisting_returns_NotFound()
    {
        // Update
        {
            var response = _repo.Update(new WorkItemUpdateDTO(1, "title", 1, "desc", new List<String> { }, State.Active));
            response.Should().Be(Response.NotFound);
        }

        // Delete
        {
            var response = _repo.Delete(1);
            response.Should().Be(Response.NotFound);
        }
    }

    // 2. Create, Find, and Update should return a proper Response
    [Fact]
    public void create_read_update_should_return_response()
    {
        // Create
        var (createResponse, tagId) = _repo.Create(new WorkItemCreateDTO("unique title", 1, "desc", new List<string> { "Active" }));
        createResponse.Should().Be(Response.Created);

        // Read
        {
            var taskDetails = _repo.Find(tagId);
            taskDetails.Title.Should().Be("unique title");
        }

        // Update
        {
            var response = _repo.Update(new WorkItemUpdateDTO(tagId, "title updated", 1, "desc", new List<String> { }, State.New));
            response.Should().Be(Response.Updated);
        }

        // Find
        {
            var taskDetails = _repo.Find(tagId);
            taskDetails.Title.Should().Be("title updated");
        }


        // Delete
        {
            var response = _repo.Delete(tagId);
            response.Should().Be(Response.Deleted);
        }
    }

    // 5. If a task, tag, or user is not found, return null
    [Fact]
    public void NotFound_returns_null()
    {
        // Find
        _repo.Find(2).Id.Should().Be(-1);
    }

    /* --------------------------------- Unique --------------------------------- */

    // 1. Only tasks with the state New can be deleted from the database
    [Fact]
    public void only_new_WorkItems_can_be_deleted()
    {
        // Given
        var (_, id) = _repo.Create(new WorkItemCreateDTO("Delete me", null, null, new List<string> { }));

        var res = _repo.Delete(id);

        res.Should().Be(Response.Deleted);
    }

    // 2. Deleting a WorkItem which is Active should set its state to Removed
    [Fact]
    public void delete_active_WorkItem_set_to_removed()
    {
        // Given
        var (rep, id) = _repo.Create(new WorkItemCreateDTO("Delete me", null, null, new List<string> { }));
        _repo.Update(new WorkItemUpdateDTO(id, "updated", null, null, new List<string> { }, State.Active));

        // Because only removed
        var res = _repo.Delete(id);
        res.Should().Be(Response.Updated);
    }

    // 3. Deleting a WorkItem which is Resolved, Closed, or Removed should return Conflict
    [Fact]
    public void delete_WorkItem_resolved_closed_remoted_returns_conflict()
    {
        var (_, id) = _repo.Create(new WorkItemCreateDTO("Delete me", null, null, new List<string> { }));

        {
            // Resolved
            _repo.Update(new WorkItemUpdateDTO(id, "resolved", null, null, new List<string> { }, State.Resolved));
            var res = _repo.Delete(id);
            res.Should().Be(Response.Conflict);
        }

        {
            // Closed
            _repo.Update(new WorkItemUpdateDTO(id, "resolved", null, null, new List<string> { }, State.Closed));
            var res = _repo.Delete(id);
            res.Should().Be(Response.Conflict);
        }

        {
            // Removed
            _repo.Update(new WorkItemUpdateDTO(id, "resolved", null, null, new List<string> { }, State.Removed));
            var res = _repo.Delete(id);
            res.Should().Be(Response.Conflict);
        }
    }

    // 4. Creating a WorkItem will set its state to New and Created/StateUpdated to current time in UTC
    [Fact]
    public void create_WorkItem_set_new_and_created_updated_to_current_utc()
    {
        // Given
        var (response, id) = _repo.Create(new WorkItemCreateDTO("new WorkItem", null, null, new List<string> { }));
        var expected = DateTime.UtcNow;

        // When
        var WorkItem = _repo.Find(id);

        // Then
        WorkItem.Created.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(5));
        WorkItem.StateUpdated.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(5));
    }

    // 5. Create/update WorkItem must allow for editing tags
    [Fact]
    public void create_update_WorkItem_allow_editing_tags()
    {
        // Given
        var (response, id) = _repo.Create(new WorkItemCreateDTO("new WorkItem", null, null, new List<string> { "Active" }));
        var res = _repo.Update(new WorkItemUpdateDTO(id, "updated", null, "desc", new List<string> { "Dead" }, State.Active));

        res.Should().Be(Response.Updated);
    }

    // 6. Updating the State of a WorkItem will change the StateUpdated to current time in UTC
    [Fact]
    async public Task updating_state_change_updated_to_current_utc()
    {
        // Given
        var (response, id) = _repo.Create(new WorkItemCreateDTO("new WorkItem", null, null, new List<string> { }));

        await Task.Delay(2000);

        var expected = DateTime.UtcNow;
        var res = _repo.Update(new WorkItemUpdateDTO(id, "updated", null, "desc", new List<string> { }, State.Active));

        // When
        var WorkItem = _repo.Find(id);

        // Then
        WorkItem.StateUpdated.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(2));
    }

    // 7. Assigning a user which does not exist should return BadRequest
    [Fact]
    public void assign_nonexisting_user_returns_badrequest()
    {
        // Given
        var (response, id) = _repo.Create(new WorkItemCreateDTO("new WorkItem", -1, null, new List<string> { }));

        // Then
        response.Should().Be(Response.BadRequest);
    }
}
