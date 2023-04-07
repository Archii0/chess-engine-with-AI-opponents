<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class StartMenu
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
        Me.btnStart = New System.Windows.Forms.Button()
        Me.tbWhitePlayer = New System.Windows.Forms.TrackBar()
        Me.tbBlackPlayer = New System.Windows.Forms.TrackBar()
        Me.lblSlider2 = New System.Windows.Forms.Label()
        Me.lblSlider1 = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblBlackPlayer = New System.Windows.Forms.Label()
        Me.lblWhitePlayer = New System.Windows.Forms.Label()
        Me.lblSlider1Desc = New System.Windows.Forms.Label()
        Me.lblSlider2Desc = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbAI1Time = New System.Windows.Forms.ComboBox()
        Me.cbAI2Time = New System.Windows.Forms.ComboBox()
        Me.lblAI2Time = New System.Windows.Forms.Label()
        Me.lblAI1Time = New System.Windows.Forms.Label()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.cbChangeStartFEN = New System.Windows.Forms.CheckBox()
        Me.picLogo = New System.Windows.Forms.PictureBox()
        Me.lblFEN = New System.Windows.Forms.Label()
        Me.tbCustomFEN = New System.Windows.Forms.TextBox()
        Me.btnMoveTestStart = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        CType(Me.tbWhitePlayer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbBlackPlayer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnStart
        '
        Me.btnStart.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnStart.Font = New System.Drawing.Font("Book Antiqua", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStart.Location = New System.Drawing.Point(458, 658)
        Me.btnStart.Margin = New System.Windows.Forms.Padding(2)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(185, 104)
        Me.btnStart.TabIndex = 0
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'tbWhitePlayer
        '
        Me.tbWhitePlayer.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbWhitePlayer.LargeChange = 1
        Me.tbWhitePlayer.Location = New System.Drawing.Point(18, 219)
        Me.tbWhitePlayer.Margin = New System.Windows.Forms.Padding(2)
        Me.tbWhitePlayer.Maximum = 4
        Me.tbWhitePlayer.Name = "tbWhitePlayer"
        Me.tbWhitePlayer.Size = New System.Drawing.Size(362, 45)
        Me.tbWhitePlayer.TabIndex = 2
        Me.tbWhitePlayer.TickStyle = System.Windows.Forms.TickStyle.Both
        '
        'tbBlackPlayer
        '
        Me.tbBlackPlayer.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbBlackPlayer.AutoSize = False
        Me.tbBlackPlayer.Cursor = System.Windows.Forms.Cursors.Default
        Me.tbBlackPlayer.LargeChange = 1
        Me.tbBlackPlayer.Location = New System.Drawing.Point(741, 219)
        Me.tbBlackPlayer.Margin = New System.Windows.Forms.Padding(2)
        Me.tbBlackPlayer.Maximum = 4
        Me.tbBlackPlayer.Name = "tbBlackPlayer"
        Me.tbBlackPlayer.Size = New System.Drawing.Size(362, 46)
        Me.tbBlackPlayer.TabIndex = 2
        Me.tbBlackPlayer.TickStyle = System.Windows.Forms.TickStyle.Both
        '
        'lblSlider2
        '
        Me.lblSlider2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblSlider2.Font = New System.Drawing.Font("Book Antiqua", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSlider2.Location = New System.Drawing.Point(743, 301)
        Me.lblSlider2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSlider2.Name = "lblSlider2"
        Me.lblSlider2.Size = New System.Drawing.Size(350, 31)
        Me.lblSlider2.TabIndex = 3
        Me.lblSlider2.Text = "Player"
        Me.lblSlider2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSlider1
        '
        Me.lblSlider1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblSlider1.Font = New System.Drawing.Font("Book Antiqua", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSlider1.Location = New System.Drawing.Point(29, 301)
        Me.lblSlider1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSlider1.Name = "lblSlider1"
        Me.lblSlider1.Size = New System.Drawing.Size(350, 31)
        Me.lblSlider1.TabIndex = 3
        Me.lblSlider1.Text = "Player"
        Me.lblSlider1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTitle
        '
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.lblTitle.Font = New System.Drawing.Font("Book Antiqua", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(1112, 89)
        Me.lblTitle.TabIndex = 4
        Me.lblTitle.Text = "Chess"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBlackPlayer
        '
        Me.lblBlackPlayer.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblBlackPlayer.Font = New System.Drawing.Font("Book Antiqua", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBlackPlayer.Location = New System.Drawing.Point(748, 150)
        Me.lblBlackPlayer.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBlackPlayer.Name = "lblBlackPlayer"
        Me.lblBlackPlayer.Size = New System.Drawing.Size(350, 31)
        Me.lblBlackPlayer.TabIndex = 3
        Me.lblBlackPlayer.Text = "Black Player"
        Me.lblBlackPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblWhitePlayer
        '
        Me.lblWhitePlayer.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblWhitePlayer.Font = New System.Drawing.Font("Book Antiqua", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWhitePlayer.Location = New System.Drawing.Point(34, 150)
        Me.lblWhitePlayer.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblWhitePlayer.Name = "lblWhitePlayer"
        Me.lblWhitePlayer.Size = New System.Drawing.Size(355, 31)
        Me.lblWhitePlayer.TabIndex = 3
        Me.lblWhitePlayer.Text = "White Player"
        Me.lblWhitePlayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSlider1Desc
        '
        Me.lblSlider1Desc.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblSlider1Desc.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSlider1Desc.Location = New System.Drawing.Point(14, 349)
        Me.lblSlider1Desc.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSlider1Desc.Name = "lblSlider1Desc"
        Me.lblSlider1Desc.Size = New System.Drawing.Size(391, 36)
        Me.lblSlider1Desc.TabIndex = 3
        Me.lblSlider1Desc.Text = "Takes user moves"
        Me.lblSlider1Desc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSlider2Desc
        '
        Me.lblSlider2Desc.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblSlider2Desc.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSlider2Desc.Location = New System.Drawing.Point(722, 349)
        Me.lblSlider2Desc.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSlider2Desc.Name = "lblSlider2Desc"
        Me.lblSlider2Desc.Size = New System.Drawing.Size(389, 36)
        Me.lblSlider2Desc.TabIndex = 3
        Me.lblSlider2Desc.Text = "Takes user moves"
        Me.lblSlider2Desc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.Font = New System.Drawing.Font("Book Antiqua", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(534, 388)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 28)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "VS"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cbAI1Time
        '
        Me.cbAI1Time.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.cbAI1Time.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbAI1Time.FormattingEnabled = True
        Me.cbAI1Time.Items.AddRange(New Object() {"As Fast As Possible", "0.5 Second Delay", "1 Second Delay", "1.5 Seconds Delay", "2 Seconds Delay"})
        Me.cbAI1Time.Location = New System.Drawing.Point(106, 476)
        Me.cbAI1Time.Margin = New System.Windows.Forms.Padding(2)
        Me.cbAI1Time.Name = "cbAI1Time"
        Me.cbAI1Time.Size = New System.Drawing.Size(188, 27)
        Me.cbAI1Time.TabIndex = 6
        Me.cbAI1Time.Text = "As Fast As Possible"
        '
        'cbAI2Time
        '
        Me.cbAI2Time.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.cbAI2Time.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbAI2Time.FormattingEnabled = True
        Me.cbAI2Time.Items.AddRange(New Object() {"As Fast As Possible", "0.5 Second Delay", "1 Second Delay", "1.5 Seconds Delay", "2 Seconds Delay"})
        Me.cbAI2Time.Location = New System.Drawing.Point(824, 476)
        Me.cbAI2Time.Margin = New System.Windows.Forms.Padding(2)
        Me.cbAI2Time.Name = "cbAI2Time"
        Me.cbAI2Time.Size = New System.Drawing.Size(188, 27)
        Me.cbAI2Time.TabIndex = 6
        Me.cbAI2Time.Text = "As Fast As Possible"
        '
        'lblAI2Time
        '
        Me.lblAI2Time.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblAI2Time.Font = New System.Drawing.Font("Book Antiqua", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAI2Time.Location = New System.Drawing.Point(765, 434)
        Me.lblAI2Time.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblAI2Time.Name = "lblAI2Time"
        Me.lblAI2Time.Size = New System.Drawing.Size(297, 28)
        Me.lblAI2Time.TabIndex = 3
        Me.lblAI2Time.Text = "AI Response Time"
        Me.lblAI2Time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblAI1Time
        '
        Me.lblAI1Time.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblAI1Time.Font = New System.Drawing.Font("Book Antiqua", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAI1Time.Location = New System.Drawing.Point(56, 434)
        Me.lblAI1Time.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblAI1Time.Name = "lblAI1Time"
        Me.lblAI1Time.Size = New System.Drawing.Size(297, 28)
        Me.lblAI1Time.TabIndex = 3
        Me.lblAI1Time.Text = "AI Response Time"
        Me.lblAI1Time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cbChangeStartFEN
        '
        Me.cbChangeStartFEN.AutoSize = True
        Me.cbChangeStartFEN.Font = New System.Drawing.Font("Book Antiqua", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbChangeStartFEN.Location = New System.Drawing.Point(704, 633)
        Me.cbChangeStartFEN.Name = "cbChangeStartFEN"
        Me.cbChangeStartFEN.Size = New System.Drawing.Size(279, 31)
        Me.cbChangeStartFEN.TabIndex = 7
        Me.cbChangeStartFEN.Text = "Change Starting Position"
        Me.cbChangeStartFEN.UseVisualStyleBackColor = True
        '
        'picLogo
        '
        Me.picLogo.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.picLogo.Image = Global.Chess__Project_.My.Resources.Resources.ChessLogo
        Me.picLogo.Location = New System.Drawing.Point(394, 122)
        Me.picLogo.Margin = New System.Windows.Forms.Padding(2)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(329, 263)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 10
        Me.picLogo.TabStop = False
        '
        'lblFEN
        '
        Me.lblFEN.AutoSize = True
        Me.lblFEN.Font = New System.Drawing.Font("Book Antiqua", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFEN.Location = New System.Drawing.Point(701, 678)
        Me.lblFEN.Name = "lblFEN"
        Me.lblFEN.Size = New System.Drawing.Size(135, 20)
        Me.lblFEN.TabIndex = 9
        Me.lblFEN.Text = "Insert FEN String:"
        '
        'tbCustomFEN
        '
        Me.tbCustomFEN.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCustomFEN.Location = New System.Drawing.Point(704, 701)
        Me.tbCustomFEN.Multiline = True
        Me.tbCustomFEN.Name = "tbCustomFEN"
        Me.tbCustomFEN.Size = New System.Drawing.Size(389, 59)
        Me.tbCustomFEN.TabIndex = 8
        '
        'btnMoveTestStart
        '
        Me.btnMoveTestStart.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnMoveTestStart.Font = New System.Drawing.Font("Book Antiqua", 11.0!, System.Drawing.FontStyle.Bold)
        Me.btnMoveTestStart.Location = New System.Drawing.Point(38, 36)
        Me.btnMoveTestStart.Margin = New System.Windows.Forms.Padding(2)
        Me.btnMoveTestStart.Name = "btnMoveTestStart"
        Me.btnMoveTestStart.Size = New System.Drawing.Size(101, 52)
        Me.btnMoveTestStart.TabIndex = 0
        Me.btnMoveTestStart.Text = "Move Test"
        Me.btnMoveTestStart.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Silver
        Me.GroupBox1.Controls.Add(Me.btnMoveTestStart)
        Me.GroupBox1.Font = New System.Drawing.Font("Book Antiqua", 10.0!)
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox1.Location = New System.Drawing.Point(10, 664)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(185, 100)
        Me.GroupBox1.TabIndex = 11
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Engine Move Testing"
        '
        'StartMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(1112, 774)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.tbCustomFEN)
        Me.Controls.Add(Me.cbChangeStartFEN)
        Me.Controls.Add(Me.lblFEN)
        Me.Controls.Add(Me.picLogo)
        Me.Controls.Add(Me.cbAI2Time)
        Me.Controls.Add(Me.cbAI1Time)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblWhitePlayer)
        Me.Controls.Add(Me.lblSlider1Desc)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblSlider1)
        Me.Controls.Add(Me.lblBlackPlayer)
        Me.Controls.Add(Me.lblAI1Time)
        Me.Controls.Add(Me.lblAI2Time)
        Me.Controls.Add(Me.lblSlider2Desc)
        Me.Controls.Add(Me.lblSlider2)
        Me.Controls.Add(Me.tbBlackPlayer)
        Me.Controls.Add(Me.tbWhitePlayer)
        Me.Controls.Add(Me.btnStart)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "StartMenu"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Start Menu"
        CType(Me.tbWhitePlayer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbBlackPlayer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnStart As Button
    Friend WithEvents tbWhitePlayer As TrackBar
    Friend WithEvents tbBlackPlayer As TrackBar
    Friend WithEvents lblSlider2 As Label
    Friend WithEvents lblSlider1 As Label
    Friend WithEvents lblTitle As Label
    Friend WithEvents lblBlackPlayer As Label
    Friend WithEvents lblWhitePlayer As Label
    Friend WithEvents lblSlider1Desc As Label
    Friend WithEvents lblSlider2Desc As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents cbAI1Time As ComboBox
    Friend WithEvents cbAI2Time As ComboBox
    Friend WithEvents lblAI2Time As Label
    Friend WithEvents lblAI1Time As Label
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents cbChangeStartFEN As CheckBox
    Friend WithEvents picLogo As PictureBox
    Friend WithEvents lblFEN As Label
    Friend WithEvents tbCustomFEN As TextBox
    Friend WithEvents btnMoveTestStart As Button
    Friend WithEvents GroupBox1 As GroupBox
End Class
