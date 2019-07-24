Imports MahApps.Metro.Controls
Imports System.IO
Class MainWindow
    Inherits MetroWindow
    Property PathDir As String = ""
    Property DicoSize As New Dictionary(Of String, List(Of String))
    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        RadioButton_Alignement_C.IsChecked = True
        RadioButton_Parameters_Random.IsChecked = True
        RadioButton_Repetitions_None.IsChecked = True
        RadioButton_Transition_Triangle.IsChecked = True
        RadioButton_Tuile_Circulaire.IsChecked = True
        StackPanel_Options.IsEnabled = False

    End Sub

    Private Sub Button_GetDirPictures_Click(sender As Object, e As RoutedEventArgs)
        Dim dialog = New System.Windows.Forms.FolderBrowserDialog()
        If dialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

            Me.PathDir = dialog.SelectedPath

        Else
            Me.PathDir = ""
        End If
        Me.ListeSize()

    End Sub

    Private Sub ListeSize()
        DicoSize = New Dictionary(Of String, List(Of String))
        ComboBox_ListSizes.Items.Clear()
        If Me.PathDir <> "" Then

            Dim ArrayPath = Directory.GetFiles(PathDir)
            For Each PathFile As String In ArrayPath
                Dim newImage As System.Drawing.Image = Nothing
                Try
                    newImage = System.Drawing.Image.FromFile(PathFile)

                Catch ex As Exception

                End Try
                If newImage IsNot Nothing Then
                    Dim Height = newImage.Height
                    Dim Width = newImage.Width
                    Dim Key = Width.ToString + "x" + Height.ToString

                    If Not DicoSize.ContainsKey(Key) Then
                        DicoSize.Add(Key, New List(Of String))
                    End If
                    DicoSize.Item(Key).Add(PathFile)
                    newImage.Dispose()
                    GC.Collect()
                End If
            Next

            For Each KV In DicoSize
                ComboBox_ListSizes.Items.Add(KV.Key + " - " + KV.Value.Count.ToString + " File(s)")
            Next


        End If

        If DicoSize.Count = 0 Then
            StackPanel_Options.IsEnabled = False
        Else
            StackPanel_Options.IsEnabled = True
            ComboBox_ListSizes.SelectedIndex = 0
        End If
    End Sub

    Private Sub Button_GenerateOnePicture_Click(sender As Object, e As RoutedEventArgs)
        Module_Runtime.Runtime(Me)
    End Sub

    Private Sub RadioButton_Alignement_D1_Checked(sender As Object, e As RoutedEventArgs)
        StackPanel_Tuile.Visibility = Visibility.Visible
    End Sub

    Private Sub RadioButton_Alignement_D2_Checked(sender As Object, e As RoutedEventArgs)
        StackPanel_Tuile.Visibility = Visibility.Visible
    End Sub

    Private Sub RadioButton_Alignement_C_Checked(sender As Object, e As RoutedEventArgs)
        StackPanel_Tuile.Visibility = Visibility.Visible
    End Sub

    Private Sub RadioButton_Alignement_Rdm_Checked(sender As Object, e As RoutedEventArgs)
        StackPanel_Tuile.Visibility = Visibility.Visible
    End Sub

    Private Sub RadioButton_Alignement_V_Checked(sender As Object, e As RoutedEventArgs)
        StackPanel_Tuile.Visibility = Visibility.Collapsed
    End Sub

    Private Sub RadioButton_Alignement_H_Checked(sender As Object, e As RoutedEventArgs)
        StackPanel_Tuile.Visibility = Visibility.Collapsed
    End Sub
End Class


