﻿using System;
using System.Diagnostics;
using DailyDuty.CommandSystem;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem;
using DailyDuty.System;
using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;

namespace DailyDuty
{
    public sealed class DailyDutyPlugin : IDalamudPlugin
    {
        public string Name => "DailyDuty";

        private DisplayManager DisplayManager { get; init; }
        private CommandManager CommandManager { get; init; }
        private ModuleManager ModuleManager { get; init; }
        private readonly Stopwatch onLoginStopwatch = new();

        public DailyDutyPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);
            Service.ClientState.Login += OnLogin;
            Service.ClientState.Logout += OnLogout;
            Service.Configuration.UpdateCharacter();


            // Create Systems
            ModuleManager = new ModuleManager();
            DisplayManager = new DisplayManager(ModuleManager);
            CommandManager = new CommandManager(DisplayManager);

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            Service.Framework.Update += OnFrameworkUpdate;

            // Register Windows
            Service.WindowSystem.AddWindow(DisplayManager);

            Service.Chat.Enable();
        }

        private void OnLogout(object? sender, EventArgs e)
        {
            Service.LoggedIn = false;
        }

        private void OnLogin(object? sender, EventArgs e)
        {
            onLoginStopwatch.Start();
            Service.LoggedIn = true;
        }

        private void OnFrameworkUpdate(Framework framework)
        {
            ModuleManager.Update();

            if (onLoginStopwatch.Elapsed >= TimeSpan.FromSeconds(3) && onLoginStopwatch.IsRunning)
            {
                // If for some reason the number is still zero, wait again
                if (Service.ClientState.LocalContentId == 0)
                {
                    PluginLog.Error("[System] LocalContentID is still Invalid. Retrying in 3s.");
                    onLoginStopwatch.Restart();
                    return;
                }

                Service.Configuration.UpdateCharacter();

                onLoginStopwatch.Stop();
                onLoginStopwatch.Reset();
            }
        }

        private void DrawUI()
        {
            Service.WindowSystem.Draw();
        }
        private void DrawConfigUI()
        {
            DisplayManager.IsOpen = true;
        }

        public void Dispose()
        {
            CommandManager.Dispose();
            DisplayManager.Dispose();
            ModuleManager.Dispose();

            Service.ClientState.Login -= OnLogin;
            Service.ClientState.Logout -= OnLogout;
        }
    }
}
