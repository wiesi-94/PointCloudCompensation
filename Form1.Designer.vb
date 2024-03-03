<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.BT_AddPTX = New System.Windows.Forms.Button()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.BT_Read = New System.Windows.Forms.Button()
        Me.PB_Files = New System.Windows.Forms.ProgressBar()
        Me.PB_File = New System.Windows.Forms.ProgressBar()
        Me.Files = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.BT_Save = New System.Windows.Forms.Button()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BT_AddPTX
        '
        Me.BT_AddPTX.Location = New System.Drawing.Point(12, 12)
        Me.BT_AddPTX.Name = "BT_AddPTX"
        Me.BT_AddPTX.Size = New System.Drawing.Size(162, 23)
        Me.BT_AddPTX.TabIndex = 1
        Me.BT_AddPTX.Text = "Add Files"
        Me.BT_AddPTX.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.HorizontalScrollbar = True
        Me.ListBox1.ItemHeight = 15
        Me.ListBox1.Location = New System.Drawing.Point(12, 41)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(162, 214)
        Me.ListBox1.TabIndex = 0
        '
        'BT_Read
        '
        Me.BT_Read.Location = New System.Drawing.Point(12, 354)
        Me.BT_Read.Name = "BT_Read"
        Me.BT_Read.Size = New System.Drawing.Size(162, 23)
        Me.BT_Read.TabIndex = 2
        Me.BT_Read.Text = "RUN"
        Me.BT_Read.UseVisualStyleBackColor = True
        '
        'PB_Files
        '
        Me.PB_Files.Location = New System.Drawing.Point(12, 281)
        Me.PB_Files.Name = "PB_Files"
        Me.PB_Files.Size = New System.Drawing.Size(162, 23)
        Me.PB_Files.TabIndex = 20
        '
        'PB_File
        '
        Me.PB_File.Location = New System.Drawing.Point(12, 325)
        Me.PB_File.Name = "PB_File"
        Me.PB_File.Size = New System.Drawing.Size(162, 23)
        Me.PB_File.TabIndex = 21
        '
        'Files
        '
        Me.Files.AutoSize = True
        Me.Files.Location = New System.Drawing.Point(12, 263)
        Me.Files.Name = "Files"
        Me.Files.Size = New System.Drawing.Size(30, 15)
        Me.Files.TabIndex = 22
        Me.Files.Text = "Files"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(12, 307)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(25, 15)
        Me.Label9.TabIndex = 23
        Me.Label9.Text = "File"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 500
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PropertyGrid1.Location = New System.Drawing.Point(180, 12)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.Size = New System.Drawing.Size(361, 336)
        Me.PropertyGrid1.TabIndex = 24
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Location = New System.Drawing.Point(181, 383)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(361, 103)
        Me.TextBox1.TabIndex = 25
        Me.TextBox1.Text = resources.GetString("TextBox1.Text")
        '
        'BT_Save
        '
        Me.BT_Save.Location = New System.Drawing.Point(180, 354)
        Me.BT_Save.Name = "BT_Save"
        Me.BT_Save.Size = New System.Drawing.Size(361, 23)
        Me.BT_Save.TabIndex = 26
        Me.BT_Save.Text = "Save to File"
        Me.BT_Save.UseVisualStyleBackColor = True
        '
        'TextBox2
        '
        Me.TextBox2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox2.Location = New System.Drawing.Point(12, 383)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(163, 103)
        Me.TextBox2.TabIndex = 27
        Me.TextBox2.Text = "For PTX files, the application assumes that the first half of the columns represe" &
    "nts the front view and the second half the back view."
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 494)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(553, 22)
        Me.StatusStrip1.TabIndex = 28
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(71, 17)
        Me.ToolStripStatusLabel1.Text = "© Stefan W."
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(553, 516)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.BT_Save)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.PropertyGrid1)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Files)
        Me.Controls.Add(Me.PB_File)
        Me.Controls.Add(Me.PB_Files)
        Me.Controls.Add(Me.BT_Read)
        Me.Controls.Add(Me.BT_AddPTX)
        Me.Controls.Add(Me.ListBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.Text = "Compensation"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BT_AddPTX As Button
    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents BT_Read As Button
    Friend WithEvents PB_Files As ProgressBar
    Friend WithEvents PB_File As ProgressBar
    Friend WithEvents Files As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Timer1 As Timer
    Friend WithEvents PropertyGrid1 As PropertyGrid
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents BT_Save As Button
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
End Class
