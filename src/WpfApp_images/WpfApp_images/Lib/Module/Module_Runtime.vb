Imports System.Drawing
Imports ImageMagick

Public Module Module_Runtime
    Public Sub Runtime(Window As MainWindow)
        Dim Str_Cbb As String = Window.ComboBox_ListSizes.SelectedItem.ToString
        Dim Key As String = Str_Cbb.Split(" "c)(0)
        Dim Width As Integer = CInt(Key.Split("x"c)(0))
        Dim Height As Integer = CInt(Key.Split("x"c)(1))

        Dim ArrayListe = Window.DicoSize.Item(Key).ToArray
        Dim ListeFiles = ArrayListe.ToList
        Dim ListeRaw = ArrayListe.ToList
        If Window.RadioButton_Parameters_AZ.IsChecked Then
            ListeFiles.Sort()

            ListeRaw.Sort()
        ElseIf Window.RadioButton_Parameters_ZA.IsChecked Then
            ListeFiles.Sort()
            ListeFiles.Reverse()

            ListeRaw.Sort()
            ListeRaw.Reverse()
        Else
            ListeFiles.Sort()
            ListeRaw.Clear()
            Dim NewListe As New List(Of String)
            Dim random As New Random()
            Do
                Dim rndnbr = random.Next(0, ListeFiles.Count - 1)
                NewListe.Add(ListeFiles.Item(rndnbr))
                ListeRaw.Add(ListeFiles.Item(rndnbr))
                ListeFiles.RemoveAt(rndnbr)
                If ListeFiles.Count = 1 Then
                    Exit Do
                End If
            Loop
            NewListe.Add(ListeFiles.Item(0))
            ListeRaw.Add(ListeFiles.Item(0))
            ListeFiles = NewListe
        End If

        If Window.RadioButton_Repetitions_Miroir.IsChecked Then
            For i As Integer = 1 To CInt(Window.NumericUpDown_Miroir.Value)
                ListeRaw.Reverse()
                ListeFiles.AddRange(ListeRaw)
            Next

        ElseIf Window.RadioButton_Repetitions_Serie.IsChecked Then
            For i As Integer = 1 To CInt(Window.NumericUpDown_Serie.Value)
                ListeFiles.AddRange(ListeRaw)
            Next
        End If

        Dim nbSection = ListeFiles.Count
        Dim PasH = CDbl(Height) / CDbl(nbSection * 1.5 + 0.5)
        Dim PasW = CDbl(Width) / CDbl(nbSection * 1.5 + 0.5)

        Dim LenH = CSng(PasH)
        Dim LenW = CSng(PasW)



        If Window.RadioButton_Alignement_V.IsChecked Then
            Class_ArrayByte.Init(Height + CInt(PasH) + 1, Width + CInt(PasW) + 1)
            Dim H1 As Integer = 0
            Dim H2 As Integer = 0
            Dim C As Integer = 0

            Dim NoImage As Integer = 0
            For Each FileImg In ListeFiles
                H1 = CInt(PasH * 1.5 * CDbl(NoImage))
                H2 = H1 + CInt(PasH * 2.0)
                C = H1 + CInt(PasH)

                Dim image = New MagickImage(FileImg)
                image.Border(CInt(PasW / 2), CInt(PasH / 2))
                For w As Integer = 0 To image.Width - 1

                    For h As Integer = H1 To H2
                        If h >= image.Height Then Exit For
                        Dim Pixel = image.GetPixels(w, h)
                        Dim Dif = CSng(Math.Abs(C - h))
                        Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                        Dim Coef = Module_Signals.Signal(Dif, LenH, Window)
                        oldPixeRH.Add(Coef, Pixel.ToColor)
                        Class_ArrayByte.SetValue(h, w, oldPixeRH)
                    Next
                Next
                NoImage += 1
                image.Dispose()
                GC.Collect()
            Next

            Dim ImgOut As New Bitmap(Height, Width)
            For w As Integer = CInt(PasH / 2.0) To Width - CInt(PasH / 2.0) - 1
                For h As Integer = CInt(PasH / 2.0) To Height - CInt(PasH / 2.0) - 1
                    Dim PixeRH = Class_ArrayByte.GetValue(h, w)
                    ImgOut.SetPixel(h - CInt(PasH / 2.0), w - CInt(PasH / 2.0), System.Drawing.Color.FromArgb(PixeRH.R, PixeRH.G, PixeRH.B))
                Next
            Next
            Dim pathFile = System.IO.Path.GetTempFileName + ".jpg"
            ImgOut.Save(pathFile)
            ImgOut.Dispose()
            MsgBox(pathFile)
        ElseIf Window.RadioButton_Alignement_H.IsChecked() Then
            Class_ArrayByte.Init(Height + CInt(PasW) + 1, Width + CInt(PasW) + 1)
            Dim W1 As Integer = 0
            Dim W2 As Integer = 0
            Dim C As Integer = 0

            Dim NoImage As Integer = 0
            For Each FileImg In ListeFiles
                W1 = CInt(PasW * 1.5 * CDbl(NoImage))
                W2 = W1 + CInt(PasW * 2.0)
                C = W1 + CInt(PasW)

                Using image = New MagickImage(FileImg)
                    image.Border(CInt(PasW / 2), CInt(PasH / 2))
                    For h As Integer = 0 To image.Height - 1
                        For w As Integer = W1 To W2
                            If w >= image.Width Then Exit For
                            Dim Pixel = image.GetPixels(w, h)
                            Dim Dif = CSng(Math.Abs(C - w))
                            Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                            Dim Coef = Module_Signals.Signal(Dif, LenW, Window)
                            oldPixeRH.Add(Coef, Pixel.ToColor)
                            Class_ArrayByte.SetValue(h, w, oldPixeRH)
                        Next
                    Next
                    NoImage += 1
                End Using
                GC.Collect()
            Next

            Dim ImgOut As New Bitmap(Height, Width)
            For w As Integer = CInt(PasW / 2.0) To Width - CInt(PasW / 2.0) - 1
                For h As Integer = CInt(PasW / 2.0) To Height - CInt(PasW / 2.0) - 1
                    Dim PixeRH = Class_ArrayByte.GetValue(h, w)
                    ImgOut.SetPixel(h - CInt(PasW / 2.0), w - CInt(PasW / 2.0), System.Drawing.Color.FromArgb(PixeRH.R, PixeRH.G, PixeRH.B))
                Next
            Next
            Dim pathFile = System.IO.Path.GetTempFileName + ".jpg"
            ImgOut.Save(pathFile)
            ImgOut.Dispose()
            MsgBox(pathFile)

        ElseIf Window.RadioButton_Alignement_C.IsChecked() Then
            Class_ArrayByte.Init(Height + CInt(PasW * 2.0) + 1, Width + CInt(PasW * 2.0) + 1)
            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75
            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5
                    Dim PoseXC = CDbl(X) * PasW * 1.5 + PasW * 1.5
                    Dim PoseYC = CDbl(Y) * PasH * 1.5 + PasH * 1.5


                    Dim DeltaX = Math.Abs(CDbl(Width + CInt(PasW)) / 2.0 - PoseXC)
                    Dim DeltaY = Math.Abs(CDbl(Height + CInt(PasH)) / 2.0 - PoseYC)
                    Dim NoPicture = CInt(Math.Round(Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY) / Diagonale, 0))


                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)

                    Using image = New MagickImage(ListeFiles.Item(NoPicture Mod (ListeFiles.Count)))
                        image.Border(CInt(PasW), CInt(PasH))
                        Dim W1 As Integer = CInt(PoseX)
                        Dim W2 As Integer = W1 + CInt(2.0 * PasW)
                        Dim CW As Integer = W1 + CInt(PasW)

                        Dim H1 As Integer = CInt(PoseY)
                        Dim H2 As Integer = H1 + CInt(2.0 * PasH)
                        Dim CH As Integer = H1 + CInt(PasH)

                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = image.GetPixels(w, h)
                                Dim Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(coef, Pixel.ToColor)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)
                            Next
                        Next

                    End Using
                    GC.Collect()
                Next
            Next


            Dim ImgOut As New Bitmap(Height, Width)
            For w As Integer = CInt(PasW) To Width - CInt(PasW) - 1
                For h As Integer = CInt(PasW) To Height - CInt(PasW) - 1
                    Dim PixeRH = Class_ArrayByte.GetValue(h, w)
                    ImgOut.SetPixel(h - CInt(PasW), w - CInt(PasW), System.Drawing.Color.FromArgb(PixeRH.R, PixeRH.G, PixeRH.B))
                Next
            Next
            Dim pathFile = System.IO.Path.GetTempFileName + ".jpg"
            ImgOut.Save(pathFile)
            ImgOut.Dispose()
            MsgBox(pathFile)

        ElseIf Window.RadioButton_Alignement_D1.IsChecked() Then
            Class_ArrayByte.Init(Height + CInt(PasW * 2.0) + 1, Width + CInt(PasW * 2.0) + 1)
            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75 * 2
            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5
                    Dim PoseXC = CDbl(X) * PasW * 1.5 + PasW * 1.5
                    Dim PoseYC = CDbl(Y) * PasH * 1.5 + PasH * 1.5


                    Dim DeltaX = Math.Abs(0 - PoseXC)
                    Dim DeltaY = Math.Abs(0 - PoseYC)
                    Dim NoPicture = CInt(Math.Round(Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY) / Diagonale, 0))


                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)

                    Using image = New MagickImage(ListeFiles.Item(NoPicture Mod (ListeFiles.Count)))
                        image.Border(CInt(PasW), CInt(PasH))
                        Dim W1 As Integer = CInt(PoseX)
                        Dim W2 As Integer = W1 + CInt(2.0 * PasW)
                        Dim CW As Integer = W1 + CInt(PasW)

                        Dim H1 As Integer = CInt(PoseY)
                        Dim H2 As Integer = H1 + CInt(2.0 * PasH)
                        Dim CH As Integer = H1 + CInt(PasH)

                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = image.GetPixels(w, h)
                                Dim Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(coef, Pixel.ToColor)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)
                            Next
                        Next

                    End Using
                    GC.Collect()
                Next
            Next


            Dim ImgOut As New Bitmap(Height, Width)
            For w As Integer = CInt(PasW) To Width - CInt(PasW) - 1
                For h As Integer = CInt(PasW) To Height - CInt(PasW) - 1
                    Dim PixeRH = Class_ArrayByte.GetValue(h, w)
                    ImgOut.SetPixel(h - CInt(PasW), w - CInt(PasW), System.Drawing.Color.FromArgb(PixeRH.R, PixeRH.G, PixeRH.B))
                Next
            Next
            Dim pathFile = System.IO.Path.GetTempFileName + ".jpg"
            ImgOut.Save(pathFile)
            ImgOut.Dispose()
            MsgBox(pathFile)


        ElseIf Window.RadioButton_Alignement_D2.IsChecked() Then
            Class_ArrayByte.Init(Height + CInt(PasW * 2.0) + 1, Width + CInt(PasW * 2.0) + 1)
            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75 * 2
            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5
                    Dim PoseXC = CDbl(X) * PasW * 1.5 + PasW * 1.5
                    Dim PoseYC = CDbl(Y) * PasH * 1.5 + PasH * 1.5


                    Dim DeltaX = Math.Abs((ListeFiles.Count - 1) * PasW * 1.5 + PasW * 1.5 - PoseXC)
                    Dim DeltaY = Math.Abs(0 - PoseYC)
                    Dim NoPicture = CInt(Math.Round(Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY) / Diagonale, 0))


                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)

                    Using image = New MagickImage(ListeFiles.Item(NoPicture Mod (ListeFiles.Count)))
                        image.Border(CInt(PasW), CInt(PasH))
                        Dim W1 As Integer = CInt(PoseX)
                        Dim W2 As Integer = W1 + CInt(2.0 * PasW)
                        Dim CW As Integer = W1 + CInt(PasW)

                        Dim H1 As Integer = CInt(PoseY)
                        Dim H2 As Integer = H1 + CInt(2.0 * PasH)
                        Dim CH As Integer = H1 + CInt(PasH)

                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = image.GetPixels(w, h)
                                Dim Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(coef, Pixel.ToColor)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)
                            Next
                        Next

                    End Using
                    GC.Collect()
                Next
            Next


            Dim ImgOut As New Bitmap(Height, Width)
            For w As Integer = CInt(PasW) To Width - CInt(PasW) - 1
                For h As Integer = CInt(PasW) To Height - CInt(PasW) - 1
                    Dim PixeRH = Class_ArrayByte.GetValue(h, w)
                    ImgOut.SetPixel(h - CInt(PasW), w - CInt(PasW), System.Drawing.Color.FromArgb(PixeRH.R, PixeRH.G, PixeRH.B))
                Next
            Next
            Dim pathFile = System.IO.Path.GetTempFileName + ".jpg"
            ImgOut.Save(pathFile)
            ImgOut.Dispose()
            MsgBox(pathFile)

        ElseIf Window.RadioButton_Alignement_Rdm.IsChecked() Then
            Class_ArrayByte.Init(Height + CInt(PasW * 2.0) + 1, Width + CInt(PasW * 2.0) + 1)
            Dim random As New Random()
            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75 * 2
            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5

                    Dim NoPicture = random.Next(0, ListeFiles.Count - 1)

                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)

                    Using image = New MagickImage(ListeFiles.Item(NoPicture Mod (ListeFiles.Count)))
                        image.Border(CInt(PasW), CInt(PasH))
                        Dim W1 As Integer = CInt(PoseX)
                        Dim W2 As Integer = W1 + CInt(2.0 * PasW)
                        Dim CW As Integer = W1 + CInt(PasW)

                        Dim H1 As Integer = CInt(PoseY)
                        Dim H2 As Integer = H1 + CInt(2.0 * PasH)
                        Dim CH As Integer = H1 + CInt(PasH)

                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = image.GetPixels(w, h)
                                Dim Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(coef, Pixel.ToColor)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)
                            Next
                        Next

                    End Using
                    GC.Collect()
                Next
            Next


            Dim ImgOut As New Bitmap(Height, Width)
            For w As Integer = CInt(PasW) To Width - CInt(PasW) - 1
                For h As Integer = CInt(PasW) To Height - CInt(PasW) - 1
                    Dim PixeRH = Class_ArrayByte.GetValue(h, w)
                    ImgOut.SetPixel(h - CInt(PasW), w - CInt(PasW), System.Drawing.Color.FromArgb(PixeRH.R, PixeRH.G, PixeRH.B))
                Next
            Next
            Dim pathFile = System.IO.Path.GetTempFileName + ".jpg"
            ImgOut.Save(pathFile)
            ImgOut.Dispose()
            MsgBox(pathFile)



        End If

        Class_ArrayByte.ClearMemory()
        GC.Collect()


    End Sub
End Module
