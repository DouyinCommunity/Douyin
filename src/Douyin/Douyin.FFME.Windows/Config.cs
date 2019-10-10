using System;
using System.Collections.Generic;
using System.Text;

namespace Douyin
{
    public static class Config
    {
        public static string UserAgent = "Aweme 3.1.0 rv:31006 (iPhone; iOS 12.0; zh_CN) Cronet";
        public static string UserAgent2 = "Mozilla/5.0 (Linux; Android 8.0.0; MI 6 Build/OPR1.170623.027; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/62.0.3202.84 Mobile Safari/537.36";

        public static string HotSearchUrl = "https://api.amemv.com/aweme/v1/hot/search/list/";

        public static string HotVideoUrl = "https://api.amemv.com/aweme/v1/hotsearch/aweme/billboard/";

        public static string HotEnergyUrl = "https://api.amemv.com/aweme/v1/hotsearch/positive_energy/billboard/";

        public static string HotMusicUrl = "https://api.amemv.com/aweme/v1/hotsearch/music/billboard/";

        public static string HotTrendUrl = "https://api.amemv.com/aweme/v1/category/list/";

        public static string Topic2VideoUrl = "https://api.amemv.com/aweme/v1/challenge/aweme/";

        public static string Music2VideoUrl = "https://api.amemv.com/aweme/v1/music/aweme/";
    }
}
