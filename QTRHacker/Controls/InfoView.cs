﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QTRHacker.Controls
{
	public class InfoView : UserControl
	{
		public enum TipDock
		{
			Top, Left, Down, Right
		}
		public override string Text
		{
			get => Tip.Text;
			set => Tip.Text = value;
		}
		public Label Tip
		{
			get;
		}
		public Control View
		{
			get;
		}
		private TipDock ViewDock;
		private int TipWidth;
		public InfoView(Control View, TipDock Dock, bool Border = true, int TipWidth = 40)
		{
			this.TipWidth = TipWidth;
			this.View = View;
			this.ViewDock = Dock;
			this.Tip = new Label();
			this.Tip.BorderStyle = BorderStyle.FixedSingle;
			this.BorderStyle = Border ? BorderStyle.FixedSingle : BorderStyle.None;
			this.Tip.TextAlign = ContentAlignment.MiddleCenter;
			this.Controls.Add(Tip);
			this.Controls.Add(View);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			switch (ViewDock)
			{
				case TipDock.Top:
					Tip.Bounds = new Rectangle(0, 0, Width, 20);
					View.Bounds = new Rectangle(0, 20, Width, Height - 20);
					break;
				case TipDock.Left:
					Tip.Bounds = new Rectangle(0, 0, TipWidth, Height);
					View.Bounds = new Rectangle(TipWidth, 0, Width - TipWidth, Height);
					break;
				case TipDock.Down:
					Tip.Bounds = new Rectangle(0, Height - 20, Width, 20);
					View.Bounds = new Rectangle(0, 0, Width, Height - 20);
					break;
				case TipDock.Right:
					Tip.Bounds = new Rectangle(Width - TipWidth, 0, TipWidth, Height);
					View.Bounds = new Rectangle(0, 0, Width - TipWidth, Height);
					break;
			}
		}
	}
}
