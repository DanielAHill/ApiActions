namespace ApiActions.TodoMvc.Domain
{
    public class TodoEntry
    {
        public int Id { get; set; }
        public int? Order { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
        public string Url { get; set; }
    }
}
