using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

namespace ChessBoardModel
{
    public class Board
    {
        // default should be 8
        public int Size { get; set; }
        public Cell[,] TheGrid { get; set; }

        private string[] pieces = {"Rook","Knight","Bishop","Queen","King","Bishop","Knight","Rook"};

        public Board(int size)
        {
            this.Size = size;

            TheGrid = new Cell[Size, Size];

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    TheGrid[i, j] = new Cell(i, j);
                }
            }
            PopulateGridWithPieces();
        }

        public void PopulateGridWithPieces()
        {
            PopulateCellsWithFriendlyPieces();
            PopulateCellsWithEnemyPieces();
        }
        public void PopulateCellsWithEnemyPieces()
        {
            PopulateEnemyPawns();
            PopulateOtherEnemyPieces();
        }

        private void PopulateEnemyPawns()
        {
            for (int i = 0; i < Size; i++)
            {
                Piece piece = new Piece("Pawn", false);
                PopulateCell(TheGrid[i, 1], piece);
            }
        }
        private void PopulateOtherEnemyPieces()
        {
            for (int i = 0; i < Size; i++)
            {
                Piece piece = new Piece(pieces[i], false);
                PopulateCell(TheGrid[i, 0], piece);
            }
        }
        public void PopulateCellsWithFriendlyPieces()
        {
            PopulateFriendlyPawns();
            PopulateOtherFriendlyPieces();
        }

        private void PopulateOtherFriendlyPieces()
        {
            for (int i = 0; i < Size; i++)
            {
                Piece piece = new Piece(pieces[i], true);
                PopulateCell(TheGrid[i, Size - 1], piece);
            }
        }

        public void PopulateFriendlyPawns()
        {
            for (int i = 0; i < Size; i++)
            {
                Piece piece = new Piece("Pawn", true);
                PopulateCell(TheGrid[i, Size - 2], piece);
            }
        }

        public void PopulateCell(Cell currentCell, Piece piece)
        {
            currentCell.IsCurrentlyOccupied = true;
            currentCell.CurrentPiece = piece;
        }

        public void MarkNextLegalMoves(Cell currentCell, string chessPiece, bool isPlayerTurn)
        {
            switch (chessPiece)
            {
                case "Knight":
                    MarkNextLegalKnightMoves(currentCell, isPlayerTurn);
                    break;
                case "King":
                    MarkNextLegalKingMoves(currentCell, isPlayerTurn);
                    break;
                case "Rook":
                    MarkNextLegalRookMoves(currentCell, isPlayerTurn);
                    break;
                case "Bishop":
                    MarkNextLegalBishopMoves(currentCell, isPlayerTurn);
                    break;
                case "Queen":
                    MarkNextLegalQueenMoves(currentCell, isPlayerTurn);
                    break;
                case "Pawn":
                    MarkPawnLegalMoves(currentCell, isPlayerTurn);
                    break;
                default:
                    break;
            }
            TheGrid[currentCell.RowNumber, currentCell.ColumnNumber].IsCurrentlyOccupied = true;
        }

        public void MakeMove(Cell currentCell, Cell selectedCell, string chessPiece)
        {
            TheGrid[currentCell.RowNumber, currentCell.ColumnNumber].CurrentPiece = selectedCell.CurrentPiece;
            TheGrid[currentCell.RowNumber, currentCell.ColumnNumber].IsCurrentlyOccupied = true;
            TheGrid[selectedCell.RowNumber, selectedCell.ColumnNumber].CurrentPiece = null;
            TheGrid[selectedCell.RowNumber, selectedCell.ColumnNumber].IsCurrentlyOccupied = false;
           
            UpdatePawns();
            MakeAllCellsIllegal();
        }
        public void MarkNextLegalEnemyyMoves(Cell currentCell, string chessPiece)
        {

        }

        public void MakeNextLegalFriendlyMove(Cell currentCell, string chessPiece)
        {

        }

        public bool IsCellFriendlyOccupied(Cell currentCell)
        {
            if (currentCell.CurrentPiece != null)
            {
                return currentCell.CurrentPiece.IsPlayerControlled;
            }
            return false;
        }
        public bool IsThereAValidMove()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (TheGrid[i, j].IsLegalNextMove)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void UpdatePawns()
        {
            UpdatePawnsInRow(Size - 1);
            UpdatePawnsInRow(0);
        }

        public void UpdatePawnsInRow(int Row)
        {
            for (int i = 0; i < Size; i++)
            {
                if (TheGrid[i, Row].CurrentPiece != null)
                {
                    if (TheGrid[i, Row].CurrentPiece.PieceType == "Pawn")
                    {
                        TheGrid[i, Row].CurrentPiece.PieceType = "Queen";
                    }
                }
            }
        }

        public void MakeAllCellsIllegal()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    TheGrid[i, j].IsLegalNextMove = false;
                }
            }
        }

        public void MarkPawnLegalMoves(Cell currentCell, bool isPlayerControlled)
        {
            if (isPlayerControlled)
            {
                MarkFriendlyLegalMoves(currentCell, isPlayerControlled);
            }
            else
            {
                MarkEnemyLegalMoves(currentCell, isPlayerControlled);
            }
        }

        public void MarkFriendlyLegalMoves(Cell currentCell, bool isPlayerControlled)
        {
            if (currentCell.ColumnNumber == Size - 2)
            {
                //This is the moment I realised I got the rows and columns wrong the whole time 🙃
                TryMove(currentCell, 0, -1);
                TryMove(currentCell, 0, -2);
            }
            else
            {
                TryMove(currentCell, 0, -1);
            }
            TryTakePawn(currentCell, -1, -1, isPlayerControlled);
            TryTakePawn(currentCell, 1, -1, isPlayerControlled);
        }
        public void MarkEnemyLegalMoves(Cell currentCell, bool isPlayerControlled)
        {
            if (currentCell.ColumnNumber == 1)
            {
                //This is the moment I realised I got the rows and columns wrong the whole time 🙃
                TryMove(currentCell, 0, 1);
                TryMove(currentCell, 0, 2);
            }
            else
            {
                TryMove(currentCell, 0, 1);
            }
            TryTakePawn(currentCell, 1, 1, isPlayerControlled);
            TryTakePawn(currentCell, -1, 1, isPlayerControlled);
        }
        public void MarkNextLegalKnightMoves(Cell currentCell, bool isPlayerControlled)
        {
            // Try to move down right
            TryMove(currentCell,2,1, isPlayerControlled);
            TryMove(currentCell, 1, 2, isPlayerControlled);
            // Try to move down left
            TryMove(currentCell, 2, -1, isPlayerControlled);
            TryMove(currentCell, 1, -2, isPlayerControlled);
            // Try to move up right
            TryMove(currentCell, -2, 1, isPlayerControlled);
            TryMove(currentCell, -1, 2, isPlayerControlled);
            // Try to move up left
            TryMove(currentCell, -2, -1, isPlayerControlled);
            TryMove(currentCell, -1, -2, isPlayerControlled);
        }

        public void MarkNextLegalKingMoves(Cell currentCell,bool isPlayerControlled)
        {
            TryMove(currentCell, 1, 0, isPlayerControlled);
            TryMove(currentCell, 1, 1, isPlayerControlled);
            TryMove(currentCell, 0, 1, isPlayerControlled);
            TryMove(currentCell, -1, 1, isPlayerControlled);
            TryMove(currentCell, 1, -1, isPlayerControlled);
            TryMove(currentCell, -1, 0, isPlayerControlled);
            TryMove(currentCell, -1, -1, isPlayerControlled);
            TryMove(currentCell, 0, -1, isPlayerControlled);
        }

        public void MarkNextLegalRookMoves(Cell currentCell, bool isPlayerTurn)
        {
            MoveOrthogonallyEndless(currentCell, isPlayerTurn);
        }

        public void MarkNextLegalBishopMoves(Cell currentCell, bool isPlayerTurn)
        {
            MoveDiagonallyEndless(currentCell,isPlayerTurn);
        }

        public void MarkNextLegalQueenMoves(Cell currentCell,bool isPlayerTurn)
        {
            MoveOrthogonallyEndless(currentCell, isPlayerTurn);
            MoveDiagonallyEndless(currentCell, isPlayerTurn);

        }

        public void MoveOrthogonallyEndless(Cell currentCell,bool isPlayerTurn)
        {
            MoveInDirectionTillEndOfBoard(currentCell, -1, 0, isPlayerTurn);
            MoveInDirectionTillEndOfBoard(currentCell, 1, 0, isPlayerTurn);
            MoveInDirectionTillEndOfBoard(currentCell, 0, -1, isPlayerTurn);
            MoveInDirectionTillEndOfBoard(currentCell, 0, 1, isPlayerTurn);
        }
        public void MoveDiagonallyEndless(Cell currentCell,bool isPlayerTurn)
        {
            MoveInDirectionTillEndOfBoard(currentCell, -1, -1, isPlayerTurn);
            MoveInDirectionTillEndOfBoard(currentCell, 1, 1, isPlayerTurn);
            MoveInDirectionTillEndOfBoard(currentCell, 1, -1, isPlayerTurn);
            MoveInDirectionTillEndOfBoard(currentCell, -1, 1, isPlayerTurn);
        }

        public void MoveInDirectionTillEndOfBoard(Cell currentCell, int moveRow, int moveColumn, bool isPlayerTurn)
        {
            bool KeepMoving = true;
            bool IsMovingRow = moveRow != 0;
            bool IsMovingColumn = moveColumn != 0;
            while (KeepMoving)
            {
                KeepMoving = TryMoveAndReturnStatus(currentCell, moveRow, moveColumn, isPlayerTurn);
                if (IsMovingRow && moveRow > 0)
                {
                    moveRow += 1;
                } 
                else if (IsMovingRow && moveRow < 0)
                {
                    moveRow -= 1;
                }
                if (IsMovingColumn && moveColumn > 0)
                {
                    moveColumn += 1;
                }
                else if (IsMovingColumn && moveColumn < 0)
                {
                    moveColumn -= 1;
                }
            }
        }
        public bool TryMoveAndReturnStatus(Cell currentCell, int moveRow, int moveColumn, bool isPlayerTurn)
        {
            if (IsCellValid(currentCell, moveRow, moveColumn))
            {
                TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
                return true;
            }
            else
            {
                if (IsCellTakeable(currentCell, moveRow, moveColumn, isPlayerTurn))
                {
                    TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
                }
                return false;
            }
        }

        public bool IsCellTakeable(Cell currentCell, int moveRow, int moveColumn, bool isPlayerTurn)
        {
            bool IsRowSafe = false;
            bool IsColumnSafe = false;
            if (currentCell.RowNumber + moveRow < Size && currentCell.RowNumber + moveRow >= 0)
            {
                IsRowSafe = true;
            }
            if (currentCell.ColumnNumber + moveColumn < Size && currentCell.ColumnNumber + moveColumn  >= 0)
            {
                IsColumnSafe = true;
            }

            if (IsColumnSafe && IsRowSafe)
            {
                if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].CurrentPiece != null)
                {
                    if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsCurrentlyOccupied)
                    {
                        if (!TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn]
                            .CurrentPiece.IsPlayerControlled == isPlayerTurn)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void TryMove(Cell currentCell, int moveRow, int moveColumn)
        {
            if (IsCellValid(currentCell, moveRow, moveColumn))
            {
                TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
            }
        }
        public void TryMove(Cell currentCell, int moveRow, int moveColumn, bool isPlayerControlled)
        {
            if (IsCellValid(currentCell, moveRow, moveColumn, isPlayerControlled))
            {
                TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
            }
        }
        public void TryTakePawn(Cell currentCell, int moveRow, int moveColumn, bool isFriendlyControlled)
        {
            if (IsCellValidPawn(currentCell, moveRow, moveColumn, isFriendlyControlled))
            {
                TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
            }
        }
        public void Move(Cell currentCell, int moveRow, int moveColumn)
        {
            TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
        }
        public bool IsCellValidPawn(Cell currentCell, int moveRow, int moveColumn, bool isPlayerControlled)
        {
            bool IsRowSafe = false;
            bool IsColumnSafe = false;
            if (currentCell.RowNumber + moveRow < Size && currentCell.RowNumber + moveRow >= 0)
            {
                IsRowSafe = true;
            }
            if (currentCell.ColumnNumber + moveColumn < Size && currentCell.ColumnNumber + moveColumn >= 0)
            {
                IsColumnSafe = true;
            }
            if (IsColumnSafe && IsRowSafe)
            {
                if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].CurrentPiece != null)
                {
                    if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].CurrentPiece
                        .IsPlayerControlled != isPlayerControlled)
                    {
                        if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsCurrentlyOccupied)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public bool IsCellValid(Cell currentCell, int moveRow, int moveColumn, bool isPlayerControlled)
        {
            bool IsRowSafe = false;
            bool IsColumnSafe = false;
            if (currentCell.RowNumber + moveRow < Size && currentCell.RowNumber + moveRow >= 0)
            {
                IsRowSafe = true;
            }
            if (currentCell.ColumnNumber + moveColumn < Size && currentCell.ColumnNumber + moveColumn >= 0)
            {
                IsColumnSafe = true;
            }

            if (IsColumnSafe && IsRowSafe)
            {
                if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].CurrentPiece != null)
                {
                    if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].CurrentPiece.IsPlayerControlled == isPlayerControlled)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsCellValid(Cell currentCell, int moveRow, int moveColumn)
        {
            bool IsRowSafe = false;
            bool IsColumnSafe = false;
            if (currentCell.RowNumber + moveRow < Size && currentCell.RowNumber + moveRow >= 0)
            {
                IsRowSafe = true;
            }
            if (currentCell.ColumnNumber + moveColumn < Size && currentCell.ColumnNumber + moveColumn  >= 0)
            {
                IsColumnSafe = true;
            }

            if (IsColumnSafe && IsRowSafe)
            {
                if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].CurrentPiece != null)
                {
                    if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsCurrentlyOccupied)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        

        public void ClearBoard()
        {
            // clear board
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    TheGrid[i, j].IsLegalNextMove = false;
                    TheGrid[i, j].IsCurrentlyOccupied = false;
                }
            }
        }
    }
}
