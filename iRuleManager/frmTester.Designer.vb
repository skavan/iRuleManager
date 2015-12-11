<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTester
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
        Me.btnInit = New System.Windows.Forms.Button()
        Me.btnScan = New System.Windows.Forms.Button()
        Me.tv1 = New System.Windows.Forms.TreeView()
        Me.dg1 = New System.Windows.Forms.DataGridView()
        Me.pg1 = New System.Windows.Forms.PropertyGrid()
        Me.btnWriteWidgets = New System.Windows.Forms.Button()
        Me.btnWidgets = New System.Windows.Forms.Button()
        Me.tv2 = New System.Windows.Forms.TreeView()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.tv3 = New System.Windows.Forms.TreeView()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnReadWidgets = New System.Windows.Forms.Button()
        CType(Me.dg1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnInit
        '
        Me.btnInit.Location = New System.Drawing.Point(626, 12)
        Me.btnInit.Name = "btnInit"
        Me.btnInit.Size = New System.Drawing.Size(141, 136)
        Me.btnInit.TabIndex = 0
        Me.btnInit.Text = "Init"
        Me.btnInit.UseVisualStyleBackColor = True
        '
        'btnScan
        '
        Me.btnScan.Enabled = False
        Me.btnScan.Location = New System.Drawing.Point(773, 12)
        Me.btnScan.Name = "btnScan"
        Me.btnScan.Size = New System.Drawing.Size(147, 65)
        Me.btnScan.TabIndex = 1
        Me.btnScan.Text = "ScanSystem"
        Me.btnScan.UseVisualStyleBackColor = True
        '
        'tv1
        '
        Me.tv1.Location = New System.Drawing.Point(12, 12)
        Me.tv1.Name = "tv1"
        Me.tv1.Size = New System.Drawing.Size(596, 509)
        Me.tv1.TabIndex = 2
        '
        'dg1
        '
        Me.dg1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dg1.Location = New System.Drawing.Point(6, 370)
        Me.dg1.Name = "dg1"
        Me.dg1.RowHeadersVisible = False
        Me.dg1.RowTemplate.Height = 28
        Me.dg1.Size = New System.Drawing.Size(593, 445)
        Me.dg1.TabIndex = 3
        '
        'pg1
        '
        Me.pg1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.pg1.HelpVisible = False
        Me.pg1.Location = New System.Drawing.Point(6, 6)
        Me.pg1.Name = "pg1"
        Me.pg1.Size = New System.Drawing.Size(593, 361)
        Me.pg1.TabIndex = 4
        Me.pg1.ToolbarVisible = False
        '
        'btnWriteWidgets
        '
        Me.btnWriteWidgets.Location = New System.Drawing.Point(926, 12)
        Me.btnWriteWidgets.Name = "btnWriteWidgets"
        Me.btnWriteWidgets.Size = New System.Drawing.Size(147, 65)
        Me.btnWriteWidgets.TabIndex = 5
        Me.btnWriteWidgets.Text = "Write Widgets"
        Me.btnWriteWidgets.UseVisualStyleBackColor = True
        '
        'btnWidgets
        '
        Me.btnWidgets.Location = New System.Drawing.Point(773, 83)
        Me.btnWidgets.Name = "btnWidgets"
        Me.btnWidgets.Size = New System.Drawing.Size(147, 65)
        Me.btnWidgets.TabIndex = 6
        Me.btnWidgets.Text = "Scan Widgets"
        Me.btnWidgets.UseVisualStyleBackColor = True
        '
        'tv2
        '
        Me.tv2.Location = New System.Drawing.Point(12, 527)
        Me.tv2.Name = "tv2"
        Me.tv2.Size = New System.Drawing.Size(596, 497)
        Me.tv2.TabIndex = 7
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(622, 178)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(604, 845)
        Me.TabControl1.TabIndex = 8
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.tv3)
        Me.TabPage1.Location = New System.Drawing.Point(4, 29)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(596, 812)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'tv3
        '
        Me.tv3.Location = New System.Drawing.Point(8, 6)
        Me.tv3.Name = "tv3"
        Me.tv3.Size = New System.Drawing.Size(586, 809)
        Me.tv3.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.pg1)
        Me.TabPage2.Controls.Add(Me.dg1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 29)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(596, 812)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(1079, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(147, 65)
        Me.Button1.TabIndex = 9
        Me.Button1.Text = "Load page"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnReadWidgets
        '
        Me.btnReadWidgets.Location = New System.Drawing.Point(926, 83)
        Me.btnReadWidgets.Name = "btnReadWidgets"
        Me.btnReadWidgets.Size = New System.Drawing.Size(147, 65)
        Me.btnReadWidgets.TabIndex = 10
        Me.btnReadWidgets.Text = "Read Widgets"
        Me.btnReadWidgets.UseVisualStyleBackColor = True
        '
        'frmTester
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1805, 1036)
        Me.Controls.Add(Me.btnReadWidgets)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.tv2)
        Me.Controls.Add(Me.btnWidgets)
        Me.Controls.Add(Me.btnWriteWidgets)
        Me.Controls.Add(Me.tv1)
        Me.Controls.Add(Me.btnScan)
        Me.Controls.Add(Me.btnInit)
        Me.Name = "frmTester"
        Me.Text = "frmTester"
        CType(Me.dg1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnInit As Button
    Friend WithEvents btnScan As Button
    Friend WithEvents tv1 As TreeView
    Friend WithEvents dg1 As DataGridView
    Friend WithEvents pg1 As PropertyGrid
    Friend WithEvents btnWriteWidgets As Button
    Friend WithEvents btnWidgets As Button
    Friend WithEvents tv2 As TreeView
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents tv3 As TreeView
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents Button1 As Button
    Friend WithEvents btnReadWidgets As Button
End Class
