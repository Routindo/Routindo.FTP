using System;
using System.Globalization;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Routindo.Plugins.FTP.UI.Controls
{
	[TemplatePart(Name = "PART_NumericTextBox", Type = typeof(TextBox))]
	[TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
	[TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
	public partial class IntegerBox : UserControl
	{
		#region Variables
        private string valueFormat;
        private int    initialValue;

		#endregion

		public IntegerBox()
		{
			this.InitializeComponent();
		}

		#region Properties

		public static readonly DependencyProperty SilentErrorProperty =
			DependencyProperty.Register("SilentErrorSeparator", typeof(bool), typeof(IntegerBox), new PropertyMetadata(false));
		public bool SilentError
		{
			get { return (bool)this.GetValue(SilentErrorProperty); }
			set 
			{ 
				this.SetValue(SilentErrorProperty, value); 
			}
		}

		public static readonly DependencyProperty AllowManualEditProperty =
			DependencyProperty.Register("AllowManualEditSeparator", typeof(bool), typeof(IntegerBox), new PropertyMetadata(true));
		public bool AllowManualEdit
		{
			get { return (bool)this.GetValue(AllowManualEditProperty); }
			set 
			{ 
				this.SetValue(AllowManualEditProperty, value); 
			}
		}

		public static readonly DependencyProperty MinimumProperty =
			DependencyProperty.Register("Minimum", typeof(int), typeof(IntegerBox), new PropertyMetadata(int.MinValue));
		public int Minimum
		{
			get { return Math.Max(int.MinValue, (int)this.GetValue(MinimumProperty)); }
			set { this.SetValue(MinimumProperty, value); }
		}

		public static readonly DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum", typeof(int), typeof(IntegerBox), new PropertyMetadata(int.MaxValue));
		public int Maximum
		{
			get { return Math.Min(int.MaxValue, (int)this.GetValue(MaximumProperty)); }
			set { this.SetValue(MaximumProperty, value); }
		}

		public static readonly DependencyProperty IncrementProperty =
			DependencyProperty.Register("Increment", typeof(int), typeof(IntegerBox), new PropertyMetadata(1));
		public int Increment
		{
			get { return Math.Min(int.MaxValue, Math.Abs((int)this.GetValue(IncrementProperty))); }
			set { this.SetValue(IncrementProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(int), typeof(IntegerBox), new PropertyMetadata(0, OnValueChanged));
		private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			IntegerBox numericBoxControl = (IntegerBox)sender;
			numericBoxControl.OnValueChanged((int)args.OldValue, (int)args.NewValue);
		}
		public int Value
		{
			get { return (int)this.GetValue(ValueProperty); }
			set { this.SetValue(ValueProperty, value); }
		}

		#endregion

		public static readonly RoutedEvent ValueChangedEvent =
			EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<int>), typeof(IntegerBox));
		public event RoutedPropertyChangedEventHandler<int> ValueChanged
		{
			add    { this.AddHandler   (ValueChangedEvent, value); }
			remove { this.RemoveHandler(ValueChangedEvent, value); }
		}
		private void OnValueChanged(int oldValue, int newValue)
		{
			RoutedPropertyChangedEventArgs<int> args = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
			args.RoutedEvent = IntegerBox.ValueChangedEvent;
			this.RaiseEvent(args);
		}


		#region Private Methods

		private void IncreaseValue()
		{
			this.Value = Math.Min(this.Maximum, this.Value + this.Increment);
            this.PART_NumericTextBox.Text = this.Value.ToString(this.valueFormat);
		}
		
		private void DecreaseValue()
		{
			this.Value = Math.Max(this.Minimum, this.Value - this.Increment);
            this.PART_NumericTextBox.Text = this.Value.ToString(this.valueFormat);
		}

		#endregion

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.PART_IncreaseButton.Click            += increaseBtn_Click;
			this.PART_DecreaseButton.Click            += decreaseBtn_Click;

			if (this.AllowManualEdit)
			{
				this.PART_NumericTextBox.PreviewTextInput += numericBox_PreviewTextInput;
				this.PART_NumericTextBox.TextChanged      += this.PART_NumericTextBox_TextChanged;
			}

			this.PART_NumericTextBox.IsReadOnly        = !this.AllowManualEdit;
			this.PART_NumericTextBox.MouseWheel       += numericBox_MouseWheel;


			this.valueFormat = "0";
            this.initialValue = this.Value;
			this.PART_NumericTextBox.Text = this.Value.ToString(this.valueFormat);
		}

		#region Events

		private void numericBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			int caretIndex = textbox.CaretIndex;
			try
			{
                bool isError = false;
				string text = this.PART_NumericTextBox.Text.Insert(caretIndex, e.Text);
				int value;
				isError = (!int.TryParse(text, out value));
				isError |= (value < this.Minimum || value > this.Maximum);
				if (isError)
				{
					if (!this.SilentError)
					{
						SystemSounds.Hand.Play();
					}
					e.Handled = true;
				}
			}
			catch (FormatException)
			{
			}
		}

		private void PART_NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string text = this.PART_NumericTextBox.Text;
			if (string.IsNullOrEmpty(text))
			{
				text = this.initialValue.ToString(this.valueFormat);
				this.PART_NumericTextBox.Text = text;
			}
            this.Value = (text == "-") ? 0 : int.Parse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign);
		}

		private void numericBox_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0) 
			{
				this.IncreaseValue();
			}
			else if (e.Delta < 0) 
			{
				this.DecreaseValue();
			}
		}

		private void increaseBtn_Click(object sender, RoutedEventArgs e)
		{
			this.IncreaseValue();
		}

		private void decreaseBtn_Click(object sender, RoutedEventArgs e)
		{
			this.DecreaseValue();
		}

		#endregion Events

	}
}
