using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AzureStaticWeb.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AzureStaticWeb.ViewModels
{
    internal class QuotesViewModel : INotifyPropertyChanged
    {
        private const string EtagKey = "quotesEtag";
        private string _quotesFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "quotes.json");
        private readonly Random _randomizer = new Random(1337);
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
            private set => RaiseAndSetIfChanged(ref _quote, value);
        }
        public string Author
        {
            get => _author;
            private set => RaiseAndSetIfChanged(ref _author, value);
        }
        public bool IsBusy
        {
            get => _isBusy;
            private set => RaiseAndSetIfChanged(ref _isBusy, value);
        }

        private void RaiseAndSetIfChanged<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (currentValue != null && currentValue.Equals(newValue)) return;
            currentValue = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand NextQuoteCommand { get; }

        public async Task Init()
        {
            if (_quote != null) return;

            IsBusy = true;
            string quotesJson;
            using (var httpClient = new HttpClient())
            {
                if(!string.IsNullOrEmpty(CurrentEtagVersion)) httpClient.DefaultRequestHeaders.Add("If-None-Match", CurrentEtagVersion);
                var response = await httpClient.GetAsync("https://gnabberonlinestorage.blob.core.windows.net/alpha/quotes.json");

                quotesJson = response.StatusCode == HttpStatusCode.NotModified
                    ? ReadQuotesFromCache()
                    : await response.Content.ReadAsStringAsync();

                //quotesJson = await response.Content.ReadAsStringAsync();
                UpdateLocalCache(response.Headers.ETag, quotesJson);
            }
            _quotes = JsonConvert.DeserializeObject<List<QuoteInfo>>(quotesJson);

            PickAndSetQuote();
            IsBusy = false;
        }

        //public EntityTagHeaderValue CurrentEtagVersion => JsonConvert.DeserializeObject<EntityTagHeaderValue>(Preferences.Get(EtagKey, string.Empty));
        public string CurrentEtagVersion => Preferences.Get(EtagKey, string.Empty);

        private void UpdateLocalCache(EntityTagHeaderValue eTag, string quotesJson)
        {
            // Only update the cache if we need to
            if (eTag == null || CurrentEtagVersion == eTag.Tag) return;

            //Preferences.Set(EtagKey, JsonConvert.SerializeObject(eTag));
            Preferences.Set(EtagKey, eTag.Tag);
            File.WriteAllText(_quotesFilename, quotesJson);
        }


        private string ReadQuotesFromCache()
        {
            if (!File.Exists(_quotesFilename)) return string.Empty;

            return File.ReadAllText(_quotesFilename);
        }

        private void PickAndSetQuote()
        {
            var quote = _quotes[_randomizer.Next(_quotes.Count - 1)];
            Quote = quote.Quote;
            Author = quote.Name;
        }
    }
}
