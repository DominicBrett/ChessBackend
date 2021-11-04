using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoardModel
{
    public class Cell
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public bool IsCurrentlyOccupied { get; set; }
        public bool IsLegalNextMove { get; set; }
        public Piece CurrentPiece { get; set; }

        public Cell(int rowNumber, int columnNumber)
        {
            this.RowNumber = rowNumber;
            this.ColumnNumber = columnNumber;
        }
    }
}
