using Optional;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Timereporter.Core.Models;

namespace Timereporter
{
	public partial class Form1 : Form
	{
		private GridActor _gridActor;
		private GridMutator _mutator;
		private BindingSource bindingSource;

		// https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-bind-data-to-the-windows-forms-datagridview-control
		public Form1()
		{
			InitializeComponent();
		}

		private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Show();
			this.CenterToScreen();
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void Form1_Move(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.Hide();
				notifyIcon1.ShowBalloonTip(1000, "Notice", "Something has happened", ToolTipIcon.Info);
			}
		}

		private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
		}

		private void LoadData()
		{
			_mutator.Load(comboBox1);
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			LoadData();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			_gridActor = new GridActor(dataGridView1);
			_mutator = new GridMutator(dataGridView1);
			bindingSource = new BindingSource();

			comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			LoadData();
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			var comboBox = sender as ComboBox;
			var monthOption = comboBox.SelectedItem as DateOption;
			monthOption.SomeNotNull().MatchSome(o => _mutator.Load(o.Date.Some()));
		}
	}
}
