Imports Microsoft
Public Class Class_PixelRH
    Property Coef As Single = 0.0
    Property R As Byte = 0
    Property G As Byte = 0
    Property B As Byte = 0

    Public Sub New()

    End Sub

    Public Sub New(Coef As Single, Pixel As System.Drawing.Color)
        Me.Coef = Coef
        Me.R = Pixel.R
        Me.G = Pixel.G
        Me.B = Pixel.B
    End Sub

    Public Sub Add(Coef As Single, Pixel As System.Drawing.Color)

        Me.Coef = Math.Abs(Me.Coef)
        Coef = Math.Abs(Coef)
        If Coef <> 0.0 Or Me.Coef <> 0.0 Then
            Me.R = CByte((Coef * CSng(Pixel.R) + Me.Coef * CSng(Me.R)) / CSng(Me.Coef + Coef))
            Me.G = CByte((Coef * CSng(Pixel.G) + Me.Coef * CSng(Me.G)) / CSng(Me.Coef + Coef))
            Me.B = CByte((Coef * CSng(Pixel.B) + Me.Coef * CSng(Me.B)) / CSng(Me.Coef + Coef))
            Me.Coef += Coef
        Else
            Me.R = 0
            Me.G = 0
            Me.B = 0
        End If
    End Sub


End Class
