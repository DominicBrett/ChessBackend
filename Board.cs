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
        public Game game { get; set; }

        private string[] pieces = {"Rook","Knight","Bishop","Queen","King","Bishop","Knight","Rook"};

        public Board(int size)
        {
            game = new Game();
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

        public void MarkNextLegalMoves(Cell curretCell, string chessPiece)
        {
            //Clear Board
            //ClearBoard();
            if (game.IsPlayerTurn)
            {
                if(game.IsPieceSelected && IsThereAValidMove())
                {
                    Cell selectedCell = game.SelectedCell;
                    if (curretCell.IsLegalNextMove)
                    {
                        TheGrid[curretCell.RowNumber, curretCell.ColumnNumber].CurrentPiece = selectedCell.CurrentPiece;
                        TheGrid[curretCell.RowNumber, curretCell.ColumnNumber].IsCurrentlyOccupied = true;
                        TheGrid[game.SelectedCell.RowNumber, game.SelectedCell.ColumnNumber].CurrentPiece = null;
                        TheGrid[game.SelectedCell.RowNumber, game.SelectedCell.ColumnNumber].IsCurrentlyOccupied = false;
                        game.SelectedCell = null;
                        game.IsPieceSelected = false;
                        UpdatePawns();
                        MakeAllCellsIllegal();
                    } 
                    else if (IsCellFriendlyOccupied(curretCell))
                    {
                        game.IsPieceSelected = false;
                        MakeAllCellsIllegal();
                    }
                }
                else
                {
                    if (curretCell.CurrentPiece != null)
                    {
                        if (curretCell.CurrentPiece.IsPlayerControlled)
                        {
                            switch(chessPiece)
                            {
                                case "Knight":
                                MarkNextLegalKnightMoves(curretCell);
                                break;
                                case "King":
                                MarkNextLegalKingMoves(curretCell);
                                break;
                                case "Rook":
                                MarkNextLegalRookMoves(curretCell);
                                break;
                                case "Bishop":
                                MarkNextLegalBishopMoves(curretCell);
                                break;
                                case "Queen":
                                MarkNextLegalQueenMoves(curretCell);
                                break;
                                case "Pawn":
                                MarkPawnLegalMoves(curretCell);
                                break;
                                default:
                                break;
                            }

                            if (chessPiece != null)
                            {
                                TheGrid[curretCell.RowNumber, curretCell.ColumnNumber].IsCurrentlyOccupied = true;
                                game.IsPieceSelected = true;
                                game.SelectedCell = curretCell;
                            }
                        }
                    }
                }
            }
            // find all legal moves and mark them

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

        public void MarkPawnLegalMoves(Cell currentCell)
        {
            if (currentCell.CurrentPiece.IsPlayerControlled)
            {
                MarkFriendlyLegalMoves(currentCell);
            }
            else
            {
                MarkEnemyLegalMoves(currentCell);
            }

            TryTakePawn(currentCell, -1, -1);
            TryTakePawn(currentCell, 1, -1);
        }

        public void MarkFriendlyLegalMoves(Cell currentCell)
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
        }
        public void MarkEnemyLegalMoves(Cell currentCell)
        {
            
        }
        public void MarkNextLegalKnightMoves(Cell currentCell)
        {
            // Try to move down right
            TryMove(currentCell,2,1);
            TryMove(currentCell, 1, 2);
            // Try to move down left
            TryMove(currentCell, 2, -1);
            TryMove(currentCell, 1, -2);
            // Try to move up right
            TryMove(currentCell, -2, 1);
            TryMove(currentCell, -1, 2);
            // Try to move up left
            TryMove(currentCell, -2, -1);
            TryMove(currentCell, -1, -2);
        }

        public void MarkNextLegalKingMoves(Cell currentCell)
        {
            TryMove(currentCell, 1, 0);
            TryMove(currentCell, 1, 1);
            TryMove(currentCell, 0, 1);
            TryMove(currentCell, -1, 1);
            TryMove(currentCell, 1, -1);
            TryMove(currentCell, -1, 0);
            TryMove(currentCell, -1, -1);
            TryMove(currentCell, 0, -1);
        }

        public void MarkNextLegalRookMoves(Cell currentCell)
        {
            MoveOrthogonallyEndless(currentCell);
        }

        public void MarkNextLegalBishopMoves(Cell currentCell)
        {
            MoveDiagonallyEndless(currentCell);
        }

        public void MarkNextLegalQueenMoves(Cell currentCell)
        {
            MoveOrthogonallyEndless(currentCell);
            MoveDiagonallyEndless(currentCell);

        }

        public void MoveOrthogonallyEndless(Cell currentCell)
        {
            MoveInDirectionTillEndOfBoard(currentCell, -1, 0);
            MoveInDirectionTillEndOfBoard(currentCell, 1, 0);
            MoveInDirectionTillEndOfBoard(currentCell, 0, -1);
            MoveInDirectionTillEndOfBoard(currentCell, 0, 1);
        }
        public void MoveDiagonallyEndless(Cell currentCell)
        {
            MoveInDirectionTillEndOfBoard(currentCell, -1, -1);
            MoveInDirectionTillEndOfBoard(currentCell, 1, 1);
            MoveInDirectionTillEndOfBoard(currentCell, 1, -1);
            MoveInDirectionTillEndOfBoard(currentCell, -1, 1);
        }

        public void MoveInDirectionTillEndOfBoard(Cell currentCell, int moveRow, int moveColumn)
        {
            bool KeepMoving = true;
            bool IsMovingRow = moveRow != 0;
            bool IsMovingColumn = moveColumn != 0;
            while (KeepMoving)
            {
                KeepMoving = TryMoveAndReturnStatus(currentCell, moveRow, moveColumn);
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
        public bool TryMoveAndReturnStatus(Cell currentCell, int moveRow, int moveColumn)
        {
            if (IsCellValid(currentCell, moveRow, moveColumn))
            {
                TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
                return true;
            }
            else
            {
                if (IsCellTakeable(currentCell, moveRow, moveColumn))
                {
                    TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
                }
                return false;
            }
        }

        public bool IsCellTakeable(Cell currentCell, int moveRow, int moveColumn)
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
                            .CurrentPiece.IsPlayerControlled)
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
         public void TryTakePawn(Cell currentCell, int moveRow, int moveColumn)
        {
            if (IsCellValidPawn(currentCell, moveRow, moveColumn))
            {
                TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
            }
        }
        public void Move(Cell currentCell, int moveRow, int moveColumn)
        {
            TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsLegalNextMove = true;
        }
        public bool IsCellValidPawn(Cell currentCell, int moveRow, int moveColumn)
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
                    if (TheGrid[currentCell.RowNumber + moveRow, currentCell.ColumnNumber + moveColumn].IsCurrentlyOccupied)
                    {
                        return true;
                    }
                }
                return false;
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
