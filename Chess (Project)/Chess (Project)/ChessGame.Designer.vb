<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ChessGame
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ChessGame))
        Me.lblMoveTest = New System.Windows.Forms.Label()
        Me.btnMoveTest = New System.Windows.Forms.Button()
        Me.lblBlackPlayer = New System.Windows.Forms.Label()
        Me.lblWhitePlayer = New System.Windows.Forms.Label()
        Me.lblBlackDescription = New System.Windows.Forms.Label()
        Me.lblWhiteDescription = New System.Windows.Forms.Label()
        Me.lblVS = New System.Windows.Forms.Label()
        Me.lblPlayerTurn = New System.Windows.Forms.Label()
        Me.lblBestEvaluation = New System.Windows.Forms.Label()
        Me.lblBranchesPruned = New System.Windows.Forms.Label()
        Me.btnUndoMove = New System.Windows.Forms.Button()
        Me.btnStartGame = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblSelectedPiece = New System.Windows.Forms.Label()
        Me.lblMovesEvaluated = New System.Windows.Forms.Label()
        Me.lblSearchDepth = New System.Windows.Forms.Label()
        Me.lblCheckMessage = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblMoveTest
        '
        Me.lblMoveTest.AutoSize = True
        Me.lblMoveTest.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblMoveTest.ForeColor = System.Drawing.Color.White
        Me.lblMoveTest.Location = New System.Drawing.Point(1422, 632)
        Me.lblMoveTest.Name = "lblMoveTest"
        Me.lblMoveTest.Size = New System.Drawing.Size(286, 45)
        Me.lblMoveTest.TabIndex = 5
        Me.lblMoveTest.Text = "Generated Moves"
        Me.lblMoveTest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblMoveTest.Visible = False
        '
        'btnMoveTest
        '
        Me.btnMoveTest.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnMoveTest.Font = New System.Drawing.Font("Calibri", 12.0!)
        Me.btnMoveTest.Location = New System.Drawing.Point(1660, 655)
        Me.btnMoveTest.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnMoveTest.Name = "btnMoveTest"
        Me.btnMoveTest.Size = New System.Drawing.Size(195, 105)
        Me.btnMoveTest.TabIndex = 4
        Me.btnMoveTest.Text = "Move Test"
        Me.btnMoveTest.UseVisualStyleBackColor = True
        '
        'lblBlackPlayer
        '
        Me.lblBlackPlayer.AutoSize = True
        Me.lblBlackPlayer.Font = New System.Drawing.Font("Calibri", 26.0!)
        Me.lblBlackPlayer.ForeColor = System.Drawing.Color.White
        Me.lblBlackPlayer.Location = New System.Drawing.Point(980, 11)
        Me.lblBlackPlayer.Name = "lblBlackPlayer"
        Me.lblBlackPlayer.Size = New System.Drawing.Size(125, 54)
        Me.lblBlackPlayer.TabIndex = 3
        Me.lblBlackPlayer.Text = "Chess"
        '
        'lblWhitePlayer
        '
        Me.lblWhitePlayer.AutoSize = True
        Me.lblWhitePlayer.Font = New System.Drawing.Font("Calibri", 26.0!)
        Me.lblWhitePlayer.ForeColor = System.Drawing.Color.White
        Me.lblWhitePlayer.Location = New System.Drawing.Point(979, 795)
        Me.lblWhitePlayer.Name = "lblWhitePlayer"
        Me.lblWhitePlayer.Size = New System.Drawing.Size(125, 54)
        Me.lblWhitePlayer.TabIndex = 3
        Me.lblWhitePlayer.Text = "Chess"
        '
        'lblBlackDescription
        '
        Me.lblBlackDescription.AutoSize = True
        Me.lblBlackDescription.Font = New System.Drawing.Font("Calibri", 16.0!)
        Me.lblBlackDescription.ForeColor = System.Drawing.Color.White
        Me.lblBlackDescription.Location = New System.Drawing.Point(980, 64)
        Me.lblBlackDescription.Name = "lblBlackDescription"
        Me.lblBlackDescription.Size = New System.Drawing.Size(78, 33)
        Me.lblBlackDescription.TabIndex = 3
        Me.lblBlackDescription.Text = "Chess"
        '
        'lblWhiteDescription
        '
        Me.lblWhiteDescription.AutoSize = True
        Me.lblWhiteDescription.Font = New System.Drawing.Font("Calibri", 16.0!)
        Me.lblWhiteDescription.ForeColor = System.Drawing.Color.White
        Me.lblWhiteDescription.Location = New System.Drawing.Point(980, 848)
        Me.lblWhiteDescription.Name = "lblWhiteDescription"
        Me.lblWhiteDescription.Size = New System.Drawing.Size(78, 33)
        Me.lblWhiteDescription.TabIndex = 3
        Me.lblWhiteDescription.Text = "Chess"
        '
        'lblVS
        '
        Me.lblVS.AutoSize = True
        Me.lblVS.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVS.ForeColor = System.Drawing.Color.White
        Me.lblVS.Location = New System.Drawing.Point(1036, 430)
        Me.lblVS.Name = "lblVS"
        Me.lblVS.Size = New System.Drawing.Size(38, 29)
        Me.lblVS.TabIndex = 3
        Me.lblVS.Text = "VS"
        '
        'lblPlayerTurn
        '
        Me.lblPlayerTurn.AutoSize = True
        Me.lblPlayerTurn.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblPlayerTurn.ForeColor = System.Drawing.Color.White
        Me.lblPlayerTurn.Location = New System.Drawing.Point(1421, 223)
        Me.lblPlayerTurn.Name = "lblPlayerTurn"
        Me.lblPlayerTurn.Size = New System.Drawing.Size(187, 45)
        Me.lblPlayerTurn.TabIndex = 3
        Me.lblPlayerTurn.Text = "Player Turn"
        Me.lblPlayerTurn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblPlayerTurn.Visible = False
        '
        'lblBestEvaluation
        '
        Me.lblBestEvaluation.AutoSize = True
        Me.lblBestEvaluation.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblBestEvaluation.ForeColor = System.Drawing.Color.White
        Me.lblBestEvaluation.Location = New System.Drawing.Point(1421, 497)
        Me.lblBestEvaluation.Name = "lblBestEvaluation"
        Me.lblBestEvaluation.Size = New System.Drawing.Size(248, 45)
        Me.lblBestEvaluation.TabIndex = 3
        Me.lblBestEvaluation.Text = "Best Evaluation"
        Me.lblBestEvaluation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblBestEvaluation.Visible = False
        '
        'lblBranchesPruned
        '
        Me.lblBranchesPruned.AutoSize = True
        Me.lblBranchesPruned.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblBranchesPruned.ForeColor = System.Drawing.Color.White
        Me.lblBranchesPruned.Location = New System.Drawing.Point(1421, 564)
        Me.lblBranchesPruned.Name = "lblBranchesPruned"
        Me.lblBranchesPruned.Size = New System.Drawing.Size(271, 45)
        Me.lblBranchesPruned.TabIndex = 3
        Me.lblBranchesPruned.Text = "Branches Pruned"
        Me.lblBranchesPruned.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblBranchesPruned.Visible = False
        '
        'btnUndoMove
        '
        Me.btnUndoMove.Font = New System.Drawing.Font("Calibri", 12.0!)
        Me.btnUndoMove.Location = New System.Drawing.Point(1554, 794)
        Me.btnUndoMove.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnUndoMove.Name = "btnUndoMove"
        Me.btnUndoMove.Size = New System.Drawing.Size(195, 105)
        Me.btnUndoMove.TabIndex = 7
        Me.btnUndoMove.Text = "Undo Move"
        Me.btnUndoMove.UseVisualStyleBackColor = True
        '
        'btnStartGame
        '
        Me.btnStartGame.Enabled = False
        Me.btnStartGame.Font = New System.Drawing.Font("Calibri", 12.0!)
        Me.btnStartGame.Location = New System.Drawing.Point(1355, 794)
        Me.btnStartGame.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnStartGame.Name = "btnStartGame"
        Me.btnStartGame.Size = New System.Drawing.Size(195, 105)
        Me.btnStartGame.TabIndex = 8
        Me.btnStartGame.Text = "Start Game"
        Me.btnStartGame.UseVisualStyleBackColor = True
        Me.btnStartGame.Visible = False
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Font = New System.Drawing.Font("Calibri", 12.0!)
        Me.btnClose.Location = New System.Drawing.Point(1752, 794)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(195, 105)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'lblSelectedPiece
        '
        Me.lblSelectedPiece.AutoSize = True
        Me.lblSelectedPiece.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblSelectedPiece.ForeColor = System.Drawing.Color.White
        Me.lblSelectedPiece.Location = New System.Drawing.Point(1421, 297)
        Me.lblSelectedPiece.Name = "lblSelectedPiece"
        Me.lblSelectedPiece.Size = New System.Drawing.Size(243, 45)
        Me.lblSelectedPiece.TabIndex = 3
        Me.lblSelectedPiece.Text = "Selected Piece:"
        Me.lblSelectedPiece.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblSelectedPiece.Visible = False
        '
        'lblMovesEvaluated
        '
        Me.lblMovesEvaluated.AutoSize = True
        Me.lblMovesEvaluated.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblMovesEvaluated.ForeColor = System.Drawing.Color.White
        Me.lblMovesEvaluated.Location = New System.Drawing.Point(1417, 430)
        Me.lblMovesEvaluated.Name = "lblMovesEvaluated"
        Me.lblMovesEvaluated.Size = New System.Drawing.Size(274, 45)
        Me.lblMovesEvaluated.TabIndex = 3
        Me.lblMovesEvaluated.Text = "Moves Evaluated"
        Me.lblMovesEvaluated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblMovesEvaluated.Visible = False
        '
        'lblSearchDepth
        '
        Me.lblSearchDepth.AutoSize = True
        Me.lblSearchDepth.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblSearchDepth.ForeColor = System.Drawing.Color.White
        Me.lblSearchDepth.Location = New System.Drawing.Point(1421, 368)
        Me.lblSearchDepth.Name = "lblSearchDepth"
        Me.lblSearchDepth.Size = New System.Drawing.Size(219, 45)
        Me.lblSearchDepth.TabIndex = 5
        Me.lblSearchDepth.Text = "Search Depth"
        Me.lblSearchDepth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblSearchDepth.Visible = False
        '
        'lblCheckMessage
        '
        Me.lblCheckMessage.AutoSize = True
        Me.lblCheckMessage.Font = New System.Drawing.Font("Calibri", 22.0!)
        Me.lblCheckMessage.ForeColor = System.Drawing.Color.White
        Me.lblCheckMessage.Location = New System.Drawing.Point(1421, 169)
        Me.lblCheckMessage.Name = "lblCheckMessage"
        Me.lblCheckMessage.Size = New System.Drawing.Size(249, 45)
        Me.lblCheckMessage.TabIndex = 3
        Me.lblCheckMessage.Text = "Check Message"
        Me.lblCheckMessage.Visible = False
        '
        'ChessGame
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(15, Byte), Integer), CType(CType(15, Byte), Integer))
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(1924, 950)
        Me.Controls.Add(Me.btnStartGame)
        Me.Controls.Add(Me.btnUndoMove)
        Me.Controls.Add(Me.lblSearchDepth)
        Me.Controls.Add(Me.lblMoveTest)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnMoveTest)
        Me.Controls.Add(Me.lblVS)
        Me.Controls.Add(Me.lblWhiteDescription)
        Me.Controls.Add(Me.lblWhitePlayer)
        Me.Controls.Add(Me.lblMovesEvaluated)
        Me.Controls.Add(Me.lblBranchesPruned)
        Me.Controls.Add(Me.lblBestEvaluation)
        Me.Controls.Add(Me.lblSelectedPiece)
        Me.Controls.Add(Me.lblCheckMessage)
        Me.Controls.Add(Me.lblPlayerTurn)
        Me.Controls.Add(Me.lblBlackDescription)
        Me.Controls.Add(Me.lblBlackPlayer)
        Me.DoubleBuffered = True
        Me.ForeColor = System.Drawing.Color.Black
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.MaximumSize = New System.Drawing.Size(2999, 1998)
        Me.Name = "ChessGame"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Chess Engine"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnMoveTest As Button
    Friend WithEvents lblMoveTest As Label
    Friend WithEvents lblBlackPlayer As Label
    Friend WithEvents lblWhitePlayer As Label
    Friend WithEvents lblBlackDescription As Label
    Friend WithEvents lblWhiteDescription As Label
    Friend WithEvents lblVS As Label
    Friend WithEvents lblPlayerTurn As Label
    Friend WithEvents lblBestEvaluation As Label
    Friend WithEvents lblBranchesPruned As Label
    Friend WithEvents btnUndoMove As Button
    Friend WithEvents btnStartGame As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents lblSelectedPiece As Label
    Friend WithEvents lblMovesEvaluated As Label
    Friend WithEvents lblSearchDepth As Label
    Friend WithEvents lblCheckMessage As Label
End Class
