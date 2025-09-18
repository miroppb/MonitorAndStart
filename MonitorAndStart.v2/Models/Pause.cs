using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Models
{
    public class Pause : Job
    {
        public int seconds;

        public Pause(string _name, int _seconds)
        {
            Name = _name;
            seconds = _seconds;
        }

        public override int TypeOfJob => 5;

        public static List<string> Vars => ["Seconds"];

        public override string ToString => $"Pause for {seconds} seconds";

        public override async Task ExecuteJob(bool force)
        {
            //pretty simple
            await Task.Delay(seconds * 1000);
            CompletedSuccess = true;
            return;
        }
    }
}
