Imports System.ComponentModel
Imports iRuleManager.Manager

Public Class frmTester

    Dim iRule As Scraper
    Dim ST As New Stopwatch

    Private Sub ShowProgress(message As String, bKill As Boolean)
        If bKill Then
            frmProgress.Enabled = False
            frmProgress.Visible = False
            Me.BringToFront()
        Else
            frmProgress.Visible = True
            frmProgress.Enabled = True
            frmProgress.DisplayText(message)
            frmProgress.BringToFront()
        End If
    End Sub
    Private Sub btnInit_Click(sender As Object, e As EventArgs) Handles btnInit.Click

        ShowProgress("Initializing System...", False)
        iRule = New Scraper("http://builder.iruleathome.com/iRule.html#main")
        btnInit.Enabled = False
        btnScan.Enabled = True


        ShowProgress("Initializing System...", True)
    End Sub

    Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
        ST.Start()
        ShowProgress("Scanning Device Tree...", False)
        iRule.ScanDeviceTree()

        If iRule.NodeTree IsNot Nothing Then
            tv1.Nodes.Clear()
            tv1.ImageList = iRule.imageList
            tv1.Nodes.Add(iRule.NodeTree)
            tv1.ExpandAll()
        End If
        ShowProgress("Scanning Device Tree...", True)
    End Sub

    Private Sub btnWidgets_Click(sender As Object, e As EventArgs) Handles btnWidgets.Click
        ShowProgress("Scanning Widgets Tree...", False)
        iRule.ScanWidgetTree()
        If iRule.WidgetTree IsNot Nothing Then
            tv2.Nodes.Clear()
            tv2.ImageList = iRule.imageList
            tv2.Nodes.Add(iRule.WidgetTree)
            tv2.ExpandAll()
        End If
        ShowProgress("Scanning Widgets Tree...", True)
    End Sub

    Private Sub btnWriteWidgets_Click(sender As Object, e As EventArgs) Handles btnWriteWidgets.Click
        iRule.WriteWidgetListsToFile()
        'Dim node As MyTreeNode = tv1.SelectedNode
        'iRule.ScanPage(node)
        'tv2.Nodes.Clear()
        'tv2.Nodes.Add(iRule.PageNodeTree)
        'tv2.ExpandAll()
    End Sub

    Private Sub frmTester_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        If iRule IsNot Nothing Then
            iRule.Cleanup()
            iRule = Nothing
        End If

    End Sub

    Private Sub tv1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tv1.AfterSelect
        Dim node As MyTreeNode = e.Node

        pg1.SelectedObject = node.BasicInfo
    End Sub

    Private Sub tv2_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tv2.AfterSelect
        Dim node As MyTreeNode = e.Node
        pg1.SelectedObject = node.BasicInfo
        dg1.DataSource = node.Data
    End Sub



    Private Sub frmTester_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Left = 1200
        tv1.ShowNodeToolTips = True
        tv2.ShowNodeToolTips = True
    End Sub

    Private Sub btnReadWidgets_Click(sender As Object, e As EventArgs) Handles btnReadWidgets.Click
        iRule.ReadWidgetListsFromFile()
        If iRule.WidgetTree IsNot Nothing Then
            tv2.Nodes.Clear()
            tv2.ImageList = iRule.imageList
            tv2.Nodes.Add(iRule.WidgetTree)
            tv2.ExpandAll()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        pg1.SelectedObject = iRule.GetWidgetListByName
    End Sub
End Class