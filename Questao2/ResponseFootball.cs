namespace Questao2
{
	public class ResponseFootball
	{
        public int Page { get; set; }
		public int Per_page { get; set; }
        public int Total { get; set; }
        public int Total_pages { get; set; }
        public List<Match>? Data { get; set; }
    }
}
