Imports System

Module Program
    Private ReadOnly Multipliers As Decimal() = {5D, 3D, 2D, 1.2D, 0.4D, 1.2D, 2D, 3D, 5D}
    Private ReadOnly Weights As Integer() = {3, 7, 12, 18, 20, 18, 12, 7, 3}
    Private Balance As Integer = 1000
    Private ReadOnly Rng As New Random()

    Sub Main()
        Console.Title = "Plinko Casino"
        ShowHome()
    End Sub

    Private Sub ShowHome()
        While True
            Console.Clear()
            Console.WriteLine("=== PLINKO CASINO ===")
            Console.WriteLine($"Balance: {Balance} coins")
            Console.WriteLine()
            Console.WriteLine("1) Play Plinko")
            Console.WriteLine("2) Exit")
            Console.Write("Choose an option: ")

            Dim choice = Console.ReadLine()
            If choice = "1" Then
                PlayRoundLoop()
            ElseIf choice = "2" Then
                Return
            Else
                ShowMessage("Invalid menu option.")
            End If
        End While
    End Sub

    Private Sub PlayRoundLoop()
        While True
            Console.Clear()
            DrawPyramidBoard()
            DrawMultipliers()
            Console.WriteLine()
            Console.WriteLine($"Balance: {Balance} coins")
            Console.WriteLine("Enter a bet amount, H for home, or Q to quit.")
            Console.Write("Bet: ")

            Dim input = Console.ReadLine()
            If input Is Nothing Then
                Continue While
            End If

            input = input.Trim().ToUpperInvariant()
            If input = "H" Then
                Return
            End If

            If input = "Q" Then
                Environment.Exit(0)
            End If

            Dim bet As Integer
            If Not Integer.TryParse(input, bet) OrElse bet < 1 Then
                ShowMessage("Please enter a valid bet of at least 1.")
                Continue While
            End If

            If bet > Balance Then
                ShowMessage("Not enough coins for that bet.")
                Continue While
            End If

            ResolveDrop(bet)

            If Balance <= 0 Then
                ShowMessage("You ran out of coins. Resetting balance to 1000.")
                Balance = 1000
            End If
        End While
    End Sub

    Private Sub ResolveDrop(bet As Integer)
        Balance -= bet
        Dim slotIndex = PickSlotIndex()
        Dim multiplier = Multipliers(slotIndex)
        Dim payout = CInt(Math.Floor(bet * multiplier))
        Balance += payout

        Console.WriteLine()
        Console.WriteLine("Dropping ball...")
        System.Threading.Thread.Sleep(400)
        Console.WriteLine($"Landed in slot #{slotIndex + 1} ({multiplier}x)")

        Dim net = payout - bet
        Dim netPrefix = If(net >= 0, "+", String.Empty)
        Console.WriteLine($"Payout: {payout} coins ({netPrefix}{net})")
        Console.WriteLine($"New Balance: {Balance} coins")
        Console.WriteLine("Press ENTER to continue...")
        Console.ReadLine()
    End Sub

    Private Function PickSlotIndex() As Integer
        Dim totalWeight = 0
        For Each w In Weights
            totalWeight += w
        Next

        Dim roll = Rng.Next(1, totalWeight + 1)
        For i = 0 To Weights.Length - 1
            roll -= Weights(i)
            If roll <= 0 Then
                Return i
            End If
        Next

        Return Weights.Length - 1
    End Function

    Private Sub DrawPyramidBoard()
        Console.WriteLine("Plinko Pyramid")
        Console.WriteLine("(lowest multiplier in the center, highest on both sides)")
        Console.WriteLine()

        Dim rows As Integer = 9
        For row = 0 To rows - 1
            Dim indent = New String(" "c, (rows - row - 1) * 2)
            Console.Write(indent)
            For peg = 0 To row
                Console.Write("●   ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub

    Private Sub DrawMultipliers()
        For i = 0 To Multipliers.Length - 1
            Console.Write($"[{Multipliers(i),4}x]")
        Next
        Console.WriteLine()
    End Sub

    Private Sub ShowMessage(message As String)
        Console.WriteLine()
        Console.WriteLine(message)
        Console.WriteLine("Press ENTER to continue...")
        Console.ReadLine()
    End Sub
End Module
