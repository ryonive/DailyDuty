﻿using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class DailyTreasureMap : DisplayModule
    {
        protected readonly Daily.TreasureMapSettings Settings = Service.Configuration.TreasureMapSettings;

        private readonly HashSet<int> mapLevels;
        private int selectedMinimumMapLevel = 0;

        public DailyTreasureMap()
        {
            CategoryString = "Daily Treasure Map";

            mapLevels = DataObjects.MapList.Select(m => m.Level).ToHashSet();

            selectedMinimumMapLevel = Settings.MinimumMapLevel;
        }

        protected override void DrawContents()
        {
            ImGui.Checkbox("Enabled##TreasureMap", ref Settings.Enabled);
            ImGui.Spacing();

            if (Settings.Enabled)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);
                DrawTimeStatusDisplayAndCountdown();

                ImGui.Checkbox("Notifications##TreasureMap", ref Settings.NotificationEnabled);
                ImGui.Spacing();

                if (Settings.NotificationEnabled)
                {
                    ImGui.Indent(15 * ImGuiHelpers.GlobalScale);
                    DrawMinimumMapLevelComboBox();
                    ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
                }

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            ImGui.Spacing();
        }

        private static void DrawTimeStatusDisplayAndCountdown()
        {
            if (Service.Configuration.TreasureMapSettings.LastMapGathered == new DateTime())
            {
                ImGui.Text($"Last Map Collected: Never");
            }
            else
            {
                ImGui.Text($"Last Map Collected: {Service.Configuration.TreasureMapSettings.LastMapGathered}");
            }
            ImGui.Spacing();

            var timeSpan = TreasureMapModule.TimeUntilNextMap();
            ImGui.Text($"Time Until Next Map: ");
            ImGui.SameLine();

            if (timeSpan == TimeSpan.Zero)
            {
                ImGui.TextColored(new(0, 255, 0, 255), $" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }
            else
            {
                ImGui.Text($" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }

            ImGui.Spacing();
        }

        private void DrawMinimumMapLevelComboBox()
        {
            ImGui.PushItemWidth(50);

            if (ImGui.BeginCombo("Minimum Map Level", selectedMinimumMapLevel.ToString(), ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var element in mapLevels)
                {
                    bool isSelected = element == selectedMinimumMapLevel;
                    if (ImGui.Selectable(element.ToString(), isSelected))
                    {
                        selectedMinimumMapLevel = element;
                        Settings.MinimumMapLevel = selectedMinimumMapLevel;
                        Service.Configuration.Save();
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            ImGuiComponents.HelpMarker("Only show notifications that a map is available if the map is at least this level.");

            ImGui.PopItemWidth();
            ImGui.Spacing();

        }

        public override void Dispose()
        {

        }
    }
}
