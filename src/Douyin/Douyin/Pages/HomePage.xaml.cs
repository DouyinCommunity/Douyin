using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Forms.Shared;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Douyin.Pages
{
    public class HomePage : UserControl
    {
        private static readonly HttpClient client = new HttpClient();
        private const string VIDEO_URL = "rtsp://184.72.239.149/vod/mp4:BigBuckBunny_175k.mov";
        private readonly LibVLC _libvlc;


        public class StateData
        {
            public string Name { get; private set; }
            public string Abbreviation { get; private set; }
            public string Capital { get; private set; }

            public StateData(string name, string abbreviatoin, string capital)
            {
                Name = name;
                Abbreviation = abbreviatoin;
                Capital = capital;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private Aweme BuildAllAwemes()
        {
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", Config.UserAgent2);
                HttpResponseMessage response =client.GetAsync(Config.HotEnergyUrl).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                Aweme aweme = Aweme.FromJson(responseBody);
                return aweme;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }
        public Aweme Awemes { get; private set; }
        
        public HomePage()
        {
            this.InitializeComponent();

            Awemes = BuildAllAwemes();



    }

    private IEnumerable<AutoCompleteBox> GetAllAutoCompleteBox()
        {
            return
                this.GetLogicalDescendants()
                    .OfType<AutoCompleteBox>();
        }

        private bool StringContains(string str, string query)
        {
            return str.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private async Task<IEnumerable<object>> PopulateAsync(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1.5), cancellationToken);

            //return
            //    Awemes.Where(data => StringContains(data.AwemeList, searchText) || StringContains(data.Capital, searchText))
            //          .ToList();
            return null;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
