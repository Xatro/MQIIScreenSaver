using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace MQIIScreensaver
{
	class Program
	{
		static void Main()
		{
			Application.Run(new frmScreensaver());
		}
	}

	class frmScreensaver : Form
	{
		// Fields
		bool initialized;
		Timer timer;
		Bitmap stars;
		RectangleF starsArea;
		Bitmap background;
		RectangleF backgroundArea;

		// Constructor
		public frmScreensaver() : base()
		{
			BackColor = Color.Black;
			FormBorderStyle = FormBorderStyle.None;
			//WindowState = FormWindowState.Maximized;
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			Cursor.Hide();
			Paint += frmScreensaver_Paint;

			var screen = Screen.FromControl(this).Bounds;
			Location = new Point(screen.X, screen.Y);
			ClientSize = new Size(screen.Width, screen.Height);

			var assembly = Assembly.GetExecutingAssembly();
			stars = new Bitmap(assembly.GetManifestResourceStream("Stars.png"));
			var starsFactor = (float)(Width) / (float)(stars.Width);
			starsArea = new RectangleF(0, 0, Width, stars.Height * starsFactor);
			background = new Bitmap(assembly.GetManifestResourceStream("Background.png"));
			var backgroundFactor = (float)(Width * 3) / (float)(background.Width);
			var backgroundHeight = (float)(background.Height) * (float)(backgroundFactor);
			backgroundArea = new RectangleF(0, Height - backgroundHeight + 50, Width * 3, backgroundHeight);

			if (Screen.AllScreens.Length > 1)
			{
				var extend = 0;
				foreach (var s in Screen.AllScreens)
				{
					extend += s.Bounds.Width;
				}
				ClientSize = new Size(extend, Height);
			}

			timer = new Timer();
			timer.Interval = 1000;
			timer.Tick += timer_Tick;
			timer.Start();
		}

		// Event Handlers
		void timer_Tick(object sender, EventArgs e)
		{
			Invalidate();

			if (!initialized)
			{
				Click += activeInput;
				KeyPress += activeInput;
				MouseMove += activeInput;
				timer.Interval = 50;
				initialized = true;
			}

			starsArea.X -= 2;
			backgroundArea.X -= 1;
			if (starsArea.X <= -starsArea.Width)
			{
				starsArea.X = 0;
			}
			if (backgroundArea.X <= -backgroundArea.Width)
			{
				backgroundArea.X = 0;
			}
		}
		void frmScreensaver_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.DrawImage(stars, starsArea);
			e.Graphics.DrawImage(stars, new RectangleF(starsArea.X + starsArea.Width - 1, starsArea.Y,
				starsArea.Width, starsArea.Height));
			e.Graphics.DrawImage(stars, new RectangleF(starsArea.X + starsArea.Width * 2 - 1, starsArea.Y,
				starsArea.Width, starsArea.Height));
			e.Graphics.DrawImage(background, backgroundArea);
			e.Graphics.DrawImage(background, new RectangleF(backgroundArea.X + backgroundArea.Width - 2, backgroundArea.Y,
				backgroundArea.Width, backgroundArea.Height));
		}
		void activeInput(object sender, EventArgs e)
		{
			Application.Exit();
		}
	}
}