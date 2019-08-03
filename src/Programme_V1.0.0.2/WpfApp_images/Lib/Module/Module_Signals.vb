Public Module Module_Signals
    Private Function SignalCarre(pos As Single, len As Single) As Single
        Dim Position = Math.Abs(pos / len)
        If Position <= CSng(0.75) Then
            Return CSng(2.0)
        Else
            Return CSng(0.0)
        End If
    End Function

    Private Function SignalTriangle(pos As Single, len As Single) As Single
        Dim Position = 1 - Math.Abs(pos / len)
        Return Position * CSng(2.0)
    End Function


    Private Function SignalSinus(pos As Single, len As Single) As Single
        Dim Position = (1 - Math.Abs(CDbl(pos / len))) * (Math.PI / 2.0)
        Return CSng(Math.Sin(Position) * 2.0)
    End Function

    Public Function Signal(pos As Single, len As Single, window As MainWindow) As Single
        If window.RadioButton_Transition_Carré.IsChecked Then
            Return SignalCarre(pos, len)
        ElseIf window.RadioButton_Transition_Triangle.IsChecked Then
            Return SignalTriangle(pos, len)
        ElseIf window.RadioButton_Transition_Sinus.IsChecked Then
            Return SignalSinus(pos, len)
        Else
            Return CSng(0.0)
        End If
    End Function

End Module
