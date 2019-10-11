using System;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;

/*====================================

 =====================================*/
namespace Mehran
{
	public enum CardValues{Ace, One = Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King};
	public enum CardType{Club, Spade, Diamond, Heart};
	
	public class Card : ICloneable
	{
		public  const byte Width  = 71;
		public  const byte Heigth = 96;
		private const byte InnerWidth    = Width - 2;
		private const byte InnerHeigth   = Heigth - 2;
		private const byte LeftRight = 12;
		private const byte TopDown   = 11;

		private Point  _topLeft = new Point(20,20);
		private CardValues _value = CardValues.Ace;
		private CardType   _type  = CardType.Club;

		public CardValues Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}
		public CardType   Type
		{
			get
			{
				return _type;
			}
			set
			{
				
				_type = value;
			}
		}
		
		public Point TopLeft
		{
			get
			{
				return _topLeft;
			}
			set
			{
				_topLeft = value;
			}
		}
		
		public Rectangle  ClientRect
		{
			get
			{
				return new Rectangle(_topLeft,new Size(Width,Heigth));
			}
		}
		
		
		public  Card()
		{
		}
		
		public  Card(CardValues value, CardType type)
		{
			_value = value;
			_type = type;
		}
		
		public  Card(CardValues value, CardType type, Point topLeft)
		{
			_value = value;
			_type = type;
			_topLeft = topLeft;
		}
		
		public  void Draw(Graphics grp)
		{
			using(Font fnt = new Font("Gungsuh",11,FontStyle.Bold))
			{
				Assembly asm= this.GetType().Assembly;
				ResourceManager recMag = new ResourceManager("Card.CardImages", asm);
				Bitmap bigTypeImage =new Bitmap((Image)recMag.GetObject("Big"+_type.ToString()));
				bigTypeImage.MakeTransparent();
				Bitmap bigTypeImageRev =new Bitmap((Image)bigTypeImage.Clone());
				bigTypeImageRev.RotateFlip(RotateFlipType.Rotate180FlipNone);
				Image smallTypeImage = (Image)recMag.GetObject("Small"+_type.ToString());
				Image numImage = (Image)recMag.GetObject(IsBlack() ? "BlackNum" : "RedNum");
				Rectangle numRect = new Rectangle(_topLeft.X + 1, _topLeft.Y + 1, 11, 14);
				Rectangle typeRect = new Rectangle(_topLeft.X + 1, _topLeft.Y + 15, 11, 11);
				
				grp.FillRectangle(Brushes.White, _topLeft.X + 1, _topLeft.Y + 1, InnerWidth, InnerHeigth);
			
				grp.DrawLine(Pens.Black, _topLeft.X + 1, _topLeft.Y, _topLeft.X + Width - 2, _topLeft.Y);
				grp.DrawLine(Pens.Black, _topLeft.X + 1, _topLeft.Y + Heigth - 1, _topLeft.X + Width - 2, _topLeft.Y + Heigth - 1);
				grp.DrawLine(Pens.Black, _topLeft.X , _topLeft.Y + 1, _topLeft.X, _topLeft.Y + Heigth - 2);
				grp.DrawLine(Pens.Black, _topLeft.X + Width - 1, _topLeft.Y + 1, _topLeft.X + Width - 1, _topLeft.Y + Heigth - 2);
				if(_value == CardValues.Ace && _type == CardType.Spade)
				{
					Image face = (Image)recMag.GetObject("AceOfSpade");
					grp.DrawImage(face, _topLeft.X + 15, _topLeft.Y + 24, 40, 40);
				}
				else if(IsFace())
				{
					Image face = (Image)recMag.GetObject(_value.ToString() + "Of" + _type.ToString());
					grp.DrawImage(face, _topLeft.X + LeftRight, _topLeft.Y + TopDown, Width - 2 * LeftRight, Heigth  - 2 * TopDown);
				}
				else
				{
					#region Draw Main
					const int size = 15;
					const int top = 10;
					const int right = 16;
					Rectangle dest = new Rectangle(0,0,size,size);
					switch(_value)
					{
						case CardValues.One :
							dest.X = _topLeft.X + (Width - size) / 2;
							dest.Y = _topLeft.Y + (Heigth - size) / 2;
							grp.DrawImage(bigTypeImage,dest);
							break;
						case CardValues.Two :
							dest.X = _topLeft.X + (Width - size) / 2;
							dest.Y = _topLeft.Y + top;
							grp.DrawImage(bigTypeImage,dest);
							dest.Y = _topLeft.Y + Heigth - size - top;
							grp.DrawImage(bigTypeImageRev,dest);
							break;
						case CardValues.Three :
							dest.X = _topLeft.X + (Width - size) / 2;
							dest.Y = _topLeft.Y + (Heigth - size) / 2;
							grp.DrawImage(bigTypeImage,dest);
							goto case CardValues.Two;
						case CardValues.Four :
							dest.X = _topLeft.X + right;
							dest.Y = _topLeft.Y + top;
							grp.DrawImage(bigTypeImage,dest);
							dest.X = _topLeft.X + Width - right - size;
							grp.DrawImage(bigTypeImage,dest);
							dest.Y = _topLeft.Y + Heigth - top - size;
							grp.DrawImage(bigTypeImageRev,dest);
							dest.X = _topLeft.X + right;
							grp.DrawImage(bigTypeImageRev,dest);
							break;
						case CardValues.Five :
							dest.X = _topLeft.X + (Width - size) / 2;
							dest.Y = _topLeft.Y + (Heigth - size) / 2;
							grp.DrawImage(bigTypeImage,dest);
							goto case CardValues.Four;
						case CardValues.Six :
							dest.X = _topLeft.X + right;
							dest.Y = _topLeft.Y + (Heigth - size) / 2;
							grp.DrawImage(bigTypeImage,dest);
							dest.X = _topLeft.X + Width - right - size;
							dest.Y = _topLeft.Y + (Heigth - size) / 2;
							grp.DrawImage(bigTypeImage,dest);
							goto case CardValues.Four;
						case CardValues.Seven :
							dest.X = _topLeft.X + (Width - size) / 2;
							dest.Y = _topLeft.Y + top + 20;
							grp.DrawImage(bigTypeImage,dest);
							goto case CardValues.Six;
						case CardValues.Eight :
							dest.X = _topLeft.X + right;
							dest.Y = _topLeft.Y + top + 20;
							grp.DrawImage(bigTypeImage,dest);
							dest.X = _topLeft.X + Width - right - size;
							grp.DrawImage(bigTypeImage,dest);
							dest.Y = _topLeft.Y + Heigth - top - 20 - size;
							grp.DrawImage(bigTypeImageRev,dest);
							dest.X = _topLeft.X + right;
							grp.DrawImage(bigTypeImageRev,dest);
							goto case CardValues.Four;
						case CardValues.Nine :
							dest.X = _topLeft.X + (Width - size) / 2;
							dest.Y = _topLeft.Y + (Heigth - size) / 2;
							grp.DrawImage(bigTypeImage,dest);
							goto case CardValues.Eight;
						case CardValues.Ten :
							dest.X = _topLeft.X + (Width - size) / 2;
							dest.Y = _topLeft.Y + top + 10;
							grp.DrawImage(bigTypeImage,dest);
							dest.Y = _topLeft.Y + Heigth - top - 10 - size;
							grp.DrawImage(bigTypeImageRev,dest);
							goto case CardValues.Eight;
					}
					#endregion
				}
				
				grp.DrawImage(numImage, numRect, new Rectangle(11 * (int)_value,0, 11, 14),GraphicsUnit.Pixel);
				grp.DrawImage(smallTypeImage, typeRect);
 
				smallTypeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
				numImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

				numRect.Offset(Width - 2 * 1 - 11, Heigth - 2 * 1 - 14);
				typeRect.Offset(Width - 2 * 1 - 11, Heigth - 15 * 2 - 11);
				grp.DrawImage(numImage, numRect, new Rectangle(11 * ((int)CardValues.King - (int)_value),0, 11, 14),GraphicsUnit.Pixel);
				grp.DrawImage(smallTypeImage, typeRect);
			}
		}

		public  void DrawFaceBack(Graphics grp)
		{
			Assembly asm= this.GetType().Assembly;
			ResourceManager recMag = new ResourceManager("Card.CardImages", asm);
			Image img = (Image)recMag.GetObject("FaceDown");
			grp.DrawImage(img,new Rectangle(_topLeft.X + 1, _topLeft.Y + 1, InnerWidth, InnerHeigth));
			
			grp.DrawLine(Pens.Black, _topLeft.X + 1, _topLeft.Y, _topLeft.X + Width - 2, _topLeft.Y);
			grp.DrawLine(Pens.Black, _topLeft.X + 1, _topLeft.Y + Heigth - 1, _topLeft.X + Width - 2, _topLeft.Y + Heigth - 1);
			grp.DrawLine(Pens.Black, _topLeft.X , _topLeft.Y + 1, _topLeft.X, _topLeft.Y + Heigth - 2);
			grp.DrawLine(Pens.Black, _topLeft.X + Width - 1, _topLeft.Y + 1, _topLeft.X + Width - 1, _topLeft.Y + Heigth - 2);
		}
		public bool IsFace()
		{
			return _value == CardValues.Jack || _value == CardValues.Queen || _value == CardValues.King;
		}
		public bool IsBlack()
		{
			return _type == CardType.Club || _type == CardType.Spade;
		}


		public static bool operator ==(Card x,Card y)
		{
			return (x._value == y._value) && (x._type == y._type);
		}

		public static bool operator !=(Card x,Card y)
		{
			return (x._value != y._value) || (x._type != y._type);
		}

		public static bool operator < (Card x,Card y)
		{
			if(x._type != y._type)
			throw new ArgumentException("Two card should be same type for compare");
			return x._value < y._value;
		}

		public static bool operator > (Card x,Card y)
		{
			if(x._type != y._type)
			throw new ArgumentException("Two card should be same type for compare");
			return x._value > y._value;
		}

		public static bool operator <= (Card x,Card y)
		{
			if(x._type != y._type)
				throw new ArgumentException("Two card should be same type for compare");
			return x._value <= y._value;
		}

		public static bool operator >= (Card x,Card y)
		{
			if(x._type != y._type)
				throw new ArgumentException("Two card should be same type for compare");
			return x._value >= y._value;
		}


		public override string ToString()
		{
			return _value.ToString() + " of " + _type.ToString();
		}


		public object Clone()
		{
			return new Card(_value,_type,_topLeft);
		}
	}
}
