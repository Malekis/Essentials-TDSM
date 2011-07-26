﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;


using Essentials;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Events;
using Essentials.Kit;
using Terraria_Server.Logging;

namespace Essentials
{
    public class Essentials : Plugin
    {
        /*
         * @Developers
         *
         * Plugins need to be in .NET 3.5
         * Otherwise TDSM will be unable to load it.
         *
         * As of June 16, 1:15 AM, TDSM should now load Plugins Dynamically.
         */

        // tConsole is used for when logging Output to the console & a log file.

        public bool isEnabled = false;
        public int i;
        public static Dictionary<String, PlayerCommandEvent> lastEventByPlayer;
        public static Properties properties;
        public static KitManager kitManager { get; set; }
        public static Dictionary<int, bool> essentialsPlayerList; //int - player ID, bool - god mode

        public override void Load()
        {
            Name = "Essentials";
            Description = "Essential commands for TDSM.";
            Author = "Luke";
            Version = "0.5";
            TDSMBuild = 27;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Essentials";
            string kitsFile = pluginFolder + Path.DirectorySeparatorChar + "kits.xml";
            string propertiesFile = pluginFolder + Path.DirectorySeparatorChar + "essentials.properties";
            
            lastEventByPlayer = new Dictionary<String, PlayerCommandEvent>();
            essentialsPlayerList = new Dictionary<int, bool>();

            if (!Directory.Exists(pluginFolder))
                CreateDirectory(pluginFolder); //Touch Directory, Wee need this.

            //We do not want to delete records!
            if (!File.Exists(kitsFile))
                File.Create(kitsFile).Close();

            if (!File.Exists(propertiesFile))
                File.Create(propertiesFile).Close();

            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "essentials.properties");
            properties.Load();
            properties.pushData();
            properties.Save();

            Log("Loading Kits...");
            kitManager = new KitManager(kitsFile);

        LOADKITS:
            try
            {
                    kitManager.LoadKits();
            }
            catch (Exception e)
            {
                Log(e.Message);
                Console.Write("Create a parsable file? [Y/n]: ");
                if (Console.ReadLine().ToLower() == "y")
                {
                    kitManager.CreateTemplate();
                    goto LOADKITS; //Go back to reload data ;)...Im lazy I KNOW
                }
            }
            Log("Complete, Loaded " + kitManager.KitList.Count + " Kit(s)");
            
            isEnabled = true;
        }

        public override void Enable()
        {
            ProgramLog.Log(base.Name + " enabled.");

            //Prepare & Start the God Mode Thread.
            new God.GodMode(this);

            //Add Commands
            AddCommand("!")
                .Calls(Commands.LastCommand);

            AddCommand("slay")
                .Calls(Commands.Slay);

            AddCommand("heal")
                .Calls(Commands.HealPlayer);

            AddCommand("ping")
                .Calls(Commands.ConnectionTest_Ping); //Need to make a single static function

            AddCommand("pong")
                .Calls(Commands.ConnectionTest_Ping); //^^

            AddCommand("suicide")
                .Calls(Commands.Suicide);

            AddCommand("butcher")
                .Calls(Commands.Butcher);

            AddCommand("kit")
                .Calls(Commands.Kit);

            AddCommand("god")
                .Calls(Commands.GodMode);

            AddCommand("spawn")
                .Calls(Commands.Spawn);

            AddCommand("info")
                .Calls(Commands.Info);

            AddCommand("plugins")
                .Calls(Commands.Plugins);

            AddCommand("plugin")
                .Calls(Commands.Plugins);
        }

        public override void Disable()
        {
            ProgramLog.Log(base.Name + " disabled.");
            isEnabled = false;
        }

        public static void Log(String message, String pluginName)
        {
            ProgramLog.Log("[" + pluginName + "] " + message);
        }

        public void Log(String message)
        {
           Log(message, base.Name);
        }
       
        private static void CreateDirectory(String dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}
