﻿using System;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.ModuleConfiguration.Features
{
    internal class DutyRoulette : IConfigurable
    {
        public string ConfigurationPaneLabel => Strings.Module.DutyRouletteLabel;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteInformation);
            }
        };
        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteAutomationInformation);
            }
        };
        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteTechnicalInformation);
            }
        };
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;
        public ModuleName ModuleName => ModuleName.DutyRoulette;
        private static DutyRouletteSettings Settings => Service.CharacterConfiguration.DutyRoulette;


        private readonly InfoBox currentStatus = new()
        {
            Label = Strings.Common.CurrentStatusLabel,
            ContentsAction = () =>
            {
                if (ImGui.BeginTable($"##Status", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);
            
                    foreach (var tracked in Settings.TrackedRoulettes)
                    {
                        if (tracked.Tracked == true)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.Text(tracked.Type.ToString());

                            ImGui.TableNextColumn();
                            Draw.CompleteIncomplete(tracked.Completed);
                        }
                    }

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox trackedRoulettes = new()
        {
            Label = Strings.Module.DutyRouletteTrackedRoulettesLabel,
            ContentsAction = () =>
            {
                var contentWidth = ImGui.GetContentRegionAvail().X;

                if (ImGui.BeginTable($"", (int)(contentWidth / 200.0f)))
                {
                    foreach (var roulette in Settings.TrackedRoulettes)
                    {
                        ImGui.TableNextColumn();
                        if (ImGui.Checkbox($"{roulette.Type}", ref roulette.Tracked))
                        {
                            Service.LogManager.LogMessage(ModuleName.DutyRoulette, $"{roulette.Type} " + (roulette.Tracked ? "Tracking Enabled" : "Tracking Disabled"));
                            Service.CharacterConfiguration.Save();
                        }

                        if (roulette.Type == RouletteType.Mentor)
                        {
                            ImGui.SameLine();
                            ImGuiComponents.HelpMarker("You know it's going to be an extreme... right?");
                        }
                    }

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox options = new()
        {
            Label = Strings.Configuration.OptionsTabLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.Enabled))
                {
                    Service.LogManager.LogMessage(ModuleName.DutyRoulette, Settings.Enabled ? "Enabled" : "Disabled");
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox notificationOptions = new()
        {
            Label = Strings.Common.NotificationOptionsLabel,
            ContentsAction = () =>
            {
                if(Draw.Checkbox(Strings.Common.NotifyOnLoginLabel, ref Settings.LoginReminder, Strings.Common.NotifyOnLoginHelpText))
                {
                    Service.LogManager.LogMessage(ModuleName.DutyRoulette, "Login Notifications " + (Settings.Enabled ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleName.DutyRoulette, "Zone Change Notifications" + (Settings.Enabled ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        public DutyRoulette()
        {
            AboutImage = Image.LoadImage("DutyRoulette");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Module.DutyRouletteLabel);
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            options.DrawCentered(0.80f);
            
            ImGuiHelpers.ScaledDummy(30.0f);
            trackedRoulettes.DrawCentered(0.80f);

            ImGuiHelpers.ScaledDummy(30.0f);
            notificationOptions.DrawCentered(0.80f);

            ImGuiHelpers.ScaledDummy(20.0f);
        }

        public void DrawStatusContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            currentStatus.DrawCentered(0.80f);
        }
    }
}
