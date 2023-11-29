// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using Open_TK_Tut_1;
using System;






MainMenu();

static bool MainMenu()
{
    Console.Clear();
    Console.WriteLine("Welcome to the Menu!");
    Console.WriteLine("1. Play");
    Console.WriteLine("2. Exit");
    Console.Write("\nEnter your choice: ");

    switch (Console.ReadLine())
    {
        case "1":
           Console.WriteLine("You've selected Option 1!");
           using (var myGame = new Game(800, 800, "Computer Graphics "))
           {
               myGame.Run();
           }

           return true;
        case "2":
            return false;
        default:
            return true;
    }
}



