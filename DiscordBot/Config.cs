﻿using System.IO;
using Newtonsoft.Json;
using Gideon.Handlers;
using Gideon.Minigames;
using System.Collections.Generic;

namespace Gideon
{
    class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";
        private const string resourcesFile = "resources.json";
        private const string questionsFile = "questions.json";
        private const string triviaQuestionsFile = "trivia_questions.json";

        public static BotConfig bot;
        public static BotResources botResources;
        public static TriviaQuestions triviaQuestions;

        public static ImageFetcher ImageFetcher = new ImageFetcher();
        public static Utilities Utilities = new Utilities();
        public static StatsHandler StatsHandler = new StatsHandler();
        public static CoinsHandler CoinHandler = new CoinsHandler();
        public static MinigameHandler MinigameHandler = new MinigameHandler();
        public static RankHandler RankHandler = new RankHandler();

		public static void ResetTTT() => MinigameHandler.TTT = new TicTacToe();

        static Config()
        {
            RankHandler.Start();

            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }

            if (!File.Exists(configFolder + "/" + resourcesFile))
            {
                string json = JsonConvert.SerializeObject(botResources, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + resourcesFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + resourcesFile);
                botResources = JsonConvert.DeserializeObject<BotResources>(json);
            }

            if (!File.Exists(configFolder + "/" + triviaQuestionsFile))
            {
                string json = JsonConvert.SerializeObject(triviaQuestions, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + triviaQuestionsFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + triviaQuestionsFile);
                triviaQuestions = JsonConvert.DeserializeObject<TriviaQuestions>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string DisordBotToken;
        public string MovieTVAPIKey;
        public string YouTubeAPIKey;
    }

    public struct BotResources
    {
        public List<string> bannedWords;
        public List<string> allowedChannels;
    }

    public struct TriviaQuestion
    {
        public string Question;
        public string Answer;
        public List<string> IncorrectAnswers;
    }

    public struct TriviaQuestions
    {
        public List<TriviaQuestion> Questions;
    }
}