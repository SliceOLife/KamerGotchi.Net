using Kamergotchi.Entities;
using RestSharp;
using System;
using System.Linq;
using System.Threading;
using System.Timers;

namespace Kamergotchi
{
    public class GameHandler
    {
        private const string API_BASE_URI = "https://api.kamergotchi.nl";
        private string API_PLAYER_TOKEN;
        IRestResponse<RootObject> gameState;

        public GameHandler(string playerToken)
        {
            API_PLAYER_TOKEN = playerToken;

            initBot();

            System.Timers.Timer gameTimer = new System.Timers.Timer(30000);
            gameTimer.Elapsed += new ElapsedEventHandler(checkGameStateEvent);
            gameTimer.Enabled = true;
        }

        private void initBot()
        {
            getGameState();
            onGameStateUpdate();
        }

        private void checkGameStateEvent(object sender, ElapsedEventArgs e)
        {
            getGameState();
            onGameStateUpdate();
        }

        private void onGameStateUpdate()
        {
            // Print info
            printInfo();

            // We got gamestate, figure out if we need to care/claim
            DateTime careReset = DateTime.Parse(gameState.Data.game.careReset);
            DateTime claimReset = DateTime.Parse(gameState.Data.game.claimReset);
            DateTime rightNow = DateTime.Now;
            int careLeft = gameState.Data.game.careLeft;

            if(rightNow > claimReset)
            {
                claimScoreBonus();
                printInfo();
            }
            else
            {
                Console.WriteLine("Need to wait {0} more seconds to claim", (claimReset - rightNow).TotalSeconds);
            }

            if (careLeft > 0 || rightNow > careReset)
            {
                careLowestStat();
                printInfo();
            }else
            {
                Console.WriteLine("Need to wait {0} more seconds", (careReset - rightNow).TotalSeconds);
            }
        }

        private void careLowestStat()
        {
                int careLeft = gameState.Data.game.careLeft;
                int foodStat = gameState.Data.game.current.food;
                int attentionStat = gameState.Data.game.current.attention;
                int knowledgeStat = gameState.Data.game.current.knowledge;

                if (foodStat < attentionStat)
                {
                    if (knowledgeStat < foodStat)
                    {
                        giveCare(CareType.knowledge);
                    }
                    else
                    {
                        giveCare(CareType.food);
                    }
                }
                else
                {
                    if (attentionStat < knowledgeStat)
                    {
                        giveCare(CareType.attention);
                    }
                    else
                    {
                        giveCare(CareType.knowledge);
                    }
                }

            Thread.Sleep(1000);

            // Re-check
            onGameStateUpdate();
        }

        private void printInfo()
        {
            // Seperate from previous data.
            Console.WriteLine("--------------------------------------------------");

            Console.WriteLine(String.Format("Politicus: {0} \nPartij: {1} \nScore: {2}", gameState.Data.game.gotchi.displayName, gameState.Data.game.gotchi.partyText, gameState.Data.game.score));

            Console.WriteLine(String.Format(@"Current Stats:
                Health: {0}
                Food: {1}
                Knowledge: {2}
                Attention: {3}
                You've been alive for {4} days", 
                gameState.Data.game.health,gameState.Data.game.current.food, gameState.Data.game.current.knowledge, gameState.Data.game.current.attention, gameState.Data.game.daysAlive));
        }

        private void getGameState()
        {
            var client = new RestClient(API_BASE_URI);
            var request = new RestRequest("game", Method.GET);
            request.AddHeader("x-player-token", API_PLAYER_TOKEN);

            gameState = client.Execute<RootObject>(request);
        }

        private void claimScoreBonus()
        {
            var client = new RestClient(API_BASE_URI);
            var claimPost = new RestRequest("game/claim", Method.POST);
            claimPost.AddHeader("x-player-token", API_PLAYER_TOKEN);

            claimPost.AddJsonBody(new { bar = "foo" });

            gameState = client.Execute<RootObject>(claimPost);

            Console.WriteLine("Claimed bonus!");
        }

        private void giveCare(CareType careType)
        {
            var client = new RestClient(API_BASE_URI);
            var carePost = new RestRequest("game/care", Method.POST);
            carePost.AddHeader("x-player-token", API_PLAYER_TOKEN);

            carePost.AddJsonBody(new { bar = careType.ToString()});

            gameState = client.Execute<RootObject>(carePost);

            Console.WriteLine("Posted CareType: {0}", careType.ToString());
        }
    
    }

    enum CareType
    {
        food,
        attention,
        knowledge
    }
}
