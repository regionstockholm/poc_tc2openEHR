using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

namespace Spine.Foundation.Web.OpenEhr.Client
{
    public interface ITokenService
    {
        Task<dynamic> TokenByClientCredentials();
    }

    // ToDo  Encapsulation of the token
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<Credentials> _credentialOptions;

        public TokenService(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IOptions<Credentials> credentialOptions)
        {
            _httpClientFactory = httpClientFactory;
            _credentialOptions = credentialOptions;
            _configuration = configuration;
        }

        public async Task<dynamic> TokenByClientCredentials()
        {
            // ToDo 
            // 1. Add a Caching for token

            var endPointUrl = _configuration["Endpoint:Url"];

            if (endPointUrl == null)
            {
                throw new ArgumentNullException("Endpoint:Url is not set");
            }

            var IdpPath = _configuration["Endpoint:IdpPath"];

            if (IdpPath == null)
            {
                throw new ArgumentNullException("Endpoint:IdpPath is not set");
            }

            var formContent = FormUrlEncodedCredentials();
            var idpUrl = $"{endPointUrl}{IdpPath}/protocol/openid-connect/token";
            var token = string.Empty;
            using (var client = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, idpUrl);
                request.Content = formContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                // ToDo Async
                var response = client.Send(request);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(content);
                token = jsonDocument.RootElement.GetProperty("access_token").GetString();
            }

            return token;
        }

        private FormUrlEncodedContent FormUrlEncodedCredentials()
        {

            var credentails = new Credentials
            {
                ClientId = _configuration["TokenCredentials:ClientId"],
                ClientSecret = _configuration["TokenCredentials:ClientSecret"],
                GrantType = _configuration["TokenCredentials:GrantType"]
            };

            return new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("grant_type", credentails.GrantType),
                new KeyValuePair<string, string>("client_id", credentails.ClientId),
                new KeyValuePair<string, string>("client_secret", credentails.ClientSecret)
            });


        }
    }

    public class EhrResponse
    {
        public string EhrId { get; set; }
    }

    public class CompositionSaveResponse
    {
        public string CompositionUid { get; set; }
    }

    public class OpenEhrServiceAgent : IOpenEhrServiceAgent
    {
        private ITokenService _tokenService => _lazyTokenService.Value;
        private readonly Lazy<ITokenService> _lazyTokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public OpenEhrServiceAgent(
             Lazy<ITokenService> lazyTokenService,
             IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _lazyTokenService = lazyTokenService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<Guid> GetEhrId(string extenalId, string nameSpace)
        {
            var endPointUrl = _configuration["Endpoint:Url"];

            if (endPointUrl == null)
            {
                throw new ArgumentNullException("Endpoint:Url is not set");
            }

            var ehrPath = _configuration["Endpoint:EhrPath"];

            if (ehrPath == null)
            {
                throw new ArgumentNullException("Endpoint:IdpPath is not set");
            }

            var token = await _tokenService.TokenByClientCredentials();


            using (var client = _httpClientFactory.CreateClient())
            {
                var ehrUrl = $"{endPointUrl}{ehrPath}/rest/v1/ehr?subjectId={extenalId}&subjectNamespace={nameSpace}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(ehrUrl).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var ehrResponse = JsonConvert.DeserializeObject<EhrResponse>(content);
                return Guid.Parse(ehrResponse.EhrId);
            }
        }

        // ...

        public async Task<dynamic> SaveComposition(JObject composition, Guid ehrId)
        {
            var endPointUrl = _configuration["Endpoint:Url"];
            var templateId = _configuration["Template:TemplateId"];           
            var namespaces = _configuration["Template:Namespace"];
            var format = _configuration["Template:Format"];
            var lifecycleState = _configuration["Template:LifecycleState"];
            var auditChangeType = _configuration["Template:AuditChangeType"];

            if (endPointUrl == null)
            {
                throw new ArgumentNullException("Endpoint:Url is not set");
            }

            var ehrPath = _configuration["Endpoint:EhrPath"];

            if (ehrPath == null)
            {
                throw new ArgumentNullException("Endpoint:IdpPath is not set");
            }

            var token = await _tokenService.TokenByClientCredentials();

            using (var client = _httpClientFactory.CreateClient())
            {
                var ehrUrl = $"{endPointUrl}{ehrPath}/rest/v1/composition?templateId={templateId}&ehrId={ehrId}&subjectNamespace={namespaces}&format={format}&lifecycleState={lifecycleState}&auditChangeType={auditChangeType}";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Add request body
                //var requestBody = new StringContent(composition.ToString(), Encoding.UTF8, "application/json");
                HttpContent content = new StringContent(composition.ToString(), Encoding.UTF8, "application/json");
                var response = client.PostAsync(ehrUrl, content).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
                var content2 = await response.Content.ReadAsStringAsync();
                var ehrResponse = JsonConvert.DeserializeObject<CompositionSaveResponse>(content2);
                return ehrResponse.CompositionUid;
            }
        }
    }

    public class Credentials
    {
        private string clientId;
        public string ClientId
        {
            get => clientId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidCredentialException("TokenCredentials:ClientId is not set");
                }
                clientId = value;
            }
        }

        private string clientSecret;
        public string ClientSecret
        {
            get => clientSecret;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidCredentialException("TokenCredentials:ClientSecret is not set");
                }
                clientSecret = value;
            }
        }

        private string grantType;
        public string GrantType
        {
            get => grantType;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidCredentialException("TokenCredentials:GrantType is not set");
                }
                grantType = value;
            }
        }
    }
    public class OpenEhrConfigurations
    {
        public string EndpointUrl { get; set; }
        public Credentials ClientCredentials { get; set; }
        public string NameSpace { get; set; }
    }
}