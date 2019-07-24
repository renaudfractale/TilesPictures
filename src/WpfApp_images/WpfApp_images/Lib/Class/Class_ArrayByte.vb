Imports System.IO

Public Class Class_ArrayByte
    Shared Property _IsInit As Boolean = False
    Shared Property _lenght As Integer = 1
    Shared Property _Height As Integer
    Shared Property _Width As Integer
    Shared Property _NbPointByDimension As Integer = 1
    Shared Property _NbDimension As Integer = 1

    Shared Property _memStream As MemoryStream

    Public Shared Sub Init(Height As Integer, Width As Integer)
        _IsInit = True
        _lenght = (Height * Width)
        _lenght *= (4 + 3)
        _Height = Height
        _Width = Width
        'ClearMemory()

        _memStream = New MemoryStream(_lenght + 1)
        _memStream.Position = _memStream.Capacity - 1
        _memStream.WriteByte(CByte(1))
    End Sub

    Public Shared Sub SetValue(PoseHeight As Integer, PoseWidth As Integer, value As Class_PixelRH)
        If _IsInit = False Then Exit Sub
        Dim Pose As Integer = (_Height * PoseHeight + PoseWidth) * 7
        Dim Coef = BitConverter.GetBytes(value.Coef)
        '    SyncLock _memStream
        _memStream.Position = Pose
        _memStream.Write(Coef, 0, 4)
        _memStream.Position = Pose + 4
        _memStream.WriteByte(value.R)
        _memStream.Position = Pose + 5
        _memStream.WriteByte(value.G)
        _memStream.Position = Pose + 6
        _memStream.WriteByte(value.B)

        '  End SyncLock

    End Sub


    Public Shared Function GetValue(PoseHeight As Integer, PoseWidth As Integer) As Class_PixelRH
        If _IsInit = False Then Return New Class_PixelRH
        Dim Pose As Integer = (_Height * PoseHeight + PoseWidth) * 7
        Dim value As New Class_PixelRH

        Dim Coef As Byte() = {0, 0, 0, 0}
        _memStream.Position = Pose
        _memStream.Read(Coef, 0, 4)
        value.Coef = BitConverter.ToSingle(Coef, 0)
        If CInt(Coef.GetValue(0)) = 0 And CInt(Coef.GetValue(1)) = 0 And CInt(Coef.GetValue(2)) = 0 And CInt(Coef.GetValue(3)) = 0 Then
            value.Coef = CSng(0.0)
        End If

        _memStream.Position = Pose + 4
        value.R = CByte(_memStream.ReadByte())

        _memStream.Position = Pose + 5
        value.G = CByte(_memStream.ReadByte())

        _memStream.Position = Pose + 6
        value.B = CByte(_memStream.ReadByte())


        Return value
    End Function


    Public Shared Sub ClearMemory()
        If _IsInit = False Then Exit Sub
        _memStream.Close()
        GC.Collect()
        _memStream = Nothing
        GC.Collect()
        _IsInit = False
    End Sub




End Class


