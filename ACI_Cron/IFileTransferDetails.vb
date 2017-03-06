Public Class IFileTransferDetails

    Private Sub IFileTransferDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IFileTransferListBox.Items.Clear()
        For Each item As String In My.Settings.IFileTransferStatusList
            IFileTransferListBox.Items.Add(item)
        Next
        IFileTransferListBox.TopIndex = IFileTransferListBox.Items.Count - 1
    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        If MsgBox("This will clear all items and cannot be undone. Proceed?", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            My.Settings.IFileTransferStatusList.Clear()
            IFileTransferListBox.Items.Clear()
        End If
    End Sub

End Class