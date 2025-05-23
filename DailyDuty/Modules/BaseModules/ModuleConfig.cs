﻿using System.Drawing;
using System.Text.Json.Serialization;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;

namespace DailyDuty.Modules.BaseModules;

public abstract class ModuleConfig {
    public bool ModuleEnabled;
	    
    public bool OnLoginMessage = true;
    public bool OnZoneChangeMessage = true;
    public bool ResetMessage;
	    
    public bool TodoEnabled = true;

    public bool UseCustomChannel;
    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    public bool UseCustomStatusMessage;
    public string CustomStatusMessage = string.Empty;
    public bool UseCustomResetMessage;
    public string CustomResetMessage = string.Empty;

    public bool Suppressed;

    [JsonIgnore] public bool ConfigChanged;
	    
    protected virtual void DrawModuleConfig() {
        ImGui.TextColored(KnownColor.Orange.Vector(), "No additional options for this module");
    }
    
    public void DrawConfigUi(Module module) {
        using var tabBar = ImRaii.TabBar("config_tabs");
        if (!tabBar) return;
        
        using (var moduleTab = ImRaii.TabItem("Module")) {
            if (moduleTab) {
                using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
                if (tabChild) {
                    ImGuiTweaks.Header(Strings.ModuleEnable);
                    using (ImRaii.PushIndent()) {
                        ConfigChanged |= ImGui.Checkbox(Strings.Enable, ref ModuleEnabled);
                    }

                    ImGuiTweaks.Header(Strings.ModuleConfiguration);
                    using (ImRaii.PushIndent()) {
                        DrawModuleConfig();
                    }
                }
            }
        }
					
        using (var notificationTab = ImRaii.TabItem("Notifications")) {
            if (notificationTab) {
                using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
                if (tabChild) {
                    ImGuiTweaks.Header(Strings.NotificationOptions);
                    using (ImRaii.PushIndent()) {
                        ConfigChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnLogin, ref OnLoginMessage, Strings.SendStatusOnLoginHelp);
                        ConfigChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnZoneChange, ref OnZoneChangeMessage, Strings.SendStatusOnZoneChangeHelp);
                        ConfigChanged |= ImGuiTweaks.Checkbox(Strings.SendMessageOnReset, ref ResetMessage, Strings.SendMessageOnResetHelp);
                    }

                    ImGuiTweaks.Header(Strings.NotificationCustomization);
                    using (ImRaii.PushIndent()) {
                        ConfigChanged |= ImGui.Checkbox(Strings.EnableCustomChannel, ref UseCustomChannel);

                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                        ConfigChanged |= ImGuiTweaks.EnumCombo("##ChannelSelect", ref MessageChatChannel);

                        ImGuiHelpers.ScaledDummy(3.0f);
                        ConfigChanged |= ImGui.Checkbox(Strings.EnableCustomStatusMessage, ref UseCustomStatusMessage);
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                        ImGui.InputTextWithHint("##CustomStatusMessage", Strings.StatusMessage, ref CustomStatusMessage, 1024);
                        if (ImGui.IsItemDeactivatedAfterEdit()) {
                            ConfigChanged = true;
                        }

                        ImGuiHelpers.ScaledDummy(3.0f);
                        ConfigChanged |= ImGui.Checkbox(Strings.EnableCustomResetMessage, ref UseCustomResetMessage);
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                        ImGui.InputTextWithHint("##CustomResetMessage", Strings.ResetMessage, ref CustomResetMessage, 1024);
                        if (ImGui.IsItemDeactivatedAfterEdit()) {
                            ConfigChanged = true;
                        }
                    }
                }
            }
        }
					
        using (var todoTab = ImRaii.TabItem("Todo")) {
            if (todoTab) {
                using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
                if (tabChild) {
                    ImGuiTweaks.Header(Strings.TodoConfiguration);
                    using (ImRaii.PushIndent()) {
                        ConfigChanged |= ImGui.Checkbox(Strings.TodoEnable, ref TodoEnabled);
                    }
                    
                    ImGuiTweaks.Header("Style Configuration");
                    using (var styleChild = ImRaii.Child("style_child", ImGui.GetContentRegionAvail() - ImGuiHelpers.ScaledVector2(0.0f, 33.0f))) {
                        if (styleChild) {
                            module.TodoTaskNode?.DrawConfig();
                        }
                    }
                    
                    ImGui.Separator();
        
                    if (ImGui.Button("Save", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)) && module.TodoTaskNode is not null) {
                        module.TodoTaskNode?.Save(StyleFileHelper.GetPath($"{module.ModuleName}.style.json"));
                        System.TodoListController.Refresh();
                        StatusMessage.PrintTaggedMessage("Saved configuration options for Todo List", "Todo List Config");
                    }
        
                    ImGui.SameLine(ImGui.GetContentRegionMax().X / 2.0f - 75.0f * ImGuiHelpers.GlobalScale);
                    if (ImGui.Button("Refresh Layout", ImGuiHelpers.ScaledVector2(150.0f, 23.0f)) && module.TodoTaskNode is not null) {
                        System.TodoListController.Refresh();
                    }
                    if (ImGui.IsItemHovered()) {
                        ImGui.SetTooltip("Triggers a refresh of the UI element to recalculate dynamic element size/positions");
                    }
        
                    ImGui.SameLine(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
                    ImGuiTweaks.DisabledButton("Reset", () => {
                        if (module.TodoTaskNode is not null) {
                            module.TodoTaskNode?.Load(StyleFileHelper.GetPath($"{module.ModuleName}.style.json"));
                            System.TodoListController.Refresh();
                            StatusMessage.PrintTaggedMessage("Loaded last saved configuration options for Todo List", "Todo List Config");
                        }
                    });
                }
            }
        }
    }
}