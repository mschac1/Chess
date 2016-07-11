namespace Chess {
	public class ChessPiece {
		protected Color color;
		protected Rank rank;

		public ChessPiece(Rank rank, Color color) {
			this.rank = rank;
			this.color = color;
		}
		
		public Color Color {
			get {return color;}
		}
		public Rank Rank {
			get {return rank;} 
		}
	
	}
	
	public enum Color {WHITE, BLACK};
	public enum Rank {KING, QUEEN, ROOK, KNIGHT, BISHOP, PAWN};
}