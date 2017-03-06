<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Start
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
        Dim Label1 As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label5 As System.Windows.Forms.Label
        Me.JpegTransferBackgroundWorker = New System.ComponentModel.BackgroundWorker()
        Me.JpegTransferTimer = New System.Windows.Forms.Timer(Me.components)
        Me.JpegTransferProgressBar = New System.Windows.Forms.ProgressBar()
        Me.JpegTransferDetailsLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.SpectDataDetailsLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.SpectTransferProgressBar = New System.Windows.Forms.ProgressBar()
        Me.SpectTransferBackgroundWorker = New System.ComponentModel.BackgroundWorker()
        Me.SpectTransferTimer = New System.Windows.Forms.Timer(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.InterfileTransferTimer = New System.Windows.Forms.Timer(Me.components)
        Me.IfileTransferDetailsLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.IFileTransferProgressBar = New System.Windows.Forms.ProgressBar()
        Me.IFileTransferBackgroundWorker = New System.ComponentModel.BackgroundWorker()
        Me.OnlineInquiryDetailsLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.OnlineInquiryProgressBar = New System.Windows.Forms.ProgressBar()
        Me.OnlineInquiryBackgroundWorker = New System.ComponentModel.BackgroundWorker()
        Me.OnlineInquiryTimer = New System.Windows.Forms.Timer(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.EnableTimersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JpegTransferToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpectTransferToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.IFileTransferToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OnlineInquiryToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReadingsImportToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ManualStartToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JpegTransferToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpectTransferToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IFileTransferToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OnlineInquiryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReadingsImportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReadingsImportLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.ReadingsImportProgressBar = New System.Windows.Forms.ProgressBar()
        Me.ReadingsImportBackgroundWorker = New System.ComponentModel.BackgroundWorker()
        Me.ReadingsImportTimer = New System.Windows.Forms.Timer(Me.components)
        Me.VersionLabel = New System.Windows.Forms.Label()
        Me.TimerJpegWatcher = New System.Windows.Forms.Timer(Me.components)
        Me.TimerSpectWatcher = New System.Windows.Forms.Timer(Me.components)
        Me.TimerDirectoryScanner = New System.Windows.Forms.Timer(Me.components)
        Label1 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        Label3 = New System.Windows.Forms.Label()
        Label4 = New System.Windows.Forms.Label()
        Label5 = New System.Windows.Forms.Label()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(67, 46)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(72, 13)
        Label1.TabIndex = 0
        Label1.Text = "Jpeg Transfer"
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(62, 81)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(77, 13)
        Label2.TabIndex = 3
        Label2.Text = "Spect Transfer"
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(67, 118)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(68, 13)
        Label3.TabIndex = 6
        Label3.Text = "IFile Transfer"
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(62, 153)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(71, 13)
        Label4.TabIndex = 9
        Label4.Text = "Online Inquiry"
        '
        'Label5
        '
        Label5.AutoSize = True
        Label5.Location = New System.Drawing.Point(51, 191)
        Label5.Name = "Label5"
        Label5.Size = New System.Drawing.Size(84, 13)
        Label5.TabIndex = 12
        Label5.Text = "Readings Import"
        '
        'JpegTransferBackgroundWorker
        '
        Me.JpegTransferBackgroundWorker.WorkerReportsProgress = True
        '
        'JpegTransferTimer
        '
        Me.JpegTransferTimer.Interval = 600000
        '
        'JpegTransferProgressBar
        '
        Me.JpegTransferProgressBar.Location = New System.Drawing.Point(145, 46)
        Me.JpegTransferProgressBar.Name = "JpegTransferProgressBar"
        Me.JpegTransferProgressBar.Size = New System.Drawing.Size(100, 13)
        Me.JpegTransferProgressBar.TabIndex = 1
        '
        'JpegTransferDetailsLinkLabel
        '
        Me.JpegTransferDetailsLinkLabel.AutoSize = True
        Me.JpegTransferDetailsLinkLabel.Location = New System.Drawing.Point(251, 46)
        Me.JpegTransferDetailsLinkLabel.Name = "JpegTransferDetailsLinkLabel"
        Me.JpegTransferDetailsLinkLabel.Size = New System.Drawing.Size(39, 13)
        Me.JpegTransferDetailsLinkLabel.TabIndex = 2
        Me.JpegTransferDetailsLinkLabel.TabStop = True
        Me.JpegTransferDetailsLinkLabel.Text = "Details"
        '
        'SpectDataDetailsLinkLabel
        '
        Me.SpectDataDetailsLinkLabel.AutoSize = True
        Me.SpectDataDetailsLinkLabel.Location = New System.Drawing.Point(251, 81)
        Me.SpectDataDetailsLinkLabel.Name = "SpectDataDetailsLinkLabel"
        Me.SpectDataDetailsLinkLabel.Size = New System.Drawing.Size(39, 13)
        Me.SpectDataDetailsLinkLabel.TabIndex = 5
        Me.SpectDataDetailsLinkLabel.TabStop = True
        Me.SpectDataDetailsLinkLabel.Text = "Details"
        '
        'SpectTransferProgressBar
        '
        Me.SpectTransferProgressBar.Location = New System.Drawing.Point(145, 81)
        Me.SpectTransferProgressBar.Name = "SpectTransferProgressBar"
        Me.SpectTransferProgressBar.Size = New System.Drawing.Size(100, 13)
        Me.SpectTransferProgressBar.TabIndex = 4
        '
        'SpectTransferBackgroundWorker
        '
        Me.SpectTransferBackgroundWorker.WorkerReportsProgress = True
        Me.SpectTransferBackgroundWorker.WorkerSupportsCancellation = True
        '
        'SpectTransferTimer
        '
        Me.SpectTransferTimer.Interval = 7200000
        '
        'InterfileTransferTimer
        '
        Me.InterfileTransferTimer.Interval = 7200000
        '
        'IfileTransferDetailsLinkLabel
        '
        Me.IfileTransferDetailsLinkLabel.AutoSize = True
        Me.IfileTransferDetailsLinkLabel.Location = New System.Drawing.Point(251, 118)
        Me.IfileTransferDetailsLinkLabel.Name = "IfileTransferDetailsLinkLabel"
        Me.IfileTransferDetailsLinkLabel.Size = New System.Drawing.Size(39, 13)
        Me.IfileTransferDetailsLinkLabel.TabIndex = 8
        Me.IfileTransferDetailsLinkLabel.TabStop = True
        Me.IfileTransferDetailsLinkLabel.Text = "Details"
        '
        'IFileTransferProgressBar
        '
        Me.IFileTransferProgressBar.Location = New System.Drawing.Point(145, 118)
        Me.IFileTransferProgressBar.Name = "IFileTransferProgressBar"
        Me.IFileTransferProgressBar.Size = New System.Drawing.Size(100, 13)
        Me.IFileTransferProgressBar.TabIndex = 7
        '
        'IFileTransferBackgroundWorker
        '
        Me.IFileTransferBackgroundWorker.WorkerReportsProgress = True
        '
        'OnlineInquiryDetailsLinkLabel
        '
        Me.OnlineInquiryDetailsLinkLabel.AutoSize = True
        Me.OnlineInquiryDetailsLinkLabel.Location = New System.Drawing.Point(251, 153)
        Me.OnlineInquiryDetailsLinkLabel.Name = "OnlineInquiryDetailsLinkLabel"
        Me.OnlineInquiryDetailsLinkLabel.Size = New System.Drawing.Size(39, 13)
        Me.OnlineInquiryDetailsLinkLabel.TabIndex = 11
        Me.OnlineInquiryDetailsLinkLabel.TabStop = True
        Me.OnlineInquiryDetailsLinkLabel.Text = "Details"
        '
        'OnlineInquiryProgressBar
        '
        Me.OnlineInquiryProgressBar.Location = New System.Drawing.Point(145, 153)
        Me.OnlineInquiryProgressBar.Name = "OnlineInquiryProgressBar"
        Me.OnlineInquiryProgressBar.Size = New System.Drawing.Size(100, 13)
        Me.OnlineInquiryProgressBar.TabIndex = 10
        '
        'OnlineInquiryBackgroundWorker
        '
        Me.OnlineInquiryBackgroundWorker.WorkerReportsProgress = True
        '
        'OnlineInquiryTimerS
        '
        Me.OnlineInquiryTimer.Interval = 3600000
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EnableTimersToolStripMenuItem, Me.ManualStartToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(153, 70)
        '
        'EnableTimersToolStripMenuItem
        '
        Me.EnableTimersToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.JpegTransferToolStripMenuItem1, Me.SpectTransferToolStripMenuItem1, Me.IFileTransferToolStripMenuItem1, Me.OnlineInquiryToolStripMenuItem1, Me.ReadingsImportToolStripMenuItem1})
        Me.EnableTimersToolStripMenuItem.Name = "EnableTimersToolStripMenuItem"
        Me.EnableTimersToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.EnableTimersToolStripMenuItem.Text = "Enable Timers"
        '
        'JpegTransferToolStripMenuItem1
        '
        Me.JpegTransferToolStripMenuItem1.CheckOnClick = True
        Me.JpegTransferToolStripMenuItem1.Name = "JpegTransferToolStripMenuItem1"
        Me.JpegTransferToolStripMenuItem1.Size = New System.Drawing.Size(161, 22)
        Me.JpegTransferToolStripMenuItem1.Text = "Jpeg Transfer"
        '
        'SpectTransferToolStripMenuItem1
        '
        Me.SpectTransferToolStripMenuItem1.CheckOnClick = True
        Me.SpectTransferToolStripMenuItem1.Name = "SpectTransferToolStripMenuItem1"
        Me.SpectTransferToolStripMenuItem1.Size = New System.Drawing.Size(161, 22)
        Me.SpectTransferToolStripMenuItem1.Text = "Spect Transfer"
        '
        'IFileTransferToolStripMenuItem1
        '
        Me.IFileTransferToolStripMenuItem1.CheckOnClick = True
        Me.IFileTransferToolStripMenuItem1.Name = "IFileTransferToolStripMenuItem1"
        Me.IFileTransferToolStripMenuItem1.Size = New System.Drawing.Size(161, 22)
        Me.IFileTransferToolStripMenuItem1.Text = "IFile Transfer"
        '
        'OnlineInquiryToolStripMenuItem1
        '
        Me.OnlineInquiryToolStripMenuItem1.CheckOnClick = True
        Me.OnlineInquiryToolStripMenuItem1.Name = "OnlineInquiryToolStripMenuItem1"
        Me.OnlineInquiryToolStripMenuItem1.Size = New System.Drawing.Size(161, 22)
        Me.OnlineInquiryToolStripMenuItem1.Text = "Online Inquiry"
        '
        'ReadingsImportToolStripMenuItem1
        '
        Me.ReadingsImportToolStripMenuItem1.CheckOnClick = True
        Me.ReadingsImportToolStripMenuItem1.Name = "ReadingsImportToolStripMenuItem1"
        Me.ReadingsImportToolStripMenuItem1.Size = New System.Drawing.Size(161, 22)
        Me.ReadingsImportToolStripMenuItem1.Text = "Readings Import"
        '
        'ManualStartToolStripMenuItem
        '
        Me.ManualStartToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.JpegTransferToolStripMenuItem, Me.SpectTransferToolStripMenuItem, Me.IFileTransferToolStripMenuItem, Me.OnlineInquiryToolStripMenuItem, Me.ReadingsImportToolStripMenuItem})
        Me.ManualStartToolStripMenuItem.Name = "ManualStartToolStripMenuItem"
        Me.ManualStartToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ManualStartToolStripMenuItem.Text = "Manual Start"
        '
        'JpegTransferToolStripMenuItem
        '
        Me.JpegTransferToolStripMenuItem.Name = "JpegTransferToolStripMenuItem"
        Me.JpegTransferToolStripMenuItem.Size = New System.Drawing.Size(161, 22)
        Me.JpegTransferToolStripMenuItem.Text = "Jpeg Transfer"
        '
        'SpectTransferToolStripMenuItem
        '
        Me.SpectTransferToolStripMenuItem.Name = "SpectTransferToolStripMenuItem"
        Me.SpectTransferToolStripMenuItem.Size = New System.Drawing.Size(161, 22)
        Me.SpectTransferToolStripMenuItem.Text = "Spect Transfer"
        '
        'IFileTransferToolStripMenuItem
        '
        Me.IFileTransferToolStripMenuItem.Name = "IFileTransferToolStripMenuItem"
        Me.IFileTransferToolStripMenuItem.Size = New System.Drawing.Size(161, 22)
        Me.IFileTransferToolStripMenuItem.Text = "IFile Transfer"
        '
        'OnlineInquiryToolStripMenuItem
        '
        Me.OnlineInquiryToolStripMenuItem.Name = "OnlineInquiryToolStripMenuItem"
        Me.OnlineInquiryToolStripMenuItem.Size = New System.Drawing.Size(161, 22)
        Me.OnlineInquiryToolStripMenuItem.Text = "Online Inquiry"
        '
        'ReadingsImportToolStripMenuItem
        '
        Me.ReadingsImportToolStripMenuItem.Name = "ReadingsImportToolStripMenuItem"
        Me.ReadingsImportToolStripMenuItem.Size = New System.Drawing.Size(161, 22)
        Me.ReadingsImportToolStripMenuItem.Text = "Readings Import"
        '
        'ReadingsImportLinkLabel
        '
        Me.ReadingsImportLinkLabel.AutoSize = True
        Me.ReadingsImportLinkLabel.Location = New System.Drawing.Point(251, 191)
        Me.ReadingsImportLinkLabel.Name = "ReadingsImportLinkLabel"
        Me.ReadingsImportLinkLabel.Size = New System.Drawing.Size(39, 13)
        Me.ReadingsImportLinkLabel.TabIndex = 14
        Me.ReadingsImportLinkLabel.TabStop = True
        Me.ReadingsImportLinkLabel.Text = "Details"
        '
        'ReadingsImportProgressBar
        '
        Me.ReadingsImportProgressBar.Location = New System.Drawing.Point(145, 191)
        Me.ReadingsImportProgressBar.Name = "ReadingsImportProgressBar"
        Me.ReadingsImportProgressBar.Size = New System.Drawing.Size(100, 13)
        Me.ReadingsImportProgressBar.TabIndex = 13
        '
        'ReadingsImportBackgroundWorker
        '
        Me.ReadingsImportBackgroundWorker.WorkerReportsProgress = True
        '
        'ReadingsImportTimer
        '
        Me.ReadingsImportTimer.Interval = 7200000
        '
        'VersionLabel
        '
        Me.VersionLabel.Location = New System.Drawing.Point(182, 231)
        Me.VersionLabel.Name = "VersionLabel"
        Me.VersionLabel.Size = New System.Drawing.Size(168, 17)
        Me.VersionLabel.TabIndex = 15
        Me.VersionLabel.Text = "[Version]"
        Me.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TimerJpegWatcher
        '
        Me.TimerJpegWatcher.Interval = 1200000
        '
        'TimerSpectWatcher
        '
        Me.TimerSpectWatcher.Interval = 1200000
        '
        'TimerDirectoryScanner
        '
        Me.TimerDirectoryScanner.Interval = 21600000
        '
        'Start
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(362, 257)
        Me.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Controls.Add(Me.VersionLabel)
        Me.Controls.Add(Me.ReadingsImportLinkLabel)
        Me.Controls.Add(Me.ReadingsImportProgressBar)
        Me.Controls.Add(Label5)
        Me.Controls.Add(Me.OnlineInquiryDetailsLinkLabel)
        Me.Controls.Add(Me.OnlineInquiryProgressBar)
        Me.Controls.Add(Label4)
        Me.Controls.Add(Me.IfileTransferDetailsLinkLabel)
        Me.Controls.Add(Me.IFileTransferProgressBar)
        Me.Controls.Add(Label3)
        Me.Controls.Add(Me.SpectDataDetailsLinkLabel)
        Me.Controls.Add(Me.SpectTransferProgressBar)
        Me.Controls.Add(Label2)
        Me.Controls.Add(Me.JpegTransferDetailsLinkLabel)
        Me.Controls.Add(Me.JpegTransferProgressBar)
        Me.Controls.Add(Label1)
        Me.Name = "Start"
        Me.Text = "Start"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents JpegTransferBackgroundWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents JpegTransferTimer As System.Windows.Forms.Timer
    Friend WithEvents JpegTransferProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents JpegTransferDetailsLinkLabel As System.Windows.Forms.LinkLabel
    Friend WithEvents SpectDataDetailsLinkLabel As System.Windows.Forms.LinkLabel
    Friend WithEvents SpectTransferProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents SpectTransferBackgroundWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents SpectTransferTimer As System.Windows.Forms.Timer
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents InterfileTransferTimer As System.Windows.Forms.Timer
    Friend WithEvents IfileTransferDetailsLinkLabel As System.Windows.Forms.LinkLabel
    Friend WithEvents IFileTransferProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents IFileTransferBackgroundWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents OnlineInquiryDetailsLinkLabel As System.Windows.Forms.LinkLabel
    Friend WithEvents OnlineInquiryProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents OnlineInquiryBackgroundWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents OnlineInquiryTimer As System.Windows.Forms.Timer
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ManualStartToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents JpegTransferToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SpectTransferToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IFileTransferToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OnlineInquiryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReadingsImportLinkLabel As System.Windows.Forms.LinkLabel
    Friend WithEvents ReadingsImportProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents ReadingsImportBackgroundWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents ReadingsImportTimer As System.Windows.Forms.Timer
    Friend WithEvents ReadingsImportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EnableTimersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents JpegTransferToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SpectTransferToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IFileTransferToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OnlineInquiryToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReadingsImportToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents VersionLabel As System.Windows.Forms.Label
    Friend WithEvents TimerJpegWatcher As System.Windows.Forms.Timer
    Friend WithEvents TimerSpectWatcher As System.Windows.Forms.Timer
    Friend WithEvents TimerDirectoryScanner As System.Windows.Forms.Timer
End Class
