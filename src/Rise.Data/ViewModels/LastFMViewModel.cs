using Rise.Common.Constants;
using Rise.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Media.Playback;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;
using Windows.Web.Http;

namespace Rise.Data.ViewModels
{
    public sealed partial class LastFMViewModel : ViewModel
    {
        private readonly string _key;
        private readonly string _secret;

        /// <summary>
        /// Initializes a new instance of this class with the provided
        /// API keys.
        /// </summary>
        /// <param name="key">last.fm API key.</param>
        /// <param name="secret">last.fm secret key.</param>
        public LastFMViewModel(string key, string secret)
        {
            _key = key;
            _secret = secret;
        }

        private bool _authenticated = false;
        /// <summary>
        /// WHether the user has been authenticated.
        /// </summary>
        public bool Authenticated
        {
            get => _authenticated;
            private set => Set(ref _authenticated, value);
        }

        /// <summary>
        /// Session key used for the LastFM API.
        /// </summary>
        private string _sessionKey;

        private string _username;
        /// <summary>
        /// Username for the currently logged in user.
        /// </summary>
        public string Username
        {
            get => _username;
            private set => Set(ref _username, value);
        }

        /// <summary>
        /// Attempts to authenticate the user to last.fm.
        /// </summary>
        /// <returns>true if the authentication is successful,
        /// false otherwise.</returns>
        public async Task<bool> TryAuthenticateAsync()
        {
            var token = await GetTokenAsync();
            var uriBuilder = new StringBuilder();
            _ = uriBuilder.Append("https://www.last.fm/api/auth?api_key=");
            _ = uriBuilder.Append(_key);
            _ = uriBuilder.Append("&token=");
            _ = uriBuilder.Append(token);
            _ = uriBuilder.Append("&redirect_uri=");
            _ = uriBuilder.Append(Uri.EscapeDataString("https://www.google.com"));

            Uri startUri = new(uriBuilder.ToString());
            Dictionary<string, string> args = new()
            {
                { "method", "auth.getSession" },
                { "api_key", _key },
                { "token", token }
            };

            Uri endUri = GetSignedUri(args);

            WebAuthenticationResult result;
            try
            {
                result = await WebAuthenticationBroker.
                    AuthenticateAsync(WebAuthenticationOptions.UseTitle, startUri, endUri);
            }
            catch (FileNotFoundException)
            {
                // FileNotFound generally means no access to the host
                return false;
            }

            if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                return false;
            }

            string response;
            using (HttpClient client = new())
            {
                try
                {
                    response = await client.GetStringAsync(endUri);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            XmlDocument doc = new();
            doc.LoadXml(response);

            _sessionKey = GetNodeFromResponse(doc, "/lfm/session/key");
            Username = GetNodeFromResponse(doc, "/lfm/session/name");

            Authenticated = true;
            return true;
        }

        /// <summary>
        /// Saves the username and session key to the vault.
        /// </summary>
        public void SaveCredentialsToVault(string resource)
        {
            if (!_authenticated)
            {
                return;
            }

            PasswordVault vault = new();
            vault.Add(new PasswordCredential(resource, Username, _sessionKey));
        }

        /// <summary>
        /// Attempts to load credentials from the vault.
        /// </summary>
        /// <returns>true if the credentials were loaded successfully,
        /// false otherwise.</returns>
        public bool TryLoadCredentials(string resource)
        {
            try
            {
                PasswordVault vault = new();
                IEnumerable<PasswordCredential> credentials = vault.RetrieveAll().Where(p => p.Resource == resource);

                foreach (PasswordCredential credential in credentials)
                {
                    credential.RetrievePassword();
                    Username = credential.UserName;
                    _sessionKey = credential.Password;
                }

                Authenticated = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(_sessionKey);
                return Authenticated;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to scrobble the provided <see cref="MediaPlaybackItem"/>.
        /// </summary>
        /// <returns>true if the item was successfully scrobbled,
        /// false otherwise.</returns>
        public async Task<bool> TryScrobbleItemAsync(MediaPlaybackItem item)
        {
            if (!_authenticated)
            {
                return false;
            }

            if (item == null)
            {
                return false;
            }

            TimeSpan span = DateTime.UtcNow - new DateTime(1970, 1, 1);
            string curr = ((int)span.TotalSeconds).ToString();

            Windows.Media.MusicDisplayProperties props = item.GetDisplayProperties().MusicProperties;
            string title = props.Title;
            string artist = props.Artist;

            Dictionary<string, string> parameters = new()
            {
                { "artist[0]", artist },
                { "track[0]", title },
                { "timestamp[0]", curr },
                { "method", "track.scrobble" },
                { "api_key", _key },
                { "sk", _sessionKey }
            };

            string signature = GetSignature(parameters);

            StringBuilder comboBuilder = new();
            _ = comboBuilder.Append("https://ws.audioscrobbler.com/2.0/?method=track.scrobble&api_key=");
            _ = comboBuilder.Append(_key);
            _ = comboBuilder.Append("&artist[0]=");
            _ = comboBuilder.Append(artist);
            _ = comboBuilder.Append("&track[0]=");
            _ = comboBuilder.Append(title);
            _ = comboBuilder.Append("&sk=");
            _ = comboBuilder.Append(_sessionKey);
            _ = comboBuilder.Append("&timestamp[0]=");
            _ = comboBuilder.Append(curr);
            _ = comboBuilder.Append("&api_sig=");
            _ = comboBuilder.Append(signature);

            Uri uri = new(comboBuilder.ToString());
            HttpStringContent content = new("");
            using HttpClient client = new();
            try
            {
                _ = await client.PostAsync(uri, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                content.Dispose();
            }
        }

        /// <summary>
        /// Logs out of the current session, clearing
        /// the username and session key.
        /// </summary>
        public void LogOut()
        {
            Authenticated = false;

            Username = null;
            _sessionKey = null;
        }
    }

    // Private methods
    public partial class LastFMViewModel
    {
        private async Task<string> GetTokenAsync()
        {
            string m_strFilePath = URLs.LastFM + "auth.gettoken&api_key=" + _key;

            string response;
            using (HttpClient client = new())
            {
                try
                {
                    response = await client.GetStringAsync(new Uri(m_strFilePath));
                }
                catch (Exception)
                {
                    return null;
                }
            }

            XmlDocument doc = new();
            doc.LoadXml(response);
            return GetNodeFromResponse(doc, "/lfm/token");
        }

        private Uri GetSignedUri(Dictionary<string, string> args)
        {
            StringBuilder stringBuilder = new();
            _ = stringBuilder.Append("https://ws.audioscrobbler.com/2.0/?");

            foreach (KeyValuePair<string, string> kvp in args)
            {
                _ = stringBuilder.AppendFormat("{0}={1}&", kvp.Key, kvp.Value);
            }

            _ = stringBuilder.Append("api_sig=");
            _ = stringBuilder.Append(SignCall(args));

            return new Uri(stringBuilder.ToString());
        }

        private string GetSignature(Dictionary<string, string> parameters)
        {
            StringBuilder resultBuilder = new();
            IOrderedEnumerable<KeyValuePair<string, string>> data = parameters.OrderBy(x => x.Key);
            foreach (KeyValuePair<string, string> kvp in data)
            {
                _ = resultBuilder.Append(kvp.Key);
                _ = resultBuilder.Append(kvp.Value);
            }

            _ = resultBuilder.Append(_secret);
            return resultBuilder.ToString().GetEncodedHash("MD5");
        }

        private string SignCall(Dictionary<string, string> args)
        {
            IOrderedEnumerable<KeyValuePair<string, string>> sortedArgs = args.OrderBy(arg => arg.Key);

            string signature = sortedArgs.Select(pair => pair.Key + pair.Value).
                Aggregate((first, second) => first + second);
            signature += _secret;

            return signature.GetEncodedHash("MD5");
        }

        private string GetNodeFromResponse(XmlDocument doc, string node)
        {
            XmlNode selected = doc.DocumentElement.SelectSingleNode(node);
            return selected.InnerText;
        }
    }
}
