using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamergotchi.Entities
{
    public class Game
    {
        public string claimReset { get; set; }
        public int careLeft { get; set; }
        public object endDate { get; set; }
        public string startDate { get; set; }
        public Gotchi gotchi { get; set; }
        public int batchNumber { get; set; }
        public string player { get; set; }
        public string careReset { get; set; }
        public List<Quote> quotes { get; set; }
        public int dayScore { get; set; }
        public int score { get; set; }
        public Current current { get; set; }
        public Device device { get; set; }
        public int daysAlive { get; set; }
        public int health { get; set; }
        public bool active { get; set; }
        public bool ended { get; set; }
        public bool started { get; set; }
        public int claimLimitSeconds { get; set; }
        public int careLimitSeconds { get; set; }
        public string id { get; set; }
    }
}
