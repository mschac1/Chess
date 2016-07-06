using System;
using System.Collections;

namespace Chess
{

	// The delegate type used to find out what piece to promote pawns to
	public delegate ChessPiece PawnCallback();

	public class Game
	{
		#region Variables

		private  ChessPiece [ , ] board = new ChessPiece[8, 8];

		// Indexers used to get chess pieces
		public  ChessPiece this [int i, int j]
		{
			get
			{
				return board[i, j];
			}
		}
	
		private ChessPiece this [Position pos]
		{
			get
			{
				return board[pos.X, pos.Y];
			}
			set
			{
				board[pos.X, pos.Y] = value;
			}
		}


		// The board used for checking the validity of moves and posible checks
		private Game checkBoard;

		private Position moveFrom;
		private Position moveTo;

		private string turn;
		public string Turn
		{
			get
			{
				return turn;
			}
		}


		// Castle status flags
		private bool WhiteLeftCastle;
		private bool WhiteRightCastle;
		private bool BlackLeftCastle;
		private bool BlackRightCastle;

		private int enPassant;

		// The delegate used to find out what piece to promote pawns to
		public PawnCallback pawnPromoter;
		#endregion

		public Game()
		{
			newGame();
		}

		
		// Copy Constructor
		public Game(Game game)
		{
			board = new ChessPiece[8, 8];
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					board[i, j] = game.board[i, j];

			turn = game.turn;
			
			WhiteLeftCastle = game.WhiteLeftCastle;
			WhiteRightCastle = game.WhiteLeftCastle;
			BlackLeftCastle = game.WhiteLeftCastle;
			BlackRightCastle = game.WhiteLeftCastle;

			enPassant = game.enPassant;
		}
		
		
		public StatusFlag Move(Position from, Position to)
		{
			moveFrom = from;
			moveTo = to;

			// Check whether there is a piece in the "from" position
			if (this[from] == null)
				return ChessStatus.INVALID_PIECE;

			// Check whether the piece being moved is of the same color as the turn
			if (IsTurn() == false)
				return ChessStatus.OPPONENTS_PIECE;

			// If the piece is being moved onto a square occupied by another piece of the same color
			if (this[moveTo] != null && this[moveTo].Color == turn)
				return ChessStatus.INVALID_BLOCK;

			// Check whether the move is valid based on the manner in which the piece moves
			StatusFlag status = IsValidMove();

			// Return any invalid flags
			if (status != ChessStatus.VALID)
				return status;

			// Check whether the move would place the player in check
			if (IsCheck())
				return ChessStatus.INVALID_CHECK;

			// Make the move
			this[to] = this[from];
			this[from] = null;

			// If the player moved his pawn to the opposite side, promote it
			if (this[to].Name == "Pawn")
			{
				if ((turn == "White" && to.Y == 0) || (turn == "Black" && to.Y == 7))
					board[to.X, to.Y] = pawnPromoter();
			}

			ToggleTurn();

			// Update casrling flags
			SetCastleFlags();

			return ChessStatus.VALID;
		}

		
		public void newGame()
		{
			turn = "White";

			// Reset castle flags
			WhiteLeftCastle = true;
			WhiteRightCastle = true;
			BlackLeftCastle = true;
			BlackRightCastle = true;

			enPassant = -1;
			// Reset the board
			board[0, 0] = new Rook("Black");
			board[1, 0] = new Knight("Black");
			board[2, 0] = new Bishop("Black");
			board[3, 0] = new Queen("Black");
			board[4, 0] = new King("Black");
			board[5, 0] = new Bishop("Black");
			board[6, 0] = new Knight("Black");
			board[7, 0] = new Rook("Black");

			for (int i = 0; i < 8; i++)
				board[i, 1] = new Pawn("Black");

			for (int i = 2; i < 6; i++)
				for (int j = 0; j < 8; j++)
						board[j, i] = null;
	
			for (int i = 0; i < 8; i++)
				board[i, 6] = new Pawn("White");

			board[0, 7] = new Rook("White");
			board[1, 7] = new Knight("White");
			board[2, 7] = new Bishop("White");
			board[3, 7] = new Queen("White");
			board[4, 7] = new King("White");
			board[5, 7] = new Bishop("White");
			board[6, 7] = new Knight("White");
			board[7, 7] = new Rook("White");
		}
		
		
		private void ToggleTurn()
		{
			if (turn == "White")
				turn = "Black";
			else
				turn = "White";

		}
		
		
		private bool IsTurn()
		{
			if (turn == this[moveFrom].Color)
				return true;
			return false;
		}
		
		
		private StatusFlag IsValidMove ()
		{
			// The piece being moved
			ChessPiece piece = this[moveFrom];

			// Call the pieces individual move validation and return the resulting status
			if (piece.Name == "Pawn")
				return IsValidMove((Pawn) piece);
			else if (piece.Name == "Rook")
				return IsValidMove((Rook) piece);
			else if (piece.Name == "Knight")
				return IsValidMove((Knight) piece);
			else if (piece.Name == "Bishop")
				return IsValidMove((Bishop) piece);
			else if (piece.Name == "Queen")
				return IsValidMove((Queen) piece);
			else 
				return IsValidMove((King) piece);
		}
		
		
		private StatusFlag IsValidMove(Pawn pawn)
		{
			if (turn.Equals("White"))
			{
				if (moveFrom.Y <= moveTo.Y || moveFrom.Y > moveTo.Y + 2)
					return ChessStatus.INVALID_MOVE;
				if (moveFrom.Y == moveTo.Y + 2)
				{
					if (moveFrom.Y != 6 || moveFrom.X != moveTo.X)
						return ChessStatus.INVALID_MOVE;
					if (board[moveFrom.X, moveFrom.Y - 1] != null || board[moveTo.X, moveTo.Y] != null)
						return ChessStatus.INVALID_BLOCK;
				}
				else
				{
					if (Math.Abs(moveFrom.X - moveTo.X) > 1)
						return ChessStatus.INVALID_MOVE;

					if (moveFrom.X == moveTo.X && board[moveTo.X, moveTo.Y] != null)
						return ChessStatus.INVALID_BLOCK;
					if (Math.Abs(moveFrom.X - moveTo.X) == 1 && board[moveTo.X, moveTo.Y] == null)
						return ChessStatus.INVALID_MOVE;
				}
			}
			else
			{
				if (moveFrom.Y >= moveTo.Y || moveFrom.Y < moveTo.Y - 2)
					return ChessStatus.INVALID_MOVE;
				if (moveFrom.Y == moveTo.Y - 2)
				{
					if (moveFrom.Y != 1 || moveFrom.X != moveTo.X)
						return ChessStatus.INVALID_MOVE;
					if (board[moveFrom.X, moveFrom.Y + 1] != null || board[moveTo.X, moveTo.Y] != null)
						return ChessStatus.INVALID_BLOCK;
				}
				else
				{
					if (Math.Abs(moveFrom.X - moveTo.X) > 1)
						return ChessStatus.INVALID_MOVE;

					if (moveFrom.X == moveTo.X && board[moveTo.X, moveTo.Y] != null)
						return ChessStatus.INVALID_BLOCK;
					if (Math.Abs(moveFrom.X - moveTo.X) == 1 && board[moveTo.X, moveTo.Y] == null)
						return ChessStatus.INVALID_MOVE;
				}
			}

			return ChessStatus.VALID;
		}
		private StatusFlag IsValidMove (Rook rook)
		{
			// Validate move
			int dX = Math.Abs(moveTo.X - moveFrom.X);
			int dY = Math.Abs(moveTo.Y - moveFrom.Y);
			if (dY * dX != 0)
				return ChessStatus.INVALID_MOVE;

			// Check if pieces are in the way
			if (dX != 0)
				dX = dX /(moveTo.X - moveFrom.X);
			if (dY != 0)
				dY = dY /(moveTo.Y - moveFrom.Y);

			// Vertical Move
			if (dX == 0)
			{
				int j = moveFrom.Y + dY;
				while(j != moveTo.Y)
				{
					if (board[moveFrom.X, j] != null)
						return ChessStatus.INVALID_BLOCK;

					j += dY;
				}
			}

				// Horizontal Move
			else
			{
				int i = moveFrom.X + dX;
				while(i != moveTo.X)
				{
					if (board[i, moveFrom.Y] != null)
						return ChessStatus.INVALID_BLOCK;

					i += dX;
				}
			}

			return ChessStatus.VALID;
		}
		private StatusFlag IsValidMove(Queen queen)
		{
			int dX = Math.Abs(moveTo.X - moveFrom.X);
			int dY = Math.Abs(moveTo.Y - moveFrom.Y);

			if (dY * dX != 0 && dY != dX)
				return ChessStatus.INVALID_MOVE;

			// Check if pieces are in the way
			if (dX != 0)
				dX = dX /(moveTo.X - moveFrom.X);
			if (dY != 0)
				dY = dY /(moveTo.Y - moveFrom.Y);
			// Vertical Move
			if (dX == 0)
			{
				int j = moveFrom.Y + dY;
				while(j != moveTo.Y)
				{
					if (board[moveFrom.X, j] != null)
						return ChessStatus.INVALID_BLOCK;

					j += dY;
				}
			}

			// Horizontal Move
			else if(dY == 0)
			{
				int i = moveFrom.X + dX;
				while(i != moveTo.X)
				{
					if (board[i, moveFrom.Y] != null)
						return ChessStatus.INVALID_BLOCK;

					i += dX;
				}
			}

			// Diagonal Move
			else
			{
				int i = moveFrom.X + dX;
				int j = moveFrom.Y + dY;
				while(i != moveTo.X)
				{
					if (board[i, j] != null)
						return ChessStatus.INVALID_BLOCK;

					i += dX; j += dY; 
				}
			}

			return ChessStatus.VALID;
		}
		private StatusFlag IsValidMove(King king)
		{
			int dX = Math.Abs(moveTo.X - moveFrom.X);
			int dY = Math.Abs(moveTo.Y - moveFrom.Y);

			if (dY > 1 || dX > 2)
				return ChessStatus.INVALID_MOVE;

			if (dX == 2)
			{
				StatusFlag status = Castle();
				if (status != ChessStatus.VALID)
					return status;
			}
	
			return ChessStatus.VALID;
		}
		private StatusFlag IsValidMove(Knight knight)
		{
			int dX = Math.Abs(moveTo.X - moveFrom.X);
			int dY = Math.Abs(moveTo.Y - moveFrom.Y);

			if (dY * dX != 2)
				return ChessStatus.INVALID_MOVE;

			return ChessStatus.VALID;
		}
		private StatusFlag IsValidMove(Bishop bishop)
		{
			int dX = Math.Abs(moveTo.X - moveFrom.X);
			int dY = Math.Abs(moveTo.Y - moveFrom.Y);

			if (dY != dX)
				return ChessStatus.INVALID_MOVE;

			dX = dX / (moveTo.X - moveFrom.X);
			dY = dY / (moveTo.Y - moveFrom.Y);

			// Check if pieces are in the way
			int i = moveFrom.X + dX;
			int j = moveFrom.Y + dY;
			while(i != moveTo.X)
			{
				if (board[i, j] != null)
					return ChessStatus.INVALID_BLOCK;

				i += dX; j += dY; 
			}

			return ChessStatus.VALID;
		}
		
		
		private void ForceMove(Position from, Position to)
		{
			board[to.X, to.Y] = board[from.X, from.Y];
			if (!from.Equals(to))
				board[from.X, from.Y] = null;
		}
		
		
		private bool IsCheck()
		{
			ArrayList FromSet = new ArrayList(16);

			checkBoard = new Game(this);
			checkBoard.ToggleTurn();
			checkBoard.ForceMove(moveFrom, moveTo);

			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
				{
					if (checkBoard[i, j] != null && checkBoard[i, j].Color != turn)
						FromSet.Add(new Position(i, j)); 
					else if (checkBoard[i, j] != null && checkBoard[i, j].Name == "King")
						checkBoard.moveTo = new Position(i, j);
				}

			foreach (Position pos in FromSet)
			{
				checkBoard.moveFrom = pos;
				if (checkBoard.IsValidMove() == ChessStatus.VALID)
					return true;
			}

			return false;
		}
		
		
		private void SetCastleFlags()
		{
			if (moveFrom.Equals(0, 0))
				BlackLeftCastle = false;
			if (moveFrom.Equals(4, 0))
			{
				BlackLeftCastle = false;
				BlackRightCastle = false;
			}
			if (moveFrom.Equals(7, 0))
				BlackRightCastle = false;

			if (moveFrom.Equals(0, 7))
				WhiteLeftCastle = false;
			if (moveFrom.Equals(4, 7))
			{
				WhiteLeftCastle = false;
				WhiteRightCastle = false;
			}
			if (moveFrom.Equals(7, 7))
				WhiteRightCastle = false;

		}


		private StatusFlag Castle()
		{
			int inc;
			int Y;
			int rook;

			if (turn.Equals("White"))
			{
				Y = 7;

				if (moveFrom.X < moveTo.X) // Castle right
				{
					if (WhiteRightCastle == false)
						return ChessStatus.INVALID_CASTLE;
					inc = 1;
					rook = 7;
				}
				else // Castle left
				{
					if (WhiteLeftCastle == false)
						return ChessStatus.INVALID_CASTLE;
					inc = -1;
					rook = 0;
				}

			}
			else // if (turn.Equals("Black"))
			{
				Y = 0;

				if (moveFrom.X < moveTo.X) // Castle right
				{
					if (BlackRightCastle == false)
						return ChessStatus.INVALID_CASTLE;
					inc = 1;
					rook = 7;
				}
				else // Castle left
				{
					if (BlackLeftCastle == false)
						return ChessStatus.INVALID_CASTLE;
					inc = -1;
					rook = 0;
				}
			}

			if (moveFrom.Y != Y)
				return ChessStatus.INVALID_MOVE;

			if (board[4 + inc, Y] != null)
				return ChessStatus.INVALID_BLOCK;
			
			checkBoard = new Game(this);
			checkBoard.moveFrom = new Position(rook, Y);
			checkBoard.moveTo = new Position(moveFrom.X + inc, Y);
			if (checkBoard.IsValidMove() == ChessStatus.INVALID_BLOCK)
				return ChessStatus.INVALID_BLOCK;

			Position tempMoveTo = new Position(moveTo);

			int i;
			for (i = moveFrom.X; i!= tempMoveTo.X; i += inc)
			{
				moveTo = new Position(i, Y);
				if (IsCheck())
					return ChessStatus.INVALID_CASTLE;
			}

			// last iteration
			moveTo = new Position(i, Y);
			if (IsCheck())
				return ChessStatus.INVALID_CASTLE;

			moveTo = new Position(tempMoveTo); 
			ForceMove(new Position(rook, Y), new Position(moveTo.X - inc, Y));
			return ChessStatus.VALID;
		}


	}

	
	public abstract class ChessPiece
	{
		protected string name;
		protected string color;

		public string Name
		{
			get
			{
				return name;
			}
		}
		public string Color
		{
			get
			{
				return color;
			}
		}
	}

	
	class Pawn : ChessPiece
	{
		public Pawn(string color)
		{
			name = "Pawn";
			this.color = color;
		}
	}


	class Knight : ChessPiece
	{
		public Knight(string color)
		{
			name = "Knight";
			this.color = color;
		}
	}

	
	class Rook : ChessPiece
	{
		public Rook(string color)
		{
			name = "Rook";
			this.color = color;
		}
	}

	
	class Bishop : ChessPiece
	{
		public Bishop(string color)
		{
			name = "Bishop";
			this.color = color;
		}
	}

	
	class Queen : ChessPiece
	{
		public Queen(string color)
		{
			name = "Queen";
			this.color = color;
		}
	}
	
	
	class King : ChessPiece
	{
		public King(string color)
		{
			name = "King";
			this.color = color;
		}
	}

	
	public class Position
	{
		internal int X;
		internal int Y;

		public Position()
		{}

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}
		public Position(Position P)
		{
			X = P.X;
			Y = P.Y;
		}
		
		public bool Equals(Position P)
		{
			if (X == P.X && Y == P.Y)
				return true;
			else
				return false;
		}
		public bool Equals(int x, int y)
		{
			return Equals(new Position(x, y));
		}

	}

	
	public class StatusFlag
	{
		public readonly string Text;

		internal StatusFlag (string text)
		{
			Text = text;
		}
	}

	
	public class ChessStatus
	{
		public static StatusFlag VALID = new StatusFlag("");

		public static StatusFlag INVALID_PIECE = new StatusFlag("You are attempting to move an invalid piece");

		public static StatusFlag OPPONENTS_PIECE = new StatusFlag("You are attempting to move a piece of the wrong color");

		public static StatusFlag INVALID_MOVE = new StatusFlag("The piece you are attempting to move does not move in that manner");

		public static StatusFlag INVALID_BLOCK = new StatusFlag("There is a piece blocking that move");

		public static StatusFlag INVALID_CHECK = new StatusFlag("That move would leave your king in check");

		public static StatusFlag INVALID_CASTLE = new StatusFlag("You can not castle now");
	}



}
