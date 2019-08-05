Public Module Module_TuileSignals
    Private Function TuileCarré(CH As Integer, CW As Integer,
                                h As Integer, w As Integer,
                                PasH As Integer, PasW As Integer,
                                windows As MainWindow) As Single
        Dim WX = Math.Min(Math.Abs(CW - w) / PasW, 1.0)

        Dim HY = Math.Min(Math.Abs(CH - h) / PasH, 1.0)
        Dim Pos = Math.Max(WX, HY)

        If WX + HY = 0 Then
            Return Module_Signals.Signal(0.0, 2.0, windows)
        ElseIf WX <> 0 And HY <> 0 Then
            'Dim Xp = CDbl(PasH) * CDbl(WX) / CDbl(HY)
            'Dim Yp = CDbl(PasW) * CDbl(HY) / CDbl(WX)
            'Dim Len = PasW
            'If CInt(Xp) > PasW Then
            '    Len = PasH
            'End If

            Return Module_Signals.Signal(CSng(Pos), CSng(1.0), windows)
        Else
            If Math.Abs(CW - w) = 0 Then
                Return Module_Signals.Signal(CSng(Pos), CSng(1.0), windows)
            Else
                Return Module_Signals.Signal(CSng(Pos), CSng(1.0), windows)
            End If
        End If


    End Function


    Private Function TuileCirculaire(CH As Integer, CW As Integer,
                               h As Integer, w As Integer,
                               PasH As Integer, PasW As Integer,
                               windows As MainWindow) As Single
        Dim WX = CDbl(Math.Abs(CW - w))
        Dim HY = CDbl(Math.Abs(CH - h))

        Dim Pos = Math.Sqrt(WX * WX + HY * HY)
        If WX + HY = 0.0 Then
            Return Module_Signals.Signal(0.0, 2.0, windows)
        ElseIf WX <> 0.0 And HY <> 0.0 Then
            Dim Xp = CDbl(PasH) * WX / HY
            Dim Yp = CDbl(PasW) * HY / WX
            Dim XLimite = CDbl(PasW)
            Dim YLimite = CDbl(PasH)


            If Xp < CDbl(PasW) Then
                XLimite = Xp
            End If

            If Yp < CDbl(PasH) Then
                YLimite = Yp
            End If

            Dim Len = Math.Sqrt(XLimite * XLimite + YLimite * YLimite)
            If Pos > Len Then
                Len = Pos
            End If

            Return Module_Signals.Signal(CSng(Pos), CSng(Len), windows)
        Else
            If WX = 0 Then
                Return Module_Signals.Signal(CSng(Pos), CSng(PasH), windows)
            Else
                Return Module_Signals.Signal(CSng(Pos), CSng(PasW), windows)
            End If
        End If
    End Function

    Private Function TuileCombiné(CH As Integer, CW As Integer,
                               h As Integer, w As Integer,
                               PasH As Integer, PasW As Integer,
                               windows As MainWindow) As Single
        Dim WX = Math.Abs(CW - w)
        Dim HY = Math.Abs(CH - h)

        Dim PosW = WX
        Dim PosH = HY

        Dim CoefH = Module_Signals.Signal(CSng(PosH), CSng(PasH), windows)
        Dim CoefW = Module_Signals.Signal(CSng(PosW), CSng(PasW), windows)

        Return CoefW + CoefH
    End Function


    Public Function Tuiles(CH As Integer, CW As Integer,
                               h As Integer, w As Integer,
                               PasH As Integer, PasW As Integer,
                               windows As MainWindow) As Single
        If windows.RadioButton_Tuile_Carré.IsChecked Then
            Return TuileCarré(CH, CW, h, w, PasH, PasW, windows)
        ElseIf windows.RadioButton_Tuile_Circulaire.IsChecked Then
            Return TuileCirculaire(CH, CW, h, w, PasH, PasW, windows)
        ElseIf windows.RadioButton_Tuile_Combinée.IsChecked Then
            Return TuileCombiné(CH, CW, h, w, PasH, PasW, windows)
        Else
            Return CSng(0.0)
        End If
    End Function
End Module
