using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using static Chess.Color;
using static Chess.Rank;

namespace Chess
{

	// The delegate type used to find out what piece to promote pawns to
	public delegate Rank PawnCallback();

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
	
        Stack<ChessMove> moveStack = new Stack<ChessMove>();

		// The delegate used to find out what piece to promote pawns to
		public PawnCallback pawnPromoter;

        public string Status { get; private set; }
        public Color Turn { get; private set; }
        public bool GameOver { get; private set; }

        internal int LEFT = 0, RIGHT = 1;
        #endregion

        public ChessGame() {
			newGame();
		}


        public void newGame() {
            Turn = WHITE;

            GameOver = false;

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
            if (IsCheck(piece.Color)) {
                Status = "That move would put your king in check";
                ReverseMove();
                return false;
            }

            int oppositeSide = (this[move.To].Color == BLACK ? 7 : 0);
            if (this[move.To].Rank == PAWN && move.To.Y == oppositeSide) {
                this[move.To].Rank = pawnPromoter();
                move.Promotion = true;
            }


            // TODO check for checkmate, and stalemate

            // TODO Uncomment this ToggleTurn();

            // Find opponents King
            Point kingSquare = FindKing(Toggle(piece.Color));
            if (IsThreatened(kingSquare, piece.Color))
                Status = "Check!";

            return true;
		}

        private Point FindKing(Color color) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (this[i, j] != null && this[i, j].Rank == KING && this[i, j].Color == color)
                        return new Point(i, j);
                }
            }
            throw new Exception("No " + color + " King found on the board!");
        }

        private static Color Toggle(Color color) {
            return (color == BLACK ? WHITE : BLACK);
        }

        // Reverse the previous move
        public void ReverseMove() {
            if (moveStack.Count == 0)
                return;

            ChessMove move = moveStack.Pop();

            // Move Piece back
            this[move.From] = this[move.To];
            this[move.To] = move.Opponent;

            // Unpromote pawn
            if (move.Promotion)
                this[move.From].Rank = PAWN;

            // Uncastle
            int dX = move.From.X - move.To.X;
            if (this[move.From].Rank == KING && Math.Abs(dX) == 2) {
                // If king moved left, move the left rook back
                if (dX == 2) {
                    this[new Point(0, move.To.Y)] = this[new Point(3, move.To.Y)];
                    this[new Point(3, move.To.Y)] = null;
                }
                // If king moved right , move the right rook back
                else if (dX == -2) {
                    this[new Point(7, move.To.Y)] = this[new Point(5, move.To.Y)];
                    this[new Point(5, move.To.Y)] = null;
                }
            }
                
        }

        private void makeMove(ChessMove move) {
            this[move.To] = this[move.From];
            this[move.From] = null;

            if (move.EnPassant)
                this[new Point(move.To.X, move.From.Y)] = null;

            ProcessCastleStatus(move);

            moveStack.Push(move);
        }

        private void ProcessCastleStatus(ChessMove move) {
            // Fill in the move with the castle status as of the previous move (or all true if it's the first move)
            if (moveStack.Count == 0) {
                move.CastleStatus = new bool[,] { { true, true }, { true, true } };
            }
            else {
                ChessMove pMove = moveStack.Peek();
                for (int i = 0; i < 2; i++) {
                    for (int j = 0; j < 2; j++) {
                        move.CastleStatus[i, j] = pMove.CastleStatus[i, j];
                    }
                }
            }

            Rank rank = this[move.To].Rank;
            Color color = this[move.To].Color;

            // Move rook, if castleing
            int dX = move.To.X - move.From.X;
            if (rank == KING) {
                // If king moves left, left rook moves right
                if (dX == -2) {
                    this[new Point(3, move.To.Y)] = this[new Point(0, move.To.Y)];
                    this[new Point(0, move.To.Y)] = null;
                }
                // If king moves right , right rook moves left
                else if (dX == 2) {
                    this[new Point(5, move.To.Y)] = this[new Point(7, move.To.Y)];
                    this[new Point(7, move.To.Y)] = null;
                }

            }

            int homeRow = (color == WHITE ? 7 : 0);
            // If the king moves, set both of that color's castle flags to false
            if (rank == KING) {
                move.CastleStatus[(int) color, LEFT] = move.CastleStatus[(int) color, RIGHT] = false;
            }
            else if(rank == ROOK && move.From.Y == homeRow && (move.From.X == 0 || move.From.X == 7)) {
                int side = (move.From.X == 0 ? LEFT : RIGHT);
                move.CastleStatus[(int)color, side] = false;
            }

            // Castle status can also be affected if the a Rook is captured (otherwise one rook could move around to the other side
            // after the other rook has been taken and the game would think you could castle because a rook never moved from that spot
            int opponentRow = (color == WHITE ? 0 : 7);
            
            if (move.Opponent != null && move.Opponent.Rank == ROOK && move.To.Y == opponentRow && (move.To.X == 0 || move.To.X == 7)) {
                int side = (move.To.X == 0 ? LEFT : RIGHT);
                move.CastleStatus[(int) move.Opponent.Color, side] = false;
            } 
        }

        // Is the King of Color <color> in Check
        private bool IsCheck(Color color) {
            Point kingSquare = FindKing(color);
            Color opponentColor = Toggle(color);
            return IsThreatened(kingSquare, opponentColor);
        }

        // TODO is the <square> threatened by any <color> piece
        private bool IsThreatened(Point square, Color color) {
            
            return false;
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
            if (dX == 0 && dY == allowedDirection && move.Opponent == null)
                return move;

            // Move 2 squares forward
            int homeRow = (piece.Color == BLACK ? 1 : 6);
            if (dX == 0 && move.From.Y == homeRow && dY == allowedDirection * 2 && move.Opponent == null)
                return move;


            // Check attack
            if (dX == 1 && dY == allowedDirection) {
                if (move.Opponent != null)
                    return move;
                else
                    return AttemptEnPassant(move);
                    return null;
            }
            return null;
        }

        private ChessMove AttemptEnPassant(ChessMove move) {
            ChessMove pMove = moveStack.Peek();
            
            if (this[pMove.To].Rank == PAWN && Math.Abs(pMove.To.Y - pMove.From.Y) == 2 && pMove.To.X == move.To.X) {
                move.EnPassant = true;
                return move;
            }
            return null;
        }

        private bool IsCastleAllowed(Point from, Point to) {
            // Check castle flag
            Color color = this[from].Color;
            int side = (to.X == 2 ? LEFT : RIGHT);
            if (!moveStack.Peek().CastleStatus[(int)color, side])
                return false;

            // Check whether there are any pieces in between
            int rookColumn = (side == LEFT ? 0 : 7);
            if (IsPieceBetween(from, new Point(rookColumn, from.Y)))
                return false;

            // TODO Check wether any of the spaces between the king and where it is moving are in check
            // We don't need to check the space wherer the king is moving to because that will be checked at the IsCheck stage
            if (IsCheck(color))
                return false;

            return true;
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

		
		
		private void ToggleTurn() {
            Turn = Toggle(Turn);
		}
		
	}



}
