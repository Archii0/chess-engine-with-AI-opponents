<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ResultMessage
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.btnPlayAgain = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblResult = New System.Windows.Forms.Label()
        Me.lblWinningSide = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblTitle.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblTitle.Font = New System.Drawing.Font("Book Antiqua", 22.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(201, 38)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(189, 39)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Game Result"
        '
        'btnPlayAgain
        '
        Me.btnPlayAgain.Location = New System.Drawing.Point(104, 274)
        Me.btnPlayAgain.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPlayAgain.Name = "btnPlayAgain"
        Me.btnPlayAgain.Size = New System.Drawing.Size(104, 54)
        Me.btnPlayAgain.TabIndex = 1
        Me.btnPlayAgain.Text = "Play Again"
        Me.btnPlayAgain.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(422, 262)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(2)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(94, 66)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "Close Application"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'lblResult
        '
        Me.lblResult.AutoSize = True
        Me.lblResult.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblResult.Font = New System.Drawing.Font("Book Antiqua", 22.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblResult.Location = New System.Drawing.Point(208, 116)
        Me.lblResult.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblResult.Name = "lblResult"
        Me.lblResult.Size = New System.Drawing.Size(164, 37)
        Me.lblResult.TabIndex = 0
        Me.lblResult.Text = "Checkmate"
        '
        'lblWinningSide
        '
        Me.lblWinningSide.AutoSize = True
        Me.lblWinningSide.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblWinningSide.Font = New System.Drawing.Font("Book Antiqua", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWinningSide.Location = New System.Drawing.Point(216, 171)
        Me.lblWinningSide.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblWinningSide.Name = "lblWinningSide"
        Me.lblWinningSide.Size = New System.Drawing.Size(138, 28)
        Me.lblWinningSide.TabIndex = 0
        Me.lblWinningSide.Text = "White Wins"
        '
        'ResultMessage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(600, 366)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnPlayAgain)
        Me.Controls.Add(Me.lblWinningSide)
        Me.Controls.Add(Me.lblResult)
        Me.Controls.Add(Me.lblTitle)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "ResultMessage"
        Me.Text = "Game Result"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As Label
    Friend WithEvents btnPlayAgain As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents lblResult As Label
    Friend WithEvents lblWinningSide As Label
End Class
