<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IFileTransferDetails
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.IFileTransferListBox = New System.Windows.Forms.ListBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ClearAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'IFileTransferListBox
        '
        Me.IFileTransferListBox.BackColor = System.Drawing.SystemColors.Control
        Me.IFileTransferListBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.IFileTransferListBox.ContextMenuStrip = Me.ContextMenuStrip1
        Me.IFileTransferListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.IFileTransferListBox.FormattingEnabled = True
        Me.IFileTransferListBox.HorizontalScrollbar = True
        Me.IFileTransferListBox.Location = New System.Drawing.Point(0, 0)
        Me.IFileTransferListBox.Name = "IFileTransferListBox"
        Me.IFileTransferListBox.Size = New System.Drawing.Size(347, 285)
        Me.IFileTransferListBox.TabIndex = 5
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ClearAllToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(153, 48)
        '
        'ClearAllToolStripMenuItem
        '
        Me.ClearAllToolStripMenuItem.Name = "ClearAllToolStripMenuItem"
        Me.ClearAllToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ClearAllToolStripMenuItem.Text = "Clear All"
        '
        'IFileTransferDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(347, 285)
        Me.Controls.Add(Me.IFileTransferListBox)
        Me.Name = "IFileTransferDetails"
        Me.Text = "IFileTransferDetails"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents IFileTransferListBox As System.Windows.Forms.ListBox
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ClearAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
