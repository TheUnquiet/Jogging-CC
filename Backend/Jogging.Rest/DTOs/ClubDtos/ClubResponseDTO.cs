using Jogging.Rest.DTOs.PersonDtos;

namespace Jogging.Rest.DTOs.ClubDtos {
    public class ClubResponseDTO {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }

        public List<PersonResponseDTO>? Members { get; set; }
    }
}
