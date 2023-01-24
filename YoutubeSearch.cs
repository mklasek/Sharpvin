using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;


internal class YouTubeSearcher
{
    private const string API_key = "AIzaSyC98V5xKK5ba027fvhue02zrc7ctPTyFPc";

    private readonly YouTubeService service;
    private SearchListResponse response;
    private int index;

    public YouTubeSearcher()
    {
        this.service = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = API_key,
            ApplicationName = "Sharpvin"
        });

        this.index = 0;
        response = new SearchListResponse();
    }

    public async Task<String> Search(string query)
    {
        var request = this.service.Search.List("snippet");

        request.Q = query;
        request.MaxResults = 5;
        request.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;
        request.Type = "video";
        request.RelevanceLanguage = "en";

        this.response = await request.ExecuteAsync();
        this.index = 0;

        if (response.Items.Count == 0)
        {
            throw new Exception("No results from youtube");
        }

        return $"https://www.youtube.com/watch?v={response.Items[0].Id.VideoId}";
    }

    public String NextVideo()
    {
        index++;

        if (index < response.Items.Count)
        {
            return $"https://www.youtube.com/watch?v={response.Items[this.index].Id.VideoId}";
        }
        else
            return "";
    }
}

