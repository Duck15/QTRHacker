﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QTRHacker.NewDimension.Controls
{
	public class MTabControl : TabControl
	{
		public Color BColor
		{
			get; set;
		}
		public Color TColor
		{
			get; set;
		}
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}
		public MTabControl()
		{
			BColor = Color.FromArgb(200, 200, 200);
			TColor = Color.FromArgb(150, 150, 150);
			SetStyle(ControlStyles.UserPaint |
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.SupportsTransparentBackColor, true);
			UpdateStyles();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Brush bBrush = new SolidBrush(BColor);
			Brush tBrush = new SolidBrush(TColor);
			if (TabCount > 0)
			{
				e.Graphics.FillRectangle(bBrush, new RectangleF(0, 0, Width, Height));
				for (int i = 0; i < TabCount; i++)
				{
					Rectangle bounds = GetTabRect(i);
					if (SelectedIndex == i)
						e.Graphics.FillRectangle(tBrush, GetTabRect(i));
					SizeF textSize = TextRenderer.MeasureText(TabPages[i].Text, Font);
					PointF textPoint = new PointF(bounds.X + bounds.Width / 2 - textSize.Width / 2, bounds.Y + bounds.Height / 2 - textSize.Height / 2);

					GetTabRect(i);
					using (Brush b = new SolidBrush(ForeColor))
						e.Graphics.DrawString(TabPages[i].Text, Font, b, textPoint);
				}
				e.Graphics.DrawLine(new Pen(TColor, 3), new Point(0, 23), new Point(Width, 23));
			}
			else
			{
				base.OnPaint(e);
				e.Graphics.FillRectangle(bBrush, new RectangleF(0, 0, Width, Height));
			}
			bBrush.Dispose();
			tBrush.Dispose();
		}
	}
}
