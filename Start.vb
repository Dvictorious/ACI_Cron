Imports System.IO
Imports System.Net
Imports Microsoft.Office.Interop
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Security
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.Data.Sql

Public Class Start
    Dim acf As ACI_Common_Functions.ACICommonFunctions
    Dim Startnow As Integer = 0
    Dim SpectXferLogFile As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_SpectXferLogFile.txt"
    Dim BNContactLog As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_BN_Contact_Log.txt"
    Private Sub Start_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim logFile As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Cron_LogFile.txt"
        If File.Exists(logFile) = False Then
            File.Create(logFile)
        End If
        Dim Emailog As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Con_Emaillogs.txt"
        If File.Exists(Emailog) = False Then
            File.Create(Emailog)
        End If
        Dim jpegnameLog As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_JpegNameLog.txt"
        If File.Exists(jpegnameLog) = False Then
            File.Create(jpegnameLog)
        End If
        If File.Exists(BNContactLog) = False Then
            File.Create(BNContactLog)
        End If
        If File.Exists(SpectXferLogFile) = False Then
            File.Create(SpectXferLogFile)
        End If
        Dim procs() As Process = Process.GetProcessesByName("OUTLOOK")
        If procs.Count = 0 Then
            MessageBox.Show("Please Open Outlook")
        End If
        acf = New ACI_Common_Functions.ACICommonFunctions
        acf.logPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Cron_LogFile.txt"
        acf.RConn = My.Settings.ResearchConString
        acf.FSConn = My.Settings.LocalConString
        'VersionLabel.Text = My.Application.Deployment.CurrentVersion.ToString
    End Sub

    Private Sub JpegTransferTimer_Tick(sender As Object, e As EventArgs) Handles JpegTransferTimer.Tick
        If JpegTransferBackgroundWorker.IsBusy = False Then
            JpegTransferProgressBar.Value = 0
            If Directory.Exists("\\ac2\data\Patient Archive") Then
                JpegTransferBackgroundWorker.RunWorkerAsync(My.Settings.JpegTransferSrcDestList)
            Else
                My.Settings.JpegTransferStatusList.Add(Now & ": \\ac2\data\Patient Archive is unavailable")
                If JpegTransferDetails.Visible = True Then
                    JpegTransferDetails.JpegTransferListBox.Items.Add(Now & ": \\ac2\data\Patient Archive is unavailable")
                    JpegTransferDetails.JpegTransferListBox.TopIndex = JpegTransferDetails.JpegTransferListBox.Items.Count - 1
                End If
            End If
            My.Settings.RecentJpegTransfer = Now
            My.Settings.Save()
        ElseIf JpegTransferBackgroundWorker.IsBusy = True Then
            My.Settings.JpegTransferStatusList.Add(Now & ": Jpeg Transfer is currently busy.")
            If JpegTransferDetails.Visible = True Then
                JpegTransferDetails.JpegTransferListBox.Items.Add(Now & ": Jpeg Transfer is currently busy.")
                JpegTransferDetails.JpegTransferListBox.TopIndex = JpegTransferDetails.JpegTransferListBox.Items.Count - 1
            End If
            My.Settings.RecentJpegTransfer = Now
            My.Settings.Save()
        End If
    End Sub

    Private Function getFTPFolders(ByVal destAndftpSites As List(Of KeyValuePair(Of String, String))) As List(Of KeyValuePair(Of String, KeyValuePair(Of String, String)))
        Dim folderList As New List(Of KeyValuePair(Of String, String))
        Dim mainList As New List(Of KeyValuePair(Of String, KeyValuePair(Of String, String)))
        Dim curMonth As String = Today.ToString("m").Substring(0, 3)
        For Each pair As KeyValuePair(Of String, String) In destAndftpSites
            Try
                If My.Computer.Network.Ping(pair.Value.Substring(0, pair.Value.IndexOf("/"))) = False Then
                    'try one more time
                    If My.Computer.Network.Ping(pair.Value.Substring(0, pair.Value.IndexOf("/"))) = False Then
                        logError(pair.Value & " is unreachable")
                        Continue For
                    End If
                End If
                Dim ftpRequest As FtpWebRequest = WebRequest.Create("ftp://" & pair.Value)
                ftpRequest.Credentials = New NetworkCredential(My.Settings.OdysseyUN, My.Settings.OdysseyPW)
                ftpRequest.KeepAlive = False
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails
                Using ftpResponse As FtpWebResponse = ftpRequest.GetResponse()
                    Dim sr As New StreamReader(ftpResponse.GetResponseStream())
                    Do Until sr.EndOfStream
                        Dim directoryWithDetails As String = sr.ReadLine
                        If (directoryWithDetails.Contains("total") = False) Then
                            Dim monthInStringIndex As Integer = directoryWithDetails.IndexOf(CStr(" " & curMonth & " "))
                            If monthInStringIndex = -1 Then
                                Continue Do
                            End If
                            Dim dayInstringIndex As Integer
                            If directoryWithDetails.Contains(":") Then
                                dayInstringIndex = directoryWithDetails.IndexOf(":") - 2
                            ElseIf directoryWithDetails.Contains(Today.Year) Then
                                dayInstringIndex = directoryWithDetails.IndexOf(Today.Year) - 4
                            End If
                            Dim modDate As Date = CDate(directoryWithDetails.Substring(monthInStringIndex, (dayInstringIndex - monthInStringIndex)) & Today.Year)
                            If DateDiff(DateInterval.Hour, modDate, Today.AddDays(-7)) <= 24 Then
                                Dim directoryOfInterest As String = directoryWithDetails.Substring(directoryWithDetails.LastIndexOf(" ") + 1)
                                Dim kvPair As New KeyValuePair(Of String, String)(pair.Value, directoryOfInterest)
                                mainList.Add(New KeyValuePair(Of String, KeyValuePair(Of String, String))(pair.Key, kvPair))
                            End If
                        End If
                    Loop
                    sr.Close()
                    ftpResponse.Close()
                End Using
            Catch ex As Exception
                logError("Error occurred in getFTPFolders(); var pair=" & pair.ToString & ". The full error is: " & ex.ToString)
                ''
            End Try
        Next
        Return mainList
    End Function

    Private Sub JpegTransferBackgroundWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles JpegTransferBackgroundWorker.DoWork
        'Dim ftpReqParent As FtpWebRequest, ftpReqSubDirectory As FtpWebRequest, ftpReqJpeg As FtpWebRequest
        'Dim nc As New NetworkCredential(My.Settings.OdysseyUN, My.Settings.OdysseyPW)

        'Dim tempList As New List(Of String)     'we'll put directories and jpegs in and delete them based on last modified. In the end everything that should be transferred will be copied to jpegList
        'Dim jpegList As New List(Of String)
        'Dim remoteParent As String            'e.g. 192.168.0.20/img4/prism/images/2013
        'Dim destinationParent As String     'e.g. \\ac2\data\Patient Archive\NEWPORT\2013
        'Dim patientDirectory As String, details_line As String
        'Dim lastModString As String, lastMod As Date

        'For Each clinic As String In {"192.168.0.20/img4/prism/images/2013;\\ac2\data\patient archive\newport\2013"} 'In My.Settings.JpegTransferSrcDestList    'in the form remotepath;destinationpath
        '    remoteParent = clinic.Split(";")(0)             'split the string on ';' the first (0-order) index is the remote path, the 2nd (index = 1) is the destination
        '    destinationParent = clinic.Split(";")(1)

        '    JpegTransferBackgroundWorker.ReportProgress(0, "Retrieving list of files from " & remoteParent)

        '    'get all jpeg directories from remoteParent with last modified times within 7 days
        '    ftpReqParent = FtpWebRequest.Create("ftp://" & remoteParent)
        '    ftpReqParent.Credentials = nc
        '    ftpReqParent.Method = WebRequestMethods.Ftp.ListDirectory
        '    Try
        '        Using ftpRes As FtpWebResponse = ftpReqParent.GetResponse
        '            Using sr As New StreamReader(ftpRes.GetResponseStream)
        '                Do Until sr.EndOfStream
        '                    patientDirectory = sr.ReadLine
        '                    patientDirectory = patientDirectory.Substring(patientDirectory.IndexOf("/") + 1)    'files returned from ListDirectory are in the form [ParentFolder]/[File or Folder]
        '                    If Not patientDirectory.Contains(".jpg") Then   'i.e. the tech didn't put the jpegs into a folder
        '                        ftpReqSubDirectory = WebRequest.Create("ftp://" & remoteParent & "/" & patientDirectory)
        '                        ftpReqSubDirectory.Credentials = nc
        '                        ftpReqSubDirectory.Method = WebRequestMethods.Ftp.ListDirectoryDetails
        '                        Try
        '                            Using ftpRes2 As FtpWebResponse = ftpReqSubDirectory.GetResponse
        '                                Using sr2 As New StreamReader(ftpRes2.GetResponseStream)
        '                                    Do Until sr2.EndOfStream
        '                                        details_line = details_line.Substring(details_line.IndexOf("/") + 1)        'files returned from ListDirectory are in the form [ParentFolder]/[File or Folder]
        '                                        If details_line.Contains("/") Or details_line.Contains("\") Then  'if a / or \ is included in the path, it will be interpreted as a secondary subfolder with the jpeg inside of that; therefore, remove the \ or /
        '                                            details_line = details_line.Replace("/", "").Replace("\", "")
        '                                        End If
        '                                        tempList.Add(remoteParent & "/" & patientDirectory & "/" & details_line)
        '                                    Loop
        '                                    sr2.Close()
        '                                End Using
        '                                ftpRes2.Close()
        '                            End Using
        '                        Catch ex As Exception
        '                            Dim x As Exception = ex
        '                        End Try
        '                    End If

        '                Loop
        '                sr.Close()
        '            End Using
        '            ftpRes.Close()
        '        End Using
        '    Catch ex As Exception
        '        JpegTransferBackgroundWorker.ReportProgress(0, ex.Message)
        '    End Try
        'Next 
            Dim tyear As String = DatePart(DateInterval.Year, Date.Today).ToString
            Try
                Dim destAndftpSites As New List(Of KeyValuePair(Of String, String))
                'key = remote parent path | value = destination parent path
                For Each srcDtnPair As String In My.Settings.JpegTransferSrcDestList
                    destAndftpSites.Add(New KeyValuePair(Of String, String)(srcDtnPair.Substring(srcDtnPair.IndexOf(";") + 1), _
                            srcDtnPair.Substring(0, srcDtnPair.IndexOf(";"))))
                Next
                Dim mainList As New List(Of KeyValuePair(Of String, KeyValuePair(Of String, String)))
                mainList = getFTPFolders(destAndftpSites)
                Dim dirList As New List(Of String)
                Dim i As Integer = 0

                For Each triplet As KeyValuePair(Of String, KeyValuePair(Of String, String)) In mainList
                    Try
                        Dim ftpRequest As FtpWebRequest = FtpWebRequest.Create("ftp://" & triplet.Value.Key & "/" & triplet.Value.Value)
                        ftpRequest.Credentials = New NetworkCredential(My.Settings.OdysseyUN, My.Settings.OdysseyPW)
                        ftpRequest.KeepAlive = False
                        ftpRequest.Timeout = -1
                        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory
                        Using ftpResponse As FtpWebResponse = ftpRequest.GetResponse
                            Using sr As StreamReader = New StreamReader(ftpResponse.GetResponseStream)
                                Do Until sr.EndOfStream
                                    dirList.Add(sr.ReadLine)
                                Loop
                            End Using
                        End Using
                        For Each jpeg As String In dirList
                            Try
                                Dim targetFolder As String = getTargetFolder(triplet.Value.Value, jpeg.Substring(jpeg.LastIndexOf("/") _
                                    + 1), triplet.Key)
                                Dim MoveJpeg As String = MoveJpegs(triplet.Value.Value, jpeg.Substring(jpeg.LastIndexOf("/") _
                                    + 1), triplet.Key)
                                If File.Exists(targetFolder & "/" & jpeg.Substring(jpeg.LastIndexOf("/") + 1)) = False Then
                                    My.Computer.Network.DownloadFile("ftp://" & triplet.Value.Key & "/" & jpeg, targetFolder & "/" & jpeg.Substring(jpeg _
                                                            .LastIndexOf("/") + 1), My.Settings.OdysseyUN, My.Settings.OdysseyPW)
                                    JpegTransferBackgroundWorker.ReportProgress((i / (CType(mainList, List(Of KeyValuePair(Of String, KeyValuePair(Of _
                                        String, String)))).Count - 1)) * 100, CStr(CStr(Now) & ": " & triplet.Value.Key & "/" & jpeg & _
                                        "   ------------------>   " & targetFolder))
                                    My.Computer.Network.DownloadFile("ftp://" & triplet.Value.Key & "/" & jpeg, MoveJpeg & "/" & jpeg.Substring(jpeg _
                                                             .LastIndexOf("/") + 1), My.Settings.OdysseyUN, My.Settings.OdysseyPW)
                                    Dim ContactID As Integer = Nothing
                                    Dim Delimitercheck As String = Path.GetFileNameWithoutExtension(jpeg).Substring(Path.GetFileNameWithoutExtension(jpeg).Length - 10)
                                    Dim Results As String = Nothing
                                    Dim JpegString As String() = Nothing
                                    Dim SplitCount As Integer = Nothing
                                    Dim lastname As String = Nothing
                                    Dim firstname As String = Nothing
                                    If jpeg.Contains("tif") = False Then
                                        If Delimitercheck.Contains("-") Then
                                            JpegString = Split(Path.GetFileNameWithoutExtension(jpeg), "-")
                                            SplitCount = JpegString.Count
                                            ContactID = JpegString(SplitCount - 1)
                                        ElseIf Delimitercheck.Contains("_") Then
                                            JpegString = Split(Path.GetFileNameWithoutExtension(jpeg), "_")
                                            SplitCount = JpegString.Count
                                            ContactID = JpegString(SplitCount - 1)
                                        End If
                                        Dim CIDCheck As Integer = 0
                                        Using con As New SqlConnection(My.Settings.ResearchConString)
                                            Using cmd As SqlCommand = New SqlCommand
                                                ''06/29/2015 Derek Vincent Taylor
                                                ''Previous version got only 4 letters of last name
                                                ''Now will use full last and first name 
                                                'If JpegString(0).Count > 4 Then
                                                '    lastname = JpegString(0).Substring(0, 4)
                                                'Else
                                                '    lastname = JpegString(0)
                                                'End If
                                                ''06/29/2015 Derek V. Taylor 
                                                ''Using Parameters to pass to new function that uses but first and last name
                                                lastname = JpegString(0)
                                                firstname = JpegString(1)
                                                cmd.Connection = con
                                                cmd.CommandText = "execute contact_matches  @ContactID, @lastname, @firstname "
                                                cmd.Parameters.Add("@ContactID", SqlDbType.Int).Value = ContactID
                                                cmd.Parameters.Add("@lastname", SqlDbType.VarChar, 50).Value = lastname
                                                cmd.Parameters.Add("@firstname", SqlDbType.VarChar, 50).Value = firstname
                                                con.Open()
                                                CIDCheck = cmd.ExecuteScalar
                                                con.Close()
                                            End Using
                                        End Using
                                        If CIDCheck = 0 Then
                                            LogJpeg(targetFolder, ContactID, "CID Check = 0")
                                            'if CID isnt in database do nothing
                                            Dim cid As String = Nothing
                                            Dim fso As New Scripting.FileSystemObject
                                            Dim file As Scripting.TextStream
                                            Dim fileBnCID As Scripting.TextStream
                                            Using sw1 As StreamWriter = New StreamWriter(BNContactLog, True)
                                                Try
                                                    fileBnCID = fso.OpenTextFile(BNContactLog)
                                                    Do While Not fileBnCID.AtEndOfLine
                                                        cid = fileBnCID.ReadAll
                                                        If Not InStr(1, Trim(cid), ContactID, vbTextCompare) > 0 Then
                                                            sw1.WriteLine(JpegString)
                                                            Dim olApp As Outlook.Application = New Outlook.Application
                                                            Dim mail As Outlook.MailItem = olApp.CreateItem(Outlook.OlItemType.olMailItem)
                                                            mail.To = "dtaylor@amenclinic.com;jshafer@amenclinic.com"
                                                            mail.Subject = "BN Jpeg Flagger - " & triplet.Value.Value
                                                            mail.HTMLBody = "<a href=""file:///" & targetFolder & """>" & targetFolder & "</>  ----  " & ContactID
                                                            mail.Send()
                                                        End If
                                                    Loop
                                                Catch ex As Exception
                                                    Dim h As Integer = 0
                                                End Try
                                            End Using
                                        Else
                                            LogJpeg(targetFolder, ContactID, "CID check Found Patient")
                                            Dim JpegFile As String = targetFolder & "/" & jpeg.Substring(jpeg.LastIndexOf("/") + 1)
                                            Results = acf.BestNotesAPiTransfer(JpegFile, ContactID)
                                            JpegTransferBackgroundWorker.ReportProgress(0, CStr(CStr(Now) & " : " & Results))
                                        End If
                                    Else
                                        'do nothing
                                    End If
                                Else
                                    'do nothing if a Tif File
                                End If
                            Catch ex As Exception
                                logError("Error occurred in JpegTransfer bw worker_dowork; var jpeg=" & jpeg & "; var triplet=" & triplet.ToString & ". The full error is: " & ex.ToString)
                            End Try
                        Next
                        Try
                            If dirList.Count Mod 2 > 0 Then
                                Dim items As String
                                Dim fso As New Scripting.FileSystemObject
                                Dim file As Scripting.TextStream
                                Dim filejpegname As Scripting.TextStream
                                Dim line As String
                                Dim line2 As String
                                Dim Emaillog As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Con_Emaillogs.txt"
                                Dim jpegnameLog As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_JpegNameLog.txt"
                                Using sw As StreamWriter = New StreamWriter(Emaillog, True)
                                    For Each jpeg In dirList
                                        items &= jpeg
                                        Try
                                            If jpeg.Contains("45") Then
                                                Continue For
                                            ElseIf jpeg.Contains("cap00") Then
                                                Continue For
                                            Else
                                                file = fso.OpenTextFile(Emaillog)
                                                Do While Not file.AtEndOfLine
                                                    line = file.ReadAll
                                                    If Not InStr(1, Trim(line), jpeg, vbTextCompare) > 0 Then
                                                        sw.WriteLine(jpeg)
                                                        Using sw1 As StreamWriter = New StreamWriter(jpegnameLog, True)
                                                            Try
                                                                filejpegname = fso.OpenTextFile(jpegnameLog)
                                                                Do While Not filejpegname.AtEndOfLine
                                                                    line2 = filejpegname.ReadAll
                                                                    If Not InStr(1, Trim(line2), triplet.Value.Value, vbTextCompare) > 0 Then
                                                                        sw1.WriteLine(triplet.Value.Value)
                                                                        Dim targetfolder As String = getTargetFolder(triplet.Value.Value, items.Substring(items.LastIndexOf("/") + 1), triplet.Key)
                                                                        Dim olApp As Outlook.Application = New Outlook.Application
                                                                        Dim mail As Outlook.MailItem = olApp.CreateItem(Outlook.OlItemType.olMailItem)
                                                                        mail.To = "apickles@amenclinic.com;jshafer@amenclinic.com"
                                                                        mail.Subject = "Jpegs Flagger - " & triplet.Value.Value
                                                                        mail.HTMLBody = "<a href=""file:///" & targetfolder & """>" & targetfolder & "</>"
                                                                        mail.Send()
                                                                    End If
                                                                Loop
                                                            Catch ex As Exception
                                                                Dim h As Integer = 0
                                                            End Try
                                                        End Using
                                                    End If
                                                Loop
                                            End If
                                        Catch ex As Exception
                                            Dim x As String = "0"
                                        End Try
                                    Next
                                End Using
                            End If
                        Catch ex As Exception
                            Dim D As String = "0"
                        End Try
                        i += 1
                        dirList.Clear()
                    Catch ex As Exception
                        If ex.Message.Contains("(550) File unavailable") = False Then
                            logError("Error occurred in JpegTransfer bw worker_dowork; var triplet=" & triplet.ToString & ". The full error is: " & ex.ToString)
                        End If
                    Finally
                        If dirList.Count > 0 Then
                            dirList.Clear()
                        End If
                    End Try
                    System.Threading.Thread.Sleep(100)
                Next
            Catch ex As Exception
                logError("Error occurred in JpegTransfer bw worker_dowork. The full error is: " & ex.ToString)
            End Try
    End Sub

    Private Sub LogJpeg(ByVal jpegstring As String, ByVal ContactID As String, ByVal Message As String)
        If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Cron_BNJpegLogFile.txt") Then
            File.Create(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Cron_BNJpegLogFile.txt")
        End If
        Using sw As New StreamWriter(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\ACI_Cron_BNJpegLogFile.txt", True)
            sw.WriteLine(Now & " | " & Environment.UserName & " | " & jpegstring & " | " & ContactID & " | " & Message)
        End Using
    End Sub

    Private Function MoveJpegs(ByVal jpegDir As String, ByVal jpegFile As String, ByVal destDir As String) As String
        Dim fullTargetPath As String = Nothing
        Try
            Dim archYear As String = String.Empty
            If IsNumeric(jpegFile.Substring(jpegFile.Length - 2, 2)) Then
                archYear = "20" & jpegFile.Substring(jpegFile.Length - 2, 2)
            Else
                archYear = Today.Year.ToString
            End If
            Dim archDir As String = String.Empty
            archDir = destDir.Substring(destDir.LastIndexOf("/") + 1)
            Dim destComb As String = destDir & "\" & archYear & "\" & "Cumulative_3D_Images"
            fullTargetPath = destComb
        Catch ex As Exception

        End Try
        Return fullTargetPath
    End Function

    Private Function getTargetFolder(ByVal jpegDir As String, ByVal jpegFile As String, ByVal destDir As String) As String
        Dim fullTargetPath As String = Nothing
        Try
            Dim archYear As String = String.Empty
            If IsNumeric(jpegFile.Substring(jpegFile.Length - 2, 2)) Then
                archYear = "20" & jpegFile.Substring(jpegFile.Length - 2, 2)
            Else
                archYear = Today.Year.ToString
            End If
            Dim archIndex As String = String.Empty
            Select Case jpegDir.Substring(0, 1).ToUpper
                Case "A" To "B"
                    archIndex = "A-B"
                Case "C" To "D"
                    archIndex = "C-D"
                Case "E" To "G"
                    archIndex = "E-G"
                Case "H" To "J"
                    archIndex = "H-J"
                Case "K" To "L"
                    archIndex = "K-L"
                Case "M" To "N"
                    archIndex = "M-N"
                Case "O" To "R"
                    archIndex = "O-R"
                Case "S" To "T"
                    archIndex = "S-T"
                Case "U" To "Z"
                    archIndex = "U-Z"
            End Select
            Dim patFolder As String = Nothing
            Dim foundFolder As Boolean = False
            Dim narrowedpatFolders As String() = Directory.GetDirectories(destDir & "\" & archYear & "\" & archIndex, jpegDir.Substring(0, jpegDir.IndexOf("_")) & "*", _
                    IO.SearchOption.TopDirectoryOnly)
            If narrowedpatFolders.Count > 0 Then
                For Each folder As String In narrowedpatFolders
                    If folder.Substring(folder.LastIndexOf("\") + 1).Replace(",", "_").Replace(" ", "").ToLower Like "*" & jpegDir.ToLower & "*" Then
                        patFolder = folder.Substring(folder.LastIndexOf("\") + 1)
                        foundFolder = True
                        Exit For
                    End If
                Next
            End If
            If foundFolder = False Then
                Directory.CreateDirectory(destDir & "\" & archYear & "\" & archIndex & "\" & jpegDir)
                patFolder = jpegDir
            End If
            Dim destComb As String = destDir & "\" & archYear & "\" & archIndex & "\" & patFolder
            fullTargetPath = destComb
            Dim subFolders As String() = Directory.GetDirectories(destComb, "Eval*", SearchOption.TopDirectoryOnly)
            If subFolders.Count > 0 Then
                fullTargetPath = destComb & "\" & subFolders(0).Substring(subFolders(0).LastIndexOf("\") + 1)
            End If
        Catch ex As Exception
            logError("Error occurred attempting to get jpeg target directory; var jpegDir=" & jpegDir & "; var jpegFile=" & jpegFile & "; var destDir=" & destDir & _
                     ". The full error is " & ex.ToString)
        End Try
        Return fullTargetPath
    End Function

    Private Sub JpegTransferBackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles JpegTransferBackgroundWorker.ProgressChanged
        JpegTransferProgressBar.Value = e.ProgressPercentage
        If My.Settings.JpegTransferStatusList.Count > 1000000 Then
            My.Settings.JpegTransferStatusList.RemoveAt(0)
        End If
        My.Settings.JpegTransferStatusList.Add(CStr(e.UserState))
        If JpegTransferDetails.Visible = True Then
            If TimerJpegWatcher.Enabled = False Then
                TimerJpegWatcher.Enabled = True
            End If
            JpegTransferDetails.JpegTransferListBox.Items.Add(e.UserState)
            JpegTransferDetails.JpegTransferListBox.TopIndex = JpegTransferDetails.JpegTransferListBox.Items.Count - 1
        End If
        My.Settings.JpegWatcher = Now
        My.Settings.Save()
    End Sub

    Private Sub JpegTransferBackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles JpegTransferBackgroundWorker.RunWorkerCompleted
        If e.Error IsNot Nothing Then
            logError(e.Error.ToString)
            If My.Settings.JpegTransferStatusList.Count > 1000000 Then
                My.Settings.JpegTransferStatusList.RemoveAt(0)
            End If
            My.Settings.JpegTransferStatusList.Add(e.Error.Message)
            If JpegTransferDetails.Visible = True Then
                JpegTransferDetails.JpegTransferListBox.Items.Add(e.Error.Message)
                JpegTransferDetails.JpegTransferListBox.TopIndex = JpegTransferDetails.JpegTransferListBox.Items.Count - 1
            End If
        End If
        JpegTransferDetails.JpegTransferListBox.Items.Add(Now & ": Jpeg Transfer Completed.")
        JpegTransferDetails.JpegTransferListBox.TopIndex = JpegTransferDetails.JpegTransferListBox.Items.Count - 1
        My.Settings.JpegWatcher = Now
        My.Settings.Save()
        JpegTransferProgressBar.Value = 0
    End Sub

    Private Sub JpegTransferDetailsLinkLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles JpegTransferDetailsLinkLabel.LinkClicked
        JpegTransferDetails.Show()
        JpegTransferDetails.BringToFront()
    End Sub

    Private Sub SpectTransferTimer_Tick(sender As Object, e As EventArgs) Handles SpectTransferTimer.Tick
        If SpectTransferBackgroundWorker.IsBusy = False Then
            SpectTransferProgressBar.Value = 0
            My.Settings.RecentSpectTransfer = Now
            My.Settings.Save()
            If Now.Hour <= 21 Then
                SpectTransferBackgroundWorker.RunWorkerAsync(My.Settings.SpectTransferSourceListFTP)
            Else
                SpectTransferBackgroundWorker.RunWorkerAsync(My.Settings.SpectTransferSourceListReadyFile)
            End If
        End If
    End Sub

    Private Sub SpectTransferBackgroundWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles SpectTransferBackgroundWorker.DoWork
        Try
            Dim filesList As New List(Of String)
            filesList.Clear()
            For Each source As String In e.Argument
                If source.Contains("/") Then
                    If My.Computer.Network.Ping(source.Substring(0, source.IndexOf("/"))) = False Then
                        If My.Computer.Network.Ping(source.Substring(0, source.IndexOf("/"))) = False Then  'try once more
                            logError(source & " is unavailable")
                            SpectTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": " & source & " is unavailable"))
                        End If
                    Else
                        SpectTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": Retrieving list of files from " & source))
                        For Each spectFile As String In ftpMethod(source)
                            filesList.Add(spectFile)
                        Next
                    End If
                Else
                    If My.Computer.Network.Ping(source) = False Then
                        If My.Computer.Network.Ping(source) = False Then     'try once more
                            logError(source & " is unavailable")
                            SpectTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": " & source & " is unavailable"))
                        End If
                    Else
                        SpectTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": Generating ready file from " & source))
                        For Each spectFile As String In readyFileMethod(source)
                            filesList.Add(spectFile)
                        Next
                    End If
                End If
            Next
            If filesList.Count = 0 Then
                SpectTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": No new files to download"))
            End If
            Dim i As Integer = 0
            For Each spectFile As String In filesList
                Try
                    SpectTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": Downloading " & spectFile.Replace("%2f", "")))
                    If File.Exists(Path.GetTempPath & spectFile.Substring(spectFile.LastIndexOf("/") + 1)) Then
                        File.Delete(Path.GetTempPath & spectFile.Substring(spectFile.LastIndexOf("/") + 1))
                    End If
                    My.Computer.Network.DownloadFile(spectFile, Path.GetTempPath & spectFile.Substring(spectFile.LastIndexOf("/") + 1), _
                            My.Settings.OdysseyUN, My.Settings.OdysseyPW)
                    SpectTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": " & acf.parseOdysseyData(Path.GetTempPath & spectFile.Substring(spectFile.LastIndexOf("/") + 1))))
                    File.Delete(Path.GetTempPath & spectFile.Substring(spectFile.LastIndexOf("/") + 1))
                Catch ex As Exception
                    logError("Error occurred iterating through filesList in spect transfer bw_dowork; var spectFile=" & spectFile & ". The full error is: " & ex.ToString)
                End Try
                SpectTransferBackgroundWorker.ReportProgress((i / (filesList.Count - 1)) * 100)
                i += 1
            Next
        Catch ex As Exception
            logError("Error occurred in spect transfer bw_dowork. The full error is: " & ex.ToString)
        End Try
    End Sub

    Private Function readyFileMethod(ByVal source As String) As List(Of String)
        Dim filesList As New List(Of String)
        Try
            filesList.Clear()
            If File.Exists(Path.GetTempPath & "ready") Then
                File.Delete(Path.GetTempPath & "ready")
            End If

            'telnet in and generate the ready file; this should take between 5 and 20 minutes, depending on how full the img is.
            'during this time, the tst10.exe program will be running
            acf.doTelnet({"./sha/ck_od"}, source)

            My.Computer.Network.DownloadFile("ftp://" & source & "/%2fusr/prism/sha/ready", Path.GetTempPath & "ready", My.Settings.OdysseyUN, My.Settings.OdysseyPW)
            Dim ct As Integer = 0
            Using sr As New StreamReader(Path.GetTempPath & "ready")
                While sr.Peek <> -1
                    Dim line As String = sr.ReadLine
                    Using con As New SqlConnection(My.Settings.LocalConString)
                        Using cmd As New SqlCommand("SELECT COUNT(*) FROM SPECT_Data_Hashes WHERE DataFileHash_Hex=@hash", con)
                            cmd.Parameters.Add("@hash", SqlDbType.NVarChar).Value = line.Substring(0, line.IndexOf(" - ")).Trim
                            con.Open()
                            ct = cmd.ExecuteScalar
                            con.Close()
                            If ct = 0 Then
                                filesList.Add("ftp://" & source & "/%2f" & line.Remove(0, 1).Substring(line.LastIndexOf(" ") + 1).Trim)
                            End If
                            con.Close()
                            cmd.Parameters.Clear()
                        End Using
                    End Using
                End While
            End Using
        Catch ex As Exception
            logError("Error occurred in readyFileMethod(); var source=" & source & ". The full error is: " & ex.ToString)
        End Try
        Return filesList
    End Function

    Private Function ftpMethod(ByVal source As String) As List(Of String)
        Dim ftpReq As FtpWebRequest
        Dim dirList As New List(Of String)
        Dim filesList As New List(Of String)
        Try
            ftpReq = WebRequest.Create("ftp://" & source) '192.168.0.80/img16
            ftpReq.Credentials = New NetworkCredential(My.Settings.OdysseyUN, My.Settings.OdysseyPW)
            ftpReq.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            Using ftpRes As FtpWebResponse = ftpReq.GetResponse
                Using sr As New StreamReader(ftpRes.GetResponseStream)
                    Do Until sr.EndOfStream
                        Dim line As String = Nothing
                        Try
                            line = sr.ReadLine
                            If isDirectoryOfInterest(line) Then
                                dirList.Add(source.Replace("/", "/%2f") & "/" & line.Substring(line.LastIndexOf(" ") + 1)) '192.168.0.80/%2fimg16/P123
                            End If
                        Catch ex As Exception
                            logError(ex.ToString)
                        End Try
                    Loop
                    ftpRes.Close()
                    sr.Close()
                End Using
            End Using
        Catch ex As Exception
            logError("Error occurred in ftpMethod(); var source=" & source & ". The full error is: " & ex.ToString)
        End Try
        For Each pFolder As String In dirList
            Try
                ftpReq = WebRequest.Create("ftp://" & pFolder) '192.168.0.80/%2fimg16/P123
                ftpReq.Credentials = New NetworkCredential(My.Settings.OdysseyUN, My.Settings.OdysseyPW)
                ftpReq.KeepAlive = False
                ftpReq.Timeout = 10000
                ftpReq.ReadWriteTimeout = 180000
                ftpReq.Method = WebRequestMethods.Ftp.ListDirectoryDetails
                Using ftpRes As FtpWebResponse = ftpReq.GetResponse
                    Using sr As New StreamReader(ftpRes.GetResponseStream)
                        Dim line As String = Nothing
                        Do Until sr.EndOfStream
                            Try
                                line = sr.ReadLine
                                If isFileOfInterest(line) Then
                                    filesList.Add("ftp://" & pFolder & "/" & line.Substring(line.LastIndexOf(" ") + 1)) 'ftp://192.168.0.80/%2fimg16/P123/R1
                                End If
                            Catch ex As Exception
                                logError("Error occurred iterating though fptRes.GetResponseStream in ftpMethod(); var source=" & source & "; var pFolder=" & pFolder & _
                                         "; var line=" & line & ". The full error is: " & ex.ToString)
                            End Try
                        Loop
                        sr.Close()
                    End Using
                End Using
            Catch ex As Exception
                logError("Error occurred iterating through dirList in ftpMethod(); var source=" & source & "; var pFolder=" & pFolder & ". The full error is: " & ex.ToString)
            End Try
        Next
        Return filesList
    End Function

    Private Function isDirectoryOfInterest(ByVal input As String) As Boolean
        If input.Substring(0, 1) = "l" Then
            Return False
        End If
        Dim details As String() = input.Split(" ", 50, StringSplitOptions.RemoveEmptyEntries)
        If details.Count = 2 Then
            Return False
        End If
        Dim dateBack As Date = CStr(Today.AddDays(-1).Date & " 0:00")
        If details(details.Count - 2).Contains(":") Then
            If DateTime.ParseExact(details(details.Count - 4), "MMM", Globalization.CultureInfo.CurrentCulture).Month <= Today.Month Then
                details(details.Count - 2) = Today.Year
            Else
                details(details.Count - 2) = Today.AddYears(-1).Year
            End If
        End If
        Dim datemod As Date = CStr(details(details.Count - 4) & " " & details(details.Count - 3) & " " & details(details.Count - 2) & " 0:00")
        If details(0).Substring(0, 1) = "d" Then
            If details(details.Count - 1).Substring(0, 1) = "P" And IsNumeric(details(details.Count - 1).Substring(1)) Then
                If DateTime.Compare(datemod, dateBack) >= 0 Then
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Private Function isFileOfInterest(ByVal input As String) As Boolean
        Dim details As String() = input.Split(" ", 50, StringSplitOptions.RemoveEmptyEntries)
        If details.Count = 2 Then
            Return False
        End If
        Dim dateBack As Date = CStr(Today.AddDays(-1).Date & " 0:00")
        If details(details.Count - 2).Contains(":") Then
            If DateTime.ParseExact(details(details.Count - 4), "MMM", Globalization.CultureInfo.CurrentCulture).Month <= Today.Month Then
                details(details.Count - 2) = Today.Year
            Else
                details(details.Count - 2) = Today.AddYears(-1).Year
            End If
        End If
        Dim datemod As Date = CStr(details(details.Count - 4) & " " & details(details.Count - 3) & " " & details(details.Count - 2) & " 0:00")
        If (details(details.Count - 1).Substring(0, 1) = "O" And IsNumeric(details(details.Count - 1).Substring(1))) Or _
            (details(details.Count - 1).Substring(0, 1) = "R" And IsNumeric(details(details.Count - 1).Substring(1))) Then
            If DateTime.Compare(datemod, dateBack) >= 0 Then
                Return True
            End If
        End If
        Return False
    End Function

    Private Sub SpectTransferBackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles SpectTransferBackgroundWorker.ProgressChanged
        Try
            If e.UserState IsNot Nothing Then
                If My.Settings.SpectTransferStatusList.Count > 1000000 Then
                    My.Settings.SpectTransferStatusList.RemoveAt(0)
                End If
                My.Settings.SpectTransferStatusList.Add(e.UserState)
                If SpectTransferDetails.Visible = True Then
                    SpectTransferDetails.SpectTransferListBox.Items.Add(e.UserState)
                    SpectTransferDetails.SpectTransferListBox.TopIndex = SpectTransferDetails.SpectTransferListBox.Items.Count - 1
                End If
            Else
                SpectTransferProgressBar.Value = e.ProgressPercentage
            End If
        Catch ex As Exception
            logError(ex.ToString)
        End Try
        My.Settings.SpectWatcher = Now
        My.Settings.Save()
    End Sub

    Private Sub SpectTransferBackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles SpectTransferBackgroundWorker.RunWorkerCompleted
        If e.Error IsNot Nothing Then
            logError(e.Error.ToString)
            If My.Settings.SpectTransferStatusList.Count > 1000000 Then
                My.Settings.JpegTransferStatusList.RemoveAt(0)
            End If
            My.Settings.JpegTransferStatusList.Add(e.Error.Message)
            If SpectTransferDetails.Visible = True Then
                SpectTransferDetails.SpectTransferListBox.Items.Add(e.Error.Message)
                SpectTransferDetails.SpectTransferListBox.TopIndex = SpectTransferDetails.SpectTransferListBox.Items.Count - 1
            End If
        End If
        SpectTransferDetails.SpectTransferListBox.Items.Add(Now & ": Spect Transfer has Completed.")
        SpectTransferDetails.SpectTransferListBox.TopIndex = SpectTransferDetails.SpectTransferListBox.Items.Count - 1
        My.Settings.Save()
        SpectTransferProgressBar.Value = 0
    End Sub

    Private Sub SpectDataDetailsLinkLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles SpectDataDetailsLinkLabel.LinkClicked
        SpectTransferDetails.Show()
        SpectTransferDetails.BringToFront()
    End Sub

    Private Sub JpegTransferProgressBar_MouseHover(sender As Object, e As EventArgs) Handles JpegTransferProgressBar.MouseHover
        ToolTip1.Show("Recent Transfer: " & My.Settings.RecentJpegTransfer.ToString("MM/dd/yy h:mm tt") & vbNewLine & _
                      "Next Transfer: " & My.Settings.RecentJpegTransfer.AddMilliseconds(JpegTransferTimer.Interval).ToString("MM/dd/yy h:mm tt"), JpegTransferProgressBar, 5000)
    End Sub

    Private Sub SpectTransferProgressBar_MouseHover(sender As Object, e As EventArgs) Handles SpectTransferProgressBar.MouseHover
        ToolTip1.Show("Recent Transfer: " & My.Settings.RecentSpectTransfer.ToString("MM/dd/yy h:mm tt") & vbNewLine & _
                      "Next Transfer: " & My.Settings.RecentSpectTransfer.AddMilliseconds(SpectTransferTimer.Interval).ToString("MM/dd/yy h:mm tt"), SpectTransferProgressBar, 5000)
    End Sub

    Private Sub InterfileTransferTimer_Tick(sender As Object, e As EventArgs) Handles InterfileTransferTimer.Tick
        If IFileTransferBackgroundWorker.IsBusy = False Then
            IFileTransferProgressBar.Value = 0
            My.Settings.RecentIFileTransfer = Now
            My.Settings.Save()
            IFileTransferBackgroundWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub IFileTransferBackgroundWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles IFileTransferBackgroundWorker.DoWork
        Try
            Dim dt As New DataTable
            Dim result_message As String
            Dim Result_number As Integer
            'make sure the Odyssey machine is online
            If My.Computer.Network.Ping("192.168.0.80") = False Then
                IFileTransferBackgroundWorker.ReportProgress(0, "192.168.0.80 is unavailable")
                Exit Sub
            End If

            IFileTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": Retrieving list of interfiles needed..."))

            Using con As New SqlConnection(My.Settings.LocalConString)
                Using cmd As New SqlCommand("select Data_ID, Lastname, Firstname, Protocol, [FileName], Max_Filename from TransObl", con)
                    Using da As New SqlDataAdapter(cmd)
                        con.Open()
                        da.Fill(dt)
                        con.Close()
                        If dt.Rows.Count = 0 Then
                            IFileTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": No interfiles needed"))
                        Else
                            Dim paths() As String

                            For i As Integer = 0 To dt.Rows.Count - 1
                                Try
                                    result_message = Nothing
                                    paths = Nothing

                                    IFileTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": Processing " & dt.Rows(i).Item("Protocol") & " scan for " & _
                                        dt.Rows(i).Item("Lastname") & ", " & dt.Rows(i).Item("Firstname") & " (" & dt.Rows(i).Item("FileName") & ")"))

                                    paths = acf.odysseyToROI(dt.Rows(i).Item("Data_ID"), "192.168.0.80/img16/ResearchOFiles", _
                                            My.Settings.OdysseyUN, My.Settings.OdysseyPW, "S:\Interfiles", "S:\Images", "S:\Images\Images", _
                                            "S:\Images\Reports", "S:\Images\Manifests")

                                    'if Max_Filename has a value, then that processing is the most recent processing of that scan (the processing from
                                    'which the spect report will (should) be based off of. Parse only this one into the ROI_Extract_Headers/ROI_Extracts.
                                    'The other processings have already gone into ROI_Data in the odysseyToROI() routine.

                                    If Not IsDBNull(dt.Rows(i).Item("Max_Filename")) AndAlso paths(3) <> Nothing Then
                                        result_message = acf.parseROI(paths(3))
                                        IFileTransferBackgroundWorker.ReportProgress(0, CStr(Now & ": " & result_message))
                                    End If
                                Catch ex As Exception
                                    logError("Error uploading/downloading/processing Data_ID " & dt.Rows(i).Item("Data_ID") & ": " & ex.ToString)
                                    IFileTransferBackgroundWorker.ReportProgress(0, "Error processing Data_ID " & dt.Rows(i).Item("Data_ID") & ": " & ex.Message)
                                End Try
                                IFileTransferBackgroundWorker.ReportProgress(((i + 1) / dt.Rows.Count) * 100)
                            Next
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            logError("Error in IFileTransferBackgroundWorker: " & ex.ToString)
            IFileTransferBackgroundWorker.ReportProgress(0, "Error in IFileTransferBackgroundWorker: " & ex.Message)
        End Try
    End Sub

    Private Sub IFileTransferBackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles IFileTransferBackgroundWorker.ProgressChanged
        If e.UserState Is Nothing Then
            IFileTransferProgressBar.Value = e.ProgressPercentage
        Else
            If My.Settings.IFileTransferStatusList.Count > 1000000 Then
                My.Settings.IFileTransferStatusList.RemoveAt(0)
            End If
            My.Settings.IFileTransferStatusList.Add(e.UserState)
            If IFileTransferDetails.Visible = True Then
                IFileTransferDetails.IFileTransferListBox.Items.Add(e.UserState)
                IFileTransferDetails.IFileTransferListBox.TopIndex = IFileTransferDetails.IFileTransferListBox.Items.Count - 1
            End If
        End If
        My.Settings.Save()
    End Sub

    Private Sub IFileTransferBackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles IFileTransferBackgroundWorker.RunWorkerCompleted
        If e.Error IsNot Nothing Then
            If My.Settings.IFileTransferStatusList.Count > 1000000 Then
                My.Settings.IFileTransferStatusList.RemoveAt(0)
            End If
            My.Settings.IFileTransferStatusList.Add(e.Error.Message)
            If IFileTransferDetails.Visible = True Then
                IFileTransferDetails.IFileTransferListBox.Items.Add(e.Error.Message)
                IFileTransferDetails.IFileTransferListBox.TopIndex = IFileTransferDetails.IFileTransferListBox.Items.Count - 1
            End If
        End If
        IFileTransferDetails.IFileTransferListBox.Items.Add(Now & ": IFile Transfer Compelted.")
        IFileTransferDetails.IFileTransferListBox.TopIndex = IFileTransferDetails.IFileTransferListBox.Items.Count - 1
        My.Settings.Save()
        IFileTransferProgressBar.Value = 0
    End Sub

    Private Sub IfileTransferDetailsLinkLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles IfileTransferDetailsLinkLabel.LinkClicked
        IFileTransferDetails.Show()
        IFileTransferDetails.BringToFront()
    End Sub

    Private Sub IFileTransferProgressBar_MouseHover(sender As Object, e As EventArgs) Handles IFileTransferProgressBar.MouseHover
        ToolTip1.Show("Recent Transfer: " & My.Settings.RecentIFileTransfer.ToString("MM/dd/yy h:mm tt") & vbNewLine & _
                      "Next Transfer: " & My.Settings.RecentIFileTransfer.AddMilliseconds(InterfileTransferTimer.Interval).ToString("MM/dd/yy h:mm tt"), IFileTransferProgressBar, 5000)
    End Sub

    Private Sub OnlineInquiryTimer_Tick(sender As Object, e As EventArgs) Handles OnlineInquiryTimer.Tick
        If OnlineInquiryBackgroundWorker.IsBusy = False Then
            OnlineInquiryProgressBar.Value = 0
            My.Settings.RecentOnlineInquiryTransfer = Now
            My.Settings.Save()
            OnlineInquiryBackgroundWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub OnlineInquiryBackgroundWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles OnlineInquiryBackgroundWorker.DoWork
        Try
            Dim maxSqlCt As Integer = Nothing
            Dim minSqlCt As Integer = Nothing
            Dim ds As New DataSet
            Using con As New SqlConnection(My.Settings.MarketingConString)
                Using cmd As New SqlCommand("SELECT MAX(Online_SubmissionID) AS Max_SID, MIN(Online_SubmissionID) AS Min_SID FROM Patient_Inquiry", con)
                    con.Open()
                    Using sdr As SqlDataReader = cmd.ExecuteReader
                        sdr.Read()
                        maxSqlCt = sdr("Max_SID")
                        minSqlCt = sdr("Min_SID")
                        sdr.Close()
                    End Using
                    con.Close()
                End Using
            End Using
            OnlineInquiryBackgroundWorker.ReportProgress(0, CStr(Now & ": Retrieving new inquiries..."))
            Using con As New MySqlConnection(My.Settings.MySQLInquiryConString)
                Using cmd As New MySqlCommand("SELECT DISTINCT SubmissionID FROM Online_Inquiry WHERE SubmissionID > @maxsid OR SubmissionID < @minsid", con)
                    cmd.Parameters.Add("@maxsid", MySqlDbType.Int64).Value = maxSqlCt
                    cmd.Parameters.Add("@minsid", MySqlDbType.Int64).Value = minSqlCt
                    Using da As New MySqlDataAdapter(cmd)
                        con.Open()
                        da.Fill(ds, "SubmissionIDs")
                        con.Close()
                    End Using
                    cmd.Parameters.Clear()
                End Using
            End Using
            If ds.Tables("SubmissionIDs").Rows.Count = 0 Then
                OnlineInquiryBackgroundWorker.ReportProgress(0, CStr(Now & ": No new inquiries"))
            End If
            Dim i As Integer = 0
            For Each dtRow As DataRow In ds.Tables("SubmissionIDs").Rows
                Try
                    Using con As New MySqlConnection(My.Settings.MySQLInquiryConString)
                        Using cmd As New MySqlCommand
                            cmd.Connection = con
                            cmd.CommandText = "NPIntakeValues"
                            cmd.CommandType = CommandType.StoredProcedure
                            cmd.Parameters.Add("@sid", MySqlDbType.Int64).Value = dtRow.Item("SubmissionID")
                            cmd.Parameters("@sid").Direction = ParameterDirection.InputOutput
                            cmd.Parameters.AddWithValue("@FullName", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@PhoneNumber", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@BestTimeToCall", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@Email", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@AppointmentFor", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@PatientFullName", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@Clinic", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@DescribeYourIssue", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@HearAbout", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@LeaveMessage", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@Message", MySqlDbType.VarChar).Direction = ParameterDirection.Output
                            cmd.Parameters.AddWithValue("@DateSubmitted", MySqlDbType.DateTime).Direction = ParameterDirection.Output
                            Using da As New MySqlDataAdapter(cmd)
                                con.Open()
                                da.Fill(ds, "Inquiry")
                                con.Close()
                            End Using
                            cmd.Parameters.Clear()
                        End Using
                    End Using
                    OnlineInquiryBackgroundWorker.ReportProgress(0, CStr(Now & ": Importing " & ds.Tables("Inquiry").Rows(0).Item("FullName") & " into MSSQL..."))
                    Dim messageSB As New System.Text.StringBuilder
                    Using con As New SqlConnection(My.Settings.MarketingConString)
                        Using cmd As New SqlCommand
                            cmd.Connection = con
                            cmd.CommandText = "execute New_OnlineInquiry @sid, @callerfullname, @phonenumber, @besttimetocall, @email, @appointmentfor, " _
                                & "@patientfullname, @clinic, @hearabout, @leavemessage, @message, @describeyourissue, @datesubmitted"
                            With ds.Tables("Inquiry").Rows(0)
                                cmd.Parameters.Add("@sid", SqlDbType.BigInt).Value = .Item("sid")
                                cmd.Parameters.Add("@callerfullname", SqlDbType.NVarChar).Value = .Item("FullName")
                                cmd.Parameters.Add("@phonenumber", SqlDbType.NVarChar).Value = .Item("PhoneNumber")
                                cmd.Parameters.Add("@besttimetocall", SqlDbType.NVarChar).Value = .Item("BestTimeToCall")
                                cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = .Item("Email")
                                cmd.Parameters.Add("@appointmentfor", SqlDbType.NVarChar).Value = .Item("AppointmentFor")
                                cmd.Parameters.Add("@patientfullname", SqlDbType.NVarChar).Value = .Item("PatientFullName")
                                cmd.Parameters.Add("@clinic", SqlDbType.NVarChar).Value = .Item("Clinic")
                                cmd.Parameters.Add("@hearabout", SqlDbType.NVarChar).Value = .Item("HearAbout")
                                cmd.Parameters.Add("@leavemessage", SqlDbType.NVarChar).Value = .Item("LeaveMessage")
                                cmd.Parameters.Add("@message", SqlDbType.NVarChar).Value = .Item("Message")
                                cmd.Parameters.Add("@describeyourissue", SqlDbType.NVarChar).Value = .Item("DescribeYourIssue")
                                cmd.Parameters.Add("@datesubmitted", SqlDbType.DateTime).Value = .Item("DateSubmitted")
                            End With
                            con.Open()
                            cmd.ExecuteNonQuery()
                            con.Close()
                            cmd.Parameters.Clear()
                            'If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("Clinic")) = False Then
                            '    If ds.Tables("Inquiry").Rows(0).Item("Clinic") = "Reston" Then
                            '        Dim sb As New System.Text.StringBuilder
                            '        sb.Append("Appointment Request").AppendLine().AppendLine()
                            '        sb.Append("Full Name:")
                            '        If IsDBNull("FullName") = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("FullName")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Who is the Appointment For?")
                            '        If IsDBNull("AppointmentFor") = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("AppointmentFor")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Patient's Full Name:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("PatientFullName")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("PatientFullName")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Preferred Appointment Date:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("AppointmentDate")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("AppointmentDate")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Which Clinic?")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("Clinic")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("Clinic")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Phone Number:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("PhoneNumber")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("PhoneNumber")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Best Time to Call:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("BestTimeToCall")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("BestTimeToCall")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Describe Your Issue:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("DescribeYourIssue")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("DescribeYourIssue")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Procedure Interest:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("ProcedureInterest")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("ProcedureInterest")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Leave Message:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("LeaveMessage")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("LeaveMessage")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Your Message:")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("Message")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("Message")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("How did you hear about us?")
                            '        If IsDBNull(ds.Tables("Inquiry").Rows(0).Item("HearAbout")) = False Then
                            '            sb.Append(" " & ds.Tables("Inquiry").Rows(0).Item("HearAbout")).AppendLine()
                            '        Else
                            '            sb.AppendLine()
                            '        End If
                            '        sb.Append("Inquiry Submitted: " & ds.Tables("Inquiry").Rows(0).Item("DateSubmitted"))
                            '        Dim outApp As Outlook.Application = New Outlook.Application
                            '        Dim mail As Outlook.MailItem = outApp.CreateItem(Outlook.OlItemType.olMailItem)
                            '        mail.To = My.Settings.RestonEmailTo
                            '        mail.CC = My.Settings.RestonEmailCc
                            '        mail.BCC = My.Settings.RestonEmailBcc
                            '        mail.Subject = "Appointment Request - Reston, VA"
                            '        mail.Body = sb.ToString
                            '        mail.Send()
                            '        sb.Remove(0, sb.Length - 1)
                            '    End If
                            'End If
                        End Using
                    End Using
                Catch ex As Exception
                    logError(ex.ToString)
                    OnlineInquiryBackgroundWorker.ReportProgress(0, CStr(Now & ": Error importing " & dtRow.Item("SubmissionID") & ": " & ex.Message))
                End Try
                ds.Tables("Inquiry").Clear()
                i += 1
                OnlineInquiryBackgroundWorker.ReportProgress((i / ds.Tables("SubmissionIDs").Rows.Count) * 100)
            Next
            ds.Tables.Clear()
        Catch ex As Exception
            logError(ex.ToString)
            OnlineInquiryBackgroundWorker.ReportProgress(0, CStr(Now & ": " & ex.Message))
        End Try
    End Sub

    Private Sub OnlineInquiryBackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles OnlineInquiryBackgroundWorker.ProgressChanged
        If e.UserState Is Nothing Then
            OnlineInquiryProgressBar.Value = e.ProgressPercentage
        Else
            If My.Settings.OnlineInquiryStatusList.Count > 1000000 Then
                My.Settings.OnlineInquiryStatusList.RemoveAt(0)
            End If
            My.Settings.OnlineInquiryStatusList.Add(e.UserState)
            If OnlineInquiryDetails.Visible = True Then
                OnlineInquiryDetails.OnlineInquiryListBox.Items.Add(e.UserState)
                OnlineInquiryDetails.OnlineInquiryListBox.TopIndex = OnlineInquiryDetails.OnlineInquiryListBox.Items.Count - 1
            End If
        End If
        My.Settings.Save()
    End Sub

    Private Sub OnlineInquiryBackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles OnlineInquiryBackgroundWorker.RunWorkerCompleted
        If e.Error IsNot Nothing Then
            If My.Settings.OnlineInquiryStatusList.Count > 1000000 Then
                My.Settings.OnlineInquiryStatusList.RemoveAt(0)
            End If
            My.Settings.OnlineInquiryStatusList.Add(e.Error.Message)
            If OnlineInquiryDetails.Visible = True Then
                OnlineInquiryDetails.OnlineInquiryListBox.Items.Add(e.Error.Message)
                OnlineInquiryDetails.OnlineInquiryListBox.TopIndex = OnlineInquiryDetails.OnlineInquiryListBox.Items.Count - 1
            End If
        End If
        OnlineInquiryDetails.OnlineInquiryListBox.Items.Add(Now & ": Online Inquiry Transfer has Completed.")
        OnlineInquiryDetails.OnlineInquiryListBox.TopIndex = OnlineInquiryDetails.OnlineInquiryListBox.Items.Count - 1
        My.Settings.Save()
        OnlineInquiryProgressBar.Value = 0
    End Sub

    Private Sub OnlineInquiryDetailsLinkLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles OnlineInquiryDetailsLinkLabel.LinkClicked
        OnlineInquiryDetails.Show()
        OnlineInquiryDetails.BringToFront()
    End Sub

    Private Sub OnlineInquiryProgressBar_MouseHover(sender As Object, e As EventArgs) Handles OnlineInquiryProgressBar.MouseHover
        ToolTip1.Show("Recent Transfer: " & My.Settings.RecentOnlineInquiryTransfer.ToString("MM/dd/yy h:mm tt") & vbNewLine & _
                      "Next Transfer: " & My.Settings.RecentOnlineInquiryTransfer.AddMilliseconds(OnlineInquiryTimer.Interval).ToString("MM/dd/yy h:mm tt"), OnlineInquiryProgressBar, 5000)
    End Sub

    'Private Sub JpegTransferToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JpegTransferToolStripMenuItem.Click
    '    If JpegTransferBackgroundWorker.IsBusy = False Then
    '        JpegTransferTimer_Tick(sender, New System.EventArgs)
    '    End If
    'End Sub

    'Private Sub SpectTransferToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpectTransferToolStripMenuItem.Click
    '    If SpectTransferBackgroundWorker.IsBusy = False Then
    '        SpectTransferTimer_Tick(sender, New System.EventArgs)
    '    End If
    'End Sub

    'Private Sub OnlineInquiryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OnlineInquiryToolStripMenuItem.Click
    '    If OnlineInquiryBackgroundWorker.IsBusy = False Then
    '        OnlineInquiryTimer_Tick(sender, New System.EventArgs)
    '    End If
    'End Sub

    Private Sub ReadingsImportTimer_Tick(sender As Object, e As EventArgs) Handles ReadingsImportTimer.Tick
        If ReadingsImportBackgroundWorker.IsBusy = False Then
            ReadingsImportProgressBar.Value = 0
            My.Settings.RecentReadingsTransfer = Now
            My.Settings.Save()
            ReadingsImportBackgroundWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub ReadingsImportBackgroundWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles ReadingsImportBackgroundWorker.DoWork
        Try
            Dim ds As New DataSet
            ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Starting Spect Report Grab."))
            'run through the patient archive directory and starts testing excel files to see if they need to be uploaded
            Dim fpath As String = "\\ac2\data\Patient Archive\Cumulative Reader Reports\"
            fpath = fpath + DatePart(DateInterval.Year, Date.Today).ToString
            ''fpath = "\\ac2\data\Patient Archive\Cumulative Reader Reports\2015\March"
            Dim files As String() = Directory.GetFiles(fpath, "*.xlsm", SearchOption.AllDirectories)
            Dim hashcheck As Integer
            Dim results As String
            For i As Integer = 0 To files.Count - 1
                Dim item_path As String = files(i)
                If Not item_path.Contains(".xlsm") Then
                    i += 1
                    Continue For
                End If
                If item_path.Contains("Reports\2015\January") Then
                    ''This whole if statement is just test code
                    item_path = item_path
                End If
                Dim hashvalue As String = acf.getSHA1Hash(item_path)
                Using con As New SqlConnection(My.Settings.ResearchConString)
                    Using cmd As SqlCommand = New SqlCommand
                        cmd.Connection = con
                        cmd.CommandText = "select count(*) from importdata..spect_reading_header where check_sum=@check_sum"
                        cmd.Parameters.Add("@check_sum", SqlDbType.VarChar, 50).Value = hashvalue
                        con.Open()
                        hashcheck = cmd.ExecuteScalar
                        con.Close()
                    End Using
                End Using
                If hashcheck = 1 Then
                    'ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Report already imported into database."))
                    Continue For
                Else
                    results = acf.parseReadings(item_path)
                    If results = 0 Then
                        ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Unable to Import Report: " & Path.GetFileName(item_path)))
                    ElseIf results = 1 Then
                        ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Imported Spect Report into Importdata but did not match: " & Path.GetFileName(item_path)))
                    ElseIf results = 2 Then
                        ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Imported Data and matched to Patient: " & Path.GetFileName(item_path)))
                    End If
                End If
            Next
        Catch ex As Exception
            logError(ex.ToString)
            ''Test Only
            MsgBox(ex.Message)

        Finally
            dt.Clear()
        End Try
        'Using con As New SqlConnection(My.Settings.ResearchConString)
        '    Using cmd As New SqlCommand("SELECT MAX(mysql_reading_id) FROM ImportData..Spect_Reading_Header", con)
        '        con.Open()
        '        maxNum = cmd.ExecuteScalar
        '        con.Close()
        '    End Using
        'End Using

        'spect_readings_full in MySQL db = spect_reading_header joined with ratings.
        'Using con As New MySqlConnection(My.Settings.MySQLIntakeConString)
        '    'Get the headers
        '    Using cmd As New MySqlCommand("SELECT DISTINCT reading_id, patient_id, NULLIF(last_name,'') AS last_name, NULLIF(first_name,'') AS first_name, NULLIF(dob,'') AS dob, NULLIF(gender,'') AS gender, " _
        '            & "NULLIF(doctor,'') AS doctor, NULLIF(following_medication,'') AS following_medication, NULLIF(clinic,'') AS clinic, NULLIF(rest_scan_date,'') AS rest_scan_date, " _
        '            & "NULLIF(conc_scan_date,'') AS conc_scan_date, NULLIF(follow_scan_date,'') AS follow_scan_date, NULLIF(read_date,'') AS read_date, NULLIF(reader,'') AS reader, " _
        '            & "NULLIF(referral,'') AS referral, NULLIF(report_type,'') AS report_type, NULLIF(outside_source,'') AS outside_source FROM spect_readings_full WHERE Reading_ID > " & maxNum, con)
        '        Using da As New MySqlDataAdapter(cmd)
        '            con.Open()
        '            da.Fill(ds, "spect_reading_header")
        '            con.Close()
        '            'get the readings
        '            cmd.CommandText = "SELECT reading_id, cerebral_area_rated, rest_inc, rest_dec, conc_inc, conc_dec FROM spect_readings_full WHERE reading_id > " & maxNum
        '            da.SelectCommand = cmd
        '            con.Open()
        '            da.Fill(ds, "spect_readings")
        '            con.Close()
        '        End Using
        '    End Using
        'End Using
        'If ds.Tables("spect_reading_header").Rows.Count > 0 Then
        '    For i As Integer = 0 To ds.Tables("spect_reading_header").Rows.Count - 1
        '        Dim reading_id As Int64 = Nothing
        '        ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Importing reading for " & ds.Tables("spect_reading_header").Rows(i).Item("last_name") & ", " & _
        '               ds.Tables("spect_reading_header").Rows(i).Item("first_name") & " (Reading_id " & ds.Tables("spect_reading_header").Rows(i).Item("reading_id") & ")"))
        '        'Try
        '        '    Using con As New SqlConnection(My.Settings.ResearchConString)
        '        '        'Bring header into MSSQL ImportData
        '        '        Using cmd As New SqlCommand("Execute PatientResearch..New_SPECT_Reading_Header @Input_URL, @lastname , @firstname, @age, @dob, @nuemd_pid, @gender, " _
        '        '            & "@physician, @medications, @clinic, @rest_scan_date, @conc_scan_date, @follow_scan_date, @read_date,@reader, @referral,@report_type,@outside_source, @check_sum, @mysqlreadingid, 0", con)
        '        '            cmd.Parameters.Add("@input_url", SqlDbType.VarChar).Value = "Returned from online intake; " & CStr(ds.Tables("spect_reading_header").Rows(i).Item("reading_id"))
        '        '            cmd.Parameters.Add("@lastname", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("last_name")
        '        '            cmd.Parameters.Add("@firstname", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("first_name")
        '        '            If IsDBNull(ds.Tables("spect_reading_header").Rows(i).Item("dob")) = False Then
        '        '                If IsDBNull(ds.Tables("spect_reading_header").Rows(i).Item("follow_scan_date")) = False Then
        '        '                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = DateDiff(DateInterval.Year, ds.Tables("spect_reading_header").Rows(i).Item("dob"), ds.Tables("spect_reading_header").Rows(i).Item("follow_scan_date"))
        '        '                ElseIf IsDBNull(ds.Tables("spect_reading_header").Rows(i).Item("conc_scan_date")) = False Then
        '        '                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = DateDiff(DateInterval.Year, ds.Tables("spect_reading_header").Rows(i).Item("dob"), ds.Tables("spect_reading_header").Rows(i).Item("conc_scan_date"))
        '        '                ElseIf IsDBNull(ds.Tables("spect_reading_header").Rows(i).Item("rest_scan_date")) = False Then
        '        '                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = DateDiff(DateInterval.Year, ds.Tables("spect_reading_header").Rows(i).Item("dob"), ds.Tables("spect_reading_header").Rows(i).Item("rest_scan_date"))
        '        '                Else
        '        '                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = DateDiff(DateInterval.Year, ds.Tables("spect_reading_header").Rows(i).Item("dob"), ds.Tables("spect_reading_header").Rows(i).Item("read_date"))
        '        '                End If
        '        '            Else
        '        '                cmd.Parameters.Add("@age", SqlDbType.Int).Value = DBNull.Value
        '        '            End If
        '        '            cmd.Parameters.Add("@dob", SqlDbType.Date).Value = ds.Tables("spect_reading_header").Rows(i).Item("dob")
        '        '            cmd.Parameters.Add("@nuemd_pid", SqlDbType.Int).Value = ds.Tables("spect_reading_header").Rows(i).Item("patient_id")
        '        '            cmd.Parameters.Add("@gender", SqlDbType.Char).Value = ds.Tables("spect_reading_header").Rows(i).Item("gender")
        '        '            cmd.Parameters.Add("@physician", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("doctor")
        '        '            cmd.Parameters.Add("@medications", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("following_medication")
        '        '            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("clinic")
        '        '            cmd.Parameters.Add("@rest_scan_date", SqlDbType.Date).Value = ds.Tables("spect_reading_header").Rows(i).Item("rest_scan_date")
        '        '            cmd.Parameters.Add("@conc_scan_date", SqlDbType.Date).Value = ds.Tables("spect_reading_header").Rows(i).Item("conc_scan_date")
        '        '            cmd.Parameters.Add("@follow_scan_date", SqlDbType.Date).Value = ds.Tables("spect_reading_header").Rows(i).Item("follow_scan_date")
        '        '            cmd.Parameters.Add("@read_date", SqlDbType.Date).Value = ds.Tables("spect_reading_header").Rows(i).Item("read_date")
        '        '            cmd.Parameters.Add("@reader", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("reader")
        '        '            cmd.Parameters.Add("@referral", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("referral")
        '        '            cmd.Parameters.Add("@report_type", SqlDbType.VarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("report_type")
        '        '            cmd.Parameters.Add("@outside_source", SqlDbType.NVarChar).Value = ds.Tables("spect_reading_header").Rows(i).Item("outside_source")
        '        '            cmd.Parameters.Add("@check_sum", SqlDbType.NVarChar).Value = DBNull.Value
        '        '            cmd.Parameters.Add("@mysqlreadingid", SqlDbType.BigInt).Value = ds.Tables("spect_reading_header").Rows(i).Item("reading_id")
        '        '            con.Open()
        '        '            reading_id = cmd.ExecuteScalar
        '        '            con.Close()
        '        '            cmd.Parameters.Clear()
        '        '        End Using
        '        '    End Using
        '        'Catch ex As Exception
        '        '    If reportReadingError(ds.Tables("spect_reading_header").Rows(i).Item("reading_id"), True, ex.Message) = False Then
        '        '        logError(ex.ToString)
        '        '    End If
        '        '    ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Error importing header for Reading_id " & ds.Tables("spect_reading_header").Rows(i).Item("reading_id") & ": " _
        '        '            & ex.Message))
        '        'End Try
        '        If reading_id <> Nothing Then
        '            Dim readings() As DataRow = ds.Tables("spect_readings").Select("reading_id = " & ds.Tables("spect_reading_header").Rows(i).Item("reading_id"))
        '            For Each reading As DataRow In readings
        '                Try
        '                    Using con As New SqlConnection(My.Settings.ResearchConString)
        '                        Using cmd As New SqlCommand("Execute patientresearch..New_SPECT_Reading @reading_id,@cerebral_area_rated, @Rest_inc, @Rest_dec, @Conc_inc, @Conc_dec", con)
        '                            cmd.Parameters.Add("@reading_id", SqlDbType.BigInt).Value = reading_id
        '                            cmd.Parameters.Add("@cerebral_area_rated", SqlDbType.VarChar).Value = reading.Item("cerebral_area_rated")
        '                            cmd.Parameters.Add("@rest_inc", SqlDbType.VarChar).Value = reading.Item("rest_inc")
        '                            cmd.Parameters.Add("@rest_dec", SqlDbType.VarChar).Value = reading.Item("rest_dec")
        '                            cmd.Parameters.Add("@conc_inc", SqlDbType.VarChar).Value = reading.Item("conc_inc")
        '                            cmd.Parameters.Add("@conc_dec", SqlDbType.VarChar).Value = reading.Item("conc_dec")
        '                            con.Open()
        '                            cmd.ExecuteNonQuery()
        '                            con.Close()
        '                            cmd.Parameters.Clear()
        '                        End Using
        '                    End Using
        '                Catch ex As Exception
        '                    If reportReadingError(reading_id, False, ex.Message) = False Then
        '                        logError(ex.ToString)
        '                    End If
        '                    ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Error importing rating for Reading_id " & reading_id & ": " & ex.Message))
        '                End Try
        '            Next
        'Try
        '    Using con As New SqlConnection(My.Settings.ResearchConString)
        '        Using cmd As New SqlCommand("Execute ImportData..usp_Match_Single_Reading " & CStr(reading_id) & ", 0", con)
        '            con.Open()
        '            If cmd.ExecuteScalar = 1 Then
        '                ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Successfully matched reading_id " & reading_id))
        '            Else
        '                ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Failed to match reading_id " & reading_id))
        '            End If
        '            con.Close()
        '        End Using
        '    End Using
        'Catch ex As Exception
        '    If reportReadingError(reading_id, False, ex.Message) = False Then
        '        logError(ex.ToString)
        '    End If
        '    ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": Error executing match_reading for Reading_id " & reading_id & ": " & ex.Message))
        'End Try
        '        End If
        'ReadingsImportBackgroundWorker.ReportProgress(((i + 1) / ds.Tables("spect_reading_header").Rows.Count) * 100)
        '    Next
        'Else
        'ReadingsImportBackgroundWorker.ReportProgress(0, CStr(Now & ": No new readings to import"))
        'End If
    End Sub

    Private Function reportReadingError(ByVal reading_id As Long, ByVal mysql As Boolean, ByVal errorMessage As String) As Boolean
        Try
            Using con As New SqlConnection(My.Settings.ResearchConString)
                Using cmd As New SqlCommand("insert into audits..error_files (Error_File, Error_Message) VALUES (@error_file, @error_message)", con)
                    If mysql Then
                        cmd.Parameters.Add("@error_file", SqlDbType.NVarChar).Value = "MySQL Reading_ID " & reading_id
                    Else
                        cmd.Parameters.Add("@error_file", SqlDbType.NVarChar).Value = "MSSQL Reading_ID " & reading_id
                    End If
                    cmd.Parameters.Add("@error_message", SqlDbType.NVarChar).Value = errorMessage
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Sub ReadingsImportBackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles ReadingsImportBackgroundWorker.ProgressChanged
        Try
            If e.UserState IsNot Nothing Then
                If My.Settings.ReadingImportStatusList.Count > 1000000 Then
                    My.Settings.ReadingImportStatusList.RemoveAt(0)
                End If
                My.Settings.ReadingImportStatusList.Add(e.UserState)
                If ReadingsImportDetails.Visible = True Then
                    ReadingsImportDetails.ReadingsImportListBox.Items.Add(e.UserState)
                    ReadingsImportDetails.ReadingsImportListBox.TopIndex = ReadingsImportDetails.ReadingsImportListBox.Items.Count - 1
                End If
            Else
                ReadingsImportProgressBar.Value = e.ProgressPercentage
            End If
        Catch ex As Exception
            logError(ex.ToString)
        End Try
        My.Settings.Save()
    End Sub

    Private Sub ReadingsImportBackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles ReadingsImportBackgroundWorker.RunWorkerCompleted
        If e.Error IsNot Nothing Then
            If My.Settings.ReadingImportStatusList.Count > 1000000 Then
                My.Settings.ReadingImportStatusList.RemoveAt(0)
            End If
            My.Settings.ReadingImportStatusList.Add(e.Error.Message)
            If ReadingsImportDetails.Visible = True Then
                ReadingsImportDetails.ReadingsImportListBox.Items.Add(e.Error.Message)
                ReadingsImportDetails.ReadingsImportListBox.TopIndex = ReadingsImportDetails.ReadingsImportListBox.Items.Count - 1
            End If
        End If
        ReadingsImportDetails.ReadingsImportListBox.Items.Add(Now & ": Readings Import has Completed.")
        My.Settings.ReadingImportStatusList.Add(Now & ": Readings Import has Completed.")
        ReadingsImportDetails.ReadingsImportListBox.TopIndex = ReadingsImportDetails.ReadingsImportListBox.Items.Count - 1
        My.Settings.Save()
        ReadingsImportProgressBar.Value = 0
    End Sub

    Private Sub ReadingsImportProgressBar_MouseHover(sender As Object, e As EventArgs) Handles ReadingsImportProgressBar.MouseHover
        ToolTip1.Show("Recent Transfer: " & My.Settings.RecentReadingsTransfer.ToString("MM/dd/yy h:mm tt") & vbNewLine & _
                      "Next Transfer: " & My.Settings.RecentReadingsTransfer.AddMilliseconds(OnlineInquiryTimer.Interval).ToString("MM/dd/yy h:mm tt"), ReadingsImportProgressBar, 5000)
    End Sub

    Private Sub ReadingsImportLinkLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles ReadingsImportLinkLabel.LinkClicked
        ReadingsImportDetails.Show()
        ReadingsImportDetails.BringToFront()
    End Sub

    'Private Sub ReadingsImportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReadingsImportToolStripMenuItem.Click
    '    ReadingsImportTimer_Tick(sender, New System.EventArgs)
    'End Sub

    'Private Sub IFileTransferToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IFileTransferToolStripMenuItem.Click
    '    InterfileTransferTimer_Tick(sender, New System.EventArgs)
    'End Sub

    Private Sub enableTimers(sender As Object, e As EventArgs) Handles JpegTransferToolStripMenuItem1.CheckedChanged, SpectTransferToolStripMenuItem1.CheckedChanged, IFileTransferToolStripMenuItem1.CheckedChanged, _
        OnlineInquiryToolStripMenuItem1.CheckedChanged, ReadingsImportToolStripMenuItem1.CheckedChanged
        Dim menuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Select Case True
            Case menuItem Is JpegTransferToolStripMenuItem1
                JpegTransferTimer.Enabled = menuItem.Checked
                TimerJpegWatcher.Enabled = menuItem.Checked
            Case menuItem Is SpectTransferToolStripMenuItem1
                SpectTransferTimer.Enabled = menuItem.Checked
                TimerSpectWatcher.Enabled = menuItem.Checked
            Case menuItem Is IFileTransferToolStripMenuItem1
                InterfileTransferTimer.Enabled = menuItem.Checked
            Case menuItem Is OnlineInquiryToolStripMenuItem1
                OnlineInquiryTimer.Enabled = menuItem.Checked
            Case menuItem Is ReadingsImportToolStripMenuItem1
                ReadingsImportTimer.Enabled = menuItem.Checked
                'Case menuItem Is DirectoryTimerToolStripMenuItem
                '      TimerDirectoryScanner.Enabled = menuItem.Checked
                Startnow = 0
        End Select
    End Sub

    Private Sub tickTimers(sender As Object, e As EventArgs) Handles JpegTransferToolStripMenuItem.Click, SpectTransferToolStripMenuItem.Click, IFileTransferToolStripMenuItem.Click, _
        OnlineInquiryToolStripMenuItem.Click, ReadingsImportToolStripMenuItem.Click
        Dim menuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Select Case True
            Case menuItem Is JpegTransferToolStripMenuItem
                JpegTransferTimer_Tick(sender, New System.EventArgs)
            Case menuItem Is SpectTransferToolStripMenuItem
                SpectTransferTimer_Tick(sender, New System.EventArgs)
            Case menuItem Is IFileTransferToolStripMenuItem
                InterfileTransferTimer_Tick(sender, New System.EventArgs)
            Case menuItem Is OnlineInquiryToolStripMenuItem
                OnlineInquiryTimer_Tick(sender, New System.EventArgs)
            Case menuItem Is ReadingsImportToolStripMenuItem
                ReadingsImportTimer_Tick(sender, New System.EventArgs)
        End Select
    End Sub

    Private Sub JpegTransferProgressBar_Click(sender As Object, e As EventArgs) Handles JpegTransferProgressBar.Click
        JpegTransferBackgroundWorker.RunWorkerAsync()

    End Sub

    Private Sub TimerJpegWatcher_Tick(sender As Object, e As EventArgs) Handles TimerJpegWatcher.Tick
        Dim CurrentTime As Date = Now
        Try
            If JpegTransferTimer.Enabled = True Then
                If CurrentTime > My.Settings.JpegWatcher.AddMinutes(45) Then
                    Try
                        Dim olApp As Outlook.Application = New Outlook.Application
                        Dim mail As Outlook.MailItem = olApp.CreateItem(Outlook.OlItemType.olMailItem)
                        mail.To = "apickles@amenclinic.com; jshafer@amenclinic.com"
                        mail.Subject = "Jpegs Watcher - Flagged"
                        mail.Body = "The Jpeg Transfer froze at: " & CStr(My.Settings.JpegWatcher)
                        mail.Send()
                        TimerJpegWatcher.Enabled = False
                        My.Computer.Audio.Play(My.Resources.Sad_Trombone_Joe_Lamb_665429450, AudioPlayMode.Background)
                    Catch ex As Exception
                        Dim h As Integer = 0
                        logError(ex.Message)
                    End Try
                End If
            End If
        Catch ex As Exception
            Dim h As Integer = 0
        End Try
    End Sub

    Private Sub TimerSpectWatcher_Tick(sender As Object, e As EventArgs) Handles TimerSpectWatcher.Tick
        Dim CurrentTime As Date = Now
        Try
            If SpectTransferTimer.Enabled = True Then
                If CurrentTime > My.Settings.SpectWatcher.AddHours(2) Then
                    Try
                        Dim olApp As Outlook.Application = New Outlook.Application
                        Dim mail As Outlook.MailItem = olApp.CreateItem(Outlook.OlItemType.olMailItem)
                        mail.To = "apickles@amenclinic.com; jshafer@amenclinic.com"
                        mail.Subject = "Spect Watcher - Flagged"
                        mail.Body = "The Spect Transfer froze at: " & CStr(My.Settings.SpectWatcher)
                        mail.Send()
                    Catch ex As Exception
                        Dim h As Integer = 0
                        logError(ex.Message)
                    End Try
                End If
            End If
        Catch ex As Exception
            Dim h As Integer = 0
        End Try
    End Sub

    Private Sub TimerDirectoryScanner_Tick(sender As Object, e As EventArgs) Handles TimerDirectoryScanner.Tick
        If Startnow = 1 Then
            If IsProcessRunning("DirectoryScanner") = True Then
                If KillProcess("DirectoryScanner") = True Then
                    'yay it worked
                    Process.Start(My.Settings.DirectoryScannerLocation)
                Else
                    Dim k As Integer = 0
                    'boo it didnt work
                End If
            Else
                Try
                    Process.Start(My.Settings.DirectoryScannerLocation)
                Catch ex As Exception
                    Dim k As Integer = 0
                End Try
            End If
            Startnow = 0
        Else
            If Now.Hour >= 21 Then
                If IsProcessRunning("DirectoryScanner") = True Then
                    If KillProcess("DirectoryScanner") = True Then
                        'yay it worked
                        Process.Start(My.Settings.DirectoryScannerLocation)
                    Else
                        Dim k As Integer = 0
                        'boo it didnt work
                    End If
                Else
                    Try
                        Process.Start(My.Settings.DirectoryScannerLocation)
                    Catch ex As Exception
                        Dim k As Integer = 0
                    End Try
                End If
            Else
                'do nothing if its to early
            End If
        End If
    End Sub
    Public Function IsProcessRunning(name As String) As Boolean
        For Each clsProcess As Process In Process.GetProcesses
            If clsProcess.ProcessName.StartsWith(name) Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Function KillProcess(name As String) As Boolean
        For Each clsProcess As Process In Process.GetProcesses
            If clsProcess.ProcessName.StartsWith(name) Then
                clsProcess.Kill()
                Return True
            End If
        Next
        Return False
    End Function
    Dim dt As New DataTable
    Dim MySL As New SortedList
    Public Sub TraverseTreeParallelForEach(ByVal root As String, ByVal action As Action(Of String))
        'Count of files traversed and timer for diagnostic output 
        Dim fileCount As Integer = 0
        Dim sw As Stopwatch = Stopwatch.StartNew()

        ' Determine whether to parallelize file processing on each folder based on processor count. 
        Dim procCount As Integer = System.Environment.ProcessorCount

        ' Data structure to hold names of subfolders to be examined for files. 
        Dim dirs As New Stack(Of String)

        If Not Directory.Exists(root) Then Throw New ArgumentException()

        dirs.Push(root)

        While (dirs.Count > 0)
            Dim currentDir As String = dirs.Pop()
            Dim subDirs() As String = Nothing
            Dim files() As String = Nothing
            Try
                subDirs = Directory.GetDirectories(currentDir)
                Try
                    For Each StrDirectory As String In Directory.GetDirectories(currentDir, "*", SearchOption.TopDirectoryOnly)
                        dt.Rows.Add(StrDirectory)
                        MySL.Add(StrDirectory, 0)
                    Next
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
                'dt.Rows.Add(currentDir)
                ' Thrown if we do not have discovery permission on the directory. 
            Catch e As UnauthorizedAccessException
                Console.WriteLine(e.Message)
                Continue While
                ' Thrown if another process has deleted the directory after we retrieved its name. 
            Catch e As DirectoryNotFoundException
                Console.WriteLine(e.Message)
                Continue While
            End Try

            Try
                files = Directory.GetFiles(currentDir)
                Try
                    For Each strFile As String In Directory.GetFiles(currentDir, "*", SearchOption.TopDirectoryOnly)
                        dt.Rows.Add(strFile)
                        MySL.Add(strFile, 0)
                    Next
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
                'dt.Rows.Add(files)
            Catch e As UnauthorizedAccessException
                Console.WriteLine(e.Message)
                Continue While
            Catch e As DirectoryNotFoundException
                Console.WriteLine(e.Message)
                Continue While
            Catch e As IOException
                Console.WriteLine(e.Message)
                Continue While
            End Try

            ' Execute in parallel if there are enough files in the directory. 
            ' Otherwise, execute sequentially.Files are opened and processed 
            ' synchronously but this could be modified to perform async I/O. 
            Try
                If files.Length < procCount Then
                    For Each file In files
                        action(file)
                        fileCount += 1
                    Next
                Else
                    Parallel.ForEach(files, Function() 0, Function(file, loopState, localCount)
                                                              action(file)
                                                              localCount = localCount + 1
                                                              Return localCount
                                                          End Function,
                                     Sub(c)
                                         Interlocked.Add(fileCount, c)
                                     End Sub)
                End If
            Catch ae As AggregateException
                ae.Handle(Function(ex)

                              If TypeOf (ex) Is UnauthorizedAccessException Then

                                  ' Here we just output a message and go on.
                                  Console.WriteLine(ex.Message)
                                  Return True
                              End If
                              ' Handle other exceptions here if necessary... 

                              Return False
                          End Function)
            End Try
            ' Push the subdirectories onto the stack for traversal. 
            ' This could also be done before handing the files. 
            For Each str As String In subDirs
                dirs.Push(str)
            Next

            ' For diagnostic purposes.
            'Console.WriteLine("Processed {0}  files in {1}:{2}:{3}:{4}", fileCount, sw.Elapsed.Hours.ToString, sw.Elapsed.Minutes.ToString, sw.Elapsed.Seconds.ToString, sw.ElapsedMilliseconds)
            'dt.Rows.Add(currentDir)
        End While
    End Sub
End Class