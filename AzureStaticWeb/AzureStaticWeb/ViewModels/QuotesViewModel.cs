using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;

namespace AzureStaticWeb.ViewModels
{
    internal class QuotesViewModel : ReactiveObject
    {
        private Random _randomizer = new Random(1337);
        private IList<QuoteInfo> _quotes;
        private string _quote;
        private string _author;
        private bool _isBusy;

        public QuotesViewModel()
        {
            NextQuoteCommand = ReactiveCommand.Create(PickAndSetQuote);
        }

        public string Quote
        {
            get => _quote;
            private set
            {
                this.RaiseAndSetIfChanged(ref _quote, value);
            }
        }
        public string Author
        {
            get => _author;
            private set
            {
                this.RaiseAndSetIfChanged(ref _author, value);
            }
        }
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                this.RaiseAndSetIfChanged(ref _isBusy, value);
            }
        }
        public ReactiveCommand<Unit, Unit> NextQuoteCommand { get; }

        public async Task Init()
        {
            if (_quote != null) return;
            IsBusy = true;
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://gnabberstaticweb.z6.web.core.windows.net/quotes.json");
                var quotesJson = await response.Content.ReadAsStringAsync();
                _quotes = JsonConvert.DeserializeObject<List<QuoteInfo>>(quotesJson);

                PickAndSetQuote();
            }
            IsBusy = false;
        }

        private void PickAndSetQuote()
        {
            var quote = _quotes[_randomizer.Next(_quotes.Count - 1)];
            Quote = quote.Quote;
            Author = quote.Name;
        }
    }
    public class QuoteInfo
    {
        public string Quote { get; set; }
        public string Name { get; set; }
    }

}
