#if DEBUG
//#define OUT_PUT_HISTORY
#define OUT_PUT_HISTORY_TOFILE
//#define SHOW_FACE
#endif
/*====================================

 =====================================*/ 
using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Mehran;

namespace Mehran.Game
{
	public class GrandSlam : System.Windows.Forms.Form
	{
		#region View constants
		private const int ShowCardWidth = 20;
		private const int BOBX = 20;
		private const int BOBY = 140;
		private const int Player2X = 140 + ShowCardWidth * 13;
		private const int Player2Y = 20;
		private const int Player3X = 120 + ShowCardWidth * 13 + 120;
		private const int Player3Y = 140 + ShowCardWidth * 13;
		private const int Player4X = 140;
		private const int Player4Y = 120 + ShowCardWidth * 13 + 120;
		private const int CenterCardX = 140;
		private const int CenterCardY = 320;
		private const int StartButtonX = CenterCardX + Card.Width * 2 - 32;
		private const int StartButtonY = CenterCardY + Card.Heigth + 20;
		private const int StartButtonWidth = 75;
		private const int StartButtonHeigth = 23;
		#endregion
		
		private bool delayElapsed = false;
		private delegate int PlayerPlayDelegate (int player);
		private PlayerPlayDelegate[] PlayerPlay = new PlayerPlayDelegate[4];

		private bool startButtonVisible = false;

		private Card[] all = new Card[52];
		private ArrayList[] collects = new ArrayList[2];
		private int[] scores = new int[2];
		private ArrayList[] remainCards = new ArrayList[4];
		private string[] names = new string[4];
		//histor divide in six element partion each partion = (starter, card1, card2, card3, card4, winner) 
		private int[] history = new int[78];
		//private bool[] played = new bool[52];
		private int[] centerCards = new int[4];
		private int selected = -1;
		private CardType trump;
		private int prefigureVlaue;
		private int prefigurestarter = 0;
		private int prefigurePlayer;
		private int deckWinner = -1;
		private int deckNum = -1;
		private int centerCardNum = 0;

		const string debugPath  = @"test.txt";
		const string historyPath  = @"history.txt";

		private PrefigureControl prefigureControl;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem muiGame;
		private System.Windows.Forms.MenuItem muiNew;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem about;
		private System.ComponentModel.IContainer components;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrandSlam));
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.muiGame = new System.Windows.Forms.MenuItem();
            this.muiNew = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.about = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.muiGame,
            this.menuItem1});
            // 
            // muiGame
            // 
            this.muiGame.Index = 0;
            this.muiGame.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.muiNew});
            this.muiGame.Text = "&Game";
            // 
            // muiNew
            // 
            this.muiNew.Index = 0;
            this.muiNew.Text = "&New";
            this.muiNew.Click += new System.EventHandler(this.muiNew_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.about});
            this.menuItem1.Text = "Help";
            this.menuItem1.Click += new System.EventHandler(this.MenuItem1_Click);
            // 
            // about
            // 
            this.about.Index = 0;
            this.about.Text = "Game Rules";
            this.about.Click += new System.EventHandler(this.about_Click);
            // 
            // GrandSlam
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(694, 647);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Menu = this.mainMenu;
            this.Name = "GrandSlam";
            this.Text = "sPADES";
            this.Load += new System.EventHandler(this.GrandSlam_Load);
            this.ResumeLayout(false);

		}
		#endregion
		
		[STAThread]
		static void Main() 
		{
			Application.Run(new GrandSlam());
		}

		
		public GrandSlam()
		{
			if(File.Exists(debugPath))
                File.Delete(debugPath);
			if(File.Exists(historyPath))
				File.Delete(historyPath);
			
            TextWriterTraceListener twtl = new TextWriterTraceListener(debugPath);
			Debug.Listeners.Add(twtl);

			PlayerPlay[0] = new PlayerPlayDelegate(GetFirstPlayerCard);
			PlayerPlay[1] = new PlayerPlayDelegate(GetSecondPlayerCard);
			PlayerPlay[2] = new PlayerPlayDelegate(GetThirdPlayerCard);
			PlayerPlay[3] = new PlayerPlayDelegate(GetFourthPlayerCard);
			collects[0] = new ArrayList();
			collects[1] = new ArrayList();
			for(int i=0;i<4;i++)
			{
				remainCards[i] = new ArrayList();
				//names[i] = "Mark"+(i+1);
                names[0] = "Mark" ;
                names[1] = "james";
                names[2] = "mike";
                names[3] = "You";


            }

			InitializeComponent();
	
			#region prefigure Control
			prefigureControl = new PrefigureControl();
			prefigureControl.Location = new Point(200,200);
			prefigureControl.OK += new EventHandler(Prefigure_OK);
			Controls.Add(prefigureControl);
			#endregion

			for(int i=0;i<4;i++)
				for(int j=0;j<12;j++)
					all[i * 12 + j] = new Card((CardValues)j,(CardType)i,new Point(0,0));
			for(int i=0;i<4;i++)
				all[48 + i] = new Card(CardValues.King,(CardType)i,new Point(0,0));
		}
		//Document
		private void Swop(int x,int y)
		{
			CardType type = all[x].Type;
			CardValues val = all[x].Value;
			all[x].Type  = all[y].Type;
			all[x].Value = all[y].Value;
			all[y].Type  = type;
			all[y].Value = val;
		}

		private void NewPeriod()
		{
			Debug.WriteLine("===============NewPeriod==============");
			for(int i=0;i<4;i++)
				for(int j=0;j<12;j++)
				{
					switch(i)
					{
						case 0:all[i * 12 + j].TopLeft = new Point(BOBX, BOBY + j * ShowCardWidth);break;
						case 1:all[i * 12 + j].TopLeft = new Point(Player2X - j * ShowCardWidth, Player2Y);break;
						case 2:all[i * 12 + j].TopLeft = new Point(Player3X, Player3Y - j * ShowCardWidth);break;
						case 3:all[i * 12 + j].TopLeft = new Point(Player4X + j * ShowCardWidth, Player4Y);break;
					}
				}
			for(int i=0;i<4;i++) all[48 + i].TopLeft = new Point(170 + Card.Heigth,190);
			LowMixCards();
			ArrangeCards();
			for(int i=0;i<4;i++) remainCards[i].Clear();
			for(int cardInd=0; cardInd < 48; cardInd++)	remainCards[cardInd / 12].Add(cardInd);
			for(int i=0;i<4;i++) centerCards[i] = -1;
			
			centerCardNum = 0;
			deckNum = -1;
			deckWinner = -1;

			Invalidate();
			Application.DoEvents();
			
			prefigurestarter++;
			prefigureControl.PlayerNames = names;
			prefigureControl.ShowPrefigures(all,prefigurestarter %= 4);
		}

		private void MixCards()
		{
			Random r = new Random();
			for(int i=0;i<200;i++)
			{
				int x=r.Next(0,51);
				int y=r.Next(0,51);
				Swop(x,y);
			}
		}

		private void LowMixCards()
		{
			Random r = new Random();
			for(int t=0;t<6;t++)
			{
				int x=r.Next(0,51);
				int y=r.Next(0,51);
				int z = r.Next(1,25);
				for(int i=0;i<z;i++)
				{
					if((x + i == 52) || (y + i == 52)) break;
					Swop(x + i, y + i);
				}
			}
		}

		private void Play(int ind)
		{
			//Debug.Listeners.Add(twtl);
			Debug.WriteLineIf(!(
				(ind >= ((centerCardNum + deckWinner) % 4) * 12) && 
				(ind < ((centerCardNum + deckWinner) % 4) * 12 + 12))
				,"//////////////////////////////\n"+"incorrect player play card\n"+"///////////////////////////");
			Debug.Indent();
			Debug.WriteLine("Play : ");
			Debug.Write("\tind = " + ind);
			Debug.WriteLine("\t " + all[ind].ToString());
			int player = ind / 12;

			//change to document
			if(deckNum == 0 && centerCardNum == 0)
				trump = all[ind].Type;
			centerCardNum++;
			centerCards[player] = ind;

			//change to view
			const int centerX = 300;
			const int centerY = 300;
			Graphics grp=CreateGraphics();
			int ramainInd = remainCards[player].IndexOf(ind);
			Debug.WriteLine("\tremainInd Index: " + ramainInd);
			int before = (ramainInd == 0) ? -1: 
				(int)remainCards[player][ramainInd - 1];
			Debug.WriteLine("\tbefore Index: " + before);
			int after = (ramainInd == (int)remainCards[player].Count - 1) 
				? -1: (int)remainCards[player][ramainInd + 1];
			Debug.WriteLine("\tafter Index: " + after);
			bool isCardUnder = false;
			bool isCardAbove = false;
			int clearWidthRight = 0;
			int clearWidthLeft = 0;
			int clearWidthTop = 0;
			int clearWidthBottom = 0;
			int showWidthRight = 0;
			int showWidthLeft = 0;
			int showWidthTop = 0;
			int showWidthBottom = 0;

			switch(player)
			{
				case 0://BOB
					showWidthLeft = clearWidthLeft = BOBX;
					showWidthRight = clearWidthRight = BOBX + Card.Width;
					isCardUnder = (before == -1) ? false : all[ind].ClientRect.Top <  all[before].ClientRect.Bottom;
					isCardAbove = (after == -1) ? false : all[after].ClientRect.Top < all[ind].ClientRect.Bottom;
					showWidthTop = all[ind].ClientRect.Top;
					showWidthBottom = isCardAbove ? all[after].ClientRect.Top : all[ind].ClientRect.Top + Card.Heigth;
					clearWidthTop = isCardUnder ? all[before].ClientRect.Bottom : all[ind].ClientRect.Top;
					clearWidthBottom = isCardAbove ? all[after].ClientRect.Top : all[ind].ClientRect.Bottom;
					all[ind].TopLeft = new Point(centerX - (Card.Width / 2 + 10), centerY - (Card.Heigth / 2 + 5));
					break;
				case 1://player2
					showWidthTop = clearWidthTop = Player2Y;
					showWidthBottom = clearWidthBottom = Player2Y + Card.Heigth;
					isCardUnder = (before == -1) ? false : all[before].ClientRect.Left < all[ind].ClientRect.Right;
					isCardAbove = (after == -1) ? false : all[ind].ClientRect.Left < all[after].ClientRect.Right;
					showWidthLeft = isCardAbove ? all[after].ClientRect.Right : all[ind].ClientRect.Right - Card.Width;
					showWidthRight = all[ind].ClientRect.Right;
					clearWidthRight = isCardUnder ? all[before].ClientRect.Left : all[ind].ClientRect.Right;
					clearWidthLeft = isCardAbove ? all[after].ClientRect.Right : all[ind].ClientRect.Left;
					all[ind].TopLeft = new Point(centerX - 5, centerY - (Card.Heigth / 2 + 10));
					break;
				case 2://player3
					showWidthLeft = clearWidthLeft = Player3X;
					showWidthRight = clearWidthRight = Player3X + Card.Width;
					isCardUnder = (before == -1) ? false : all[before].ClientRect.Top < all[ind].ClientRect.Bottom;
					isCardAbove = (after == -1) ? false : all[ind].ClientRect.Top < all[after].ClientRect.Bottom;
					showWidthTop = isCardAbove ? all[after].ClientRect.Bottom : all[ind].ClientRect.Top;
					showWidthBottom = all[ind].ClientRect.Bottom;
					clearWidthTop = isCardAbove ? all[after].ClientRect.Bottom : all[ind].ClientRect.Top;
					clearWidthBottom = isCardUnder ? all[before].ClientRect.Top : all[ind].ClientRect.Bottom;
					all[ind].TopLeft = new Point(centerX, centerY - 5);
					break;
				case 3://player4
					showWidthTop = clearWidthTop = Player4Y;
					showWidthBottom = clearWidthBottom = Player4Y + Card.Heigth;
					isCardUnder = (before == -1) ? false : all[ind].ClientRect.Left < all[before].ClientRect.Right;
					isCardAbove = (after == -1) ? false : all[after].ClientRect.Left < all[ind].ClientRect.Right;
					showWidthLeft = all[ind].ClientRect.Left;
					showWidthRight = isCardAbove ? all[after].ClientRect.Left : all[ind].ClientRect.Left + Card.Width;
					clearWidthRight = isCardAbove ? all[after].ClientRect.Left : all[ind].ClientRect.Right;
					clearWidthLeft = isCardUnder ? all[before].ClientRect.Right : all[ind].ClientRect.Left;
					all[ind].TopLeft = new Point(centerX - (Card.Width / 2 + 15), centerY - 10);
					break;
			}
			//Debug.WriteLine("\t show restangle: " + new Rectangle(showWidthLeft, showWidthTop, showWidthRight - showWidthLeft, showWidthBottom - showWidthTop).ToString());
			grp.SetClip(
				new Rectangle(showWidthLeft
				, showWidthTop
				, showWidthRight - showWidthLeft
				, showWidthBottom - showWidthTop));
			

			if((clearWidthRight > clearWidthLeft) && (clearWidthTop < clearWidthBottom))
				grp.FillRectangle(new TextureBrush(BackgroundImage)
					, clearWidthLeft, clearWidthTop, clearWidthRight - clearWidthLeft, clearWidthBottom - clearWidthTop);
			if(isCardUnder)
			{
				if(player == 3)
					all[before].Draw(grp);	
				else
				{
#if SHOW_FACE
				all[ind].Draw(grp);	
#else
					all[before].DrawFaceBack(grp);	
#endif
				}

			}
			grp.SetClip(all[ind].ClientRect);
			all[ind].Draw(grp);
			Debug.Unindent();
		}
		
		private bool IsBetter(int cardInd1,int cardInd2)
		{
			if(all[cardInd1].Type != all[cardInd2].Type) throw new Exception("Two card should be same type");
			if(all[cardInd2].Value == CardValues.Ace)
				return false;
			return (all[cardInd1].Value == CardValues.Ace) || (all[cardInd1] > all[cardInd2]);
		}
		private void ArrangeCards()
		{
			for(int player=0;player<4; player++)
			{
				int startType = player * 12;
				int endType = startType;
				foreach(CardType ct in Enum.GetValues(typeof(CardType)))
				{
					for(int j = player * 12;j < player * 12 + 12;j++)
						if(all[j].Type == ct)
							Swop(endType++, j);
					for(int i=startType;i<endType - 1;i++)
						for(int j=i + 1;j<endType;j++)
							if(IsBetter(j,i))
								Swop(j,i);
					startType = endType;
				}
			}
		}
		private void NewDeck()
		{
			Debug.Unindent();
			Debug.WriteLine("NewDeck : ");
			Debug.Write("\tDeck = ");
			Debug.WriteLine(deckNum + 1);
			Debug.Indent();

			centerCardNum = 0;
			deckNum ++;
			for(int i=deckWinner;i<3;i++)
				Play(PlayerPlay[centerCardNum](i));
		}

		private void CollectCenter()
		{
			Debug.WriteLine("CollectCenter : ");
			Debug.Write("\tDeck = ");
			Debug.WriteLine(deckNum);
			
			history[deckNum * 6] =	deckWinner;
			history[deckNum * 6 + 5] = deckWinner = GetCurrentWinner();
			for(int i=0;i<4;i++) 
			{
				remainCards[i].Remove(centerCards[i]);
				history[deckNum * 6 + i + 1] = centerCards[i];
				collects[deckWinner % 2].Add(centerCards[i]);
				centerCards[i] = -1;
			}
			this.CreateGraphics().FillRectangle(new TextureBrush(BackgroundImage),230,200,142,200);
#if OUT_PUT_HISTORY
				string t = "" 
					+ "Starter = " + history[deckNum * 6].ToString().PadLeft(5) + "   "
					+ all[history[deckNum * 6 + 1]].ToString().PadLeft(15) + "  "
					+ all[history[deckNum * 6 + 2]].ToString().PadLeft(15) + "  "
					+ all[history[deckNum * 6 + 3]].ToString().PadLeft(15) + "  "
					+ all[history[deckNum * 6 + 4]].ToString().PadLeft(15) + "  "
					+ "Winner = " + history[deckNum * 6 + 5].ToString().PadLeft(5);
				this.CreateGraphics().DrawString(t,new Font("arial",10),Brushes.Black,550,15 * deckNum + 1);
#endif
#if	OUT_PUT_HISTORY_TOFILE
			using(StreamWriter sw = new StreamWriter(historyPath,true))
			{
				string t = "" 
					+ "Starter = " + history[deckNum * 6].ToString().PadLeft(5) + "   "
					+ all[history[deckNum * 6 + 1]].ToString().PadLeft(15) + "  "
					+ all[history[deckNum * 6 + 2]].ToString().PadLeft(15) + "  "
					+ all[history[deckNum * 6 + 3]].ToString().PadLeft(15) + "  "
					+ all[history[deckNum * 6 + 4]].ToString().PadLeft(15) + "  "
					+ "Winner = " + history[deckNum * 6 + 5].ToString().PadLeft(5);
				sw.WriteLine(t);
				sw.Close();
			}
#endif
			if(deckNum == 11)
			{
				centerCardNum = 0;
				deckNum = -1;
				deckWinner = -1;
				Scores s = new Scores();
				s.Player = new string[]{"Computer","User"};
				Card[][] tempCard= new Card[2][];
				for(int p = 0; p < 2; p++)
				{
					tempCard[p] = new Card[collects[p].Count];
					for(int i=0;i<collects[p].Count;i++)
						tempCard[p][i] =(Card) all[(int)collects[p][i]].Clone();
					collects[p].Clear();
				}
				NewPeriod();
				s.Collects = tempCard;
				s.Score = scores;
				s.Show(prefigureVlaue,prefigurePlayer);
				scores[0] += s.Sum[0];
				scores[1] += s.Sum[1];
			}
			else NewDeck();
		}

		private int GetCurrentWinner()
		{
			ArrayList beters = new ArrayList();
			foreach(int i in centerCards) 
				if(i != -1 && all[i].Type == trump)
					beters.Add(i);
			if(beters.Count == 0)
			{
				beters.Add(centerCards[deckWinner]);
				foreach(int i in centerCards) 
					if(i != -1 && all[i].Type == all[centerCards[deckWinner]].Type)
						beters.Add(i);
			}
			int best = (int)beters[0];
			foreach(int i in beters)
				if(IsBetter(i,best))
					best = i;
			return best /12;
		}

		//work only for first deck
		private int GetFriendCutCard(int player)
		{//if exist suit that friend cut or pass a not trump suit and after that don't pass that suit
			Debug.WriteLine("GetFriendCutCard:");
			Debug.WriteLine("\tPlayer = " + player);
			int friend = (player + 2) % 4;
			for(int deckCount = 0; deckCount < deckNum; deckCount++)
			{
				CardType friendPlaySuit = all[history[deckCount * 6 + friend + 1]].Type;
				CardType startSuit = all[history[deckCount * 6 + history[deckCount * 6] + 1]].Type;
				if(startSuit == trump)//if deck suit is trump
					if(friendPlaySuit != trump)//friend haven't trump suit becuase pass trump suit
						return -1;
				if(startSuit != friendPlaySuit && friendPlaySuit!=trump)//friend pass
					if(history[deckCount * 6 + 5] != player)//I wasn't winner
						return -1;
			}
			Debug.WriteLine("\tPossible friend have trump:");
			for(int deckCount = 0; deckCount < deckNum; deckCount++)
			{//find suits that friend cut or pass
				int starter = history[deckCount * 6];
				CardType  startType = all[history[deckCount * 6 + starter + 1]].Type;
			//	Debug.WriteLine("\t\tSearch <//> deck = " + deckCount + " <//> starter player = " + starter + " <//> start suit = " + startType.ToString());
				if(starter != friend && startType != trump)//don't seek trump deck
					if(startType != all[history[deckCount * 6 + friend + 1]].Type)//if friend cut or pass 
					{
						CardType cutOrPassSuit = all[history[deckCount * 6 + friend + 1]].Type;
						Debug.WriteLine("\t\tFriend cut or pass " + startType);
						int[] suitNum = new int[4];
						int[] suitStarts = new int[4];
						GetRemainedCardsNum(player,ref suitNum,ref suitStarts);
						ArrayList myCards = remainCards[player];
						if(suitNum[(int)cutOrPassSuit] > 0)
						{
							for(int i = suitStarts[(int)cutOrPassSuit]
									;i <  suitStarts[(int)cutOrPassSuit] + suitNum[(int)cutOrPassSuit]
								;i++)
								if(all[(int)myCards[i]].Value != CardValues.Ace 
									&& all[(int)myCards[i]].Value != CardValues.Five
									&& all[(int)myCards[i]].Value < CardValues.Ten)
									return (int)myCards[i];
						}
					}
			}
			Debug.WriteLine("\tFriend haven't cut or pass card");
			return -1;
		}

		private int GetFeatureUp(int player)
		{
			int[] suitNum = new int[4];
			int[] suitStarts = new int[4];
			GetRemainedCardsNum(player,ref suitNum,ref suitStarts);
			ArrayList myCards = remainCards[player];
			for(int i=0;i<4;i++)
				if(suitNum[i] > 1 && (CardType)i != trump 
					&& Rank((int)myCards[suitStarts [i]]) < 3
					&& !CuttedOrPassed(0, (CardType)i)
					&& !CuttedOrPassed(1, (CardType)i)
					&& !CuttedOrPassed(2, (CardType)i)
					&& !CuttedOrPassed(3, (CardType)i))
				{
					for(int j = suitNum[i]; j > 0; j++)
						if(all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Five
							&& all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Ten)
						{
							return (int)myCards[suitStarts [i] + j];
						}
				}
			for(int i=0;i<4;i++)
				if(suitNum[i] > 1 && (CardType)i != trump 
					&& Rank((int)myCards[suitStarts [i]]) < 4
					&& !CuttedOrPassed(0, (CardType)i)
					&& !CuttedOrPassed(1, (CardType)i)
					&& !CuttedOrPassed(2, (CardType)i)
					&& !CuttedOrPassed(3, (CardType)i))
				{
					for(int j = suitNum[i]; j > 0; j++)
						if(all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Five
							&& all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Ten
							&& Rank((int)myCards[suitStarts [i] + j]) < 6)
						{
							return (int)myCards[suitStarts [i] + j];
						}
				}
			return -1;

		}

		private int GetBadCard(int player,ref int[] suitNum,ref int[] suitStarts)
		{
			Debug.WriteLine("\tGetBadCard:");
			ArrayList myCards = remainCards[player];
			int worstRank = 0;
			int worstRankInd = -1;
			for(int i=1;i<4;i++)
				if((CardType)i != trump	&& suitStarts[i] - 1 > 0)
				{
					int r = Rank((int)myCards[suitStarts[i] - 1]);
					if(r > worstRank)
					{
						worstRank = r;
						worstRankInd = (int)myCards[suitStarts[i] - 1];
					}
				}
			if((CardType)0 != trump)
			{
				int r = Rank((int)myCards[myCards.Count - 1]);
				if(r > worstRank)
				{
					worstRank = r;
					worstRankInd = (int)myCards[myCards.Count - 1];
				}
			}
			if(worstRankInd != -1)
				return worstRankInd;
			if(trump == (CardType)0)
				return (int)myCards[myCards.Count - 1];
			return (int)myCards[suitStarts[(int)trump]- 1];
		}

		private int GetFirstPlayerCard(int player)
		{
			//Debug.Listeners.Add(twtl);
			Debug.WriteLine("GetFirstPlayerCard:");
			Debug.WriteLine("\tPlayer = " + player);
			if(deckNum == 0)//if this deck is firat deck
			{
				#region Find best suit for trump and play one card of trump suit and also give up down cards
				int[] suitNums = new int[4];
				int[] suitStarts = new int[4];
				
				#region Find best suit for trump

				#region arrange down card with my card
				for(int i=0;i<4;i++) suitNums[i] = suitStarts[i] = 0;
				int[] myCurCards = new int[16];
				for(int i=0;i<12;i++) myCurCards[i] = player * 12 + i;
				for(int i=0;i<4;i++) myCurCards[12 + i] = 48 + i;
				int startType = 0;
				int endType = 0;
				foreach(CardType ct in Enum.GetValues(typeof(CardType)))
				{
					for(int i=startType;i<16;i++)
						if(all[myCurCards[i]].Type == ct)
						{
							int t = myCurCards[i];
							myCurCards[i] = myCurCards[endType];
							myCurCards[endType] = t;
							endType++;
						}
					for(int i=startType;i<endType - 1;i++)
						for(int j=i + 1;j<endType;j++)
							if(IsBetter(myCurCards[j],myCurCards[i]))
							{
								int t = myCurCards[i];
								myCurCards[i] = myCurCards[j];
								myCurCards[j] = t;
							}
					suitStarts[(int)ct] = startType;
					suitNums[(int)ct] = endType - startType;
					startType = endType;
				}
				#endregion
				
				int bestGrade = int.MinValue;
				CardType bestSuit = CardType.Club;
				for(int c=0;c<4;c++)
				{
					int thisGrade = 0;
					for(int i = suitStarts[c]; i < suitStarts[c] + suitNums[c] ;i++)
						thisGrade += (all[myCurCards[i]].Value == CardValues.Ace) 
							? 14 : (int)all[myCurCards[i]].Value + 1;
					if(thisGrade > bestGrade)
					{
						bestGrade = thisGrade;
						bestSuit = (CardType)c;
					}
				}
				Debug.Write("\tI find best suit card =  ");Debug.WriteLine(bestSuit);
				#endregion
				
				#region give up down cards
				Debug.WriteLine("\tDown Cards:");
				Debug.WriteLine("\t\t"+all[48].ToString());
				Debug.WriteLine("\t\t"+all[49].ToString());
				Debug.WriteLine("\t\t"+all[50].ToString());
				Debug.WriteLine("\t\t"+all[51].ToString());

				int[] sortNumInd = new int[4];
				for(int i=0;i<4;i++) sortNumInd[i] = i;
				sortNumInd[3] = (int)bestSuit;
				sortNumInd[(int)bestSuit] = 3;				
				for(int i=0;i<2;i++)//sort sortNumInd
					for(int j=i+1;j<3;j++)
						if(suitNums[sortNumInd[j]] < suitNums[sortNumInd[i]])
						{
							int t = sortNumInd[i];
							sortNumInd[i] = sortNumInd[j];
							sortNumInd[j] = t;
						}
				int changeCount = 1;
				ArrayList lowChangeInd = new ArrayList();
				ArrayList hiChangeInd = new ArrayList();
				for(int i=0;i<4;i++) hiChangeInd.Add(48 + i);
				for(int i=0;i<3;i++)
				{
					if(changeCount == 5) break;
					for(int j=suitStarts[sortNumInd[i]] + suitNums[sortNumInd[i]] -1; j >= suitStarts[sortNumInd[i]]; j--)
						if(Rank(myCurCards[j]) > 4)
						{
							if(myCurCards[j] >= 48)
								hiChangeInd.Remove(myCurCards[j]);
							else
								lowChangeInd.Add(myCurCards[j]);
							changeCount++;
							if(changeCount == 5) break;
						}
				}
				for(int i=0;i<lowChangeInd.Count;i++)
				{
					Swop((int)lowChangeInd[i],(int)hiChangeInd[i]);
					Debug.WriteLine("\tchange " + all[(int)lowChangeInd[i]].ToString() + "(" + lowChangeInd[i].ToString() + ")"
						+" with "+ all[(int)hiChangeInd[i]].ToString() + "(" + hiChangeInd[i].ToString() + ")");
				}
				ArrangeCards();
				for(int i=0;i<4;i++) collects[deckWinner % 2].Add(48 + i);
				#endregion
				
				GetRemainedCardsNum(player,ref suitNums,ref suitStarts);
				ArrayList myCards = remainCards[player];
				if(all[(int)myCards[suitStarts[(int)bestSuit]]].Value == CardValues.Ace)//if i have Ace of this suit
					return (int)myCards[suitStarts[(int)bestSuit]];//play ace
				else
				{//Play worst crad of this suit
					Debug.WriteLine("\tI haven't ace of trump so start with a bad card of trump suit");
					int suitEnd = suitStarts[(int)bestSuit] + 1;
					while(suitEnd < myCards.Count && all[(int)myCards[suitEnd]].Type == bestSuit) suitEnd++;
					suitEnd--;
					for(int i = suitEnd; i >= suitStarts[(int)bestSuit]; i--)
						if(all[(int)myCards[i]].Value != CardValues.Five 
							&& all[(int)myCards[i]].Value != CardValues.Ten
							&& all[(int)myCards[i]].Value != CardValues.Ace)
							return (int)myCards[i];
					return (int)myCards[suitEnd];
				}
				#endregion
			}
			else//this deck isn't first deck
			{
				int cardInd;
				int[] suitNum = new int[4];
				int[] suitStarts = new int[4];
				int vier1 = (player + 1) % 4;
				int vier2 = (player + 3) % 4;
				GetRemainedCardsNum(player,ref suitNum,ref suitStarts);
				ArrayList myCards = remainCards[player];

				#region Find best up card
				Debug.WriteLine("I search best up card");
				int[] upCards = new int[4];
				for(int i=0;i<4;i++)//Siuts Except the trump suit
					upCards[i] = (suitNum[i] != 0 && Rank((int)myCards[suitStarts[i]]) == 1)
						? (int)myCards[suitStarts[i]] : -1;
				for(int i=0;i<4;i++)
					if((CardType)i != trump && upCards[i] != -1)
					{
						if((!CuttedOrPassed(vier1, (CardType)i) || CuttedOrPassed(vier1, trump))&&
							(!CuttedOrPassed(vier2, (CardType)i) || CuttedOrPassed(vier2, trump)))
							return upCards[i];
					}
				Debug.WriteLine("I can't find complete best up card");
				if((cardInd = GetFriendCutCard(player)) != -1)
					return cardInd;
				Debug.WriteLine("Frind Cut or passed card not found");
				if(!CuttedOrPassed(vier1, trump) && !CuttedOrPassed(vier2, trump))
					if(upCards[(int)trump] != -1)
						return upCards[(int)trump];
				for(int i=0;i<4;i++)
					if(upCards[i] != -1)
						return upCards[i];
				#endregion
				
				#region Find feature cut card
				Debug.WriteLine("I search feature cut card");
				for(int i=0;i<4;i++)
					if(suitNum[i] == 1 && (CardType)i != trump
						&& all[(int)myCards[suitStarts [i]]].Value != CardValues.Five
						&& all[(int)myCards[suitStarts [i]]].Value != CardValues.Ten
						&& Rank((int)myCards[suitStarts [i]]) > 3 - (GetPlayedOf(all[(int)myCards[suitStarts [i]]].Type).Count / 4))
					{
						return (int)myCards[suitStarts[i]];
					}
				for(int i=0;i<4;i++)
					if(suitNum[i] == 2 && (CardType)i != trump
						&& all[(int)myCards[suitStarts [i]]].Value != CardValues.Five
						&& all[(int)myCards[suitStarts [i]]].Value != CardValues.Ten
						&& all[(int)myCards[suitStarts [i] + 1]].Value != CardValues.Five
						&& all[(int)myCards[suitStarts [i] + 1]].Value != CardValues.Ten
						&& Rank((int)myCards[suitStarts [i]]) > 2 - (GetPlayedOf(all[suitStarts [i]].Type).Count / 4)
						&& Rank((int)myCards[suitStarts [i] + 1]) > 2 - (GetPlayedOf(all[suitStarts [i]].Type).Count / 4))
					{
						return (int)myCards[suitStarts [i] + 1];
					}
				Debug.WriteLine("I can't find feature cut card");
				#endregion
				
				#region Find feature up card
				Debug.WriteLine("I search feature up card");
				for(int i=0;i<4;i++)
					if(suitNum[i] > 1 && (CardType)i != trump 
						&& Rank((int)myCards[suitStarts [i]]) < 3
						&& !CuttedOrPassed((player + 1) % 2, (CardType)i)
						&& !CuttedOrPassed((player + 1) % 2 + 2, (CardType)i))
					{
						for(int j = suitNum[i] - 1; j >= 0; j--)
							if(all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Five
								&& all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Ten)
							{
								return (int)myCards[suitStarts [i] + j];
							}
					}
				for(int i=0;i<4;i++)
					if(suitNum[i] > 1 && (CardType)i != trump 
						&& Rank((int)myCards[suitStarts [i]]) < 4
						&& !CuttedOrPassed((player + 1) % 2, (CardType)i)
						&& !CuttedOrPassed((player + 1) % 2 + 2, (CardType)i))
					{
						for(int j = suitNum[i] - 1; j >= 0; j--)
							if(all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Five
								&& all[(int)myCards[suitStarts [i] + j]].Value != CardValues.Ten
								&& Rank((int)myCards[suitStarts [i] + j]) < 6)
							{
								return (int)myCards[suitStarts [i] + j];
							}
					}
				Debug.WriteLine("I can't find feature up card");
				#endregion

				if(suitNum[(int)trump] != 1 && upCards[(int)trump] != -1)
					return upCards[(int)trump];
				
				return GetBadCard(player,ref suitNum,ref suitStarts);
			}
		}

		private int GetSecondPlayerCard(int player)
		{
			Debug.WriteLine("GetSecondPlayerCard:");
			Debug.Write("\tPlayer = ");Debug.WriteLine(player);
			return GetWinOrPassCard(player);
		}

		private int GetThirdPlayerCard(int player)
		{
			Debug.WriteLine("GetThirdPlayerCard:");
			Debug.Write("\tPlayer = ");Debug.WriteLine(player);
			if(GetCurrentWinner() == (player + 2) % 4)// if friend is winner
			{
					if(Rank(centerCards[(player + 2) % 4])==1)
						return GetFatCard(player);
			}
			return GetWinOrPassCard(player);
		}

		private int GetFourthPlayerCard(int player)
		{
			Debug.Write("GetFourthPlayerCard:");
			Debug.Write("\tPlayer = ");Debug.WriteLine(player);
			int[] upCards = new int[4];
			int[] suitNum = new int[4];
			int[] suitStarts = new int[4];
			GetRemainedCardsNum(player,ref suitNum,ref suitStarts);
			ArrayList myCards = remainCards[player];
			CardType deckSuit = all[centerCards[deckWinner]].Type;
			int suitEnd = suitStarts[(int)deckSuit] + suitNum[(int)deckSuit] - 1;
			if(GetCurrentWinner() == (player + 2) % 4)// if friend is winner
				return GetFatCard(player);
			else
			{
				if(suitNum[(int)deckSuit] != 0)//if i have deck suit
				{
					#region I have deck suit
					Debug.WriteLine("\tI have deck suit");
					int vierInd = centerCards[(player + 3) % 4];
					if((all[vierInd].Type != trump) || (all[vierInd].Type == trump  && deckSuit == trump))
					{//if vier wasn't cut
						for(int i = suitEnd; i >= suitStarts[(int)deckSuit]; i--)
							if(IsBetter((int)myCards[i],centerCards[deckWinner])
								&&(all[(int)myCards[i]].Type != all[vierInd].Type 
								|| IsBetter((int)myCards[i],vierInd) ))
							{
								return (int)myCards[i];
							}
					}
					for(int i = suitEnd;i >= suitStarts[(int)deckSuit];i--)
						if(all[(int)myCards[i]].Value != CardValues.Ten
							&& all[(int)myCards[i]].Value != CardValues.Five
							&& all[(int)myCards[i]].Value != CardValues.Ten)
						{
							return (int)myCards[i];					
						}
					return (int)myCards[suitEnd];
					#endregion
				}
				else//if i haven't deck suit
				{
					#region I haven't deck suit
					Debug.WriteLine("\tI haven't deck suit");
					if(suitNum[(int)trump] != 0)//if i have trump
					{
						Debug.WriteLine("\tI have trump");
						for(int i = suitStarts[(int)trump]; i < suitStarts[(int)trump] + suitNum[(int)trump]; i++)
							if(all[(int)myCards[i]].Value == CardValues.Ten
								|| all[(int)myCards[i]].Value == CardValues.Five)
								return (int)myCards[i];
						if(Rank((int)myCards[suitStarts[(int)deckSuit]]) > 2)
							return (int)myCards[suitStarts[(int)deckSuit]];
						if(suitNum[(int)trump] + GetPlayedOf(trump).Count >= 12)
							return (int)myCards[suitStarts[(int)trump] + suitNum[(int)trump] - 1];
					}
					for(int i=0;i<4;i++)
						if(suitNum[i] == 1 && Rank((int)myCards[suitStarts[i]]) > 2)
							return (int)myCards[suitStarts[i]];
					return GetBadCard(player,ref suitNum,ref suitStarts);
					#endregion
				}
			}
		}

		private int GetFatCard(int player)
		{
			Debug.WriteLine("GetFatCard");
			Debug.Write("\tPlayer = ");
			Debug.WriteLine(player);
			int[] upCards = new int[4];
			int[] suitNum = new int[4];
			int[] suitStarts = new int[4];
			GetRemainedCardsNum(player,ref suitNum,ref suitStarts);
			ArrayList myCards = remainCards[player];
			CardType deckSuit = all[centerCards[deckWinner]].Type;
			int suitEnd = suitStarts[(int)deckSuit] + suitNum[(int)deckSuit] - 1;
			if(suitNum[(int)deckSuit] != 0)//if i have deck suit
			{
				Debug.WriteLine("\tI have deck suit");
				for(int i = suitStarts[(int)deckSuit]; i <= suitEnd; i++)
					if(all[(int)myCards[i]].Value == CardValues.Ten//if i can make deck full
						|| all[(int)myCards[i]].Value == CardValues.Five)
					{
						return (int)myCards[i];
					}
				Debug.WriteLine("\tI haven't good deck suit");
				return (int)myCards[suitEnd];
			}
			else//if i haven't deck suit
			{
				Debug.WriteLine("\tI haven't deck suit");
				for(int j=0;j<4;j++)
					if(suitNum[j] > 0  && (CardType)j != trump)
						for(int i = suitStarts[j]; i < suitStarts[j] + suitNum[j]; i++)
							if(all[(int)myCards[i]].Value == CardValues.Ten//if i can make deck full
								|| all[(int)myCards[i]].Value == CardValues.Five)
							{
								return (int)myCards[i];
							}
				Debug.WriteLine("\tI haven't good trump card");
				for(int i=0;i<4;i++)
					if(suitNum[i] == 1 && Rank((int)myCards[suitStarts[i]]) > 2)
						return (int)myCards[suitStarts[i]];
				Debug.WriteLine("\tI haven't pass card");
				return GetBadCard(player,ref suitNum,ref suitStarts);
			}
		}
		private int GetWinOrPassCard(int player)
		{
			Debug.WriteLine("GetWinOrPassCard");
			Debug.Write("\tPlayer = ");
			Debug.WriteLine(player);
			int[] upCards = new int[4];
			int[] suitNum = new int[4];
			int[] suitStarts = new int[4];
			GetRemainedCardsNum(player,ref suitNum,ref suitStarts);
			ArrayList myCards = remainCards[player];
			CardType deckSuit = all[centerCards[deckWinner]].Type;
			int suitEnd = suitStarts[(int)deckSuit] + suitNum[(int)deckSuit] - 1;
			if(suitNum[(int)deckSuit] != 0)//if i have deck suit
			{
				#region if i have suit
				Debug.WriteLine("\tI have deck suit");
				if(Rank((int)myCards[suitStarts[(int)deckSuit]]) < 4)
					if(IsBetter((int)myCards[suitStarts[(int)deckSuit]],centerCards[deckWinner]))
						return (int)myCards[suitStarts[(int)deckSuit]];
				Debug.WriteLine("\tI haven't good deck suit");
				for(int i = suitEnd;i >= suitStarts[(int)deckSuit];i--)
					if(all[(int)myCards[i]].Value != CardValues.Ace
						&& all[(int)myCards[i]].Value != CardValues.Five
						&& all[(int)myCards[i]].Value != CardValues.Ten)
					{
						return (int)myCards[i];					
					}
				return (int)myCards[suitEnd];					
				#endregion
			}
			else//if i haven't deck suit
			{
				#region if i haven't deck suit
				Debug.WriteLine("\tI haven't deck suit");
				for(int i = suitStarts[(int)trump] + suitNum[(int)trump] - 1; i >= suitStarts[(int)trump]; i--)
					if(Rank((int)myCards[i]) > 1 )
					{
						bool CanCut = true;
						for(int j = 0;j<centerCardNum;j++)
							if(all[centerCards[(deckWinner + j) % 4]].Type == trump
								&& all[centerCards[(deckWinner + j) % 4]].Value > all[(int)myCards[i]].Value)
								CanCut = false;
						if(CanCut)	return (int)myCards[i];
					}
				Debug.WriteLine("\tI haven't good trump card");
				for(int i=0;i<4;i++)
					if(suitNum[i] == 1 && Rank((int)myCards[suitStarts[i]]) > 2)
						return (int)myCards[suitStarts[i]];
				Debug.WriteLine("\tI haven't good pass card");
				return GetBadCard(player,ref suitNum,ref suitStarts);
				#endregion
			}
		}

		private void GetRemainedCardsNum(int player,ref int[] num,ref int[] starts)
		{
			Debug.Indent();
			Debug.Write("GetRemainedCardsNum: ");
			Debug.WriteLine("\tPlayer = "+player);
			for(int i=0;i<4;i++) num[i] = starts[i] = 0;
			int counter = 0;
			ArrayList myCards = remainCards[player];
			foreach(CardType ct in Enum.GetValues(typeof(CardType)))
			{
				while(counter < myCards.Count && all[(int)myCards[counter]].Type == ct)
				{
					if(num[(int)ct] == 0)
					{
						Debug.WriteLine("\tSuitStarts[" + (int)ct + "] = " + counter);
						starts[(int)ct] = counter;
					}
					counter++;
					num[(int)ct]++;
				}
				Debug.WriteLine("\tSuitNums[" + (int)ct + "] = " + num[(int)ct]);
			}
			Debug.Unindent();

		}

		private bool CuttedOrPassed(int Player,CardType Suit)
		{
			for(int deckCount = 0; deckCount < deckNum; deckCount++)
				if(all[history[deckCount * 6 + Player + 1]].Type != 
					all[history[deckCount * 6 + history[deckCount * 6] +1]].Type)
					return true;
			return false;
		}

		private int Rank(int cardInd)
		{
			if(all[cardInd].Value == CardValues.Ace) 
				return 1;
			int rank = (int)CardValues.King - (int)all[cardInd].Value + 2;
			ArrayList PlayedCards = GetPlayedOf(all[cardInd].Type);
			for(int i=0;i<PlayedCards.Count;i++)
				if(IsBetter((int)PlayedCards[i],cardInd))
					rank--;
			return rank;
		}

		private CardValues Upper(CardValues cv)
		{
			if(cv == CardValues.Ace) throw new ArgumentException("Ace haven't upper card","cv");
			if(cv == CardValues.King) return CardValues.Ace;
			return (CardValues)((int)cv + 1);
		}

		private CardValues Lower(CardValues cv)
		{
			if(cv == CardValues.Two) throw new ArgumentException("Two haven't lower card","cv");
			if(cv == CardValues.Ace) return CardValues.King;
			return (CardValues)((int)cv - 1);
		}

		private ArrayList GetPlayedOf(CardType suit)
		{
			ArrayList ret = new ArrayList();
			foreach(int c in collects[0])
				if(all[c].Type == suit)
					ret.Add(c);
			foreach(int c in collects[1])
				if(all[c].Type == suit)
					ret.Add(c);
			return ret;
		}

		//View 
		private int GetBefore(int ind)
		{
			int remainCardsInd = remainCards[ind / 12].IndexOf(ind);
			if(remainCardsInd == -1 || remainCardsInd == 0)
				return -1;
			return (int)remainCards[ind / 12][remainCardsInd - 1];
		}
		
		private int GetAfter(int ind)
		{
			int remainCardsInd = remainCards[ind / 12].IndexOf(ind);
			if(remainCardsInd == -1 || remainCardsInd == remainCards[ind / 12].Count - 1)
				return -1;
			return (int)remainCards[ind / 12][remainCardsInd + 1];
		}
		
		private void DrawSelected(Graphics grp)
		{
			Debug.WriteLine("DrawSelected");
			Debug.Indent();
			int before = (selected != 0) ? selected + 36 - 1 : -1;// GetBefore(selected + 36);
			int after = (selected != 11) ? selected + 36 + 1 : -1;//GetAfter(selected + 36);
//			bool isCardUnder = (before == -1) ? false :all[selected + 36].ClientRect.Left < all[before].ClientRect.Right;
//			bool isCardAbove = (after == -1) ? false :all[after].ClientRect.Left < all[selected + 36].ClientRect.Right;
			bool isCardUnder = (before != -1);
			bool isCardAbove = (selected != 11);
			Debug.WriteLineIf(isCardAbove,"Is another card above");
			Debug.WriteLineIf(isCardUnder,"Is another card Under");
			int clearWidth = 0;
			int showWidth = 0;
			int underShowWidth = 0 ;
			if(isCardUnder)
			{
				if(isCardAbove)
				{
					clearWidth = all[after].ClientRect.Left - all[before].ClientRect.Right;
					underShowWidth = all[after].ClientRect.Left - all[selected + 36].ClientRect.Left;
					if(clearWidth > 0) underShowWidth -= clearWidth;
				}
				else
				{
					clearWidth = all[selected + 36].ClientRect.Right - all[before].ClientRect.Right;
					underShowWidth = all[before].ClientRect.Right - all[selected + 36].ClientRect.Left;
				}
				showWidth = underShowWidth;
				grp.SetClip(new Rectangle(Player4X + selected * ShowCardWidth 
					, Player4Y + Card.Heigth - 20
					, underShowWidth
					, 20));
				all[before].Draw(grp);
			}
			else 
			{
				clearWidth = (isCardAbove) ? 
					all[after].ClientRect.Left - all[selected + 36].ClientRect.Left : Card.Width;
			}
			if(clearWidth > 0)
			{
				showWidth += clearWidth;
				int startY = Player4X + selected * ShowCardWidth;
				if(underShowWidth > 0) startY += underShowWidth;
				grp.SetClip(new Rectangle(startY , Player4Y + Card.Heigth - 20, clearWidth, 20));
				grp.FillRectangle(new TextureBrush(BackgroundImage),
					startY , Player4Y + Card.Heigth - 20, clearWidth, 20);
			}
			Rectangle top = new Rectangle(Player4X + selected * ShowCardWidth, Player4Y - 20, Card.Width, 20);
			Rectangle left = new Rectangle(Player4X + selected * ShowCardWidth, Player4Y - 20, showWidth, Card.Heigth);
			Region reg = new Region(left);
			reg.Union(top);
			grp.Clip = reg;
			all[selected + 36].Draw(grp);
			Debug.Unindent();
		}
				

		private void DrawPlayerName(Graphics grp)
		{
			Font namesFont = new Font("arial",10,FontStyle.Bold);
			Brush textBrush = Brushes.Black;
			StringFormat textFormat = new StringFormat();textFormat.Alignment = StringAlignment.Center;
			Brush bgBrush = new TextureBrush(BackgroundImage);
			Rectangle namesRect = new Rectangle(0,0,Card.Width,20);

			namesRect.Location = new Point(BOBX,BOBY - 20);
			grp.FillRectangle(bgBrush,namesRect);
			grp.DrawString(names[0],namesFont,textBrush,namesRect,textFormat);

			namesRect.Location = new Point(Player3X,Player3Y + 100);
			grp.FillRectangle(bgBrush,namesRect);
			grp.DrawString(names[2],namesFont,textBrush,namesRect,textFormat);
			
			namesRect.Location = new Point(Player2X - ShowCardWidth * 6,Player2Y - 20);
			grp.FillRectangle(bgBrush,namesRect);
			grp.DrawString(names[1],namesFont,textBrush,namesRect,textFormat);
			
			namesRect.Location = new Point(Player4X + ShowCardWidth * 6,Player4Y + Card.Heigth);
			grp.FillRectangle(bgBrush,namesRect);
			grp.DrawString(names[3],namesFont,textBrush,namesRect,textFormat);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawPlayerName(e.Graphics);
			for(int player = 0; player<4; player++)
			{
				int X = 0,Y = 0;
				int W = 0,H = 0;

				#region Find X,Y
				switch(player)
				{
					case 0: 
						X = BOBX; 
						Y = BOBY;
						break;
					case 1: 
						X = Player2X; 
						Y = Player2Y;
						break;
					case 2: 
						X = Player3X; 
						Y = Player3Y;
						break;
					case 3: 
						X = Player4X; 
						Y = Player4Y;
						break;
				}
				#endregion
				
				W = (player % 2 == 0) ? 0 : ShowCardWidth * 11;
				H = (player % 2 == 1) ? 0 : ShowCardWidth * 11;
				Rectangle cardsRect = new Rectangle(X, Y, X + W + Card.Width, Y + H + Card.Heigth);
				foreach(int i in remainCards[player])
					if(i != centerCards[0]
						&& i != centerCards[1]
						&& i != centerCards[2]
						&& i != centerCards[3])
						if(player == 3)
							all[i].Draw(e.Graphics);
						else
#if SHOW_FACE
								all[i].Draw(e.Graphics);
#else
							all[i].DrawFaceBack(e.Graphics);
#endif
			}

			for(int i=0;i<centerCardNum;i++)
				all[centerCards[(deckWinner + i) % 4]].Draw(e.Graphics);

			for(int i=0;i<deckNum;i++)
			{
				string t = "" 
					+ history[i * 6] + "   "
					+ all[history[i * 6 + 1]].ToString() + "  "
					+ all[history[i * 6 + 2]].ToString() + "  "
					+ all[history[i * 6 + 3]].ToString() + "  "
					+ all[history[i * 6 + 4]].ToString() + "  "
					+ history[deckNum * 6 + 5];
				e.Graphics.DrawString(t,new Font("arial",10),Brushes.Black,550,15 * i + 1);
			}
			//if game not started and user should give down cards
			if(startButtonVisible && deckWinner != -1)DrawDownCard(e.Graphics);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Debug.WriteLine("Mouse Down");
			base.OnMouseDown (e);

			if(e.Button == MouseButtons.Left)
			{
				bool userShouldPlay  = (centerCardNum + deckWinner % 4) == 3;
				Rectangle userCardsRect = new Rectangle(Player4X
					, Player4Y
					, Player4X + ShowCardWidth * 11 + Card.Width
					, Player4Y + Card.Heigth);
				Rectangle centerCardsRect = new Rectangle(CenterCardX 
					, CenterCardY
					, Card.Width * 4
					, Card.Heigth);
				Rectangle startRect = new Rectangle(StartButtonX
					,StartButtonY
					,StartButtonWidth
					,StartButtonHeigth);

				if(userCardsRect.Contains(e.X,e.Y) && deckNum != -1 && !startButtonVisible)
				{
					Debug.WriteLine("\tdeckNum"+deckNum);
					#region Play Selected
					
					if(userShouldPlay)
					{
						int ind = (e.X - Player4X) / ShowCardWidth;
						if(ind > 11) ind = 11;
						int finalInd = remainCards[3].IndexOf(ind + 36);
						if(finalInd == -1)
						{
							Debug.WriteLine("\tselected card was played <> ind = " + (ind + 36));
							finalInd = 0;
							while(finalInd < remainCards[3].Count && (int)remainCards[3][finalInd] < ind + 36)
								finalInd++;
							finalInd = finalInd - 1;
							if((int)remainCards[3][finalInd] < 366 + ind - 3) 
								finalInd = -1;
						}
						if(finalInd != -1)
						{
							if(centerCardNum != 0)
								if(all[(int)remainCards[3][finalInd]].Type != all[centerCards[deckWinner]].Type)
								{
									int[] suitNum = new int[4];
									int[] suitStarts = new int[4];
									GetRemainedCardsNum(3,ref suitNum,ref suitStarts);
									if(suitNum[(int)all[centerCards[deckWinner]].Type] > 0)
									{
										MessageBox.Show("You can't play this card");
										return;
									}
								}
							Play((int)remainCards[3][finalInd]);
							for(int j=0;j<deckWinner;j++)
								Play(PlayerPlay[centerCardNum](j));
							System.Threading.Thread.Sleep(2000);
							CollectCenter();
						}
					}
					#endregion
				}
				else if (userCardsRect.Contains(e.X,e.Y))
				{
					#region Card Selection
					Graphics grp = CreateGraphics();
					if(selected != -1)
					{
						Rectangle clearTop = new Rectangle(all[36 + selected].TopLeft,new Size(Card.Width,20));
						grp.SetClip(clearTop);
						grp.FillRectangle(new TextureBrush(BackgroundImage),clearTop);
						int showWidth =	(selected == 11) ? (int)Card.Width :ShowCardWidth;
						grp.SetClip(new Rectangle(
							Player4X + selected * ShowCardWidth - 1
							, Player4Y - 20
							, showWidth  + 1
							, Card.Heigth + 20));
						Point pt = all[36 + selected].TopLeft;
						pt.Offset(0,20);
						all[36 + selected].TopLeft = pt;
						all[36 + selected].Draw(grp);
					}
					int ind = (e.X - Player4X) / ShowCardWidth;
					Debug.WriteLine("\t ind = " + ind );
	
					if(ind > 11) ind = 11;
					selected = (selected != ind) ? ind : -1;
					Debug.WriteLine("\t selected = " + selected );
					if(selected != -1)
					{
						Point pt = all[selected + 36].TopLeft;
						pt.Offset(0,-20);
						all[selected + 36].TopLeft = pt;
						DrawSelected(grp);
					}
					#endregion
				}
				else if (centerCardsRect.Contains(e.X,e.Y)  && selected != -1)
				{
					#region Give Down Cards
					
					//swop by center card
					Graphics g = CreateGraphics();
					int ind = (e.X - CenterCardX) / Card.Width;
					if(ind > 3) ind = 3;
					Swop(selected + 36 , 48 + ind);
					all[48 + ind].Draw(g);

					//clear top
					Rectangle clearTop = new Rectangle(all[selected + 36].TopLeft,new Size(Card.Width , 20));
					g.FillRectangle(new TextureBrush(BackgroundImage),clearTop);
					Point pt = all[36 + selected].TopLeft;
					pt.Offset(0,20);
					all[36 + selected].TopLeft = pt;
					selected = -1;
					
					ArrangeCards();

					for(int i = 36 ; i < 47 ; i++)
					{
						g.SetClip(new Rectangle(all[i].TopLeft,new Size(ShowCardWidth, Card.Heigth)));
						all[i].Draw(g);
					}
					g.SetClip(new Rectangle(all[47].TopLeft,new Size(Card.Width, Card.Heigth)));
					all[47].Draw(g);

					#endregion
				}
				else if(startRect.Contains(e.X,e.Y))
				{
					#region Start Game
					startButtonVisible = false;
					for(int i=48;i<52;i++)
						collects[1].Add(i);
					deckNum = 0;
					Graphics g = CreateGraphics();
					g.FillRectangle(new TextureBrush(BackgroundImage),centerCardsRect);
					g.FillRectangle(new TextureBrush(BackgroundImage),startRect);
					#endregion
				}
			}
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);
			if(startButtonVisible)
			{
				Rectangle startRect = new Rectangle(StartButtonX
					,StartButtonY
					,StartButtonWidth
					,StartButtonHeigth);
				if(startRect.Contains(e.X,e.Y))
					DrawStartButton(CreateGraphics(),Color.WhiteSmoke);
				else
					DrawStartButton(CreateGraphics(),Color.Black);
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				foreach(TraceListener o in  Debug.Listeners)
					o.Flush();
				if(components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		
		private void DrawStartButton(Graphics g,Color c)
		{
			Font fnt = new Font(Font.FontFamily,12,FontStyle.Bold);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			Rectangle startRect = new Rectangle(StartButtonX
				,StartButtonY
				,StartButtonWidth
				,StartButtonHeigth);
			g.DrawString("Start",fnt,new SolidBrush(c)
				,startRect
				,sf);
		}

		private void muiNew_Click(object sender, System.EventArgs e)
		{
			MixCards();
			scores[0] = scores[1] = 0;
			startButtonVisible = false;
			NewPeriod();
		}

		private void Prefigure_OK(object sender, System.EventArgs e)
		{
			Debug.WriteLine("Prefigure_OK");
			deckWinner = prefigurePlayer = prefigureControl.PrefigurePlayer;
			prefigureVlaue = 100 + prefigureControl.Prefigure;
			names = prefigureControl.PlayerNames;
			DrawPlayerName(CreateGraphics());
			Debug.WriteLine("\tprefigurePlayer = " + prefigurePlayer);
			Debug.WriteLine("\tprefigureValue = " + prefigureVlaue);
			if(deckWinner != 3)
				NewDeck();
			else
			{
				DrawDownCard(CreateGraphics());
				startButtonVisible = true;
			}
		}


		private void GrandSlam_Load(object sender, System.EventArgs e)
		{
			muiNew_Click(new object(),EventArgs.Empty);
		}		

		private void DrawDownCard(Graphics g)
		{
			Debug.WriteLine("DrawDownCard");
			for(int i=48;i<52;i++)
			{
				all[i].TopLeft = new Point(CenterCardX + Card.Width * (i - 48) , CenterCardY);
				all[i].Draw(g);
			}

			DrawStartButton(g,Color.Black);
		}

		private void about_Click(object sender, System.EventArgs e)
		{
			new About().ShowDialog();
		}

        private void MenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
