﻿using Microsoft.Extensions.Options;
using Randominer.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Randominer.Twitch
{
    public class TwitchClient : ITwitchClient
    {
        protected string _apiKey;
        protected HttpClient _httpClient;

        public TwitchClient(string apiKey, HttpClient httpClient = null)
        {
            _apiKey = apiKey;
            _httpClient = httpClient != null ? httpClient : new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", _apiKey);
        }

        public async Task<string> VerifyConnectionAsync()
        {
            var request = _httpClient.GetStringAsync("https://api.twitch.tv/helix/streams?game_id=33214");

            var msg = await request;

            return msg;
        }
        
        public async Task<StreamListDTO> GetStreams()
        {
            var offset = new Random().Next(0, await GetOffset(25));
            var request = _httpClient.GetAsync($"https://api.twitch.tv/helix/streams?offset={offset}");            

            var str = await request.Result.Content.ReadAsStringAsync();

            var streamListDTO = JsonConvert.DeserializeObject<StreamListDTO>(str);
            return streamListDTO;
        }

        
        
        public async Task<int> GetOffset(int streamLimit)
        {
            var request = _httpClient.GetAsync($"https://api.twitch.tv/helix/streams/summary");            

            var str = await request.Result.Content.ReadAsStringAsync();

            var streamSummary = JsonConvert.DeserializeObject<StreamSummaryDTO>(str);

            return streamSummary != null && streamSummary.Channels > streamLimit ? new Random().Next(0, streamSummary.Channels-streamLimit) : 0;
        }
    }
}
