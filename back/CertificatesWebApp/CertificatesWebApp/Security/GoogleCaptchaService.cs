using System.Net;
using Newtonsoft.Json;

namespace CertificatesWebApp.Security
{
    public interface IGoogleCaptchaService
    {
        Task<bool> VerifyToken(string token);
    }
    public class GoogleCaptchaService : IGoogleCaptchaService
    {
        public GoogleCaptchaService()
        {

        }

        public async Task<bool> VerifyToken(string token)
        {
            StreamReader sr = new StreamReader("recaptcha_key.txt");
            string secret = sr.ReadLine();
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage httpResult = await client.GetAsync(url);
                if (httpResult.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                string responseString = await httpResult.Content.ReadAsStringAsync();
                GoogleCaptchaResponse googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);
                return googleResult.Success && googleResult.Score >= 0.5;
            }
        }
    }
}
