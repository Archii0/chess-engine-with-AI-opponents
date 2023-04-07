Imports System.Math
Public Class Board
    Private ReadOnly boardSquares(63) As BoardSquare
    Private ReadOnly pawns(15) As Pawn
    Private ReadOnly knights(3) As Knight
    Private ReadOnly bishops(3) As Bishop
    Private ReadOnly rooks(3) As Rook
    Private ReadOnly queens(1) As Queen
    Private ReadOnly kings(1) As King

    Private pawnsGenerated As Integer = 0
    Private knightsGenerated As Integer = 0
    Private bishopsGenerated As Integer = 0
    Private rooksGenerated As Integer = 0
    Private queensGenerated As Integer = 0
    Private kingsGenerated As Integer = 0

    Const startPosition As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    Const pawnValue As Integer = 100
    Const knightValue As Integer = 300
    Const bishopValue As Integer = 300
    Const rookValue As Integer = 500
    Const queenValue As Integer = 900
    Const kingValue As Integer = 10000

    Private whitePawnCount As Integer
    Private whiteKnightCount As Integer
    Private whiteBishopCount As Integer
    Private whiteRookCount As Integer
    Private whiteQueenCount As Integer

    Private blackPawnCount As Integer
    Private blackKnightCount As Integer
    Private blackBishopCount As Integer
    Private blackRookCount As Integer
    Private blackQueenCount As Integer

    Private moves As List(Of Movement)

    Private ReadOnly MovesMade As New List(Of MoveObject)
    Private ReadOnly disposedPieces As New List(Of Disposure)
    Private branchesPruned = 0
    Private movesEvaluated As Integer = 0

    Private ReadOnly directionOffsets = New Integer() {8, -8, -1, 1, 7, -7, 9, -9}
    Private ReadOnly pawnOffsets = New Integer() {8, 16, 9, 7}
    Private ReadOnly knightOffsets = New Integer() {6, 10, 15, 17, -6, -10, -15, -17}

    Private ReadOnly PieceToPieceValue As New Dictionary(Of Integer, Integer)
    Private bestEvaluation As Integer = 0

    Private ReadOnly MaximumDepth As Integer = 3
    Public Const Infinity As Integer = 9999999
    Private disposedPieceCountTotal = 0

    Private promotingPawn As Pawn
    Private EnPassentPawn As Pawn = Nothing
    Private EnPassentMove As Boolean = False
    Private rookCastleMove As Movement
    Public Structure Movement        'Holds the details for a piece movement
        Public StartSquare As Integer
        Public TargetSquare As Integer
    End Structure

    Public Structure MoveObject     'Holds the details for a piece movement and whether it is a capture move
        Public Move As Movement
        Public Capture As Boolean   'This is so pieces can be uncaptured when capture moves are unmade
    End Structure
    Public Structure Disposure              'Holds the details for captured pieces so that the attributes
        Public pieceType As Integer         'can be restored when a piece is uncaptured
        Public pieceIndex As Integer
        Public pieceColour As Integer
        Public squareNumber As Integer
        Public firstMoveNumber As Integer
        Public promotionTag As Integer
        Public promotionMoveNumber As Integer
        Public enpassentMoveNumber As Integer
    End Structure
    Sub SetSquareColours()
        Dim darkBrush As New SolidBrush(Color.SaddleBrown)
        Dim lightBrush As New SolidBrush(Color.SandyBrown)

        Dim isLightSquare As Boolean
        Dim index As Integer

        For rank = 0 To 7           'Loop through all the squares of the board
            For file = 0 To 7
                isLightSquare = (file + rank) Mod 2.0! = 1
                index = (rank * 8) + file

                If isLightSquare Then               'Assign the square colour accordingly
                    boardSquares(index).SetSquareColour(Color.SandyBrown)
                Else
                    boardSquares(index).SetSquareColour(Color.SaddleBrown)
                End If

                boardSquares(index).SetRectangleDetails(file * 90, (90 * 7) - rank * 90)     'Set the square details
            Next
        Next
    End Sub

    Sub PrecomputatedFirstMoveVariables()
        'This code sets the first move variables for the pawn, rook and king pieces.
        'It checks if the pieces in question are in their natural starting positions and also reads the 
        'castling availability from the fen string

        Dim FENlist() As String
        Try
            FENlist = ChessGame.GetSettings.GetStartFEN.Split(" ") 'Splits the FEN string into different parts
        Catch ex As Exception
            FENlist = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1".Split(" ")
        End Try

        For Each pawnObject In pawns                    'Loops through all pawn pieces on the board
            If pawnObject IsNot Nothing Then
                pawnObject.UpdateFirstMoveTaken()
                pawnObject.SetFirstMoveNumber(-Infinity)
                If pawnObject.GetPieceColour = Piece.White Then     'Resets the first move variables if they are in their starting positions
                    If pawnObject.GetRank = 1 Then
                        pawnObject.ResetFirstMoveTaken()
                        pawnObject.ResetFirstMoveNumber()
                    End If
                ElseIf pawnObject.GetPieceColour = Piece.Black Then
                    If pawnObject.GetRank = 6 Then
                        pawnObject.ResetFirstMoveTaken()
                        pawnObject.ResetFirstMoveNumber()
                    End If
                End If
            End If
        Next

        'These condition check the castling availability of the FEN string
        If FENlist(2) <> "KQkq" Then
            'If not all castling rights are valid, all king and rook pieces are updated so that they can't castle
            For Each rookObject In rooks
                If rookObject IsNot Nothing Then
                    rookObject.UpdateFirstMoveTaken()
                    rookObject.SetFirstMoveNumber(-Infinity)
                End If

            Next
            For Each kingObject In kings
                kingObject.UpdateFirstMoveTaken()
                kingObject.SetFirstMoveNumber(-Infinity)
            Next
            If FENlist(2).Contains("K") Then        'Resets the white king and kingside rook variables 
                For Each kingObject In kings
                    If kingObject.GetPieceColour() = Piece.White Then
                        kingObject.ResetFirstMoveTaken()
                        kingObject.ResetFirstMoveNumber()
                    End If
                Next
                For Each rookObject In rooks
                    If rookObject IsNot Nothing Then
                        If rookObject.GetSquareNumber() = 7 Then
                            rookObject.ResetFirstMoveTaken()
                            rookObject.ResetFirstMoveNumber()
                        End If
                    End If

                Next
            End If
            If FENlist(2).Contains("Q") Then            'Resets the white king and queenside rook variables 
                For Each kingObject In kings
                    If kingObject.GetPieceColour() = Piece.White Then
                        kingObject.ResetFirstMoveTaken()
                        kingObject.ResetFirstMoveNumber()
                    End If
                Next
                For Each rookObject In rooks
                    If rookObject IsNot Nothing Then
                        If rookObject.GetSquareNumber() = 0 Then
                            rookObject.ResetFirstMoveTaken()
                            rookObject.ResetFirstMoveNumber()
                        End If
                    End If

                Next
            End If
            If FENlist(2).Contains("k") Then           'Resets the black king and kingside rook variables 
                For Each kingObject In kings
                    If kingObject.GetPieceColour() = Piece.Black Then
                        kingObject.ResetFirstMoveTaken()
                        kingObject.ResetFirstMoveNumber()
                    End If
                Next
                For Each rookObject In rooks
                    If rookObject IsNot Nothing Then
                        If rookObject.GetSquareNumber() = 63 Then
                            rookObject.ResetFirstMoveTaken()
                            rookObject.ResetFirstMoveNumber()
                        End If
                    End If

                Next
            End If
            If FENlist(2).Contains("q") Then        'Resets the black king and queenside rook variables 
                For Each kingObject In kings
                    If kingObject.GetPieceColour() = Piece.Black Then
                        kingObject.ResetFirstMoveTaken()
                        kingObject.ResetFirstMoveNumber()
                    End If
                Next
                For Each rookObject In rooks
                    If rookObject IsNot Nothing Then
                        If rookObject.GetSquareNumber() = 56 Then
                            rookObject.ResetFirstMoveTaken()
                            rookObject.ResetFirstMoveNumber()
                        End If
                    End If

                Next
            End If
        End If

        'Sets the player turn based on the FEN string
        If FENlist(1).Equals("w") Then
            ChessGame.lblPlayerTurn.Text = "Player Turn:" + vbNewLine + "White's Move"
        ElseIf FENlist(1).Equals("b") Then
            ChessGame.lblPlayerTurn.Text = "Player Turn:" + vbNewLine + "Black's Move"
            ChessGame.GetSettings.SetColourToMove(Piece.Black)
        End If
    End Sub

    Sub New()           'Board constructor method
        With PieceToPieceValue                  'Add the pieces and piece values to a dictionary
            .Add(Piece.Pawn, pawnValue)
            .Add(Piece.Knight, knightValue)
            .Add(Piece.Bishop, bishopValue)
            .Add(Piece.Rook, rookValue)
            .Add(Piece.Queen, queenValue)
            .Add(Piece.King, kingValue)
        End With

    End Sub

#Region "Board Setup Code"
    Sub PrecomputatedMoveData()         'Sets the directional variables for all the board squares
        'these variables are used to make sure a piece doesn't move off the board
        For file = 0 To 7
            For rank = 0 To 7
                Dim numNorth As Integer = 7 - rank
                Dim numSouth As Integer = rank
                Dim numWest As Integer = file
                Dim numEast As Integer = 7 - file

                Dim squareIndex As Integer = (rank * 8) + file
                boardSquares(squareIndex).SetDirections(numNorth, numEast, numSouth, numWest)   'Updates the variables
                boardSquares(squareIndex).SetFileAndRank(file, rank)
            Next
        Next
    End Sub
    Public Sub FEN()        'Loads the FEN string (either custom or default)
        Dim startFEN As String = ChessGame.GetSettings.GetStartFEN()
        Dim tempArray As Array
        Try
            tempArray = startFEN.Split(" ")
            If startFEN = "" Or startFEN = Nothing Then                 'Check if FEN string is nothing
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "k").Length - 1 <> 1 Then        'Check if there is 1 black king
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "K").Length - 1 <> 1 Then        'Check if there is 1 white king
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "q").Length - 1 > 1 Then        'Check if there is 1 or less black queens
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "Q").Length - 1 > 1 Then        'Check if there is 1 or less white queens
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "r").Length - 1 > 2 Then        'Check if there is 1 or less black rooks
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "R").Length - 1 > 2 Then        'Check if there is 1 or less white rooks
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "b").Length - 1 > 2 Then        'Check if there is 1 or less black bishops
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "B").Length - 1 > 2 Then        'Check if there is 1 or less white bishops
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "n").Length - 1 > 2 Then        'Check if there is 1 or less black knights
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "N").Length - 1 > 2 Then        'Check if there is 1 or less white knights
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "p").Length - 1 > 8 Then        'Check if there is 1 or less black pawns
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "P").Length - 1 > 8 Then        'Check if there is 1 or less white pawns
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            ElseIf Split(tempArray(0), "/").Length - 1 <> 7 Then        'Check if there is 8 ranks
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            End If

            If tempArray(1) <> "w" And tempArray(1) <> "b" Then         'Check if the player turn is valid
                startFEN = startPosition
                ChessGame.GetSettings.SetStartFEN(startPosition)
            End If

            If startFEN <> startPosition Then                           'Check if there is 8 squares in each rank
                tempArray = startFEN.Split(" ")
                Dim count As Integer = 0
                For Each item In tempArray(0)
                    If item = "/" Then                                  'If it is a '/' check that the rank has 8 squares
                        If count <> 8 Then
                            startFEN = startPosition                    'Reset FEN string if not 8 in rank
                            Exit For
                        Else
                            count = 0                                   'Otherwise count the next rank
                        End If
                    ElseIf Char.IsDigit(item) = True Then
                        count += Char.GetNumericValue(item)             'If it is a number, add the number to the count
                    Else
                        count += 1                                      'If it is a letter, add 1 to the count
                    End If
                Next
            End If
        Catch ex As Exception
            startFEN = startPosition
            ChessGame.GetSettings.SetStartFEN(startPosition)
        End Try

        Dim file, rank As Integer
        file = 0
        rank = 7

        Dim PieceToCount As New Dictionary(Of String, Integer)
        With PieceToCount
            .Add("p", blackPawnCount)
            .Add("n", blackKnightCount)
            .Add("b", blackBishopCount)
            .Add("r", blackRookCount)
            .Add("q", blackQueenCount)
            .Add("P", whitePawnCount)
            .Add("N", whiteKnightCount)
            .Add("B", whiteBishopCount)
            .Add("R", whiteRookCount)
            .Add("Q", whiteQueenCount)
        End With

        For Each letter In startFEN         'Loops through the FEN string
            If letter = "/" Then            'If the letter is a slash, the next rank is indicated
                file = 0
                rank -= 1
            ElseIf letter = " " Then        'A space indicates the end of the piece placement part of the string
                Exit For

            ElseIf Char.IsDigit(letter) Then    'If the character is a number, this amount of spaces are left without a piece
                Dim number As Integer = Char.GetNumericValue(letter)
                For i = 0 To (number - 1)
                    boardSquares((rank * 8) + file) = New BoardSquare
                    file += 1
                Next
            Else
                Select Case letter              'If the character is a piece letter, the count will be updated
                    Case "p"
                        blackPawnCount += 1
                    Case "n"
                        blackKnightCount += 1
                    Case "b"
                        blackBishopCount += 1
                    Case "r"
                        blackRookCount += 1
                    Case "q"
                        blackQueenCount += 1
                    Case "P"
                        whitePawnCount += 1
                    Case "N"
                        whiteKnightCount += 1
                    Case "B"
                        whiteBishopCount += 1
                    Case "R"
                        whiteRookCount += 1
                    Case "Q"
                        whiteQueenCount += 1
                End Select

                If letter <> "k" And letter <> "K" Then
                    PieceToCount.Item(letter) += 1
                End If

                Dim lowerCaseCharacter As Char = Char.ToLower(letter)
                Dim pieceColour As Integer

                If Char.IsUpper(letter) Then
                    pieceColour = Piece.White
                Else
                    pieceColour = Piece.Black
                End If

                Dim XPosition As Integer = (file * 90) + 11                'Sets the piece position on the form
                Dim YPosition As Integer = (90 * 7) - (rank * 90) + 11

                If lowerCaseCharacter = "p" Then            'Generates the specified piece type and updates its attributes
                    pawns(pawnsGenerated) = New Pawn

                    With pawns(pawnsGenerated)
                        .SetPieceColour(pieceColour)
                        .SetFile(file)
                        .SetRank(rank)
                        .SetIcon(XPosition, YPosition)
                        .SetSquareNumber((rank * 8) + file)
                        .SetPieceIndex(pawnsGenerated)
                    End With

                    boardSquares(pawns(pawnsGenerated).GetSquareNumber()) = New BoardSquare
                    boardSquares(pawns(pawnsGenerated).GetSquareNumber()).SetPieceDetails(Piece.Pawn, pieceColour, pawnsGenerated)

                    pawnsGenerated += 1

                ElseIf lowerCaseCharacter = "n" Then

                    knights(knightsGenerated) = New Knight

                    With knights(knightsGenerated)
                        .SetPieceColour(pieceColour)
                        .SetFile(file)
                        .SetRank(rank)
                        .SetIcon(XPosition, YPosition)
                        .SetSquareNumber((rank * 8) + file)
                        .SetPieceIndex(knightsGenerated)
                    End With

                    boardSquares(knights(knightsGenerated).GetSquareNumber()) = New BoardSquare
                    boardSquares(knights(knightsGenerated).GetSquareNumber()).SetPieceDetails(Piece.Knight, pieceColour, knightsGenerated)

                    knightsGenerated += 1

                ElseIf lowerCaseCharacter = "b" Then

                    bishops(bishopsGenerated) = New Bishop
                    With bishops(bishopsGenerated)
                        .SetPieceColour(pieceColour)
                        .SetFile(file)
                        .SetRank(rank)
                        .SetIcon(XPosition, YPosition)
                        .SetSquareNumber((rank * 8) + file)
                        .SetPieceIndex(bishopsGenerated)
                    End With

                    boardSquares(bishops(bishopsGenerated).GetSquareNumber()) = New BoardSquare
                    boardSquares(bishops(bishopsGenerated).GetSquareNumber()).SetPieceDetails(Piece.Bishop, pieceColour, bishopsGenerated)

                    bishopsGenerated += 1

                ElseIf lowerCaseCharacter = "r" Then

                    rooks(rooksGenerated) = New Rook
                    With rooks(rooksGenerated)
                        .SetPieceColour(pieceColour)
                        .SetFile(file)
                        .SetRank(rank)
                        .SetIcon(XPosition, YPosition)
                        .SetSquareNumber((rank * 8) + file)
                        .SetPieceIndex(rooksGenerated)
                    End With

                    boardSquares(rooks(rooksGenerated).GetSquareNumber()) = New BoardSquare
                    boardSquares(rooks(rooksGenerated).GetSquareNumber()).SetPieceDetails(Piece.Rook, pieceColour, rooksGenerated)

                    rooksGenerated += 1

                ElseIf lowerCaseCharacter = "q" Then

                    queens(queensGenerated) = New Queen

                    With queens(queensGenerated)
                        .SetPieceColour(pieceColour)
                        .SetFile(file)
                        .SetRank(rank)
                        .SetIcon(XPosition, YPosition)
                        .SetSquareNumber((rank * 8) + file)
                        .SetPieceIndex(queensGenerated)
                    End With

                    boardSquares(queens(queensGenerated).GetSquareNumber()) = New BoardSquare
                    boardSquares(queens(queensGenerated).GetSquareNumber()).SetPieceDetails(Piece.Queen, pieceColour, queensGenerated)

                    queensGenerated += 1

                ElseIf lowerCaseCharacter = "k" Then

                    kings(kingsGenerated) = New King
                    With kings(kingsGenerated)
                        .SetPieceColour(pieceColour)
                        .SetFile(file)
                        .SetRank(rank)
                        .SetIcon(XPosition, YPosition)
                        .SetSquareNumber((rank * 8) + file)
                        .SetPieceIndex(kingsGenerated)
                    End With

                    boardSquares(kings(kingsGenerated).GetSquareNumber()) = New BoardSquare
                    boardSquares(kings(kingsGenerated).GetSquareNumber()).SetPieceDetails(Piece.King, pieceColour, kingsGenerated)
                    kingsGenerated += 1

                End If
                file += 1          'Increases the file when a letter has been evaluated
            End If
        Next
    End Sub
    Function EvaluateGameState(colourIndex)         'This checks if a player is in check
        Dim inCheck As Boolean = False

        Dim file As Integer
        Dim rank As Integer
        For Each king In kings          'Selects the appropriate king for the piece colour
            If king.GetPieceColour() = colourIndex Then
                file = king.GetFile()
                rank = king.GetRank()
                Exit For
            End If
        Next

        If colourIndex = Piece.White Then                       'Checks all the bitboards to see if king is attacked
            If blackPawnBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf blackKnightBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf blackBishopBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf blackRookBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf blackQueenBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf blackKingBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            End If
        ElseIf colourIndex = Piece.Black Then
            If whitePawnBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf whiteKnightBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf whiteBishopBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf whiteRookBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf whiteQueenBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            ElseIf whiteKingBitboard(7 - rank, file).Contains("X") Then
                inCheck = True
                Return inCheck
            End If
        End If
        Return inCheck              'Returns whether the king is in check
    End Function
#End Region

#Region "Simple Subroutines and Functions"
    Sub RemoveLastMove()            'Removes the last move from the MovesMade lsit
        MovesMade.RemoveAt(MovesMade.Count - 1)
    End Sub
    Sub AddNewMove(movement, captureValue)      'Adds a new move to the MovesMade list
        MovesMade.Add(New MoveObject With {
                      .Move = movement,
                      .Capture = captureValue})
    End Sub
    Sub SetRookCastleMove(startSquare, targetSquare)
        rookCastleMove.StartSquare = startSquare
        rookCastleMove.TargetSquare = targetSquare
    End Sub
    Sub SetPromotingPawn(x)
        promotingPawn = x
    End Sub
    Sub SetEnpassentPawn(x)
        EnPassentPawn = x
    End Sub
    Sub SetEnpassentMove(x)
        EnPassentMove = x
    End Sub
    Function GetLastMove() As Movement
        Return MovesMade(MovesMade.Count - 1).Move
    End Function
    Function GetLastMoveCaptureValue()
        Return MovesMade(MovesMade.Count - 1).Capture
    End Function
    Function GetMoveCount()
        Return MovesMade.Count
    End Function
    Function GetPromotingPawn()
        Return promotingPawn
    End Function
    Function GetEnpassentPawn()
        Return EnPassentPawn
    End Function
    Function GetRookCastleMove()
        Return rookCastleMove
    End Function
    Function GetEnpassentMove()
        Return EnPassentMove
    End Function
    Function GetPieceToPieceValueDictionary()
        Return PieceToPieceValue
    End Function
    Function GetPawnValue()
        Return pawnValue
    End Function
    Function GetKnightValue()
        Return knightValue
    End Function
    Function GetBishopValue()
        Return bishopValue
    End Function
    Function GetRookValue()
        Return rookValue
    End Function
    Function GetQueenValue()
        Return queenValue
    End Function
    Function GetKingValue()
        Return kingValue
    End Function
#End Region

#Region "Promotions"
    Sub PromoteAIPawn(move As Movement, colour As Integer)
        Dim pieceImage As Bitmap

        If colour = Piece.White Then                'Selects the right queen bitmap to display after the promotion
            pieceImage = New Bitmap(My.Resources.WhiteQueen)
            whiteQueenCount += 1
            whitePawnCount -= 1
        ElseIf colour = Piece.Black Then
            pieceImage = New Bitmap(My.Resources.BlackQueen)
            blackQueenCount += 1
            blackPawnCount -= 1
        End If

        Dim file As Integer = boardSquares(move.TargetSquare).GetFile()         'Saves the important attributes of the pawn
        Dim rank As Integer = boardSquares(move.TargetSquare).GetRank()         'that is being promoted
        Dim pieceIndex As Integer = boardSquares(move.StartSquare).GetPieceIndex()
        Dim enpassentMoveNumber As Integer = pawns(pieceIndex).GetEnpassentMoveNumber()

        pawns(pieceIndex).Dispose()                     'Disposes of the pawn object
        ChessGame.RefreshBoard()
        pawns(pieceIndex) = New Pawn                    'Creates a new pawn object, so it can be drawn with a queen bitmap
        pawns(pieceIndex).SetImage(pieceImage)          'The new object's attributes are updated to match the old pawn
        pawns(pieceIndex).SetSquareNumber(file, rank)
        pawns(pieceIndex).SetPosition((file * 90) + 11, (90 * 7) - (rank * 90) + 11, 68, 68)
        pawns(pieceIndex).SetPieceIndex(pieceIndex)
        pawns(pieceIndex).SetPieceColour(colour)
        pawns(pieceIndex).SetPromotionMoveNumber(GetMoveCount())
        pawns(pieceIndex).UpdateFirstMoveTaken()
        pawns(pieceIndex).SetEnpassentMoveNumber(enpassentMoveNumber)
        pawns(pieceIndex).SetPromotionTag(Piece.Queen)

        ChessGame.RefreshBoard()            'Displays the new 'queen' on the board
    End Sub

    Sub PromoteAIPawnWithoutRefresh(move As Movement, colour As Integer)      'Does the same as above, but without refreshing the board
        Dim pieceImage As Bitmap

        If colour = Piece.White Then
            pieceImage = New Bitmap(My.Resources.WhiteQueen)
            whiteQueenCount += 1
            whitePawnCount -= 1
        ElseIf colour = Piece.Black Then
            pieceImage = New Bitmap(My.Resources.BlackQueen)
            blackQueenCount += 1
            blackPawnCount -= 1
        End If

        Dim file As Integer = boardSquares(move.TargetSquare).GetFile()
        Dim rank As Integer = boardSquares(move.TargetSquare).GetRank()
        Dim pieceIndex As Integer = boardSquares(move.TargetSquare).GetPieceIndex()
        Dim enpassentMoveNumber As Integer = pawns(pieceIndex).GetEnpassentMoveNumber()

        pawns(pieceIndex).Dispose()

        pawns(pieceIndex) = New Pawn
        pawns(pieceIndex).SetImage(pieceImage)
        pawns(pieceIndex).SetSquareNumber(file, rank)
        pawns(pieceIndex).SetPosition((file * 90) + 11, (90 * 7) - (rank * 90) + 11, 68, 68)
        pawns(pieceIndex).SetPieceIndex(pieceIndex)
        pawns(pieceIndex).SetPieceColour(colour)
        pawns(pieceIndex).SetPromotionMoveNumber(GetMoveCount())
        pawns(pieceIndex).UpdateFirstMoveTaken()
        pawns(pieceIndex).SetEnpassentMoveNumber(enpassentMoveNumber)


        pawns(pieceIndex).SetPromotionTag(Piece.Queen)
        'ChessGame.RefreshBoard()
        ChessGame.GetSettings.IncrementPlayerTurn()

    End Sub

    Sub PromotePawn(pawn As Pawn, pieceToPromote As Integer)        'Promotes a pawn moved by a user

        If promotingPawn IsNot Nothing Then
            Dim pieceImage As Bitmap
            Dim tagHolder As Integer
            Dim pieceColour As Integer
            If pawn.GetPieceColour() = Piece.Black Then         'Sets the appropriate queen bitmap and updates the
                pieceColour = Piece.Black                       'piece count variables accordingly
                blackPawnCount -= 1
                If pieceToPromote = Piece.Queen Then
                    pieceImage = New Bitmap(My.Resources.BlackQueen)
                    tagHolder = Piece.Queen
                    blackQueenCount += 1
                ElseIf pieceToPromote = Piece.Rook Then
                    pieceImage = New Bitmap(My.Resources.BlackRook)
                    tagHolder = Piece.Rook
                    blackRookCount += 1
                ElseIf pieceToPromote = Piece.Bishop Then
                    pieceImage = New Bitmap(My.Resources.BlackBishop)
                    tagHolder = Piece.Bishop
                    blackBishopCount += 1
                ElseIf pieceToPromote = Piece.Knight Then
                    pieceImage = New Bitmap(My.Resources.BlackKnight)
                    tagHolder = Piece.Knight
                    blackKnightCount += 1
                End If

            ElseIf pawn.GetPieceColour() = Piece.White Then
                pieceColour = Piece.White
                whitePawnCount -= 1
                If pieceToPromote = Piece.Queen Then
                    pieceImage = New Bitmap(My.Resources.WhiteQueen)
                    tagHolder = Piece.Queen
                    whiteQueenCount += 1
                ElseIf pieceToPromote = Piece.Rook Then
                    pieceImage = New Bitmap(My.Resources.WhiteRook)
                    tagHolder = Piece.Rook
                    whiteRookCount += 1
                ElseIf pieceToPromote = Piece.Bishop Then
                    pieceImage = New Bitmap(My.Resources.WhiteBishop)
                    tagHolder = Piece.Bishop
                    whiteBishopCount += 1
                ElseIf pieceToPromote = Piece.Knight Then
                    pieceImage = New Bitmap(My.Resources.WhiteKnight)
                    tagHolder = Piece.Knight
                    whiteKnightCount += 1
                End If
            End If

            Dim file As Integer = pawn.GetFile()                'Stores the old pawn attributes
            Dim rank As Integer = pawn.GetRank()
            Dim pieceIndex As Integer = pawn.GetPieceIndex()
            Dim enpassentMoveNumber As Integer = pawn.GetEnpassentMoveNumber()

            pawn.Dispose()                              'Disposes the promoting pawn object
            pawns(pieceIndex) = New Pawn                'Creates a new pawn object, so it can be drawn with a queen bitmap
            pawns(pieceIndex).SetImage(pieceImage)          'The new object's attributes are updated to match the old pawn
            pawns(pieceIndex).SetSquareNumber(file, rank)
            pawns(pieceIndex).SetPieceColour(pieceColour)
            pawns(pieceIndex).SetPosition((file * 90) + 11, (90 * 7) - (rank * 90) + 11, 68, 68)
            pawns(pieceIndex).SetPieceIndex(pieceIndex)
            pawns(pieceIndex).SetPromotionMoveNumber(GetMoveCount())
            pawns(pieceIndex).UpdateFirstMoveTaken()
            pawns(pieceIndex).SetEnpassentMoveNumber(enpassentMoveNumber)

            pawns(pieceIndex).SetPromotionTag(tagHolder)        'Sets the promotion tag so that the piece moves can be validated
            ChessGame.RefreshBoard()                            'according to the new piece type

            ChessGame.GetSettings.UnlockPlayerTurn()
            ChessGame.GetSettings.IncrementPlayerTurn()
            ChessGame.UndisplayPawnPromotion()
            ChessGame.EnableUndoButton()

            If ChessGame.GetSettings.GetColourToMove() = Piece.White Then       'Plays the next move, if the opponent is an AI
                Dim whitePlayer As Integer = ChessGame.GetSettings.GetWhitePlayer()
                If whitePlayer = 1 Then
                    PlayAILevel1()
                ElseIf whitePlayer = 2 Then
                    PlayAILevel2()
                ElseIf whitePlayer = 3 Then
                    PlayAILevel3()
                ElseIf whitePlayer = 4 Then
                    PlayAILevel4(0)
                End If
            ElseIf ChessGame.GetSettings.GetColourToMove() = Piece.Black Then
                Dim blackPlayer As Integer = ChessGame.GetSettings.GetBlackPlayer()
                If blackPlayer = 1 Then
                    PlayAILevel1()
                ElseIf blackPlayer = 2 Then
                    PlayAILevel2()
                ElseIf blackPlayer = 3 Then
                    PlayAILevel3()
                ElseIf blackPlayer = 4 Then
                    PlayAILevel4(0)
                End If
            End If
        End If
    End Sub

    Sub ReversePawnPromotion(pawn As Pawn)          'Reverses pawn promotions
        Dim pieceImage

        If pawn.GetPieceColour() = Piece.White Then             'Sets the appropriate pawn bitmap and
            whitePawnCount += 1                                 'updates the appropriate piece count variables
            pieceImage = New Bitmap(My.Resources.WhitePawn)
            Select Case pawn.GetPromotionTag()
                Case Piece.Queen
                    whiteQueenCount -= 1
                Case Piece.Rook
                    whiteRookCount -= 1
                Case Piece.Bishop
                    whiteBishopCount -= 1
                Case Piece.Knight
                    whiteKnightCount -= 1
            End Select
        ElseIf pawn.GetPieceColour() = Piece.Black Then
            blackPawnCount += 1
            pieceImage = New Bitmap(My.Resources.BlackPawn)
            Select Case pawn.GetPromotionTag()
                Case Piece.Queen
                    blackQueenCount -= 1
                Case Piece.Rook
                    blackRookCount -= 1
                Case Piece.Bishop
                    blackBishopCount -= 1
                Case Piece.Knight
                    blackKnightCount -= 1
            End Select
        End If

        Dim file As Integer = pawn.GetFile()                    'Stores the attributes of the promoted pawn
        Dim rank As Integer = pawn.GetRank()
        Dim pieceIndex As Integer = pawn.GetPieceIndex()
        Dim pieceColour As Integer = pawn.GetPieceColour()
        Dim enpassentMoveNumber As Integer = pawn.GetEnpassentMoveNumber()
        pawn.Dispose()                              'Disposes the promoted pawn

        pawns(pieceIndex) = New Pawn                'Creates a new pawn object
        pawns(pieceIndex).SetImage(pieceImage)              'Updates the new pawn's piece attributes
        pawns(pieceIndex).SetSquareNumber(file, rank)
        pawns(pieceIndex).SetPosition((file * 90) + 11, (90 * 7) - (rank * 90) + 11, 68, 68)
        pawns(pieceIndex).SetPieceIndex(pieceIndex)
        pawns(pieceIndex).SetPieceColour(pieceColour)
        pawns(pieceIndex).SetPromotionTag(Piece.None)
        pawns(pieceIndex).ResetPromotionMoveNumber()
        pawns(pieceIndex).UpdateFirstMoveTaken()
        pawns(pieceIndex).SetEnpassentMoveNumber(enpassentMoveNumber)

        ChessGame.RefreshBoard()
    End Sub

    Sub ReversePawnPromotionWithoutRefresh(pawn As Pawn)        'Does the same as above, but without refreshing the board
        Dim pieceImage
        yo = pawn.GetPieceColour
        If pawn.GetPieceColour() = Piece.White Then
            whitePawnCount += 1
            pieceImage = New Bitmap(My.Resources.WhitePawn)
            Select Case pawn.GetPromotionTag()
                Case Piece.Queen
                    whiteQueenCount -= 1
                Case Piece.Rook
                    whiteRookCount -= 1
                Case Piece.Bishop
                    whiteBishopCount -= 1
                Case Piece.Knight
                    whiteKnightCount -= 1
            End Select
        ElseIf pawn.GetPieceColour() = Piece.Black Then
            blackPawnCount += 1
            pieceImage = New Bitmap(My.Resources.BlackPawn)
            Select Case pawn.GetPromotionTag()
                Case Piece.Queen
                    blackQueenCount -= 1
                Case Piece.Rook
                    blackRookCount -= 1
                Case Piece.Bishop
                    blackBishopCount -= 1
                Case Piece.Knight
                    blackKnightCount -= 1
            End Select
        End If

        Dim file As Integer = pawn.GetFile()
        Dim rank As Integer = pawn.GetRank()
        Dim pieceIndex As Integer = pawn.GetPieceIndex()
        Dim pieceColour As Integer = pawn.GetPieceColour()
        Dim enpassentMoveNumber As Integer = pawn.GetEnpassentMoveNumber()
        pawn.Dispose()

        pawns(pieceIndex) = New Pawn
        pawns(pieceIndex).SetImage(pieceImage)
        pawns(pieceIndex).SetSquareNumber(file, rank)
        pawns(pieceIndex).SetPosition((file * 90) + 11, (90 * 7) - (rank * 90) + 11, 68, 68)
        pawns(pieceIndex).SetPieceIndex(pieceIndex)
        pawns(pieceIndex).SetPieceColour(pieceColour)
        pawns(pieceIndex).SetPromotionTag(Piece.None)
        pawns(pieceIndex).ResetPromotionMoveNumber()
        pawns(pieceIndex).UpdateFirstMoveTaken()
        pawns(pieceIndex).SetEnpassentMoveNumber(enpassentMoveNumber)
    End Sub

#End Region

#Region "Move Generation"
    Function GenerateMoves()    'Generates all valid moves that a side can make
        Dim pieceType As Integer
        moveList = New List(Of Movement)

        For Square = 0 To 63                'Loops through all the squares on the board
            pieceType = boardSquares(Square).GetPieceType()
            If Square = 1 Or Square = 6 Then
                Application.DoEvents()
            End If

            'Checks if there's a piece on the square, and it is the relevant colour to generate moves for
            If pieceType <> Piece.None And boardSquares(Square).GetPieceColour() = ChessGame.GetSettings.GetColourToMove() Then
                Select Case pieceType                                   'Generates moves for the specific piece type
                    Case Piece.Pawn
                        If pawns(boardSquares(Square).GetPieceIndex()).GetPromotionTag() = 0 Then
                            GeneratePawnMoves(Square, moveList)
                        Else
                            GenerateSlidingMoves(Square, pawns(boardSquares(Square).GetPieceIndex()).GetPromotionTag(), moveList)
                        End If
                    Case Piece.Rook
                        GenerateSlidingMoves(Square, pieceType, moveList)
                    Case Piece.Queen
                        GenerateSlidingMoves(Square, pieceType, moveList)
                    Case Piece.Bishop
                        GenerateSlidingMoves(Square, pieceType, moveList)
                    Case Piece.Knight
                        GenerateOtherPieceMoves(Square, pieceType, moveList)
                    Case Piece.King
                        GenerateOtherPieceMoves(Square, pieceType, moveList)
                End Select
            End If
        Next
        'Filters out illegal moves (that cause a friendly check)
        For i = moveList.Count - 1 To 0 Step -1         'The moves are evaluated from the end of the list to stop updating the indexes in the list
            Dim isCapture As Boolean = False
            If CheckIfCapture(moveList(i)) Then
                isCapture = True
                DisposeCapturedPieceWithoutRefresh(moveList(i))
            End If

            MakeMoveWithoutRefresh(moveList(i))         'Plays the move
            FillEmptyBitboards()                     'Updates bitboards after the move has been made
            GenerateBitboards()

            If EvaluateGameState(ChessGame.GetSettings.GetColourToMove()) = True Then       'Checks if the move puts the player in check
                UnmakeMoveWithoutRefresh(moveList(i))
                If isCapture = True Then
                    UndisposeCapturedPieceWithoutRefresh()
                End If
                moveList.Remove(moveList(i))                                              'If the player is in check the move will be removed from the list

            Else
                UnmakeMoveWithoutRefresh(moveList(i))

                If isCapture = True Then                                        'Undisposes a captured piece (if the move involved a capture)
                    UndisposeCapturedPieceWithoutRefresh()
                End If
            End If
        Next
        FillEmptyBitboards()
        GenerateBitboards()                                                     'Generates the bitboards again to reset them for the current board state
        Return moveList
    End Function
    Sub GeneratePawnMoves(startSquare, movelist)            'Generates move for pawn objects
        If startSquare = 14 Then
            Application.DoEvents()
        End If
        For Each directionIndex In pawnOffsets                           'Loops through the direction indexes for pawn moves
            If boardSquares(startSquare).GetPieceColour() = Piece.Black Then        'If black piece, sets the index as negative as it can only travel southwards
                directionIndex = -directionIndex
            End If

            Dim directionInterest As Integer
            Select Case directionIndex                     'Sets the dimension of motion to check if the move is still on the board
                Case 8 : directionInterest = boardSquares(startSquare).GetNorth()
                Case 16 : directionInterest = boardSquares(startSquare).GetNorth()
                Case 9 : directionInterest = boardSquares(startSquare).GetNorthEast()
                Case 7 : directionInterest = boardSquares(startSquare).GetNorthWest()
                Case -8 : directionInterest = boardSquares(startSquare).GetSouth()
                Case -16 : directionInterest = boardSquares(startSquare).GetSouth()
                Case -9 : directionInterest = boardSquares(startSquare).GetSouthWest()
                Case -7 : directionInterest = boardSquares(startSquare).GetSouthEast()
            End Select

            If directionInterest >= 1 Then                       'Checks if direction is viable and the piece would still be on the board
                Dim targetSquare As Integer = startSquare + directionIndex        'Sets target square
                If targetSquare < 0 Or targetSquare > 63 Then                'If the square number isn't valid then the move isn't added
                    Continue For
                End If
                If boardSquares(targetSquare).GetPieceType <> Piece.None Then          'Checks if there is a  piece on the target square

                    If boardSquares(startSquare).GetPieceColour() = boardSquares(targetSquare).GetPieceColour() Then    'Checks if it is blocked by friendly piece
                        Continue For                                                                            'If it is the move isn't added
                    Else                                                                        'This is if the square is blocked by an enemy piece
                        If directionIndex = 9 Or directionIndex = 7 Or directionIndex = -9 Or directionIndex = -7 Then      'Checks if the direction is a capture motion
                            movelist.Add(New Movement With {                                           'If it is the move is valid and is added
                                .StartSquare = startSquare,
                                 .TargetSquare = targetSquare
                            })
                            Continue For
                        Else
                            Continue For
                        End If
                    End If
                Else
                    If directionIndex = 9 Or directionIndex = 7 Or directionIndex = -9 Or directionIndex = -7 Then              'Checks if there is an enpassent capture 
                        If EnPassentPawn IsNot Nothing Then
                            If startSquare = EnPassentPawn.GetSquareNumber() + 1 Or startSquare = EnPassentPawn.GetSquareNumber() - 1 Then
                                If targetSquare = EnPassentPawn.GetSquareNumber() + 8 Or targetSquare = EnPassentPawn.GetSquareNumber() - 8 Then
                                    movelist.Add(New Movement With {                                                               'Adds the enpassent capture move
                                 .StartSquare = startSquare,
                                 .TargetSquare = targetSquare
                             })
                                End If
                            End If
                        End If
                        Continue For
                    ElseIf directionIndex = 16 Or directionIndex = -16 Then                             'Checks if there motion is a double pawn push
                        If boardSquares(startSquare + (directionIndex / 2)).GetPieceType() = Piece.None Then       'Checks the target square is unoccupied
                            Dim pawnIndex As Integer = boardSquares(startSquare).GetPieceIndex
                            Dim firstMoveTaken As Integer = pawns(pawnIndex).GetFirstMoveTaken()
                            If firstMoveTaken = False Then                                              'Checks the first move variable
                                movelist.Add(New Movement With {
                                 .StartSquare = startSquare,
                                 .TargetSquare = targetSquare
                             })
                            Else
                                Continue For
                            End If
                        End If
                    Else
                        movelist.Add(New Movement With {
                              .StartSquare = startSquare,
                              .TargetSquare = targetSquare
                          })
                        Continue For
                    End If
                End If
            End If
        Next
    End Sub
    Sub GenerateSlidingMoves(startSquare, pieceType, moveList)          'Generates moves for bishops, rooks and queens
        Dim startDirIndex As Integer = 0
        Dim endDirIndex As Integer = 7

        If pieceType = Piece.Bishop Then        'Selects the appropriate direction indexes for the piece type
            startDirIndex = 4
        ElseIf pieceType = Piece.Rook Then
            endDirIndex = 3
        End If

        For directionIndex = startDirIndex To endDirIndex           'Loops through all directions
            Dim direction As Integer = directionOffsets(directionIndex)
            Dim directionInterest As Integer
            Select Case direction
                Case 8
                    directionInterest = boardSquares(startSquare).GetNorth()
                Case -8
                    directionInterest = boardSquares(startSquare).GetSouth()
                Case -1
                    directionInterest = boardSquares(startSquare).GetWest()
                Case 1
                    directionInterest = boardSquares(startSquare).GetEast()
                Case 7
                    directionInterest = boardSquares(startSquare).GetNorthWest()
                Case -7
                    directionInterest = boardSquares(startSquare).GetSouthEast()
                Case 9
                    directionInterest = boardSquares(startSquare).GetNorthEast()
                Case -9
                    directionInterest = boardSquares(startSquare).GetSouthWest()

            End Select

            For n = 0 To directionInterest - 1              'Loops through the moves that are still on the board

                Dim targetSquare As Integer = startSquare + directionOffsets(directionIndex) * (n + 1)

                If targetSquare > 63 Or targetSquare < 0 Then
                    Continue For
                End If
                'Check piece on target square
                If boardSquares(targetSquare).GetPieceType <> Piece.None Then

                    'Blocked by friendly piece
                    If boardSquares(startSquare).GetPieceColour() = boardSquares(targetSquare).GetPieceColour() Then
                        'Don't add move
                        Exit For

                    Else
                        'Can't move any further in direction after capturing opponent's piece
                        moveList.Add(New Movement With {
                       .StartSquare = startSquare,
                       .TargetSquare = targetSquare
                   })
                        Exit For
                    End If
                Else
                    moveList.Add(New Movement With {
                   .StartSquare = startSquare,
                   .TargetSquare = targetSquare
               })
                End If
            Next
        Next
    End Sub
    Sub GenerateOtherPieceMoves(startSquare, pieceType, moveList)       'Generates knight and king moves

        Dim movementArray() As Integer
        Dim IsKing As Boolean = False

        If pieceType = Piece.King Then          'Selects the appropriate direction offsets for the piece type
            movementArray = directionOffsets
            IsKing = True
        ElseIf pieceType = Piece.Knight Then
            movementArray = knightOffsets
        End If

        If IsKing = True Then
            If kings(boardSquares(startSquare).GetPieceIndex()).GetFirstMoveTaken() = False Then    'Generate castling moves

                If startSquare = 4 Then             'Checks the castling conditions for the white king
                    If boardSquares(7).GetPieceType() = Piece.Rook Then             'Checks the conditions for kingside castling
                        If boardSquares(5).GetPieceType() = Piece.None And boardSquares(6).GetPieceType() = Piece.None And boardSquares(7).GetPieceColour() = kings(boardSquares(startSquare).GetPieceIndex()).GetPieceColour And rooks(boardSquares(7).GetPieceIndex()).GetFirstMoveTaken() = False Then
                            If EvaluateGameState(Piece.White) = False Then

                                If CheckIfSquareIsAttacked(5, Piece.White) = False Then
                                    If CheckIfSquareIsAttacked(6, Piece.White) = False Then


                                        moveList.Add(New Movement With {
                                           .StartSquare = startSquare,
                                           .TargetSquare = startSquare + 2
                                        })
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If boardSquares(0).GetPieceType() = Piece.Rook Then            'Checks the conditions for kingside castling
                        If boardSquares(3).GetPieceType() = Piece.None And boardSquares(2).GetPieceType() = Piece.None And boardSquares(1).GetPieceType() = Piece.None And boardSquares(0).GetPieceColour() = kings(boardSquares(startSquare).GetPieceIndex()).GetPieceColour And rooks(boardSquares(0).GetPieceIndex()).GetFirstMoveTaken() = False Then
                            If EvaluateGameState(Piece.White) = False Then
                                If CheckIfSquareIsAttacked(3, Piece.White) = False Then
                                    If CheckIfSquareIsAttacked(2, Piece.White) = False Then
                                        moveList.Add(New Movement With {
                                   .StartSquare = startSquare,
                                   .TargetSquare = startSquare - 2
                                })
                                    End If
                                End If
                            End If
                        End If
                    End If

                ElseIf startSquare = 60 Then               'Checks the castling conditions for the black king
                    If boardSquares(63).GetPieceType() = Piece.Rook Then        'Checks the conditions for kingside castling
                        If boardSquares(61).GetPieceType() = Piece.None And boardSquares(62).GetPieceType() = Piece.None And boardSquares(63).GetPieceColour() = kings(boardSquares(startSquare).GetPieceIndex()).GetPieceColour And rooks(boardSquares(63).GetPieceIndex()).GetFirstMoveTaken() = False Then
                            If EvaluateGameState(Piece.Black) = False Then
                                If CheckIfSquareIsAttacked(61, Piece.Black) = False Then
                                    If CheckIfSquareIsAttacked(62, Piece.Black) = False Then
                                        moveList.Add(New Movement With {
                                            .StartSquare = startSquare,
                                            .TargetSquare = startSquare + 2
                                         })
                                    End If
                                End If
                            End If
                        End If
                    End If
                    If boardSquares(56).GetPieceType() = Piece.Rook Then        'Checks the conditions for queenside castling
                        If boardSquares(59).GetPieceType() = Piece.None And boardSquares(58).GetPieceType() = Piece.None And boardSquares(57).GetPieceType() = Piece.None And boardSquares(56).GetPieceColour() = kings(boardSquares(startSquare).GetPieceIndex()).GetPieceColour And rooks(boardSquares(56).GetPieceIndex()).GetFirstMoveTaken() = False Then
                            If EvaluateGameState(Piece.Black) = False Then
                                If CheckIfSquareIsAttacked(59, Piece.Black) = False Then
                                    If CheckIfSquareIsAttacked(58, Piece.Black) = False Then
                                        moveList.Add(New Movement With {
                                           .StartSquare = startSquare,
                                           .TargetSquare = startSquare - 2
                                        })
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If

        For Each direction In movementArray     'Loops through all the directions 

            Dim directionInterest1 As Integer = 0
            Dim directionInterest2 As Integer = Nothing
            Select Case direction
                Case 8                      'These are the direction indexes for the king movements
                    directionInterest1 = boardSquares(startSquare).GetNorth()
                Case -8
                    directionInterest1 = boardSquares(startSquare).GetSouth()
                Case -1
                    directionInterest1 = boardSquares(startSquare).GetWest()
                Case 1
                    directionInterest1 = boardSquares(startSquare).GetEast()
                Case 7
                    directionInterest1 = boardSquares(startSquare).GetNorthWest()
                Case -7
                    directionInterest1 = boardSquares(startSquare).GetSouthEast()
                Case 9
                    directionInterest1 = boardSquares(startSquare).GetNorthEast()
                Case -9
                    directionInterest1 = boardSquares(startSquare).GetSouthWest()

                Case 6                         'These are the direction indexes for the knight movements
                    directionInterest1 = boardSquares(startSquare).GetWest()        'First interest = 2 space movement
                    directionInterest2 = boardSquares(startSquare).GetNorth()       'Second interest = 1 space movement
                Case 10
                    directionInterest1 = boardSquares(startSquare).GetEast()
                    directionInterest2 = boardSquares(startSquare).GetNorth()
                Case 15
                    directionInterest1 = boardSquares(startSquare).GetNorth()
                    directionInterest2 = boardSquares(startSquare).GetWest()
                Case 17
                    directionInterest1 = boardSquares(startSquare).GetNorth()
                    directionInterest2 = boardSquares(startSquare).GetEast()
                Case -6
                    directionInterest1 = boardSquares(startSquare).GetEast()
                    directionInterest2 = boardSquares(startSquare).GetSouth()
                Case -10
                    directionInterest1 = boardSquares(startSquare).GetWest()
                    directionInterest2 = boardSquares(startSquare).GetSouth()
                Case -15
                    directionInterest1 = boardSquares(startSquare).GetSouth()
                    directionInterest2 = boardSquares(startSquare).GetEast()
                Case -17
                    directionInterest1 = boardSquares(startSquare).GetSouth()
                    directionInterest2 = boardSquares(startSquare).GetWest()
            End Select

            Dim targetSquare As Integer = startSquare + direction
            If IsKing = True Then       'Generate moves for kings
                If directionInterest1 >= 1 Then
                    If boardSquares(targetSquare).GetPieceType = Piece.None Then
                        moveList.Add(New Movement With {               'Add the move to the list
                            .StartSquare = startSquare,
                            .TargetSquare = targetSquare
                         })
                    Else
                        If boardSquares(startSquare).GetPieceColour() <> boardSquares(targetSquare).GetPieceColour() Then   'Check there is no friendly piece on target square
                            moveList.Add(New Movement With {            'Add the move to the list
                                .StartSquare = startSquare,
                                .TargetSquare = targetSquare
                             })
                        Else
                            Continue For
                        End If
                    End If
                End If
            Else            'Generates moves for knights
                If directionInterest1 >= 2 And directionInterest2 >= 1 Then
                    If boardSquares(targetSquare).GetPieceType = Piece.None Then
                        moveList.Add(New Movement With {                'Add the move to the list
                            .StartSquare = startSquare,
                            .TargetSquare = targetSquare
                         })
                    Else
                        If boardSquares(startSquare).GetPieceColour() <> boardSquares(targetSquare).GetPieceColour() Then   'Check there is no friendly piece on target square
                            moveList.Add(New Movement With {             'Add the move to the list
                                .StartSquare = startSquare,
                                .TargetSquare = targetSquare
                             })
                        Else
                            Continue For
                        End If
                    End If
                End If
            End If
        Next
    End Sub
    Function MoveGenerationTest(depth)          'Traverses the move generation tree
        If depth = 0 Then           'Base case - Depth is zero
            Return 1
        End If
        moves = New List(Of Movement)       'Generates all possible moves
        moves = GenerateMoves()

        Dim numPositions As Integer = 0
        For Each generatedMove In moves       'Loops through all the moves
            Dim capture As Boolean = False
            capture = NewMakeMove(generatedMove)      'Plays the move
            ChessGame.GetSettings.IncrementPlayerTurn()
            numPositions += MoveGenerationTest(depth - 1)           'Recursive function call
            ChessGame.GetSettings.IncrementPlayerTurn()
            NewUnmakeMove(generatedMove, capture)     'Unplays the move
        Next
        Return numPositions
    End Function
#End Region

#Region "Move Functions"
    Sub MakeMove(move As Board.Movement, capture As Boolean)
        Dim pieceType As Integer = boardSquares(move.StartSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(move.StartSquare).GetPieceIndex()

        Dim positionX As Integer = ((move.TargetSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (move.TargetSquare \ 8)) * 90) + 11

        Select Case pieceType
            Case Piece.Pawn
                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(move.TargetSquare)

                If pawns(pieceIndex).GetFirstMoveTaken = False Then
                    pawns(pieceIndex).UpdateFirstMoveTaken()
                    pawns(pieceIndex).SetFirstMoveNumber(GetMoveCount())
                End If

            Case Piece.Knight
                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.Bishop
                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.Rook
                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(move.TargetSquare)

                If rooks(pieceIndex).GetFirstMoveTaken = False Then
                    rooks(pieceIndex).UpdateFirstMoveTaken()
                    rooks(pieceIndex).SetFirstMoveNumber(GetMoveCount())
                End If

            Case Piece.Queen
                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.King
                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(move.TargetSquare)

        End Select

        boardSquares(move.TargetSquare).SetPieceDetails(boardSquares(move.StartSquare).GetPieceType(), boardSquares(move.StartSquare).GetPieceColour(), boardSquares(move.StartSquare).GetPieceIndex())
        boardSquares(move.StartSquare).ResetPieceDetails()
        AddNewMove(move, capture)
        ChessGame.RefreshBoard()
    End Sub
    Sub MakeMoveWithoutRefresh(move As Board.Movement)
        Dim pieceType As Integer = boardSquares(move.StartSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(move.StartSquare).GetPieceIndex()

        Dim positionX As Integer = ((move.TargetSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (move.TargetSquare \ 8)) * 90) + 11

        Select Case pieceType
            Case Piece.Pawn
                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.Knight
                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.Bishop
                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.Rook
                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.Queen
                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(move.TargetSquare)

            Case Piece.King
                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(move.TargetSquare)

        End Select
        boardSquares(move.TargetSquare).SetPieceDetails(boardSquares(move.StartSquare).GetPieceType(), boardSquares(move.StartSquare).GetPieceColour(), boardSquares(move.StartSquare).GetPieceIndex())
        boardSquares(move.StartSquare).ResetPieceDetails()
    End Sub
    Sub UnmakeMove(move)
        Dim pieceType As Integer = boardSquares(move.TargetSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(move.TargetSquare).GetPieceIndex()

        Dim positionX As Integer = ((move.StartSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (move.StartSquare \ 8)) * 90) + 11
        Select Case pieceType
            Case Piece.Pawn
                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Knight
                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Bishop
                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Rook
                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Queen
                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.King
                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(move.StartSquare)

        End Select
        boardSquares(move.StartSquare).SetPieceDetails(boardSquares(move.TargetSquare).GetPieceType(), boardSquares(move.TargetSquare).GetPieceColour(), boardSquares(move.TargetSquare).GetPieceIndex())
        boardSquares(move.TargetSquare).ResetPieceDetails()
        RemoveLastMove()
        ChessGame.RefreshBoard()
    End Sub
    Sub UnmakeMoveWithoutRefresh(move)
        Dim pieceType As Integer = boardSquares(move.TargetSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(move.TargetSquare).GetPieceIndex()

        Dim positionX As Integer = ((move.StartSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (move.StartSquare \ 8)) * 90) + 11
        Select Case pieceType
            Case Piece.Pawn
                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Knight
                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Bishop
                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Rook
                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.Queen
                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(move.StartSquare)

            Case Piece.King
                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(move.StartSquare)

        End Select
        boardSquares(move.StartSquare).SetPieceDetails(boardSquares(move.TargetSquare).GetPieceType(), boardSquares(move.TargetSquare).GetPieceColour(), boardSquares(move.TargetSquare).GetPieceIndex())
        boardSquares(move.TargetSquare).ResetPieceDetails()
    End Sub

    Function NewMakeMove(Move As Movement)      'Makes user moves
        Dim isCapture As Boolean = CheckIfCapture(Move)
        If isCapture = True Then
            DisposeCapturedPiece(Move)
        End If

        Dim pieceType As Integer = boardSquares(Move.StartSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(Move.StartSquare).GetPieceIndex()

        Dim positionX As Integer = ((Move.TargetSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (Move.TargetSquare \ 8)) * 90) + 11

        Select Case pieceType
            Case Piece.Pawn
                If Move.StartSquare - Move.TargetSquare = 16 Or Move.StartSquare - Move.TargetSquare = -16 Then

                    EnPassentPawn = pawns(boardSquares(Move.StartSquare).GetPieceIndex())           'Sets enpassent pawn
                    pawns(boardSquares(Move.StartSquare).GetPieceIndex()).SetEnpassentMoveNumber(GetMoveCount)
                    'Checks for black promotion
                ElseIf boardSquares(Move.StartSquare).GetPieceColour() = Piece.Black And boardSquares(Move.TargetSquare).GetRank() = 0 And pawns(boardSquares(Move.StartSquare).GetPieceIndex()).GetPromotionTag() = 0 Then
                    PromoteAIPawn(move:=Move, colour:=Piece.Black)          'Promotes the AI pawn
                    'Checks for white promotion
                ElseIf boardSquares(Move.StartSquare).GetPieceColour() = Piece.White And boardSquares(Move.TargetSquare).GetRank() = 7 And pawns(boardSquares(Move.StartSquare).GetPieceIndex()).GetPromotionTag() = 0 Then
                    PromoteAIPawn(move:=Move, colour:=Piece.White)           'Promotes the AI pawn
                Else
                    If EnPassentPawn IsNot Nothing Then         'Checks if en passent move
                        If (EnPassentPawn.GetSquareNumber() = Move.StartSquare + 1 Or EnPassentPawn.GetSquareNumber() = Move.StartSquare - 1) Then
                            If Move.TargetSquare = EnPassentPawn.GetSquareNumber() + 8 Or Move.TargetSquare = EnPassentPawn.GetSquareNumber() - 8 Then
                                EnPassentMove = True
                            End If
                        End If
                    End If
                End If
                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(Move.TargetSquare)
                If pawns(pieceIndex).GetFirstMoveTaken() = False Then
                    pawns(pieceIndex).UpdateFirstMoveTaken()
                    pawns(pieceIndex).SetFirstMoveNumber(GetMoveCount())
                End If
                If EnPassentMove = True Then
                    DisposeEnPassentPawn()
                    SetEnpassentPawn(Nothing)
                ElseIf EnPassentPawn IsNot Nothing Then
                    If EnPassentPawn.GetPieceColour() <> ChessGame.GetSettings.GetColourToMove() Then
                        SetEnpassentPawn(Nothing)
                    End If
                End If
                                'checks for black promotion

            Case Piece.Knight

                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(Move.TargetSquare)

                EnPassentPawn = Nothing
            Case Piece.Bishop

                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(Move.TargetSquare)

                EnPassentPawn = Nothing
            Case Piece.Rook

                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(Move.TargetSquare)

                If rooks(pieceIndex).GetFirstMoveTaken() = False Then
                    rooks(pieceIndex).UpdateFirstMoveTaken()
                    rooks(pieceIndex).SetFirstMoveNumber(GetMoveCount())
                End If

                'rooks(pieceIndex).IncreaseMoveCount()
                EnPassentPawn = Nothing
            Case Piece.Queen

                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(Move.TargetSquare)

                EnPassentPawn = Nothing
            Case Piece.King

                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(Move.TargetSquare)
                'kings(pieceIndex).IncreaseMoveCount()
                EnPassentPawn = Nothing
                'for castle moves
                If kings(pieceIndex).GetFirstMoveTaken() = False Then
                    If Move.StartSquare = 4 And Move.TargetSquare = 6 Then
                        MakeRookMove(7, 5)

                    ElseIf Move.StartSquare = 4 And Move.TargetSquare = 2 Then
                        MakeRookMove(0, 3)

                    ElseIf Move.StartSquare = 60 And Move.TargetSquare = 62 Then
                        MakeRookMove(63, 61)

                    ElseIf Move.StartSquare = 60 And Move.TargetSquare = 58 Then
                        MakeRookMove(56, 59)

                    End If

                    kings(pieceIndex).UpdateFirstMoveTaken()
                    kings(pieceIndex).SetFirstMoveNumber(GetMoveCount())
                End If

        End Select

        boardSquares(Move.TargetSquare).SetPieceDetails(boardSquares(Move.StartSquare).GetPieceType(), boardSquares(Move.StartSquare).GetPieceColour(), boardSquares(Move.StartSquare).GetPieceIndex())

        boardSquares(Move.StartSquare).ResetPieceDetails()

        FillEmptyBitboards()
        GenerateBitboards()

        ChessGame.RefreshBoard()
        Dim capture As Boolean = False
        If isCapture = True Or EnPassentMove = True Then
            capture = True
            EnPassentMove = False
        End If

        AddNewMove(Move, capture)

        Return capture
    End Function
    Function NewMakeMoveWithoutRefresh(Move As Movement)

        Dim isCapture As Boolean = CheckIfCapture(Move)
        If isCapture = True Then
            DisposeCapturedPieceWithoutRefresh(Move)
        End If

        Dim pieceType As Integer = boardSquares(Move.StartSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(Move.StartSquare).GetPieceIndex()

        Dim positionX As Integer = ((Move.TargetSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (Move.TargetSquare \ 8)) * 90) + 11

        Select Case pieceType
            Case Piece.Pawn

                'sets enpassent pawn
                If Move.StartSquare - Move.TargetSquare = 16 Or Move.StartSquare - Move.TargetSquare = -16 Then

                    EnPassentPawn = pawns(boardSquares(Move.StartSquare).GetPieceIndex())
                    pawns(boardSquares(Move.StartSquare).GetPieceIndex()).SetEnpassentMoveNumber(GetMoveCount)

                    'checks if enpassent move


                    'checks for black promotion
                ElseIf boardSquares(Move.TargetSquare).GetPieceColour() = Piece.Black And boardSquares(Move.TargetSquare).GetRank() = 0 And pawns(boardSquares(Move.TargetSquare).GetPieceIndex()).GetPromotionTag() = 0 Then
                    'by default the ai will promote to queen to save time
                    PromoteAIPawnWithoutRefresh(move:=Move, colour:=Piece.Black)

                    'checks for white promotion
                ElseIf boardSquares(Move.TargetSquare).GetPieceColour() = Piece.White And boardSquares(Move.TargetSquare).GetRank() = 7 And pawns(boardSquares(Move.TargetSquare).GetPieceIndex()).GetPromotionTag() = 0 Then
                    'by default the ai will promote to queen to save time
                    PromoteAIPawnWithoutRefresh(move:=Move, colour:=Piece.White)

                Else
                    If EnPassentPawn IsNot Nothing Then
                        If EnPassentPawn.GetSquareNumber() = Move.StartSquare + 1 Or EnPassentPawn.GetSquareNumber() = Move.StartSquare - 1 Then
                            If Move.TargetSquare = EnPassentPawn.GetSquareNumber() + 8 Or Move.TargetSquare = EnPassentPawn.GetSquareNumber() - 8 Then
                                EnPassentMove = True

                            End If
                        End If
                    End If
                End If
                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(Move.TargetSquare)

                If pawns(pieceIndex).GetFirstMoveTaken = False Then
                    pawns(pieceIndex).UpdateFirstMoveTaken()
                    pawns(pieceIndex).SetFirstMoveNumber(GetMoveCount())
                End If



                If EnPassentMove = True And EnPassentPawn IsNot Nothing Then
                    DisposeEnPassentPawnWithoutRefresh()
                    SetEnpassentPawn(Nothing)
                ElseIf EnPassentPawn IsNot Nothing Then
                    If EnPassentPawn.GetPieceColour() <> ChessGame.GetSettings.GetColourToMove() Then
                        SetEnpassentPawn(Nothing)
                    End If
                End If

            Case Piece.Knight

                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(Move.TargetSquare)
                EnPassentPawn = Nothing

            Case Piece.Bishop

                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(Move.TargetSquare)
                EnPassentPawn = Nothing

            Case Piece.Rook

                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(Move.TargetSquare)
                If rooks(pieceIndex).GetFirstMoveTaken() = False Then
                    rooks(pieceIndex).UpdateFirstMoveTaken()
                    rooks(pieceIndex).SetFirstMoveNumber(GetMoveCount())

                End If

                EnPassentPawn = Nothing

            Case Piece.Queen

                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(Move.TargetSquare)
                EnPassentPawn = Nothing

            Case Piece.King

                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(Move.TargetSquare)

                EnPassentPawn = Nothing
                'for castle moves
                If kings(pieceIndex).GetFirstMoveTaken() = False Then

                    If Move.StartSquare = 4 And Move.TargetSquare = 6 Then
                        MakeRookMoveWithoutRefresh(7, 5)

                    ElseIf Move.StartSquare = 4 And Move.TargetSquare = 2 Then
                        MakeRookMoveWithoutRefresh(0, 3)

                    ElseIf Move.StartSquare = 60 And Move.TargetSquare = 62 Then
                        MakeRookMoveWithoutRefresh(63, 61)

                    ElseIf Move.StartSquare = 60 And Move.TargetSquare = 58 Then
                        MakeRookMoveWithoutRefresh(56, 59)

                    End If

                    kings(pieceIndex).UpdateFirstMoveTaken()
                    kings(pieceIndex).SetFirstMoveNumber(GetMoveCount())

                End If

        End Select

        boardSquares(Move.TargetSquare).SetPieceDetails(boardSquares(Move.StartSquare).GetPieceType(), boardSquares(Move.StartSquare).GetPieceColour(), boardSquares(Move.StartSquare).GetPieceIndex())

        boardSquares(Move.StartSquare).ResetPieceDetails()

        FillEmptyBitboards()
        GenerateBitboards()

        Dim capture As Boolean = False
        If isCapture = True Or EnPassentMove = True Then
            capture = True
            EnPassentMove = False

        End If

        AddNewMove(Move, capture)
        Return capture

    End Function

    Sub NewUnmakeMove(Move As Board.Movement, capture As Boolean)

        'TO DO: Make sure FirstMoveTaken variables are set to False at appropriate times

        Dim pieceType As Integer = boardSquares(Move.TargetSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(Move.TargetSquare).GetPieceIndex()

        Dim positionX As Integer = ((Move.StartSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (Move.StartSquare \ 8)) * 90) + 11
        Select Case pieceType
            Case Piece.Pawn

                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(Move.StartSquare)
                'pawns(pieceIndex).DecreaseMoveCount()
                If (pawns(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
                    pawns(pieceIndex).ResetFirstMoveTaken()
                    pawns(pieceIndex).ResetFirstMoveNumber()
                End If
                'reverse pawn promotions - make sure it is on the actual move of promotion - not any time when piece on promotion square
                If pawns(pieceIndex).GetPromotionTag <> 0 Then
                    If (pawns(pieceIndex).GetPromotionMoveNumber()) = GetMoveCount() Then
                        ReversePawnPromotion(pawns(pieceIndex))
                    End If
                End If
                pieceIndex = boardSquares(GetLastMove().TargetSquare).GetPieceIndex()
                If pawns(pieceIndex).GetEnpassentMoveNumber >= GetMoveCount() - 1 Then
                    pawns(pieceIndex).ResetEnpassentMoveNumber()
                End If

            Case Piece.Knight

                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(Move.StartSquare)

            Case Piece.Bishop

                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(Move.StartSquare)

            Case Piece.Rook

                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(Move.StartSquare)

                If (rooks(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
                    rooks(pieceIndex).ResetFirstMoveTaken()
                    rooks(pieceIndex).ResetFirstMoveNumber()
                End If
            Case Piece.Queen

                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(Move.StartSquare)

            Case Piece.King

                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(Move.StartSquare)
                'check if castle move

                If Move.StartSquare = 4 And Move.TargetSquare = 6 Then
                    UnmakeRookMove(7, 5)

                ElseIf Move.StartSquare = 4 And Move.TargetSquare = 2 Then
                    UnmakeRookMove(0, 3)

                ElseIf Move.StartSquare = 60 And Move.TargetSquare = 62 Then
                    UnmakeRookMove(63, 61)

                ElseIf Move.StartSquare = 60 And Move.TargetSquare = 58 Then
                    UnmakeRookMove(56, 59)

                End If

                If (kings(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
                    kings(pieceIndex).ResetFirstMoveTaken()
                    kings(pieceIndex).ResetFirstMoveNumber()
                End If

        End Select

        boardSquares(Move.StartSquare).SetPieceDetails(boardSquares(Move.TargetSquare).GetPieceType(), boardSquares(Move.TargetSquare).GetPieceColour(), boardSquares(Move.TargetSquare).GetPieceIndex())
        boardSquares(Move.TargetSquare).ResetPieceDetails()

        If capture = True Then
            UndisposeCapturedPiece()
        End If

        RemoveLastMove()

        If GetMoveCount() > 0 Then
            If boardSquares(GetLastMove().TargetSquare).GetPieceType() = Piece.Pawn Then
                pieceIndex = boardSquares(GetLastMove().TargetSquare).GetPieceIndex()
                If pawns(pieceIndex).GetEnpassentMoveNumber <> Nothing Then
                    If (GetMoveCount() - 1) = pawns(pieceIndex).GetEnpassentMoveNumber() Then
                        EnPassentPawn = pawns(pieceIndex)
                    End If
                End If
            End If
        ElseIf GetMoveCount() = 0 Then
            SetEnpassentPawn(Nothing)
        End If

        ChessGame.RefreshBoard()

    End Sub
    Sub NewUnmakeMoveWithoutRefresh(Move As Board.Movement, capture As Boolean)

        Dim pieceType As Integer = boardSquares(Move.TargetSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(Move.TargetSquare).GetPieceIndex()

        Dim positionX As Integer = ((Move.StartSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (Move.StartSquare \ 8)) * 90) + 11
        Select Case pieceType
            Case Piece.Pawn

                pawns(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                pawns(pieceIndex).SetSquareNumber(Move.StartSquare)

                If (pawns(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
                    pawns(pieceIndex).ResetFirstMoveTaken()
                    pawns(pieceIndex).ResetFirstMoveNumber()
                End If

                If pawns(pieceIndex).GetPromotionTag <> 0 Then
                    If (pawns(pieceIndex).GetPromotionMoveNumber()) = GetMoveCount() Then
                        ReversePawnPromotionWithoutRefresh(pawns(pieceIndex))
                    End If
                End If

                pieceIndex = boardSquares(GetLastMove().TargetSquare).GetPieceIndex()
                If pawns(pieceIndex).GetEnpassentMoveNumber >= GetMoveCount() - 1 Then
                    pawns(pieceIndex).ResetEnpassentMoveNumber()
                End If

            Case Piece.Knight

                knights(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                knights(pieceIndex).SetSquareNumber(Move.StartSquare)

            Case Piece.Bishop

                bishops(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                bishops(pieceIndex).SetSquareNumber(Move.StartSquare)

            Case Piece.Rook

                rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                rooks(pieceIndex).SetSquareNumber(Move.StartSquare)
                If (rooks(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
                    rooks(pieceIndex).ResetFirstMoveTaken()
                    rooks(pieceIndex).ResetFirstMoveNumber()
                End If

            Case Piece.Queen

                queens(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                queens(pieceIndex).SetSquareNumber(Move.StartSquare)

            Case Piece.King

                kings(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
                kings(pieceIndex).SetSquareNumber(Move.StartSquare)

                'check if castle move

                If Move.StartSquare = 4 And Move.TargetSquare = 6 Then
                    UnmakeRookMoveWithoutRefresh(7, 5)

                ElseIf Move.StartSquare = 4 And Move.TargetSquare = 2 Then
                    UnmakeRookMoveWithoutRefresh(0, 3)

                ElseIf Move.StartSquare = 60 And Move.TargetSquare = 62 Then
                    UnmakeRookMoveWithoutRefresh(63, 61)

                ElseIf Move.StartSquare = 60 And Move.TargetSquare = 58 Then
                    UnmakeRookMoveWithoutRefresh(56, 59)

                End If

                If (kings(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
                    kings(pieceIndex).ResetFirstMoveTaken()
                    kings(pieceIndex).ResetFirstMoveNumber()
                End If

        End Select
        boardSquares(Move.StartSquare).SetPieceDetails(boardSquares(Move.TargetSquare).GetPieceType(), boardSquares(Move.TargetSquare).GetPieceColour(), boardSquares(Move.TargetSquare).GetPieceIndex())
        boardSquares(Move.TargetSquare).ResetPieceDetails()

        If capture = True Then
            UndisposeCapturedPieceWithoutRefresh()
        End If

        RemoveLastMove()


        If GetMoveCount() > 0 Then

            If boardSquares(GetLastMove().TargetSquare).GetPieceType() = Piece.Pawn Then
                pieceIndex = boardSquares(GetLastMove().TargetSquare).GetPieceIndex()
                If pawns(pieceIndex).GetEnpassentMoveNumber <> Nothing Then
                    If (GetMoveCount() - 1) = pawns(pieceIndex).GetEnpassentMoveNumber() Then
                        EnPassentPawn = pawns(pieceIndex)

                    End If
                End If

            End If
        ElseIf GetMoveCount = 0 Then
            SetEnpassentPawn(Nothing)
        End If

    End Sub

    Sub MakeRookMove(startSquare, targetSquare)

        Dim pieceIndex As Integer = boardSquares(startSquare).GetPieceIndex()

        Dim positionX As Integer = ((targetSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (targetSquare \ 8)) * 90) + 11

        If rooks(pieceIndex).GetFirstMoveTaken = False Then
            rooks(pieceIndex).UpdateFirstMoveTaken()
            rooks(pieceIndex).SetFirstMoveNumber(GetMoveCount())

        End If

        rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
        rooks(pieceIndex).SetSquareNumber(targetSquare)
        'rooks(pieceIndex).IncreaseMoveCount()

        boardSquares(targetSquare).SetPieceDetails(boardSquares(startSquare).GetPieceType(), boardSquares(startSquare).GetPieceColour(), boardSquares(startSquare).GetPieceIndex())
        boardSquares(startSquare).ResetPieceDetails()

        ChessGame.RefreshBoard()

    End Sub
    Sub MakeRookMoveWithoutRefresh(startSquare, targetSquare)

        Dim pieceIndex As Integer = boardSquares(startSquare).GetPieceIndex()
        Dim positionX As Integer = ((targetSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (targetSquare \ 8)) * 90) + 11

        If rooks(pieceIndex).GetFirstMoveTaken = False Then
            rooks(pieceIndex).UpdateFirstMoveTaken()
            rooks(pieceIndex).SetFirstMoveNumber(GetMoveCount())

        End If

        rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
        rooks(pieceIndex).SetSquareNumber(targetSquare)

        boardSquares(targetSquare).SetPieceDetails(boardSquares(startSquare).GetPieceType(), boardSquares(startSquare).GetPieceColour(), boardSquares(startSquare).GetPieceIndex())
        boardSquares(startSquare).ResetPieceDetails()
    End Sub
    Sub UnmakeRookMove(startSquare, targetSquare)

        Dim pieceIndex As Integer = boardSquares(targetSquare).GetPieceIndex()

        Dim positionX As Integer = ((startSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (startSquare \ 8)) * 90) + 11
        rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
        rooks(pieceIndex).SetSquareNumber(startSquare)
        'rooks(pieceIndex).DecreaseMoveCount()

        If (rooks(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
            rooks(pieceIndex).ResetFirstMoveTaken()
        End If

        boardSquares(startSquare).SetPieceDetails(boardSquares(targetSquare).GetPieceType(), boardSquares(targetSquare).GetPieceColour(), boardSquares(targetSquare).GetPieceIndex())
        boardSquares(targetSquare).ResetPieceDetails()

        ChessGame.RefreshBoard()

    End Sub
    Sub UnmakeRookMoveWithoutRefresh(startSquare, targetSquare)

        Dim pieceIndex As Integer = boardSquares(targetSquare).GetPieceIndex()

        Dim positionX As Integer = ((startSquare Mod 8) * 90) + 11
        Dim positionY As Integer = ((7 - (startSquare \ 8)) * 90) + 11
        rooks(pieceIndex).pieceRectangle.Location = New Point(positionX, positionY)
        rooks(pieceIndex).SetSquareNumber(startSquare)
        'rooks(pieceIndex).DecreaseMoveCount()

        If (rooks(pieceIndex).GetFirstMoveNumber() + 1) = GetMoveCount() Then
            rooks(pieceIndex).ResetFirstMoveTaken()
        End If

        boardSquares(startSquare).SetPieceDetails(boardSquares(targetSquare).GetPieceType(), boardSquares(targetSquare).GetPieceColour(), boardSquares(targetSquare).GetPieceIndex())
        boardSquares(targetSquare).ResetPieceDetails()

    End Sub

#End Region

#Region "Player Move Functions"
    'Verifies a player move
    Function ValidatePawnMove(pieceObject As Pawn, move As Movement)
        Dim legalMovement As Boolean = False
        Dim MovementArray = pieceObject.GetMovementArray()
        Dim movementUnits As Integer = move.TargetSquare - move.StartSquare
        Dim directionInterest As Integer

        For Each number In MovementArray
            If pieceObject.GetPieceColour() = Piece.Black Then      'Makes move index negative if pawn is 
                number = -number                                    'black, as travels southwards
            End If

            If movementUnits = number Then          'The number in the array matches the movement units
                Select Case number              'Sets the direction interest
                    Case 8
                        directionInterest = boardSquares(move.StartSquare).GetNorth
                        If directionInterest < 1 Then
                            legalMovement = False
                            Return legalMovement
                        End If
                    Case 16
                        directionInterest = boardSquares(move.StartSquare).GetNorth
                        If directionInterest < 2 Then
                            legalMovement = False
                            Return legalMovement
                        End If
                    Case -8
                        directionInterest = boardSquares(move.StartSquare).GetSouth
                        If directionInterest < 1 Then
                            legalMovement = False
                            Return legalMovement
                        End If
                    Case -16
                        directionInterest = boardSquares(move.StartSquare).GetSouth
                        If directionInterest < 2 Then
                            legalMovement = False
                            Return legalMovement
                        End If
                End Select

                If boardSquares(move.TargetSquare).GetPieceColour() = boardSquares(move.StartSquare).GetPieceColour() Then  'If friendly piece on target square
                    legalMovement = False       'Move is not valid
                    Return legalMovement
                End If

                If Math.Abs(number) = 16 And pieceObject.GetFirstMoveTaken() = False Then       'If double pawn push move
                    If boardSquares(move.TargetSquare).GetPieceType <> Piece.None Then
                        Continue For                    'Piece in the way, try next move
                    Else
                        If number = 16 Then
                            If boardSquares(move.TargetSquare - 8).GetPieceType <> Piece.None Then
                                Continue For           'Piece on the first push square, try next move
                            Else
                                legalMovement = True
                                EnPassentPawn = pieceObject
                                pawns(pieceObject.GetPieceIndex).SetEnpassentMoveNumber(GetMoveCount())

                                Return legalMovement
                            End If
                        ElseIf number = -16 Then
                            If boardSquares(move.TargetSquare + 8).GetPieceType <> Piece.None Then
                                Continue For           'Piece on the first push square, try next move
                            Else
                                legalMovement = True
                                EnPassentPawn = pieceObject
                                pawns(pieceObject.GetPieceIndex).SetEnpassentMoveNumber(GetMoveCount())

                                Return legalMovement
                            End If
                        End If
                    End If
                ElseIf Math.Abs(number) <> 16 Then      
                    If boardSquares(move.TargetSquare).GetPieceType <> Piece.None Then
                        Continue For                   'Piece on target square, try next move direction
                    Else
                        legalMovement = True        'No piece on target square, move is valid 
                        Return legalMovement
                    End If
                    Exit For
                End If
            End If
        Next

        'These conditions below check it is a pawn capture move
        If legalMovement = False Then
            Dim CaptureArray = pieceObject.GetCaptureArray()

            For Each number In CaptureArray
                If pieceObject.GetPieceColour() = Piece.Black Then
                    number = -number
                End If

                If movementUnits = number Then
                    Select Case number                  'Selects capture move direction interest
                        Case 9
                            directionInterest = boardSquares(move.StartSquare).GetNorthEast()
                        Case 7
                            directionInterest = boardSquares(move.StartSquare).GetNorthWest()
                        Case -9
                            directionInterest = boardSquares(move.StartSquare).GetSouthWest()
                        Case -7
                            directionInterest = boardSquares(move.StartSquare).GetSouthEast()
                    End Select

                    If boardSquares(move.TargetSquare).GetPieceColour() <> Piece.None And boardSquares(move.TargetSquare).GetPieceColour() <> pieceObject.GetPieceColour() And directionInterest >= 1 Then
                        legalMovement = True                'If enemy piece on target square, move is valid
                        Return legalMovement
                    ElseIf EnPassentPawn IsNot Nothing Then
                        If EnPassentPawn.GetSquareNumber() = move.StartSquare + 1 Or EnPassentPawn.GetSquareNumber() = move.StartSquare - 1 Then        'Checks for en passent capture move
                            If move.TargetSquare = EnPassentPawn.GetSquareNumber() + 8 Or move.TargetSquare = EnPassentPawn.GetSquareNumber() - 8 Then
                                legalMovement = True
                                EnPassentMove = True
                                Return legalMovement
                            End If
                        End If
                    End If
                End If
            Next
        End If
        Return legalMovement            'Return if movement is valid
    End Function
    Function ValidateSlidingMove(move As Movement)

        Dim legalMovement As Boolean = False
        Dim pieceType As Integer = boardSquares(move.StartSquare).GetPieceType
        Dim pieceIndex As Integer = boardSquares(move.StartSquare).GetPieceIndex
        Dim MovementArray As Integer()

        Select Case pieceType           'Selects the appropriate piece type
            Case Piece.Bishop
                MovementArray = bishops(pieceIndex).GetMovementArray()      'Selects appropriate movement array

            Case Piece.Rook
                MovementArray = rooks(pieceIndex).GetMovementArray()

            Case Piece.Queen
                MovementArray = queens(pieceIndex).GetMovementArray()
        End Select

        Dim stepUnit As Integer
        Dim movementUnits As Integer = move.TargetSquare - move.StartSquare
        Dim negativeMovement As Boolean = False
        If movementUnits < 0 Then
            negativeMovement = True
        End If

        For Each number In MovementArray        'Loops through movement directions

            If movementUnits Mod number = 0 Then        'If movement units is a factor of direction index
                If number = 1 Or number = -1 Then       'Checks if horizontal movement
                    Dim rank1 As Integer = move.StartSquare \ 8
                    Dim rank2 As Integer = move.TargetSquare \ 8

                    If rank1 = rank2 Then             'Is a horizontal movement
                        Dim directionInterest As Integer
                        If negativeMovement = True Then     'Selects appropriate direction interest
                            stepUnit = -1
                            directionInterest = boardSquares(move.StartSquare).GetWest()
                        Else
                            stepUnit = 1
                            directionInterest = boardSquares(move.StartSquare).GetEast()
                        End If
                        If directionInterest < movementUnits / number Then
                            legalMovement = False           'If move is not on board, move is not valid
                            Return legalMovement
                        End If

                        For movements = 0 To ((movementUnits / number) - stepUnit) Step stepUnit         'Check if the piece moves through other pieces
                            If boardSquares(move.StartSquare + movements + stepUnit).GetPieceType() = Piece.None Then
                                legalMovement = True        'No piece on square, procedes to next square
                            Else
                                'There is a piece on square
                                If movements = ((movementUnits / number) - stepUnit) And boardSquares(move.StartSquare + movements + stepUnit).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                    legalMovement = True        'Piece is an enemy piece, capture move is valid
                                    Return legalMovement
                                Else
                                    legalMovement = False       'Pice is a friendly piece, capture move is not valid
                                    Return legalMovement
                                End If
                            End If
                        Next
                        Return legalMovement
                    Else
                        Continue For          'Not horizontal movement, different number applies for direction
                    End If
                Else
                    'Found correct movement direction for the move
                    If negativeMovement = True Then
                        If number > 0 Then
                            Continue For
                        End If
                    End If

                    Dim directionInterest As Integer
                    Select Case number              'Selects appropriate direction interest
                        Case 7
                            directionInterest = boardSquares(move.StartSquare).GetNorthWest()
                        Case 8
                            directionInterest = boardSquares(move.StartSquare).GetNorth()
                        Case 9
                            directionInterest = boardSquares(move.StartSquare).GetNorthEast()
                        Case -7
                            directionInterest = boardSquares(move.StartSquare).GetSouthEast()
                        Case -8
                            directionInterest = boardSquares(move.StartSquare).GetSouth()
                        Case -9
                            directionInterest = boardSquares(move.StartSquare).GetSouthWest()
                    End Select

                    If directionInterest < movementUnits / number Then      'If piece off board
                        legalMovement = False                               'Move is invalid
                        If movementUnits = 56 Or movementUnits = -56 Then
                            Continue For
                        Else
                            Return legalMovement
                        End If
                    End If

                    For movements = 0 To ((movementUnits / number) - 1)
                        Dim check As Integer = boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceType()
                        If check = Piece.None Then
                            legalMovement = True           'No piece on square, procedes to next square
                        Else
                            'There is a piece on square
                            If movements = ((movementUnits / number) - 1) And boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                legalMovement = True        'Enemy piece on target square, move is valid
                                Return legalMovement
                            Else
                                legalMovement = False       'Friendly piece on target square, move is valid
                                Return legalMovement
                            End If
                        End If
                    Next
                    Return legalMovement
                End If
            End If
        Next
        Return legalMovement
    End Function
    Function ValidatePromotedPawnMove(pieceObject, move)
        Dim legalMovement As Boolean = False
        Dim tag As Integer = pieceObject.GetPromotionTag()

        Select Case tag
            Case Piece.Queen
                Dim MovementArray As Integer() = {1, 7, 8, 9, -1, -7, -8, -9}
                Dim stepUnit As Integer
                Dim movementUnits As Integer = move.TargetSquare - move.StartSquare
                Dim negativeMovement As Boolean = False
                If movementUnits < 0 Then
                    negativeMovement = True
                End If
                For Each number In MovementArray
                    If movementUnits Mod number = 0 Then
                        If number = 1 Or number = -1 Then
                            'check if horizontal movement

                            Dim rank1 As Integer = move.StartSquare \ 8
                            Dim rank2 As Integer = move.TargetSquare \ 8

                            If rank1 = rank2 Then
                                'is a horizontal movement

                                Dim directionInterest As Integer
                                If negativeMovement = True Then
                                    stepUnit = -1
                                    directionInterest = boardSquares(move.StartSquare).GetWest()
                                Else
                                    stepUnit = 1
                                    directionInterest = boardSquares(move.StartSquare).GetEast()
                                End If
                                If directionInterest < movementUnits / number Then
                                    legalMovement = False
                                    Return legalMovement
                                End If

                                'Check if piece moves through other pieces
                                For movements = 0 To ((movementUnits / number) - stepUnit) Step stepUnit

                                    If boardSquares(move.StartSquare + movements + stepUnit).GetPieceType() = Piece.None Then
                                        legalMovement = True            'No piece on square, procedes to next square
                                    Else
                                        'There is a piece on square
                                        If movements = ((movementUnits / number) - stepUnit) And boardSquares(move.StartSquare + movements + stepUnit).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                            legalMovement = True        'Enemy piece on target square, move is valid
                                            Return legalMovement
                                        Else
                                            legalMovement = False       'Friendly piece on target square, move is invalid
                                            Return legalMovement
                                        End If
                                    End If
                                Next
                                Return legalMovement
                            Else
                                Continue For               'Not horizontal movement, different direction applies
                            End If
                        Else
                            'Found correct direction for movement
                            If negativeMovement = True Then
                                If number > 0 Then
                                    Continue For
                                End If
                            End If
                            Dim directionInterest As Integer
                            Select Case number                  'Selects appropriate direction interest
                                Case 7
                                    directionInterest = boardSquares(move.StartSquare).GetNorthWest()
                                Case 8
                                    directionInterest = boardSquares(move.StartSquare).GetNorth()
                                Case 9
                                    directionInterest = boardSquares(move.StartSquare).GetNorthEast()
                                Case -7
                                    directionInterest = boardSquares(move.StartSquare).GetSouthEast()
                                Case -8
                                    directionInterest = boardSquares(move.StartSquare).GetSouth()
                                Case -9
                                    directionInterest = boardSquares(move.StartSquare).GetSouthWest()
                            End Select

                            If directionInterest < movementUnits / number Then          'Movement if off board
                                legalMovement = False
                                If movementUnits = 56 Or movementUnits = -56 Then
                                    Continue For
                                Else
                                    Return legalMovement
                                End If
                            End If

                            For movements = 0 To ((movementUnits / number) - 1)

                                Dim check As Integer = boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceType()
                                If check = Piece.None Then
                                    legalMovement = True            'No piece on square, procedes to next square
                                Else
                                    'There is a piece on square
                                    If movements = ((movementUnits / number) - 1) And boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                        legalMovement = True            'Enemy piece on target square, move is valid
                                        Return legalMovement
                                    Else
                                        legalMovement = False           'Friendly piece on target square, move is invalid
                                        Return legalMovement
                                    End If
                                End If
                            Next
                            Return legalMovement
                        End If
                    End If
                Next
                Return legalMovement
            Case Piece.Rook

                Dim MovementArray As Integer() = {1, 8, -1, -8}
                Dim stepUnit As Integer
                Dim movementUnits As Integer = move.TargetSquare - move.StartSquare
                Dim negativeMovement As Boolean = False
                If movementUnits < 0 Then
                    negativeMovement = True
                End If
                For Each number In MovementArray
                    If movementUnits Mod number = 0 Then
                        If number = 1 Or number = -1 Then           'Check if horizontal movement
                            Dim rank1 As Integer = move.StartSquare \ 8
                            Dim rank2 As Integer = move.TargetSquare \ 8
                            If rank1 = rank2 Then           'Is a horizontal movement
                                Dim directionInterest As Integer
                                If negativeMovement = True Then
                                    stepUnit = -1
                                    directionInterest = boardSquares(move.StartSquare).GetWest()
                                Else
                                    stepUnit = 1
                                    directionInterest = boardSquares(move.StartSquare).GetEast()
                                End If
                                If directionInterest < movementUnits / number Then
                                    legalMovement = False
                                    Return legalMovement
                                End If

                                'Check if piece moves through other pieces
                                For movements = 0 To ((movementUnits / number) - stepUnit) Step stepUnit
                                    If boardSquares(move.StartSquare + movements + stepUnit).GetPieceType() = Piece.None Then
                                        legalMovement = True              'No piece on square, procedes to next square
                                    Else
                                        'There is a piece on square
                                        If movements = ((movementUnits / number) - stepUnit) And boardSquares(move.StartSquare + movements + stepUnit).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                            legalMovement = True            'Enemy piece on target square, move is valid
                                            Return legalMovement
                                        Else
                                            legalMovement = False           'Friendly piece on target square, move is invalid
                                            Return legalMovement
                                        End If
                                    End If
                                Next
                            Else
                                Continue For               'Not horizontal, different number applies for movement
                            End If

                        Else
                            'Found correct direction type for movement
                            If negativeMovement = True Then
                                If number > 0 Then
                                    Continue For
                                End If
                            End If
                            Dim directionInterest As Integer
                            Select Case number              'Selects appropriate direction interest
                                Case 7
                                    directionInterest = boardSquares(move.StartSquare).GetNorthWest()
                                Case 8
                                    directionInterest = boardSquares(move.StartSquare).GetNorth()
                                Case 9
                                    directionInterest = boardSquares(move.StartSquare).GetNorthEast()
                                Case -7
                                    directionInterest = boardSquares(move.StartSquare).GetSouthEast()
                                Case -8
                                    directionInterest = boardSquares(move.StartSquare).GetSouth()
                                Case -9
                                    directionInterest = boardSquares(move.StartSquare).GetSouthWest()
                            End Select

                            If directionInterest < movementUnits / number Then      'Movement is off board
                                legalMovement = False
                                Return legalMovement
                            End If

                            For movements = 0 To ((movementUnits / number) - 1)
                                Dim check As Integer = boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceType()
                                If check = Piece.None Then
                                    legalMovement = True        'No piece on square, procedes to next square
                                Else
                                    'There is a piece on square
                                    If movements = ((movementUnits / number) - 1) And boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                        legalMovement = True            'Enemy piece on target square, move is valid
                                        Return legalMovement
                                    Else
                                        legalMovement = False           'Friendly piece on target square, move is invalid
                                        Return legalMovement
                                    End If
                                End If
                            Next
                            Return legalMovement
                        End If
                    End If
                Next
                Return legalMovement

            Case Piece.Bishop

                Dim MovementArray As Integer() = {7, 9, -7, -9}
                Dim stepUnit As Integer
                Dim movementUnits As Integer = move.TargetSquare - move.StartSquare
                Dim negativeMovement As Boolean = False
                If movementUnits < 0 Then
                    negativeMovement = True
                End If
                For Each number In MovementArray
                    If movementUnits Mod number = 0 Then
                        If number = 1 Or number = -1 Then   'Check if horizontal movement


                            Dim rank1 As Integer = move.StartSquare \ 8
                            Dim rank2 As Integer = move.TargetSquare \ 8

                            If rank1 = rank2 Then       'Is a horizontal movement
                                Dim directionInterest As Integer
                                If negativeMovement = True Then
                                    stepUnit = -1
                                    directionInterest = boardSquares(move.StartSquare).GetWest()
                                Else
                                    stepUnit = 1
                                    directionInterest = boardSquares(move.StartSquare).GetEast()
                                End If
                                If directionInterest < movementUnits / number Then      'Movement if off board
                                    legalMovement = False
                                    Return legalMovement
                                End If

                                'Check if piece moves through other pieces
                                For movements = 0 To ((movementUnits / number) - stepUnit) Step stepUnit
                                    If boardSquares(move.StartSquare + movements + stepUnit).GetPieceType() = Piece.None Then
                                        legalMovement = True        'No piece on square, procedes to next square

                                    Else
                                        'There is a piece on square
                                        If movements = ((movementUnits / number) - stepUnit) And boardSquares(move.StartSquare + movements + stepUnit).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                            legalMovement = True            'Enemy piece on target square, move is valid
                                            Return legalMovement
                                        Else
                                            legalMovement = False           'Friendly piece on target square, move is invalid
                                            Return legalMovement
                                        End If
                                    End If
                                Next
                            Else
                                Continue For        'Not horizontal movement, different direction applies
                            End If
                        Else
                            'Found correct direction type for movement
                            If negativeMovement = True Then
                                If number > 0 Then
                                    Continue For
                                End If
                            End If
                            Dim directionInterest As Integer
                            Select Case number              'Selects appropriate direction interest
                                Case 7
                                    directionInterest = boardSquares(move.StartSquare).GetNorthWest()
                                Case 8
                                    directionInterest = boardSquares(move.StartSquare).GetNorth()
                                Case 9
                                    directionInterest = boardSquares(move.StartSquare).GetNorthEast()
                                Case -7
                                    directionInterest = boardSquares(move.StartSquare).GetSouthEast()
                                Case -8
                                    directionInterest = boardSquares(move.StartSquare).GetSouth()
                                Case -9
                                    directionInterest = boardSquares(move.StartSquare).GetSouthWest()
                            End Select

                            If directionInterest < movementUnits / number Then      'Move if off board
                                legalMovement = False
                                Return legalMovement
                            End If

                            For movements = 0 To ((movementUnits / number) - 1)
                                Dim check As Integer = boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceType()
                                If check = Piece.None Then
                                    legalMovement = True         'No piece on square, procedes to next square
                                Else
                                    'There is a piece on square
                                    If movements = ((movementUnits / number) - 1) And boardSquares(move.StartSquare + ((movements + 1) * number)).GetPieceColour() <> boardSquares(move.StartSquare).GetPieceColour() Then
                                        legalMovement = True        'Enemy piece on target square, move is valid
                                        Return legalMovement
                                    Else
                                        legalMovement = False       'Friendly piece on target square, move is invalid
                                        Return legalMovement
                                    End If
                                End If
                            Next
                            Return legalMovement
                        End If
                    End If
                Next
                Return legalMovement
            Case Piece.Knight

                Dim movementUnits As Integer = move.TargetSquare - move.StartSquare
                Dim MovementArray = {6, 10, 15, 17, -6, -10, -15, -17}
                Dim colour As Integer = boardSquares(move.startSquare).GetPieceColour()

                For Each number In MovementArray
                    If movementUnits = number Then
                        Dim directionInterest1 As Integer
                        Dim directionInterest2 As Integer
                        Select Case number              'Selects appropriate direction interest
                            Case 6
                                directionInterest1 = boardSquares(move.StartSquare).GetWest
                                directionInterest2 = boardSquares(move.StartSquare).GetNorth
                            Case 10
                                directionInterest1 = boardSquares(move.StartSquare).GetEast
                                directionInterest2 = boardSquares(move.StartSquare).GetNorth
                            Case 15
                                directionInterest1 = boardSquares(move.StartSquare).GetNorth
                                directionInterest2 = boardSquares(move.StartSquare).GetWest
                            Case 17
                                directionInterest1 = boardSquares(move.StartSquare).GetNorth
                                directionInterest2 = boardSquares(move.StartSquare).GetEast
                            Case -6
                                directionInterest1 = boardSquares(move.StartSquare).GetEast
                                directionInterest2 = boardSquares(move.StartSquare).GetSouth
                            Case -10
                                directionInterest1 = boardSquares(move.StartSquare).GetWest
                                directionInterest2 = boardSquares(move.StartSquare).GetSouth
                            Case -15
                                directionInterest1 = boardSquares(move.StartSquare).GetSouth
                                directionInterest2 = boardSquares(move.StartSquare).GetEast
                            Case -17
                                directionInterest1 = boardSquares(move.StartSquare).GetSouth
                                directionInterest2 = boardSquares(move.StartSquare).GetWest
                        End Select

                        If directionInterest1 >= 2 And directionInterest2 >= 1 Then         'Move is on the board
                            If boardSquares(move.TargetSquare).GetPieceType = Piece.None Then
                                legalMovement = True        'No piece on target square, move is valid
                                Return legalMovement
                            Else
                                'There is a piece on the target square
                                If boardSquares(move.TargetSquare).GetPieceColour <> colour Then
                                    legalMovement = True            'Enemy piece on target square, move is valid
                                    Return legalMovement
                                Else
                                    Return legalMovement            'Friendly piece on target square, move is invalid
                                End If
                            End If
                        End If
                    End If
                Next
        End Select
        Return legalMovement
    End Function
    Function ValidateOtherMove(move As Movement)            'For knight and king movements

        Dim legalMovement As Boolean = False
        Dim pieceType As Integer = boardSquares(move.StartSquare).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(move.StartSquare).GetPieceIndex()
        Dim movementUnits As Integer = move.TargetSquare - move.StartSquare

        If pieceType = Piece.Knight Then            'Validates knight movements
            Dim MovementArray = knights(pieceIndex).GetMovementArray()
            For Each number In MovementArray
                If movementUnits = number Then
                    Dim directionInterest1 As Integer
                    Dim directionInterest2 As Integer
                    Select Case number
                        Case 6
                            directionInterest1 = boardSquares(move.StartSquare).GetWest
                            directionInterest2 = boardSquares(move.StartSquare).GetNorth
                        Case 10
                            directionInterest1 = boardSquares(move.StartSquare).GetEast
                            directionInterest2 = boardSquares(move.StartSquare).GetNorth
                        Case 15
                            directionInterest1 = boardSquares(move.StartSquare).GetNorth
                            directionInterest2 = boardSquares(move.StartSquare).GetWest
                        Case 17
                            directionInterest1 = boardSquares(move.StartSquare).GetNorth
                            directionInterest2 = boardSquares(move.StartSquare).GetEast
                        Case -6
                            directionInterest1 = boardSquares(move.StartSquare).GetEast
                            directionInterest2 = boardSquares(move.StartSquare).GetSouth
                        Case -10
                            directionInterest1 = boardSquares(move.StartSquare).GetWest
                            directionInterest2 = boardSquares(move.StartSquare).GetSouth
                        Case -15
                            directionInterest1 = boardSquares(move.StartSquare).GetSouth
                            directionInterest2 = boardSquares(move.StartSquare).GetEast
                        Case -17
                            directionInterest1 = boardSquares(move.StartSquare).GetSouth
                            directionInterest2 = boardSquares(move.StartSquare).GetWest
                    End Select

                    If directionInterest1 >= 2 And directionInterest2 >= 1 Then           'Move is on the board
                        If boardSquares(move.TargetSquare).GetPieceType = Piece.None Then
                            legalMovement = True
                            Return legalMovement

                        Else
                            'There is a piece on the target square

                            If boardSquares(move.TargetSquare).GetPieceColour <> knights(pieceIndex).GetPieceColour() Then
                                legalMovement = True        'Enemy piece on target square, move is valid
                                Return legalMovement
                            Else
                                Return legalMovement        'Friendly piece on target square, move is invalid
                            End If
                        End If
                    End If
                End If
            Next
        ElseIf pieceType = Piece.King Then          'Validates king movements
            'Check if castle move
            If movementUnits = 2 Or movementUnits = -2 Then
                If kings(boardSquares(move.StartSquare).GetPieceIndex()).GetFirstMoveTaken() = False Then
                    If move.StartSquare = 4 Then                'White king castle move conditions
                        If movementUnits = 2 Then               'Queen side castle conditions
                            If boardSquares(7).GetPieceType = Piece.Rook And boardSquares(7).GetPieceColour() = kings(boardSquares(move.StartSquare).GetPieceIndex()).GetPieceColour() Then
                                If rooks(boardSquares(7).GetPieceIndex()).GetFirstMoveTaken() = False And boardSquares(5).GetPieceType = Piece.None And boardSquares(6).GetPieceType = Piece.None Then
                                    If EvaluateGameState(Piece.White) = False Then
                                        If CheckIfSquareIsAttacked(5, Piece.White) = False Then
                                            If CheckIfSquareIsAttacked(6, Piece.White) = False Then
                                                MakeRookMove(7, 5)      'Make rook castle move
                                                legalMovement = True    'Move is castle move 
                                                Return legalMovement
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        ElseIf movementUnits = -2 Then          'King side castle conditions
                            If boardSquares(0).GetPieceType = Piece.Rook And boardSquares(7).GetPieceColour() = kings(boardSquares(move.StartSquare).GetPieceIndex()).GetPieceColour() Then
                                If rooks(boardSquares(0).GetPieceIndex()).GetFirstMoveTaken() = False And boardSquares(3).GetPieceType = Piece.None And boardSquares(2).GetPieceType = Piece.None And boardSquares(1).GetPieceType = Piece.None Then
                                    If EvaluateGameState(Piece.White) = False Then
                                        If CheckIfSquareIsAttacked(3, Piece.White) = False Then
                                            If CheckIfSquareIsAttacked(2, Piece.White) = False Then
                                                MakeRookMove(0, 3)      'Make rook castle move
                                                legalMovement = True    'Move is a castle move
                                                Return legalMovement
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    ElseIf move.StartSquare = 60 Then           'Black king castle move conditions
                        If movementUnits = 2 Then               'King side castle conditions
                            If boardSquares(63).GetPieceType = Piece.Rook And boardSquares(63).GetPieceColour() = kings(boardSquares(move.StartSquare).GetPieceIndex()).GetPieceColour() Then
                                If rooks(boardSquares(63).GetPieceIndex()).GetFirstMoveTaken() = False And boardSquares(61).GetPieceType = Piece.None And boardSquares(62).GetPieceType = Piece.None Then
                                    If EvaluateGameState(Piece.Black) = False Then
                                        If CheckIfSquareIsAttacked(61, Piece.Black) = False Then
                                            If CheckIfSquareIsAttacked(62, Piece.Black) = False Then
                                                MakeRookMove(63, 61)
                                                legalMovement = True
                                                Return legalMovement
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        ElseIf movementUnits = -2 Then          'Queen side castle conditions
                            If boardSquares(56).GetPieceType = Piece.Rook And boardSquares(56).GetPieceColour() = kings(boardSquares(move.StartSquare).GetPieceIndex()).GetPieceColour() Then
                                If rooks(boardSquares(56).GetPieceIndex()).GetFirstMoveTaken() = False And boardSquares(59).GetPieceType = Piece.None And boardSquares(58).GetPieceType = Piece.None And boardSquares(57).GetPieceType = Piece.None Then
                                    If EvaluateGameState(Piece.Black) = False Then
                                        If CheckIfSquareIsAttacked(59, Piece.Black) = False Then
                                            If CheckIfSquareIsAttacked(58, Piece.Black) = False Then
                                                MakeRookMove(56, 59)
                                                legalMovement = True
                                                Return legalMovement
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            Dim MovementArray = kings(pieceIndex).GetMovementArray()
            For Each number In MovementArray
                If movementUnits = number Then
                    Dim directionInterest As Integer
                    Select Case number
                        Case 1
                            directionInterest = boardSquares(move.StartSquare).GetEast()
                        Case 7
                            directionInterest = boardSquares(move.StartSquare).GetNorthWest()
                        Case 8
                            directionInterest = boardSquares(move.StartSquare).GetNorth()
                        Case 9
                            directionInterest = boardSquares(move.StartSquare).GetNorthEast()
                        Case -1
                            directionInterest = boardSquares(move.StartSquare).GetWest()
                        Case -7
                            directionInterest = boardSquares(move.StartSquare).GetSouthEast()
                        Case -8
                            directionInterest = boardSquares(move.StartSquare).GetSouth()
                        Case -9
                            directionInterest = boardSquares(move.StartSquare).GetSouthWest()
                    End Select

                    If directionInterest >= 1 Then       'Move is on the board
                        If boardSquares(move.TargetSquare).GetPieceType = Piece.None Then
                            legalMovement = True
                            Return legalMovement

                        Else
                            'There is a piece on the target square

                            If boardSquares(move.TargetSquare).GetPieceColour <> kings(pieceIndex).GetPieceColour() Then
                                legalMovement = True        'Enemy piece on target square, move is valid
                                Return legalMovement
                            Else
                                Return legalMovement        'Friendly piece on target square, move is invalid
                            End If
                        End If
                    End If
                End If
            Next
        End If
        Return legalMovement
    End Function
#End Region
    Function CheckIfCapture(move As Board.Movement)
        If boardSquares(move.TargetSquare).GetPieceType <> Piece.None Then
            If boardSquares(move.StartSquare).GetPieceColour() <> boardSquares(move.TargetSquare).GetPieceColour() Then
                Return True
            End If
        End If
        Return False
    End Function
    Public Function GetBoardSquares()
        Return boardSquares
    End Function
    Public Function GetPawns()
        Return pawns
    End Function
    Public Function GetKnights()
        Return knights
    End Function
    Public Function GetBishops()
        Return bishops
    End Function
    Public Function GetRooks()
        Return rooks
    End Function
    Public Function GetQueens()
        Return queens
    End Function
    Public Function GetKings()
        Return kings
    End Function
    Public Sub CheckIfGameOver()
        moves = New List(Of Movement)
        moves = GenerateMoves()
        If moves.Count = 0 Then
            If EvaluateGameState(ChessGame.GetSettings.GetColourToMove()) = True Then       'If in check
                If ChessGame.GetSettings.GetColourToMove() = Piece.White Then
                    ChessGame.lblPlayerTurn.Text = "Checkmate - Black Wins"             'Display results form details (checkmate)
                    ChessGame.ToggleMouseLock()
                    ChessGame.btnStartGame.Enabled = False
                    ChessGame.btnClose.Enabled = False
                    ChessGame.btnMoveTest.Enabled = False
                    ChessGame.btnUndoMove.Enabled = False

                    ResultMessage.SetWinningSide() = Piece.Black
                    ResultMessage.ShowDialog()
                    ResultMessage.Activate()
                    Wait(Infinity)
                Else
                    ResultMessage.SetWinningSide() = Piece.White
                    ChessGame.lblPlayerTurn.Text = "Checkmate - White Wins"
                    ChessGame.ToggleMouseLock()
                    ChessGame.btnStartGame.Enabled = False
                    ChessGame.btnClose.Enabled = False
                    ChessGame.btnMoveTest.Enabled = False
                    ChessGame.btnUndoMove.Enabled = False

                    ResultMessage.ShowDialog()
                    ResultMessage.Activate()
                    Wait(Infinity)
                End If
            Else
                ResultMessage.SetWinningSide() = Piece.None                     'Display results form details (stalemate)
                ChessGame.lblPlayerTurn.Text = "Stalemate - It's A Draw"
                ChessGame.ToggleMouseLock()
                ChessGame.btnStartGame.Enabled = False
                ChessGame.btnClose.Enabled = False
                ChessGame.btnMoveTest.Enabled = False
                ChessGame.btnUndoMove.Enabled = False

                ResultMessage.ShowDialog()
                ResultMessage.Activate()
                Wait(Infinity)
            End If
        End If
    End Sub

#Region "Bitboards"
    Private ReadOnly whitePawnBitboard(7, 7) As String           'Declares bitboards For white pieces
    Private ReadOnly whiteKnightBitboard(7, 7) As String
    Private ReadOnly whiteBishopBitboard(7, 7) As String
    Private ReadOnly whiteRookBitboard(7, 7) As String
    Private ReadOnly whiteQueenBitboard(7, 7) As String
    Private ReadOnly whiteKingBitboard(7, 7) As String

    Private ReadOnly blackPawnBitboard(7, 7) As String           'Declares bitboards For black pieces
    Private ReadOnly blackKnightBitboard(7, 7) As String
    Private ReadOnly blackBishopBitboard(7, 7) As String
    Private ReadOnly blackRookBitboard(7, 7) As String
    Private ReadOnly blackQueenBitboard(7, 7) As String
    Private ReadOnly blackKingBitboard(7, 7) As String
    Sub FillEmptyBitboards()            'Fills all bitboards with null character "-"
        For rank = 0 To 7
            For file = 0 To 7
                whitePawnBitboard(7 - rank, file) = "-"
                whiteKnightBitboard(7 - rank, file) = "-"
                whiteBishopBitboard(7 - rank, file) = "-"
                whiteRookBitboard(7 - rank, file) = "-"
                whiteQueenBitboard(7 - rank, file) = "-"
                whiteKingBitboard(7 - rank, file) = "-"

                blackPawnBitboard(7 - rank, file) = "-"
                blackKnightBitboard(7 - rank, file) = "-"
                blackBishopBitboard(7 - rank, file) = "-"
                blackRookBitboard(7 - rank, file) = "-"
                blackQueenBitboard(7 - rank, file) = "-"
                blackKingBitboard(7 - rank, file) = "-"
            Next
        Next
    End Sub
    Sub GenerateKingAttacks(bitboard, file, rank)
        If boardSquares(((rank) * 8) + file).GetNorth() >= 1 Then       'Vertical and horizontal attacks
            bitboard((7 - rank) - 1, file) = "X"
        End If
        If boardSquares(((rank) * 8) + file).GetSouth() >= 1 Then
            bitboard((7 - rank) + 1, file) = "X"
        End If
        If boardSquares(((rank) * 8) + file).GetWest() >= 1 Then
            bitboard((7 - rank), file - 1) = "X"
        End If
        If boardSquares(((rank) * 8) + file).GetEast() >= 1 Then
            bitboard((7 - rank), file + 1) = "X"
        End If


        If boardSquares(((rank) * 8) + file).GetNorthEast() >= 1 Then   'Diagonal attacks
            bitboard((7 - rank) - 1, file + 1) = "X"
        End If
        If boardSquares(((rank) * 8) + file).GetSouthEast() >= 1 Then
            bitboard((7 - rank) + 1, file + 1) = "X"
        End If
        If boardSquares(((rank) * 8) + file).GetNorthWest() >= 1 Then
            bitboard((7 - rank) - 1, file - 1) = "X"
        End If
        If boardSquares(((rank) * 8) + file).GetSouthWest() >= 1 Then
            bitboard((7 - rank) + 1, file - 1) = "X"
        End If
    End Sub
    Sub GenerateKnightAttacks(bitboard, file, rank)
        If boardSquares(((rank) * 8) + file).GetNorth() >= 2 Then
            If boardSquares(((rank) * 8) + file).GetWest() >= 1 Then
                If bitboard((7 - rank) - 2, file - 1) = "N" Or bitboard((7 - rank) - 2, file - 1) = "n" Then
                    bitboard((7 - rank) - 2, file - 1) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) - 2, file - 1) = "X"
                End If
            End If
            If boardSquares(((rank) * 8) + file).GetEast() >= 1 Then
                If bitboard((7 - rank) - 2, file + 1) = "N" Or bitboard((7 - rank) - 2, file + 1) = "n" Then
                    bitboard((7 - rank) - 2, file + 1) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) - 2, file + 1) = "X"
                End If
            End If
        End If
        If boardSquares(((rank) * 8) + file).GetSouth() >= 2 Then
            If boardSquares(((rank) * 8) + file).GetWest() >= 1 Then
                If bitboard((7 - rank) + 2, file - 1) = "N" Or bitboard((7 - rank) + 2, file - 1) = "n" Then
                    bitboard((7 - rank) + 2, file - 1) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) + 2, file - 1) = "X"
                End If
            End If
            If boardSquares(((rank) * 8) + file).GetEast() >= 1 Then
                If bitboard((7 - rank) + 2, file + 1) = "N" Or bitboard((7 - rank) + 2, file + 1) = "n" Then
                    bitboard((7 - rank) + 2, file + 1) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) + 2, file + 1) = "X"
                End If
            End If
        End If
        If boardSquares(((rank) * 8) + file).GetWest() >= 2 Then
            If boardSquares(((rank) * 8) + file).GetNorth() >= 1 Then
                If bitboard((7 - rank) - 1, file - 2) = "N" Or bitboard((7 - rank) - 1, file - 2) = "n" Then
                    bitboard((7 - rank) - 1, file - 2) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) - 1, file - 2) = "X"
                End If
            End If
            If boardSquares(((rank) * 8) + file).GetSouth() >= 1 Then
                If bitboard((7 - rank) + 1, file - 2) = "N" Or bitboard((7 - rank) + 1, file - 2) = "n" Then
                    bitboard((7 - rank) + 1, file - 2) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) + 1, file - 2) = "X"
                End If
            End If
        End If
        If boardSquares(((rank) * 8) + file).GetEast() >= 2 Then
            If boardSquares(((rank) * 8) + file).GetNorth() >= 1 Then
                If bitboard((7 - rank) - 1, file + 2) = "N" Or bitboard((7 - rank) - 1, file + 2) = "n" Then
                    bitboard((7 - rank) - 1, file + 2) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) - 1, file + 2) = "X"
                End If
            End If
            If boardSquares(((rank) * 8) + file).GetSouth() >= 1 Then
                If bitboard((7 - rank) + 1, file + 2) = "N" Or bitboard((7 - rank) + 1, file + 2) = "n" Then
                    bitboard((7 - rank) + 1, file + 2) += "X"
                    bitboard((7 - rank), file) += "X"
                Else
                    bitboard((7 - rank) + 1, file + 2) = "X"
                End If
            End If
        End If
    End Sub
    Sub GeneratePawnAttacks(bitboard, file, rank)
        If bitboard Is whitePawnBitboard Then
            If boardSquares(((rank) * 8) + file).GetNorthEast() >= 1 Then       'North east attacks
                If whitePawnBitboard((7 - rank) - 1, file + 1) = "P" Then
                    whitePawnBitboard((7 - rank) - 1, file + 1) += "X"
                Else
                    whitePawnBitboard((7 - rank) - 1, file + 1) = "X"
                End If
            End If
            If boardSquares(((rank) * 8) + file).GetNorthWest() >= 1 Then       'North west attacks
                If whitePawnBitboard((7 - rank) - 1, file - 1) = "P" Then
                    whitePawnBitboard((7 - rank) - 1, file - 1) += "X"
                Else
                    whitePawnBitboard((7 - rank) - 1, file - 1) = "X"
                End If
            End If
        ElseIf bitboard Is blackPawnBitboard Then
            If boardSquares(((rank) * 8) + file).GetSouthEast() >= 1 Then       'South east attacks
                If blackPawnBitboard((7 - rank) + 1, file + 1) = "p" Then
                    blackPawnBitboard((7 - rank) + 1, file + 1) += "X"
                Else
                    blackPawnBitboard((7 - rank) + 1, file + 1) = "X"
                End If
            End If
            If boardSquares(((rank) * 8) + file).GetSouthWest() >= 1 Then       'South west attacks
                blackPawnBitboard((7 - rank) + 1, file - 1) = "X"
                If blackPawnBitboard((7 - rank) + 1, file - 1) = "p" Then
                    blackPawnBitboard((7 - rank) + 1, file - 1) += "X"
                Else
                    blackPawnBitboard((7 - rank) + 1, file - 1) = "X"
                End If
            End If
        End If
    End Sub
    Sub GenerateRookAttacks(bitboard, file, rank)

        For movement = 0 To boardSquares((rank * 8) + file).GetNorth() - 1                                              'Loop in one direction
            Dim pieceType As Integer = boardSquares(((rank + movement + 1) * 8) + file).GetPieceType()                  'Check the piece type of the square

            'Ignore the king piece when looping through squares
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank + movement + 1) * 8) + file).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank + movement + 1), file) = "X"                         'Add attacks squared to the bitboard
            ElseIf pieceType = Piece.Rook And boardSquares((rank + movement + 1) * 8 + file).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank + movement + 1), file) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank + movement + 1), file) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetSouth() - 1
            Dim pieceType As Integer = boardSquares(((rank - movement - 1) * 8) + file).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank - movement - 1) * 8) + file).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank - movement - 1), file) = "X"
            ElseIf pieceType = Piece.Rook And boardSquares((rank - movement - 1) * 8 + file).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank - movement - 1), file) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank - movement - 1), file) = "X"
                Exit For
            End If
        Next
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        For movement = 0 To boardSquares((rank * 8) + file).GetEast() - 1                                               'Generate horizontal attacks
            Dim pieceType As Integer = boardSquares(((rank) * 8) + file + movement + 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank) * 8) + file + movement + 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank), file + movement + 1) = "X"
            ElseIf pieceType = Piece.Rook And boardSquares((rank) * 8 + file + movement + 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank), file + movement + 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank), file + movement + 1) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetWest() - 1
            Dim pieceType As Integer = boardSquares(((rank) * 8) + file - movement - 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank) * 8) + file - movement - 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank), file - movement - 1) = "X"
            ElseIf pieceType = Piece.Rook And boardSquares((rank) * 8 + file - movement - 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank), file - movement - 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank), file - movement - 1) = "X"
                Exit For
            End If
        Next
    End Sub
    Sub GenerateBishopAttacks(bitboard, file, rank)
        'Generate first diagonal attacks
        For movement = 0 To boardSquares((rank * 8) + file).GetNorthEast() - 1
            Dim pieceType As Integer = boardSquares(((rank + movement + 1) * 8) + file + movement + 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank + movement + 1) * 8) + file + movement + 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank + movement + 1), file + movement + 1) = "X"
            ElseIf pieceType = Piece.Bishop And boardSquares((rank + movement + 1) * 8 + file + movement + 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank + movement + 1), file + movement + 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank + movement + 1), file + movement + 1) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetSouthWest() - 1
            Dim pieceType As Integer = boardSquares(((rank - movement - 1) * 8) + file - movement - 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank - movement - 1) * 8) + file - movement - 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank - movement - 1), file - movement - 1) = "X"
            ElseIf pieceType = Piece.Bishop And boardSquares((rank - movement - 1) * 8 + file - movement - 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank - movement - 1), file - movement - 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank - movement - 1), file - movement - 1) = "X"
                Exit For
            End If
        Next

        'Generate Second diagonal attacks
        For movement = 0 To boardSquares((rank * 8) + file).GetNorthWest() - 1
            Dim pieceType As Integer = boardSquares(((rank + movement + 1) * 8) + file - movement - 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank + movement + 1) * 8) + file - movement - 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank + movement + 1), file - movement - 1) = "X"
            ElseIf pieceType = Piece.Bishop And boardSquares((rank + movement + 1) * 8 + file - movement - 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank + movement + 1), file - movement - 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank + movement + 1), file - movement - 1) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetSouthEast() - 1
            Dim pieceType As Integer = boardSquares(((rank - movement - 1) * 8) + file + movement + 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank - movement - 1) * 8) + file + movement + 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank - movement - 1), file + movement + 1) = "X"
            ElseIf pieceType = Piece.Bishop And boardSquares((rank - movement - 1) * 8 + file + movement + 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank - movement - 1), file + movement + 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank - movement - 1), file + movement + 1) = "X"
                Exit For
            End If
        Next
    End Sub
    Sub GenerateQueenAttacks(bitboard, file, rank)
        For movement = 0 To boardSquares((rank * 8) + file).GetNorth() - 1      'Generate vertical attacks --------------
            Dim pieceType As Integer = boardSquares(((rank + movement + 1) * 8) + file).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank + movement + 1) * 8) + file).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank + movement + 1), file) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank + movement + 1) * 8 + file).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank + movement + 1), file) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank + movement + 1), file) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetSouth() - 1
            Dim pieceType As Integer = boardSquares(((rank - movement - 1) * 8) + file).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank - movement - 1) * 8) + file).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank - movement - 1), file) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank - movement - 1) * 8 + file).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank - movement - 1), file) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank - movement - 1), file) = "X"
                Exit For
            End If
        Next

        For movement = 0 To boardSquares((rank * 8) + file).GetEast() - 1      'Generate horizontal attacks --------------
            Dim pieceType As Integer = boardSquares(((rank) * 8) + file + movement + 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank) * 8) + file + movement + 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank), file + movement + 1) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank) * 8 + file + movement + 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank), file + movement + 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank), file + movement + 1) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetWest() - 1
            Dim pieceType As Integer = boardSquares(((rank) * 8) + file - movement - 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank) * 8) + file - movement - 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank), file - movement - 1) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank) * 8 + file - movement - 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank), file - movement - 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank), file - movement - 1) = "X"
                Exit For
            End If
        Next

        For movement = 0 To boardSquares((rank * 8) + file).GetNorthEast() - 1     'Generate first diagonal attacks --------------
            Dim pieceType As Integer = boardSquares(((rank + movement + 1) * 8) + file + movement + 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank + movement + 1) * 8) + file + movement + 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank + movement + 1), file + movement + 1) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank + movement + 1) * 8 + file + movement + 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank + movement + 1), file + movement + 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank + movement + 1), file + movement + 1) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetSouthWest() - 1
            Dim pieceType As Integer = boardSquares(((rank - movement - 1) * 8) + file - movement - 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank - movement - 1) * 8) + file - movement - 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank - movement - 1), file - movement - 1) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank - movement - 1) * 8 + file - movement - 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank - movement - 1), file - movement - 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank - movement - 1), file - movement - 1) = "X"
                Exit For
            End If
        Next

        For movement = 0 To boardSquares((rank * 8) + file).GetNorthWest() - 1      'Generate second diagonal attacks --------------
            Dim pieceType As Integer = boardSquares(((rank + movement + 1) * 8) + file - movement - 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank + movement + 1) * 8) + file - movement - 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank + movement + 1), file - movement - 1) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank + movement + 1) * 8 + file - movement - 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank + movement + 1), file - movement - 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank + movement + 1), file - movement - 1) = "X"
                Exit For
            End If
        Next
        For movement = 0 To boardSquares((rank * 8) + file).GetSouthEast() - 1
            Dim pieceType As Integer = boardSquares(((rank - movement - 1) * 8) + file + movement + 1).GetPieceType()
            If pieceType = Piece.None Or (pieceType = Piece.King And boardSquares(((rank - movement - 1) * 8) + file + movement + 1).GetPieceColour() <> boardSquares(rank * 8 + file).GetPieceColour()) Then
                bitboard(7 - (rank - movement - 1), file + movement + 1) = "X"
            ElseIf pieceType = Piece.Queen And boardSquares((rank - movement - 1) * 8 + file + movement + 1).GetPieceColour() = boardSquares((rank) * 8 + file).GetPieceColour() Then
                bitboard(7 - (rank - movement - 1), file + movement + 1) += "X"
                bitboard(7 - (rank), file) += "X"
                Exit For
            Else
                bitboard(7 - (rank - movement - 1), file + movement + 1) = "X"
                Exit For
            End If
        Next
    End Sub
    Function CheckIfSquareIsAttacked(squareNumber, colourIndex)
        Dim file As Integer = squareNumber Mod 8
        Dim rank As Integer = squareNumber \ 8
        If colourIndex = Piece.White Then
            If blackPawnBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf blackKnightBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf blackBishopBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf blackRookBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf blackQueenBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf blackKingBitboard(7 - rank, file).Contains("X") Then
                Return True
            End If
        ElseIf colourIndex = Piece.Black Then
            If whitePawnBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf whiteKnightBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf whiteBishopBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf whiteRookBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf whiteQueenBitboard(7 - rank, file).Contains("X") Then
                Return True
            ElseIf whiteKingBitboard(7 - rank, file).Contains("X") Then
                Return True
            End If
        End If
        Return False
    End Function
    Public Sub GenerateBitboards()
        Dim index, pieceType, pieceColour, pieceIndex As Integer

        For rank = 0 To 7                           'Loop through all squares on the board
            For file = 0 To 7
                index = (rank * 8) + file
                pieceType = boardSquares(index).GetPieceType        'Get the piece details that are on the square
                pieceColour = boardSquares(index).GetPieceColour
                pieceIndex = boardSquares(index).GetPieceIndex
                Select Case pieceType
                    Case Piece.Pawn
                        If pawns(pieceIndex).GetPromotionTag() = 0 Then      'Check the piece details to select the relevant
                            If pieceColour = Piece.White Then : whitePawnBitboard(7 - rank, file) = "P" : GeneratePawnAttacks(whitePawnBitboard, file, rank)
                            Else : blackPawnBitboard(7 - rank, file) = "p" : GeneratePawnAttacks(blackPawnBitboard, file, rank)
                            End If
                        ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Queen Then
                            If pieceColour = Piece.White Then : whiteQueenBitboard(7 - rank, file) = "Q" : GenerateQueenAttacks(whiteQueenBitboard, file, rank)
                            Else : blackQueenBitboard(7 - rank, file) = "q" : GenerateQueenAttacks(blackQueenBitboard, file, rank)
                            End If
                        ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Rook Then
                            If pieceColour = Piece.White Then : whiteRookBitboard(7 - rank, file) = "R" : GenerateRookAttacks(whiteRookBitboard, file, rank)
                            Else : blackRookBitboard(7 - rank, file) = "r" : GenerateRookAttacks(blackRookBitboard, file, rank)
                            End If
                        ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Bishop Then
                            If pieceColour = Piece.White Then : whiteBishopBitboard(7 - rank, file) = "B" : GenerateBishopAttacks(whiteBishopBitboard, file, rank)
                            Else : blackBishopBitboard(7 - rank, file) = "b" : GenerateBishopAttacks(blackBishopBitboard, file, rank)
                            End If
                        ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Knight Then
                            If pieceColour = Piece.White Then : whiteKnightBitboard(7 - rank, file) = "N" : GenerateKnightAttacks(whiteKnightBitboard, file, rank)
                            Else : blackKnightBitboard(7 - rank, file) = "n" : GenerateKnightAttacks(blackKnightBitboard, file, rank)
                            End If
                        End If
                    Case Piece.Knight
                        If pieceColour = Piece.White Then : whiteKnightBitboard(7 - rank, file) = "N" : GenerateKnightAttacks(whiteKnightBitboard, file, rank)
                        Else : blackKnightBitboard(7 - rank, file) = "n" : GenerateKnightAttacks(blackKnightBitboard, file, rank)
                        End If
                    Case Piece.Bishop
                        If pieceColour = Piece.White Then : whiteBishopBitboard(7 - rank, file) = "B" : GenerateBishopAttacks(whiteBishopBitboard, file, rank)
                        Else : blackBishopBitboard(7 - rank, file) = "b" : GenerateBishopAttacks(blackBishopBitboard, file, rank)
                        End If
                    Case Piece.Rook
                        If pieceColour = Piece.White Then : whiteRookBitboard(7 - rank, file) = "R" : GenerateRookAttacks(whiteRookBitboard, file, rank)
                        Else : blackRookBitboard(7 - rank, file) = "r" : GenerateRookAttacks(blackRookBitboard, file, rank)
                        End If
                    Case Piece.Queen
                        If pieceColour = Piece.White Then : whiteQueenBitboard(7 - rank, file) = "Q" : GenerateQueenAttacks(whiteQueenBitboard, file, rank)
                        Else : blackQueenBitboard(7 - rank, file) = "q" : GenerateQueenAttacks(blackQueenBitboard, file, rank)
                        End If
                    Case Piece.King
                        If pieceColour = Piece.White Then : whiteKingBitboard(7 - rank, file) = "K" : GenerateKingAttacks(whiteKingBitboard, file, rank)
                        Else : blackKingBitboard((7 - rank), file) = "k" : GenerateKingAttacks(blackKingBitboard, file, rank)
                        End If
                End Select
            Next
        Next
    End Sub
#End Region

#Region "Disposures"
    Public Sub DisposeCapturedPiece(move As Movement)
        Dim square As Integer = move.TargetSquare
        Dim pieceType As Integer = boardSquares(square).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(square).GetPieceIndex()

        If pieceType = Piece.Pawn Then                      'Save disposed piece details
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour(),
                                   .squareNumber = move.TargetSquare,
                                   .firstMoveNumber = pawns(pieceIndex).GetFirstMoveNumber(),
                                   .promotionTag = pawns(pieceIndex).GetPromotionTag(),
                                   .promotionMoveNumber = pawns(pieceIndex).GetPromotionMoveNumber(),
                                   .enpassentMoveNumber = pawns(pieceIndex).GetEnpassentMoveNumber()
             })
        ElseIf pieceType = Piece.Rook Then
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour(),
                                   .squareNumber = move.TargetSquare,
                                   .firstMoveNumber = rooks(pieceIndex).GetFirstMoveNumber()
                                           })
        ElseIf pieceType = Piece.King Then
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour(),
                                   .squareNumber = move.TargetSquare,
                                   .firstMoveNumber = kings(pieceIndex).GetFirstMoveNumber()
        })
        Else
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .squareNumber = move.TargetSquare,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour()
        })

        End If

        Select Case pieceType                   'Update appropriate piece counts
            Case Piece.Pawn
                If pawns(pieceIndex).GetPromotionTag() = 0 Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whitePawnCount -= 1
                    Else
                        blackPawnCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Queen Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteQueenCount -= 1
                    Else
                        blackQueenCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Rook Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteRookCount -= 1
                    Else
                        blackRookCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Bishop Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteBishopCount -= 1
                    Else
                        blackBishopCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Knight Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteKnightCount -= 1
                    Else
                        blackKnightCount -= 1
                    End If
                End If
                pawns(pieceIndex).Dispose()                 'Dispose of piece

            Case Piece.Knight
                If knights(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteKnightCount -= 1
                Else
                    blackKnightCount -= 1
                End If
                knights(pieceIndex).Dispose()

            Case Piece.Bishop
                If bishops(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteBishopCount -= 1
                Else
                    blackBishopCount -= 1
                End If
                bishops(pieceIndex).Dispose()

            Case Piece.Rook
                If rooks(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteRookCount -= 1
                Else
                    blackRookCount -= 1
                End If
                rooks(pieceIndex).Dispose()

            Case Piece.Queen
                If queens(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteQueenCount -= 1
                Else
                    blackQueenCount -= 1
                End If
                queens(pieceIndex).Dispose()
        End Select

        If disposedPieces.Count = 30 Then           'If only two kings left, call stalemate
            ResultMessage.SetWinningSide() = Piece.None
            ResultMessage.Show()
            ResultMessage.Activate()

            ChessGame.lblPlayerTurn.Text = "Stalemate - It's A Draw"
            Wait(Infinity)
        End If
    End Sub
    Public Sub DisposeCapturedPieceWithoutRefresh(move As Movement)
        Dim square As Integer = move.TargetSquare
        Dim pieceType As Integer = boardSquares(square).GetPieceType()
        Dim pieceIndex As Integer = boardSquares(square).GetPieceIndex()

        If pieceType = Piece.Pawn Then
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour(),
                                   .squareNumber = move.TargetSquare,
                                   .firstMoveNumber = pawns(pieceIndex).GetFirstMoveNumber(),
                                   .promotionTag = pawns(pieceIndex).GetPromotionTag(),
                                   .promotionMoveNumber = pawns(pieceIndex).GetPromotionMoveNumber(),
                                   .enpassentMoveNumber = pawns(pieceIndex).GetEnpassentMoveNumber()
             })
        ElseIf pieceType = Piece.Rook Then
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour(),
                                   .squareNumber = move.TargetSquare,
                                   .firstMoveNumber = rooks(pieceIndex).GetFirstMoveNumber()
                                           })
        ElseIf pieceType = Piece.King Then
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour(),
                                   .squareNumber = move.TargetSquare,
                                   .firstMoveNumber = kings(pieceIndex).GetFirstMoveNumber()
        })
        Else
            disposedPieces.Add(New Disposure With {
                                   .pieceType = pieceType,
                                   .pieceIndex = pieceIndex,
                                   .squareNumber = move.TargetSquare,
                                   .pieceColour = boardSquares(move.TargetSquare).GetPieceColour()
        })
        End If
        Select Case pieceType
            Case Piece.Pawn
                If pawns(pieceIndex).GetPromotionTag() = 0 Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whitePawnCount -= 1
                    Else
                        blackPawnCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Queen Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteQueenCount -= 1
                    Else
                        blackQueenCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Rook Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteRookCount -= 1
                    Else
                        blackRookCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Bishop Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteBishopCount -= 1
                    Else
                        blackBishopCount -= 1
                    End If
                ElseIf pawns(pieceIndex).GetPromotionTag() = Piece.Knight Then
                    If pawns(pieceIndex).GetPieceColour() = Piece.White Then
                        whiteKnightCount -= 1
                    Else
                        blackKnightCount -= 1
                    End If
                End If
                pawns(pieceIndex).Dispose()

            Case Piece.Knight
                If knights(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteKnightCount -= 1
                Else
                    blackKnightCount -= 1
                End If
                knights(pieceIndex).Dispose()

            Case Piece.Bishop
                If bishops(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteBishopCount -= 1
                Else
                    blackBishopCount -= 1
                End If
                bishops(pieceIndex).Dispose()

            Case Piece.Rook
                If rooks(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteRookCount -= 1
                Else
                    blackRookCount -= 1
                End If
                rooks(pieceIndex).Dispose()

            Case Piece.Queen
                If queens(pieceIndex).GetPieceColour() = Piece.White Then
                    whiteQueenCount -= 1
                Else
                    blackQueenCount -= 1
                End If
                queens(pieceIndex).Dispose()
        End Select
    End Sub
    Public Sub DisposeEnPassentPawn()
        Dim square As Integer = EnPassentPawn.GetSquareNumber()
        Dim pieceIndex As Integer = boardSquares(square).GetPieceIndex()
        disposedPieces.Add(New Disposure With {
                                   .pieceType = boardSquares(square).GetPieceType(),
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(square).GetPieceColour(),
                                   .squareNumber = square,
                                   .firstMoveNumber = pawns(pieceIndex).GetFirstMoveNumber(),
                                   .promotionTag = pawns(pieceIndex).GetPromotionTag(),
                                   .promotionMoveNumber = pawns(pieceIndex).GetPromotionMoveNumber(),
                                   .enpassentMoveNumber = pawns(pieceIndex).GetEnpassentMoveNumber()
             })
        boardSquares(square).ResetPieceDetails()
        EnPassentPawn.Dispose()
        ChessGame.RefreshBoard()
    End Sub
    Public Sub DisposeEnPassentPawnWithoutRefresh()
        Dim square As Integer = EnPassentPawn.GetSquareNumber()
        Dim pieceIndex As Integer = boardSquares(square).GetPieceIndex()
        disposedPieces.Add(New Disposure With {
                                   .pieceType = boardSquares(square).GetPieceType(),
                                   .pieceIndex = pieceIndex,
                                   .pieceColour = boardSquares(square).GetPieceColour(),
                                   .squareNumber = square,
                                   .firstMoveNumber = pawns(pieceIndex).GetFirstMoveNumber(),
                                   .promotionTag = pawns(pieceIndex).GetPromotionTag(),
                                   .promotionMoveNumber = pawns(pieceIndex).GetPromotionMoveNumber(),
                                   .enpassentMoveNumber = pawns(pieceIndex).GetEnpassentMoveNumber()
             })
        boardSquares(square).ResetPieceDetails()
        EnPassentPawn.Dispose()
    End Sub
    Public Sub UndisposeCapturedPieceWithoutRefresh()
        Dim disposedIndex As Integer = disposedPieces.Count - 1
        Dim pieceImage As Bitmap
        If disposedPieces(disposedIndex).promotionTag <> 0 Then                     'This works by selecting the piece type, updating the piece count, and then creating 
            If disposedPieces(disposedIndex).promotionTag = Piece.Knight Then       'a new piece and giving it the properties of the last disposed piece
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteKnight)
                    whiteKnightCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackKnight)
                    blackKnightCount += 1
                End If
                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Knight)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If
            ElseIf disposedPieces(disposedIndex).promotionTag = Piece.Bishop Then
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteBishop)
                    whiteBishopCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackBishop)
                    blackBishopCount += 1
                End If
                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Bishop)

                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)

                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()

                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If
            ElseIf disposedPieces(disposedIndex).promotionTag = Piece.Rook Then
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteRook)
                    whiteRookCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackRook)
                    blackRookCount += 1
                End If

                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Rook)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If

            ElseIf disposedPieces(disposedIndex).promotionTag = Piece.Queen Then
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteQueen)
                    whiteQueenCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackQueen)
                    blackQueenCount += 1
                End If

                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Queen)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If
            End If
        Else
            Select Case disposedPieces(disposedIndex).pieceType
                Case Piece.Pawn
                    pawns(disposedPieces(disposedIndex).pieceIndex) = New Pawn
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhitePawn)
                        whitePawnCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackPawn)
                        blackPawnCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.None Then
                        pieceImage = New Bitmap(My.Resources.BlackPawn)
                        blackPawnCount += 1
                    End If

                    pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                        If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                            pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                        End If
                    End If
                Case Piece.Knight
                    knights(disposedPieces(disposedIndex).pieceIndex) = New Knight
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteKnight)
                        whiteKnightCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackKnight)
                        blackKnightCount += 1
                    End If

                    knights(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetPosition((knights(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (knights(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    knights(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                Case Piece.Bishop
                    bishops(disposedPieces(disposedIndex).pieceIndex) = New Bishop
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteBishop)
                        whiteBishopCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackBishop)
                        blackBishopCount += 1
                    End If

                    bishops(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetPosition((bishops(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (bishops(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    bishops(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                Case Piece.Rook
                    rooks(disposedPieces(disposedIndex).pieceIndex) = New Rook
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteRook)
                        whiteRookCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackRook)
                        blackRookCount += 1
                    End If

                    rooks(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetPosition((rooks(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (rooks(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                    rooks(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                    If rooks(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                        If rooks(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                            rooks(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                        End If
                    End If

                Case Piece.Queen
                    queens(disposedPieces(disposedIndex).pieceIndex) = New Queen
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteQueen)
                        whiteQueenCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackQueen)
                        blackQueenCount += 1
                    End If

                    queens(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetPosition((queens(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (queens(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    queens(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                Case Piece.King
                    kings(disposedPieces(disposedIndex).pieceIndex) = New King
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteKing)
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackKing)
                    End If

                    kings(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetPosition((kings(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (kings(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                    kings(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                    If kings(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                        If kings(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                            kings(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                        End If
                    End If
            End Select
        End If

        boardSquares(disposedPieces(disposedIndex).squareNumber).SetPieceDetails(disposedPieces(disposedIndex).pieceType, disposedPieces(disposedIndex).pieceColour, disposedPieces(disposedIndex).pieceIndex)
        disposedPieces.Remove(disposedPieces.Last)
    End Sub
    Public Sub UndisposeCapturedPiece()
        Dim disposedIndex As Integer = disposedPieces.Count - 1
        Dim pieceImage As Bitmap
        If disposedPieces(disposedIndex).promotionTag <> 0 Then                 'This works by selecting the piece type, updating the piece count, and then creating
            If disposedPieces(disposedIndex).promotionTag = Piece.Knight Then       'a new piece and giving it the properties of the last disposed piece
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteKnight)
                    whiteKnightCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackKnight)
                    blackKnightCount += 1
                End If
                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Knight)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()

                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If
            ElseIf disposedPieces(disposedIndex).promotionTag = Piece.Bishop Then
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteBishop)
                    whiteBishopCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackBishop)
                    blackBishopCount += 1
                End If

                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Bishop)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()

                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If
            ElseIf disposedPieces(disposedIndex).promotionTag = Piece.Rook Then
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteRook)
                    whiteRookCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackRook)
                    blackRookCount += 1
                End If

                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Rook)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()

                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If
            ElseIf disposedPieces(disposedIndex).promotionTag = Piece.Queen Then
                If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                    pieceImage = New Bitmap(My.Resources.WhiteQueen)
                    whiteQueenCount += 1
                ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                    pieceImage = New Bitmap(My.Resources.BlackQueen)
                    blackQueenCount += 1
                End If

                pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionTag(Piece.Queen)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                        pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                    End If
                End If
            End If
        Else
            Select Case disposedPieces(disposedIndex).pieceType
                Case Piece.Pawn
                    pawns(disposedPieces(disposedIndex).pieceIndex) = New Pawn
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhitePawn)
                        whitePawnCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackPawn)
                        blackPawnCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.None Then
                        pieceImage = New Bitmap(My.Resources.BlackPawn)
                        blackPawnCount += 1
                    End If

                    pawns(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPosition((pawns(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (pawns(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetEnpassentMoveNumber(disposedPieces(disposedIndex).enpassentMoveNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).SetPromotionMoveNumber(disposedPieces(disposedIndex).promotionMoveNumber)
                    pawns(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()

                    If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                        If pawns(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                            pawns(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                        End If
                    End If
                Case Piece.Knight
                    knights(disposedPieces(disposedIndex).pieceIndex) = New Knight
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteKnight)
                        whiteKnightCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackKnight)
                        blackKnightCount += 1
                    End If

                    knights(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetPosition((knights(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (knights(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    knights(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    knights(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                Case Piece.Bishop
                    bishops(disposedPieces(disposedIndex).pieceIndex) = New Bishop
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteBishop)
                        whiteBishopCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackBishop)
                        blackBishopCount += 1
                    End If

                    bishops(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetPosition((bishops(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (bishops(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    bishops(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    bishops(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                Case Piece.Rook
                    rooks(disposedPieces(disposedIndex).pieceIndex) = New Rook
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteRook)
                        whiteRookCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackRook)
                        blackRookCount += 1
                    End If

                    rooks(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetPosition((rooks(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (rooks(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    rooks(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)
                    rooks(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()

                    If rooks(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                        If rooks(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                            rooks(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                        End If
                    End If
                Case Piece.Queen
                    queens(disposedPieces(disposedIndex).pieceIndex) = New Queen
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteQueen)
                        whiteQueenCount += 1
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackQueen)
                        blackQueenCount += 1
                    End If

                    queens(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetPosition((queens(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (queens(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    queens(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    queens(disposedPieces(disposedIndex).pieceIndex).ResetDisposeValue()
                Case Piece.King
                    kings(disposedPieces(disposedIndex).pieceIndex) = New King
                    If disposedPieces(disposedIndex).pieceColour = Piece.White Then
                        pieceImage = New Bitmap(My.Resources.WhiteKing)
                    ElseIf disposedPieces(disposedIndex).pieceColour = Piece.Black Then
                        pieceImage = New Bitmap(My.Resources.BlackKing)
                    End If

                    kings(disposedPieces(disposedIndex).pieceIndex).SetImage(pieceImage)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetSquareNumber(disposedPieces(disposedIndex).squareNumber)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetPosition((kings(disposedPieces(disposedIndex).pieceIndex).GetFile() * 90) + 11, (90 * 7) - (kings(disposedPieces(disposedIndex).pieceIndex).GetRank() * 90) + 11, 68, 68)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetPieceIndex(disposedPieces(disposedIndex).pieceIndex)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetPieceColour(disposedPieces(disposedIndex).pieceColour)
                    kings(disposedPieces(disposedIndex).pieceIndex).SetFirstMoveNumber(disposedPieces(disposedIndex).firstMoveNumber)

                    If kings(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <= GetMoveCount() Then
                        If kings(disposedPieces(disposedIndex).pieceIndex).GetFirstMoveNumber <> -1 Then
                            kings(disposedPieces(disposedIndex).pieceIndex).UpdateFirstMoveTaken()
                        End If
                    End If
            End Select
        End If

        boardSquares(disposedPieces(disposedIndex).squareNumber).SetPieceDetails(disposedPieces(disposedIndex).pieceType, disposedPieces(disposedIndex).pieceColour, disposedPieces(disposedIndex).pieceIndex)
        disposedPieces.Remove(disposedPieces.Last)
        ChessGame.RefreshBoard()
    End Sub
#End Region
    Public Sub Wait(timedelay)
        For i As Integer = 0 To timedelay * 100
            System.Threading.Thread.Sleep(10)
            Application.DoEvents()
        Next
    End Sub

#Region "AI Level Calls"
    Sub PlayAILevel1()
        'Plays random moves
        ChessGame.ToggleMouseLock()
        ChessGame.btnUndoMove.Enabled = False

        If disposedPieces.Count = 30 Then           'When only two kings are left - stalemate
            ResultMessage.SetWinningSide() = Piece.None                     'Display results form details (stalemate)
            ChessGame.lblPlayerTurn.Text = "Stalemate - It's A Draw"
            ChessGame.ToggleMouseLock()
            ChessGame.btnStartGame.Enabled = False
            ChessGame.btnClose.Enabled = False
            ChessGame.btnMoveTest.Enabled = False
            ChessGame.btnUndoMove.Enabled = False

            ResultMessage.ShowDialog()
            ResultMessage.Activate()
            Wait(Infinity)

        ElseIf disposedPieces.Count >= 28 Then
            PlayAILevel2()
            Exit Sub
        End If
        Dim unnessaryValue As Boolean

        moves = New List(Of Movement)
        moves = GenerateMoves()

        'Generates a random move index
        Static rn As New Random
        Dim x As Integer = rn.Next(0, moves.Count - 1)

        'Uses index to choose a random move
        Dim randomMove As Movement = moves(x)

        unnessaryValue = NewMakeMove(randomMove)
        FillEmptyBitboards()
        GenerateBitboards()
        ChessGame.GetSettings.IncrementPlayerTurnWithDelay()

        CheckIfGameOver()
        ChessGame.ToggleMouseLock()
        ChessGame.btnUndoMove.Enabled = True
        If ChessGame.GetSettings.GetColourToMove() = Piece.White Then
            Dim whitePlayer As Integer = ChessGame.GetSettings.GetWhitePlayer()
            If whitePlayer = 1 Then
                PlayAILevel1()
            ElseIf whitePlayer = 2 Then
                PlayAILevel2()
            ElseIf whitePlayer = 3 Then
                PlayAILevel3()
            ElseIf whitePlayer = 4 Then
                PlayAILevel4(0)
            End If
        ElseIf ChessGame.GetSettings.GetColourToMove() = Piece.Black Then
            Dim blackPlayer As Integer = ChessGame.GetSettings.GetBlackPlayer()
            If blackPlayer = 1 Then
                PlayAILevel1()
            ElseIf blackPlayer = 2 Then
                PlayAILevel2()
            ElseIf blackPlayer = 3 Then
                PlayAILevel3()
            ElseIf blackPlayer = 4 Then
                PlayAILevel4(0)
            End If
        End If
    End Sub
    Sub PlayAILevel2()
        'Focuses on capturing pieces
        ChessGame.ToggleMouseLock()
        ChessGame.btnUndoMove.Enabled = False

        ChessGame.lblSearchDepth.Text = "Search Depth:" + vbNewLine + "1"
        ChessGame.lblBranchesPruned.Text = "Branches Pruned:" + vbNewLine + "---"
        ChessGame.lblMovesEvaluated.Text = "Moves Evaluated:" + vbNewLine + "---"
        ChessGame.lblBestEvaluation.Text = "Best Evaluation:" + vbNewLine + "---"
        Dim bestmove As Movement = CalculateBestMove()
        NewMakeMove(bestmove)

        ChessGame.GetSettings.IncrementPlayerTurnWithDelay()
        CheckIfGameOver()
        ChessGame.ToggleMouseLock()
        ChessGame.btnUndoMove.Enabled = True
        If ChessGame.GetSettings.GetColourToMove() = Piece.White Then
            Dim whitePlayer As Integer = ChessGame.GetSettings.GetWhitePlayer()
            If whitePlayer = 1 Then
                PlayAILevel1()
            ElseIf whitePlayer = 2 Then
                PlayAILevel2()
            ElseIf whitePlayer = 3 Then
                PlayAILevel3()
            ElseIf whitePlayer = 4 Then
                PlayAILevel4(0)
            End If
        ElseIf ChessGame.GetSettings.GetColourToMove() = Piece.Black Then
            Dim blackPlayer As Integer = ChessGame.GetSettings.GetBlackPlayer()
            If blackPlayer = 1 Then
                PlayAILevel1()
            ElseIf blackPlayer = 2 Then
                PlayAILevel2()
            ElseIf blackPlayer = 3 Then
                PlayAILevel3()
            ElseIf blackPlayer = 4 Then
                PlayAILevel4(0)
            End If
        End If
    End Sub
    Function CalculateBestMove()
        'GenerateMoves()
        moves = New List(Of Movement)
        moves = GenerateMoves()
        moves = ShuffleMoves(moves)
        Dim bestmove As Movement = Nothing
        Dim bestValue As Integer
        If ChessGame.GetSettings().getcolourtoMove() = Piece.White Then
            bestValue = -Infinity
        Else
            bestValue = Infinity
        End If
        For i = 0 To moves.Count - 1
            Dim capture As Boolean = NewMakeMoveWithoutRefresh(moves(i))
            ChessGame.GetSettings.incrementplayerturn()
            If moves(i).TargetSquare = 42 And moves(i).StartSquare = 47 Then
                Application.DoEvents()
            End If

            Dim tempMoves As List(Of Movement) = GenerateMoves()
            Dim boardValue As Integer
            If tempMoves.Count = 0 Then                          'Check for checkmate or stalemate
                If ChessGame.GetSettings().getcolourtoMove() = Piece.White Then
                    If EvaluateGameState(Piece.White) = True Then : boardValue = -Infinity      'Checkmate has been discovered
                    Else : boardValue = 0                                                        'Stalemate has been discovered
                    End If
                Else
                    If EvaluateGameState(Piece.Black) = True Then : boardValue = Infinity    'Checkmate has been discovered
                    Else : boardValue = 0                                                    'Stalemate has been discovered
                    End If
                End If
            Else
                boardValue = Evaluate()
            End If
            ChessGame.GetSettings.incrementplayerturn()
            NewUnmakeMoveWithoutRefresh(moves(i), capture)

            If ChessGame.GetSettings().getcolourtoMove() = Piece.White Then
                If boardValue > bestValue Then
                    bestValue = boardValue
                    bestmove = moves(i)
                End If
            Else
                If boardValue < bestValue Then
                    bestValue = boardValue
                    bestmove = moves(i)
                End If
            End If
        Next
        ChessGame.lblBestEvaluation.Text = "Best Evaluation:" + vbNewLine + bestValue.ToString
        Return bestmove
    End Function
    Sub DisplayCheckMessage()
        Dim inCheck As Boolean = False
        If ChessGame.GetSettings.GetwhitePlayer() = 0 Then
            inCheck = EvaluateGameState(Piece.White)

            If inCheck = True Then
                ChessGame.lblCheckMessage.Text = "White is in check"
                ChessGame.lblCheckMessage.Visible = True
            Else
                ChessGame.lblCheckMessage.Visible = False
            End If
        Else
            ChessGame.lblCheckMessage.Visible = False
        End If
        If ChessGame.GetSettings.getblackplayer() = 0 And inCheck = False Then
            inCheck = EvaluateGameState(Piece.Black)
            If inCheck = True Then
                ChessGame.lblCheckMessage.Text = "Black is in check"
                ChessGame.lblCheckMessage.Visible = True
            Else
                ChessGame.lblCheckMessage.Visible = False
            End If
        ElseIf inCheck = False Then
            ChessGame.lblCheckMessage.Visible = False
        End If
    End Sub

    Sub PlayAILevel3()
        'Plays the minimax AI but at a random depth or either 1, 2 or 3

        Randomize()
        Dim value As Integer = CInt(Int((3 * Rnd()) + 1))
        PlayAILevel4(value)
    End Sub

    Sub PlayAILevel4(depth)
        'Uses a minimax algorithm
        ChessGame.ToggleMouseLock()
        ChessGame.btnUndoMove.Enabled = False

        If depth = 0 Then
            depth = MaximumDepth
        End If
        Dim playerTurn As Integer = ChessGame.GetSettings.GetColourToMove()
        Dim bestMove As Movement
        ChessGame.lblSearchDepth.Text = "Search Depth:" + vbNewLine + depth.ToString
        ChessGame.lblBranchesPruned.Text = "Branches Pruned:" + vbNewLine + "---"
        ChessGame.lblMovesEvaluated.Text = "Moves Evaluated:" + vbNewLine + "---"
        ChessGame.lblBestEvaluation.Text = "Best Evaluation:" + vbNewLine + "---"
        If playerTurn = Piece.White Then
            bestMove = SearchRootNode(depth, -Infinity, Infinity, True)
        ElseIf playerTurn = Piece.Black Then
            bestMove = SearchRootNode(depth, -Infinity, Infinity, False)
        End If
        ChessGame.lblBranchesPruned.Text = "Branches Pruned:" + vbNewLine + branchesPruned.ToString()
        ChessGame.lblMovesEvaluated.Text = "Moves Evaluated:" + vbNewLine + movesEvaluated.ToString

        ChessGame.GetSettings.SetColourToMove(playerTurn)
        NewMakeMove(bestMove)

        ChessGame.GetSettings.IncrementPlayerTurn()
        CheckIfGameOver()
        ChessGame.ToggleMouseLock()
        ChessGame.btnUndoMove.Enabled = True
        If ChessGame.GetSettings.GetColourToMove() = Piece.White Then
            Dim whitePlayer As Integer = ChessGame.GetSettings.GetWhitePlayer()
            If whitePlayer = 1 Then
                PlayAILevel1()
            ElseIf whitePlayer = 2 Then
                PlayAILevel2()
            ElseIf whitePlayer = 3 Then
                PlayAILevel3()
            ElseIf whitePlayer = 4 Then
                PlayAILevel4(0)
            End If
        ElseIf ChessGame.GetSettings.GetColourToMove() = Piece.Black Then
            Dim blackPlayer As Integer = ChessGame.GetSettings.GetBlackPlayer()
            If blackPlayer = 1 Then
                PlayAILevel1()
            ElseIf blackPlayer = 2 Then
                PlayAILevel2()
            ElseIf blackPlayer = 3 Then
                PlayAILevel3()
            ElseIf blackPlayer = 4 Then
                PlayAILevel4(0)
            End If
        End If
    End Sub
    Function CheckIfForward(possibleMove As Movement)           'Checks if a move is up the board
        Dim isForward As Boolean = False
        If ChessGame.GetSettings.GetColourToMove() = Piece.White Then
            If possibleMove.StartSquare < possibleMove.TargetSquare Then
                isForward = True
            End If
        Else
            If possibleMove.StartSquare > possibleMove.TargetSquare Then
                isForward = True
            End If
        End If
        Return isForward
    End Function
    Function CheckPawnPromotion(possibleMove As Movement)
        If boardSquares(possibleMove.StartSquare).GetPieceType = Piece.Pawn Then
            If pawns(boardSquares(possibleMove.StartSquare).GetPieceIndex).GetPromotionTag = 0 Then
                If ChessGame.GetSettings.GetColourToMove() = Piece.White Then
                    If boardSquares(possibleMove.TargetSquare).GetRank = 7 Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    If boardSquares(possibleMove.TargetSquare).GetRank = 0 Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function
    Function ShuffleMoves(moves As List(Of Movement))
        Dim rand As New Random()
        Dim temp As Movement
        Dim indexRand As Integer
        Dim indexLast As Integer = moves.Count - 1
        For index As Integer = 0 To indexLast
            indexRand = rand.Next(index, indexLast)
            temp = moves(indexRand)
            moves(indexRand) = moves(index)
            moves(index) = temp
        Next index
        Return moves
    End Function
    Function Evaluate()
        Dim whiteEval As Integer = CountMaterial(Piece.White)
        Dim blackEval As Integer = CountMaterial(Piece.Black)

        Dim evaluation As Integer = whiteEval - blackEval
        If evaluation <> 0 Then
            Application.DoEvents()
        End If
        Return evaluation
    End Function
    Function CountMaterial(colourIndex)
        Dim material As Integer = 0
        If colourIndex = Piece.White Then
            material += whitePawnCount * pawnValue
            material += whiteKnightCount * knightValue
            material += whiteBishopCount * pawnValue
            material += whiteRookCount * rookValue
            material += whiteQueenCount * queenValue
        ElseIf colourIndex = Piece.Black Then
            material += blackPawnCount * pawnValue
            material += blackKnightCount * knightValue
            material += blackBishopCount * pawnValue
            material += blackRookCount * rookValue
            material += blackQueenCount * queenValue
        End If
        Return material
    End Function
#End Region

#Region "Minimax Algorithm"
    Function SearchRootNode(depth, alpha, beta, maximisingPlayer)
        movesEvaluated = 0
        Dim bestMoves As New List(Of Movement)
        Dim checkmateMoves As New List(Of Movement)
        Dim bestValue As Integer
        If maximisingPlayer = True Then                             'When it's white's move
            bestValue = -Infinity
            moves = New List(Of Movement)
            moves = GenerateMoves()                                         'Generate all the possible legal moves for white
            moves = OrderMoves(moves)                               'Order the generated moves to increase alpha beta pruning

            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()

            For Each possibleMove In moves                           'Go through every possible move to work out which one yields the maximum (best for white) value.

                Dim capture As Boolean = NewMakeMoveWithoutRefresh(possibleMove)            'Play the move
                movesEvaluated += 1
                Dim value As Integer = Search(depth - 1, bestValue, Infinity, False)           'Recursively evaluate the move generation tree to a certain depth

                If value > bestValue Then                                        'If a better value is found, clear the list and add the new move
                    bestValue = value
                    bestMoves.Clear()
                    bestMoves.Add(possibleMove)
                ElseIf value = bestValue Then                                    'If the same value is generated, add the move to the list
                    bestMoves.Add(possibleMove)
                End If
                If value = Infinity Then                                        'If the value is valid for a checkmate, add the list to the checkmate move list
                    checkmateMoves.Add(possibleMove)
                End If
                NewUnmakeMoveWithoutRefresh(possibleMove, capture)

            Next
            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()           'Increment the player turn for the next recursive call

        Else                                                         'When it's black's move
            bestValue = Infinity
            moves = New List(Of Movement)
            moves = GenerateMoves()                                             'Generate all the possible legal moves for black
            moves = OrderMoves(moves)                                       'Order the generated moves to increase alpha beta pruning
            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()       'Increment the player turn

            For Each possibleMove In moves                            'Go through every possible move to work out which one yields the minimum (best for black) value.
                Dim capture As Boolean = NewMakeMoveWithoutRefresh(possibleMove)                    'Play the move
                movesEvaluated += 1
                Dim value As Integer = Search(depth - 1, -Infinity, bestValue, True)           'Recursively evaluate the move generation tree to a certain depth

                If value < bestValue Then                                       'If a better value is found, clear the list and add the new move
                    bestValue = value
                    bestMoves.Clear()
                    bestMoves.Add(possibleMove)
                ElseIf value = bestValue Then                                   'If the same value is generated, add the move to the list
                    bestMoves.Add(possibleMove)
                End If
                If value = -Infinity Then                                       'If the value is valid for a checkmate, add the list to the checkmate move list
                    checkmateMoves.Add(possibleMove)
                End If
                NewUnmakeMoveWithoutRefresh(possibleMove, capture)               'Undo the move
            Next
            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()            'Increment the player turn for the next recursive call
        End If

        If bestValue = Infinity Or bestValue = -Infinity Then
            ChessGame.lblBestEvaluation.Text = "Best Evaluation:" + vbNewLine + bestValue.ToString
        Else
            ChessGame.lblBestEvaluation.Text = "Best Evaluation:" + vbNewLine + (Math.Floor(bestValue / 100) * 100).ToString
        End If

        If checkmateMoves.Count > 0 Then                             'Select the final move from the collected best moves
            Return checkmateMoves(0)                                 'Play a checkmate move if there is any
        Else
            Return (CalculateMinimaxMove(bestMoves))                'Otherwise determine the best move to play
        End If
    End Function
    Function Search(depth As Integer, alpha As Integer, beta As Integer, maximisingPlayer As Boolean)

        Dim value As Integer
        If depth = 0 Then : value = Evaluate() : movesEvaluated += 1 : Return value : End If        'Base case (depth = 0) to end the recursive call

        moves = New List(Of Movement)
        moves = GenerateMoves()                                   'Generate all the possible moves for the player colour to move
        If moves.Count = 0 Then                          'Check for checkmate or stalemate
            If maximisingPlayer = True Then
                If EvaluateGameState(Piece.White) = True Then : Return -Infinity      'Checkmate has been discovered
                Else : Return 0                                                        'Stalemate has been discovered
                End If
            Else
                If EvaluateGameState(Piece.Black) = True Then : Return Infinity    'Checkmate has been discovered
                Else : Return 0                                                    'Stalemate has been discovered
                End If
            End If
        End If

        If maximisingPlayer = True Then                             'When it's white's move

            moves = OrderMoves(moves)                               'Order the possible moves to increase alpha beta pruning
            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()

            For Each possibleMove In moves                           'Go through every possible move to work out which one yields the maximum (best for white) value.
                Dim capture As Boolean = NewMakeMoveWithoutRefresh(possibleMove)            'Make the move 
                movesEvaluated += 1
                value = Search(depth - 1, alpha, beta, False)                               'Recursively evaluate the move generation tree to a certain depth
                NewUnmakeMoveWithoutRefresh(possibleMove, capture)                          'Undo the move

                alpha = Max(alpha, value)                                       'Update the new highest white value
                If alpha > beta Then
                    branchesPruned += 1
                    ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()   'Increment the player turn for the next recursive call
                    Return alpha
                End If
            Next
            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()           'Increment the player turn for the next recursive call
            Return alpha

        Else                                                        'When it's black's move
            moves = OrderMoves(moves)                                   'Order the possible moves to increase alpha beta pruning
            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()

            For Each possibleMove In moves                                     'Go through every possible move to work out which one yields the minimum (best for black) value.
                Dim capture As Boolean = NewMakeMoveWithoutRefresh(possibleMove)        'Make the move
                movesEvaluated += 1
                value = Search(depth - 1, alpha, beta, True)                            'Recursively evaluate the move generation tree to a certain depth
                NewUnmakeMoveWithoutRefresh(possibleMove, capture)                      'Undo the move

                beta = Min(beta, value)                                         'Update the new lowest black value
                If alpha > beta Then
                    branchesPruned += 1
                    ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()   'Increment the player turn for the next recursive call
                    Return beta
                End If
            Next
            ChessGame.GetSettings.IncrementPlayerTurnWithoutRefresh()           'Increment the player turn for the next recursive call
            Return beta
        End If
    End Function
    Function CalculateMinimaxMove(moveList As List(Of Movement))

        ' Determines the best move out of a list of moves by the criteria that a capture is better than just moving forward which is better than not moving forward. This
        ' will give the AI a bit more direction at the start of the game.

        Dim bestMoves As New List(Of Movement)

        'Check for pawn promotion
        ' Check if any moves lead to pawn promotion.
        For Each possibleMove In moveList
            If CheckPawnPromotion(possibleMove) Then
                bestMoves.Add(possibleMove)
            End If
        Next

        If bestMoves.Count <> 0 Then
            Return bestMoves(Math.Round(Rnd() * (bestMoves.Count - 1)))
        End If

        ' Check if any moves lead to capture.
        For Each possibleMove In moveList
            If CheckIfCapture(possibleMove) Then
                bestMoves.Add(possibleMove)
            End If
        Next

        If bestMoves.Count <> 0 Then
            Return bestMoves(Math.Round(Rnd() * (bestMoves.Count - 1)))
        End If

        ' Check if any moves go forward.
        For Each possibleMove In moveList
            If CheckIfForward(possibleMove) Then
                bestMoves.Add(possibleMove)
            End If
        Next

        If bestMoves.Count <> 0 Then
            Return bestMoves(Math.Round(Rnd() * (bestMoves.Count - 1)))
        Else
            Return moveList(0)
        End If
    End Function
#End Region


#Region "Optimisations"
    Function OrderMoves(moves)
        Dim OrderedList As New List(Of Movement)
        Dim tempList As New List(Of Integer)

        For Each moveObject As Movement In moves
            Dim moveScoreGuess As Integer = 0
            Dim movePieceType As Integer = boardSquares(moveObject.StartSquare).GetPieceType()
            Dim capturePieceType = boardSquares(moveObject.TargetSquare).GetPieceType()
            Dim rank As Integer = boardSquares(moveObject.TargetSquare).GetRank()
            Dim file As Integer = boardSquares(moveObject.TargetSquare).GetFile()

            'Prioritise capturing opponent's most valuable pieces with our least valuable pieces
            If capturePieceType <> Piece.None Then
                Dim captureValue As Integer = PieceToPieceValue.Item(capturePieceType)
                Dim moveValue As Integer = PieceToPieceValue.Item(movePieceType)
                moveScoreGuess += 10 * captureValue - moveValue
            End If

            'Promoting a pawn is likely to be good
            If movePieceType = Piece.Pawn Then

                If pawns(boardSquares(moveObject.StartSquare).GetPieceIndex()).GetPromotionTag = 0 And rank = 0 And boardSquares(moveObject.StartSquare).GetPieceColour() = Piece.Black Then
                    moveScoreGuess += queenValue
                ElseIf pawns(boardSquares(moveObject.StartSquare).GetPieceIndex()).GetPromotionTag = 0 And rank = 7 And boardSquares(moveObject.StartSquare).GetPieceColour() = Piece.White Then
                    moveScoreGuess += queenValue
                End If
            End If

            'Penalize moving our pieces to a square attacked by an opponent pawn
            If boardSquares(moveObject.StartSquare).GetPieceColour() = Piece.Black Then
                If whitePawnBitboard(7 - rank, file).Contains("X") Then
                    Dim moveValue As Integer = PieceToPieceValue.Item(movePieceType)
                    moveScoreGuess -= moveValue
                End If
            ElseIf boardSquares(moveObject.StartSquare).GetPieceColour() = Piece.White Then
                If blackPawnBitboard(7 - rank, file).Contains("X") Then
                    Dim moveValue As Integer = PieceToPieceValue.Item(movePieceType)
                    moveScoreGuess -= moveValue
                End If
            End If

            'Insertion sort - Ordering moves with the highest score value first
            tempList.Add(moveScoreGuess)
            tempList.Sort()
            Dim index As Integer = tempList.IndexOf(moveScoreGuess)
            OrderedList.Insert(index, moveObject)
        Next
        Return OrderedList
    End Function
#End Region

End Class
