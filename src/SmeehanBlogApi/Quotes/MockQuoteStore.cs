using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Quotes
{
    public class MockQuoteStore : IQuoteStore
    {
        public Task AddQuoteAsync(Quote quote)
        {
            _data.Add(quote);
            return Task.FromResult(true);
        }

        public Task BatchStoreAsync(IEnumerable<Quote> quotes)
        {
            _data.AddRange(quotes);
            return Task.FromResult(true);
        }

        public Task<Quote> GetItem(int id)
        {
            return Task.FromResult(_data.Where<Quote>(q => q.Id == id).SingleOrDefault());
        }

        public Task<IEnumerable<Quote>> BatchGetAsync(IEnumerable<int> ids)
        {
            var quotes = new List<Quote>();

            foreach (var id in ids)
            {
                var quote = _data.Where(q => q.Id == id).SingleOrDefault();
                if(quote != null)
                {
                    quotes.Add(quote);
                }
            }

            return Task.FromResult<IEnumerable<Quote>>(quotes);
        }

        public Task ModifyQuoteAsync(Quote quote)
        {
            var existingQuote = _data.Where(q => q.Id == quote.Id).SingleOrDefault();
            if(existingQuote == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _data.Remove(existingQuote);
            _data.Add(quote);

            return Task.FromResult(true);
        }

        public Task DeleteQuoteAsync(Quote quote)
        {
            var existingQuote = _data.Where(q => q.Id == quote.Id).SingleOrDefault();
            if (existingQuote == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _data.Remove(existingQuote);

            return Task.FromResult(true);
        }

        public Task<IEnumerable<Quote>> GetRandomQuotesAsync(int numberToGet, int beginingId = 1001)
        {
            if(numberToGet > _data.Count)
            {
                throw new AmazonDynamoDBException("You are trying to get more items than in the data set");
            }

            var random = new Random();
            var quotes = new List<Quote>();

            while (numberToGet > 0)
            {
                var randomQuoteId = random.Next(beginingId, _data.Last().Id);
                var quote = _data.Where(q => q.Id == randomQuoteId).SingleOrDefault();

                if(quote != null && !quotes.Any(q=>q.Id == quote.Id))
                {
                    quotes.Add(quote);
                    numberToGet--;
                }
            }

            return Task.FromResult<IEnumerable<Quote>>(quotes);
        }

        private List<Quote> _data = new List<Quote>() 
        {
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Fry",
                        Words = "Shut up and take my money!",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Eye Phone"
                },
                Id = 1001
            },
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Bender",
                        Words = "Bite my shiny metal ass.",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Every Episode"
                },
                Id = 1002
            },
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Professor Farnsworth",
                        Words = "Good News everyone.",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Every Episode"
                },
                Id = 1003
            },
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Dr. Zoidberg",
                        Words = "Fry, It's Been Years Since Medical School, So Remind Me. Disemboweling In Your Species: Fatal Or Non-Fatal?",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Mating"
                },
                Id = 1004
            },
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Amy Wong",
                        Words = "Finally, A Uniform I'd Be Happy To Be Caught Dead In!",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Every Episode"
                },
                Id = 1005
            },
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Hermes Conrad",
                        Words = "If You Ask Me, It's Mighty Suspicious. I'm Gonna Call The Police. Right After I Flush Some Things.",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Every Episode"
                },
                Id = 1006
            },
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Zap Brannigan",
                        Words = "I Got Your Distress Call And Came Here As Soon As I Wanted To.",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Every Episode"
                },
                Id = 1007
            },
            new Quote()
            {
                Speakers = new List<Speaker>
                {
                    new Speaker()
                    {
                        Person = "Scruffy",
                        Words = "I'm scruffy, the janitor.",
                        Order = 0
                    }
                },
                Source = new Source()
                {
                    Series = "Futurama",
                    Story = "Every Episode"
                },
                Id = 1008
            }
        };
    }
}
