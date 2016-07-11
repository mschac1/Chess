using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using static Chess.Color;
using static Chess.Rank;

namespace Chess
{

	// The delegate type used to find out what piece to promote pawns to
	public delegate ChessPiece PawnCallback();

	public class ChessGame {
		#region Variables

		private  ChessPiece [ , ] board = new ChessPiece[8, 8];

		// Indexers used to get chess pieces
		public  ChessPiece this [int i, int j] {
			get {return board[i, j];}
		}
	
		private ChessPiece this [Point pos] {
			get {return board[pos.X, pos.Y];}
			set {board[pos.X, pos.Y] = value;}
		}



		// Castle status flags
		internal bool whiteLeftCastle;
        internal bool whiteRightCastle;
        internal bool blackLeftCastle;
        internal bool blackRightCastle;

	
        Stack<ChessMove> moveStack = new Stack<ChessMove>();
		// The delegate used to find out what piece to promote pawns to
		public PawnCallback pawnPromoter;

        public string Status { get; private set; }
        public Color Turn { get; private set; }
        public bool GameOver { get; private set; }
        #endregion

        public ChessGame() {
			newGame();
		}

        public bool MovePiece(Point from, Point to) {

            // Check if the game is over
            if (GameOver)
                return false;

            // Check whether the coordinates are invalid
            if (IsInvalidSquare(from) || IsInvalidSquare(to)) {
                Status = "Invalid coordinate";
                return false;
            }

            ChessPiece piece = this[from];

            // Check whether there is a piece in the "from" position
            if (piece == null) {
                Status = "There is no piece at that location";
                return false;
            }
/*TODO UNCOMMENT THIS
            // Check whether the piece being moved is of the same color as the turn
            if (piece.Color != Turn) {
                Status = "You are attemptint to move your opponent's piece";
                return false;
            }
*/
            // If the piece is being moved onto a square occupied by another piece of the same color
            if (this[to] != null && this[to].Color == this[from].Color) {
                Status = "Invalid Move";
                return false;
            }

            ChessMove move = AttemptMove(from, to);

            if (move == null) {
                Status = "Invalid Move";
                return false;
            }

            makeMove(move);

            // Check whether the move would place the player in check
            if (IsCheck()) {
                Status = "That movewould put your king in check";
                ReverseMove();
                return false;
            }

            // TODO check for checkmate, and stalemate

            // TODO Uncomment this ToggleTurn();

            //TODO check for check

            return true;
		}

        private void ReverseMove() {
            throw new NotImplementedException();
        }

        private void makeMove(ChessMove move) {
            this[move.To] = this[move.From];
            this[move.From] = null;

            // TODO en Passant and castle
            moveStack.Push(move);
        }

        private bool IsCheck() {
            return false;
            //TODO implement this
        }

        private ChessMove AttemptMove(Point from, Point to) {
            ChessPiece piece = this[from];
            ChessMove move = new ChessMove(from, to, this[to]);
            int dX = Math.Abs(from.X - to.X);
            int dY = Math.Abs(from.Y - to.Y);

            switch (piece.Rank) {
                case KNIGHT:
                    // The knight can move if it is moving 2 in one direction and 1 in the other
                    if (dX * dY == 2)
                        return move;
                    return null;

                case ROOK:
                    // If dX is 0, the move is vertical, if dY is 0, it's horizontal
                    if ((dX == 0 ||  dY == 0) && !IsPieceBetween(from, to))
                        return move;
                    return null;

                case BISHOP:
                    // If dX = dY the move is diagonal
                    if ((dX == dY) && !IsPieceBetween(from, to)) 
                        return move;                    
                    return null;

                case QUEEN:
                    if ((dX == dY || dX == 0 || dY == 0) && !IsPieceBetween(from, to))
                        return move;
                    return null;

                case KING:
                    // Cannot move more than 1 in any direction
                    if (dX <= 1 && dY <= 1)
                        return move;
                    //Check for possible castle
                    if (dX == 2 && dY == 0 && IsCastleAllowed(from, to))
                        return move;
                    return null;

                case PAWN:
                    return AttemptMovePawn(move);
            }

            return move;
        }

        private ChessMove AttemptMovePawn(ChessMove move) {
            ChessPiece piece = this[move.From];
            int dX = Math.Abs(move.From.X - move.To.X);
            int dY = move.To.Y - move.From.Y;

            // Move 1 square forward
            int allowedDirection = (piece.Color == BLACK ? 1 : -1);
            if (dX == 0 && dY == allowedDirection && move.opponent == null)
                return move;

            return null;
        }

        private bool IsCastleAllowed(Point from, Point to) {
            throw new NotImplementedException();
        }

        private bool IsPieceBetween(Point from, Point to) {
            int dX = Math.Abs(from.X - to.X);
            int dY = Math.Abs(from.Y - to.Y);
            int stepX = (dX == 0? 0 : dX / (to.X - from.X));
            int stepY = (dY == 0? 0 : dY / (to.Y - from.Y));
            int currX = from.X + stepX;
            int currY = from.Y + stepY;

            while (currX != to.X || currY != to.Y) {
                if (this[currX, currY] != null)
                    return true;
                currX += stepX;
                currY += stepY;
            }
            return false;
        }

        private bool IsInvalidSquare(Point from) {
            return (from.X >= 0 && from.X <= 7 && from.Y >= 0 && from.Y > 7);
        }

        public void newGame() {
            Turn = WHITE;

            GameOver = false;

			// Reset castle flags
			whiteLeftCastle = true;
			whiteRightCastle = true;
			blackLeftCastle = true;
			blackRightCastle = true;

            moveStack.Clear();

            // Reset the board
            board[0, 0] = new ChessPiece(ROOK, BLACK);
            board[1, 0] = new ChessPiece(KNIGHT, BLACK);
            board[2, 0] = new ChessPiece(BISHOP, BLACK);
            board[3, 0] = new ChessPiece(QUEEN, BLACK);
            board[4, 0] = new ChessPiece(KING, BLACK);
            board[5, 0] = new ChessPiece(BISHOP, BLACK);
            board[6, 0] = new ChessPiece(KNIGHT, BLACK);
            board[7, 0] = new ChessPiece(ROOK, BLACK);

            for (int i = 0; i < 8; i++)
                board[i, 1] = new ChessPiece(PAWN, BLACK);

            for (int i = 2; i < 6; i++)
                for (int j = 0; j < 8; j++)
                    board[j, i] = null;

            for (int i = 0; i < 8; i++)
                board[i, 6] = new ChessPiece(PAWN, WHITE);

            board[0, 7] = new ChessPiece(ROOK, WHITE);
            board[1, 7] = new ChessPiece(KNIGHT, WHITE);
            board[2, 7] = new ChessPiece(BISHOP, WHITE);
            board[3, 7] = new ChessPiece(QUEEN, WHITE);
            board[4, 7] = new ChessPiece(KING, WHITE);
            board[5, 7] = new ChessPiece(BISHOP, WHITE);
            board[6, 7] = new ChessPiece(KNIGHT, WHITE);
            board[7, 7] = new ChessPiece(ROOK, WHITE);
        }
		
		
		private void ToggleTurn()
		{
            if (Turn == WHITE)
                Turn = BLACK;
            else
                Turn = WHITE;

		}
		
				
/*		

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
		
		private bool IsCheck()
		{
			ArrayList FromSet = new ArrayList(16);

			checkBoard = new ChessGame(this);
			checkBoard.ToggleTurn();
			checkBoard.ForceMove(moveFrom, moveTo);

			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
				{
					if (checkBoard[i, j] != null && checkBoard[i, j].Color != turn)
						FromSet.Add(new Point(i, j)); 
					else if (checkBoard[i, j] != null && checkBoard[i, j].Name == "King")
						checkBoard.moveTo = new Point(i, j);
				}

			foreach (Point pos in FromSet)
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
			
			checkBoard = new ChessGame(this);
			checkBoard.moveFrom = new Point(rook, Y);
			checkBoard.moveTo = new Point(moveFrom.X + inc, Y);
			if (checkBoard.IsValidMove() == ChessStatus.INVALID_BLOCK)
				return ChessStatus.INVALID_BLOCK;

			Point tempMoveTo = new Point(moveTo);

			int i;
			for (i = moveFrom.X; i!= tempMoveTo.X; i += inc)
			{
				moveTo = new Point(i, Y);
				if (IsCheck())
					return ChessStatus.INVALID_CASTLE;
			}

			// last iteration
			moveTo = new Point(i, Y);
			if (IsCheck())
				return ChessStatus.INVALID_CASTLE;

			moveTo = new Point(tempMoveTo); 
			ForceMove(new Point(rook, Y), new Point(moveTo.X - inc, Y));
			return ChessStatus.VALID;
		}

*/
	}



}
