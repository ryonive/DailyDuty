﻿using System.Drawing;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Newtonsoft.Json;

namespace DailyDuty.CustomNodes;

[JsonObject(MemberSerialization.OptIn)]
public sealed class TimerNode : SimpleComponentNode {
	[JsonProperty] private readonly CastBarProgressBarNode progressBarNode;
	[JsonProperty] private readonly TextNode moduleNameNode;
	[JsonProperty] private readonly TextNode timeRemainingNode;
	[JsonProperty] private readonly TextNode tooltipNode;

	public TimerNode() {
		progressBarNode = new CastBarProgressBarNode {
			NodeId = 2,
			Progress = 0.30f,
			Size = new Vector2(400.0f, 48.0f),
			NodeFlags = NodeFlags.Visible,
			BackgroundColor = KnownColor.Black.Vector(),
			BarColor = KnownColor.Aqua.Vector(),
		};
		System.NativeController.AttachNode(progressBarNode, this);

		moduleNameNode = new TextNode {
			NodeId = 3,
			Position = new Vector2(12.0f, -24.0f),
			NodeFlags = NodeFlags.Visible,
			FontType = FontType.Jupiter,
			FontSize = 24,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
		};
		System.NativeController.AttachNode(moduleNameNode, this);

		timeRemainingNode = new TextNode {
			NodeId = 4,
			NodeFlags = NodeFlags.Visible | NodeFlags.AnchorRight,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
			FontType = FontType.Axis,
			FontSize = 24,
			CharSpacing = 0,
			Position = new Vector2(0.0f, -22.0f),
			Text = "0.00:00:00",
		};
		System.NativeController.AttachNode(timeRemainingNode, this);

		tooltipNode = new TextNode {
			NodeId = 5,
			Size = new Vector2(16.0f, 16.0f),
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			IsVisible = true,
			FontSize = 16,
			FontType = FontType.Axis,
			TextFlags = TextFlags.Edge,
			AlignmentType = AlignmentType.BottomRight,
			Text = "?",
			Tooltip = "Overlay from DailyDuty plugin",
			EventFlagsSet = true,
		};
		System.NativeController.AttachNode(tooltipNode, this);
	}

	public float Progress {
		get => progressBarNode.Progress;
		set => progressBarNode.Progress = value;
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			progressBarNode.Width = value;
			timeRemainingNode.X = value - timeRemainingNode.Width - 12.0f;
			tooltipNode.Position = new Vector2(progressBarNode.Width, 8.0f);
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			progressBarNode.Height = value;
		}
	}

	public SeString ModuleName {
		get => moduleNameNode.Text;
		set => moduleNameNode.Text = value;
	}

	public SeString TimeRemainingText {
		set => timeRemainingNode.Text = value;
		get => timeRemainingNode.Text;
	}

	public Vector4 BarColor {
		get => progressBarNode.BarColor;
		set => progressBarNode.BarColor = value;
	}

	public Vector4 LabelColor {
		get => moduleNameNode.TextColor;
		set => moduleNameNode.TextColor = value;
	}

	public Vector4 TimerColor {
		get => timeRemainingNode.TextColor;
		set => timeRemainingNode.TextColor = value;
	}

	public bool ShowLabel {
		get => moduleNameNode.IsVisible;
		set => moduleNameNode.IsVisible = value;
	}

	public bool ShowTimer {
		get => timeRemainingNode.IsVisible;
		set => timeRemainingNode.IsVisible = value;
	}

	public override void DrawConfig() {
		base.DrawConfig();
				
		using (var progressBar = ImRaii.TreeNode("Progress Bar")) {
			if (progressBar) {
				progressBarNode.DrawConfig();
			}
		}
				
		using (var moduleName = ImRaii.TreeNode("Module Name")) {
			if (moduleName) {
				moduleNameNode.DrawConfig();
			}
		}
				
		using (var timeRemaining = ImRaii.TreeNode("Time Remaining")) {
			if (timeRemaining) {
				timeRemainingNode.DrawConfig();
			}
		}
				
		using (var tooltip = ImRaii.TreeNode("Tooltip")) {
			if (tooltip) {
				tooltipNode.DrawConfig();
			}
		}
	}
}