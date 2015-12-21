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
        Me.tvTree = New System.Windows.Forms.TreeView()
        Me.dg1 = New System.Windows.Forms.DataGridView()
        Me.pg1 = New System.Windows.Forms.PropertyGrid()
        Me.btnWriteWidgets = New System.Windows.Forms.Button()
        Me.btnWidgets = New System.Windows.Forms.Button()
        Me.tv2 = New System.Windows.Forms.TreeView()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.tv3 = New System.Windows.Forms.TreeView()
        Me.btnLoadPage = New System.Windows.Forms.Button()
        Me.btnReadWidgets = New System.Windows.Forms.Button()
        Me.tbDeviceTree = New System.Windows.Forms.TabControl()
        Me.tbPage1 = New System.Windows.Forms.TabPage()
        Me.tbPage2 = New System.Windows.Forms.TabPage()
        Me.tvPage = New System.Windows.Forms.TreeView()
        Me.btnWrite = New System.Windows.Forms.Button()
        Me.btnTest = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        CType(Me.dg1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.tbDeviceTree.SuspendLayout()
        Me.tbPage1.SuspendLayout()
        Me.tbPage2.SuspendLayout()
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
        'tvTree
        '
        Me.tvTree.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvTree.Location = New System.Drawing.Point(3, 3)
        Me.tvTree.Name = "tvTree"
        Me.tvTree.Size = New System.Drawing.Size(580, 552)
        Me.tvTree.TabIndex = 2
        '
        'dg1
        '
        Me.dg1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dg1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.dg1.Location = New System.Drawing.Point(3, 482)
        Me.dg1.Name = "dg1"
        Me.dg1.RowHeadersVisible = False
        Me.dg1.RowTemplate.Height = 28
        Me.dg1.Size = New System.Drawing.Size(590, 245)
        Me.dg1.TabIndex = 3
        '
        'pg1
        '
        Me.pg1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.pg1.Dock = System.Windows.Forms.DockStyle.Top
        Me.pg1.HelpVisible = False
        Me.pg1.Location = New System.Drawing.Point(3, 3)
        Me.pg1.Name = "pg1"
        Me.pg1.Size = New System.Drawing.Size(590, 473)
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
        Me.tv2.Location = New System.Drawing.Point(12, 609)
        Me.tv2.Name = "tv2"
        Me.tv2.Size = New System.Drawing.Size(596, 415)
        Me.tv2.TabIndex = 7
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Location = New System.Drawing.Point(622, 260)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(604, 763)
        Me.TabControl1.TabIndex = 8
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.pg1)
        Me.TabPage2.Controls.Add(Me.dg1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 29)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(596, 730)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Properties"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.tv3)
        Me.TabPage1.Location = New System.Drawing.Point(4, 29)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(596, 730)
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
        'btnLoadPage
        '
        Me.btnLoadPage.Location = New System.Drawing.Point(1079, 12)
        Me.btnLoadPage.Name = "btnLoadPage"
        Me.btnLoadPage.Size = New System.Drawing.Size(147, 65)
        Me.btnLoadPage.TabIndex = 9
        Me.btnLoadPage.Text = "Load page"
        Me.btnLoadPage.UseVisualStyleBackColor = True
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
        'tbDeviceTree
        '
        Me.tbDeviceTree.Controls.Add(Me.tbPage1)
        Me.tbDeviceTree.Controls.Add(Me.tbPage2)
        Me.tbDeviceTree.Location = New System.Drawing.Point(12, 12)
        Me.tbDeviceTree.Name = "tbDeviceTree"
        Me.tbDeviceTree.SelectedIndex = 0
        Me.tbDeviceTree.Size = New System.Drawing.Size(594, 591)
        Me.tbDeviceTree.TabIndex = 11
        '
        'tbPage1
        '
        Me.tbPage1.Controls.Add(Me.tvTree)
        Me.tbPage1.Location = New System.Drawing.Point(4, 29)
        Me.tbPage1.Name = "tbPage1"
        Me.tbPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.tbPage1.Size = New System.Drawing.Size(586, 558)
        Me.tbPage1.TabIndex = 0
        Me.tbPage1.Text = "Device Tree"
        Me.tbPage1.UseVisualStyleBackColor = True
        '
        'tbPage2
        '
        Me.tbPage2.Controls.Add(Me.tvPage)
        Me.tbPage2.Location = New System.Drawing.Point(4, 29)
        Me.tbPage2.Name = "tbPage2"
        Me.tbPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.tbPage2.Size = New System.Drawing.Size(586, 558)
        Me.tbPage2.TabIndex = 1
        Me.tbPage2.Text = "Selected Page"
        Me.tbPage2.UseVisualStyleBackColor = True
        '
        'tvPage
        '
        Me.tvPage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvPage.Location = New System.Drawing.Point(3, 3)
        Me.tvPage.Name = "tvPage"
        Me.tvPage.Size = New System.Drawing.Size(580, 552)
        Me.tvPage.TabIndex = 3
        '
        'btnWrite
        '
        Me.btnWrite.Location = New System.Drawing.Point(1079, 83)
        Me.btnWrite.Name = "btnWrite"
        Me.btnWrite.Size = New System.Drawing.Size(147, 65)
        Me.btnWrite.TabIndex = 12
        Me.btnWrite.Text = "TestWrite"
        Me.btnWrite.UseVisualStyleBackColor = True
        '
        'btnTest
        '
        Me.btnTest.Location = New System.Drawing.Point(773, 154)
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(147, 65)
        Me.btnTest.TabIndex = 13
        Me.btnTest.Text = "test"
        Me.btnTest.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(926, 154)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(147, 65)
        Me.Button1.TabIndex = 14
        Me.Button1.Text = "test"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'frmTester
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1237, 1036)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnTest)
        Me.Controls.Add(Me.btnWrite)
        Me.Controls.Add(Me.tbDeviceTree)
        Me.Controls.Add(Me.btnReadWidgets)
        Me.Controls.Add(Me.btnLoadPage)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.tv2)
        Me.Controls.Add(Me.btnWidgets)
        Me.Controls.Add(Me.btnWriteWidgets)
        Me.Controls.Add(Me.btnScan)
        Me.Controls.Add(Me.btnInit)
        Me.Name = "frmTester"
        Me.Text = "frmTester"
        CType(Me.dg1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.tbDeviceTree.ResumeLayout(False)
        Me.tbPage1.ResumeLayout(False)
        Me.tbPage2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnInit As Button
    Friend WithEvents btnScan As Button
    Friend WithEvents tvTree As TreeView
    Friend WithEvents dg1 As DataGridView
    Friend WithEvents pg1 As PropertyGrid
    Friend WithEvents btnWriteWidgets As Button
    Friend WithEvents btnWidgets As Button
    Friend WithEvents tv2 As TreeView
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents tv3 As TreeView
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents btnLoadPage As Button
    Friend WithEvents btnReadWidgets As Button
    Friend WithEvents tbDeviceTree As TabControl
    Friend WithEvents tbPage1 As TabPage
    Friend WithEvents tbPage2 As TabPage
    Friend WithEvents tvPage As TreeView
    Friend WithEvents btnWrite As Button
    Friend WithEvents btnTest As Button
    Friend WithEvents Button1 As Button
End Class
