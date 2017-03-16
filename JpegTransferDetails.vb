Public Class JpegTransferDetails

    Private Sub JpegTransferDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        JpegTransferListBox.Items.Clear()
        For Each item As String In My.Settings.JpegTransferStatusList
            JpegTransferListBox.Items.Add(item)
        Next
        JpegTransferListBox.TopIndex = JpegTransferListBox.Items.Count - 1
    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        If MsgBox("This will clear all items and cannot be undone. Proceed?", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            My.Settings.JpegTransferStatusList.Clear()
            JpegTransferListBox.Items.Clear()
        End If
    End Sub

    Private Sub JpegTransferListBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles JpegTransferListBox.SelectedIndexChanged

    End Sub
End Class