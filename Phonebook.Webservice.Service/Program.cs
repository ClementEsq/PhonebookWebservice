using Topshelf;

namespace Phonebook.Webservice.Service
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var exitCode = HostFactory.Run(c =>
            {
                c.SetServiceName("PhonebookService");
                c.StartAutomatically();
                c.Service<PhonebookService>(s =>
                {
                    s.ConstructUsing(() => new PhonebookService());
                    s.WhenStarted(a => a.Start());
                    s.WhenStopped(a => a.Stop());
                });
                c.EnableServiceRecovery(s =>
                {
                    s.RestartService(0);
                    s.SetResetPeriod(1);
                });
            });

            return (int)exitCode;
        }
    }
}
