using InstagramHTMLParser;
using System;
using System.IO;

namespace TechShare.CLI.Instagram.Return.HTML
{
    class Program
    {
        static void Main(string[] args)
        {
            InstagramHTMLParse pm = new InstagramHTMLParse(Properties.Settings.Default.DefaultDirectory);
            WriteLogo();
            Console.WriteLine("Enter or Copy (ctrl-c) & Paste (ctrl-v) the path for the Zip file:");
            if (true)
            {
                if (args.Length == 0)
                {
                    string line = Console.ReadLine();
                    if (line == "exit") // Check string
                    {
                        //   break;
                    }
                    if (!File.Exists(line))
                    {
                        System.Console.WriteLine("File not found");
                    }
                    else
                    {
                        string command = line;
                        if (command == "-help")
                        {
                            Console.WriteLine(".Social Help");
                            Console.WriteLine("_____________________________________________");
                            Console.WriteLine(@"1- Path of File ex: C:\example\report.zip");
                            Console.WriteLine("_____________________________________________");
                        }
                        else
                        {
                            String path = command;
                            String CLF = pm.ParseInstagramHTMLExtract(path);
                        }
                    }
                }
                else if (args.Length == 1)
                {
                    string command = args[0];
                    if (command == "-help")
                    {
                        Console.WriteLine(".Social Help");
                        Console.WriteLine("_____________________________________________");
                        Console.WriteLine(@"1- Path of File ex: C:\example\report.zip");
                        Console.WriteLine("_____________________________________________");
                    }
                    else
                    {
                        String path = command;
                        String CLF = pm.ParseInstagramHTMLExtract(path);

                    }

                }
#if DEBUG
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
#endif
            }
        }
        public static void WriteLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("         SSSSSSSSSSSSSSSS                                        iiii                    lllllll  ");
            Console.WriteLine("        SS:::::::::::::::S                                      i::::i                   l:::::l ");
            Console.WriteLine("       S:::::SSSSSS::::::S                                       iiii                    l:::::l ");
            Console.WriteLine("       S:::::S     SSSSSSS                                                               l:::::l ");
            Console.WriteLine("       S:::::S               ooooooooooo       cccccccccccccccc iiiiii   aaaaaaaaaaaaa    l::::l ");
            Console.WriteLine("       S:::::S             oo:::::::::::oo   cc:::::::::::::::c i::::i   a::::::::::::a   l::::l ");
            Console.WriteLine("        S::::SSSS         o:::::::::::::::o c:::::::::::::::::c i::::i   aaaaaaaaa:::::a  l::::l ");
            Console.WriteLine("         SS::::::SSSSS    o:::::ooooo:::::oc:::::::cccccc:::::c i::::i            a::::a  l::::l ");
            Console.WriteLine("           SSS::::::::SS  o::::o     o::::oc::::::c     ccccccc i::::i     aaaaaaa:::::a  l::::l ");
            Console.WriteLine("              SSSSSS::::S o::::o     o::::oc:::::c              i::::i   aa::::::::::::a  l::::l ");
            Console.WriteLine("                   S:::::So::::o     o::::oc:::::c              i::::i  a::::aaaa::::::a  l::::l ");
            Console.WriteLine("                   S:::::So::::o     o::::oc::::::c     ccccccc i::::i a::::a    a:::::a  l::::l ");
            Console.WriteLine("       SSSSSSS     S:::::So:::::ooooo:::::oc:::::::cccccc:::::c i::::i a::::a    a:::::a  l::::l");
            Console.WriteLine("      S::::::SSSSSSS:::::So:::::::::::::::oc::::::::::::::::::c i::::i a::::aaaaaa:::::a  l::::l");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@@@@@ ");
            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine("S:::::::::::::::::S  oo:::::::::::oo  cc:::::::::::::::c  i::::i a::::::::::aa:::a  l::::l");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("@@@@@@ "); Console.ForegroundColor =
            ConsoleColor.Cyan; Console.WriteLine("SSSSSSSSSSSSSSSSS    ooooooooooooo     ccccccccccccccc   iiiiii  aaaaaaaaaa  aaaa  llllll");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("    ██╗███╗   ██╗███████╗████████╗ █████╗  ██████╗ ██████╗  █████╗ ███╗   ███╗");
            Console.WriteLine("    ██║████╗  ██║██╔════╝╚══██╔══╝██╔══██╗██╔════╝ ██╔══██╗██╔══██╗████╗ ████║");
            Console.WriteLine("    ██║██╔██╗ ██║███████╗   ██║   ███████║██║  ███╗██████╔╝███████║██╔████╔██║");
            Console.WriteLine("    ██║██║╚██╗██║╚════██║   ██║   ██╔══██║██║   ██║██╔══██╗██╔══██║██║╚██╔╝██║");
            Console.WriteLine("    ██║██║ ╚████║███████║   ██║   ██║  ██║╚██████╔╝██║  ██║██║  ██║██║ ╚═╝ ██║");
            Console.WriteLine("    ╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" DISTRIBUTED BY THE NDCAC");
            Console.ForegroundColor = ConsoleColor.Gray; Console.WriteLine("_________________________________________________________________________________________________");
            Console.ForegroundColor = ConsoleColor.White;

        }
    }
}
