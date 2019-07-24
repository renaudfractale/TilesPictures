Class Application

    ' Les événements de niveau application, par exemple Startup, Exit et DispatcherUnhandledException
    ' peuvent être gérés dans ce fichier.
    Property ArrayByte As Class_ArrayByte
    Public Sub New()
        ArrayByte = New Class_ArrayByte
    End Sub
End Class
