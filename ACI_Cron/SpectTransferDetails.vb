Public Class SpectTransferDetails

    Private Sub SpectTransferDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SpectTransferListBox.Items.Clear()
        For Each item As String In My.Settings.SpectTransferStatusList
            SpectTransferListBox.Items.Add(item)
        Next
        SpectTransferListBox.TopIndex = SpectTransferListBox.Items.Count - 1
    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        If MsgBox("This will clear all items and cannot be undone. Proceed?", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            My.Settings.SpectTransferStatusList.Clear()
            SpectTransferListBox.Items.Clear()
        End If
    End Sub
End Class