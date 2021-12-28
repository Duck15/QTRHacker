﻿using QHackLib;
using QTRHacker.Functions;
using QTRHacker.Functions.GameObjects;
using QTRHacker.Functions.GameObjects.Terraria;
using QTRHacker.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QTRHacker.PagePanels
{
	public class InfoViewEx : InfoView
	{
		private static readonly Color EditBoxBackColor = Color.FromArgb(40, 40, 40);
		public InfoViewEx(int TipWidth) : base(
			new TextBox()
			{
				BackColor = EditBoxBackColor,
				ForeColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle,
				TextAlign = HorizontalAlignment.Center,
				Font = new Font("Consolas", 8)
			}, TipDock.Left, false, TipWidth)
		{
			Tip.ForeColor = Color.White;
			Tip.BackColor = EditBoxBackColor;
		}
	}
	public class PagePanel_MainPage : PagePanel
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;
			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
		[DllImport("User32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);
		[DllImport("User32.dll")]
		public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);
		[DllImport("User32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
		private bool Dragging = false;
		private readonly Font TextFont;
		private readonly Image CrossImage;
		private readonly Button RefreshButton;
		private readonly InfoView PlayerArrayBaseAddressInfoView, CurrentPlayerBaseAddressInfoView,
			CurrentPlayerInventoryBaseAddressInfoView, CurrentPlayerArmorBaseAddressInfoView,
			CurrentPlayerDyeBaseAddressInfoView, CurrentPlayerMiscBaseAddressInfoView,
			CurrentPlayerMiscDyeBaseAddressInfoView, CurrentPlayerFirstItemBaseAddressInfoView,
			Terraria_Main_Update_BaseAddressInfoView;
		public PagePanel_MainPage(int Width, int Height) : base(Width, Height)
		{
			TextFont = new Font("Arial", 10);
			using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("QTRHacker.Res.Image.cross.png"))
				CrossImage = Image.FromStream(s);

			PlayerArrayBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 60, Width, 20),
				Text = HackContext.CurrentLanguage["PlayerArrayAddress"]
			};
			Controls.Add(PlayerArrayBaseAddressInfoView);

			CurrentPlayerBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 80, Width, 20),
				Text = HackContext.CurrentLanguage["MyPlayerAddress"]
			};
			Controls.Add(CurrentPlayerBaseAddressInfoView);

			CurrentPlayerInventoryBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 100, Width, 20),
				Text = HackContext.CurrentLanguage["MyPlayerInvAddress"]
			};
			Controls.Add(CurrentPlayerInventoryBaseAddressInfoView);

			CurrentPlayerArmorBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 120, Width, 20),
				Text = HackContext.CurrentLanguage["MyPlayerArmorAddress"]
			};
			Controls.Add(CurrentPlayerArmorBaseAddressInfoView);

			CurrentPlayerDyeBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 140, Width, 20),
				Text = HackContext.CurrentLanguage["MyPlayerDyeAddress"]
			};
			Controls.Add(CurrentPlayerDyeBaseAddressInfoView);

			CurrentPlayerMiscBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 160, Width, 20),
				Text = HackContext.CurrentLanguage["MyPlayerMiscAddress"]
			};
			Controls.Add(CurrentPlayerMiscBaseAddressInfoView);

			CurrentPlayerMiscDyeBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 180, Width, 20),
				Text = HackContext.CurrentLanguage["MyPlayerMiscDyeAddress"]
			};
			Controls.Add(CurrentPlayerMiscDyeBaseAddressInfoView);

			CurrentPlayerFirstItemBaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 200, Width, 20),
				Text = HackContext.CurrentLanguage["InvFirstItemAddress"]
			};
			Controls.Add(CurrentPlayerFirstItemBaseAddressInfoView);

			Terraria_Main_Update_BaseAddressInfoView = new InfoViewEx(220)
			{
				Bounds = new Rectangle(0, 220, Width, 20),
				Text = HackContext.CurrentLanguage["Terraria_Main_Update"]
			};
			Controls.Add(Terraria_Main_Update_BaseAddressInfoView);


			RefreshButton = new Button();
			RefreshButton.FlatStyle = FlatStyle.Flat;
			RefreshButton.ForeColor = Color.White;
			RefreshButton.Text = HackContext.CurrentLanguage["FetchAddressesAgain"];
			RefreshButton.Bounds = new Rectangle(Width - 125, Height - 35, 120, 30);
			RefreshButton.Click += (s, e) =>
			{
				if (HackContext.GameContext == null)
					return;
				InitializeAddresses();
			};
			Controls.Add(RefreshButton);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (!Dragging)
				e.Graphics.DrawImage(CrossImage, Width - 50, 15, 25, 25);
			e.Graphics.DrawString(HackContext.CurrentLanguage["DragTip"], TextFont, Brushes.White, Width - 280, 20);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.X >= Width - 50 && e.Y >= 15 && e.X <= Width - 50 + 25 && e.Y <= 15 + 25)
			{
				Dragging = true;
				Refresh();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.X >= Width - 50 && e.Y >= 15 && e.X <= Width - 50 + 25 && e.Y <= 15 + 25)
			{
				Cursor = Cursors.Cross;
			}
			else
			{
				if (!Dragging)
					Cursor = Cursors.Default;
			}
		}

		private void InitGame(Process process)
		{
			HackContext.SetContext(GameContext.OpenGame(process));

			InitializeAddresses();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (Dragging)
			{
				Dragging = false;
				GetCursorPos(out POINT p);
				IntPtr wnd = WindowFromPoint(p.X, p.Y);
				GetWindowThreadProcessId(wnd, out var processID);
				if (processID == Environment.ProcessId)
				{
					MessageBox.Show("拖动十字！！！\n拖动啊！！！！！！！！！");
					return;
				}
				Refresh();

				InitGame(Process.GetProcessById(processID));

				MainForm.MainFormInstance.OnInitialized();
			}
		}
		public void InitializeAddresses()
		{
			var ctx = HackContext.GameContext;
			PlayerArrayBaseAddressInfoView.View.Text = ctx.Players.BaseAddress.ToString("X8");
			CurrentPlayerBaseAddressInfoView.View.Text = ctx.MyPlayer.BaseAddress.ToString("X8");
			CurrentPlayerInventoryBaseAddressInfoView.View.Text = ctx.MyPlayer.Inventory.BaseAddress.ToString("X8");
			CurrentPlayerArmorBaseAddressInfoView.View.Text = ctx.MyPlayer.Armor.BaseAddress.ToString("X8");
			CurrentPlayerDyeBaseAddressInfoView.View.Text = ctx.MyPlayer.Dye.BaseAddress.ToString("X8");
			CurrentPlayerMiscBaseAddressInfoView.View.Text = ctx.MyPlayer.MiscEquips.BaseAddress.ToString("X8");
			CurrentPlayerMiscDyeBaseAddressInfoView.View.Text = ctx.MyPlayer.MiscDyes.BaseAddress.ToString("X8");
			CurrentPlayerFirstItemBaseAddressInfoView.View.Text = ctx.MyPlayer.Inventory[0].BaseAddress.ToString("X8");
			Terraria_Main_Update_BaseAddressInfoView.View.Text = ctx.GameModuleHelper["Terraria.Main", "Update"].ToString("X8");
		}
	}
}
