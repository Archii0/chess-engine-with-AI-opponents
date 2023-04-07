Public Class GameSettings

    Private whitePlayer As Integer
    Private blackPlayer As Integer
    Private ai1TimeDelay As Integer
    Private ai2TimeDelay As Integer
    Private startFEN As String
    Private moveTest As Boolean = False
    Private playerTurnLocked As Boolean
    Private colourToMove As Integer = Piece.White

    Public Sub IncrementPlayerTurn()
        If playerTurnLocked = False Then
            If colourToMove = Piece.White Then
                colourToMove = Piece.Black
                ChessGame.lblPlayerTurn.Text = "Player Turn:" + vbNewLine + "Black's Move"
            Else
                colourToMove = Piece.White
                ChessGame.lblPlayerTurn.Text = "Player Turn:" + vbNewLine + "White's Move"
            End If
        End If
    End Sub
    Public Sub IncrementPlayerTurnWithoutRefresh()
        If playerTurnLocked = False Then
            If colourToMove = Piece.White Then
                colourToMove = Piece.Black

            Else
                colourToMove = Piece.White

            End If
        End If
    End Sub
    Public Sub IncrementPlayerTurnWithDelay()
        If playerTurnLocked = False Then
            If colourToMove = Piece.White Then
                colourToMove = Piece.Black
                If ai1TimeDelay <> 0 Then
                    AITimeDelay(ai1TimeDelay / 2)
                End If
                ChessGame.lblPlayerTurn.Text = "Player Turn:" + vbNewLine + "Black's Move"
            Else
                colourToMove = Piece.White
                If ai2TimeDelay <> 0 Then
                    AITimeDelay(ai2TimeDelay / 2)
                End If
                ChessGame.lblPlayerTurn.Text = "Player Turn:" + vbNewLine + "White's Move"
            End If
        End If
    End Sub
    Sub AITimeDelay(timeDelay)
        For i As Integer = 0 To timeDelay * 100
            System.Threading.Thread.Sleep(10)
            Application.DoEvents()
        Next
    End Sub
    Public Sub LockPlayerTurn()
        playerTurnLocked = True
    End Sub
    Public Sub UnlockPlayerTurn()
        playerTurnLocked = False
    End Sub
    Public Sub SetMoveTest(x)
        moveTest = x
    End Sub
    Sub SetColourToMove(x)
        colourToMove = x
    End Sub
    Sub SetWhitePlayerType(x)
        whitePlayer = x
    End Sub
    Sub SetBlackPlayerType(x)
        blackPlayer = x
    End Sub
    Sub SetAI1Delay(x)
        ai1TimeDelay = x
    End Sub
    Sub SetAI2Delay(x)
        ai2TimeDelay = x
    End Sub
    Sub SetStartFEN(x)
        startFEN = x
    End Sub
    Function GetStartFEN()
        Return startFEN
    End Function
    Function GetWhitePlayer()
        Return whitePlayer
    End Function
    Function GetBlackPlayer()
        Return blackPlayer
    End Function
    Function GetAI1Delay()
        Return ai1TimeDelay
    End Function
    Function GetAI2Delay()
        Return ai2TimeDelay
    End Function
    Public Function GetColourToMove()
        Return colourToMove
    End Function
    Public Function GetMoveTest()
        Return moveTest
    End Function
End Class
