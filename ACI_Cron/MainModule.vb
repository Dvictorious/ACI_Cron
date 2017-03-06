Module MainModule

    Public Sub logError(ByVal msg As String)
        Using sw As New IO.StreamWriter(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Cron_LogFile.txt", True)
            sw.WriteLine(Now & " | " & Environment.UserDomainName & "\" & Environment.UserName & " | " & msg)
            sw.Close()
        End Using
    End Sub
End Module
