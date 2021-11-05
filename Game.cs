using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoardModel
{
    public class Game
    {
        public bool IsPlayerTurn { get; set; }
        public bool IsPieceSelected { get; set; }
        public Cell SelectedCell { get; set; }
        public Board TheBoard { get; set; }
        public Game(Board board)
        {
            IsPlayerTurn = true;
            IsPieceSelected = false;
            this.TheBoard = board;
        }

        public void SwitchTurn()
        {
            IsPlayerTurn = !IsPlayerTurn;
        }

        public void Turn(int x, int y, string piece)
        {
            Cell currentCell = TheBoard.TheGrid[x, y];
            if (IsPlayerTurn)
            {
                MakePlayerTurn(currentCell, piece);
            }
            else
            {
                MakeEnemyTurn(currentCell, piece);
            }
        }
        public void MakePlayerTurn(Cell currentCell, string piece)
        {
            if (IsPieceSelected)
            {
                if (currentCell.IsLegalNextMove)
                {
                    TheBoard.MakeMove(currentCell, SelectedCell, piece);
                    SelectedCell = null;
                    IsPieceSelected = false;
                    SwitchTurn();
                }
                else
                {
                    IsPieceSelected = false;
                    TheBoard.MakeAllCellsIllegal();
                }
            }
            else
            {
                if (currentCell.CurrentPiece != null)
                {
                    if (currentCell.CurrentPiece.IsPlayerControlled)
                    {
                        TheBoard.MarkNextLegalMoves(currentCell, piece);
                        IsPieceSelected = true;
                        SelectedCell = currentCell;
                    }
                }
            }
        }
        public void MakeEnemyTurn(Cell currentCell, string piece)
        {
            if (IsPieceSelected)
            {
                if (currentCell.IsLegalNextMove)
                {
                    TheBoard.MakeMove(currentCell, SelectedCell, piece);
                    SelectedCell = null;
                    IsPieceSelected = false;
                    SwitchTurn();
                }
                else
                {
                    IsPieceSelected = false;
                    TheBoard.MakeAllCellsIllegal();
                }
            }
            else
            {
                if (currentCell.CurrentPiece != null)
                {
                    if (!currentCell.CurrentPiece.IsPlayerControlled)
                    {
                        TheBoard.MarkNextLegalMoves(currentCell, piece);
                        IsPieceSelected = true;
                        SelectedCell = currentCell;
                    }
                }
            }
        }
    }
}
