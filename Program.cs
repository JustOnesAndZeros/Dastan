// ReSharper disable All
#pragma warning disable CS8625

//Skeleton Program code for the AQA A Level Paper 1 Summer 2023 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Dastan
{
    class Program
    {
        static void Main(string[] args)
        {
            Dastan ThisGame = new Dastan(6, 6, 4); //creates new game with 6x6 board and 4 pieces
            ThisGame.PlayGame(); //start game
            Console.WriteLine("Goodbye!");
            Console.ReadLine(); //asks for user input so the program doesn't close until the user presses enter
        }
    }

    class Dastan
    {
        protected List<Square> Board;
        protected int NoOfRows, NoOfColumns, MoveOptionOfferPosition;
        protected List<Player> Players = new List<Player>();
        protected List<string> MoveOptionOffer = new List<string>();
        protected Player CurrentPlayer;
        protected Random RGen = new Random();

        public Dastan(int R, int C, int NoOfPieces)
        {
            //creates players and their direction on the board
            Players.Add(new Player("Player One", 1));
            Players.Add(new Player("Player Two", -1));
            CreateMoveOptions(); //creates the list of options for each player
            NoOfRows = R;
            NoOfColumns = C;
            MoveOptionOfferPosition = 0;
            CreateMoveOptionOffer(); //creates list of possible offers
            CreateBoard(); //creates baord
            CreatePieces(NoOfPieces);
            CurrentPlayer = Players[0];
        }

        private void DisplayBoard() //displays the board probably
        {
            Console.Write(Environment.NewLine + "   "); //prints new line
            for (int Column = 1; Column <= NoOfColumns; Column++) //iterates through the columns
            {
                Console.Write(Column.ToString() + "  "); //numbers the columns 1 2 3 4 5 6
            }
            Console.Write(Environment.NewLine + "  "); //new line
            for (int Count = 1; Count <= NoOfColumns; Count++) 
            {
                Console.Write("---"); //prints top line -------------------
            }
            Console.WriteLine("-"); //prints another - for the aesthetic
            for (int Row = 1; Row <= NoOfRows; Row++) //iterates through rows
            {
                Console.Write(Row.ToString() + " "); //prints row number 
                for (int Column = 1; Column <= NoOfColumns; Column++) //iterates through columns
                {
                    int Index = GetIndexOfSquare(Row * 10 + Column); //pass row and column as 2 digit number and get position in board list
                    Console.Write("|" + Board[Index].GetSymbol()); //print left square dividing line and the k/K symbol if there is one (space if not)
                    Piece PieceInSquare = Board[Index].GetPieceInSquare(); //gets the piece in that square
                    if (PieceInSquare == null)
                    {
                        Console.Write(" "); //if no piece then print blank space
                    }
                    else
                    {
                        Console.Write(PieceInSquare.GetSymbol()); //print the symbol of piece if there is one
                    }
                }
                Console.WriteLine("|"); //print right edge of board
            }
            Console.Write("  -"); //print offset for drawing bottom line on board
            for (int Column = 1; Column <= NoOfColumns; Column++)
            {
                Console.Write("---"); //draw line for each column
            }
            Console.WriteLine(); //create two new lines
            Console.WriteLine(); //create two new lines
        }

        private void DisplayState() //outputs game state to console
        {
            DisplayBoard(); //prints the current board state to the screen
            Console.WriteLine("Move option offer: " + MoveOptionOffer[MoveOptionOfferPosition]); //prints offer for this turn
            Console.WriteLine();
            Console.WriteLine(CurrentPlayer.GetPlayerStateAsString()); //print state of current player
            Console.WriteLine("Turn: " + CurrentPlayer.GetName()); //print which player's turn it is
            Console.WriteLine();
        }

        private int GetIndexOfSquare(int SquareReference) //converts 2 digit number RC into position in board list
        {
            int Row = SquareReference / 10; //gets first digit (row)
            int Col = SquareReference % 10; //gets second digit (column)
            return (Row - 1) * NoOfColumns + (Col - 1); //converts into list position
        }

        private bool CheckSquareInBounds(int SquareReference)
        {
            int Row = SquareReference / 10; 
            int Col = SquareReference % 10; 
            if (Row < 1 || Row > NoOfRows)
            {
                return false;
            }
            else if (Col < 1 || Col > NoOfColumns)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CheckSquareIsValid(int SquareReference, bool StartSquare)
        {
            if (!CheckSquareInBounds(SquareReference))
            {
                return false;
            }
            Piece PieceInSquare = Board[GetIndexOfSquare(SquareReference)].GetPieceInSquare();
            if (PieceInSquare == null)
            {
                if (StartSquare)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()))
            {
                if (StartSquare)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (StartSquare)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private bool CheckIfGameOver()
        {
            bool Player1HasMirza = false;
            bool Player2HasMirza = false;
            foreach (var S in Board)
            {
                Piece PieceInSquare = S.GetPieceInSquare();
                if (PieceInSquare != null)
                {
                    if (S.ContainsKotla() && PieceInSquare.GetTypeOfPiece() == "mirza" && !PieceInSquare.GetBelongsTo().SameAs(S.GetBelongsTo()))
                    {
                        return true;
                    }
                    else if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[0]))
                    {
                        Player1HasMirza = true;
                    }
                    else if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[1]))
                    {
                        Player2HasMirza = true;
                    }
                }
            }
            return !(Player1HasMirza && Player2HasMirza);
        }

        private int GetSquareReference(string Description)
        {
            int SelectedSquare;
            Console.Write("Enter the square " + Description + " (row number followed by column number): ");
            SelectedSquare = Convert.ToInt32(Console.ReadLine());
            return SelectedSquare;
        }

        private void UseMoveOptionOffer()
        {
            int ReplaceChoice;
            Console.Write("Choose the move option from your queue to replace (1 to 5): ");
            ReplaceChoice = Convert.ToInt32(Console.ReadLine()); //input which move the user should replace
            //ReplaceChoice - 1 is the index of the choice being replaced
            //
            CurrentPlayer.UpdateMoveOptionQueueWithOffer(ReplaceChoice - 1, CreateMoveOption(MoveOptionOffer[MoveOptionOfferPosition], CurrentPlayer.GetDirection()));
            CurrentPlayer.ChangeScore(-(10 - (ReplaceChoice * 2)));
            MoveOptionOfferPosition = RGen.Next(0, 5);
        }

        private int GetPointsForOccupancyByPlayer(Player CurrentPlayer)
        {
            int ScoreAdjustment = 0;
            foreach (var S in Board)
            {
                ScoreAdjustment += (S.GetPointsForOccupancy(CurrentPlayer));
            }
            return ScoreAdjustment;
        }

        private void UpdatePlayerScore(int PointsForPieceCapture)
        {
            CurrentPlayer.ChangeScore(GetPointsForOccupancyByPlayer(CurrentPlayer) + PointsForPieceCapture);
        }

        private int CalculatePieceCapturePoints(int FinishSquareReference)
        {
            if (Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare() != null)
            {
                return Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare().GetPointsIfCaptured();
            }
            return 0;
        }

        public void PlayGame() //runs at start of game
        {
            //loops until GameOver == true
            bool GameOver = false;
            while (!GameOver)
            {
                DisplayState(); //display game's current state
                bool SquareIsValid = false;
                int Choice;
                do
                {
                    Console.Write("Choose move option to use from queue (1 to 3) or 9 to take the offer: ");
                    Choice = Convert.ToInt32(Console.ReadLine()); //input number
                    if (Choice == 9) //if player chooses to take the offer
                    {
                        UseMoveOptionOffer();
                        DisplayState();
                    }
                }
                while (Choice < 1 || Choice > 3);
                bool MoveLegal;
                do
                {
                    int StartSquareReference = 0;
                    while (!SquareIsValid)
                    {
                        StartSquareReference = GetSquareReference("containing the piece to move");
                        SquareIsValid = CheckSquareIsValid(StartSquareReference, true);
                    }
                    int FinishSquareReference = 0;
                    SquareIsValid = false;
                    while (!SquareIsValid)
                    {
                        FinishSquareReference = GetSquareReference("to move to");
                        SquareIsValid = CheckSquareIsValid(FinishSquareReference, false);
                    }
                    SquareIsValid = false; //
                    MoveLegal = CurrentPlayer.CheckPlayerMove(Choice, StartSquareReference, FinishSquareReference);
                    if (MoveLegal)
                    {
                        int PointsForPieceCapture = CalculatePieceCapturePoints(FinishSquareReference);
                        CurrentPlayer.ChangeScore(-(Choice + (2 * (Choice - 1))));
                        CurrentPlayer.UpdateQueueAfterMove(Choice);
                        UpdateBoard(StartSquareReference, FinishSquareReference);
                        UpdatePlayerScore(PointsForPieceCapture);
                        Console.WriteLine("New score: " + CurrentPlayer.GetScore() + Environment.NewLine);
                    }
                    else
                    {
                        Console.WriteLine("Illegal move");
                    }
                }
                while (!MoveLegal);
                if (CurrentPlayer.SameAs(Players[0]))
                {
                    CurrentPlayer = Players[1];
                }
                else
                {
                    CurrentPlayer = Players[0];
                }
                GameOver = CheckIfGameOver();
            }
            DisplayState();
            DisplayFinalResult();
        }

        private void UpdateBoard(int StartSquareReference, int FinishSquareReference)
        {
            Board[GetIndexOfSquare(FinishSquareReference)].SetPiece(Board[GetIndexOfSquare(StartSquareReference)].RemovePiece());
        }

        private void DisplayFinalResult()
        {
            if (Players[0].GetScore() == Players[1].GetScore())
            {
                Console.WriteLine("Draw!");
            }
            else if (Players[0].GetScore() > Players[1].GetScore())
            {
                Console.WriteLine(Players[0].GetName() + " is the winner!");
            }
            else
            {
                Console.WriteLine(Players[1].GetName() + " is the winner!");
            }
        }

        private void CreateBoard()
        {
            Square S;
            Board = new List<Square>(); //board is a 1D list of squares
            for (int Row = 1; Row <= NoOfRows; Row++) //iterates through rows
            {
                for (int Column = 1; Column <= NoOfColumns; Column++) //iterates through columns
                {
                    if (Row == 1 && Column == NoOfColumns / 2)
                    {
                        S = new Kotla(Players[0], "K"); //places p1 kotla in the middle of the top row
                    }
                    else if (Row == NoOfRows && Column == NoOfColumns / 2 + 1)
                    {
                        S = new Kotla(Players[1], "k"); //places p2 kotla in the middle of the bottom row
                    }
                    else
                    {
                        S = new Square(); //creates empty square everywhere else
                    }
                    Board.Add(S);
                }
            }
        }

        private void CreatePieces(int NoOfPieces)
        {
            Piece CurrentPiece;
            for (int Count = 1; Count <= NoOfPieces; Count++)
            {
                CurrentPiece = new Piece("piece", Players[0], 1, "!"); //creates pieces for player 1 with ! symbol
                Board[GetIndexOfSquare(2 * 10 + Count + 1)].SetPiece(CurrentPiece); //sets pieces on the second row from the top to the current peice
            }
            CurrentPiece = new Piece("mirza", Players[0], 5, "1"); //create player 1 mirza with symbol 1
            Board[GetIndexOfSquare(10 + NoOfColumns / 2)].SetPiece(CurrentPiece); //sets mirza to middle of the top row
            for (int Count = 1; Count <= NoOfPieces; Count++)
            {
                CurrentPiece = new Piece("piece", Players[1], 1, "\""); //creates pieces for player 2 with " symbol
                Board[GetIndexOfSquare((NoOfRows - 1) * 10 + Count + 1)].SetPiece(CurrentPiece); //sets pieces on the second row from the bottom to the current peice
            }
            CurrentPiece = new Piece("mirza", Players[1], 5, "2"); //create player 2 mirza with symbol 2
            Board[GetIndexOfSquare(NoOfRows * 10 + (NoOfColumns / 2 + 1))].SetPiece(CurrentPiece); //sets mirza to middle of the bottom row
        }

        private void CreateMoveOptionOffer()
        {
            //adds all 5 offers to move option offer list
            MoveOptionOffer.Add("jazair");
            MoveOptionOffer.Add("chowkidar");
            MoveOptionOffer.Add("cuirassier");
            MoveOptionOffer.Add("ryott");
            MoveOptionOffer.Add("faujdar");
        }

        private MoveOption CreateRyottMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("ryott"); //create move option called ryott
            //add all possible moves for this option to list, column is multiplied by direction because player 1 and player 2 move in opposite directions
            Move NewMove = new Move(0, 1 * Direction); //1 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -1 * Direction); //1 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, 0); //1 forward
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, 0); //1 backwards
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateFaujdarMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("faujdar");
            Move NewMove = new Move(0, -1 * Direction); //1 backwards
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 1 * Direction); //1 forward
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 2 * Direction); //2 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -2 * Direction); //2 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateJazairMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("jazair");
            Move NewMove = new Move(2 * Direction, 0); //2 forward
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, -2 * Direction); //2 forward, 2 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, 2 * Direction); //2 forward, 2 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 2 * Direction); //2 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -2 * Direction); //2 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, -1 * Direction); //1 backwards, 1 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, 1 * Direction); // 1 backwards, 1 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateCuirassierMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("cuirassier");
            Move NewMove = new Move(1 * Direction, 0); //1 forward
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, 0); //2 forward
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, -2 * Direction); //1 forward, 2 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, 2 * Direction); //1 forward, 2 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateChowkidarMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("chowkidar");
            Move NewMove = new Move(1 * Direction, 1 * Direction); //1 forward, 1 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, -1 * Direction); //1 forward, 1 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, 1 * Direction); //1 backwards, 1 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, -1 * Direction); //1 backwards, 1 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 2 * Direction); //2 right
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -2 * Direction); //2 left
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateMoveOption(string Name, int Direction)
        {
            if (Name == "chowkidar")
            {
                return CreateChowkidarMoveOption(Direction);
            }
            else if (Name == "ryott")
            {
                return CreateRyottMoveOption(Direction);
            }
            else if (Name == "faujdar")
            {
                return CreateFaujdarMoveOption(Direction);
            }
            else if (Name == "jazair")
            {
                return CreateJazairMoveOption(Direction);
            }
            else
            {
                return CreateCuirassierMoveOption(Direction);
            }
        }

        private void CreateMoveOptions()
        {
            Players[0].AddToMoveOptionQueue(CreateMoveOption("ryott", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("chowkidar", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("cuirassier", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("faujdar", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("jazair", 1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("ryott", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("chowkidar", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("jazair", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("faujdar", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("cuirassier", -1));
        }
    }

    class Piece
    {
        protected string TypeOfPiece, Symbol;
        protected int PointsIfCaptured;
        protected Player BelongsTo;

        public Piece(string T, Player B, int P, string S)
        {
            TypeOfPiece = T;
            BelongsTo = B;
            PointsIfCaptured = P;
            Symbol = S;
        }

        public string GetSymbol()
        {
            return Symbol;
        }

        public string GetTypeOfPiece()
        {
            return TypeOfPiece;
        }

        public Player GetBelongsTo()
        {
            return BelongsTo;
        }

        public int GetPointsIfCaptured()
        {
            return PointsIfCaptured;
        }
    }

    class Square
    {
        protected string Symbol;
        protected Piece PieceInSquare;
        protected Player BelongsTo;

        public Square()
        {
            PieceInSquare = null;
            BelongsTo = null;
            Symbol = " ";
        }

        public virtual void SetPiece(Piece P)
        {
            PieceInSquare = P;
        }

        public virtual Piece RemovePiece()
        {
            Piece PieceToReturn = PieceInSquare;
            PieceInSquare = null;
            return PieceToReturn;
        }

        public virtual Piece GetPieceInSquare()
        {
            return PieceInSquare;
        }

        public virtual string GetSymbol()
        {
            return Symbol;
        }

        public virtual int GetPointsForOccupancy(Player CurrentPlayer)
        {
            return 0;
        }

        public virtual Player GetBelongsTo()
        {
            return BelongsTo;
        }

        public virtual bool ContainsKotla()
        {
            if (Symbol == "K" || Symbol == "k")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class Kotla : Square
    {
        public Kotla(Player P, string S) : base()
        {
            BelongsTo = P;
            Symbol = S;
        }

        public override int GetPointsForOccupancy(Player CurrentPlayer)
        {
            if (PieceInSquare == null)
            {
                return 0;
            }
            else if (BelongsTo.SameAs(CurrentPlayer))
            {
                if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) && (PieceInSquare.GetTypeOfPiece() == "piece" || PieceInSquare.GetTypeOfPiece() == "mirza"))
                {
                    return 5;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) && (PieceInSquare.GetTypeOfPiece() == "piece" || PieceInSquare.GetTypeOfPiece() == "mirza"))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    class MoveOption
    {
        protected string Name;
        protected List<Move> PossibleMoves;

        public MoveOption(string N)
        {
            Name = N;
            PossibleMoves = new List<Move>();
        }

        public void AddToPossibleMoves(Move M)
        {
            PossibleMoves.Add(M);
        }

        public string GetName()
        {
            return Name;
        }

        public bool CheckIfThereIsAMoveToSquare(int StartSquareReference, int FinishSquareReference)
        {
            //get start/finish coords
            int StartRow = StartSquareReference / 10;
            int StartColumn = StartSquareReference % 10;
            int FinishRow = FinishSquareReference / 10;
            int FinishColumn = FinishSquareReference % 10;
            foreach (var M in PossibleMoves)
            {
                if (StartRow + M.GetRowChange() == FinishRow && StartColumn + M.GetColumnChange() == FinishColumn) //check 
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Move
    {
        protected int RowChange, ColumnChange;

        public Move(int R, int C)
        {
            RowChange = R;
            ColumnChange = C;
        }

        public int GetRowChange()
        {
            return RowChange;
        }

        public int GetColumnChange()
        {
            return ColumnChange;
        }
    }

    class MoveOptionQueue
    {
        private List<MoveOption> Queue = new List<MoveOption>();

        public string GetQueueAsString()
        {
            string QueueAsString = "";
            int Count = 1;
            foreach (var M in Queue)
            {
                QueueAsString += Count.ToString() + ". " + M.GetName() + "   ";
                Count += 1;
            }
            return QueueAsString;
        }

        public void Add(MoveOption NewMoveOption)
        {
            Queue.Add(NewMoveOption);
        }

        public void Replace(int Position, MoveOption NewMoveOption)
        {
            Queue[Position] = NewMoveOption;
        }

        public void MoveItemToBack(int Position)
        {
            MoveOption Temp = Queue[Position];
            Queue.RemoveAt(Position);
            Queue.Add(Temp);
        }

        public MoveOption GetMoveOptionInPosition(int Pos)
        {
            return Queue[Pos];
        }
    }

    class Player
    {
        private string Name;
        private int Direction, Score;
        private MoveOptionQueue Queue = new MoveOptionQueue();

        public Player(string N, int D)
        {
            Score = 100;
            Name = N;
            Direction = D;
        }

        public bool SameAs(Player APlayer) //check if this player is the same as the passed in player
        {
            if (APlayer == null)
            {
                return false;
            }
            else if (APlayer.GetName() == Name) //if the players have the same name then they are the same
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetPlayerStateAsString() //returns a string displaying the name, score, move queue
        {
            return Name + Environment.NewLine + "Score: " + Score.ToString() + Environment.NewLine + "Move option queue: " + Queue.GetQueueAsString() + Environment.NewLine;
        }

        public void AddToMoveOptionQueue(MoveOption NewMoveOption)
        {
            Queue.Add(NewMoveOption);
        }

        public void UpdateQueueAfterMove(int Position)
        {
            Queue.MoveItemToBack(Position - 1);
        }

        public void UpdateMoveOptionQueueWithOffer(int Position, MoveOption NewMoveOption)
        {
            Queue.Replace(Position, NewMoveOption);
        }

        public int GetScore()
        {
            return Score;
        }

        public string GetName()
        {
            return Name;
        }

        public int GetDirection()
        {
            return Direction;
        }

        public void ChangeScore(int Amount)
        {
            Score += Amount;
        }

        public bool CheckPlayerMove(int Pos, int StartSquareReference, int FinishSquareReference)
        {
            MoveOption Temp = Queue.GetMoveOptionInPosition(Pos - 1); //get selected move option
            return Temp.CheckIfThereIsAMoveToSquare(StartSquareReference, FinishSquareReference); //
        }
    }
}