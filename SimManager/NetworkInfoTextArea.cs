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

		public bool ConnectionStatus
		{
			get { return Handler.ConnectionStatus; }
			set { Handler.ConnectionStatus = value; }
		}

		public string Warnings
		{
			get { return Handler.Warnings; }
			set { Handler.Warnings = value; }
		}

		public string IP
		{
			get { return Handler.IP; }
			set { Handler.IP = value; }
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
			bool ConnectionStatus { get; set; }
			string Warnings { get; set; }
			string IP { get; set; }
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
			Control.AppendText("BitRate: \t0 bytes/second\n");
			Control.AppendText("Connection Status: \tN/A\n");
			Control.AppendText("Server IP: \tN/A\n");
			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			PropertyChanged += UpdateText;
		}

		private void UpdateText(object sender, EventArgs e)
		{
			Control.Clear();
			Control.AppendText("View: \t");
			Control.AppendText(View);
			Control.AppendText("\n");

			Control.AppendText("BitRate: \t");
			Control.AppendText(BitRate);
			Control.AppendText(" bytes/second");
			Control.AppendText("\n");

			Control.AppendText("Connection Status: \t");
			if (ConnectionStatus)
			{
				Control.AppendText("Connected\n");
			} else
			{
				Control.AppendText("Not Connected\n");
			}

			Control.AppendText("Server IP: \t");
			Control.AppendText(IP);

			Control.AppendText("\n\n");
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

		private bool connectionStatus;
		public bool ConnectionStatus
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

		private string ip;
		public string IP
		{
			get { return ip; }
			set { ip = value; OnPropertyChanged(EventArgs.Empty); }
		}
	}
}
