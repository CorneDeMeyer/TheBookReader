namespace TheBookReader.Domain.Models
{
    // Generic Result Model that uses Generics
    public class ResultModel<IFilterBase, IResult> where IFilterBase : SearchFilterBase
    {
        public string? BookTitle { get; set; }
        public int NumberOfPages { get; set; }
        public TimeSpan Duration { get; set; }
        public IFilterBase? Filter { get; set; }
        public IResult? ResultSet { get; set; }
        public string[]? Errors { get; set; }
    }
}
