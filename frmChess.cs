using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using static Chess.Rank;
using System.IO;

namespace Chess {
    public class frmChess : System.Windows.Forms.Form
	{
		#region Variables

		private boardLabel [ , ] board = new boardLabel[8, 8];

		ChessGame game;

		Point moveFrom = new Point();
		Point moveTo = new Point();
		bool moveFlag;

		// string direc = @"c:\Menachem\c#\Chess\";
		string fileAppend = "2.bmp";

		private System.Windows.Forms.Label lblTemplate;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.Label statusBar;
		private System.Windows.Forms.Label lblTurn;
		private System.Windows.Forms.Label lblStatus;

        #endregion

        private MenuItem menuItem3;
        private IContainer components;

        public frmChess()
		{
			InitializeComponent();

			InitializeMyComponents();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChess));
            this.lblTemplate = new System.Windows.Forms.Label();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.statusBar = new System.Windows.Forms.Label();
            this.lblTurn = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTemplate
            // 
            this.lblTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTemplate.Image = ((System.Drawing.Image)(resources.GetObject("lblTemplate.Image")));
            this.lblTemplate.Location = new System.Drawing.Point(8, 32);
            this.lblTemplate.Name = "lblTemplate";
            this.lblTemplate.Size = new System.Drawing.Size(72, 72);
            this.lblTemplate.TabIndex = 0;
            this.lblTemplate.Visible = false;
            this.lblTemplate.Click += new System.EventHandler(this.lblTemplate_Click);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem3});
            this.menuItem1.Text = "&Game";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.menuItem2.Text = "&New";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.menuItem3.Text = "&Undo";
            this.menuItem3.Click += new System.EventHandler(this.menuUndo_Click);
            // 
            // statusBar
            // 
            this.statusBar.BackColor = System.Drawing.Color.White;
            this.statusBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statusBar.Location = new System.Drawing.Point(0, 184);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(328, 40);
            this.statusBar.TabIndex = 1;
            // 
            // lblTurn
            // 
            this.lblTurn.BackColor = System.Drawing.Color.White;
            this.lblTurn.Location = new System.Drawing.Point(16, 192);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(64, 24);
            this.lblTurn.TabIndex = 2;
            this.lblTurn.Text = "Turn: White";
            this.lblTurn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(88, 192);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(216, 24);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmChess
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(602, 267);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTurn);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.lblTemplate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "frmChess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmChess_Load);
            this.ResumeLayout(false);

		}
		#endregion


		public void InitializeMyComponents()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					board[i, j] = new boardLabel();

					board[i, j].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
					board[i, j].Location = new System.Drawing.Point(i * lblTemplate.Width, j * lblTemplate.Height);
					board[i, j].Size = lblTemplate.Size;
					board[i, j].TabStop = false;
					board[i, j].Visible = true;
					board[i, j].X = i;
					board[i, j].Y = j;
					board[i, j].Click += new System.EventHandler(this.lblTemplate_Click);


					if ((i + j) % 2 == 0)
					{
						board[i, j].BackColor = System.Drawing.Color.White;
						board[i, j].color = "White";
					}
					else
					{
						board[i, j].BackColor = System.Drawing.Color.Black;
						board[i, j].color = "Black";
					}

					Controls.AddRange(new System.Windows.Forms.Control[] {board[i, j]});

				}
				
				statusBar.Top = lblTemplate.Height *  8;// + 34;
				Height = statusBar.Top + statusBar.Height + 32;//lblTemplate.Height *  8 + 34;
				Width = lblTemplate.Width * 8 + 6;
				statusBar.Width = lblTemplate.Width * 8;

				lblTurn.Top = statusBar.Top + 8;
				lblStatus.Top = statusBar.Top + 8;
				lblStatus.Left = (Width - lblStatus.Width) / 2;
			}
		}

		[STAThread]
		static void Main() 
		{
			Application.Run(new frmChess());
		}

		private void frmChess_Load(object sender, System.EventArgs e)
		{
			game = new ChessGame();			
			newGame();
		}
		private void newGame()
		{
			game.newGame();
			RefreshBoard();
			moveFlag = false;
            game.pawnPromoter = new PawnCallback(PromotePawn);
        }

        public static String EnumTitleCase (Enum e) {
            String s = e.ToString();
            return s.Substring(0, 1) + s.Substring(1).ToLower();
        }
		private void RefreshBoard()
		{

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = null;
            String fileName = null;
            for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
                    if (game[i, j] != null) {
                        fileName = EnumTitleCase(game[i, j].Color) + EnumTitleCase(game[i, j].Rank) + board[i, j].color + fileAppend;
                        myStream = myAssembly.GetManifestResourceStream("Chess.res." + fileName);
                        board[i, j].Image = System.Drawing.Image.FromStream(myStream);
                    }
                    else
                        board[i, j].Image = null;

				}
			}
			lblTurn.Text = "Turn: " + game.Turn;
		}

		private void lblTemplate_Click(object sender, System.EventArgs e)
		{
			boardLabel lbl = (boardLabel) sender;

			if (moveFlag == false && lbl.Image != null)
			{
				moveFrom = new Point(lbl.X, lbl.Y);
				moveFlag = true;
				lblStatus.Text = "Moving: " + game[lbl.X, lbl.Y].Rank;
                lbl.BorderColor = System.Drawing.Color.Red;
                lbl.BorderWidth = 3;

            }
			else
			{
				moveTo = new Point(lbl.X, lbl.Y);
				moveFlag = false;
				MovePiece(moveFrom, moveTo);
                board[moveFrom.X, moveFrom.Y].BorderWidth = 1;
                board[moveFrom.X, moveFrom.Y].BorderColor = System.Drawing.Color.Black;

            }
		}
		private void MovePiece(Point from, Point to)
		{
			if (game.MovePiece(from, to))
			{
				RefreshBoard();
			}
			
		    lblStatus.Text = game.Status;

		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
            newGame();
		}
		private Rank PromotePawn()
		{
			frmPromotePawn PawnDialog = new frmPromotePawn();
			PawnDialog.LoadPieces(game.Turn.ToString());

			PawnDialog.ShowDialog();
			if (PawnDialog.PromoteTo.Equals("Knight"))
				return KNIGHT;
			else if (PawnDialog.PromoteTo.Equals("Bishop"))
				return BISHOP;
			else if (PawnDialog.PromoteTo.Equals("Rook"))
				return ROOK;
			else
				return QUEEN;
		}

        private void menuUndo_Click(object sender, EventArgs e) {
            game.UndoMove();
            RefreshBoard();
            lblStatus.Text = game.Status;
        }
    }

    public class boardLabel : System.Windows.Forms.Label
	{
		public int X;
		public int Y;
		public string color;

        // The code below came from Mohammad Dehghan @ http://stackoverflow.com/questions/14691758/setting-a-size-for-a-label-border
        private int _borderWidth = 1;
        public int BorderWidth {
            get { return _borderWidth; }
            set {
                _borderWidth = value;
                Invalidate();
            }
        }

        private System.Drawing.Color _borderColor = System.Drawing.Color.Black;
        public System.Drawing.Color BorderColor {
            get { return _borderColor; }
            set {
                _borderColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            int xy = 0;
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;
            Pen pen = new Pen(_borderColor);
            for (int i = 0; i < _borderWidth; i++)
                e.Graphics.DrawRectangle(pen, xy + i, xy + i, width - (i << 1) - 1, height - (i << 1) - 1);
        }
    }

			
}
