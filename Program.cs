using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;

namespace TheHangman
{
    class Program
    {

        public struct countryAndCapitalStruct
        {
            public string country { get; set; }
            public string capital { get; set; }
        }

        static void Main(string[] args)
        {
            string line;
            string[] words = new string[2];
            var countryAndCapitalList = new List<countryAndCapitalStruct>();
            var box = new countryAndCapitalStruct();
            ConsoleKeyInfo choice;

            //open file
            System.IO.StreamReader file = new System.IO.StreamReader(@"E:\Projekty\TheHangmanGame\TheHangman\resources\countries_and_capitals.txt");

            while ((line = file.ReadLine()) != null)
            {
                words = line.Split(" | ");               // loading words from file to list
                box.country = words[0];
                box.capital = words[1];
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

        private static void Game(List<countryAndCapitalStruct> countryAndCapitalList)
        {
            bool winGame = false;
            int randomIndex;
            int lifes = 6;
            int lettersLeft = 0;
            string userGuess;
            string capitalToGuess;
            string wrongLetters = "";
            bool[] isLetterGuessed = new bool[countryAndCapitalList.Count];
            ConsoleKeyInfo choice;

            Console.Clear();
            Random rnd = new Random();
            randomIndex = rnd.Next(countryAndCapitalList.Count);                      //  picking a random pair of
            capitalToGuess = countryAndCapitalList.ElementAt(randomIndex).capital;    //  country and it's capitol

            for (int i = 0; i < capitalToGuess.Length; i++)
            {
                if (capitalToGuess[i] == ' ') isLetterGuessed[i] = true;              // setting starting flags
                else isLetterGuessed[i] = false;
            }

            while (lifes > 0)
            {
                CreateGameInterface(lifes, lettersLeft, capitalToGuess, isLetterGuessed, wrongLetters);

                do
                {
                    choice = Console.ReadKey(true);
                    if (choice.KeyChar == '1')
                    {
                        Console.WriteLine("Podaj litere");
                        userGuess = Console.ReadLine();
                        if (userGuess.Length != 1) continue;

                        if (checkLetter(userGuess[0], capitalToGuess, isLetterGuessed, ref wrongLetters))
                        {
                            Console.WriteLine("Well done!");
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Console.WriteLine("Wrong letter!");
                            Thread.Sleep(2000);
                            lifes--;
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
                            lifes--;
                        }
                    }
                } while (choice.KeyChar != '1' && choice.KeyChar != '2');

                if (winGame)
                {
                    Console.Clear();
                    Console.WriteLine("YOU WON!!!!");
                    Console.WriteLine("Wait to play again");
                    Thread.Sleep(5000);
                    break;
                }
            }
        }

        private static void CreateGameInterface(int lifes, int lettersLeft, string capitalToGuess, bool[] isLetterGuessed, string wrongLetters)
        {
            int counter = 0;

            Console.Clear();
            Console.WriteLine(capitalToGuess);
            Console.WriteLine("Press 1 to guess a letter, press 2 to guess whole word");
            Console.WriteLine("zostało znaków: " + lettersLeft);
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
            Console.Write("\n\n");
            drawHangman(lifes);

        }

        private static bool checkLetter(char letter, string word, bool[] isLetterGuessed, ref string wrongLetters)
        {
            bool correct = false;
            for (int i = 0; i < word.Length; i++)
            {
                if (char.ToLower(letter) == char.ToLower(word[i]))   
                    {
                    correct = true;
                    isLetterGuessed[i] = true;
                }
            }
            if (correct) return true;
            else
            {
                wrongLetters += letter;
                return false;
            }

        }

        private static void drawHangman(int lifes)
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
