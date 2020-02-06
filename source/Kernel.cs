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
using Cosmos.System.Graphics;
using Cosmos.HAL;
using Cosmos.HAL.Drivers.PCI.Network;
using Cosmos.System.FileSystem;

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
        public static string PhysicalAddress()
        {
            PCIDevice device;
            device = PCI.GetDevice(VendorID.AMD, DeviceID.PCNETII);
            if (NetworkCardAvailable())
            {
                AMDPCNetII nic = new AMDPCNetII(device);
                return nic.MACAddress.ToString();
            }
            else
            {
                return "";
            }
        }
        public static bool NetworkCardAvailable()
        {
            PCIDevice device;
            device = PCI.GetDevice(VendorID.AMD, DeviceID.PCNETII);
            if (device != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public bool fs_initializated = false;
        protected override void BeforeRun()
        {
            if (fs_initializated == true)
            {
                Console.WriteLine("Filesystem is already initializated! Skipping..");
            }
            else
            {
                try
                {
                    Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(vFS);
                    bool fs_initializated = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Filesystem is already initializated! Skipping..");
                }
            }
            Console.Clear();
            Console.WriteLine("Loading mouseKernel...");
            processes.Add("System");
            processes.Add("core");
            processes.Add("bootloader");
            system_processes.Add("System");
            system_processes.Add("core");
            system_processes.Add("bootloader");
            vFS.Initialize();
            int disk_id = 0;
            Cosmos.HAL.Drivers.PCI.Video.VMWareSVGAII.Register DRIVER_VIDEO;
            Cosmos.HAL.Drivers.USB.USBHost USB_dRIVER;
            // Ну дак пошли разбираться как им пользоваться :/
            Machine = new Machine("ClassicServer",
            new OSData("MouseOS", new Version(0, 0, 0, 0, 1)),
            new User("Administrator", 7));
            Console.WriteLine("Initializating configuration...");
            Console.WriteLine("Ok.");
            uint memory = Cosmos.Core.CPU.GetAmountOfRAM();
            string processor_author = Cosmos.Core.ProcessorInformation.GetVendorName();
            Console.WriteLine("Memory intiailzated as " + MEMORY_ACCEPTED + "MB");
            Console.WriteLine(DateTime.Now);
            string key_private = Convert.ToString(rnd.Next(1024, 2048));
            Console.WriteLine("[OK] Started Authentication Manager.");
            Cosmos.HAL.PCSpeaker.Beep(100, 1000);
            Console.WriteLine("[OK] Started RAID Service");
            Console.WriteLine("[OK] Started 2Myboot Manager");
            Cosmos.HAL.PCSpeaker.Beep(100, 1000);
            Console.WriteLine("[OK] Started X2Fire");
            Cosmos.HAL.PCSpeaker.Beep(100, 1000);
            Console.WriteLine("[OK] Started XServer");
            Cosmos.HAL.PCSpeaker.Beep(100, 1000);
            Console.WriteLine("[ERROR] XServer not found graphical interface. Required install GUI to use X");
            Console.WriteLine("[OK] Started Firewall");
            Cosmos.Core.ACPI.Enable();
            Cosmos.Core.Global.CPU.InitSSE();
            Cosmos.Core.Global.CPU.InitFloat();
            Cosmos.Core.Global.CPU.GetType();
            Cosmos.Core.IOPort.Wait();
            string[] devices = { "Unknown Monitor",$"Default Hard Drive",$"Processor {Cosmos.Core.ProcessorInformation.GetVendorName()}",$"Memory {Cosmos.Core.CPU.GetAmountOfRAM()}MB" };
            int indentificated_devices = 0;
            while (indentificated_devices != devices.Length )
            {
                Console.WriteLine(devices[indentificated_devices]);
                indentificated_devices++;
            }
            Console.WriteLine("[OK] Started HDDManager");
            Console.WriteLine("[OK] Started RAIDController");
            Console.Beep();
            Cosmos.System.PCSpeaker.Beep(300, 500);
            Cosmos.HAL.PCSpeaker.Beep(100, 1000);
            Console.WriteLine("[OK] Started VIRTIO");
            Console.WriteLine(PhysicalAddress());
            Console.WriteLine("[OK] NET Driver is loaded!");
            Cosmos.HAL.PCSpeaker.Beep(100, 1000);
            Console.WriteLine("[WARN] Nginx server is cannot be loaded");
            Console.Write("Username:");
            username = Console.ReadLine();
            if (username == "root" || username == "mouse")
            {
                Console.Write("Password:"); // Снова тести
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
            try
            {
                string dat_fs_now = Directory.GetCurrentDirectory();
                Console.Write($"{username}@system[{dat_fs_now.Replace("0:\\","")}]$");
                string[] consoleIn = Console.ReadLine().Split(' ');
                if (consoleIn.Length == 0) return;

                if (consoleIn[0] == "ls")
                {

                    string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory());
                    string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
                    if (directories.Length > 0)
                    {
                        for (int i = 0; i < directories.Length; i++)
                        {
                            Console.WriteLine($"{directories[i]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not directories found");
                    }
                    if (files.Length > 0)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            Console.WriteLine($"{files[i]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not files found");
                    }
                }

                if (consoleIn[0] == "touch")
                {
                    try
                    {
                        File.Create($"{Directory.GetCurrentDirectory()}\\{consoleIn[1]}");
                        string text_filter = consoleIn[2].Replace("\\n", "\n");
                        File.WriteAllText($"{Directory.GetCurrentDirectory()}\\{consoleIn[1]}", $"{text_filter}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Try other filename, check file contents or restart system.");
                    }
                }


                if (consoleIn[0] == "mkdir")
                {
                    try
                    {
                        if (Directory.Exists($"{Directory.GetCurrentDirectory()}\\{consoleIn[1]}"))
                        {
                            Console.WriteLine("Folder is already created!!!");
                        }
                        else
                        {
                            Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\{consoleIn[1]}");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Try other foldername or restart system.");
                    }
                }

                if (consoleIn[0] == "cd")
                {
                    try
                    {
                        if (Directory.Exists($"{consoleIn[1]}"))
                        {
                            Directory.SetCurrentDirectory($"{Directory.GetCurrentDirectory()}\\{consoleIn[1]}");
                        }
                        else
                        {
                            if (consoleIn[1] == "..")
                            {
                                Directory.SetCurrentDirectory($"0:\\");
                            }
                            else
                            {
                                Console.WriteLine("Directory not created.");
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                if (consoleIn[0] == "del")
                {
                    string directory = Directory.GetCurrentDirectory();
                    if (File.Exists($"{directory}\\" + consoleIn[1]))
                    {
                        File.WriteAllText($"{directory}\\" + consoleIn[1], "");
                        File.Delete($"{directory}\\" + consoleIn[1]);
                    } else
                    {
                        Console.WriteLine("[ WARNING ] While deleting folder, check folder contents.If folder have other folders or files.Delete folders and files and try again if folder not deleted. ");
                        Directory.Delete($"{directory}\\" + consoleIn[1]);
                    }
                }
                if (consoleIn[0] == "echo")
                {
                    string[] text = consoleIn[1].Split(' ');
                    int lines = 0;
                    while (lines != text.Length)
                    {
                        Console.WriteLine(text[lines]);
                        lines++;
                    }
                }
                if (consoleIn[0] == "cat")
                {
                    if (File.Exists($"{Directory.GetCurrentDirectory()}{consoleIn[1]}"))
                    {
                        File.OpenRead($"{Directory.GetCurrentDirectory()}{consoleIn[1]}");
                        Console.WriteLine( $"{File.ReadAllText($"{Directory.GetCurrentDirectory()}{consoleIn[1]}")}" );
                    }
                }

                if ( consoleIn[0] == "make")
                {
                    if (consoleIn[1] == " " || consoleIn[1].Contains(" "))
                    {
                        Console.WriteLine("I can't build Air.I'm not Linux to do this.");
                    } else
                    {
                        if ( File.Exists($"{Directory.GetCurrentDirectory()}\\{consoleIn[1]}"))
                        {
                            Console.WriteLine("Compilling...");
                            File.OpenRead($"{Directory.GetCurrentDirectory()}{consoleIn[1]}");
                            string[] file_contents = File.ReadAllLines($"{Directory.GetCurrentDirectory()}{consoleIn[1]}");
                            int lines_done = 0;
                            while (lines_done != file_contents.Length)
                            {
                                if (file_contents[lines_done] == "ver")
                                {
                                    Console.WriteLine("1.0");
                                }
                                if (file_contents[lines_done] == "uname")
                                {
                                    Console.WriteLine("mouseKernel-Alpha-1.1 Build 06022020");
                                }
                                if (file_contents[lines_done] == "time")
                                {
                                    Console.WriteLine(DateTime.Now);
                                }
                                lines_done++;
                            }
                        }
                    }
                }

                if ( consoleIn[0] == "top")
                {
                    while (true) {
                        Console.Write("You want view processes?");
                        string key = Console.ReadLine();
                        if (key == "Y" || key == "y")
                        {
                            Console.WriteLine("[ Process ] [ User ]");
                            int PROCESSES_INJ = 0;
                            while (PROCESSES_INJ != processes.Count)
                            {
                                Console.WriteLine($"[PROCESS NAME : {processes[PROCESSES_INJ]}] [USER : {username}]");
                                PROCESSES_INJ++;
                            }
                        } else
                        {
                            break;
                        }
                    }
                }

                if ( consoleIn[0] == "kill")
                {
                    if ( processes.Contains(consoleIn[1]))
                    {
                        if (system_processes.Contains(consoleIn[1]))
                        {
                            Console.WriteLine("This is system process. Cannot be killed.");
                        }
                        else
                        {
                            processes.Remove(consoleIn[1]);
                            Console.WriteLine($"{consoleIn[1]} Successfully killed from memory.");
                        }
                    } else
                    {
                        Console.WriteLine("Process not found!");
                    }
                }

                if (consoleIn[0] == "uname")
                {
                    Console.WriteLine("mouseKernel-Alpha-1.1 Build 06022020");
                    processes.Add("uname");
                }

                if (consoleIn[0] == "sound[test]")
                {
                    int tester_sound = 36;
                    while (tester_sound != 1000)
                    {
                        Cosmos.System.PCSpeaker.Beep(Convert.ToUInt16(tester_sound), 500);
                        tester_sound++;
                    }
                }

                if (consoleIn[0] == "reboot")
                {
                    Cosmos.System.Power.Reboot();
                }
                if (consoleIn[0] == "shutdown")
                {
                    Cosmos.System.Power.Shutdown();
                }
                if (consoleIn[0] == "bungeecore")
                {
                    Console.WriteLine("Bungee Core [ 1.0 ]");
                }

                if (consoleIn[0] == "help")
                {
                    Console.WriteLine("uname - Version");
                    Console.WriteLine("help - This list");
                    Console.WriteLine("shutdown - Shutdown your Server");
                    Console.WriteLine("reboot - Reboot your server");
                    Console.WriteLine("sound[test] - Test your sound card! In development.");
                    Console.WriteLine("make [command, example config] - build software");
                    Console.WriteLine("touch [filename] [contents without space]- Create file!");
                    Console.WriteLine("echo [contents without space] - Text!");
                    Console.WriteLine("ls - List of folders and files.");
                    Console.WriteLine("mkdir [foldername] - create folder");
                    Console.WriteLine("cd [foldername] - Change directory to...");
                }

                if (consoleIn[0] == "math")
                {
                    string value1 = consoleIn[1];
                    string value2 = consoleIn[2];
                    if ( value1 == "" || value2 == "")
                    {
                        Console.WriteLine("Required values! Example, 4+5");
                    }
                    else
                    {
                        try
                        {
                            int fix = Convert.ToInt32(value1) + Convert.ToInt32(value2);
                            int fix2 = Convert.ToInt32(value1) - Convert.ToInt32(value2);
                            int fix3 = Convert.ToInt32(value1) / Convert.ToInt32(value2);
                            int fix4 = Convert.ToInt32(value1) * Convert.ToInt32(value2);
                            Console.WriteLine($"{value1}+{value2}={fix}");
                            Console.WriteLine($"{value1}-{value2}={fix2}");
                            Console.WriteLine($"{value1}/{value2}={fix3}");
                            Console.WriteLine($"{value1}*{value2}={fix4}");
                        } 
                        catch (Exception)
                        {
                            int x = 0;
                        }
                    }
                }
                if (consoleIn[0] == "clear")
                {
                    Console.Clear();
                }

            } catch (Exception CRASH_CODE)
            {
                Console.WriteLine("");
            }
        }
        public string username_login;
        public string username;
        private static System.Timers.Timer sleep_arg;
        public uint MEMORY_ACCEPTED = Cosmos.Core.CPU.GetAmountOfRAM() + 1;
        public string REAL_ADMIN_PASSWORD = "admin"; // топ пароль
        List<string> processes = new List<string>();
        List<string> system_processes = new List<string>();
        string data_files;
        List<string> services = new List<string>();
        public static CosmosVFS vFS = new CosmosVFS();
        Random rnd = new Random(); // random function
        public Machine Machine { get; private set; }
    }
}