using StatsdClient;

namespace Phonebook.Webservice
{
    public class StatsDConfig
    {
        public static void Configuration()
        {
            var dogstatsdConfig = new StatsdConfig
            {
                StatsdServerName = "127.0.0.1",
                StatsdPort = 8125,
                Prefix = "WebServices"
            };

            DogStatsd.Configure(dogstatsdConfig);
        }
    }
}