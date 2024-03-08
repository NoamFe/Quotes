using QuotesApp;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Json;
using System.Xml;

namespace Quotes.Repository
{

    public interface IQuotesRepository
    {
        bool Init();
        int Count();
        bool Update(Quote quote);
        Quote Get(int id);
        bool Delete(Quote quote);
        Quote Add(Quote quote);
        int GetNumberOfPairQuotes(int maxLengthCombined);
    }

    public class QuotesRepository : IQuotesRepository
    {
        List<Quote> _inMemoryDB = new List<Quote>();
        SortedDictionary<int,int> _quoteSizeDB = new ();
        Dictionary<string, int> unique = new();

        public QuotesRepository()
        {
            using (var sr = new StreamReader(@"Resources/ShortDb.json"))
            {
                var text =  sr.ReadToEnd();

                var loadedDB = JsonSerializer.Deserialize<List<Quote>>(text);
               
                foreach (var item in loadedDB)
                {
                    Add(item);
                }
            }
        }


        public int GetNumberOfPairQuotes(int maxLengthCombined)
        { 
            var list = new List<int>();

            foreach (var item in _quoteSizeDB.Where(e => e.Key < maxLengthCombined)
                .Select(q => new { q.Key, q.Value }))
            {
                for (int i = 0; i < item.Value; i++)
                {
                    list.Add(item.Key);
                } 
            }


            var count = 0;
            if (list.Count < 2)
                return count;
             
            bool shouldContinue = true;
            int leftIndex = 0;
            int rightIndex = 1;

            while (shouldContinue)
            {
                if (rightIndex < list.Count)
                {
                    var left = list[leftIndex];
                    var right = list[rightIndex];
                    var total = left+right;
                    if (total <= maxLengthCombined)
                    {
                        count++;
                        rightIndex++;
                    }
                    else 
                    {
                        leftIndex++;
                        rightIndex = leftIndex + 1;
                    }
                }
                if (leftIndex == list.Count)
                    shouldContinue = false;
                if (rightIndex == list.Count)
                {
                    leftIndex++;
                    rightIndex = leftIndex + 1;
                } 
            }
           
            return count;
        }

        private void AddTextSizeDictionary(Quote item)
        {
            var length = item.Text.Length;

            if (_quoteSizeDB.Keys.Contains(length))
            {
                _quoteSizeDB[length] = 1 + _quoteSizeDB[length];
            }
            else
                _quoteSizeDB.Add(length, 1);
        }

        private void RemoveTextSizeDictionary(Quote item)
        {
            var length = item.Text.Length;

            _quoteSizeDB[length] = _quoteSizeDB[length] - 1;
        }


        public Quote Get(int id) => _inMemoryDB.FirstOrDefault(q=>q.Id == id); 
        
        public Quote Add(Quote quote)
        {
            quote.Id = _inMemoryDB.Count == 0 ? 1 : _inMemoryDB.Last().Id + 1;

            if (unique.ContainsKey(quote.Text))
            { 
                //text already persisted
            }
            else 
            {
                unique.Add(quote.Text,1);

                _inMemoryDB.Add(quote);

                AddTextSizeDictionary(quote);
            } 

            return quote;
        }

        public bool Delete(Quote quote)
        {
            if (_inMemoryDB.Remove(quote))
            {
                RemoveTextSizeDictionary(quote);
                if (unique.ContainsKey(quote.Text))
                {
                    unique.Remove(quote.Text);
                }

                return true;
            }

            return false;
        }
        public bool Update(Quote quote)
        {    
            var current = _inMemoryDB.FirstOrDefault(q => q.Id == quote.Id);

            if (current == null)
                return false;

            if (unique.ContainsKey(quote.Text))
            {
                return false;
            }

            int index = _inMemoryDB.FindIndex(q => q.Id == quote.Id);

            if (index != -1)
            {
                _inMemoryDB[index] = quote;

                unique.Remove(current.Text);
                unique.Add(quote.Text,1);
                RemoveTextSizeDictionary(current);
                AddTextSizeDictionary(quote);
                return true;
            }
              
            return false;
        }

        public int Count() => _inMemoryDB.Count;

        public bool Init() => true;
    }
}
