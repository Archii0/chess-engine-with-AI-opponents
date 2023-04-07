Public Class StartMenu
    Private whitePlayer As Integer = Nothing
    Private blackPlayer As Integer = Nothing
    Private aiDelay1 As Integer = Nothing
    Private aiDelay2 As Integer = Nothing
    Private customFEN As String = Nothing
    Private moveTest As Boolean = False

    Private Sub tbWhitePlayer_ValueChanged(sender As Object, e As EventArgs) Handles tbWhitePlayer.ValueChanged
        If tbWhitePlayer.Value = 0 Then         'Updates the labels for the black slider
            lblSlider1.Text = "Player"
            whitePlayer = 0
            lblSlider1Desc.Text = "Takes User Moves"
            DisableAITime1()
        ElseIf tbWhitePlayer.Value = 1 Then
            lblSlider1.Text = "AI Level 1"
            whitePlayer = 1
            lblSlider1Desc.Text = "Plays Random Moves"
            EnableAITime1()
        ElseIf tbWhitePlayer.Value = 2 Then
            lblSlider1.Text = "AI Level 2"
            whitePlayer = 2
            lblSlider1Desc.Text = "Focuses on Capturing Pieces"
            EnableAITime1()
        ElseIf tbWhitePlayer.Value = 3 Then
            lblSlider1.Text = "AI Level 3"
            whitePlayer = 3
            lblSlider1Desc.Text = "Advanced Captures and Observation"
            DisableAITime1()
        ElseIf tbWhitePlayer.Value = 4 Then
            lblSlider1.Text = "AI Level 4"
            whitePlayer = 4
            lblSlider1Desc.Text = "Extensive Decision Making"
            DisableAITime1()
        End If
    End Sub
    Private Sub tbBlackPlayer_ValueChanged(sender As Object, e As EventArgs) Handles tbBlackPlayer.ValueChanged
        If tbBlackPlayer.Value = 0 Then         'Updates the labels for the black slider
            lblSlider2.Text = "Player"
            blackPlayer = 0
            lblSlider2Desc.Text = "Takes User Moves"
            DisableAITime2()
        ElseIf tbBlackPlayer.Value = 1 Then
            lblSlider2.Text = "AI Level 1"
            blackPlayer = 1
            lblSlider2Desc.Text = "Plays Random Moves"
            EnableAITime2()
        ElseIf tbBlackPlayer.Value = 2 Then
            lblSlider2.Text = "AI Level 2"
            blackPlayer = 2
            lblSlider2Desc.Text = "Focuses on Capturing Pieces"
            EnableAITime2()
        ElseIf tbBlackPlayer.Value = 3 Then
            lblSlider2.Text = "AI Level 3"
            blackPlayer = 3
            lblSlider2Desc.Text = "Advanced Captures and Observation"
            DisableAITime2()
        ElseIf tbBlackPlayer.Value = 4 Then
            lblSlider2.Text = "AI Level 4"
            blackPlayer = 4
            lblSlider2Desc.Text = "Extensive Decision Making"
            DisableAITime2()
        End If
    End Sub
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        ChessGame.SetAIDelay1() = aiDelay1          'Sends properties to the game form
        ChessGame.SetAIDelay2() = aiDelay2
        ChessGame.SetBlackPlayer() = blackPlayer
        ChessGame.SetWhitePlayer() = whitePlayer
        ChessGame.SetCustomFen() = customFEN

        ChessGame.Show()            'Shows the game form
        ChessGame.Activate()
        Me.Close()                  'Closes the start menu
    End Sub
    Private Sub StartMenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Left = (Screen.PrimaryScreen.WorkingArea.Width - Me.Width) / 2       'Sets the form position
        Me.Top = (Screen.PrimaryScreen.WorkingArea.Height - Me.Height) / 2
        Me.MaximizeBox = False

        DisableAITime1()            'Hides unselected fields
        DisableAITime2()
        DisableCustomFEN()
    End Sub
    Sub DisableCustomFEN()      'Hides custom FEN fields
        tbCustomFEN.Enabled = False
        tbCustomFEN.Visible = False
        lblFEN.Enabled = False
        lblFEN.Visible = False
    End Sub
    Sub EnableCustomFEN()       'Shows custom FEN fields
        tbCustomFEN.Enabled = True
        tbCustomFEN.Visible = True
        lblFEN.Enabled = True
        lblFEN.Visible = True
    End Sub
    Sub EnableAITime1()         'Shows AI 1 time delay fields
        cbAI1Time.Enabled = True
        cbAI1Time.Visible = True
        lblAI1Time.Enabled = True
        lblAI1Time.Visible = True
    End Sub
    Sub EnableAITime2()         'Shows AI 2 time delay fields
        cbAI2Time.Enabled = True
        cbAI2Time.Visible = True
        lblAI2Time.Enabled = True
        lblAI2Time.Visible = True
    End Sub
    Sub DisableAITime1()        'Hides AI 1 time delay fields
        cbAI1Time.Enabled = False
        cbAI1Time.Visible = False
        lblAI1Time.Enabled = False
        lblAI1Time.Visible = False

        aiDelay1 = 0
    End Sub
    Sub DisableAITime2()        'Hides AI 1 time delay fields
        cbAI2Time.Enabled = False
        cbAI2Time.Visible = False
        lblAI2Time.Enabled = False
        lblAI2Time.Visible = False

        aiDelay2 = 0
    End Sub
    Private Sub cbChangeStartFEN_CheckedChanged(sender As Object, e As EventArgs) Handles cbChangeStartFEN.CheckedChanged
        If cbChangeStartFEN.Checked = True Then
            EnableCustomFEN()                           'Shows the custom FEN field
        ElseIf cbChangeStartFEN.Checked = False Then
            DisableCustomFEN()                          'Hides the custom FEN field
        End If
    End Sub
    Private Sub tbCustomFEN_TextChanged(sender As Object, e As EventArgs) Handles tbCustomFEN.TextChanged
        customFEN = tbCustomFEN.Text
    End Sub
    Private Sub cbAI1Time_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbAI1Time.SelectedIndexChanged
        aiDelay1 = cbAI1Time.SelectedIndex / 2
    End Sub
    Private Sub cbAI2Time_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbAI2Time.SelectedIndexChanged
        aiDelay2 = cbAI2Time.SelectedIndex / 2
    End Sub
    Private Sub btnMoveTestStart_Click(sender As Object, e As EventArgs) Handles btnMoveTestStart.Click
        ChessGame.SetAIDelay1() = 0     'Sends properties to the game form
        ChessGame.SetAIDelay2() = 0
        ChessGame.SetBlackPlayer() = 0
        ChessGame.SetWhitePlayer() = 0
        ChessGame.SetCustomFen() = customFEN
        ChessGame.SetMoveTest() = True

        ChessGame.Show()        'Shows the game form
        ChessGame.Activate()
        Me.Close()              'Closes the start form
    End Sub
End Class