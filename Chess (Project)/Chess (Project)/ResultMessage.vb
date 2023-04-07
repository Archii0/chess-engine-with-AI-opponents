Public Class ResultMessage
    Private WinningSide As Integer = 0

    Public WriteOnly Property SetWinningSide As Integer
        Set(value As Integer)
            WinningSide = value
        End Set
    End Property

    Private Sub ResultMessage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Left = (Screen.PrimaryScreen.WorkingArea.Width - Me.Width) / 2       'Sets the form position
        Me.Top = (Screen.PrimaryScreen.WorkingArea.Height - Me.Height) / 2
        Me.MaximizeBox = False

        Select Case WinningSide         'Updates form labels
            Case 0
                lblResult.Text = "Stalemate"
                lblWinningSide.Text = "It's A Draw"
            Case Piece.White
                lblResult.Text = "Checkmate"
                lblWinningSide.Text = "White Wins"
            Case Piece.Black
                lblResult.Text = "Checkmate"
                lblWinningSide.Text = "Black Wins"
        End Select
    End Sub

    Private Sub btnPlayAgain_Click(sender As Object, e As EventArgs) Handles btnPlayAgain.Click
        StartMenu.Show()            'Restarts the game and shows user the start form
        ChessGame.Close()
        Me.Close()
    End Sub
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        End             'Closes the program
    End Sub
End Class