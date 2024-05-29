Imports WebSocketSharp

Module BackendSocket
    Public ws As WebSocket
    Public connected As Boolean = False
    Public clientRoom As String
    Public Sub Connect(room As String)
        ws = New WebSocket("ws://localhost:" & Globals.PORT)
        AddHandler ws.OnOpen, Sub()
                                  RaiseEvent OnOpen()
                                  ws.Send(
                                      Globals.DictionaryToJSON(New Dictionary(Of String, String) From {{"room", room}, {"type", "join"}})
                                  )
                              End Sub
        AddHandler ws.OnMessage, Sub(sender, e) RaiseEvent OnMessage(e.Data)
        AddHandler ws.OnClose, Sub(sender, e) RaiseEvent OnClose(sender, e)
        AddHandler ws.OnError, Sub(sender, e)
                                   MsgBox(e)
                                   ws.Connect()
                               End Sub

        ws.Connect()
        clientRoom = room

        'Timer every 5 seconds to check if the connection is still alive
        Dim timer As New System.Timers.Timer(5000)
        AddHandler timer.Elapsed, Sub()
                                      If Not ws.IsAlive Then
                                          ws.Connect()
                                      End If
                                  End Sub
        timer.Start()
    End Sub

    Public Event OnOpen()
    Public Event OnMessage(data As String)
    Public Event OnClose(sender As Object, e As CloseEventArgs)

    Public Sub Send(data As String)
        ws.Send(data)
    End Sub

    Public Sub Close()
        Try
            ws.Send(
                Globals.DictionaryToJSON(New Dictionary(Of String, String) From {{"room", clientRoom}, {"type", "leave"}})
            )
        Catch ex As Exception

        End Try
        ws.Close()
    End Sub
End Module
