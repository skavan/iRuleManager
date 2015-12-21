Imports System.ComponentModel

Public Class frmProgressIndicator
    Private Sub buttonStart_Click(sender As Object, e As EventArgs) Handles buttonStart.Click
        Me.Close()
    End Sub

    Public Sub DisplayText(message As String)
        If Me.InvokeRequired Then
            Invoke(New MethodInvoker(Sub() DisplayText(message)))
        Else
            lblProgress.Text = message
            Me.Visible = True
        End If

        lblProgress.Text = message
    End Sub

    Public Sub KillMe()
        If Me.InvokeRequired Then
            Invoke(New MethodInvoker(Sub() KillMe()))
        Else
            Me.Close()
        End If
    End Sub

    Public Sub HideMe()
        If Me.InvokeRequired Then
            Invoke(New MethodInvoker(Sub() HideMe()))
        Else
            Me.Visible = False
        End If
    End Sub

    Private Sub frmProgressIndicator_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.Hide()
    End Sub
End Class