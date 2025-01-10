using Jogging.Rest.DTOs.PersonDtos;

namespace Jogging.Rest.DTOs.ClubDtos {
    public class ClubRequestDTO {
        public string Name { get; set; }
        public string? Logo { get; set; }
        public List<int>? MemberIds { get; set; }
    }
}
