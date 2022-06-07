using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using Data;

using UnityEngine.SceneManagement;
using Object = System.Object;

public class Thegame : MonoBehaviourPun, IPunObservable
{
    const int RowCount = 6;
    const int ColsCount = 7;

    const char Empty = '.';
    const char Player1Piece = '1';
    const char Player2Piece = '2';
    private const int WindowLen = 4;
    
    private char[][] Board;//
    private bool aienabled;
    private bool isplayerturn;
    private bool notyet;
    private bool gamestarted;
    private int AiDifficulty;
    private int turn;//
    private string support;
    
    public FirebaseManager Firebase;
    private Player[] listPlayer;


    public GameObject Pions1;
    public GameObject Pions2;
    public GameObject Plateau;
    public GameObject Buttons;
    public Text TurnIndicator;
    public Canvas DifficultyCanvas;

    public GameObject EndGameUi;
    public GameObject YouWonUi;
    public GameObject YouLostUi;
    public GameObject DrawUi;

    public GameObject DisconnectedPlayer;
    //public PhotonStream stream;
    

    
 
    
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
    [PunRPC]
    private void DropPiece(char[][] b, int row, int col, char piece)
    {
        b[row][col] = piece;
    }
    
    
    
    //check if location is valid
    private bool IsValidLocation(char[][] b, int? col)
    {
        return b[0][(int) col] == '.';
    }
    
    
    
    [PunRPC]
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

    private bool Draw(char[][] b)
    {
        var a = "";
        foreach (var col in b)
        {
            foreach (var pion in col)
            {
                a += " " + pion;

            }

            a += '\n';
        }
        Debug.Log((a));
        return b[0].Count(c => c == '.') ==0;
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
            score += 5;
        }

        if (blo.Count(c => piece == c) == 2 && blo.Count(c => Empty == c) == 2)
        {
            score += 2;
        }

        if (blo.Count(c => opp_piece == c) == 3 && blo.Count(c => Empty == c) == 1)
        {
            score -= 4;
        }
        

        return score;
    }

    private char[] MakeListVertical(char[][] b, int col)
    {
        List<char> res = new List<char>();
        for (int i = 0; i < RowCount; i++)
        {
            res.Add(b[i][col]);
        }

        return res.ToArray();
    }
    private char[] MakeListHorizontal(char[][] b, int row)
    {
        return b[row];
    }

    private char[] MakeListPosSLope(char[][] b, int row, int col)
    {
        List<char> res = new List<char>();

        for (int i = 0; i < WindowLen; i++)
        {
            res.Add(b[row + i][col + i]);
        }

        return res.ToArray();
    }
    
    private char[] MakeListNegSLope(char[][] b, int row, int col)
    {
        List<char> res = new List<char>();

        for (int i = 0; i < WindowLen; i++)
        {
            res.Add(b[row + 3 - i][col + i]);
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
            char[] row_array = MakeListHorizontal(b, r);
            for (int c = 0; c < ColsCount - 3; c++)
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
            if (IsValidLocation(b,col))
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
    
    private (int?, int) Minimax(char[][] b, int depth, int alpha, int beta, bool maximizingPlayer)
            {
                int[] valid_locations = GetValidLocations(b);
                bool is_terminal = IsTerminalNode(b);
                if (depth == 0 || is_terminal)
                {
                    if (is_terminal)
                    {
                        if (Win(b, Player2Piece))
                        {
                            return (null, Int32.MaxValue);
                        }
                        if (Win(b, Player1Piece))
                        {
                            return (null, Int32.MinValue);
                        }
                        else
                        {
                            Console.WriteLine("yo");
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
                        DropPiece(copy, row, col, Player2Piece);
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
                        DropPiece(copy, row, col, Player1Piece);
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
    public char[][] setup()
    {
        char[][] board = CreateBoard();
        return board;
    }
    
    public void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
        Firebase=FirebaseManager.Instance;

        
        
        Board = setup();
        if (PhotonNetwork.IsConnected)
        {
            aienabled = PhotonNetwork.CurrentRoom.PlayerCount != 2;
            if (!aienabled)
            {
                turn = 0;
                listPlayer = PhotonNetwork.PlayerList;
                int rand = Random.Range(0, 2);
                gamestarted = true;
                //(listPlayer[rand],listPlayer[1-rand]) = (listPlayer[1],listPlayer[0]);
                
                TurnIndicator.gameObject.SetActive(true);
                ToggleButtonsOnline();
                
            }
        }
        else
        {
            
            aienabled = true;
            
            //isplayerturn = isplayerturn = Random.Range(0,2) == 1;
            isplayerturn = Random.Range(0,2) == 1;
            DifficultyCanvas.gameObject.SetActive(true);
            gamestarted = false;
            for (int i = 0; i < 7; i++)
            {
                Buttons.transform.Find("Button_Col_" + i + "_").gameObject.SetActive(false);
            }
        }

        //TurnIndicator.gameObject.SetActive(false);
        
        
    }

    public void Update()
    {
        if (!gamestarted)
        {
            Debug.Log("update : gamestarted is false");
            return;
        }

        if (aienabled)
        {
            if (Win(Board, Player1Piece) || Win(Board, Player2Piece) || Draw(Board))
            {
                TurnIndicator.gameObject.SetActive(false);
                EndGameUi.SetActive(true);
                Setting.SetActive(false);

                if (Win(Board, Player1Piece))
                {
                    Firebase.xpPuissance4 += 150 * AiDifficulty;
                    YouWonUi.SetActive(true);
                }
                else
                {
                    if (Draw(Board))
                    {
                        DrawUi.SetActive(true);
                    }
                    else
                    {
                        Firebase.xpPuissance4 -= 100/AiDifficulty;
                        YouLostUi.SetActive(true);
                    }
                }
                
                gamestarted = false;
            }

            if (!isplayerturn && !notyet)
            {
                ToggleButtons();
                var res = Minimax(Board, AiDifficulty, Int32.MinValue, Int32.MaxValue, true);
                if (res.Item1 != null && IsValidLocation(Board, res.Item1))
                {
                    notyet = true;
                    switch (res.Item1)
                    {
                        case 0:
                            Invoke("DropCol0P2", 3);
                            break;
                        case 1:
                            Invoke("DropCol1P2", 3);
                            break;
                        case 2:
                            Invoke("DropCol2P2", 3);
                            break;
                        case 3:
                            Invoke("DropCol3P2", 3);
                            break;
                        case 4:
                            Invoke("DropCol4P2", 3);
                            break;
                        case 5:
                            Invoke("DropCol5P2", 3);
                            break;
                        case 6:
                            Invoke("DropCol6P2", 3);
                            break;
                    }

                    Invoke("ToggleButtons", 6);
                }
                else
                {
                    var col = Random.Range(0, 7);
                    while (!IsValidLocation(Board,col))
                    {
                        col = Random.Range(0, 7);
                    }
                    
                    notyet = true;
                    switch (col)
                    {
                        case 0:
                            Invoke("DropCol0P2", 3);
                            break;
                        case 1:
                            Invoke("DropCol1P2", 3);
                            break;
                        case 2:
                            Invoke("DropCol2P2", 3);
                            break;
                        case 3:
                            Invoke("DropCol3P2", 3);
                            break;
                        case 4:
                            Invoke("DropCol4P2", 3);
                            break;
                        case 5:
                            Invoke("DropCol5P2", 3);
                            break;
                        case 6:
                            Invoke("DropCol6P2", 3);
                            break;
                    }

                    Invoke("ToggleButtons", 6);
                }
            }
        }
        else
        {
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                DisconnectedPlayer.SetActive(true); 
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(2);
                return;

            }
            if (!gamestarted)
            {
                
                Debug.Log("update : gamestarted is false");
                return;
            }
            
            if (Win(Board, Player1Piece) || Win(Board, Player2Piece) || Draw(Board))
            {
                TurnIndicator.gameObject.SetActive(false);
                EndGameUi.SetActive(true);
                Setting.SetActive(false);

                if (Win(Board, Player1Piece))
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Firebase.xpPuissance4 += 200 * turn;
                        YouWonUi.SetActive(true);
                    }
                    else
                    {
                        Firebase.xpPuissance4 -= 100 * turn;
                        YouLostUi.SetActive(true);
                    }
                }
                else
                {
                    if (Draw(Board))
                    {
                        DrawUi.SetActive(true);
                    }
                    else
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Firebase.xpPuissance4 -= 100 * turn;
                            YouLostUi.SetActive(true);
                        }
                        else
                        {
                            Firebase.xpPuissance4 += 200 * turn;
                            YouWonUi.SetActive(true);
                        }
                    }
                }

                gamestarted = false;
            }
        }
    }
    public void ToggleButtons()
    {
        bool active = !Buttons.transform.Find("Button_Col_0_").gameObject.activeInHierarchy;

        if (TurnIndicator.gameObject.activeInHierarchy)
        {

            Text toChange = GameObject.Find("Canvas/Turn Indicator").GetComponent<Text>();
            if (active)
            {
                toChange.text = "Your Turn !";
                toChange.color = new Color((248.0f / 255.0f), (144.0f / 255.0f), (231.0f / 255.0f));
            }
            else
            {
                toChange.text = "Wait ...";
                toChange.color = new Color((11.0f / 255.0f), (211.0f / 255.0f), (211.0f / 255.0f));
            }
        }

        for (int i = 0; i < 7; i++)
        {
            Buttons.transform.Find("Button_Col_" + i +"_").gameObject.SetActive(active);
        }
    }
    
    [PunRPC]
    public void ToggleButtonsOnline()
    {
        bool active = Equals(PhotonNetwork.PlayerList[turn % 2], PhotonNetwork.LocalPlayer);
        active = ((turn % 2 == 0 && PhotonNetwork.IsMasterClient) ||
                  (turn % 2 == 1 && (!PhotonNetwork.IsMasterClient)));

        if (TurnIndicator.gameObject.activeInHierarchy)
        {
            Text toChange = GameObject.Find("Canvas/Turn Indicator").GetComponent<Text>();
            if (active)
            {
                toChange.text = "Your Turn !";
                toChange.color = new Color((248.0f / 255.0f), (144.0f / 255.0f), (231.0f / 255.0f));
            }
            else
            {
                toChange.text = "Wait ...";
                toChange.color = new Color((11.0f / 255.0f), (211.0f / 255.0f), (211.0f / 255.0f));
            }
        }

        for (int i = 0; i < 7; i++)
        {
            Buttons.transform.Find("Button_Col_" + i +"_").gameObject.SetActive(active);
        }
    }

    //C moche mais flemme
    #region DropColsPlayer1
    [PunRPC]
    public void DropCol0()
    {
        if (aienabled)
        {
            if (!isplayerturn)
            {
                return;
            }

            if (!IsValidLocation(Board, 0))
            {
                Debug.Log("Col full");
                return;
            }

            int row = GetNextOpenRow(Board, 0);
            DropPiece(Board, row, 0, Player1Piece);
            var spawn = new Vector3(265, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.0" + row).gameObject.SetActive(true);
                support = "support.0" + row;
            }

            Instantiate(Pions1, spawn, transform.rotation);
            isplayerturn = false;
            notyet = false;
            
        }
        else
        {
            Debug.Log(Equals(PhotonNetwork.PlayerList[turn % 2],PhotonNetwork.LocalPlayer));
            //if (!Equals(PhotonNetwork.PlayerList[turn % 2],PhotonNetwork.LocalPlayer))
            if ((turn % 2 != 0 && PhotonNetwork.IsMasterClient)||(turn % 2 != 1 && (!PhotonNetwork.IsMasterClient)))
            {
                return;
            }

            if (!IsValidLocation(Board, 0))
            {
                Debug.Log("Col full");
                return;
            }

            char p = Player1Piece;
            GameObject r = Pions1;
            int row = GetNextOpenRow(Board, 0);
            if (!PhotonNetwork.IsMasterClient)
            {
                p = Player2Piece;
                r = Pions2;
            }

            DropPiece(Board, row, 0, p);
            var spawn = new Vector3(265, 25, 120);
            Debug.Log("row: "+row);
           
            if (row!= 0)
            {
                //photonView.RPC("Support",RpcTarget.All,'0',row);
                Debug.Log(("active supp"));
                
                Plateau.transform.Find("support.0"+row.ToString()).gameObject.SetActive(true);
            }
        
            PhotonNetwork.Instantiate(r.name, spawn, transform.rotation);
            photonView.RPC("AddTurn",RpcTarget.All);
            notyet = false;
            ToggleButtonsOnline();
        }
    }

    [PunRPC]
    private void Support(char col, int row)
    {
        if (row != 5)
        {
            Plateau.transform.Find("support." + col.ToString() + row.ToString()).gameObject.SetActive(true);
            Debug.Log("support  active " + row);
        }
    }

    [PunRPC]
    private void AddTurn()
    {
        turn++;
    }
    
    public void DropCol1()
    {
        if (aienabled)
        {
            if (!isplayerturn)
            {
                return;
            }

            if (!IsValidLocation(Board, 1))
            {
                Debug.Log("Col full");
                return;
            }

            int row = GetNextOpenRow(Board, 1);
            DropPiece(Board, row, 1, Player1Piece);
            var spawn = new Vector3((float) 267.5, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.1" + row).gameObject.SetActive(true);
            }

            Instantiate(Pions1, spawn, transform.rotation);
            isplayerturn = false;
            notyet = false;
        }
        else
        {
            if ((turn % 2 != 0 && PhotonNetwork.IsMasterClient)||(turn % 2 != 1 && (!PhotonNetwork.IsMasterClient)))
            {
                return;
            }
            if (!IsValidLocation(Board, 1))
            {
                Debug.Log("Col full");
                return;
            }

            char p = Player1Piece;
            GameObject r = Pions1;
            int row = GetNextOpenRow(Board, 1);
            if (!PhotonNetwork.IsMasterClient)
            {
                p = Player2Piece;
                r = Pions2;
            }
            
            DropPiece(Board, row, 1, p);
            var spawn = new Vector3((float) 267.5, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.1" + row).gameObject.SetActive(true);
            }

            PhotonNetwork.Instantiate(r.name, spawn, transform.rotation);
            photonView.RPC("AddTurn",RpcTarget.All);
            notyet = false;
            Debug.Log(Board[0][5]);
            ToggleButtonsOnline();
            
        }
    }
    
    public void DropCol2()
    {
        if (aienabled)
        {


            if (!isplayerturn)
            {
                return;
            }

            if (!IsValidLocation(Board, 2))
            {
                Debug.Log("Col full");
                return;
            }

            int row = GetNextOpenRow(Board, 2);
            DropPiece(Board, row, 2, Player1Piece);
            var spawn = new Vector3((float) 270.25, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.2" + row).gameObject.SetActive(true);
            }

            Instantiate(Pions1, spawn, transform.rotation);
            isplayerturn = false;
            notyet = false;
        }
        else
        {
            if ((turn % 2 != 0 && PhotonNetwork.IsMasterClient)||(turn % 2 != 1 && (!PhotonNetwork.IsMasterClient)))
            {
                return;
            }
            if (!IsValidLocation(Board, 2))
            {
                Debug.Log("Col full");
                return;
            }

            char p = Player1Piece;
            GameObject r = Pions1;
            int row = GetNextOpenRow(Board, 2);
            if (!PhotonNetwork.IsMasterClient)
            {
                p = Player2Piece;
                r = Pions2;
            }
            
            DropPiece(Board, row, 2, p);
            var spawn = new Vector3((float) 270.25, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.2" + row).gameObject.SetActive(true);
            }

            PhotonNetwork.Instantiate(r.name, spawn, transform.rotation);
            photonView.RPC("AddTurn",RpcTarget.All);
            notyet = false;
            Debug.Log(Board[0][5]);
            ToggleButtonsOnline();
        }
    }
    
    public void DropCol3()
    {
        if (aienabled)
        {

            if (!isplayerturn)
            {
                return;
            }

            if (!IsValidLocation(Board, 3))
            {
                Debug.Log("Col full");
                return;
            }

            int row = GetNextOpenRow(Board, 3);
            DropPiece(Board, row, 3, Player1Piece);
            var spawn = new Vector3((float) 272.75, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.3" + row).gameObject.SetActive(true);
            }

            Instantiate(Pions1, spawn, transform.rotation);
            isplayerturn = false;
            notyet = false;
        }
        else
        {
            
            if ((turn % 2 != 0 && PhotonNetwork.IsMasterClient)||(turn % 2 != 1 && (!PhotonNetwork.IsMasterClient)))
            {
                return;
            }
            if (!IsValidLocation(Board, 3))
            {
                Debug.Log("Col full");
                return;
            }

            char p = Player1Piece;
            GameObject r = Pions1;
            int row = GetNextOpenRow(Board, 3);
            if (!PhotonNetwork.IsMasterClient)
            {
                p = Player2Piece;
                r = Pions2;
            }
            
            DropPiece(Board, row, 3, p);
            var spawn = new Vector3((float) 272.75, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.3" + row).gameObject.SetActive(true);
            }

            PhotonNetwork.Instantiate(r.name, spawn, transform.rotation);
            photonView.RPC("AddTurn",RpcTarget.All);
            notyet = false;
            Debug.Log(Board[0][5]);
            ToggleButtonsOnline();
        }
    }
    
    public void DropCol4()
    {
        if (aienabled)
        {
            if (!isplayerturn)
            {
                return;
            }

            if (!IsValidLocation(Board, 4))
            {
                Debug.Log("Col full");
                return;
            }

            int row = GetNextOpenRow(Board, 4);
            DropPiece(Board, row, 4, Player1Piece);
            var spawn = new Vector3((float) 275.25, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.4" + row).gameObject.SetActive(true);
            }

            Instantiate(Pions1, spawn, transform.rotation);
            isplayerturn = false;
            notyet = false;
        }
        else
        {
            if ((turn % 2 != 0 && PhotonNetwork.IsMasterClient)||(turn % 2 != 1 && (!PhotonNetwork.IsMasterClient)))
            {
                return;
            }
            if (!IsValidLocation(Board, 4))
            {
                Debug.Log("Col full");
                return;
            }

            char p = Player1Piece;
            GameObject r = Pions1;
            int row = GetNextOpenRow(Board, 4);
            if (!PhotonNetwork.IsMasterClient)
            {
                p = Player2Piece;
                r = Pions2;
            }
            
            DropPiece(Board, row, 4, p);
            var spawn = new Vector3((float) 275.25, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.4" + row).gameObject.SetActive(true);
            }

            PhotonNetwork.Instantiate(r.name, spawn, transform.rotation);
            photonView.RPC("AddTurn",RpcTarget.All);
            notyet = false;
            Debug.Log(Board[0][5]);
            ToggleButtonsOnline();
        }

    }
    
    public void DropCol5()
    {
        if (aienabled)
        {
            if (!isplayerturn)
            {
                return;
            }

            if (!IsValidLocation(Board, 5))
            {
                Debug.Log("Col full");
                return;
            }

            int row = GetNextOpenRow(Board, 5);
            DropPiece(Board, row, 5, Player1Piece);
            var spawn = new Vector3((float) 277.75, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.5" + row).gameObject.SetActive(true);
            }

            Instantiate(Pions1, spawn, transform.rotation);
            isplayerturn = false;
            notyet = false;
        }
        else
        {
            if ((turn % 2 != 0 && PhotonNetwork.IsMasterClient)||(turn % 2 != 1 && (!PhotonNetwork.IsMasterClient)))
            {
                return;
            }
            if (!IsValidLocation(Board, 5))
            {
                Debug.Log("Col full");
                return;
            }

            char p = Player1Piece;
            GameObject r = Pions1;
            int row = GetNextOpenRow(Board, 5);
            if (!PhotonNetwork.IsMasterClient)
            {
                p = Player2Piece;
                r = Pions2;
            }
            
            DropPiece(Board, row, 5, p);
            var spawn = new Vector3((float) 277.75, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.5" + row).gameObject.SetActive(true);
            }

            PhotonNetwork.Instantiate(r.name, spawn, transform.rotation);
            photonView.RPC("AddTurn",RpcTarget.All);
            notyet = false;
            Debug.Log(Board[0][5]);
            ToggleButtonsOnline();
        }

    }
    
    public void DropCol6()
    {
        if (aienabled)
        {

            if (!isplayerturn)
            {
                return;
            }

            if (!IsValidLocation(Board, 6))
            {
                Debug.Log("Col full");
                return;
            }

            int row = GetNextOpenRow(Board, 6);
            DropPiece(Board, row, 6, Player1Piece);
            var spawn = new Vector3((float) 280.25, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.6" + row).gameObject.SetActive(true);
            }

            Instantiate(Pions1, spawn, transform.rotation);
            isplayerturn = false;
            notyet = false;
        }
        else
        {
            if ((turn % 2 != 0 && PhotonNetwork.IsMasterClient)||(turn % 2 != 1 && (!PhotonNetwork.IsMasterClient)))
            {
                return;
            }
            if (!IsValidLocation(Board, 6))
            {
                Debug.Log("Col full");
                return;
            }

            char p = Player1Piece;
            GameObject r = Pions1;
            int row = GetNextOpenRow(Board, 6);
            if (!PhotonNetwork.IsMasterClient)
            {
                p = Player2Piece;
                r = Pions2;
            }
            
            DropPiece(Board, row, 6, p);
            var spawn = new Vector3((float) 280.25, 25, 120);
            if (row != 5)
            {
                Plateau.transform.Find("support.6" + row).gameObject.SetActive(true);
            }

            PhotonNetwork.Instantiate(r.name, spawn, transform.rotation);
            photonView.RPC("AddTurn",RpcTarget.All);
            notyet = false;
            Debug.Log(Board[0][5]);
            ToggleButtonsOnline();
        }
    }

    #endregion

    #region DropColsPlayer2

    public void DropCol0P2()
    {
        
        if (!IsValidLocation(Board,0))
        {
            Debug.Log("Col full");
            return;
        }

        int row = GetNextOpenRow(Board, 0);
        var spawn = new Vector3(265, 25, 120);
        if (row != 5)
        {
            Plateau.transform.Find("support.0" + row).gameObject.SetActive(true);
        }
        Instantiate(Pions2,spawn,transform.rotation);
        isplayerturn = true;
        Board[row][0] = '2';
        
    }
    
    public void DropCol1P2()
    {
        if (!IsValidLocation(Board,1))
        {
            Debug.Log("Col full");
            return;
        }

        int row = GetNextOpenRow(Board, 1);
        var spawn = new Vector3((float) 267.5, 25, 120);
        if (row != 5)
        {
            Plateau.transform.Find("support.1" + row).gameObject.SetActive(true);
        }
        Instantiate(Pions2,spawn,transform.rotation);
        isplayerturn = true;
        Board[row][1] = '2';
    }
    
    public void DropCol2P2()
    {
        if (!IsValidLocation(Board,2))
        {
            Debug.Log("Col full");
            return;
        }

        int row = GetNextOpenRow(Board, 2);
        var spawn = new Vector3((float) 270.25, 25, 120);
        if (row != 5)
        {
            Plateau.transform.Find("support.2" + row).gameObject.SetActive(true);
        }
        Instantiate(Pions2,spawn,transform.rotation);
        isplayerturn = true;
        Board[row][2] = '2';
    }
    
    public void DropCol3P2()
    {
        if (!IsValidLocation(Board,3))
        {
            Debug.Log("Col full");
            return;
        }

        int row = GetNextOpenRow(Board, 3);
        var spawn = new Vector3((float) 272.75, 25, 120);
        if (row != 5)
        {
            Plateau.transform.Find("support.3" + row).gameObject.SetActive(true);
        }
        Instantiate(Pions2,spawn,transform.rotation);
        isplayerturn = true;
        Board[row][3] = '2';
    }
    
    public void DropCol4P2()
    {
        
        if (!IsValidLocation(Board,4))
        {
            Debug.Log("Col full");
            return;
        }

        int row = GetNextOpenRow(Board, 4);
        var spawn = new Vector3((float) 275.25, 25, 120);
        if (row != 5)
        {
            Plateau.transform.Find("support.4" + row).gameObject.SetActive(true);
        }
        Instantiate(Pions2,spawn,transform.rotation);
        isplayerturn = true;
        Board[row][4] = '2';
    }
    
    public void DropCol5P2()
    {
        if (!IsValidLocation(Board,5))
        {
            Debug.Log("Col full");
            return;
        }

        int row = GetNextOpenRow(Board, 5);
        var spawn = new Vector3((float) 277.75, 25, 120);
        if (row != 5)
        {
            Plateau.transform.Find("support.5" + row).gameObject.SetActive(true);
        }
        Instantiate(Pions2,spawn,transform.rotation);
        isplayerturn = true;
        Board[row][5] = '2';
    }
    
    public void DropCol6P2()
    {
        
        if (!IsValidLocation(Board,6))
        {
            Debug.Log("Col full");
            return;
        }

        int row = GetNextOpenRow(Board, 6);
        var spawn = new Vector3((float) 280.25, 25, 120);
        if (row != 5)
        {
            Plateau.transform.Find("support.6" + row).gameObject.SetActive(true);
        }
        Instantiate(Pions2,spawn,transform.rotation);
        isplayerturn = true;
        Board[row][6] = '2';
    }

    #endregion

    #region Difficulty Chooser

    public void PussyAss()
    {
        AiDifficulty = 3;
        aienabled = true;
        gamestarted = true;
        TurnIndicator.gameObject.SetActive(true);
        DifficultyCanvas.gameObject.SetActive(false);
        
        for (int i = 0; i < 7; i++)
        {
            Buttons.transform.Find("Button_Col_" + i + "_").gameObject.SetActive(true);
        }
    }

    public void MediumAss()
    {
        AiDifficulty = 4;
        aienabled = true;
        gamestarted = true;
        TurnIndicator.gameObject.SetActive(true);
        DifficultyCanvas.gameObject.SetActive(false);
        
        for (int i = 0; i < 7; i++)
        {
            Buttons.transform.Find("Button_Col_" + i + "_").gameObject.SetActive(true);
        }
    }

    public void BadAss()
    {
        AiDifficulty = 5;
        aienabled = true;
        gamestarted = true;
        TurnIndicator.gameObject.SetActive(true);
        DifficultyCanvas.gameObject.SetActive(false);
        
        for (int i = 0; i < 7; i++)
        {
            Buttons.transform.Find("Button_Col_" + i + "_").gameObject.SetActive(true);
        }
    }

    #endregion

    #region Eog

    public void Exit()
    {
        
        Firebase.SaveDataButton("Puissance4");
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel(3);
        else SceneManager.LoadScene(1);
    }

    public void PlayAgain()
    {
        if (PhotonNetwork.CountOfPlayers==1)  SceneManager.LoadScene(2); 
        EndGameUi.SetActive(false);
        Setting.SetActive(true);
        YouLostUi.SetActive(false);
        YouWonUi.SetActive(false);
        DrawUi.SetActive(false);
        
        
        TurnIndicator.gameObject.SetActive(true);

        if(! PhotonNetwork.IsConnected)DifficultyCanvas.gameObject.SetActive(true);
        Board = setup();
        turn = 0;

        GameObject[] allpawns = GameObject.FindGameObjectsWithTag("Pion");
        foreach (var pawn in allpawns)
        {
            GameObject.Destroy(pawn);
        }

        isplayerturn = Random.Range(0,2) == 1;

        if (!isplayerturn)
        {
            notyet = false;
        }
        
        Text toChange = GameObject.Find("Canvas/Turn Indicator").GetComponent<Text>();
        if (isplayerturn)
        {
            toChange.text = "Your Turn !";
            toChange.color = new Color((248.0f / 255.0f), (144.0f / 255.0f), (231.0f / 255.0f));
        }
        else
        {
            toChange.text = "Wait ...";
            toChange.color = new Color((11.0f / 255.0f), (211.0f / 255.0f), (211.0f / 255.0f));
        }

        for (int col = 0; col < ColsCount; col++)
        {
            for (int row = 0; row < RowCount - 1; row++)
            {
                Plateau.transform.Find("support." + col + row).gameObject.SetActive(false);
            }
        }
        
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            Debug.Log("writing");
            //stream.SendNext(Board);
            stream.SendNext(turn);

            var a = "";
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColsCount; j++)
                {
                    a += " " + Board[i][j];
                    if (Board[i][j]=='.') stream.SendNext(0);
                    else if (Board[i][j] is Player1Piece) stream.SendNext(1);
                    else stream.SendNext(2);
                }
                a += '\n';
            }
            //stream.SendNext(support);
            Debug.Log("send"+ a);
        }
        else if (stream.IsReading)
        {
            Debug.Log("reading");
            //Board = (char[][]) stream.ReceiveNext();
            turn =(int) stream.ReceiveNext();
            var b= "";
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColsCount; j++)
                {
                    var a = stream.ReceiveNext();
                    b += " " + a;
                    if (a == (object) 1)
                    {
                        
                        //Support(i.ToString()[0],j.ToString()[0]);
                        Board[i][j] = Player1Piece;
                        
                    }
                    else if (a == (object) 2)
                    {
                        
                        //Support(i.ToString()[0],j.ToString()[0]);
                        Board[i][j] = Player2Piece;
                    }
                    else Board[i][j] = '.';
                }

                b += '\n';

            }
            Debug.Log("receive "+ b);
            //support = (string) stream.ReceiveNext();
            //if (support !=null)Plateau.transform.Find(support).gameObject.SetActive(true);
        }
        ToggleButtonsOnline();
        
    }

    public GameObject OptionsUI;
    public GameObject BTNChangeGame;
    public GameObject Setting;
    public void SettingScren()
    {
        OptionsUI.SetActive(true);
        if (PhotonNetwork.IsMasterClient) BTNChangeGame.SetActive(true);

    }
    
    public void SettingExit()
    {
        OptionsUI.SetActive(false);
        BTNChangeGame.SetActive(false);
    }

    public void ChangeGame()
    {
        PhotonNetwork.LoadLevel(3);
    }

    public void MainMenu()
    {
        Firebase.SignOutButton();
        PhotonNetwork.DestroyAll();
        SceneManager.LoadScene(1);

    }

    public Slider slider;

    public void SliderControll()
    {
        Debug.Log(Firebase.Audio);
        Firebase.Audio.volume = slider.value;
    }
}
