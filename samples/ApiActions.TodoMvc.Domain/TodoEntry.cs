using System.ComponentModel.DataAnnotations;

namespace ApiActions.TodoMvc.Domain
{
    public class TodoEntry
    {
        public int Id { get; set; }
        public int? Order { get; set; }

        [Required]
        [MaxLength(128)]
        public string Title { get; set; }
        public bool Completed { get; set; }

        public string Url { get; set; }
    }
}
