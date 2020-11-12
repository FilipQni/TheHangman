using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;
using System.IO;

namespace TheHangman
{
    class Program
    {

        public struct CountryAndCapitalStruct
        {
            public string Country { get; set; }
            public string Capital { get; set; }
        }

        public struct PlayerStats
        {
            public string Name { get; set; }

            public string Date { get; set; }

            public int GuessingTime { get; set; }

            public int GuessedLettters { get; set; }

            public string GuessedCapital { get; set; }
        }

        static void Main(string[] args)
        {
            string playerName;
            string line;
            string[] words;
            var countryAndCapitalList = new List<CountryAndCapitalStruct>();
            var countryAndCapitalBox = new CountryAndCapitalStruct();

            ConsoleKeyInfo choice;

            //open files
            System.IO.StreamReader fileWithCapitals = new System.IO.StreamReader(@"E:\Projekty\TheHangmanGame\TheHangman\resources\countries_and_capitals.txt");

            while ((line = fileWithCapitals.ReadLine()) != null)
            {
                words = line.Split(" | ");                                              // loading words from file to list
                countryAndCapitalBox.Country = words[0];
                countryAndCapitalBox.Capital = words[1];
                countryAndCapitalList.Add(countryAndCapitalBox);
            }

            Console.WriteLine("The game Hangman");
            Console.WriteLine("Write your name");
            playerName = Console.ReadLine();
            Console.WriteLine("\nWould you like to start? (press s to start, e to exit)");

            do
            {
                choice = Console.ReadKey(true);
                if (choice.KeyChar == 's' ^ choice.KeyChar == 'S') Game(countryAndCapitalList, playerName);   //start menu

            } while (choice.KeyChar != 'e' && choice.KeyChar != 'E');
        }

        private static void Game(List<CountryAndCapitalStruct> countryAndCapitalList, string playerName)
        {
            bool winGame = false;
            short gameStatus = 0;
            int randomIndex;
            int lifes = 6;
            int guessedLettersNumber = 0;
            string userGuess;
            string capitalToGuess;
            string wrongLetters = "";
            bool[] isLetterGuessed;
            ConsoleKeyInfo choice;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeSpan = new TimeSpan();

            Console.Clear();
            Random rnd = new Random();
            randomIndex = rnd.Next(countryAndCapitalList.Count);                      //  picking a random pair of
            capitalToGuess = countryAndCapitalList.ElementAt(randomIndex).Capital;    //  country and it's capitol
            isLetterGuessed = new bool[capitalToGuess.Length];

            for (int i = 0; i < capitalToGuess.Length; i++)
            {
                if (capitalToGuess[i] == ' ') isLetterGuessed[i] = true;              // setting starting flags
                else isLetterGuessed[i] = false;
            }

            stopwatch.Start();

            while (lifes > 0)
            {
                CreateGameInterface(lifes, capitalToGuess, isLetterGuessed, wrongLetters, gameStatus);

                if (winGame)
                {
                    stopwatch.Stop();
                    timeSpan = stopwatch.Elapsed;
                    Console.WriteLine("YOU WON!!!!");
                    Console.WriteLine("You guessed the capital after " + guessedLettersNumber + " letters");
                    Console.WriteLine("Your time: " + timeSpan.Seconds + " seconds");
                    break;
                }

                do
                {
                    choice = Console.ReadKey(true);
                    if (choice.KeyChar == '1')
                    {
                        Console.WriteLine("Write a letter");
                        userGuess = Console.ReadLine();
                        if (userGuess.Length != 1) continue;

                        if (CheckLetter(ref guessedLettersNumber, ref lifes, userGuess[0], capitalToGuess, isLetterGuessed, ref wrongLetters))
                        {
                            gameStatus = 1;
                        }
                        else
                        {
                            gameStatus = 2;
                        }
                    }
                    if (choice.KeyChar == '2')
                    {
                        Console.WriteLine("Write a capital");
                        userGuess = Console.ReadLine();
                        if (userGuess.Length < 2) continue;

                        if (String.Compare(capitalToGuess, userGuess, true) == 0)
                        {
                            gameStatus = 3;
                            winGame = true;
                        }
                        else
                        {
                            gameStatus = 4;
                            lifes -= 2;
                        }
                    }

                } while (choice.KeyChar != '1' && choice.KeyChar != '2');

                if (IsWordGuessed(isLetterGuessed)) winGame = true;

            }

            if (!winGame)
            {
                stopwatch.Stop();
                timeSpan = stopwatch.Elapsed;
                Console.WriteLine("You lost...");
                Console.WriteLine("You guessed " + guessedLettersNumber + " letters");
                Console.WriteLine("The Capital that you had to guess: " + capitalToGuess);
                Console.WriteLine("Your time: " + timeSpan.Seconds + " seconds\n");
            }

            ScoreBoard(playerName, timeSpan.Seconds, guessedLettersNumber, capitalToGuess);
            Console.WriteLine("\nWould you like to start next game? (press s to start, e to exit)");
        }

        private static void CreateGameInterface(int lifes, string capitalToGuess, bool[] isLetterGuessed, string wrongLetters, short gameStatus)
        {
            int counter = 0;

            Console.Clear();
            Console.WriteLine(capitalToGuess);
            Console.WriteLine("Press 1 to guess a letter, press 2 to guess a whole word");
            Console.WriteLine("Lifes: " + lifes);
            Console.Write("Wrong letters: ");
            foreach (char letter in wrongLetters)
            {
                Console.Write(letter + " ");
            }
            Console.Write("\n\n");

            foreach (char sign in capitalToGuess)
            {
                if (sign == ' ')
                {
                    Console.Write("  ");
                }
                else
                {
                    if (isLetterGuessed[counter])
                    {
                        Console.Write(capitalToGuess[counter] + " ");
                    }
                    else
                    {
                        Console.Write("_ ");
                    }
                }
                counter++;
            }
            
            Console.Write("\n");
            Console.WriteLine();
            DrawHangman(lifes);

            switch (gameStatus)
            {
                case 0:
                    break;
                case 1:
                    Console.WriteLine("Well done!");
                    break;
                case 2:
                    Console.WriteLine("Wrong letter!");
                    break;
                case 3:
                    Console.WriteLine("Well done!");
                    break;
                case 4:
                    Console.WriteLine("Wrong capital!");
                    break;
                default:
                    break;
            }
        }

        private static bool CheckLetter(ref int guessedLetters, ref int lifes, char letter, string word, bool[] isLetterGuessed, ref string wrongLetters)
        {
            bool correct = false;
            bool firstTime = true;

            for (int i = 0; i < wrongLetters.Length; i++)
            {
                if (letter == wrongLetters[i])
                {
                    return false;
                }
            }
            for (int i = 0; i < word.Length; i++)
            {
                if (char.ToLower(letter) == char.ToLower(word[i]))
                {
                    correct = true;
                    isLetterGuessed[i] = true;
                    if (firstTime)
                    {
                        firstTime = !firstTime;
                        guessedLetters++;
                    }
                }
            }
            if (correct) return true;
            else
            {
                wrongLetters += letter;
                lifes--;
                return false;
            }

        }
        private static bool IsWordGuessed(bool[] isLetterGuessed)
        {
            foreach (bool flag in isLetterGuessed)
            {
                if (!flag) return false;

            }
            return true;

        }

        private static void ScoreBoard(string playerName, int seconds, int guessedLettersNumber, string capitalToGuess)
        {
            string line;
            string[] data;
            DateTime thisDay = DateTime.Now;
            var playerStatsBox = new PlayerStats();
            var playersStatsList = new List<PlayerStats>();
            StreamReader fileWithTop10 = new StreamReader(@"E:\Projekty\TheHangmanGame\TheHangman\resources\top10.txt");


            while ((line = fileWithTop10.ReadLine()) != null)
            {
                data = line.Split(" | ");                                              // loading words from file to list
                playerStatsBox.Name = data[0];
                playerStatsBox.Date = data[1];
                playerStatsBox.GuessingTime = Int32.Parse(data[2]);
                playerStatsBox.GuessedLettters = Int32.Parse(data[3]);
                playerStatsBox.GuessedCapital = data[4];
                playersStatsList.Add(playerStatsBox);
            }

            fileWithTop10.Close();
            StreamWriter fileToSave = new StreamWriter(@"E:\Projekty\TheHangmanGame\TheHangman\resources\top10.txt", false);

            playerStatsBox.Name = playerName;
            playerStatsBox.Date = thisDay.ToString();
            playerStatsBox.GuessingTime = seconds;
            playerStatsBox.GuessedLettters = guessedLettersNumber;
            playerStatsBox.GuessedCapital = capitalToGuess;

            playersStatsList.Add(playerStatsBox);

            List<PlayerStats> playerStatsSortedList = playersStatsList.OrderBy(playerStats => playerStats.GuessingTime).ToList();

            if (playerStatsSortedList.Count == 11) playerStatsSortedList.RemoveAt(playerStatsSortedList.Count - 1);

            Console.Write("\n");
            foreach (PlayerStats playerStatsDummy in playerStatsSortedList)
            {
                line = (playerStatsDummy.Name + " | " + playerStatsDummy.Date + " | " + playerStatsDummy.GuessingTime
                + " | " + playerStatsDummy.GuessedLettters + " | " + playerStatsDummy.GuessedCapital);

                Console.WriteLine(line);
                fileToSave.WriteLine(line); //nie kasuje tego, co było wcześniej
            }
            fileToSave.Close();

        }
        private static void DrawHangman(int lifes)
        {
            switch (lifes)
            {
                case 0:
                    Console.WriteLine("  +---+");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("  O   |");
                    Console.WriteLine(@" /|\  |");
                    Console.WriteLine(@" / \  |");
                    Console.WriteLine("      |");
                    Console.WriteLine("=========");
                    break;
                case 1:
                    Console.WriteLine("  +---+");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("  O   |");
                    Console.WriteLine(@" /|\  |");
                    Console.WriteLine(" /    |");
                    Console.WriteLine("      |");
                    Console.WriteLine("=========");
                    break;
                case 2:
                    Console.WriteLine("  +---+");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("  O   |");
                    Console.WriteLine(@" /|\  |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("=========");
                    break;
                case 3:
                    Console.WriteLine("  +---+");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("  O   |");
                    Console.WriteLine(" /|   |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("=========");
                    break;
                case 4:
                    Console.WriteLine("  +---+");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("  O   |");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("=========");
                    break;
                case 5:
                    Console.WriteLine("  +---+");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("  O   |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("=========");
                    break;
                case 6:
                    Console.WriteLine("  +---+");
                    Console.WriteLine("  |   |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("      |");
                    Console.WriteLine("=========");
                    break;
                default:
                    Console.WriteLine("Error");
                    break;
            }
        }
    }
}
