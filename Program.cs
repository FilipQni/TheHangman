using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;

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
            string line;
            string[] words;
            var countryAndCapitalList = new List<CountryAndCapitalStruct>();
            var box = new CountryAndCapitalStruct();
            ConsoleKeyInfo choice;

            //open files
            System.IO.StreamReader fileWithCapitals = new System.IO.StreamReader(@"E:\Projekty\TheHangmanGame\TheHangman\resources\countries_and_capitals.txt");
            System.IO.StreamReader fileWithTop10 = new System.IO.StreamReader(@"E:\Projekty\TheHangmanGame\TheHangman\resources\top10.txt");

            while ((line = fileWithCapitals.ReadLine()) != null)
            {
                words = line.Split(" | ");                                              // loading words from file to list
                box.Country = words[0];
                box.Capital = words[1];
                countryAndCapitalList.Add(box);
            }

            Console.WriteLine("The game Hangman");
            Console.WriteLine("\nWould you like to start? (press s to start, e to exit)");

            do
            {
                choice = Console.ReadKey(true);
                if (choice.KeyChar == 's' ^ choice.KeyChar == 'S') Game(countryAndCapitalList);   //start menu
            } while (choice.KeyChar != 'e' && choice.KeyChar != 'E');
        }

        private static void Game(List<CountryAndCapitalStruct> countryAndCapitalList)
        {
            bool winGame = false;
            int randomIndex;
            int lifes = 6;
            int guessedLettersNumber = 0;
            string userGuess;
            string capitalToGuess;
            string wrongLetters = "";
            bool[] isLetterGuessed;
            ConsoleKeyInfo choice;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeSpan;

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
                CreateGameInterface(lifes, capitalToGuess, isLetterGuessed, wrongLetters);

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
                            Console.WriteLine("Well done!");
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Console.WriteLine("Wrong letter!");
                            Thread.Sleep(2000);
                        }
                    }
                    if (choice.KeyChar == '2')
                    {
                        Console.WriteLine("Write a capital");
                        userGuess = Console.ReadLine();
                        if (userGuess.Length < 2) continue;

                        if (String.Compare(capitalToGuess, userGuess, true) == 0)
                        {
                            Console.WriteLine("Well done!");
                            Thread.Sleep(2000);
                            winGame = true;
                        }
                        else
                        {
                            Console.WriteLine("Wrong capital!");
                            Thread.Sleep(2000);
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
                Console.WriteLine("Your time: " + timeSpan.Seconds + " seconds");
            }

            Console.WriteLine("\nWould you like to start? (press s to start, e to exit)");
        }

        private static void CreateGameInterface(int lifes, string capitalToGuess, bool[] isLetterGuessed, string wrongLetters)
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
            DrawHangman(lifes);
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
