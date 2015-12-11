Imports System.Collections.Specialized
Imports WatiN.Core
Imports System.Threading
Imports System.ComponentModel

Public Class frmMain

    Dim colPanels As New Collection
    Dim [ie] As IE = Nothing
    Dim leftHandPanels As Div = Nothing '// The Div holding the three section in the leftHand Panel
    Dim panTree As Div = Nothing
    Dim tableProps As Table = Nothing
    Dim targetPanel As String = "Main"
    Dim thisPanel As String = ""
    Dim ST As New Stopwatch
    <STAThread>
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Dim ie = New IE("http://www.google.com")
        ie.TextField(Find.ByName("q")).TypeText("Suresh Kavan")
        ie.NativeDocument.Body.SetFocus()
        ie.Button(Find.ByName("btnK")).Click()
        'Dim results As String = ie.Para(Find.ById("ires")).Text
        Dim resBlock As Div = ie.Div("ires")
        For Each rSpan As Span In resBlock.Spans.Filter(Find.ByClass("st"))
            Debug.Print(rSpan.InnerHtml)
        Next

        'TextBox1.Text = results
    End Sub

    '// Get the properties table
    Private Function GetTableProps() As Table
        For Each subDiv As Div In leftHandPanels.Children
            If subDiv.Children.Count > 0 Then
                Dim sectionDiv As Div = subDiv.Children(0)
                If sectionDiv.ClassName = "irule-PropertiesManagerView" Then
                    Return sectionDiv.Tables.Filter(Find.ByClass("irule-PropertiesItem")).First
                End If
            End If
        Next
        Return Nothing
    End Function

    '// Get the Device Panel table
    Private Function GetPanelTree() As Div
        For Each subDiv As Div In leftHandPanels.Children
            If subDiv.Children.Count > 0 Then
                Dim sectionDiv As Div = subDiv.Children(0)
                If sectionDiv.ClassName = "irule-TabBottomPanel irule-HandsetsManagerView" Then
                    Return sectionDiv.Divs.Filter(Find.ByClass("gwt-Tree")).First
                End If
            End If
        Next
        Return Nothing
    End Function


    <STAThread>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ie = IE.AttachTo(Of IE)(Find.ByUrl("http://builder.iruleathome.com/iRule.html#main"), 5)
        ie.AutoClose = False
        '// there are multiple gwt-Tree's -- lets only get the first.
        leftHandPanels = ie.Divs.Filter(Find.ByClass("gwt-SplitLayoutPanel")).First
        'Dim closeAll As Div = ie.Divs.Filter(Find.ById("gwt-uid-8176")).First
        'closeAll.UIEvent("mousedown")
        panTree = GetPanelTree()
        Button3.Enabled = True
        Button2.Enabled = False
    End Sub
    <STAThread>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ST.Start()
        ListBox1.Items.Clear()

        '// Now get the large block of panel items
        Dim panItems As ElementCollection = panTree.Children

        Dim tv As TreeView = tv1
        tv.Nodes.Clear()
        Dim rootNode As New TreeNode("iRule Project")
        tv.Nodes.Add(rootNode)
        rootNode.ExpandAll()

        ParseTreeItem2(rootNode, panItems, tableProps, 0, 5, False)
        '//iterate through all the panels in the tree
        Debug.Print("Finished in {0} s", ST.Elapsed.Seconds.ToString)

        Debug.Print(ST.Elapsed.Seconds)
        ST.Stop()
    End Sub

    Private Sub ParseTreeItem1(node As TreeNode, panelItems As ElementCollection, tableProps As Table, level As Integer, stopAtLevel As Integer, bAutoExpand As Boolean)
        '// Step 1 - iterate through the DIV's. Each Div is a parent Element, such as a panel, or an entrance
        '//iterate through all the panels in the tree
        If level > stopAtLevel Then Exit Sub
        For Each wiItem As Div In panelItems

            '// get the first table (there should be one - the parent of this panel element)
            Dim parentTable As Table = wiItem.ChildOfType(Of Table)(Find.Any)
            '// if we don't have one it's not a parseable element.
            If parentTable.Exists = True Then
                '// get the item name from the child table embedded within the parentTable
                Dim tableNameDiv As Div = GetTableNameDiv(parentTable)

                Dim itemName As String = tableNameDiv.InnerHtml

                Debug.Print(">".PadLeft(level * 5, "-") & itemName)
                '// so we now have a node! 
                Dim newNode As TreeNode = AddNode(node, itemName, tableNameDiv, level, 3)

                If bAutoExpand Then
                    '//EXPAND THE NODE ON THE PAGE
                    '// get the first image -- this should be the Tree Expander icon
                    Dim img As Image = parentTable.Images.First
                    '// expand/close the tree
                    If img IsNot Nothing Then img.UIEvent("mousedown")
                End If
                '// the table gives the name. its sibling is a div which hold its children
                '// let's try the child panels - should be only one. The panItem shuld have just a table (used above for panel name) and a DIV for all the children

                'ParseTreeItem2(newNode, New ElementCollection(Of Div){wiItem}, level + 1)

                Dim childPanelsParent As Div = wiItem.ChildOfType(Of Div)(Find.Any)
                If childPanelsParent IsNot Nothing And childPanelsParent.Exists Then
                    '// now process the children
                    'Dim subItems As ElementCollection(Of Div) = childPanelsParent.ChildrenOfType(Of Div)
                    Dim subItems As ElementCollection = childPanelsParent.Children
                    ParseTreeItem1(newNode, subItems, tableProps, level + 1, stopAtLevel, bAutoExpand)
                End If
                newNode.Expand()
            Else
                '// it could be a node without children, in which case we seem to have a DIV wrapped around the table with the name
                '// this div seems to have a class="gwt-TreeItem"
                Dim parentDiv As Div = wiItem.ChildOfType(Of Div)((Find.ByClass("gwt-TreeItem")))
                If parentDiv.Children.Count = 1 Then
                    '// we should only have one item - the table with the label in it.
                    '// so lets get that label and add it to the tree
                    Dim pnlName As String = GetNameFromTable(parentDiv.Children(0))
                    Dim newNode As TreeNode = AddNode(node, pnlName, parentDiv.Children(0), level, 3)
                    'Dim newNode As New TreeNode(pnlName)
                    'node.Nodes.Add(newNode)
                Else
                    '// Let's see why we get here...
                    If wiItem.Children.Count > 0 Then
                        If wiItem.Children(0).TagName = "INPUT" Then
                            Debug.Print("Input element - ignoring!")
                        Else
                            Debug.Print("Why am I here?")
                        End If
                    Else
                        Debug.Print("No children. End of the line.")
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ParseTreeItem2(node As TreeNode, panelItems As ElementCollection, tableProps As Table, level As Integer, stopAtLevel As Integer, bAutoExpand As Boolean)
        '// Step 1 - iterate through the DIV's. Each Div is a parent Element, such as a panel, or an entrance
        '//         it has two children. a table that contains info about this element and a div that contains its children.
        '//         so we iterate through all the DIV's. one at a time.
        If level > stopAtLevel Then Exit Sub                '// this is just to bring a halt to proceeedings prematurely at a given "depth" or level.

        For Each wiItem As Div In panelItems

            Dim itemName As String = ""
            Dim itemType As String = ""
            Dim itemXtraInfo As String = ""
            Dim itemID As String = ""
            Dim bEndOfTheLine As Boolean = False

            '// Process the elements core information (name, type, etc)
            Dim parentTable As Table = GetCurrentItemInfo(wiItem, itemName, itemType, itemXtraInfo, itemID)
            If parentTable Is Nothing Then
                '// this case may occur when there are no further children and this is the last element in this branch
                parentTable = GetCurrentItemInfo(wiItem.ChildOfType(Of Div)(Find.First), itemName, itemType, itemXtraInfo, itemID)
                '// so we redo looking inside the first DIV of wiItem. The children processing below needs to be cut short.
                bEndOfTheLine = True
            End If

            If itemName <> "" Then
                Debug.Print(">".PadLeft(level * 5, "-") & itemName)
                '// so we now have a node! 
                Dim newNode As TreeNode = AddNode2(node, itemType & ":" & itemName, level)
                '// Insert some code to process the node if required.
                '// Let's start by clicking the node
                If itemType = "Panel" Then thisPanel = itemName
                If thisPanel = targetPanel Then
                    If level <= stopAtLevel Then
                        Dim img As Image = parentTable.Images.First             '// get the first image -- this should be the Tree Expander icon
                        If img IsNot Nothing Then img.UIEvent("mousedown")      '// expand/close the tree
                    End If
                    GetTableNameDiv(parentTable).UIEvent("mousedown")
                    newNode.Tag = ReadProperties2(itemID, itemName, itemType, itemXtraInfo)
                End If
                'If level > -1 Then

                'End If

                If bAutoExpand Then                                         '//EXPAND THE NODE ON THE PAGE -- TAKES A LONG TIME IN BULK!
                    Dim img As Image = parentTable.Images.First             '// get the first image -- this should be the Tree Expander icon
                    If img IsNot Nothing Then img.UIEvent("mousedown")      '// expand/close the tree
                End If

                '// OK time to work on the children of this item, which should be contained in the first DIV under the 
                '// let's try the child panels - should be only one. The panItem shuld have just a table (used above for panel name) and a DIV for all the children
                If Not bEndOfTheLine Then
                    Dim childPanelsParent As Div = wiItem.ChildOfType(Of Div)(Find.Any)
                    If childPanelsParent IsNot Nothing And childPanelsParent.Exists Then
                        '// now process the children
                        Dim subItems As ElementCollection = childPanelsParent.Children
                        ParseTreeItem2(newNode, subItems, tableProps, level + 1, stopAtLevel, bAutoExpand)
                    End If
                End If
                newNode.Expand()
            Else
                '// Let's see why we get here...
                If wiItem.Children.Count > 0 Then
                    If wiItem.Children(0).TagName = "INPUT" Then
                        Debug.Print("Input element - ignoring!")
                    Else
                        Debug.Print("Why am I here?")
                    End If
                Else
                    Debug.Print("No children. End of the line.")
                End If
            End If
        Next
    End Sub

    Private Function AddNode2(parentNode As TreeNode, nodeName As String, level As Integer) As TreeNode
        '// so we now have a node! 
        Dim newNode As New TreeNode(nodeName)
        'If level = 3 Then
        'Dim itemProps As iRuleItem = ReadProperties()
        'newNode.Tag = itemProps
        'End If
        parentNode.Nodes.Add(newNode)
        Return newNode
    End Function

    Private Function AddNode(parentNode As TreeNode, nodeName As String, el As Element, level As Integer, targetLevel As Integer) As TreeNode
        '// so we now have a node! 
        Dim newNode As New TreeNode(level & ":" & nodeName)

        If level = 3 Then
            el.UIEvent("mousedown")
            Dim itemProps As iRuleItem = ReadProperties()
            newNode.Tag = itemProps
        End If

        parentNode.Nodes.Add(newNode)
        Return newNode
    End Function

    Private Function ReadProperties2(itemID As String, itemName As String, itemType As String, itemXtraInfo As String) As List(Of KeyValuePair(Of String, String))
        '// long winded -- but only want to refresh tableProps if we have to.
        If tableProps IsNot Nothing Then
            If tableProps.Exists = False Then
                tableProps = GetTableProps()
            End If
        Else
            tableProps = GetTableProps()
        End If
        Dim lstKVP As New List(Of KeyValuePair(Of String, String))
        lstKVP.Add(New KeyValuePair(Of String, String)("elementName", itemName))
        lstKVP.Add(New KeyValuePair(Of String, String)("elementType", itemType))
        lstKVP.Add(New KeyValuePair(Of String, String)("elementMacro", itemXtraInfo))
        lstKVP.Add(New KeyValuePair(Of String, String)("ID", itemID))

        For Each tableRow As TableRow In tableProps.TableRows
            Dim nameCell = tableRow.TableCell(Find.ByClass("nameCell"))
            Dim valueCell = tableRow.TableCell(Find.ByClass("valueCell"))
            Dim nameDiv As Div = nameCell.Children(0)
            '// inspect the div, because the div we want might be nested, one level down
            If nameDiv.Children.Count > 0 Then
                nameDiv = nameDiv.Children(0)
            End If
            '// now get the name of the field
            Dim fldName As String = nameDiv.InnerHtml
            Dim fldValue As String = valueCell.Children(0).Title
            If fldValue = "" Then
                fldValue = valueCell.Children(0).GetAttributeValue("value")
            End If
            Dim kvp As New KeyValuePair(Of String, String)(fldName, fldValue)
            lstKVP.Add(kvp)
        Next
        Return lstKVP
    End Function


    Private Function ReadProperties() As iRuleItem
        '// long winded -- but only want to refresh tableProps if we have to.
        If tableProps IsNot Nothing Then
            If tableProps.Exists = False Then
                tableProps = GetTableProps()
            End If
        Else
            tableProps = GetTableProps()
        End If
        Dim item As New iRuleItem
        For Each tableRow As TableRow In tableProps.TableRows
            Dim nameCell = tableRow.TableCell(Find.ByClass("nameCell"))
            Dim valueCell = tableRow.TableCell(Find.ByClass("valueCell"))
            Dim nameDiv As Div = nameCell.Children(0)
            '// inspect the div, because the div we want might be nested, one level down
            If nameDiv.Children.Count > 0 Then
                nameDiv = nameDiv.Children(0)
            End If
            '// now get the name of the field
            Dim fldValue As String = nameDiv.InnerHtml
            Select Case fldValue
                Case "name"
                    item.Name = valueCell.Children(0).Title
                Case "row"
                    item.Row = valueCell.Children(0).Title
                Case "column"
                    item.Column = valueCell.Children(0).Title
                Case "width (cells)"
                    item.Width = valueCell.Children(0).Title
                Case "height (cells)"
                    item.Height = valueCell.Children(0).Title
                Case "hide after (sec)"
                    item.HideAfter = valueCell.Children(0).Title
                Case "font"
                    item.Font = valueCell.Children(0).Title
                Case "font size"
                    item.FontSize = valueCell.Children(0).Title
                Case "font bold"
                    item.FontBold = valueCell.Children(0).GetAttributeValue("value")
                'valueCell.Children(0).GetAttributeValue("value")
                Case "text color"
                    item.TextColor = valueCell.Children(0).Title
                Case "text alignment"
                    item.TextAlignment = valueCell.Children(0).Title
                Case "scrolling text"
                    item.ScrollingText = valueCell.Children(0).Title
            End Select

        Next
        Return item
    End Function

    Private Sub ParseTreeItem(node As TreeNode, wiItem As Div)
        '// get the first table (there should be one - the parent of this panel element)
        Dim parentTable As Table = wiItem.ChildOfType(Of Table)(Find.Any)

        '// get the panel name
        Dim pnlName As String = GetNameFromTable(parentTable)
        If pnlName <> "" Then
            Debug.Print("---->" & pnlName)
            Dim newNode As New TreeNode(pnlName)
            node.Nodes.Add(newNode)
            '// let's try the child panels - should be only one. The panItem shuld have just a table (used above for panel name) and a DIV for all the children
            Dim childPanelsParent As Div = wiItem.ChildOfType(Of Div)(Find.Any)
            If childPanelsParent IsNot Nothing And childPanelsParent.Exists Then
                For Each child As Div In childPanelsParent.Children
                    ParseTreeItem(newNode, child)
                Next

            End If
            newNode.Expand()
        End If

    End Sub

    '// Get the name and type of the current item
    Private Function GetCurrentItemInfo(wiItem As Div, ByRef itemName As String, ByRef itemType As String, ByRef itemXtraInfo As String, ByRef itemID As String) As Table
        Dim parentTable As Table = wiItem.ChildOfType(Of Table)(Find.First) '// Get the first table within this DIV
        If parentTable.Exists = False Then Return Nothing                   '// There is no set of tables!
        Dim itemTable As Table = parentTable.Tables.First                   '// Get the Table with the stuff we want from within this DIV (should be the first one!)
        If itemTable Is Nothing And parentTable.Title <> "" Then
            '// this is the end of the line type event.
            itemTable = parentTable
        End If
        '// try and find a uniqueID
        Dim divID As Element = itemTable.Parent
        itemID = divID.Id


        If itemTable.Exists Then
            Dim info As String = itemTable.Title
            If info <> "" Then

                Dim firstParam As String = info.Split("*")(0) '// example: Button: BtnMain * Link: Outdoors --> OutdoorZones
                If firstParam <> "" Then

                    If firstParam.Contains(":") Then
                        itemType = firstParam.Split(":")(0).Trim
                        itemName = firstParam.Split(":")(1).Trim
                        itemXtraInfo = info.Remove(0, firstParam.Length).Trim '// remove the first param from the string
                        If itemXtraInfo.Length > 0 Then
                            itemXtraInfo = itemXtraInfo.TrimStart("*")
                        End If
                    Else
                        itemName = itemTable.Divs.First.InnerHtml
                        itemType = "MACRO"
                        itemXtraInfo = info
                    End If

                End If
            End If
        End If

        '// This should catch EMPTY Categories -- like a Portrait category that has no elements in it.
        'itemName = parentTable.Divs.First.InnerHtml

        Return parentTable
    End Function

    Private Function GetTableNameDiv(itemTable As Table) As Div
        If itemTable IsNot Nothing And itemTable.Exists Then
            Dim panName As Div = itemTable.Divs.Filter(Find.ByClass("irule-TreeItemWithImage label")).First
            If panName IsNot Nothing Then
                Return panName
            End If
        End If
        Return Nothing
    End Function

    Private Function GetNameFromTable(itemTable As Table) As String
        If itemTable IsNot Nothing And itemTable.Exists Then
            Dim panName As Div = itemTable.Divs.Filter(Find.ByClass("irule-TreeItemWithImage label")).First
            If panName IsNot Nothing Then
                Return panName.InnerHtml
            End If
        End If
        Return ""
    End Function

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Dim tv As TreeView = tv1
        tv.Nodes.Clear()
        Dim rootNode As New TreeNode("iRule Project")
        tv.Nodes.Add(rootNode)
        For i = 1 To 4
            rootNode.Nodes.Add(New TreeNode("Leaf " & i.ToString))
        Next
        rootNode.Expand()
    End Sub

    Private Sub tv1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tv1.AfterSelect
        If e.Node.Tag IsNot Nothing Then
            dg1.DataSource = Nothing
            dg1.DataSource = e.Node.Tag
            'pg1.SelectedObject = e.Node.Tag
        End If
    End Sub

End Class
