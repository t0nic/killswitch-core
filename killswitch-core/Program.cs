/*
 * 
 * 
 * 
 * 
 * 
 *  KillSwitch-Core, a cross platform VPN protection system written in C# using the .NET Core framework.
 *      code written by t0nic, the og over at uncle/b/.
 *      if you have any questions just message me in irc.deadnet.org #graveyard
 *      
 *      firt written in .NET Framework, now optimized for .NET Core and compiled for all operating systems.     
 *      
 *      uses:
 *         
 * 
 */

using System;
using System.Collections.Genereenric;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace killswitch_core
{
    class KSCore
    {

        // general killswitch stuff
        private const string assLogo =@"
*********The Cross Platform VPN Kill Switch******************************
██╗**██╗██╗██╗*****██╗*****███████╗██╗****██╗██╗████████╗*██████╗██╗**██╗
██║*██╔╝██║██║*****██║*****██╔════╝██║****██║██║╚══██╔══╝██╔════╝██║**██║
█████╔╝*██║██║*****██║*****███████╗██║*█╗*██║██║***██║***██║*****███████║
██╔═██╗*██║██║*****██║*****╚════██║██║███╗██║██║***██║***██║*****██╔══██║
██║**██╗██║███████╗███████╗███████║╚███╔███╔╝██║***██║***╚██████╗██║**██║
╚═╝**╚═╝╚═╝╚══════╝╚══════╝╚══════╝*╚══╝╚══╝*╚═╝***╚═╝****╚═════╝╚═╝**╚═╝
*****************keeping all the k00l kids safe**************************
*************************************************************************";
        private static string vpnPath;
        private static bool isWindows;
        private static bool isLinux;
        private static bool shouldReset;

        // firewall helper
        private static string resetCmd;
        private static string blockCmd;
        private static string enableCmd;

        // input handler
        private string input;
        private bool killHandler;

        static void Main(string[] args)
        {
            isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            blockCmd = "/C netsh advfirewall firewall add rule name=\"Killswitch\" dir=out action=block";
            enableCmd = "/C netsh advfirewall firewall set rule name=\"Killswitch\" new enable=yes";
            resetCmd = "/C netsh advfirewall firewall set rule name=\"Killswitch\" new enable=no";
            InputStart();
            KillSwitchStart(0);
            
        }

        // this activates the listener/event
        static void KillSwitchStart(int mode)
        {
            if (mode == 0)
            {
                NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
            }
            else
            {
            }

        }

        // this function is called upon the event we specified above but is not specific to the vpn NIC
        static void AddressChangedCallback(object sender, EventArgs e)
        {
            if (CheckVPN()) // <-- CheckVPN will check if the vpn is down
            {
                // there was some sort of network change but the vpn is still alive 
            }
            else
            {
                // vpn is down, disable internet
                Console.WriteLine("[ATTENTION] Your VPN connection dropped.");
                Disable();
                shouldReset = true; // <-- see detailed use in InputHandler
            }

        }

        // checks for a interface with tap-windows- adapter in descriptiong and if its down or up (true = up, false = down)
        // this needs to be modified to check for vpn descriptions
        static bool CheckVPN()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface Interface in interfaces)
                {
                    if (Interface.Description.Contains("TAP-Windows Adapter") && Interface.OperationalStatus == OperationalStatus.Up)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /////////////////////////////////////////////////////
        /// Firewall Configuration /////////////////////////
        /// 
        public static void Disable()
        {
            if (isWindows)
            { 
                Execute(blockCmd);
            }
            else if (isLinux)
            { }
            else
            {

            }
        }

        public static void Reset()
        {
            if (isWindows)
            {
                Execute(resetCmd);
            }
            else if (isLinux)
            {

            }
            else
            {

            }
        }

        public static void Execute(string cmd)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmd;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }



        /////////////////////////////////////////
        //// INPUT HANDLER
        ////////////////////////////////////////
        /// <summary>
        /// 
        /// --------------------------------------------------------------------------------------------- 
        /// shouldReset is used to determine if we should reset and disable the firewall rules          |
        /// if we dont use this bool, the user could disable the firewall at any time by pressing enter |
        /// or giving any input.                                                                        |
        /// ---------------------------------------------------------------------------------------------
        /// 
        /// </summary>
        /// <returns></returns>
        /// 


        static async Task<int> HandleFileAsync()
        {
            Console.Write(">");
            do
            {
                string cmd = Console.ReadLine();
                if (shouldReset)
                    Reset();
                else
                    continue;
                
            } while (true);
        }

        public static void InputStart()
        {
            Task<int> task = HandleFileAsync();
            task.Wait();
            var x = task.Result;
            Console.Write(x);
        }



    }

}
