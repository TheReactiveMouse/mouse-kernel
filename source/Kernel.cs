using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using System.Threading;
using System.Timers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading.Tasks;
using System.Data;

namespace mouseKernel
{
    public struct Json
    {
        //public Read(string filename){

        //}        
    }
    
    public struct User
    {
        public User(string name, int permissions)
        {
            Name = name;
            Permissions = permissions;
        }

        public string Name { get; set; }
        public int Permissions { get; set; }
    }

    public struct Machine
    {
        public Machine(string name, OSData data, User owner)
        {
            Name = name;
            System = data;
            Owner = owner;
        }

        public string Name { get; }
        public OSData System { get; }
        public User Owner { get; }
    }

    public struct OSData
    {
        public OSData(string name, Version version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }
        public Version Version { get; set; }
    }

    public struct Version
    {
        public Version(int major, int minor, int spec, int build, int code)
        {
            Major = major;
            Minor = minor;
            Specification = spec;
            Build = build;
            Code = code;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Specification { get; set; }
        public int Build { get; set; }
        public int Code { get; set; }
    }

    public static class Parser
    {
        public static string[] Parse(string str)
        {
            return str.Split(' ');
        }
    }

    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("Loading mouseKernel...");
            // Ну дак пошли разбираться как им пользоваться :/
            Machine = new Machine("ClassicServer",
            new OSData("MouseOS", new Version(0, 0, 0, 0, 1)),
            new User("Administrator", 7));
            // Init done.
            Console.WriteLine("Creating filesystem in memory...");
            data_fs.Add("coreboot.yml");
            data_fs.Add("kernel.bin");
            data_fs.Add("filekernel.pic");
            data_fs.Add("boot.cfg");
            data_fs.Add("kernel.dtx");
            data_fs.Add("license.conf");
            data_fs.Add("boot.conf");
            data_fs.Add("syslinux.conf");
            data_fs.Add("namedata.conf");
            data_fs.Add("kernelconfigurator.mouse");
            data_fs.Add("driver-vga.ngx");
            data_fs.Add("drivers-loader.ngx");
            data_fs.Add("filesystem-loader.ngx");
            data_fs.Add("filesystem.bin");
            Console.WriteLine("Initializating configuration...");
            Console.WriteLine("Ok.");
            uint memory = Cosmos.Core.CPU.GetAmountOfRAM();
            string processor_author = Cosmos.Core.ProcessorInformation.GetVendorName();
            Console.WriteLine("Memory intiailzated as " + MEMORY_ACCEPTED + "MB");
            Console.WriteLine(DateTime.Now);
            string key_private = Convert.ToString(rnd.Next(1024, 2048));
            Console.WriteLine("[mouseFS] mouseFS Is started.. FS Key is" + key_private.GetHashCode());
            Console.WriteLine("[OK] Started Authentication Manager.");
            Console.WriteLine("[OK] Started RAID Service");
            Console.WriteLine("[OK] Started 2Myboot Manager");
            Console.WriteLine("[OK] Started X2Fire");
            Console.WriteLine("[OK] Started XServer");
            Console.WriteLine("[ERROR] XServer not found graphical interface. Required install GUI to use X");
            Console.WriteLine("[OK] Started Firewall");
            Console.WriteLine("[OK] Started HDDManager");
            Console.WriteLine("[OK] Started RAIDController");
            Console.WriteLine("[OK] Started VIRTIO");
            Console.WriteLine("[ERROR] Error, Network driver is not started.");
            Console.WriteLine("[WARN] Nginx server is cannot be loaded");
            Console.Write("Username:");
            username = Console.ReadLine();
            if (username == "root" || username == "mouse")
            {
                Console.Write("Password: "); // Снова тести
                string password = ReadPassword(); // пробуй
                if (password == REAL_ADMIN_PASSWORD) // пашет?
                {
                    Console.WriteLine($"Welcome, {username}!");
                }
                else
                {
                    Console.WriteLine("Incorrect login or password.");
                    BeforeRun();
                }
            }
            else
            {
                username = "guest";
            }
        }

        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }

        protected override void Run()
        {
            Console.Write($"{username}@system~>");
            string[] consoleIn = Console.ReadLine().Split(' ');
            if (consoleIn.Length == 0) return;
            if (consoleIn[0] == "time")
            {
                Console.WriteLine(DateTime.Now);
            }

            if (consoleIn[0] == "del")
            {

                try
                {
                    Console.WriteLine("Searching...");
                    if (data_fs.Contains(consoleIn[1]))
                    {
                        int first_value = rnd.Next(1824,8695);
                        int two_value = rnd.Next(8496,19205);
                        int true_value = first_value + two_value;
                        Console.Write($"{consoleIn[1]} Found! Captcha, {first_value}+{two_value} = ?");
                        string value_accept = Console.ReadLine();
                        if (value_accept == Convert.ToString(true_value) )
                        {
                            Console.WriteLine($"File {consoleIn[1]} deleted");
                            data_fs.Remove(consoleIn[1]);
                        } else
                        {
                            Console.WriteLine("False. True value is " + true_value);
                        }
                    } else
                    {
                        Console.WriteLine("File not found! If you want delete folder, use del.folder");
                    }
                } catch (Exception error)
                {
                    Console.WriteLine("Required arguments.");
                }

            }

            if (consoleIn[0] == "del.folder")
            {
                try
                {
                    Console.WriteLine("Searching...");
                    if (data_fs.Contains($"{consoleIn[1]} [FOLDER]"))
                    {
                        int first_value = rnd.Next(1824, 8695);
                        int two_value = rnd.Next(8496, 19205);
                        int true_value = first_value + two_value;
                        Console.Write($"{consoleIn[1]} Found! Captcha, {first_value}+{two_value} = ?");
                        string value_accept = Console.ReadLine();
                        if (value_accept == Convert.ToString(true_value))
                        {
                            Console.WriteLine($"Folder {consoleIn[1]} deleted");
                            data_fs.Remove($"{consoleIn[1]} [FOLDER]");
                        }
                        else
                        {
                            Console.WriteLine("False. True value is " + true_value);
                        }
                    }
                    else
                    {
                        Console.WriteLine("folder not found! If you want delete file, use del");
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Required arguments.");
                }
            }

            if (consoleIn[0] == "math")
            {
                // Using processor power. Without video calculating.

                string value1 = consoleIn[1];
                string value2 = consoleIn[2];

                if (value1 == "" || value2 == "")
                {
                    Console.WriteLine("[ERROR] Required valid value.");
                }
                else
                {
                    try
                    {
                        long DATA_RESULT_MINUS = Convert.ToInt64(value1) - Convert.ToInt64(value2);
                        long DATA_RESULT_PLUS = Convert.ToInt32(value1) + Convert.ToInt32(value2);
                        long DATA_RESULT_OPERANT_TREE = Convert.ToInt32(value1) / Convert.ToInt32(value2);
                        long DATA_RESULT_DOUBLE_TREE = Convert.ToInt32(value1) * Convert.ToInt32(value2);
                        Console.WriteLine($"{value1} + {value2} = {DATA_RESULT_PLUS}");
                        Console.WriteLine($"{value1} - {value2} = {DATA_RESULT_MINUS}");
                        Console.WriteLine($"{value1} / {value2} = {DATA_RESULT_OPERANT_TREE}");
                        Console.WriteLine($"{value1} * {value2} = {DATA_RESULT_DOUBLE_TREE}");
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine("Required valid value.");
                    }
                }

            }

            if (consoleIn[0] == "mem")
            {
                Console.WriteLine($"{Cosmos.Core.CPU.GetAmountOfRAM() + 1}Mb is available.");
            }

            if (consoleIn[0] == "files")
            {
                foreach (string returned in data_fs)
                {
                    Console.WriteLine(returned);
                }
            }

            if (consoleIn[0] == "help")
            {
                Console.WriteLine("time - Get time now.");
                Console.WriteLine("mem - Memory available now on your pc.");
                Console.WriteLine("files - files is virtual filesystem.");
                Console.WriteLine("mkdir - create new folder");
                Console.WriteLine("touch - new file");
                Console.WriteLine("about - About kernel.");
                Console.WriteLine("power - Working with ACPI Functions. ");
                Console.WriteLine("passwd - change password");
                Console.WriteLine("Logout - Break this session and go to Authentication Manager.");
                Console.WriteLine("request - Mouses function. Requests to mouse render function");

            }

            if (consoleIn[0] == "mkdir")
            {
                try
                {
                    string foldername = consoleIn[1];
                    if (foldername == "" || data_fs.Contains($"{foldername}"))
                    {
                        Console.WriteLine("Error : Required valid name for this operation.");
                    }
                    else
                    {
                        data_fs.Add(foldername + " [FOLDER]");
                        Console.WriteLine("Ok.");
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine($"ERROR : You are using folder name?");
                }
            }

            if (consoleIn[0] == "touch")
            {
                try
                {
                    string filename = consoleIn[1];

                    if (filename == "" || filename.Length < 1 || data_fs.Contains($"{filename}"))
                    {
                        Console.WriteLine("Error : Required valid name for this operation.");
                    }
                    else
                    {
                        Console.WriteLine("File is created successfully.");
                        if (filename.Contains(".txt"))
                        {
                            data_fs.Add($"{filename}");

                        }
                        else
                        {
                            if (filename.Contains(".sql"))
                            {
                                data_fs.Add($"{filename}");
                            }
                            else
                            {
                                data_fs.Add($"{filename}");
                            }
                        }
                    }

                }
                catch (Exception error)
                {
                    Console.WriteLine($"ERROR : You are using file name?");
                }
            }

            if (consoleIn[0] == "about")
            {
                Console.WriteLine("mouseKernel Alpha 1.0 ");
                Console.WriteLine("Authors");
                // хм... щас подумаем...
                Console.WriteLine("Mouse#3040 - Discord");
                Console.WriteLine("sudo#4677 - Discord");
                Console.WriteLine("artem6191#7777 - Discord");
                Console.WriteLine("---------------------------");
                Console.WriteLine("2020. ");
                Console.WriteLine("Made in Almaty by Stas Ivanov.");
                Console.WriteLine("About Stas");
                Console.WriteLine("I am Stas, I am a developer with extensive experience.I started with Basic.It was very interestingBut then I switched to C++, Python, Assembler, now I am to C #. No, I have not switched from Python forever. No. Python is my main programming language and the date the system was created, but on 02/01/2020 I will be 14 years old, but now I'm 13. The system is protected by the GPL. Please don’t scold if there are serious bugs, just report them. You will help us a lot! Thank you");


            }

            if (consoleIn[0] == "power")
            {
                try
                {
                    String operation = consoleIn[1];
                    if (operation == "" || operation.Length == 0)
                    {
                        Console.WriteLine("Error : Required argument.");
                    }
                    else
                    {
                        if (operation == "--help" || operation == "-h")
                        {
                            Console.WriteLine("--off/-off - Power off your server.\n--reboot/-reboot - Reboot your server.");
                        }
                        if (operation == "--off" || operation == "-off")
                        {
                            Cosmos.Core.ACPI.Shutdown();
                        }
                        if (operation == "--reboot" || operation == "-reboot")
                        {
                            Cosmos.Core.ACPI.Reboot();
                        }
                    }
                }
                catch (Exception error_code)
                {
                }
            }
            if (consoleIn[0] == "passwd")
            {
                if (username != "root" || username != "mouse")
                {
                    Console.Write("Currently password:");
                    string current_password = ReadPassword();
                    if (current_password == REAL_ADMIN_PASSWORD)
                    {
                        Console.Write("New password:");
                        string new_pass = ReadPassword();
                        // security
                        if (new_pass.Length < 8 || new_pass == "")
                        {
                            Console.WriteLine("Required password with 8 symbols.It's minimal!");
                        }
                        else
                        {
                            Console.Write("Repeat password:");
                            string new_pass_repeat = ReadPassword();
                            if (new_pass_repeat == new_pass)
                            {
                                Console.WriteLine("Ok.");
                                REAL_ADMIN_PASSWORD = new_pass;
                            }
                            else
                            {
                                Console.WriteLine("New password is not correct. Try again.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("User don't have password currently.");
                }
            }
            if (consoleIn[0] == "logout")
            {
                BeforeRun();
            }

            if (consoleIn[0] == "request")
            {

                // executing
                try
                {
                    String type_request = consoleIn[1];
                    String address = consoleIn[2];
                    String package_size = consoleIn[3];
                    String package_name = consoleIn[4];
                    if (type_request == "MOUSE_TAX_REQUEST")
                    {
                        if (address == "76.0")
                        {
                            if (package_size.Length > 5)
                            {
                                Console.WriteLine("Request error: Size is too many big.");
                            }
                            else
                            {
                                if (package_name == "" || package_name.Length > 5)
                                {
                                    Console.WriteLine("Package type is not supported.Or not entered.");
                                }
                                else
                                {
                                    Console.WriteLine($"Request accept: {package_name.GetHashCode()}.{package_size.GetHashCode()}");
                                }
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Command is need argument.");
                }
            }
        }


        public int hours = 0;
        public string username_login;
        public string username;
        private static System.Timers.Timer sleep_arg;
        public uint MEMORY_ACCEPTED = Cosmos.Core.CPU.GetAmountOfRAM() + 1;
        public string REAL_ADMIN_PASSWORD = "thereactivecheese"; // топ пароль
        List<string> data_fs = new List<string>();
        public string ip;
        public string host;
        public string dns;
        Random rnd = new Random(); // random function
        public Machine Machine { get; private set; }
    }
}