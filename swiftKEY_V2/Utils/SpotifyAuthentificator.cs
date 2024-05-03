using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace swiftKEY_V2.Utils
{
    public class SpotifyAuthentificator
    {
        private static string _clientId = "";
        private static string _clientSecret = "";
        private static string _redirectUri = "";
        private static string _authorizationEndpoint = "https://accounts.spotify.com/authorize";
        private static string _tokenEndpoint = "https://accounts.spotify.com/api/token";

        private static string _accessToken;
        private static string _refreshToken;
        private SpotifyConfig config;

        public async Task InitializeAuthAsync()
        {
            config = ConfigManager.LoadSpotifyConfig();
            _clientId = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientID").Value;
            _clientSecret = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientSecret").Value;
            _redirectUri = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RedirectUri").Value;
            _refreshToken = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RefreshToken").Value;

            // Überprüfe, ob ein Refresh Token vorhanden ist
            if (LoadRefreshTokenFromSettings())
            {
                bool isValid = await CheckRefreshTokenValidity();
                if (isValid)
                {
                    await RefreshAccessToken();
                }
                else
                {
                    // Refresh Token ist ungültig, ein neues wird angefordert
                    // TODO
                }
            }
            else
            {
                // Starte den HTTP-Listener für die Rückgabe von Spotify
                await StartHttpListener();
            }
        }

        private bool LoadRefreshTokenFromSettings()
        {
            _refreshToken = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RefreshToken").Value;
            return !string.IsNullOrEmpty(_refreshToken);
        }

        private async Task<bool> CheckRefreshTokenValidity()
        {
            try
            {
                // Versuche, ein neues Access-Token mit dem Refresh-Token abzurufen
                await RefreshAccessToken();
                // Wenn das Token erneuert werden konnte, ist das Refresh-Token noch gültig
                return true;
            }
            catch (Exception)
            {
                // Wenn beim Aktualisieren des Tokens ein Fehler auftritt, ist das Refresh-Token ungültig
                return false;
            }
        }

        private async Task StartHttpListener()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(_redirectUri + "/");
            listener.Start();

            var context = await listener.GetContextAsync();
            var request = context.Request;

            var response = context.Response;
            string responseString = "<html><head><title>Spotify Authorization</title></head><body>Authorization successful. You can close this window.</body></html>";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            // Extrahiere den Autorisierungscode aus der Antwort
            var authorizationCode = request.QueryString["code"];

            // Tausche den Autorisierungscode gegen ein Access Token und ein Refresh Token aus
            await ExchangeAuthorizationCodeForTokens(authorizationCode);
        }

        private async Task ExchangeAuthorizationCodeForTokens(string authorizationCode)
        {
            var postData = $"grant_type=authorization_code&code={authorizationCode}&redirect_uri={Uri.EscapeUriString(_redirectUri)}&client_id={_clientId}&client_secret={_clientSecret}";

            var request = (HttpWebRequest)WebRequest.Create(_tokenEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                await streamWriter.WriteAsync(postData);
                await streamWriter.FlushAsync();
            }

            using (var response = await request.GetResponseAsync())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var json = await streamReader.ReadToEndAsync();
                var tokenData = JObject.Parse(json);
                _accessToken = tokenData["access_token"].ToString();
                _refreshToken = tokenData["refresh_token"].ToString();
            }
            SaveRefreshTokenToSettings(_refreshToken);
        }

        private void SaveRefreshTokenToSettings(string refreshToken)
        {
            config = ConfigManager.LoadSpotifyConfig();
            config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RefreshToken").Value = refreshToken;
            ConfigManager.SaveConfig(config);
        }

        private async Task RefreshAccessToken()
        {
            var postData = $"grant_type=refresh_token&refresh_token={_refreshToken}&client_id={_clientId}&client_secret={_clientSecret}";

            var request = (HttpWebRequest)WebRequest.Create(_tokenEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                await streamWriter.WriteAsync(postData);
                await streamWriter.FlushAsync();
            }

            using (var response = await request.GetResponseAsync())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var json = await streamReader.ReadToEndAsync();
                var tokenData = JObject.Parse(json);
                _accessToken = tokenData["access_token"].ToString();
            }
        }

        public void login()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                MessageBox.Show("Du bist bereits angemeldet.");
                return;
            }

            config = ConfigManager.LoadSpotifyConfig();
            _clientId = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientID").Value;
            _clientSecret = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "ClientSecret").Value;
            _redirectUri = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RedirectUri").Value;
            _refreshToken = config.SpotifyConfigurations.FirstOrDefault(entry => entry.Title == "RefreshToken").Value;
            Task.Run(() => StartHttpListener());
            var authorizationRequestUrl = $"{_authorizationEndpoint}?response_type=code&client_id={_clientId}&redirect_uri={Uri.EscapeUriString(_redirectUri)}&scope=user-read-playback-state user-modify-playback-state";
            System.Diagnostics.Process.Start(authorizationRequestUrl);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            await InitializeAuthAsync();
            return _accessToken;
        }
    }
}
