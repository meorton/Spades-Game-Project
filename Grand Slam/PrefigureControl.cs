using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Mehran;

namespace Mehran
{	
	public class PrefigureControl : System.Windows.Forms.UserControl
	{
		private int[] perfigures = new int[4];
		private int[] maxPerfigures = new int[3];
		private int bestPerfigure = 0;
		private int playerPerfigure = -1;

		private System.Windows.Forms.TextBox Name4;
		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.ComboBox comboBox;
		private System.Windows.Forms.TextBox Name3;
		private System.Windows.Forms.TextBox Name2;
		private System.Windows.Forms.TextBox Name1;
		private System.ComponentModel.Container components = null;

		public event EventHandler OK;

		public string PlayerName1
		{
			get
			{
				return Name1.Text;
			}
			set
			{
				Name1.Text = value;
			}
		}

		public string PlayerName2
		{
			get
			{
				return Name2.Text;
			}
			set
			{
				Name2.Text = value;
			}
		}

		public string PlayerName3
		{
			get
			{
				return Name3.Text;
			}
			set
			{
				Name3.Text = value;
			}
		}

		public string PlayerName4
		{
			get
			{
				return Name4.Text;
			}
			set
			{
				Name4.Text = value;
			}
		}

		public string[] PlayerNames
		{
			get
			{
				string[] ret = new string[4];
				ret[0] = Name1.Text;
				ret[1] = Name2.Text;
				ret[2] = Name3.Text;
				ret[3] = Name4.Text;
				return ret;
			}
			set
			{
				Name1.Text = value[0];
				Name2.Text = value[1];
				Name3.Text = value[2];
				Name4.Text = value[3];
			}
		}
		public int Prefigure
		{
			get
			{
				return bestPerfigure;
			}
		}

		public int PrefigurePlayer
		{
			get
			{
				return playerPerfigure;
			}
		}

		public string PrefigurePlayerName
		{
			get
			{
				switch(playerPerfigure)
				{
					case 0:	return PlayerName1;
					case 1:	return PlayerName2;
					case 2:	return PlayerName3;
					case 3:	return PlayerName4;
				}
				throw new Exception("Impossible playerPerfigure");
			}
		}


		public PrefigureControl()
		{
			Visible = false;
			InitializeComponent();
		}
		public void ShowPrefigures(Card[] all,int starter)
		{
			Visible = true;
			ok.Enabled = false;

			perfigures = new int[4];
			maxPerfigures = new int[3];
			bestPerfigure = 0;
			playerPerfigure = -1;
			for(int i = 0; i < 3; i++)
			{
				perfigures[i] = 0;
				maxPerfigures[i] = -1;
				int cardCount = i * 13;
				foreach(CardType ct in Enum.GetValues(typeof(CardType)))
				{
					int thisGrade = 0;
					int thisSiutStart = cardCount;
					while((all[cardCount].Type == ct)  && (cardCount < i * 12 + 12))
					{
						cardCount++;
						thisGrade += (all[cardCount].Value == CardValues.Ace) 
							? 14 : (int)all[cardCount].Value + 1;
					}
					if(thisGrade > maxPerfigures[i])
						maxPerfigures[i] = thisGrade;
				}
				Debug.WriteLine("Player"+i+" MaxScore is: "+maxPerfigures[i]);
				if(maxPerfigures[i] > 60)
				{
					maxPerfigures[i] /= 5;
					maxPerfigures[i] *= 4;
				}
				else
				{
					maxPerfigures[i] /= 2;
					//maxPerfigures[i] *= 1;
				}
				maxPerfigures[i] = (maxPerfigures[i] / 5 + 1) * 5;
				Debug.WriteLine("Player"+i+" Maxfigure is: "+maxPerfigures[i] + "\n");
			}

			comboBox.Items.Clear();
			for(int i = 0; i <= 65; i+=5) comboBox.Items.Add(i);
			comboBox.Items.Add("Pass");
			
			if(starter + 1 < 3)
				comboBox.Items.RemoveAt(0);
			for(int i = starter + 1; i < 3; i++)
				PlayerPrefigure(i);
		}

		
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawString("Players"
				,new Font("arial",10,FontStyle.Bold)
				,Brushes.Black
				,new Rectangle(0,10,100,20));
			e.Graphics.DrawString("Bid"
				,new Font("arial",10,FontStyle.Bold)
				, Brushes.White
                , new Rectangle(70,10,100,20));
			for(int i=0;i<4;i++)
			{
				e.Graphics.DrawString(
					perfigures[i] != -1 ? perfigures[i].ToString() : "Pass"
					,new Font("arial",10,FontStyle.Bold)
					,Brushes.White
					,new Rectangle(70,40 + i * 40,100,20));
			}
			if(comboBox.Enabled == false)
				e.Graphics.DrawString(
					PrefigurePlayerName+" Out bids with $: "+bestPerfigure
					,new Font("arial",10,FontStyle.Bold)
					,Brushes.White
					,10,200);

		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrefigureControl));
            this.Name4 = new System.Windows.Forms.TextBox();
            this.ok = new System.Windows.Forms.Button();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.Name3 = new System.Windows.Forms.TextBox();
            this.Name2 = new System.Windows.Forms.TextBox();
            this.Name1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Name4
            // 
            this.Name4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Name4.Location = new System.Drawing.Point(5, 160);
            this.Name4.Multiline = true;
            this.Name4.Name = "Name4";
            this.Name4.Size = new System.Drawing.Size(56, 29);
            this.Name4.TabIndex = 10;
            this.Name4.Text = "FLANK";
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Enabled = false;
            this.ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ok.Location = new System.Drawing.Point(107, 244);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(58, 23);
            this.ok.TabIndex = 12;
            this.ok.Text = "OK";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // comboBox
            // 
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.Location = new System.Drawing.Point(107, 217);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(56, 21);
            this.comboBox.TabIndex = 11;
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // Name3
            // 
            this.Name3.BackColor = System.Drawing.Color.Tomato;
            this.Name3.Location = new System.Drawing.Point(5, 120);
            this.Name3.Multiline = true;
            this.Name3.Name = "Name3";
            this.Name3.Size = new System.Drawing.Size(56, 34);
            this.Name3.TabIndex = 9;
            this.Name3.Text = "BOB";
            this.Name3.TextChanged += new System.EventHandler(this.Name3_TextChanged);
            // 
            // Name2
            // 
            this.Name2.BackColor = System.Drawing.Color.Maroon;
            this.Name2.Location = new System.Drawing.Point(5, 80);
            this.Name2.Multiline = true;
            this.Name2.Name = "Name2";
            this.Name2.Size = new System.Drawing.Size(56, 34);
            this.Name2.TabIndex = 8;
            this.Name2.Text = "JAMES";
            // 
            // Name1
            // 
            this.Name1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Name1.Location = new System.Drawing.Point(5, 40);
            this.Name1.Multiline = true;
            this.Name1.Name = "Name1";
            this.Name1.Size = new System.Drawing.Size(56, 34);
            this.Name1.TabIndex = 7;
            this.Name1.Text = "MIKE";
            // 
            // PrefigureControl
            // 
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.Name4);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.Name3);
            this.Controls.Add(this.Name2);
            this.Controls.Add(this.Name1);
            this.Name = "PrefigureControl";
            this.Size = new System.Drawing.Size(170, 270);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void PlayerPrefigure(int player)
		{
			if(perfigures[player] != -1 && comboBox.Enabled == true)//don't pass
			{
				Graphics grp = this.CreateGraphics();
				perfigures[player] = 
					bestPerfigure + 5 > maxPerfigures[player] ? -1 : bestPerfigure + 5;
				if(perfigures[player] != -1)
				{
					comboBox.Items.Remove(perfigures[player]);
					bestPerfigure = perfigures[player];
					playerPerfigure = player;
					if(perfigures[player] == 65) perfigures[player] = -1;
				}
				if(perfigures[player] == -1)
				{
					if(comboBox.Enabled == true 
						&& perfigures[0] == -1 
						&& perfigures[1] == -1
						&& perfigures[2] == -1)
					{
						comboBox.Enabled = false;
						ok.Enabled = true;
					}
				}
			}
		}

		private void comboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Graphics grp = this.CreateGraphics();
			int sel = comboBox.SelectedIndex;
			if(sel == -1)return;
			perfigures[3] = (comboBox.Items[sel].ToString() == "Pass") 
				? -1 : perfigures[3] = (int)comboBox.Items[sel];
			if(perfigures[3] != -1)
			{
				bestPerfigure = perfigures[3];
				playerPerfigure = 3;
				comboBox.BeginUpdate();
				for(int i=0;i<=sel;i++)
					comboBox.Items.RemoveAt(0);
				comboBox.EndUpdate();
				for(int i = 0; i < 3; i++)
					PlayerPrefigure(i);
			}
			else
			{
				bestPerfigure = 0;
				for(int i=0;i<3;i++)
					if(maxPerfigures[i] > bestPerfigure)
					{
						bestPerfigure = maxPerfigures[i];
						playerPerfigure = i;
					}
				for(int i=0;i<4;i++) 
					perfigures[i] = -1;
				perfigures[playerPerfigure] = bestPerfigure;
				ok.Enabled = true;
				comboBox.Enabled = false;
			}
			Invalidate(false);
		}

		private void ok_Click(object sender, System.EventArgs e)
		{
			Visible = false;
			ok.Enabled = false;
			comboBox.Enabled = true;
			comboBox.Items.Clear();

			OK(this,EventArgs.Empty);
		}

        private void Name3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
