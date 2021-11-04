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
        public Game()
        {
            IsPlayerTurn = true;
            IsPieceSelected = false;
        }

        public void SwitchTurn()
        {
            IsPlayerTurn = !IsPlayerTurn;
        }
    }
}
