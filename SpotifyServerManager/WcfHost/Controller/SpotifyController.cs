using SpotifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WcfHost.Models;

namespace WcfHost.Controller
{
    public class SpotifyController : ApiController
    {
        public static ServerVM ServerVM { get; set; } = new ServerVM();

        private SpotifyProvider SpotifyProvider { get; set; }

        public SpotifyController()
        {
            // Zur Verwendung von Spotify
            SpotifyProvider = new SpotifyProvider();
            SpotifyProvider.Connect();
        }

        // GET api/<controller>
        // http://localhost:8082/api/spotify/Get
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        // http://localhost:8082/api/spotify/Get?text=1
        public string Get(string text)
        {
            return text;
        }

        [HttpGet]
        [ActionName("Register")]
        public string Register()
        {
            return "Herzlich Willkommen auf dem Server von Erik";
        }

        [HttpGet]
        [ActionName("PlayPause")]
        public async Task<string> PausePlay(IPAddress ipAdress, string hostname)
        {
            //var member = ServerVM.ConnectedMembers.Where(c => c.IPAddress.Equals(hostname)).SingleOrDefault();
            //if(member == null)
            //{
            //    member = new WcfMember { Hostname = hostname, LoginDate = DateTime.Now };
            //    ServerVM.ConnectedMembers.Add(member);
            //    ServerVM.WriteToLog($"{member.Hostname} hat Pause/Play gedrückt");
            //}


            ServerVM.WriteToLog($"{hostname} hat Pause/Play gedrückt");
            string res = await SpotifyProvider.PerformPlayAsync();

            return res;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
