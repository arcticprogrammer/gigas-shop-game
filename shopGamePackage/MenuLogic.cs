using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ShopGameApp
{
    class MenuLogic
    {
        private const String DIRECTORY_PATH_WITH_ENVIRONMENT = @"%USERPROFILE%\Documents\My Games\Shop Game";
        private static String DIRECTORY_PATH = Environment.ExpandEnvironmentVariables(DIRECTORY_PATH_WITH_ENVIRONMENT);

        public static void mainMenu()
        {
            String directoryPath = DIRECTORY_PATH;
            //check if save directory exists
            // > if does not exist, create save directory
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            Console.WriteLine("\n====SHOP GAME====");
            Console.WriteLine
                ("1. New Shop\n"
                +"2. Load a Shop\n"
                +"3. Delete Shop\n"
                +"4. Exit\n");

            String input = "";
            bool validInput;
            int i = 0;

            do
            {
                validInput = false;
                Console.Write("Choice: ");
                input = Console.ReadLine();
                validInput = checkInputValidityInt(input);

                if (validInput)
                {
                    i = Convert.ToInt32(input);
                    switch (i)
                    {
                        case 1:
                            createNewShop();
                            break;

                        case 2:
                            loadExistingShop();
                            break;

                        case 3:
                            deleteExistingShop();
                            break;

                        case 4:
                            exitGame();
                            break;

                        default:
                            Console.WriteLine("Please enter a number listed on the menu.");
                            validInput = false;
                            break;
                    }
                }

            } while (validInput == false);
        }

        public static Boolean createNewShop()
        {
            String directoryPath = DIRECTORY_PATH;
            Console.WriteLine("\n=====CREATE A NEW SHOP=====");

            Console.Write("Please enter your new shop's name \n" +
                "(Type 'x' to cancel): ");
            String shopName = Console.ReadLine();

            if (shopName.Equals("x"))
            {
                Console.WriteLine("Cancelled creating a new shop.");
                mainMenu();
                //return false for method abort
                return false;
            }

            // > otherwise, designate save directory
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            
            //read savefile with largest number
            //if the directory is empty, use default file number "0000"
            String saveNumber = "";
            if (Directory.GetDirectories(directoryPath).Length == 0)
            {
                saveNumber = "0000";
            }
            else
            {
                foreach (var save in dirInfo.GetDirectories())
                {
                    saveNumber = save.Name;
                }
            }

            //convert fileNumber to int newFileNumber, add 1 to it
            int newSaveNumberInt = int.Parse(saveNumber);
            newSaveNumberInt++;

            //create new save file
            Console.WriteLine("Creating new directory for save.");
            String newSaveNumber = newSaveNumberInt.ToString("0000");

            String filePath = Path.Combine(directoryPath, newSaveNumber);
            
            //create the numbered save dir
            Directory.CreateDirectory(filePath);

            Console.WriteLine("Writing new data.");
            String[] lines = { shopName, "20", "250000" };
            
            //write data to individual files
            File.WriteAllLines(Path.Combine(filePath, "shop"), lines); 
            File.CreateText(Path.Combine(filePath, "storage")); 
            File.CreateText(Path.Combine(filePath, "display")); 
            
            Console.WriteLine("Successfully wrote new save.");
            //return true for method completion
            return true;
        }

        public static Boolean loadExistingShop()
        {
            Console.WriteLine("\n=====LOAD A SHOP=====");

            //get saveNumberList and display dir contents
            ArrayList saveList = getAndDisplaySaveDirContents();

            if (saveList.Count == 0)
            {
                Console.WriteLine("There are no saves in here...");
                mainMenu();

                //return false for method abort
                return false;
            }

            bool validInput = false;
            String requestedSaveNumber = "";
            do
            {
                //get file number through user input
                Console.WriteLine();
                Console.Write("Which file do you want to load? (Enter file number) \n" +
                    "(Type 'x' to cancel): ");
                String input = Console.ReadLine();

                if (input.Equals("x"))
                {
                    Console.WriteLine("Going back to the main menu...");
                    mainMenu();
                    //return false for method abort
                    return false;
                }

                validInput = checkInputValidityInt(input);

                //if validInput is true, check that the string is 4 characters long
                if (validInput == true)
                {
                    //if string.length is < 4, add "0" to the front until it is 4 characters long
                    if (input.Length < 4)
                    {
                        do { input = "0" + input; } while (input.Length < 4);
                    }

                    //if validInput is true and input is 4 characters long, save to requestedSaveNumber
                    requestedSaveNumber = input;
                }
                
            } while (validInput == false);

            bool saveExists = false;
            foreach (String s in saveList)
            {
                if (s.Equals(requestedSaveNumber))
                {
                    Console.WriteLine("Save " + s + " found!");
                    saveExists = true;
                    break;
                }
            }

            if (saveExists == false)
            {
                Console.WriteLine("Save " + requestedSaveNumber + " not found!");
                Console.WriteLine("Returning to load screen.");
                Console.WriteLine("=========================");
                loadExistingShop();
            } else
            {
                //load shop data
                loadShopData(requestedSaveNumber);
            }

            //return true for method completion
            return true;
        }

        public static Boolean deleteExistingShop()
        {
            Console.WriteLine();
            Console.WriteLine("=====DELETE A SHOP=====");

            //get save list and display dir contents
            ArrayList saveList = getAndDisplaySaveDirContents();

            if (saveList.Count == 0)
            {
                Console.WriteLine("There are no saves in here...");
                mainMenu();

                //return false for method abort
                return false;
            }

            bool validInput = false;
            String requestedSaveNumber = "";

            do
            {
                //get file number through user input
                Console.WriteLine();
                Console.Write("Which file do you want to delete? (Enter file number) \n" +
                    "(Type 'x' to cancel): ");
                String input = Console.ReadLine();

                if (input.Equals("x"))
                {
                    Console.WriteLine("Going back to the main menu...");
                    mainMenu();
                    //return false for method abort
                    return false;
                }

                validInput = checkInputValidityInt(input);

                //if validInput is true, check that the string is 4 characters long
                if (validInput == true)
                {
                    //if string.length is < 4, add "0" to the front until it is 4 characters long
                    if (input.Length < 4)
                    {
                        do { input = "0" + input; } while (input.Length < 4);
                    }

                    //if validInput is true and input is 4 characters long, save to requestedSaveNumber
                    requestedSaveNumber = input;
                }

            } while (validInput == false);

            bool saveExists = false;
            foreach (String s in saveList)
            {
                if (s.Equals(requestedSaveNumber))
                {
                    //Console.WriteLine("Save " + s + " found!");
                    saveExists = true;
                    break;
                }
            }

            if (saveExists == false)
            {
                Console.WriteLine("Save " + requestedSaveNumber + " not found!");
                Console.WriteLine("Returning to deletion screen.");
                Console.WriteLine("=============================\n");
                deleteExistingShop();
            }
            else
            {
                //delete shop data
                deleteShopData(requestedSaveNumber);
            }

            //return true for method completion
            return true;
        }

        public static Boolean exitGame()
        {
            Console.WriteLine("See you again soon! (^u^)/");
            Console.ReadKey();
            Environment.Exit(0);

            //return true for method completion (even though it'll never get to this point)
            return true;
        }

        
        //---QOL methods---//
        public static bool checkInputValidityInt(String input)
        {
            //check if the string is empty
            if (String.IsNullOrEmpty(input))
            {
                Console.WriteLine("You forgot to put a number in!");
                return false;
            }

            //check each character in input
            //if any character is not a number, return false
            //if no character is not a number (i.e. all characters are numbers), return true
            foreach (char c in input)
            {
                if (!Char.IsDigit(c))
                {
                    Console.WriteLine("Unless my eyes decieve me, that's not a number!");
                    return false;
                }
            }
            return true;
        }

        public static ArrayList getAndDisplaySaveDirContents()
        {
            String directoryPath = DIRECTORY_PATH;
            ArrayList saveNumberList = new ArrayList();
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            
            // load save numbers to saveNumberList
            foreach (var save in dirInfo.GetDirectories())
            {
                saveNumberList.Add(save.Name);
            }

            //display contents of saveNumberList
            // look into linq implementation:
            //  >   using System.Linq;
            //  >   string line1 = File.ReadLines("filepath").First();
            for (int i = 0; i < saveNumberList.Count; i++)
            {
                String[] contents = File.ReadAllLines(directoryPath + "\\" + saveNumberList[i] + "\\shop");
                String saveName = contents[0];
                Console.WriteLine(saveNumberList[i] + " " + saveName);
            }

            return saveNumberList;
        }

        public static Shop loadShopData(String s)
        {
            //Console.WriteLine("DEBUG: Called loadShopData(s)");
            
            //add string s to directory file path
            String savePath = Path.Combine(DIRECTORY_PATH, s);

            //get string[] array of lines in the save
            string[] lines = File.ReadAllLines(Path.Combine(savePath, "shop"));

            //create shop object
            Shop shopObject = new Shop();
            shopObject.ShopName = lines[0];
            shopObject.ShopSize = int.Parse(lines[1]);
            shopObject.ShopFunds = int.Parse(lines[2]);

            return shopObject;

        }

        public static Boolean deleteShopData(String s)
        {
            String input = "";
            bool validInput = false;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Are you sure you want to delete this save? (y/n) ");
                Console.Write("(Type 'x' to cancel): ");
                input = Console.ReadLine();
                Console.WriteLine();

                if (input.Equals("x"))
                {
                    Console.WriteLine("Returning to main menu.");
                    mainMenu();
                    //return false for method abort
                    return false;
                }

                if (input.Equals("y"))
                {
                    //get save details
                    String savePath = Path.Combine(DIRECTORY_PATH, s);
                    Shop shopObject = loadShopData(s);

                    //display save details
                    Console.WriteLine();
                    Console.WriteLine("Shop Name: " + shopObject.ShopName);
                    Console.WriteLine("Shop Funds: " + shopObject.ShopFunds + " G");
                    Console.WriteLine();

                    //double confirm deletion
                    Console.WriteLine("Are you ABSOLUTELY SURE you want to delete this? If so, write the shop's name.");
                    Console.Write("(Type anything else to cancel): ");
                    input = Console.ReadLine();
                    if (input.Equals(shopObject.ShopName))
                    {
                        Directory.Delete(savePath, true);
                        Console.WriteLine("Shop has been deleted.");
                        Console.WriteLine();
                        validInput = true;

                    } else
                    {
                        Console.WriteLine("Your hesitation has been noted.");
                        Console.WriteLine();
                        validInput = true;
                    }
                } else if (input.Equals("n"))
                {
                    Console.WriteLine("Going back a step...");
                    Console.WriteLine();
                    validInput = true;
                    deleteExistingShop();
                } else
                {
                    Console.WriteLine("Invalid input. Please try again!");
                    Console.WriteLine();
                }

            } while (validInput == false);

            Console.ReadKey();
            Console.WriteLine("Returning to main menu.");
            mainMenu();

            //return true for method completion
            return true;
        }

    }
}
