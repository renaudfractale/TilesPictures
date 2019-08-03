Imports System.Drawing
Imports ImageMagick
Imports System.IO

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
        Dim PasH = CDbl(Height) / CDbl((nbSection + ((nbSection - 1) * 0.5)))
        Dim PasW = CDbl(Width) / CDbl((nbSection + ((nbSection - 1) * 0.5)))

        Dim LenH = CSng(PasH)
        Dim LenW = CSng(PasW)


        Class_ArrayByte.Init(Height + CInt(PasW) + 1, Width + CInt(PasW) + 1)
        If Window.RadioButton_Alignement_V.IsChecked Then
            Dim H1 As Integer = 0
            Dim H2 As Integer = 0
            Dim C As Integer = 0

            Dim NoImage As Integer = 0
            For Each FileImg In ListeFiles
                H1 = CInt(Math.Max(PasH * 1.5 * CDbl(NoImage) - 1.0, 0.0))
                H2 = CInt(PasH * 1.5 * CDbl(NoImage) + PasH * 2.0 + 1)
                C = CInt(PasH * 1.5 * CDbl(NoImage) + PasH)

                Dim image = New MagickImage(FileImg)
                image.Border(CInt(PasW / 2.0), CInt(PasH / 2.0))
                Dim Img = image.ToBitmap
                For w As Integer = 0 To image.Width - 1
                    For h As Integer = H1 To H2
                        If h >= image.Height Then Exit For
                        Dim Pixel = Img.GetPixel(w, h)
                        Dim Dif = CSng(Math.Abs(C - h))
                        Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                        Dim Coef = Module_Signals.Signal(Dif, LenH, Window)
                        oldPixeRH.Add(Coef, Pixel)
                        Class_ArrayByte.SetValue(h, w, oldPixeRH)
                    Next
                Next
                NoImage += 1
                Img.Dispose()
                image.Dispose()
                GC.Collect()
            Next


        ElseIf Window.RadioButton_Alignement_H.IsChecked() Then
            Dim W1 As Integer = 0
            Dim W2 As Integer = 0
            Dim C As Integer = 0

            Dim NoImage As Integer = 0
            For Each FileImg In ListeFiles
                W1 = CInt(Math.Max(PasW * 1.5 * CDbl(NoImage) - 1.0, 0.0))
                W2 = CInt(PasW * 2.0 + PasW * 1.5 * CDbl(NoImage) + 1)
                C = CInt(PasW + PasW * 1.5 * CDbl(NoImage))
                'If NoImage > 0 Then
                '    W1 += CInt(PasW / 2.0)
                '    W2 += CInt(PasW / 2.0)
                '    C += CInt(PasW / 2.0)
                'End If
                Using image = New MagickImage(FileImg)
                    image.Border(CInt(PasW / 2.0), CInt(PasH / 2.0))
                    Dim Img = image.ToBitmap
                    For h As Integer = 0 To image.Height - 1
                        For w As Integer = W1 To W2
                            If w >= image.Width Then Exit For
                            Dim Pixel = Img.GetPixel(w, h)
                            Dim Dif = CSng(Math.Abs(C - w))
                            Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                            Dim Coef = Module_Signals.Signal(Dif, LenW, Window)
                            oldPixeRH.Add(Coef, Pixel)
                            Class_ArrayByte.SetValue(h, w, oldPixeRH)
                        Next
                    Next
                    Img.Dispose()
                    image.Dispose()
                    NoImage += 1
                End Using
                GC.Collect()
            Next


        ElseIf Window.RadioButton_Alignement_C.IsChecked() Then
            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75
            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5
                    Dim PoseXC = CDbl(X) * PasW * 1.5 + PasW * 1
                    Dim PoseYC = CDbl(Y) * PasH * 1.5 + PasH * 1


                    Dim DeltaX = Math.Abs(CDbl(Width + CInt(PasW)) / 2.0 - PoseXC)
                    Dim DeltaY = Math.Abs(CDbl(Height + CInt(PasH)) / 2.0 - PoseYC)
                    Dim NoPicture = CInt(Math.Round(Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY) / Diagonale, 0))


                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)

                    Using image = New MagickImage(ListeFiles.Item(NoPicture Mod (ListeFiles.Count)))
                        image.Border(CInt(PasW * 0.5), CInt(PasH * 0.5))
                        Dim Img = image.ToBitmap
                        Dim W1 As Integer = CInt(Math.Max(PoseX - 1.0, 0))
                        Dim W2 As Integer = CInt(PoseX + 2.0 * PasW + 1.0)
                        Dim CW As Integer = CInt(PoseX + PasW)

                        Dim H1 As Integer = CInt(Math.Max(PoseY - 1.0, 0))
                        Dim H2 As Integer = CInt(PoseY + 2.0 * PasH + 1.0)
                        Dim CH As Integer = CInt(PoseY + PasH)

                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = Img.GetPixel(w, h)
                                Dim Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(Coef, Pixel)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)
                            Next
                        Next
                        Img.Dispose()
                        image.Dispose()
                    End Using
                    GC.Collect()
                Next
            Next



        ElseIf Window.RadioButton_Alignement_D1.IsChecked() Then

            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75 * 2
            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5
                    Dim PoseXC = CDbl(X) * PasW * 1.5 + PasW
                    Dim PoseYC = CDbl(Y) * PasH * 1.5 + PasH


                    Dim DeltaX = Math.Abs(PasW - PoseXC)
                    Dim DeltaY = Math.Abs(PasH - PoseYC)
                    Dim NoPicture = CInt(Math.Round(Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY) / Diagonale, 0))


                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)

                    Using image = New MagickImage(ListeFiles.Item(NoPicture Mod (ListeFiles.Count)))
                        image.Border(CInt(PasW / 2.0), CInt(PasH / 2.0))
                        Dim Img = image.ToBitmap
                        Dim W1 As Integer = CInt(Math.Max(PoseX - 1.0, 0))
                        Dim W2 As Integer = CInt(PoseX + 2.0 * PasW + 1.0)
                        Dim CW As Integer = CInt(PoseX + PasW)

                        Dim H1 As Integer = CInt(Math.Max(PoseY - 1.0, 0))
                        Dim H2 As Integer = CInt(PoseY + 2.0 * PasH + 1.0)
                        Dim CH As Integer = CInt(PoseY + PasH)

                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = Img.GetPixel(w, h)
                                Dim Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(Coef, Pixel)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)
                            Next
                        Next
                        Img.Dispose()
                        image.Dispose()
                    End Using
                    GC.Collect()
                Next
            Next



        ElseIf Window.RadioButton_Alignement_D2.IsChecked() Then

            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75 * 2
            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5
                    Dim PoseXC = CDbl(X) * PasW * 1.5 + PasW
                    Dim PoseYC = CDbl(Y) * PasH * 1.5 + PasH


                    Dim DeltaX = Math.Abs((ListeFiles.Count - 1) * PasW * 1.5 + PasW - PoseXC)
                    Dim DeltaY = Math.Abs(PasH - PoseYC)
                    Dim NoPicture = CInt(Math.Round(Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY) / Diagonale, 0))


                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)

                    Using image = New MagickImage(ListeFiles.Item(NoPicture Mod (ListeFiles.Count)))
                        image.Border(CInt(PasW * 0.5), CInt(PasH * 0.5))
                        Dim Img = image.ToBitmap
                        Dim W1 As Integer = CInt(Math.Max(PoseX - 1.0, 0))
                        Dim W2 As Integer = CInt(PoseX + 2.0 * PasW + 1.0)
                        Dim CW As Integer = CInt(PoseX + PasW)

                        Dim H1 As Integer = CInt(Math.Max(PoseY - 1.0, 0))
                        Dim H2 As Integer = CInt(PoseY + 2.0 * PasH + 1.0)
                        Dim CH As Integer = CInt(PoseY + PasH)

                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = Img.GetPixel(w, h)
                                Dim Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                Dim oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(Coef, Pixel)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)
                            Next
                        Next
                        Img.Dispose()
                        image.Dispose()
                    End Using
                    GC.Collect()
                Next
            Next

        ElseIf Window.RadioButton_Alignement_Rdm.IsChecked() Then
            Dim random As New Random()
            Dim Liste As New List(Of String)
            Dim Diagonale = Math.Sqrt(PasW * PasW + PasH * PasH) * 0.75 * 2

            If Liste.Count = 0 Then
                For i = 0 To ListeFiles.Count - 1
                    For Each FilePic In ListeFiles
                        Liste.Add(FilePic)
                    Next
                Next
            End If

            For X As Integer = 0 To ListeFiles.Count - 1
                For Y As Integer = 0 To ListeFiles.Count - 1
                    Dim PoseX = CDbl(X) * PasW * 1.5
                    Dim PoseY = CDbl(Y) * PasH * 1.5



                    Dim NoPicture = random.Next(0, Liste.Count - 1)

                    '    MsgBox("X=" + X.ToString + ",  Y=" + Y.ToString + ", NoPicture=" + NoPicture.ToString)
                    Dim W1 As Integer = CInt(Math.Max(PoseX - 1.0, 0))
                    Dim W2 As Integer = CInt(PoseX + 2.0 * PasW + 1.0)
                    Dim CW As Integer = CInt(PoseX + PasW)

                    Dim H1 As Integer = CInt(Math.Max(PoseY - 1.0, 0))
                    Dim H2 As Integer = CInt(PoseY + 2.0 * PasH + 1.0)
                    Dim CH As Integer = CInt(PoseY + PasH)


                    Dim Coef As Single
                    Dim oldPixeRH As Class_PixelRH

                    Using image = New MagickImage(Liste.Item(NoPicture))
                        image.Border(CInt(PasW * 0.5), CInt(PasH * 0.5))
                        Dim Img = image.ToBitmap
                        For h As Integer = H1 To H2
                            For w As Integer = W1 To W2
                                If w >= image.Width Then Continue For
                                If h >= image.Height Then Continue For
                                Dim Pixel = Img.GetPixel(w, h)
                                Coef = Module_TuileSignals.Tuiles(CH, CW, h, w, CInt(PasH), CInt(PasW), Window)
                                oldPixeRH = Class_ArrayByte.GetValue(h, w)
                                oldPixeRH.Add(Coef, Pixel)
                                Class_ArrayByte.SetValue(h, w, oldPixeRH)

                            Next
                        Next
                        Img.Dispose()
                        image.Dispose()
                    End Using
                    GC.Collect()
                    GC.WaitForPendingFinalizers()
                    GC.Collect()
                    Liste.RemoveAt(NoPicture)

                Next
            Next

        End If

        Dim ImgOut As New Bitmap(Width, Height)
        For w As Integer = CInt(PasW * 0.5) To (Width + CInt(PasW * 0.5) - 1)
            For h As Integer = CInt(PasH * 0.5) To (Height + CInt(PasH * 0.5) - 1)
                Dim PixeRH = Class_ArrayByte.GetValue(h, w)
                ImgOut.SetPixel(w - CInt(PasW * 0.5), h - CInt(PasH * 0.5), System.Drawing.Color.FromArgb(PixeRH.R, PixeRH.G, PixeRH.B))
            Next
        Next
        Dim pathFile = System.IO.Path.GetTempFileName + ".jpg"
        ImgOut.Save(pathFile)
        ImgOut.Dispose()


        Dim No As Integer = 0
        Dim pathOutFile = Window.DirOut + "/OutFile_" + No.ToString + "_" + NameFileOut(Window) + ".jpg"

        Do
            pathOutFile = Window.DirOut + "/OutFile_" + No.ToString + "_" + NameFileOut(Window) + ".jpg"

            If Not File.Exists(pathOutFile) Then Exit Do

            No += 1
        Loop
        Using image = New MagickImage(pathFile)
            image.Format = MagickFormat.Jpg
            image.Quality = 70
            image.Strip()
            image.Write(pathOutFile)
        End Using
        Class_ArrayByte.ClearMemory()
        File.Delete(pathFile)
        GC.Collect()


    End Sub




    Private Function NameFileOut(Window As MainWindow) As String
        Dim Out As String = ""
        If Window.RadioButton_Parameters_AZ.IsChecked() Then
            Out += "AZ_"
        ElseIf Window.RadioButton_Parameters_ZA.IsChecked Then
            Out += "ZA_"
        ElseIf Window.RadioButton_Parameters_Random.IsChecked Then
            Out += "rndm_"
        End If

        If Window.RadioButton_Repetitions_None.IsChecked Then
            Out += "None_"
        ElseIf Window.RadioButton_Repetitions_Serie.IsChecked Then
            Out += "Serie_"
        ElseIf Window.RadioButton_Repetitions_Miroir.IsChecked Then
            Out += "Miroir_"
        End If

        If Window.RadioButton_Alignement_C.IsChecked Then
            Out += "C_"
        ElseIf Window.RadioButton_Alignement_D1.IsChecked Then
            Out += "D1_"
        ElseIf Window.RadioButton_Alignement_D2.IsChecked Then
            Out += "D2_"
        ElseIf Window.RadioButton_Alignement_Rdm.IsChecked Then
            Out += "Rndm_"
        ElseIf Window.RadioButton_Alignement_H.IsChecked Then
            Out += "H_"
        ElseIf Window.RadioButton_Alignement_V.IsChecked Then
            Out += "V_"
        End If

        If Window.RadioButton_Transition_Carré.IsChecked Then
            Out += "Square_"
        ElseIf Window.RadioButton_Transition_Sinus.IsChecked Then
            Out += "Sinus_"
        ElseIf Window.RadioButton_Transition_Triangle.IsChecked Then
            Out += "Tri_"
        End If

        If Window.StackPanel_Tuile.IsVisible Then
            If Window.RadioButton_Tuile_Carré.IsChecked Then
                Out += "Square_"
            ElseIf Window.RadioButton_Tuile_Circulaire.IsChecked Then
                Out += "Cir"
            ElseIf Window.RadioButton_Tuile_Combinée.IsChecked Then
                Out += "Comb_"
            End If
        Else
            Out += "None_"
        End If
        Return Out
    End Function
End Module
