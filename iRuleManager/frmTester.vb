Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports iRuleManager.Manager

Public Class frmTester

    Dim iRule As Scraper
    Dim ST As New Stopwatch
    Dim _thread As Thread
    Dim _progressForm As frmProgressIndicator

    Private Sub CreateProgressForm()
        If _thread Is Nothing Then
            _thread = New Thread(Sub()
                                     Using frm As New frmProgressIndicator
                                         Application.Run(frm)
                                         'frm.DisplayText(message)
                                     End Using
                                 End Sub)
            _thread.Start()
            Application.DoEvents()

        End If
    End Sub
    Private Sub ShowProgress(message As String, bKill As Boolean)

        If _progressForm Is Nothing Then
            _progressForm = Application.OpenForms.OfType(Of frmProgressIndicator).First
        End If

        If bKill Then
            _progressForm.HideMe()
            'frmProgress.Enabled = False
            'frmProgress.Visible = False
            'Me.BringToFront()
        Else
            _progressForm.DisplayText(message)

            'frmProgress.Visible = True
            'frmProgress.Enabled = True
            'frmProgress.DisplayText(message)
            'frmProgress.BringToFront()
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
        iRule.ScanDeviceTree("Device Tree", "Entrances|Motions|Gestures", 3)

        If iRule.NodeTree IsNot Nothing Then
            tvTree.Nodes.Clear()
            tvTree.ImageList = iRule.imageList
            tvTree.Nodes.Add(iRule.NodeTree)
            tvTree.ExpandAll()
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

    End Sub

    Private Sub frmTester_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If iRule IsNot Nothing Then
            iRule.Cleanup()
            iRule = Nothing
        End If
        _progressForm.KillMe()
    End Sub

    Private Sub tvTree_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvTree.AfterSelect
        Dim node As MyTreeNode = e.Node

        pg1.SelectedObject = node.BasicInfo
        If node.Data IsNot Nothing Then
            'dg1.DataSource = node.Data.ValueCollection
            Dim dic As Dictionary(Of String, String) = node.Data
            'pg1.SelectedObject = dic.ToArray
        End If

    End Sub

    Private Sub tv2_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tv2.AfterSelect
        Dim node As MyTreeNode = e.Node
        pg1.SelectedObject = node.BasicInfo
        If node.Data IsNot Nothing Then
            dg1.DataSource = node.Data.Values
        End If

    End Sub

    Private Sub frmTester_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Left = 1200
        tvTree.ShowNodeToolTips = True
        tv2.ShowNodeToolTips = True
        CreateProgressForm()

        'ShowProgress("Loading...", True)

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

    Private Sub btnLoadPage_Click(sender As Object, e As EventArgs) Handles btnLoadPage.Click

        Dim node As MyTreeNode = tvTree.SelectedNode
        If node.BasicInfo.Type = "Page" Then

            Dim panel As String = DirectCast(node.Parent.Parent, MyTreeNode).BasicInfo.Name
            Dim group As String = DirectCast(node.Parent, MyTreeNode).BasicInfo.Name
            Dim page As String = node.BasicInfo.Name
            Dim pagePath As String = panel & "\" & group & "\" & page
            ShowProgress("Scanning " & pagePath, False)
            iRule.ScanDeviceTree("Active Page", panel, group, page, 4, True)
            If iRule.PageTree IsNot Nothing Then
                tvPage.Nodes.Clear()
                tvPage.ImageList = iRule.imageList
                tvPage.Nodes.Add(iRule.PageTree)
                tvPage.ExpandAll()
                tbDeviceTree.SelectedTab = tbPage2
            End If
            ShowProgress("Scanning " & pagePath, True)
        Else
            MsgBox("You must Select a Page", MsgBoxStyle.Information)
        End If
        'pg1.SelectedObject = iRule.GetWidgetListByName
    End Sub

    Private Sub tvPage_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvPage.AfterSelect
        Dim node As MyTreeNode = e.Node
        If node.Data IsNot Nothing Then
            'dg1.DataSource = node.Data.ValueCollection
            iRule.GoToElement(node)
            Dim dic As Dictionary(Of String, String) = node.Data
            pg1.SelectedObject = dic.ToArray
        End If
    End Sub

    Private Sub btnWrite_Click(sender As Object, e As EventArgs) Handles btnWrite.Click
        Dim node As MyTreeNode = tvPage.SelectedNode
        If node.Data IsNot Nothing Then
            'dg1.DataSource = node.Data.ValueCollection
            iRule.WriteToElement(node)
            Dim dic As Dictionary(Of String, String) = node.Data
            pg1.SelectedObject = dic.ToArray
        End If
    End Sub

    Private Sub btnTest_Click(sender As Object, e As EventArgs) Handles btnTest.Click


    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        '_thread.Abort()
        '_thread.Abort()
    End Sub


End Class