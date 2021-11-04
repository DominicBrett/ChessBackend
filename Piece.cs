using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoardModel
{
    public class Piece
    {
        public string PieceType { get; set; }
        public bool IsAlive { get; set; }
        public bool IsPlayerControlled { get; set; }

        public Piece(string pieceType , bool isPlayerControlled)
        {
            this.PieceType = pieceType;
            this.IsPlayerControlled = isPlayerControlled;
        }
    }
}
