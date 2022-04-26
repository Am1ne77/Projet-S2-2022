using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Thegame : MonoBehaviour
{
    const int RowCount = 6;
    const int ColsCount = 7;

    const char Empty = '.';
    const char Player1Piece = '1';
    const char Player2Piece = '1';

    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;
    public Button Button5;
    public Button Button6;
    public Button Button7;
    

    int Player1 = 1;
    int Player2 = 0;

    
    //Initialise the board
    private char[][] CreateBoard()
    {
        List<char[]> b = new List<char[]>();
        for (int i = 0; i < 6; i++)
        {
            List<char> un = new List<char>();
            for (int j = 0; j < 7; j++)
            {
                un.Add('.');
            }
            b.Add(un.ToArray());
        }

        return b.ToArray();
    }
    
    
    
    //drop a piece
    private void DropPiece(char[][] b, int row, int col, char piece)
    {
        b[row][col] = piece;
    }
    
    
    
    //check if location is valid
    private bool IsValidLocation(char[][] b, int col)
    {
        return b[0][col] == '.';
    }
    
    
    
    //find the next open row to place the piece
    private int GetNextOpenRow(char[][] b, int col)
    {
        for (int i = 5; i >= 0; i--)
        {
            if (b[i][col] == '.')
            {
                return i;
            }
        }

        return -1;
    }
    
    

    //is the game over?
    private bool Win(char[][] b, char piece)
    {
        
        //Horizontal check
        for (int c = 0; c < ColsCount - 3; c++)
        {
            for (int r = 0; r < RowCount; r++)
            {
                if (b[r][c] == piece && b[r][c+1] == piece && b[r][c+2] == piece && b[r][c+3] == piece)
                {
                    return true;
                }
            }
        }
        
        //Vertical check
        for (int c = 0; c < ColsCount; c++)
        {
            for (int r = 0; r < RowCount - 3; r++)
            {
                if (b[r][c] == piece && b[r+1][c] == piece && b[r+2][c] == piece && b[r+3][c] == piece)
                {
                    return true;
                }
            }
        }
        
        //Inclined 1
        for (int c = 0; c < ColsCount - 3; c++)
        {
            for (int r = 0; r < RowCount - 3; r++)
            {
                if (b[r][c] == piece && b[r+1][c+1] == piece && b[r+2][c+2] == piece && b[r+3][c+3] == piece)
                {
                    return true;
                }
            }
        }
        
        //Inclined 2
        for (int c = 0; c < ColsCount - 3; c++)
        {
            for (int r = 3; r < RowCount; r++)
            {
                if (b[r][c] == piece && b[r-1][c+1] == piece && b[r-2][c+2] == piece && b[r-3][c+3] == piece)
                {
                    return true;
                }
            }
        }

        return false;
    }


    #region AI
    
    //Heuristic of board
    private int EvaluateBlock(char[] blo, char piece)
    {
        int score = 0;
        char opp_piece = Player1Piece;
        if (piece == Player1Piece)
        {
            opp_piece = Player2Piece;
        }

        if (blo.Count(c => piece == c) == 4)
        {
            score += 100;
        }
        
        if (blo.Count(c => piece == c) == 3 && blo.Count(c => Empty == c) == 1)
        {
            score += 15;
        }
        
        if (blo.Count(c => piece == c) == 2 && blo.Count(c => Empty == c) == 2)
        {
            score += 5;
        }
        
        if (blo.Count(c => piece == c) == 3 && blo.Count(c => Empty == c) == 1)
        {
            score += 2;
        }
        
        if (blo.Count(c => opp_piece == c) == 4)
        {
            score -= 100;
        }
        
        if (blo.Count(c => opp_piece == c) == 3 && blo.Count(c => Empty == c) == 1)
        {
            score -= 10;
        }
        
        if (blo.Count(c => opp_piece == c) == 2 && blo.Count(c => Empty == c) == 2)
        {
            score -= 5;
        }
        
        if (blo.Count(c => opp_piece == c) == 1 && blo.Count(c => Empty == c) == 3)
        {
            score -= 5;
        }

        return score;
    }

    private char[] MakeListVertical(char[][]b, int col)
    {
        List<char> res = new List<char>();
        for (int i = 0; i < RowCount; i++)
        {
            res.Add(b[i][col]);
        }

        return res.ToArray();
    }
    private char[] MakeListHorizontal(char[][]b, int row)
    {
        return b[row];
    }

    private char[] MakeListPosSLope(char[][] b, int row, int col)
    {
        List<char> res = new List<char>();

        for (int i = 0; i < WindowLen; i++)
        {
            res.Add(b[row+i][col+i]);
        }

        return res.ToArray();
    }
    
    private char[] MakeListNegSLope(char[][] b, int row, int col)
    {
        List<char> res = new List<char>();

        for (int i = 0; i < WindowLen; i++)
        {
            res.Add(b[row+3-i][col+i]);
        }

        return res.ToArray();
    }

    private char[] Slice(char[] l, int start, int end)
    {
        List<char> tamtam = new List<char>();
        for (int i = start; i < end; i++)
        {
            tamtam.Add(l[i]);
        }

        return tamtam.ToArray();
    }
    
    private const int WindowLen = 4;
    
    private int ScorePosition(char[][] b, char piece)
    {
        int score = 0;

        //Score center column
        char[] center = MakeListVertical(b, 3);
        int center_count = center.Count(c => c == piece);
        score += center_count * 2;
        
        //Score Horizontal
        for (int r = 0; r < RowCount; r++)
        {
            char[] row_array = MakeListHorizontal(b,r);
            for (int c = 0; c < RowCount - 3; c++)
            {
                char[] window = Slice(row_array, c, c + WindowLen);
                score += EvaluateBlock(window, piece);
            }
        }
        
        //Score Vertical
        for (int c = 0; c < ColsCount; c++)
        {
            char[] cols_array = MakeListVertical(b, c);
            for (int r = 0; r < RowCount - 3; r++)
            {
                char[] window = Slice(cols_array, r, r + WindowLen);
                score += EvaluateBlock(window, piece);
            }
        }
        
        //Score Positive Slope
        for (int r = 0; r < RowCount - 3; r++)
        {
            for (int c = 0; c < ColsCount - 3; c++)
            {
                char[] window = MakeListPosSLope(b, r, c);
                score += EvaluateBlock(window, piece);
            }
        }
        
        //Score Negative Slope
        for (int r = 0; r < RowCount - 3; r++)
        {
            for (int c = 0; c < ColsCount - 3; c++)
            {
                char[] window = MakeListNegSLope(b, r, c);
                score += EvaluateBlock(window, piece);
            }
        }

        return score;
    }


    //Makes list of valid cols
    private int[] GetValidLocations(char[][] b)
    {
        List<int> loc = new List<int>();
        for (int col = 0; col < ColsCount; col++)
        {
            if (IsValidLocation(b, col))
            {
                loc.Add(col);
            }
        }

        return loc.ToArray();
    }

    //Check for the end
    private bool IsTerminalNode(char[][] b)
    {
        return Win(b, Player1Piece) || Win(b, Player2Piece) || GetValidLocations(b).Length == 0;
    }

    private int Max(int a, int b)
    {
        if (a > b)
        {
            return a;
        }

        return b;
    }
    private int Min(int a, int b)
    {
        if (a < b)
        {
            return a;
        }

        return b;
    }
    
    private char[][] Copy(char[][] b)
    {
        List<char[]> res = new List<char[]>();
        for (int i = 0; i < 6; i++)
        {
            List<char> dub = new List<char>();
            for (int j = 0; j < 7; j++)
            {
                dub.Add(b[i][j]);
            }
            res.Add(dub.ToArray());
        }

        return res.ToArray();
    }
    
    private (int?,int) Minimax(char[][] b, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        int[] valid_locations = GetValidLocations(b);
        bool is_terminal = IsTerminalNode(b);
        if (depth == 0 || is_terminal)
        {
            if (is_terminal)
            {
                if (Win(b,Player2Piece))
                {
                    return (null,Int32.MaxValue);
                }
                else if (Win(b, Player1Piece))
                {
                    return (null, Int32.MinValue);
                }
                else
                {
                    return (null, 0);
                }
            }
            else
            {
                return (null, ScorePosition(b, Player2Piece));
            }
        }

        if (maximizingPlayer)
        {
            int value = Int32.MinValue;
            int column = 3;
            foreach (var col in valid_locations)
            {
                int row = GetNextOpenRow(b, col);
                char[][] copy = Copy(b);
                DropPiece(copy,row,col,Player2Piece);
                int new_score = Minimax(copy, depth - 1, alpha, beta, false).Item2;
                if (new_score > value)
                {
                    value = new_score;
                    column = col;
                }

                alpha = Max(alpha, value);
                if (alpha >= beta)
                {
                    break;
                }
            }

            return (column, value);
        }
        else
        {
            int value = Int32.MaxValue;
            int column = 3;
            foreach (var col in valid_locations)
            {
                int row = GetNextOpenRow(b, col);
                char[][] copy = Copy(b);
                DropPiece(copy,row,col,Player1Piece);
                int new_score = Minimax(copy, depth - 1, alpha, beta, true).Item2;
                if (new_score < value)
                {
                    value = new_score;
                    column = col;
                }

                beta = Min(beta, value);
                if (alpha >= beta)
                {
                    break;
                }
            }

            return (column, value);
        }
    }
    #endregion

    //setup the game
    public void setup()
    {
        char[][] board = CreateBoard();
    }
}
