using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
namespace Mehran.Game
{
	public class Scores : System.Windows.Forms.Form
	{
		private const int ShowCardHeight = 40;
		private const int ShowCardWidth = 20;
		
		private System.ComponentModel.Container components = null;
		private string[] name = new string[2];
		private System.Windows.Forms.Button ok;
		private int[] score = new int[2];
		private int[] sum = new int[2];
		private Card[][] collects = new Card[2][];
		private int prefigureValue;
		private int prefigurePlayer;

		public string[] Player
		{
			get
			{
				return (string[])name.Clone();
			}
			set
			{
				name = (string[])value.Clone();
			}
		}

		public int[] Score
		{
			get
			{
				return (int[])score.Clone();
			}
			set
			{
				score = (int[])value.Clone();
			}
		}

		public Card[][] Collects
		{
			set
			{
				collects [0]= (Card[])value[0].Clone();
				collects [1]= (Card[])value[1].Clone();
			}
		}

		public int[] Sum
		{
			get
			{
				return (int[])sum.Clone();
			}
		}


		public Scores()
		{
			InitializeComponent();
			ClientSize = new Size(Card.Width + 11 * ShowCardWidth + Card.Width * 2,
				(Card.Heigth + ShowCardHeight * 4) * 2);
			ok.Location = new Point(Size.Width / 2 - 36,ClientSize.Height - 35);
		}

		public DialogResult Show(int PrefigureValue,int PrefigurePlayer)
		{
			prefigureValue = PrefigureValue;
			prefigurePlayer = PrefigurePlayer;
			CalculateRecords();
			for(int p=0;p<2;p++)
				for(int i = 0; i < collects[p].Length; i++)
					collects[p][i].TopLeft = new Point(10 + Card.Width + (i % 12) * ShowCardWidth
						,((p==0)?10:0) + ShowCardHeight * (i / 12) + p * Size.Height / 2);
			return ShowDialog();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing && components != null)
					components.Dispose();
			base.Dispose( disposing );
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			StringFormat sf = new StringFormat(StringFormatFlags.DirectionVertical);
			Font fnt = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif),14,FontStyle.Bold);
			Rectangle nameRect = new Rectangle(0, 0, Card.Width, ClientSize.Height / 2);
			
			#region Draw Border
			Rectangle HR = new Rectangle(0
				, ClientSize.Height / 2 - 5
				, ClientSize.Width
				, 10);
			Rectangle VR = new Rectangle(Card.Width - 5
				, 0
				, 10
				, ClientSize.Height);
			Color startColor = Color.FromArgb(100,100,150);
			Color endColor = Color.FromArgb(250,250,250);
			Blend b =  new Blend();
			b.Factors = new float[]{0, 1, 0};
			b.Positions =  new float[]{0, .5F ,1};
			LinearGradientBrush hLgb=new LinearGradientBrush(HR
				,startColor,endColor,LinearGradientMode.Vertical);
			hLgb.Blend = b;
			LinearGradientBrush vLgb=new LinearGradientBrush(VR
				,startColor,endColor,LinearGradientMode.Horizontal);
			vLgb.Blend = b;
			Rectangle rect = ClientRectangle;
			rect.Inflate(-5,-5);
			e.Graphics.DrawLine(new Pen(vLgb,10)
				,rect.Left,rect.Top - 5,rect.Left,rect.Bottom + 5);
			e.Graphics.DrawLine(new Pen(vLgb,10)
				,rect.Right,rect.Top - 5,rect.Right,rect.Bottom + 5);
			e.Graphics.DrawLine(new Pen(hLgb,10)
				,rect.Left - 5,rect.Top,rect.Right + 5,rect.Top);
			e.Graphics.DrawLine(new Pen(hLgb,10)
				,rect.Left - 5,rect.Bottom,rect.Right + 5,rect.Bottom);
			HR.Offset(4,4);
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150,Color.Black)),HR);
			HR.Offset(-4,-4);
			e.Graphics.FillRectangle(hLgb,HR);
			VR.Offset(4,4);
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150,Color.Black)),VR);
			VR.Offset(-4,-4);
			e.Graphics.FillRectangle(vLgb,VR);
			#endregion

			for(int p=0;p<2;p++)
			{
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Center;
				nameRect.Offset(4,4);
				e.Graphics.DrawString(name[p],fnt,new SolidBrush(Color.FromArgb(150,Color.Black)),nameRect,sf);
				nameRect.Offset(-4,-4);
				e.Graphics.DrawString(name[p],fnt,new SolidBrush(Color.WhiteSmoke),nameRect,sf);
				for(int i = 0; i < collects[p].Length; i++)
					collects[p][i].Draw(e.Graphics);
				nameRect.Offset(0,Size.Height / 2);

				e.Graphics.DrawString(score[p] +"\n"+ sum[p],fnt,Brushes.Black,
					new Rectangle(ClientRectangle.Width - Card.Width + 10
					,(ClientRectangle.Height / 2)*p + 40
					,Card.Width - 10
					,60));
				e.Graphics.DrawLine(Pens.Black
					,ClientRectangle.Width - Card.Width + 10
					,(ClientRectangle.Height / 2)*p + 100
					,ClientRectangle.Width - 30
					,(ClientRectangle.Height / 2)*p + 100);
				e.Graphics.DrawString((score[p]+sum[p]).ToString(),fnt,Brushes.Red,
					new Rectangle(ClientRectangle.Width - Card.Width + 10
					,(ClientRectangle.Height / 2)*p + 105
					,Card.Width - 10
					,30));
			}
			base.OnPaint (e);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ok
            // 
            this.ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ok.Location = new System.Drawing.Point(318, 404);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 0;
            this.ok.Text = "OK";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // Scores
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 16);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(692, 440);
            this.Controls.Add(this.ok);
            this.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Scores";
            this.ShowInTaskbar = false;
            this.Text = "Scores";
            this.Load += new System.EventHandler(this.Scores_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ok_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void CalculateRecords()
		{
			Debug.WriteLine("CalculateRecords");
			Debug.Indent();
			for(int p=0;p<2;p++)
			{
				sum[p] = 0;
				foreach(Card crd  in collects[p])
				{
					if(crd.Value == CardValues.Five)
						sum[p]+=5;
					else if(crd.Value == CardValues.Ten)
						sum[p]+=10;
					else if(crd.Value == CardValues.Ace)
						sum[p]+=10;
				}
				sum[p] += (collects[p].Length / 4) * 5;
				if(prefigurePlayer % 2 == p && prefigureValue > sum[p])
					sum[p] = -prefigureValue;
				else if(sum[p] == 165)
					sum[p] = 330;
				Debug.WriteLine("Sum For " + (p == 0 ? "Computer" : "User") + " = " + sum[p]);
				//scores[p] += sum[p];
			}
			Debug.Unindent();
		}

        private void Scores_Load(object sender, EventArgs e)
        {

        }
    }
}

