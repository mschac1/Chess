namespace Chess {
	public class ChessPiece {

		public ChessPiece(Rank rank, Color color) {
			Rank = rank;
			Color = color;
		}
		
		public Color Color { get; internal set; }
        public Rank Rank { get; internal set; }	
	}
	
	public enum Color {WHITE, BLACK};
	public enum Rank {KING, QUEEN, ROOK, KNIGHT, BISHOP, PAWN};
}