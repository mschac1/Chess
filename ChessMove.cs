using System.Drawing;

namespace Chess {
	internal class ChessMove {
        internal Point To;
        internal Point From;
        internal ChessPiece Opponent;
		internal bool Promotion;
        internal bool EnPassant;
        internal bool[,] CastleStatus = new bool[2,2];

        internal ChessMove(Point from, Point to, ChessPiece opponent) {
			From = from;
			To = to;
			Opponent = opponent;
		}
		
	}
}
