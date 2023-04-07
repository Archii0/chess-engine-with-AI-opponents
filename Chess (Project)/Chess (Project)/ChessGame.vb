Public Class ChessGame
    Private ReadOnly ChessBoard As New Board
    Private Go As Boolean
    Private mouseLocked As Boolean = False
    Private ReadOnly Settings As New GameSettings
    Private selectedPawn As Pawn
    Private selectedKnight As Knight
    Private selectedBishop As Bishop
    Private selectedRook As Rook
    Private selectedQueen As Queen
    Private selectedKing As King
    Private pieceSelected As Boolean

    Private ReadOnly lblPromotionTitle As New Label
    Private ReadOnly picQueenPromotion As New PictureBox
    Private ReadOnly picRookPromotion As New PictureBox
    Private ReadOnly picBishopPromotion As New PictureBox
    Private ReadOnly picKnightPromotion As New PictureBox

    Private ReadOnly playerTurnLocked As Boolean = False


#Region "Start Form Properties"
    Public WriteOnly Property SetWhitePlayer As Integer
        Set(value As Integer)
            Settings.SetWhitePlayerType(value)
        End Set
    End Property
    Public WriteOnly Property SetBlackPlayer As Integer
        Set(value As Integer)
            Settings.SetBlackPlayerType(value)
        End Set
    End Property
    Public WriteOnly Property SetAIDelay1 As Integer
        Set(value As Integer)
            Settings.SetAI1Delay(value)
        End Set
    End Property
    Public WriteOnly Property SetAIDelay2 As Integer
        Set(value As Integer)
            Settings.SetAI2Delay(value)
        End Set
    End Property
    Public WriteOnly Property SetCustomFen As String
        Set(value As String)
            Settings.SetStartFEN(value)
        End Set
    End Property
    Public WriteOnly Property SetMoveTest As Boolean
        Set(value As Boolean)
            Settings.SetMoveTest(value)
        End Set
    End Property
#End Region
#Region "Simple Mutator and Accessor Methods"
    'Settings Object
    Function GetSettings()
        Return Settings
    End Function
#End Region

#Region "Mouse Control Events"
    Private Sub ChessAI_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If mouseLocked = False Then
            Go = True

            If pieceSelected = False Then                   'If no piece is selected, the program will check if a new piece is being selected

                For Each square In ChessBoard.GetBoardSquares       'Loops through all board squares
                    Dim rectangleFind As Rectangle = square.GetRectangle()

                    If rectangleFind.Contains(e.X, e.Y) Then                'If the rectangle contains the mouse position
                        Static PieceColourToColour As New Dictionary(Of Integer, String) From {     'A dictionary for displaying the piece colour in a label
                            {Piece.White, "White"},
                            {Piece.Black, "Black"}
                        }

                        If square.GetPieceType() <> Nothing And square.GetPieceColour() = Settings.GetColourToMove() Then       'Checks if the square contains a friendly piece
                            pieceSelected = True                'If it does, the piece is selected
                            Select Case square.GetPieceType()
                                Case Piece.Pawn
                                    Dim pawns = ChessBoard.GetPawns()
                                    selectedPawn = pawns(square.GetPieceIndex)      'Saves the selected pawn so that it can be moved by the user

                                    If pawns(square.GetPieceIndex).GetPromotionTag() = 0 Then               'Updates the label to show which piece is selected
                                        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(pawns(square.GetPieceIndex).GetPieceColour()) + " Pawn"
                                    ElseIf pawns(square.GetPieceIndex).GetPromotionTag() = Piece.Queen Then
                                        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(pawns(square.GetPieceIndex).GetPieceColour()) + " Queen"
                                    ElseIf pawns(square.GetPieceIndex).GetPromotionTag() = Piece.Rook Then
                                        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(pawns(square.GetPieceIndex).GetPieceColour()) + " Rook"
                                    ElseIf pawns(square.GetPieceIndex).GetPromotionTag() = Piece.Bishop Then
                                        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(pawns(square.GetPieceIndex).GetPieceColour()) + " Bishop"
                                    ElseIf pawns(square.GetPieceIndex).GetPromotionTag() = Piece.Knight Then
                                        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(pawns(square.GetPieceIndex).GetPieceColour()) + " Knight"
                                    End If

                                Case Piece.Knight           'These cases follow the same format as above, saving the selected piece and updating the selected piece label
                                    Dim knights = ChessBoard.GetKnights()
                                    selectedKnight = knights(square.GetPieceIndex)
                                    lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(knights(square.GetPieceIndex).GetPieceColour()) + " Knight"
                                Case Piece.Bishop
                                    Dim bishops = ChessBoard.GetBishops()
                                    selectedBishop = bishops(square.GetPieceIndex)
                                    lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(bishops(square.GetPieceIndex).GetPieceColour()) + " Bishop"
                                Case Piece.Rook
                                    Dim rooks = ChessBoard.GetRooks()
                                    selectedRook = rooks(square.GetPieceIndex)
                                    lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(rooks(square.GetPieceIndex).GetPieceColour()) + " Rook"
                                Case Piece.Queen
                                    Dim queens = ChessBoard.GetQueens()
                                    selectedQueen = queens(square.GetPieceIndex)
                                    lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(queens(square.GetPieceIndex).GetPieceColour()) + " Queen"
                                Case Piece.King
                                    Dim kings = ChessBoard.GetKings()
                                    selectedKing = kings(square.GetPieceIndex)
                                    lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + PieceColourToColour.Item(kings(square.GetPieceIndex).GetPieceColour()) + " King"
                            End Select
                        Else                'If no piece is selected, the label is updated to show this
                            lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + "---"
                        End If
                    End If
                Next
            End If
        End If
    End Sub
    Private Sub ChessAI_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove

        If mouseLocked = False Then
            If Go = True Then                      'Checks that the mouse has been clicked down
                If pieceSelected = True Then            'If a piece is selected
                    Cursor = Cursors.Cross              'The cursor changes to a crosshair

                    Dim XPoint As Integer = e.X - 34        'The mouse position is calculated (minus the width and
                    Dim YPoint As Integer = e.Y - 34        'length of the piece size, so that the piece is centred on the mouse)
                    If selectedPawn IsNot Nothing Then                                  'Selects the correct piece type, and updates the location 
                        selectedPawn.pieceRectangle.Location = New Point(XPoint, YPoint)        'to follow the mouse
                    ElseIf selectedKnight IsNot Nothing Then
                        selectedKnight.pieceRectangle.Location = New Point(XPoint, YPoint)
                    ElseIf selectedBishop IsNot Nothing Then
                        selectedBishop.pieceRectangle.Location = New Point(XPoint, YPoint)
                    ElseIf selectedRook IsNot Nothing Then
                        selectedRook.pieceRectangle.Location = New Point(XPoint, YPoint)
                    ElseIf selectedQueen IsNot Nothing Then
                        selectedQueen.pieceRectangle.Location = New Point(XPoint, YPoint)
                    ElseIf selectedKing IsNot Nothing Then
                        selectedKing.pieceRectangle.Location = New Point(XPoint, YPoint)
                    End If
                End If
                Me.Refresh()                    'Redraws the piece on the board
            End If
        End If
    End Sub
    Private Sub ChessAI_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        If e.X < 0 Or e.X > 720 Or e.Y < 0 Or e.Y > 720 Then       'Checks that the mouse is within the range of the board 

            If selectedPawn IsNot Nothing Then                      'If a piece is selected, the piece will be moved back to its original square
                selectedPawn.pieceRectangle.Location = UnmakePlayerMove(selectedPawn)
                selectedPawn = Nothing
            ElseIf selectedKnight IsNot Nothing Then
                selectedKnight.pieceRectangle.Location = UnmakePlayerMove(selectedKnight)
                selectedKnight = Nothing
            ElseIf selectedBishop IsNot Nothing Then
                selectedBishop.pieceRectangle.Location = UnmakePlayerMove(selectedBishop)
                selectedBishop = Nothing
            ElseIf selectedRook IsNot Nothing Then
                selectedRook.pieceRectangle.Location = UnmakePlayerMove(selectedRook)
                selectedRook = Nothing
            ElseIf selectedQueen IsNot Nothing Then
                selectedQueen.pieceRectangle.Location = UnmakePlayerMove(selectedQueen)

                selectedQueen = Nothing
            ElseIf selectedKing IsNot Nothing Then
                selectedKing.pieceRectangle.Location = UnmakePlayerMove(selectedKing)
                selectedKing = Nothing
            End If
            pieceSelected = False                   'Resets the appropriate piece selection variables
            Go = False
            lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + "---"
            Cursor = Cursors.Default                'Resets the cursor
            RefreshBoard()

        ElseIf mouseLocked = False Then                 'Runs if the mouse is within the board range
            lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + "---"
            ChessBoard.SetPromotingPawn(Nothing)
            Dim moveMade As Boolean = False
            Go = False

            Cursor = Cursors.Default
            Dim mousePosX As Integer = e.X                  'Gets the mouse position
            Dim mousePosY As Integer = e.Y

            If pieceSelected = True Then
                Dim legalMovement As Boolean
                Dim isCapture As Boolean
                If selectedPawn IsNot Nothing Then              'Selects the relevant piece type
                    Dim temporaryMove As New Board.Movement
                    With temporaryMove                          'Declares a move structure, based on the mouse position
                        .StartSquare = selectedPawn.GetSquareNumber()
                        .TargetSquare = ((7 - Math.Floor(mousePosY / 90)) * 8) + (Math.Floor(mousePosX / 90))
                    End With

                    If selectedPawn.GetPromotionTag() = 0 Then                  'Check if the move is valid
                        legalMovement = ChessBoard.ValidatePawnMove(selectedPawn, temporaryMove)      'This validates normal pawn moves
                    Else
                        legalMovement = ChessBoard.ValidatePromotedPawnMove(selectedPawn, temporaryMove)       'This validates promoted pawn moves
                    End If
                    If legalMovement = False Then               'If the move is not valid, the piece is moved back to its original square
                        selectedPawn.pieceRectangle.Location = UnmakePlayerMove(selectedPawn)
                    Else
                        isCapture = ChessBoard.CheckIfCapture(temporaryMove)            'Checks if the move is a capture
                        Dim disposePiece As Boolean = False
                        If isCapture = True Then
                            ChessBoard.DisposeCapturedPiece(temporaryMove)          'Disposes the piece on the target square
                            disposePiece = True
                        ElseIf ChessBoard.GetEnpassentMove = True Then
                            ChessBoard.DisposeEnPassentPawn()                       'Disposes the en passent captured piece
                            disposePiece = True
                        End If

                        ChessBoard.MakeMove(temporaryMove, disposePiece)                'Plays the move
                        ChessBoard.FillEmptyBitboards()                                 'Updates the bitboard
                        ChessBoard.GenerateBitboards()

                        If ChessBoard.EvaluateGameState(Settings.GetColourToMove()) = True Then     'If the move puts the user in check

                            ChessBoard.UnmakeMove(temporaryMove)        'The move is unmade
                            If ChessBoard.GetEnpassentPawn IsNot Nothing Then
                                If ChessBoard.GetEnpassentPawn.GetPieceColour = Settings.GetColourToMove() Then
                                    ChessBoard.SetEnpassentPawn(Nothing)             'Resets en passent pawn if it is the colour to move
                                End If
                            End If
                            If isCapture = True Or ChessBoard.GetEnpassentMove = True Then      'Undisposes the captured piece
                                ChessBoard.UndisposeCapturedPiece()
                            End If
                            ChessBoard.FillEmptyBitboards()             'Updates the bitboards
                            ChessBoard.GenerateBitboards()
                        Else
                            moveMade = True
                            If ChessBoard.GetEnpassentPawn IsNot Nothing Then
                                If ChessBoard.GetEnpassentPawn.GetPieceColour() <> Settings.GetColourToMove() Then
                                    ChessBoard.SetEnpassentPawn(Nothing)            'Resets en passent pawn for the move before
                                End If
                            End If
                            'Deals with pawn promotion if a pawn gets to the opposite side of the board
                            If selectedPawn.GetPieceColour() = Piece.Black And selectedPawn.GetRank() = 0 And selectedPawn.GetPromotionTag() = 0 Then
                                ChessBoard.SetPromotingPawn(selectedPawn)
                                Settings.LockPlayerTurn()
                                DisplayPawnPromotion(Piece.Black)
                            ElseIf selectedPawn.GetPieceColour() = Piece.White And selectedPawn.GetRank() = 7 And selectedPawn.GetPromotionTag() = 0 Then
                                ChessBoard.SetPromotingPawn(selectedPawn)
                                Settings.LockPlayerTurn()
                                DisplayPawnPromotion(Piece.White)
                            End If
                            Settings.IncrementPlayerTurn()
                            selectedPawn.UpdateFirstMoveTaken()
                        End If
                    End If
                    selectedPawn = Nothing                  'Resets important variables
                    ChessBoard.SetEnpassentMove(False)

                ElseIf selectedKnight IsNot Nothing Then                    'The below cases follow the same general structure as above
                    Dim temporaryMove As New Board.Movement
                    With temporaryMove
                        .StartSquare = selectedKnight.GetSquareNumber()
                        .TargetSquare = ((7 - Math.Floor(mousePosY / 90)) * 8) + (Math.Floor(mousePosX / 90))
                    End With

                    legalMovement = ChessBoard.ValidateOtherMove(temporaryMove)

                    If legalMovement = False Then
                        selectedKnight.pieceRectangle.Location = UnmakePlayerMove(selectedKnight)
                    Else
                        isCapture = ChessBoard.CheckIfCapture(temporaryMove)
                        If isCapture = True Then
                            ChessBoard.DisposeCapturedPiece(temporaryMove)
                        End If
                        ChessBoard.MakeMove(temporaryMove, isCapture)
                        ChessBoard.FillEmptyBitboards()
                        ChessBoard.GenerateBitboards()

                        If ChessBoard.EvaluateGameState(Settings.GetColourToMove()) = True Then
                            ChessBoard.UnmakeMove(temporaryMove)
                            If isCapture = True Then
                                ChessBoard.UndisposeCapturedPiece()
                            End If
                            ChessBoard.FillEmptyBitboards()
                            ChessBoard.GenerateBitboards()
                        Else
                            moveMade = True
                            Settings.IncrementPlayerTurn()

                        End If
                    End If
                    selectedKnight = Nothing

                ElseIf selectedBishop IsNot Nothing Then
                    Dim temporaryMove As New Board.Movement
                    With temporaryMove
                        .StartSquare = selectedBishop.GetSquareNumber()
                        .TargetSquare = ((7 - Math.Floor(mousePosY / 90)) * 8) + (Math.Floor(mousePosX / 90))
                    End With

                    legalMovement = ChessBoard.ValidateSlidingMove(temporaryMove)
                    If legalMovement = False Then
                        selectedBishop.pieceRectangle.Location = UnmakePlayerMove(selectedBishop)
                    Else
                        isCapture = ChessBoard.CheckIfCapture(temporaryMove)
                        If isCapture = True Then
                            ChessBoard.DisposeCapturedPiece(temporaryMove)
                        End If
                        ChessBoard.MakeMove(temporaryMove, isCapture)
                        ChessBoard.FillEmptyBitboards()
                        ChessBoard.GenerateBitboards()

                        If ChessBoard.EvaluateGameState(Settings.GetColourToMove()) = True Then
                            ChessBoard.UnmakeMove(temporaryMove)
                            If isCapture = True Then
                                ChessBoard.UndisposeCapturedPiece()
                            End If
                            ChessBoard.FillEmptyBitboards()
                            ChessBoard.GenerateBitboards()
                        Else
                            moveMade = True
                            Settings.IncrementPlayerTurn()

                        End If
                    End If
                    selectedBishop = Nothing

                ElseIf selectedRook IsNot Nothing Then
                    Dim temporaryMove As New Board.Movement
                    With temporaryMove
                        .StartSquare = selectedRook.GetSquareNumber()
                        .TargetSquare = ((7 - Math.Floor(mousePosY / 90)) * 8) + (Math.Floor(mousePosX / 90))
                    End With

                    legalMovement = ChessBoard.ValidateSlidingMove(temporaryMove)

                    If legalMovement = False Then
                        selectedRook.pieceRectangle.Location = UnmakePlayerMove(selectedRook)
                    Else
                        isCapture = ChessBoard.CheckIfCapture(temporaryMove)
                        If isCapture = True Then
                            ChessBoard.DisposeCapturedPiece(temporaryMove)
                        End If

                        ChessBoard.MakeMove(temporaryMove, isCapture)
                        ChessBoard.FillEmptyBitboards()
                        ChessBoard.GenerateBitboards()

                        If ChessBoard.EvaluateGameState(Settings.GetColourToMove()) = True Then
                            ChessBoard.UnmakeMove(temporaryMove)
                            If isCapture = True Then
                                ChessBoard.UndisposeCapturedPiece()
                            End If
                            ChessBoard.FillEmptyBitboards()
                            ChessBoard.GenerateBitboards()
                        Else
                            moveMade = True
                            Settings.IncrementPlayerTurn()
                            If selectedRook.GetFirstMoveTaken() = False Then
                                selectedRook.UpdateFirstMoveTaken()
                            End If
                        End If
                    End If
                    selectedRook = Nothing

                ElseIf selectedQueen IsNot Nothing Then

                    Dim temporaryMove As New Board.Movement
                    With temporaryMove
                        .StartSquare = selectedQueen.GetSquareNumber()
                        .TargetSquare = ((7 - Math.Floor(mousePosY / 90)) * 8) + (Math.Floor(mousePosX / 90))
                    End With

                    legalMovement = ChessBoard.ValidateSlidingMove(temporaryMove)

                    If legalMovement = False Then
                        selectedQueen.pieceRectangle.Location = UnmakePlayerMove(selectedQueen)
                    Else
                        isCapture = ChessBoard.CheckIfCapture(temporaryMove)
                        If isCapture = True Then
                            ChessBoard.DisposeCapturedPiece(temporaryMove)
                        End If
                        ChessBoard.MakeMove(temporaryMove, isCapture)
                        ChessBoard.FillEmptyBitboards()
                        ChessBoard.GenerateBitboards()

                        If ChessBoard.EvaluateGameState(Settings.GetColourToMove()) = True Then
                            ChessBoard.UnmakeMove(temporaryMove)
                            If isCapture = True Then
                                ChessBoard.UndisposeCapturedPiece()
                            End If
                            ChessBoard.FillEmptyBitboards()
                            ChessBoard.GenerateBitboards()
                        Else
                            moveMade = True
                            Settings.IncrementPlayerTurn()
                        End If
                    End If

                    selectedQueen = Nothing

                ElseIf selectedKing IsNot Nothing Then

                    Dim temporaryMove As New Board.Movement
                    With temporaryMove
                        .StartSquare = selectedKing.GetSquareNumber()
                        .TargetSquare = ((7 - Math.Floor(mousePosY / 90)) * 8) + (Math.Floor(mousePosX / 90))
                    End With

                    legalMovement = ChessBoard.ValidateOtherMove(temporaryMove)

                    If legalMovement = False Then
                        selectedKing.pieceRectangle.Location = UnmakePlayerMove(selectedKing)
                    Else
                        isCapture = ChessBoard.CheckIfCapture(temporaryMove)
                        If isCapture = True Then
                            ChessBoard.DisposeCapturedPiece(temporaryMove)
                        End If

                        ChessBoard.MakeMove(temporaryMove, isCapture)
                        ChessBoard.FillEmptyBitboards()
                        ChessBoard.GenerateBitboards()
                        If ChessBoard.EvaluateGameState(Settings.GetColourToMove()) = True Then
                            ChessBoard.UnmakeMove(temporaryMove)
                            If isCapture = True Then
                                ChessBoard.UndisposeCapturedPiece()
                            End If
                            ChessBoard.FillEmptyBitboards()
                            ChessBoard.GenerateBitboards()
                        Else
                            moveMade = True
                            If ChessBoard.GetRookCastleMove.StartSquare <> Nothing Or ChessBoard.GetRookCastleMove.TargetSquare <> Nothing Then
                                ChessBoard.MakeMove(ChessBoard.GetRookCastleMove, False)
                            End If
                            ChessBoard.SetRookCastleMove(Nothing, Nothing)
                            Settings.IncrementPlayerTurn()
                            ChessBoard.FillEmptyBitboards()
                            ChessBoard.GenerateBitboards()
                        End If
                    End If
                    selectedKing = Nothing

                End If

                If isCapture = True And legalMovement = True Then
                    My.Computer.Audio.Play(My.Resources.pieceCaptureSound, AudioPlayMode.Background)            'Plays a capture sound
                    ChessBoard.CheckIfGameOver()
                    If moveMade = True And ChessBoard.GetPromotingPawn Is Nothing Then

                        If Settings.GetColourToMove() = Piece.White Then                    'Selects the next move to be played
                            Dim whitePlayer As Integer = Settings.GetWhitePlayer()
                            If whitePlayer = 1 Then
                                ChessBoard.PlayAILevel1()
                            ElseIf whitePlayer = 2 Then
                                ChessBoard.PlayAILevel2()
                            ElseIf whitePlayer = 3 Then
                                ChessBoard.PlayAILevel3()
                            ElseIf whitePlayer = 4 Then
                                ChessBoard.PlayAILevel4(0)
                            End If
                        ElseIf Settings.GetColourToMove() = Piece.Black Then
                            Dim blackPlayer As Integer = Settings.GetBlackPlayer()
                            If blackPlayer = 1 Then
                                ChessBoard.PlayAILevel1()
                            ElseIf blackPlayer = 2 Then
                                ChessBoard.PlayAILevel2()
                            ElseIf blackPlayer = 3 Then
                                ChessBoard.PlayAILevel3()
                            ElseIf blackPlayer = 4 Then
                                ChessBoard.PlayAILevel4(0)
                            End If
                        End If
                    End If
                ElseIf legalMovement = True Then
                    My.Computer.Audio.Play(My.Resources.pieceMoveSound, AudioPlayMode.Background)           'Plays a piece move sound
                    ChessBoard.CheckIfGameOver()

                    If moveMade = True And ChessBoard.GetPromotingPawn Is Nothing Then              'Selects the next move to be played
                        If Settings.GetColourToMove() = Piece.White Then
                            Dim whitePlayer As Integer = Settings.GetWhitePlayer()
                            If whitePlayer = 1 Then
                                ChessBoard.PlayAILevel1()
                            ElseIf whitePlayer = 2 Then
                                ChessBoard.PlayAILevel2()
                            ElseIf whitePlayer = 3 Then
                                ChessBoard.PlayAILevel3()
                            ElseIf whitePlayer = 4 Then
                                ChessBoard.PlayAILevel4(0)
                            End If
                        ElseIf Settings.GetColourToMove() = Piece.Black Then
                            Dim blackPlayer As Integer = Settings.GetBlackPlayer()
                            If blackPlayer = 1 Then
                                ChessBoard.PlayAILevel1()
                            ElseIf blackPlayer = 2 Then
                                ChessBoard.PlayAILevel2()
                            ElseIf blackPlayer = 3 Then
                                ChessBoard.PlayAILevel3()
                            ElseIf blackPlayer = 4 Then
                                ChessBoard.PlayAILevel4(0)
                            End If
                        End If
                    Else
                        My.Computer.Audio.Play(My.Resources.pieceMoveSound, AudioPlayMode.Background)       'Plays a piece move sound
                    End If
                End If
            End If
            Me.Refresh()                'Redraws the piece on the board
            pieceSelected = False
        End If
    End Sub
    Function UnmakePlayerMove(pieceObject)
        Dim rank As Integer = pieceObject.GetRank()     'Sets the coordinates of the original square on 
        Dim file As Integer = pieceObject.GetFile()     'the board, based from the original file and rank

        Dim correctivePosition As New Point((file * 90) + 11, ((7 - rank) * 90) + 11)

        Return correctivePosition
    End Function
#End Region
#Region "Create Board and Pieces"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.Size = New Size(1500, 790)               'Sets the form position
        Me.Left = (Screen.PrimaryScreen.WorkingArea.Width - Me.Width) / 2
        Me.Top = (Screen.PrimaryScreen.WorkingArea.Height - Me.Height) / 2
        Me.MaximizeBox = False

        ChessBoard.FillEmptyBitboards()     'Clears the bitboard
        ChessBoard.FEN()                        'Loads the board piece positions
        ChessBoard.SetSquareColours()                   'Sets the board square colours
        ChessBoard.PrecomputatedFirstMoveVariables()    'Sets some starting variables, based on the FEN string
        ChessBoard.PrecomputatedMoveData()              'Calculates the direction variables of each board square
        ChessBoard.GenerateBitboards()                   'Updates the bitboards
        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + "---"                   'Sets some starting labels
        lblBestEvaluation.Text = "Best Evaluation:" + vbNewLine + "---"
        lblBranchesPruned.Text = "Branches Pruned:" + vbNewLine + "---"
        lblCheckMessage.Text = "Check Message:" + vbNewLine + "---"
        lblMovesEvaluated.Text = "Moves Evaluated:" + vbNewLine + "---"
        lblMoveTest.Text = "Moves Generated:" + vbNewLine + "---"
        lblSearchDepth.Text = "Search Depth:" + vbNewLine + "---"

        If Settings.GetMoveTest = True Then         'Sets the labels and buttons for move testing
            mouseLocked = True
            btnStartGame.Enabled = False
            btnStartGame.Visible = False
            btnUndoMove.Enabled = False
            btnUndoMove.Visible = False

            lblWhitePlayer.Text = "Move Test Player"
            lblWhiteDescription.Text = "Generates Moves"

            lblBlackPlayer.Text = "Move Test Player"
            lblBlackDescription.Text = "Generates Moves"
            lblMoveTest.Visible = True
            lblMoveTest.Location = New Point(1075, 425)
            btnMoveTest.Location = New Point(995, 650)

            lblSearchDepth.Visible = True
            lblSearchDepth.Location = New Point(1075, 275)

        Else
            btnMoveTest.Enabled = False
            btnMoveTest.Visible = False

            If Settings.GetWhitePlayer = 0 And Settings.GetBlackPlayer = 0 Then     'Sets the variables and buttons if two users are playing
                lblWhitePlayer.Text = "Player 1"
                lblWhiteDescription.Text = "Takes user moves"
                lblBlackPlayer.Text = "Player 2"
                lblBlackDescription.Text = "Takes user moves"
                lblSelectedPiece.Visible = True
                lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + "---"
                lblSelectedPiece.Location = New Point(1075, 400)
                lblPlayerTurn.Visible = True
                lblPlayerTurn.Location = New Point(1075, 250)
                btnUndoMove.Enabled = False

            Else                'Sets whether the start button should show, if an AI moves first
                If Settings.GetWhitePlayer <> 0 And Settings.GetBlackPlayer <> 0 Then
                    ShowStartButton()
                ElseIf Settings.GetWhitePlayer <> 0 And Settings.GetColourToMove() = Piece.White Then
                    ShowStartButton()
                ElseIf Settings.GetBlackPlayer <> 0 And Settings.GetColourToMove() = Piece.Black Then
                    ShowStartButton()
                End If
                Select Case Settings.GetWhitePlayer()               'Sets the appropriate buttons and labels for the player types
                    Case 0
                        lblWhitePlayer.Text = "Player"
                        lblWhiteDescription.Text = "Takes User Moves"
                        lblSelectedPiece.Visible = True
                        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + "---"
                        lblPlayerTurn.Visible = True
                    Case 1
                        lblWhitePlayer.Text = "AI Level 1"
                        lblWhiteDescription.Text = "Plays Random Moves"

                        lblSelectedPiece.Location = New Point(1075, 400)
                        lblPlayerTurn.Visible = True
                        lblPlayerTurn.Location = New Point(1075, 250)
                    Case 2
                        lblWhitePlayer.Text = "AI Level 2"
                        lblWhiteDescription.Text = "Focuses on Capturing Pieces"

                        lblSelectedPiece.Location = New Point(1075, 275)
                        lblSearchDepth.Visible = True
                        lblSearchDepth.Location = New Point(1075, 400)
                        lblPlayerTurn.Visible = True
                        lblPlayerTurn.Location = New Point(1075, 150)
                        lblBestEvaluation.Visible = True
                        lblBestEvaluation.Location = New Point(1075, 500)

                    Case 3
                        lblWhitePlayer.Text = "AI Level 3"
                        lblWhiteDescription.Text = "Advanced Captures and" + vbNewLine + "Observation"

                        lblPlayerTurn.Visible = True
                        lblPlayerTurn.Location = New Point(1075, 100)
                        lblSelectedPiece.Location = New Point(1075, 200)
                        lblSearchDepth.Visible = True
                        lblSearchDepth.Location = New Point(1075, 300)
                        lblBestEvaluation.Visible = True
                        lblBestEvaluation.Location = New Point(1075, 375)
                        lblMovesEvaluated.Visible = True
                        lblMovesEvaluated.Location = New Point(1075, 450)
                        lblBranchesPruned.Visible = True
                        lblBranchesPruned.Location = New Point(1075, 525)

                    Case 4
                        lblWhitePlayer.Text = "AI Level 4"
                        lblWhiteDescription.Text = "Extensive Decision Making"

                        lblPlayerTurn.Visible = True
                        lblPlayerTurn.Location = New Point(1075, 100)
                        lblSelectedPiece.Location = New Point(1075, 200)
                        lblSearchDepth.Visible = True
                        lblSearchDepth.Location = New Point(1075, 300)
                        lblBestEvaluation.Visible = True
                        lblBestEvaluation.Location = New Point(1075, 375)
                        lblMovesEvaluated.Visible = True
                        lblMovesEvaluated.Location = New Point(1075, 450)
                        lblBranchesPruned.Visible = True
                        lblBranchesPruned.Location = New Point(1075, 525)

                End Select
                Select Case Settings.GetBlackPlayer()
                    Case 0
                        lblBlackPlayer.Text = "Player"
                        lblBlackDescription.Text = "Takes User Moves"
                        lblSelectedPiece.Visible = True
                        lblSelectedPiece.Text = "Selected Piece:" + vbNewLine + "---"
                        lblPlayerTurn.Visible = True

                    Case 1
                        lblBlackPlayer.Text = "AI Level 1"
                        lblBlackDescription.Text = "Plays Random Moves"
                        If Settings.GetWhitePlayer < 1 Then
                            lblSelectedPiece.Location = New Point(1075, 400)
                            lblPlayerTurn.Visible = True
                            lblPlayerTurn.Location = New Point(1075, 250)
                        End If

                    Case 2
                        lblBlackPlayer.Text = "AI Level 2"
                        lblBlackDescription.Text = "Focuses on Capturing Pieces"
                        If Settings.GetWhitePlayer < 2 Then
                            lblSelectedPiece.Location = New Point(1075, 275)
                            lblSearchDepth.Visible = True
                            lblSearchDepth.Location = New Point(1075, 400)
                            lblPlayerTurn.Visible = True
                            lblPlayerTurn.Location = New Point(1075, 150)
                            lblBestEvaluation.Visible = True
                            lblBestEvaluation.Location = New Point(1075, 500)
                        End If

                    Case 3
                        lblBlackPlayer.Text = "AI Level 3"
                        lblBlackDescription.Text = "Advanced Captures and Observation"
                        If Settings.GetWhitePlayer < 3 Then
                            lblPlayerTurn.Visible = True
                            lblPlayerTurn.Location = New Point(1075, 100)
                            lblSelectedPiece.Location = New Point(1075, 200)
                            lblSearchDepth.Visible = True
                            lblSearchDepth.Location = New Point(1075, 300)
                            lblBestEvaluation.Visible = True
                            lblBestEvaluation.Location = New Point(1075, 375)
                            lblMovesEvaluated.Visible = True
                            lblMovesEvaluated.Location = New Point(1075, 450)
                            lblBranchesPruned.Visible = True
                            lblBranchesPruned.Location = New Point(1075, 525)
                        End If

                    Case 4
                        lblBlackPlayer.Text = "AI Level 4"
                        lblBlackDescription.Text = "Extensive Decision Making"
                        If Settings.GetWhitePlayer < 4 Then
                            lblPlayerTurn.Visible = True
                            lblPlayerTurn.Location = New Point(1075, 100)
                            lblSelectedPiece.Location = New Point(1075, 200)
                            lblSearchDepth.Visible = True
                            lblSearchDepth.Location = New Point(1075, 300)
                            lblBestEvaluation.Visible = True
                            lblBestEvaluation.Location = New Point(1075, 375)
                            lblMovesEvaluated.Visible = True
                            lblMovesEvaluated.Location = New Point(1075, 450)
                            lblBranchesPruned.Visible = True
                            lblBranchesPruned.Location = New Point(1075, 525)
                        End If
                End Select
            End If
        End If
    End Sub
    Sub Draw(pieceArray As Array, g As Graphics)        'Draws the pieces on the board
        Dim pieceColour, pieceType As Integer
        Dim pieceIcon As Image
        Dim pieceRectangle As Rectangle
        For Each element In pieceArray          'Loops through the array
            If element Is Nothing Then
                Continue For
            Else
                pieceColour = element.GetPieceColour()
                pieceType = element.GetPieceType()

                If pieceColour <> 0 Or pieceType <> 0 Then
                    pieceIcon = element.GetImage()
                    pieceRectangle = element.GetPieceRectangle()
                    g.DrawImage(pieceIcon, pieceRectangle)          'Draws the rectangle with the appropriate bitmap
                End If
            End If
        Next
    End Sub
    Private Sub CreateGraphicalBoard(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim graphicsObject As Graphics = e.Graphics

        Static darkBrush As New SolidBrush(Color.SaddleBrown)           'Declares the brushes for drawing
        Static lightBrush As New SolidBrush(Color.SandyBrown)           'the piece squares
        Dim boardSquares = ChessBoard.GetBoardSquares()

        For Each squareObject As BoardSquare In boardSquares            'Draws all the board squares on the form
            Dim rectangleObject As Rectangle = squareObject.GetRectangle()
            Select Case squareObject.GetSquareColour()
                Case Color.SandyBrown
                    graphicsObject.FillRectangle(lightBrush, rectangleObject)
                Case Color.SaddleBrown
                    graphicsObject.FillRectangle(darkBrush, rectangleObject)
            End Select
        Next

        Draw(ChessBoard.GetPawns, graphicsObject)           'Draws all the pieces on the board
        Draw(ChessBoard.GetKnights, graphicsObject)
        Draw(ChessBoard.GetBishops, graphicsObject)
        Draw(ChessBoard.GetRooks, graphicsObject)
        Draw(ChessBoard.GetQueens, graphicsObject)
        Draw(ChessBoard.GetKings, graphicsObject)
    End Sub
    Sub RefreshBoard()      'Redraws the board and all pieces
        Me.Refresh()
    End Sub

    Public Sub ToggleMouseLock()        'Changes whether the user can use the mouse
        mouseLocked = Not mouseLocked
    End Sub
#End Region
#Region "Pawn Promotion"
    Sub DisplayPawnPromotion(colour)        'Displays the user's options for promotions
        btnUndoMove.Enabled = False
        ToggleMouseLock()
        lblPromotionTitle.Text = "Pawn Promotion"
        lblPromotionTitle.Font = New Font("Calibri", 15, FontStyle.Bold)
        lblPromotionTitle.ForeColor = Color.White

        AddHandler picQueenPromotion.Click, AddressOf picQueenPromotion_Click
        picQueenPromotion.BorderStyle = BorderStyle.FixedSingle

        AddHandler picRookPromotion.Click, AddressOf picRookPromotion_Click
        picRookPromotion.BorderStyle = BorderStyle.FixedSingle

        AddHandler picBishopPromotion.Click, AddressOf picBishopPromotion_Click
        picBishopPromotion.BorderStyle = BorderStyle.FixedSingle

        AddHandler picKnightPromotion.Click, AddressOf picKnightPromotion_Click
        picKnightPromotion.BorderStyle = BorderStyle.FixedSingle

        Select Case colour              'Sets buttons and titles according to the promoting piece colour
            Case Piece.Black
                lblPromotionTitle.Location = New Point(925, 380)
                lblPromotionTitle.AutoSize = True

                picQueenPromotion.Image = My.Resources.BlackQueen
                picQueenPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picQueenPromotion.Location = New Point(950, 410)

                picRookPromotion.Image = My.Resources.BlackRook
                picRookPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picRookPromotion.Location = New Point(950, 470)

                picBishopPromotion.Image = My.Resources.BlackBishop
                picBishopPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picBishopPromotion.Location = New Point(950, 530)

                picKnightPromotion.Image = My.Resources.BlackKnight
                picKnightPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picKnightPromotion.Location = New Point(950, 590)

                picQueenPromotion.BackColor = Color.AntiqueWhite
                picRookPromotion.BackColor = Color.AntiqueWhite
                picBishopPromotion.BackColor = Color.AntiqueWhite
                picKnightPromotion.BackColor = Color.AntiqueWhite

            Case Piece.White
                lblPromotionTitle.Location = New Point(925, 80)
                lblPromotionTitle.AutoSize = True

                picQueenPromotion.Image = My.Resources.WhiteQueen
                picQueenPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picQueenPromotion.Location = New Point(950, 110)

                picRookPromotion.Image = My.Resources.WhiteRook
                picRookPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picRookPromotion.Location = New Point(950, 170)

                picBishopPromotion.Image = My.Resources.WhiteBishop
                picBishopPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picBishopPromotion.Location = New Point(950, 230)

                picKnightPromotion.Image = My.Resources.WhiteKnight
                picKnightPromotion.SizeMode = PictureBoxSizeMode.Zoom
                picKnightPromotion.Location = New Point(950, 290)

                picQueenPromotion.BackColor = Me.BackColor
                picRookPromotion.BackColor = Me.BackColor
                picBishopPromotion.BackColor = Me.BackColor
                picKnightPromotion.BackColor = Me.BackColor

        End Select

        lblPromotionTitle.Visible = True        'Displays all buttons on the form
        lblPromotionTitle.Enabled = True

        picQueenPromotion.Visible = True
        picRookPromotion.Visible = True
        picBishopPromotion.Visible = True
        picKnightPromotion.Visible = True

        picQueenPromotion.Enabled = True
        picRookPromotion.Enabled = True
        picBishopPromotion.Enabled = True
        picKnightPromotion.Enabled = True
        Controls.Add(lblPromotionTitle)
        Controls.Add(picQueenPromotion)
        Controls.Add(picRookPromotion)
        Controls.Add(picBishopPromotion)
        Controls.Add(picKnightPromotion)

    End Sub
    Sub UndisplayPawnPromotion()            'Hides all promotion buttons and labels 
        lblPromotionTitle.Visible = False
        lblPromotionTitle.Enabled = False

        picQueenPromotion.Visible = False
        picRookPromotion.Visible = False
        picBishopPromotion.Visible = False
        picKnightPromotion.Visible = False

        picQueenPromotion.Enabled = False
        picRookPromotion.Enabled = False
        picBishopPromotion.Enabled = False
        picKnightPromotion.Enabled = False

        ChessBoard.SetPromotingPawn(Nothing)
        ToggleMouseLock()
    End Sub
#Region "Promotion Button Events"
    Sub picQueenPromotion_Click(sender As Object, e As MouseEventArgs)
        ChessBoard.PromotePawn(ChessBoard.GetPromotingPawn, Piece.Queen)
    End Sub
    Sub picRookPromotion_Click(sender As Object, e As MouseEventArgs)
        ChessBoard.PromotePawn(ChessBoard.GetPromotingPawn, Piece.Rook)
    End Sub
    Sub picBishopPromotion_Click(sender As Object, e As MouseEventArgs)
        ChessBoard.PromotePawn(ChessBoard.GetPromotingPawn, Piece.Bishop)
    End Sub
    Sub picKnightPromotion_Click(sender As Object, e As MouseEventArgs)
        ChessBoard.PromotePawn(ChessBoard.GetPromotingPawn, Piece.Knight)
    End Sub
#End Region
#End Region
#Region "Button Events"
    Private Sub BtnMoveTest_Click(sender As Object, e As EventArgs) Handles btnMoveTest.Click
        Dim moveTestDepth As Integer = 2                        'Run the perft test move generation tree
        lblSearchDepth.Text = "Search Depth:" + vbNewLine + moveTestDepth.ToString
        lblMoveTest.Text = "Moves Generated: " + vbNewLine + ChessBoard.MoveGenerationTest(moveTestDepth).ToString
    End Sub
    Private Sub btnUndoMove_Click(sender As Object, e As EventArgs) Handles btnUndoMove.Click
        If ChessBoard.GetMoveCount() <> 0 Then              'Checks if any moves have been played
            ChessBoard.NewUnmakeMove(ChessBoard.GetLastMove(), ChessBoard.GetLastMoveCaptureValue)  'Unplays the last move
            Settings.IncrementPlayerTurn()
        Else
            MsgBox("No Moves Have Been Played" + vbNewLine + vbNewLine + "Move Cannot Be Undone!", 48, "Undo Move Error")
        End If
    End Sub
    Sub EnableUndoButton()
        btnUndoMove.Enabled = True
    End Sub
    Sub ShowStartButton()
        mouseLocked = True
        btnStartGame.Visible = True
        btnStartGame.Enabled = True
    End Sub
    Private Sub btnStartGame_Click(sender As Object, e As EventArgs) Handles btnStartGame.Click
        btnStartGame.Enabled = False
        ToggleMouseLock()
        Select Case Settings.GetColourToMove            'Plays the starting AI move
            Case Piece.White
                Select Case Settings.GetWhitePlayer()
                    Case 1
                        ChessBoard.PlayAILevel1()
                    Case 2
                        ChessBoard.PlayAILevel2()
                    Case 3
                        ChessBoard.PlayAILevel3()
                    Case 4
                        ChessBoard.PlayAILevel4(0)
                End Select
            Case Piece.Black
                Select Case Settings.GetBlackPlayer()
                    Case 1
                        ChessBoard.PlayAILevel1()
                    Case 2
                        ChessBoard.PlayAILevel2()
                    Case 3
                        ChessBoard.PlayAILevel3()
                    Case 4
                        ChessBoard.PlayAILevel4(0)
                End Select
        End Select

    End Sub
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()              'Closes the program
    End Sub

#End Region
End Class
