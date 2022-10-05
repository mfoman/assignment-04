using Assignment.Core;
using Assignment.Infrastructure;

namespace Assignment3.Entities.Tests;

public class UserRepositoryTests : TestBase
{
    private UserRepository _repo;

    public UserRepositoryTests()
    {
        // Given
        _repo = new UserRepository(_context);
    }

    // 1. Trying to update or delete a non-existing entity should return NotFound
    [Fact]
    public void update_delete_on_nonexisting_returns_NotFound()
    {
        // Update
        {
            var response = _repo.Update(new UserUpdateDTO(-1, "asge", "frai@itu.dk"));
            response.Should().Be(Response.NotFound);
        }

        // Delete
        {
            var response = _repo.Delete(-1);
            response.Should().Be(Response.NotFound);
        }
    }

    // 2. Create, Read, and Update should return a proper Response
    [Fact]
    public void create_read_update_should_return_response()
    {
        var (responseCreated, tagId) = _repo.Create(new UserCreateDTO("frederik", "asger@itu.dk"));
        responseCreated.Should().Be(Response.Created);

        // Read
        {
            var userDetails = _repo.Find(tagId);
            userDetails.Name.Should().Be("frederik");
        }

        // Update
        {
            var response = _repo.Update(new UserUpdateDTO(tagId, "Asger", "asger@itu.dk"));
            response.Should().Be(Response.Updated);
        }

        // Read
        {
            var userDetails = _repo.Find(tagId);
            userDetails.Name.Should().Be("Asger");
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
        // Read
        _repo.Find(-1).Id.Should().Be(-1);
    }

    /* --------------------------------- Unique --------------------------------- */

    // 1. Users who are assigned to a task may only be deleted using the force
    // [Fact]
    // public void users_assigned_to_task_only_delete_with_force()
    // {
    //     var response = _repo.Delete(2, true);

    //     response.Should().Be(Response.Deleted);
    // }

    // 2. Trying to delete a user in use without the force should return Conflict
    [Fact]
    public void delete_active_user_without_force_returns_conflict()
    {
        {
            var response = _repo.Delete(1);

            response.Should().Be(Response.Conflict);
        }

        {
            var response = _repo.Delete(1, true);

            response.Should().Be(Response.Deleted);
        }
    }

    // 3. Trying to create a user which exists already (same email) should return Conflict.
    [Fact]
    public void create_user_that_exists_email_returns_conflict()
    {
        // Given
        _repo.Create(new UserCreateDTO("Lundsteen", "lundsteen@itu.dk"));
        var (res, id) = _repo.Create(new UserCreateDTO("Lundsteen", "lundsteen@itu.dk"));

        res.Should().Be(Response.Conflict);
    }
}
