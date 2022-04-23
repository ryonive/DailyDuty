﻿using System;
using System.IO;
using DailyDuty.Utilities;
using Newtonsoft.Json;

namespace DailyDuty.Data
{
    [Serializable]
    public class CharacterConfiguration
    {
        public int Version { get; set; } = 1;

        public string CharacterName = "Unknown";
        public ulong LocalContentID = 0;
        
        public void Save()
        {
            if (LocalContentID != 0)
            {
                Chat.Log("Saving", $"{DateTime.Now} - {CharacterName} Saved");

                var configFileInfo = Configuration.GetConfigFileInfo(CharacterName);

                File.WriteAllText(configFileInfo.FullName, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }
    }
}