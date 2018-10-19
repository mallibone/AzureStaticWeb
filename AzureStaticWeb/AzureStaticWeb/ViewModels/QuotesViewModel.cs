using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AzureStaticWeb.ViewModels
{
    internal class QuotesViewModel : INotifyPropertyChanged
    {
        private Random _randomizer = new Random(1337);
        private IList<QuoteInfo> _quotes;
        private string _quote;
        private string _author;
        private bool _isBusy;

        public QuotesViewModel()
        {
            NextQuoteCommand = new Command(PickAndSetQuote);
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

        private void RaiseAndSetIfChanged<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (currentValue != null && currentValue.Equals(newValue)) return;
            currentValue = newValue;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand NextQuoteCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task Init()
        {
            if (_quote != null) return;
            IsBusy = true;
            string quotesJson;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("https://gnabberonlinestorage.blob.core.windows.net/alpha/quotes.json");
                    quotesJson = await response.Content.ReadAsStringAsync();
                }
                _quotes = JsonConvert.DeserializeObject<List<QuoteInfo>>(quotesJson);
            }
            catch
            {
                _quotes = new List<QuoteInfo> { new QuoteInfo { Name = "Life is 10% what happens to me and 90% of how I react to it.", Quote = "Charles Swindoll" } };
            }

            PickAndSetQuote();
            IsBusy = false;
        }

        private void PickAndSetQuote()
        {
            if (_quotes == null || !_quotes.Any()) return;
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
