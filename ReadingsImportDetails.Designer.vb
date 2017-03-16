<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ReadingsImportDetails
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
        Me.ReadingsImportListBox = New System.Windows.Forms.ListBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ClearAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ReadingsImportListBox
        '
        Me.ReadingsImportListBox.BackColor = System.Drawing.SystemColors.Control
        Me.ReadingsImportListBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ReadingsImportListBox.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ReadingsImportListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReadingsImportListBox.FormattingEnabled = True
        Me.ReadingsImportListBox.HorizontalScrollbar = True
        Me.ReadingsImportListBox.Location = New System.Drawing.Point(0, 0)
        Me.ReadingsImportListBox.Name = "ReadingsImportListBox"
        Me.ReadingsImportListBox.Size = New System.Drawing.Size(347, 285)
        Me.ReadingsImportListBox.TabIndex = 10
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
        Me.ClearAllToolStripMenuItem.Text = "Clear all"
        '
        'ReadingsImportDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(347, 285)
        Me.Controls.Add(Me.ReadingsImportListBox)
        Me.Name = "ReadingsImportDetails"
        Me.Text = "ReadingsImportDetails"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ReadingsImportListBox As System.Windows.Forms.ListBox
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ClearAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
