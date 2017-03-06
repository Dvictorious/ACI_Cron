Public Class ReadingsImportDetails

    Private Sub ReadingsImportDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ReadingsImportListBox.Items.Clear()
        For Each item As String In My.Settings.ReadingImportStatusList
            ReadingsImportListBox.Items.Add(item)
        Next
        ReadingsImportListBox.TopIndex = ReadingsImportListBox.Items.Count - 1
    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        If MsgBox("This will clear all items and cannot be undone. Proceed?", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            My.Settings.ReadingImportStatusList.Clear()
            ReadingsImportListBox.Items.Clear()
        End If
    End Sub
End Class