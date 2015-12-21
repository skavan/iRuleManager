<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmProgressIndicator
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
        Me.buttonCancel = New System.Windows.Forms.Button()
        Me.buttonStart = New System.Windows.Forms.Button()
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.pictureBox = New System.Windows.Forms.PictureBox()
        Me.panel1.SuspendLayout()
        CType(Me.pictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'buttonCancel
        '
        Me.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.buttonCancel.Enabled = False
        Me.buttonCancel.Location = New System.Drawing.Point(346, 9)
        Me.buttonCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.buttonCancel.Name = "buttonCancel"
        Me.buttonCancel.Size = New System.Drawing.Size(132, 40)
        Me.buttonCancel.TabIndex = 6
        Me.buttonCancel.Text = "&Cancel"
        Me.buttonCancel.UseVisualStyleBackColor = True
        '
        'buttonStart
        '
        Me.buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.buttonStart.Location = New System.Drawing.Point(206, 9)
        Me.buttonStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.buttonStart.Name = "buttonStart"
        Me.buttonStart.Size = New System.Drawing.Size(132, 40)
        Me.buttonStart.TabIndex = 5
        Me.buttonStart.Text = "&OK"
        Me.buttonStart.UseVisualStyleBackColor = True
        '
        'panel1
        '
        Me.panel1.BackColor = System.Drawing.SystemColors.Control
        Me.panel1.Controls.Add(Me.buttonCancel)
        Me.panel1.Controls.Add(Me.buttonStart)
        Me.panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panel1.Location = New System.Drawing.Point(0, 88)
        Me.panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(487, 58)
        Me.panel1.TabIndex = 8
        '
        'lblProgress
        '
        Me.lblProgress.AutoSize = True
        Me.lblProgress.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProgress.Location = New System.Drawing.Point(115, 35)
        Me.lblProgress.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(86, 32)
        Me.lblProgress.TabIndex = 9
        Me.lblProgress.Text = "Ready!"
        '
        'pictureBox
        '
        Me.pictureBox.Image = Global.iRuleManager.My.Resources.Resources.image_930721
        Me.pictureBox.Location = New System.Drawing.Point(16, 8)
        Me.pictureBox.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pictureBox.Name = "pictureBox"
        Me.pictureBox.Size = New System.Drawing.Size(72, 74)
        Me.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pictureBox.TabIndex = 7
        Me.pictureBox.TabStop = False
        '
        'frmProgressIndicator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ClientSize = New System.Drawing.Size(487, 146)
        Me.ControlBox = False
        Me.Controls.Add(Me.pictureBox)
        Me.Controls.Add(Me.panel1)
        Me.Controls.Add(Me.lblProgress)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "frmProgressIndicator"
        Me.ShowIcon = False
        Me.Text = "iRuleManager Operation In Progress"
        Me.panel1.ResumeLayout(False)
        CType(Me.pictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents pictureBox As PictureBox
    Private WithEvents buttonCancel As Button
    Private WithEvents buttonStart As Button
    Private WithEvents panel1 As Panel
    Private WithEvents lblProgress As Label
End Class
