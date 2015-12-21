Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports iRuleManager.Manager

Public Class frmTester

    Dim iRule As New Scraper
    Dim ST As New Stopwatch
    Dim _thread As Thread
    Dim _progressForm As frmProgressIndicator

    '// This creates a form on a seperate thread...not great practice...but it works.
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

    '// The method to display the progress form and kill(hide) it
    Private Sub ShowProgress(message As String, bKill As Boolean)

        If _progressForm Is Nothing Then
            _progressForm = Application.OpenForms.OfType(Of frmProgressIndicator).First
        End If

        If bKill Then
            _progressForm.HideMe()
        Else
            _progressForm.DisplayText(message)
        End If
    End Sub

    '// Initialization of the iRule system
    Private Sub btnInit_Click(sender As Object, e As EventArgs) Handles btnInit.Click
        ShowProgress("Initializing System...", False)
        iRule = New Scraper("https://builder.iruleathome.com/iRule.html#main")
        btnInit.Enabled = False
        btnScan.Enabled = True
        ShowProgress("Initializing System...", True)
    End Sub

    '// Get the basic tree of the current device.
    Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
        ST.Start()
        ShowProgress("Scanning Device Tree...", False)
        iRule.ScanDeviceTree("Device Tree", "Entrances|Motions|Gestures", eTreeLevel.Page)

        If iRule.NodeTree IsNot Nothing Then
            tvTree.Nodes.Clear()
            tvTree.ImageList = iRule.imageList
            tvTree.Nodes.Add(iRule.NodeTree)
            tvTree.ExpandAll()
        End If
        ShowProgress("Scanning Device Tree...", True)
    End Sub

    '// Scan the Right Tree of iRule and build the list of available widgets
    Private Sub btnWidgets_Click(sender As Object, e As EventArgs) Handles btnWidgets.Click
        ShowProgress("Scanning Widgets Tree...", False)
        iRule.ScanWidgetTree()
        If iRule.WidgetTree IsNot Nothing Then
            tvWidgetList.Nodes.Clear()
            tvWidgetList.ImageList = iRule.imageList
            tvWidgetList.Nodes.Add(iRule.WidgetTree)
            tvWidgetList.ExpandAll()
        End If
        ShowProgress("Scanning Widgets Tree...", True)
    End Sub

    '// Write the widgets list to a file (well two files)
    Private Sub btnWriteWidgets_Click(sender As Object, e As EventArgs) Handles btnWriteWidgets.Click
        Dim node As MyTreeNode = tvWidgetList.TopNode
        iRule.WriteWidgetListsToFile(node)
    End Sub

    '// Clean up the iRule Manager Class and Exit
    Private Sub frmTester_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If iRule IsNot Nothing Then
            iRule.Cleanup()
            iRule = Nothing
        End If
        If Application.OpenForms.OfType(Of frmProgressIndicator).Count = 1 Then
            _progressForm = Application.OpenForms.OfType(Of frmProgressIndicator).First
            _progressForm.KillMe()
        End If

    End Sub

    '// React to a click on a node of the Main iRule Tree
    Private Sub tvTree_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvTree.AfterSelect
        Dim node As MyTreeNode = e.Node

        pg1.SelectedObject = node.BasicInfo
        If node.Data IsNot Nothing Then
            'dg1.DataSource = node.Data.ValueCollection
            Dim dic As Dictionary(Of String, String) = node.Data
            'pg1.SelectedObject = dic.ToArray
        End If

    End Sub

    Private Sub tv2_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvWidgetList.AfterSelect
        Dim node As MyTreeNode = e.Node
        pg1.SelectedObject = node.BasicInfo
        If node.Data IsNot Nothing Then
            dg1.DataSource = node.Data.Values
        End If

    End Sub

    '// Initial form load actions
    Private Sub frmTester_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Left = 1200
        tvTree.ShowNodeToolTips = True
        tvWidgetList.ShowNodeToolTips = True
        CreateProgressForm()

        'ShowProgress("Loading...", True)

    End Sub

    '// Read the widgets file from disk.
    Private Sub btnReadWidgets_Click(sender As Object, e As EventArgs) Handles btnReadWidgets.Click
        iRule.ReadWidgetListsFromFile()
        If iRule.WidgetTree IsNot Nothing Then
            tvWidgetList.Nodes.Clear()
            tvWidgetList.ImageList = iRule.imageList
            tvWidgetList.Nodes.Add(iRule.WidgetTree)
            tvWidgetList.ExpandAll()
        End If
    End Sub

    '// Load a specified page for detailed work (with a deep property scan)
    Private Sub btnLoadPage_Click(sender As Object, e As EventArgs) Handles btnLoadPage.Click

        Dim node As MyTreeNode = tvTree.SelectedNode
        If node.BasicInfo.Type = "Page" Then

            Dim panel As String = DirectCast(node.Parent.Parent, MyTreeNode).BasicInfo.Name
            Dim group As String = DirectCast(node.Parent, MyTreeNode).BasicInfo.Name
            Dim page As String = node.BasicInfo.Name
            Dim pagePath As String = panel & "\" & group & "\" & page
            ShowProgress("Scanning " & pagePath, False)
            iRule.ScanDeviceTree("Active Page", panel, group, page, 4, True)
            InitPageNodeTree()
            ShowProgress("Scanning " & pagePath, True)
        Else
            MsgBox("You must Select a Page", MsgBoxStyle.Information)
        End If
        'pg1.SelectedObject = iRule.GetWidgetListByName
    End Sub

    '// After a node on the target page is selected, jump to that element in the iRule tree and also, display its property data
    Private Sub tvPage_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvPage.AfterSelect
        Dim node As MyTreeNode = e.Node
        If node.Data IsNot Nothing Then
            'dg1.DataSource = node.Data.ValueCollection
            iRule.GoToElement(node)
            Dim dic As Dictionary(Of String, String) = node.Data
            pg1.SelectedObject = node.BasicInfo
            dg1.DataSource = DictionarytoList(node.Data)
            'dg1.DataSource = node.Data
        End If
    End Sub

    '// test the write (to iRule) data routines.
    Private Sub btnWrite_Click(sender As Object, e As EventArgs) Handles btnWrite.Click
        Dim node As MyTreeNode = tvPage.SelectedNode
        If node.Data IsNot Nothing Then
            'dg1.DataSource = node.Data.ValueCollection
            iRule.WriteToElement(node)
            Dim dic As Dictionary(Of String, String) = node.Data
            pg1.SelectedObject = dic.ToArray
        End If
    End Sub

    '// Initialize the Page Node Tree
    Private Sub InitPageNodeTree()
        If iRule.PageTree IsNot Nothing Then
            tvPage.Nodes.Clear()
            tvPage.ImageList = iRule.imageList
            tvPage.Nodes.Add(iRule.PageTree)
            tvPage.ExpandAll()
            tbDeviceTree.SelectedTab = tbPage2
        End If
    End Sub
    Private Sub btnTest_Click(sender As Object, e As EventArgs) Handles btnTest.Click
        Dim node As MyTreeNode = tvPage.TopNode
        iRule.WriteToTemplate(node)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        iRule.ReadFromtemplate()
        InitPageNodeTree()
    End Sub
End Class