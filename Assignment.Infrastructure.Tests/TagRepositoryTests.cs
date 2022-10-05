using Assignment.Infrastructure;
using Assignment.Core;

namespace Assignment3.Entities.Tests;

public class TagRepositoryTests : TestBase
{
    private TagRepository _repo;

    public TagRepositoryTests()
    {
        // Given
        _repo = new TagRepository(_context);
    }

    // 1. Trying to update or delete a non-existing entity should return NotFound
    [Fact]
    public void update_delete_on_nonexisting_returns_NotFound()
    {
        // Update
        {
            var response = _repo.Update(new TagUpdateDTO(-1, "changed custom"));
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
        // Create
        {
            var (response, tagId) = _repo.Create(new TagCreateDTO("custom tag"));

            response.Should().Be(Response.Created);
            tagId.Should().Be(2);
        }

        // Read
        {
            var (id, name) = _repo.Find(2) ?? new TagDTO(0, "fake");
            id.Should().Be(2);
            name.Should().Be("custom tag");
        }

        // Update
        {
            var response = _repo.Update(new TagUpdateDTO(2, "changed custom"));
            response.Should().Be(Response.Updated);
        }

        // Find
        {
            var (id, name) = _repo.Find(2) ?? new TagDTO(0, "fake");
            id.Should().Be(2);
            name.Should().Be("changed custom");
        }


        // Delete
        {
            var response = _repo.Delete(2);
            response.Should().Be(Response.Deleted);
        }
    }

    // 5. If a task, tag, or user is not found, return null
    [Fact]
    public void NotFound_returns_null()
    {
        // Find
        _repo.Find(2).Should().BeNull();
    }

    /* --------------------------------- Unique --------------------------------- */

    // 1. Tags which are assigned to a task may only be deleted using the force.
    // [Fact]
    // public void tag_assigned_to_task_only_delete_using_force()
    // {
    //     var response = _repo.Delete(1, true);
    //     response.Should().Be(Response.Deleted);

    //     // When

    //     // Then
    // }

    // 2. Trying to delete a tag in use without the force should return Conflict
    [Fact]
    public void delete_tag_assigned_without_force_returns_conflict_unless_forced()
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

    // 3. Trying to create a tag which exists already should return Conflict
    [Fact]
    public void create_tag_already_exists_returns_conflict()
    {
        // Given
        {
            var (response, tagId) = _repo.Create(new TagCreateDTO("custom tag"));
        }

        // When
        {
            var (response, tagId) = _repo.Create(new TagCreateDTO("custom tag"));

            response.Should().Be(Response.Conflict);
        }
    }
}
