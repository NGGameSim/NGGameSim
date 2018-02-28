using Eto;
using Eto.Forms;
using Eto.Wpf.Forms;
using Eto.CustomControls;
using NGAPI;
using System;

namespace NGSim
{
	// control to use in your eto.forms code
	[Handler(typeof(INetworkInfoTextArea))]
	public class NetworkInfoTextArea : Control
	{
		new INetworkInfoTextArea Handler
		{
			get { return (INetworkInfoTextArea)base.Handler; }
		}

		public string View
		{
			get { return Handler.View; }
			set { Handler.View = value; }
		}

		public string BitRate
		{
			get { return Handler.BitRate; }
			set { Handler.BitRate = value; }
		}

		public string ConnectionStatus
		{
			get { return Handler.ConnectionStatus; }
			set { Handler.ConnectionStatus = value; }
		}

		public string Warnings
		{
			get { return Handler.Warnings; }
			set { Handler.Warnings = value; OnPropertyChanged(EventArgs.Empty); }
		}

		public event EventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(EventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		// interface to the platform implementations
		public interface INetworkInfoTextArea : Control.IHandler
		{
			string View { get; set; }
			string BitRate { get; set; }
			string ConnectionStatus { get; set; }
			string Warnings { get; set; }
			event EventHandler PropertyChanged;
			void OnPropertyChanged(EventArgs e);
		}
	}

	public class NetworkInfoTextAreaHandler : WpfControl<System.Windows.Controls.TextBox, NetworkInfoTextArea , NetworkInfoTextArea.ICallback>, NetworkInfoTextArea.INetworkInfoTextArea
	{
		public NetworkInfoTextAreaHandler()
		{
			Control = new System.Windows.Controls.TextBox{ IsReadOnly = true };
			Control.AppendText("View: \tNormal\n");
			Control.AppendText("BitRate: \t0 b/s");
			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			PropertyChanged += UpdateText;
		}

		private void UpdateText(object sender, EventArgs e)
		{
			Console.WriteLine("A field was changed...");
			Control.Clear();
			Control.AppendText("View: \t");
			Control.AppendText(View);
			Control.AppendText("\n");

			Control.AppendText("BitRate: \t");
			Control.AppendText(BitRate);
			Control.AppendText("\n");

			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			Control.AppendText(Warnings);
		}

		public event EventHandler PropertyChanged;
		public void OnPropertyChanged(EventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		private string view;
		public string View
		{
			get { return view; }
			set { view = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private string bitRate;
		public string BitRate
		{
			get { return bitRate; }
			set { bitRate = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private string connectionStatus;
		public string ConnectionStatus
		{
			get { return connectionStatus; }
			set { connectionStatus = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private string warnings;
		public string Warnings
		{
			get { return warnings; }
			set { warnings = value; OnPropertyChanged(EventArgs.Empty); }
		}
	}
}
