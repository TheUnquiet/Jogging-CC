using System.Collections.Generic;

namespace Jogging.Domain.Models {
    public class ClubDom {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }

        public List<PersonDom>? Members { get; set; }
    }
}
