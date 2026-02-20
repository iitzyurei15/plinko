Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Class MainForm
    Inherits Form

    Private ReadOnly Multipliers As Decimal() = {5D, 3D, 2D, 1.2D, 0.4D, 1.2D, 2D, 3D, 5D}
    Private ReadOnly Weights As Integer() = {3, 7, 12, 18, 20, 18, 12, 7, 3}

    Private ReadOnly rng As New Random()
    Private balance As Integer = 1000

    Private ReadOnly lblBalance As New Label()
    Private ReadOnly numBet As New NumericUpDown()
    Private ReadOnly btnDrop As New Button()
    Private ReadOnly btnReset As New Button()
    Private ReadOnly lblResult As New Label()
    Private ReadOnly slotLabels As New List(Of Label)()
    Private ReadOnly boardPanel As New Panel()

    Public Sub New()
        Text = "Plinko Casino"
        ClientSize = New Size(860, 680)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        StartPosition = FormStartPosition.CenterScreen
        BackColor = Color.FromArgb(15, 23, 42)

        SetupUi()
        UpdateBalance(1000)
    End Sub

    Private Sub SetupUi()
        Dim title As New Label() With {
            .Text = "Plinko",
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 24, FontStyle.Bold),
            .Location = New Point(24, 16),
            .AutoSize = True
        }

        lblBalance.Text = "Balance: 1000 coins"
        lblBalance.ForeColor = Color.FromArgb(226, 232, 240)
        lblBalance.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        lblBalance.Location = New Point(660, 26)
        lblBalance.AutoSize = True

        boardPanel.Location = New Point(24, 80)
        boardPanel.Size = New Size(800, 430)
        boardPanel.BackColor = Color.FromArgb(11, 18, 32)
        boardPanel.BorderStyle = BorderStyle.FixedSingle
        AddHandler boardPanel.Paint, AddressOf DrawPyramidBoard

        Dim betLabel As New Label() With {
            .Text = "Bet Amount",
            .ForeColor = Color.White,
            .Location = New Point(24, 535),
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }

        numBet.Location = New Point(24, 560)
        numBet.Width = 120
        numBet.Minimum = 1
        numBet.Maximum = 1000000
        numBet.Value = 25

        btnDrop.Text = "Drop Ball"
        btnDrop.Location = New Point(160, 558)
        btnDrop.Size = New Size(120, 32)
        AddHandler btnDrop.Click, AddressOf OnDropBall

        btnReset.Text = "Reset Coins"
        btnReset.Location = New Point(292, 558)
        btnReset.Size = New Size(120, 32)
        AddHandler btnReset.Click, Sub(sender, e)
                                       UpdateBalance(1000)
                                       lblResult.Text = "Balance reset to 1000 coins."
                                   End Sub

        Dim slotsPanel As New FlowLayoutPanel() With {
            .Location = New Point(24, 598),
            .Size = New Size(800, 40),
            .BackColor = Color.Transparent,
            .WrapContents = False
        }

        For Each multi In Multipliers
            Dim slot As New Label() With {
                .Text = $"{multi}x",
                .ForeColor = Color.White,
                .BackColor = Color.FromArgb(30, 41, 59),
                .TextAlign = ContentAlignment.MiddleCenter,
                .Size = New Size(84, 30),
                .Margin = New Padding(2),
                .BorderStyle = BorderStyle.FixedSingle
            }
            slotLabels.Add(slot)
            slotsPanel.Controls.Add(slot)
        Next

        lblResult.Location = New Point(430, 560)
        lblResult.Size = New Size(394, 32)
        lblResult.ForeColor = Color.FromArgb(148, 163, 184)
        lblResult.Text = "Set your bet and drop a ball."

        Controls.Add(title)
        Controls.Add(lblBalance)
        Controls.Add(boardPanel)
        Controls.Add(betLabel)
        Controls.Add(numBet)
        Controls.Add(btnDrop)
        Controls.Add(btnReset)
        Controls.Add(lblResult)
        Controls.Add(slotsPanel)
    End Sub

    Private Sub OnDropBall(sender As Object, e As EventArgs)
        Dim bet As Integer = Decimal.ToInt32(numBet.Value)

        If bet > balance Then
            lblResult.Text = "Not enough coins. Lower your bet."
            Return
        End If

        UpdateBalance(balance - bet)

        ClearSlotHighlights()

        Dim index = PickSlotIndex()
        Dim multiplier = Multipliers(index)
        Dim payout = CInt(Math.Floor(bet * multiplier))
        UpdateBalance(balance + payout)

        slotLabels(index).BackColor = Color.FromArgb(22, 163, 74)

        Dim net = payout - bet
        Dim netPrefix = If(net >= 0, "+", String.Empty)
        lblResult.Text = $"Landed on {multiplier}x. Won {payout} ({netPrefix}{net})."
    End Sub

    Private Sub DrawPyramidBoard(sender As Object, e As PaintEventArgs)
        Dim g = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        Using brush As New SolidBrush(Color.FromArgb(226, 232, 240))
            Dim rows As Integer = 11
            Dim centerX As Single = boardPanel.Width / 2.0F
            Dim topY As Single = 30.0F
            Dim rowGap As Single = 32.0F
            Dim pegGap As Single = 34.0F

            For row As Integer = 0 To rows - 1
                Dim pegsInRow = row + 1
                Dim startX = centerX - ((pegsInRow - 1) * pegGap / 2.0F)
                Dim y = topY + row * rowGap

                For peg As Integer = 0 To pegsInRow - 1
                    Dim x = startX + peg * pegGap
                    g.FillEllipse(brush, x - 3.0F, y - 3.0F, 6.0F, 6.0F)
                Next
            Next
        End Using
    End Sub

    Private Function PickSlotIndex() As Integer
        Dim total = 0
        For Each w In Weights
            total += w
        Next

        Dim roll = rng.Next(1, total + 1)
        For i As Integer = 0 To Weights.Length - 1
            roll -= Weights(i)
            If roll <= 0 Then
                Return i
            End If
        Next

        Return Weights.Length - 1
    End Function

    Private Sub ClearSlotHighlights()
        For Each slot In slotLabels
            slot.BackColor = Color.FromArgb(30, 41, 59)
        Next
    End Sub

    Private Sub UpdateBalance(nextValue As Integer)
        balance = Math.Max(0, nextValue)
        lblBalance.Text = $"Balance: {balance} coins"
    End Sub
End Class
