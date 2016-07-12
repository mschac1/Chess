using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.IO;

namespace Chess
{
	/// <summary>
	/// Summary description for PromotePawn.
	/// </summary>
	public class frmPromotePawn : System.Windows.Forms.Form
	{
		public string PromoteTo;

		// string direc = @"c:\Menachem\c#\Chess\";
		string fileAppend = "2.bmp";
		private System.Windows.Forms.Label Knight;
		private System.Windows.Forms.Label Bishop;
		private System.Windows.Forms.Label Rook;
		private System.Windows.Forms.Label Queen;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmPromotePawn()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmPromotePawn));
			this.Knight = new System.Windows.Forms.Label();
			this.Bishop = new System.Windows.Forms.Label();
			this.Rook = new System.Windows.Forms.Label();
			this.Queen = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Knight
			// 
			this.Knight.Image = ((System.Drawing.Bitmap)(resources.GetObject("Knight.Image")));
			this.Knight.Name = "Knight";
			this.Knight.Size = new System.Drawing.Size(72, 72);
			this.Knight.TabIndex = 5;
			this.Knight.Text = "label1";
			this.Knight.Click += new System.EventHandler(this.Piece_Click);
			// 
			// Bishop
			// 
			this.Bishop.Image = ((System.Drawing.Bitmap)(resources.GetObject("Bishop.Image")));
			this.Bishop.Location = new System.Drawing.Point(72, 0);
			this.Bishop.Name = "Bishop";
			this.Bishop.Size = new System.Drawing.Size(72, 72);
			this.Bishop.TabIndex = 6;
			this.Bishop.Text = "label1";
			this.Bishop.Click += new System.EventHandler(this.Piece_Click);
			// 
			// Rook
			// 
			this.Rook.Image = ((System.Drawing.Bitmap)(resources.GetObject("Rook.Image")));
			this.Rook.Location = new System.Drawing.Point(144, 0);
			this.Rook.Name = "Rook";
			this.Rook.Size = new System.Drawing.Size(72, 72);
			this.Rook.TabIndex = 7;
			this.Rook.Click += new System.EventHandler(this.Piece_Click);
			// 
			// Queen
			// 
			this.Queen.Image = ((System.Drawing.Bitmap)(resources.GetObject("Queen.Image")));
			this.Queen.Location = new System.Drawing.Point(216, 0);
			this.Queen.Name = "Queen";
			this.Queen.Size = new System.Drawing.Size(72, 72);
			this.Queen.TabIndex = 8;
			this.Queen.Text = "label1";
			this.Queen.Click += new System.EventHandler(this.Piece_Click);
			// 
			// frmPromotePawn
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(4, 11);
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ClientSize = new System.Drawing.Size(282, 72);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.Queen,
																		  this.Rook,
																		  this.Bishop,
																		  this.Knight});
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "frmPromotePawn";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Click the piece you would like to promote to";
			this.Load += new System.EventHandler(this.frmPromotePawn_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void Piece_Click(object sender, System.EventArgs e)
		{
			PromoteTo = ((System.Windows.Forms.Label) sender).Name;
			DialogResult = DialogResult.OK;
		}

        internal static Image GetImage(String fileName) {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Chess.res." + fileName);
            return System.Drawing.Image.FromStream(myStream);
        }
		public void LoadPieces (string color)
		{
            color = color.Substring(0, 1) + color.Substring(1).ToLower();
            Knight.Image = GetImage(color + "Knight" + "Black" + fileAppend);
			Bishop.Image = GetImage(color + "Bishop" + "Black" + fileAppend);
			Rook.Image = GetImage(color + "Rook" + "Black" + fileAppend);
			Queen.Image = GetImage(color + "Queen" + "Black" + fileAppend);;
		}

		private void frmPromotePawn_Load(object sender, System.EventArgs e)
		{
			// Directory.SetCurrentDirectory(direc);
		}
	}
}
