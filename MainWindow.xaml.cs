using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClockScreensaver
{
	public partial class MainWindow : Window
	{
		private DispatcherTimer timer;
		private string lastTime = "";

		public MainWindow()
		{
			InitializeComponent();
			LoadInitialDigits();
			StartClock();

			// Allow ESC to close the app (for dev use)
			this.KeyDown += (s, e) =>
			{
				if (e.Key == Key.Escape)
					this.Close();
			};
		}

		private void LoadInitialDigits()
		{
			for (int i = 0; i < 10; i++)
			{
				HourTens.Children.Add(CreateDigitBlock(i));
				HourOnes.Children.Add(CreateDigitBlock(i));
				MinuteTens.Children.Add(CreateDigitBlock(i));
				MinuteOnes.Children.Add(CreateDigitBlock(i));
				SecondTens.Children.Add(CreateDigitBlock(i));
				SecondOnes.Children.Add(CreateDigitBlock(i));
			}
		}

		private TextBlock CreateDigitBlock(int digit)
		{
			return new TextBlock
			{
				Text = digit.ToString(),
				FontSize = 80,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
				Width = 100,
				Height = 108,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center

			};
		}

		private void StartClock()
		{
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += UpdateClock;
			timer.Start();
			UpdateClock(this, EventArgs.Empty); // Immediate update
		}

		private void UpdateClock(object sender, EventArgs e)
		{
			string time = DateTime.Now.ToString("HHmmss");

			if (time != lastTime)
			{
				UpdateDigitIfChanged(HourTens, time[0], lastTime.Length == 6 ? lastTime[0] : ' ', HourTensTransform);
				UpdateDigitIfChanged(HourOnes, time[1], lastTime.Length == 6 ? lastTime[1] : ' ', HourOnesTransform);
				UpdateDigitIfChanged(MinuteTens, time[2], lastTime.Length == 6 ? lastTime[2] : ' ', MinuteTensTransform);
				UpdateDigitIfChanged(MinuteOnes, time[3], lastTime.Length == 6 ? lastTime[3] : ' ', MinuteOnesTransform);
				UpdateDigitIfChanged(SecondTens, time[4], lastTime.Length == 6 ? lastTime[4] : ' ', SecondTensTransform);
				UpdateDigitIfChanged(SecondOnes, time[5], lastTime.Length == 6 ? lastTime[5] : ' ', SecondOnesTransform);
				lastTime = time;
			}
		}

		private void UpdateDigitIfChanged(StackPanel panel, char newChar, char oldChar, TranslateTransform transform)
		{
			if (newChar != oldChar)
			{
				int index = newChar - '0';
				double digitHeight = 108;
				double targetOffset = -(index * digitHeight) + (540 - digitHeight / 2); // Center current digit

				foreach (TextBlock block in panel.Children)
					block.Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)); // dim

				((TextBlock)panel.Children[index]).Foreground = new SolidColorBrush(Colors.White); // active

				var animation = new DoubleAnimation
				{
					To = targetOffset,
					Duration = TimeSpan.FromMilliseconds(400),
					EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
				};

				transform.BeginAnimation(TranslateTransform.YProperty, animation);
			}
		}

	}
}