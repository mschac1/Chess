using System.Drawing;

namespace Chess {
	internal class ChessMove {
        internal Point To;
        internal Point From;
        internal ChessPiece opponent;
		internal Rank? Promotion = null;
        internal bool EnPassant;
        internal bool CastleStatusChangedLeft;
        internal bool CastleStatusChangedRight;

        internal ChessMove(Point from, Point to, ChessPiece opponent) {
			From = from;
			To = to;
			this.opponent = opponent;
		}
		
	}
}
