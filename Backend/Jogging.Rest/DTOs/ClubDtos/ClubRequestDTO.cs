using Jogging.Rest.DTOs.PersonDtos;
using Microsoft.AspNetCore.Http;

namespace Jogging.Rest.DTOs.ClubDtos {
    public class ClubRequestDTO {
        public string Name { get; set; }
        public IFormFile Logo { get; set; }
        public List<int>? MemberIds { get; set; }
    }
}
