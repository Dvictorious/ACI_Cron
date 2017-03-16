Public Class OnlineInquiryDetails

    Private Sub OnlineInquiryDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        OnlineInquiryListBox.Items.Clear()
        For Each item As String In My.Settings.OnlineInquiryStatusList
            OnlineInquiryListBox.Items.Add(item)
        Next
        OnlineInquiryListBox.TopIndex = OnlineInquiryListBox.Items.Count - 1
    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        If MsgBox("This will clear all items and cannot be undone. Proceed?", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            My.Settings.OnlineInquiryStatusList.Clear()
            OnlineInquiryListBox.Items.Clear()
        End If
    End Sub
End Class