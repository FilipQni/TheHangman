using System;
using System.Collections.Generic;

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
            
            System.IO.StreamReader file = new System.IO.StreamReader(@"E:\Projekty\TheHangmanGame\TheHangman\resources\countries_and_capitals.txt");

            while ((line = file.ReadLine()) != null)
            {
                words = line.Split('|');
                box.country = words[0];
                box.capital = words[1];
                countryAndCapitalList.Add(box);
            }
 
            Console.WriteLine("The game Hangman");
            Console.WriteLine("\nWould you like to start? (press s to start, e to exit)");
            do
            {
                choice = Console.ReadKey(true);
                if (choice.KeyChar == 's' ^ choice.KeyChar == 'S') round();
            } while (choice.KeyChar != 'e' && choice.KeyChar != 'E'); 

        }

        private static void round()
        {
            Console.Clear();
            Random rnd = new Random();
            int randomIndex;
            //randomIndex = rnd.Next(countryAndCapitalList.Count);
        }



    }
}
