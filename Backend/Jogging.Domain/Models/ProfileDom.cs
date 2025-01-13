namespace Jogging.Domain.Models;

public class ProfileDom
{
    public string Id { get; set; }
    public string Role { get; set; }

    public int? PersonId { get; set; }

    public PersonDom Person { get; set; }
}