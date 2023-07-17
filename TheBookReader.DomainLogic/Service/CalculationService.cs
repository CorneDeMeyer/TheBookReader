﻿namespace TheBookReader.DomainLogic.Service
{
    public class CalculationService
    {
        private string _content;

        public CalculationService(string content) => _content = content;

        public CalculationService RemoveBadCharacters()
        {
            // Remove Junk Symbols that will skew result
            _content = _content
                .Replace(".", string.Empty)
                .Replace(",", string.Empty)
                .Replace("?", string.Empty)
                .Replace("!", string.Empty)
                .Replace("@", string.Empty)
                .Replace("#", string.Empty)
                .Replace("$", string.Empty)
                .Replace("%", string.Empty)
                .Replace("^", string.Empty)
                .Replace("&", string.Empty)
                .Replace("*", string.Empty)
                .Replace("`", string.Empty)
                .Replace("~", string.Empty)
                .Replace("/", string.Empty)
                .Replace("\r\n", string.Empty)
                .Replace("\"", string.Empty);

            // Excluding hyphen words

            return this; 
        }

        public Dictionary<string, int> CalculateWordUsage(int WordMinLength, int SelectTopRows)
        {
            var usageDictionary = new Dictionary<string, int>();
            RemoveBadCharacters();

            // Split Sentences on spaces
            var contentWords = _content.Split(new char[0]);

            // Ensure we have atleast one word
            if (contentWords.Length > 0)
            {
                // To place minimum word lenght when specified
                if (WordMinLength > 1)
                {
                    contentWords = contentWords.Where(word => word.Length >= WordMinLength).ToArray();
                }

                // Might be a better and way of counting these words, but distinct and then do count
                foreach (var word in contentWords.Select(wrd => wrd.ToLower().Trim()).Distinct())
                {
                    var count = contentWords.Where(wordSearch =>
                                                wordSearch.ToLower().Equals(word, StringComparison.CurrentCultureIgnoreCase)).Count();
                    var nthValue = usageDictionary.Count() > SelectTopRows
                                    ? usageDictionary.OrderByDescending(x => x.Value).ElementAt(SelectTopRows)
                                    : new KeyValuePair<string, int>("None", 0);
                    if (count > nthValue.Value)
                    {
                        usageDictionary.Add(word, count);
                    }
                }
            }
            
            // Simplist and best take the TOP X rows
            return usageDictionary
                .OrderByDescending(word => word.Value)
                .ThenBy(word => word.Key)
                .Take(SelectTopRows)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
