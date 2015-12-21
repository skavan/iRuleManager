Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml.Serialization
Imports WatiN.Core
'Imports System.Runtime.Serialization

Namespace Manager

    Public Class Scraper

#Region "Variables"
        Property ie As IE = Nothing
        Property AvailablePanels As New Dictionary(Of String, String)   '// ID and Panel Name
        Property NodeTree As MyTreeNode
        Property PageTree As MyTreeNode
        Property PageNodeTree As MyTreeNode
        Property WidgetTree As MyTreeNode
        Property imageList As New ImageList
        Property iRuleURL As String = "https://builder.iruleathome.com/iRule.html#main"

        Property Device As New iRuleDevice
        Property Panel As New iRulePanel
        Property Page As New iRulePage

        Private WidgetListByHash As New Dictionary(Of String, IRuleBasicItemInfo)
        Private WidgetListByName As New Dictionary(Of String, IRuleBasicItemInfo)

        Private leftPanel As Div = Nothing      '// The Div holding the tree section and properties section in the leftHand Panel
        Private leftTree As Div = Nothing       '// The Device tree
        Private tableProps As Table = Nothing
        Private leftCollapseAll As Image = Nothing
        'Private WidgetList As New List(Of String)
        Private rightPanel As Div = Nothing
        Private rightBody As Div = Nothing
        Private rightTree As Div = Nothing      '// should be grabbed after section is selected
        Private rightTabs As Div = Nothing
        Private rightCollapseAll As Image = Nothing
        Private _isConnected As Boolean = False
        Private expandImage As String   '// the style of the expandImageIcon
#End Region


        Public Event Progress(percentage As Single)
        Public Event Message(message As String)

        Public Function GetWidgetListByName() As String()
            Return WidgetListByName.Keys.ToArray
        End Function

#Region "Initialization & Cleanup"
        Public Sub New()

        End Sub

        Public Sub New(url As String)
            _isConnected = InitializeSystem(url)
        End Sub

        '// get the basics!
        Private Function InitializeSystem(url As String) As Boolean
            '// attach to existing iRule Internet Explorer Instance
            ie = IE.AttachTo(Of IE)(Find.ByUrl(url), 5)
            ie.AutoClose = False

            '// lets get the whole leftHandPanel including the property panel
            leftPanel = ie.Divs.Filter(Find.ByClass("gwt-SplitLayoutPanel")).First
            '// there are multiple gwt-Tree's -- lets only get the first.
            leftTree = GetChildDivByClassName(leftPanel, "irule-TabBottomPanel irule-HandsetsManagerView")

            '// get the right hand area (including top tabs)
            rightPanel = ie.Divs.Filter(Find.ByClass("gwt-TabLayoutPanel")).First

            rightTabs = rightPanel.Divs.Filter(Find.ByClass("gwt-TabLayoutPanelTabs")).First
            Debug.Print("Initialization Complete")
            'ExecutePopupMenu(ie, leftHandPanel, "Collapse All")
            Return True
        End Function
#End Region


#Region "Prior Art"
        Public Sub Scan(scanLevel As eScanLevel)
            Select Case scanLevel
                Case eScanLevel.PanelsAndPages
                    ExecutePopupMenu(ie, leftPanel, "Collapse All")
                    NodeTree = ParsePanelTreeItems(leftPanel, leftTree.Children, tableProps, New MyTreeNode("iRule Project"), eiRuleItemType.Device, scanLevel, eScanDetail.NoDetail, "", "")
            End Select
        End Sub

        Public Sub ScanPage(node As MyTreeNode)
            If node Is Nothing Then Exit Sub
            ExecutePopupMenu(ie, leftPanel, "Collapse All")
            '// we want to grab the parent panel DIV and then launch a parse event on
            '// the children - And a detailed parse on the target page.
            Dim panelNode As MyTreeNode = node.GetParent.GetParent '// (it's two up))

            '// Let's get the DIV of this panel, using its ID an dout it into a collection.
            Dim colPanelTree As ElementCollection = leftPanel.Elements.Filter(Find.ById(panelNode.Info.ID))
            Dim newNode As New MyTreeNode("Selected Project:" & panelNode.Info.Name)
            newNode.Info = panelNode.Info

            PageNodeTree = ParsePanelTreeItems(leftPanel, leftTree.Children, tableProps, newNode, eiRuleItemType.Panel, eScanLevel.PanelsAndPagesAndWidgets, eScanDetail.WidgetInfo, panelNode.Info.Name, node.Info.Name)


            ''// Let's get the ID.
            'Dim ID = node.Info.ID
            ''Speed Test
            'Dim pan As Element = leftHandPanels.Element(node.Info.ID)
            'Debug.Print("Finished")
            'Dim pageDiv As Div = leftHandPanels.Divs.Filter(Find.ById(node.Info.ID)).First
            'Dim pageTreeCol As ElementCollection = leftHandPanels.Elements.Filter(Find.ById(node.Info.ID))
            'PageNodeTree = ParsePanelTreeItems(leftHandPanels, pageTreeCol, tableProps, New MyTreeNode(node.Info.ParentPanel), eiRuleItemType.Page, eScanLevel.PanelsAndPagesAndWidgets, eScanDetail.WidgetInfo, node.Info.ParentPanel, node.Info.Name)
            Debug.Print("Page Scanned")
        End Sub

#End Region


#Region "Scanning Functions"
        '// Test version
        Public Sub ScanDeviceTree(title As String)
            'ScanDeviceTree("Main", "Landscape Pages", "Home", 4, False)
            ScanDeviceTree(title, "", "", "", 3, False)
        End Sub

        Public Sub ScanDeviceTree(title As String, excludeGroups As String, level As Integer)
            'ScanDeviceTree("Main", "Landscape Pages", "Home", 4, False)
            Dim filters As iRuleScanFilter() = BuildScanFilters(excludeGroups, 3)
            ScanDeviceTree(title, False, filters)
        End Sub


        '// flexible version
        Public Sub ScanDeviceTree(title As String, panel As String, group As String, page As String, level As Integer, bDoDeepScan As Boolean)
            Dim filters As iRuleScanFilter() = BuildScanFilters(panel, group, page, level)
            ScanDeviceTree(title, bDoDeepScan, filters)
        End Sub

        '// execution version to scan a device Tree
        Public Sub ScanDeviceTree(title As String, bDoDeepScan As Boolean, filters As iRuleScanFilter())
            If Not _isConnected Then _isConnected = InitializeSystem(iRuleURL)
            '// collapse everything so we know our beginning state
            leftCollapseAll = FindActiveImageByTitleName(leftPanel, "Collapse All")
            leftCollapseAll.UIEvent("click")
            '//bDoDeepScan = True '//TEMPORARY
            Dim leftBody = FindActiveDivByClassName(leftPanel, "irule-GwtPanelBody")
            leftTree = leftBody.Divs.Filter(Find.ByClass("gwt-Tree")).First

            If bDoDeepScan Then
                SetLeftPanel(leftPanel)
                Dim rootNode As MyTreeNode = GetHandsetNode(leftPanel, True)
                If rootNode Is Nothing Then Exit Sub
                PageTree = ParseTree(leftTree, rootNode, 0, bDoDeepScan, filters)
                ProcessImageList(PageTree, True)
            Else
                Dim rootNode As MyTreeNode = GetHandsetNode(leftPanel, False)
                If rootNode Is Nothing Then Exit Sub
                NodeTree = ParseTree(leftTree, rootNode, 0, bDoDeepScan, filters)
                ProcessImageList(NodeTree, True)
            End If     '// need to set the reference for deep scans.



        End Sub

        Public Sub ScanWidgetTree()
            Dim filters As iRuleScanFilter() = BuildScanFilters("Modules|Conditionals", 4)
            ScanWidgetTree(filters)
        End Sub

        Public Sub ScanWidgetTree(filters As iRuleScanFilter())
            If Not _isConnected Then _isConnected = InitializeSystem(iRuleURL)
            '// Select the more tab at the head of the right Tree
            Dim widgetTab As Div = FindDivContainingText(rightTabs, "MORE")
            SelectAndClickElement(widgetTab)
            '// collapse everything so we know our beginning state
            rightCollapseAll = FindActiveImageByTitleName(rightPanel, "Collapse All")
            rightCollapseAll.UIEvent("click")
            '// get the tree

            '// I'm choosing this approach because there are multiple PanelBody's on the right hand side.
            '// one for each tab. We want the active tab (which in this case is "MORE" and also the last one!
            '//  --to do -- fix this to find the one that "exists = true""!
            rightBody = FindActiveDivByClassName(rightPanel, "irule-GwtPanelBody")
            rightTree = rightBody.Divs.Filter(Find.ByClass("gwt-Tree")).First

            'Dim filters(0) As iRuleScanFilter
            'filters(0) = New iRuleScanFilter(eTreeSearchOptions.ExcludeGroup, "Modules|Conditionals")
            'filters(0) = New iRuleScanFilter(eTreeSearchOptions.RestrictToPanel, "Widgets")

            WidgetListByHash.Clear()
            WidgetListByName.Clear()
            WidgetTree = ParseTree(rightTree, New MyTreeNode("LiveScan Widget Tree"), 0, False, filters)
            ProcessImageList(WidgetTree, False)
            CleanupImageList()

        End Sub
#End Region

        Private Sub CleanupImageList()
            WidgetListByName = AppendWidgetBasedOn(WidgetListByName, "Button", "Slider")
            WidgetListByName = AppendWidgetBasedOn(WidgetListByName, "Message", "Numeric Feedback")
            WidgetListByName = AppendWidgetBasedOn(WidgetListByName, "Message", "Textual Feedback")
            WidgetListByName = AppendWidgetBasedOn(WidgetListByName, "App", "Launch App")
            WidgetListByName = AppendWidgetBasedOn(WidgetListByName, "Browser", "Launch Browser")
        End Sub





        Public Function FillTree() As MyTreeNode
            Dim node As New MyTreeNode("Loaded Widgets")
            Dim thisLevel As Integer = -1
            Dim previousLevel As Integer = -1
            Dim currentNode As MyTreeNode = node
            For Each item As IRuleBasicItemInfo In WidgetListByName.Values
                Dim thisNode As New MyTreeNode(item.Name)
                thisNode.BasicInfo = item
                '// level is the same as the last - brother [go up a node, add a node]
                '// level is greater than the last -- child (create child node)
                '// level is less - walk up to node at the level-1 and create one
                If item.Name = "Launch" Then
                    Debug.Print("Stop here")
                End If
                Select Case item.Level
                    Case previousLevel
                        '// go up a level and reset level
                        If currentNode.Parent Is Nothing Then
                            Debug.Print("why am i here")
                        End If


                        currentNode = currentNode.Parent
                        currentNode.Nodes.Add(thisNode)
                        previousLevel = item.Level
                        currentNode = thisNode
                    Case > previousLevel
                        currentNode.Nodes.Add(thisNode)
                        '// go down a level
                        currentNode = thisNode
                        previousLevel = item.Level
                    Case < previousLevel
                        For i As Integer = item.Level To previousLevel
                            currentNode = currentNode.Parent

                        Next
                        currentNode.Nodes.Add(thisNode)
                        previousLevel = item.Level
                        currentNode = thisNode
                End Select
                Dim hashkey As String = CalculateMD5Hash(thisNode.BasicInfo.ImageSrc)
                If Not imageList.Images.ContainsKey(hashKey) Then
                    Dim img As System.Drawing.Image = CreateImageFromBase64(thisNode.BasicInfo.Base64Data)
                    imageList.Images.Add(hashKey, img)
                End If
                thisNode.ImageKey = hashkey
            Next
            Return node
        End Function

        '// create master image list and clean up type tages in the tree.
        Private Sub ProcessImageList(tree As MyTreeNode, isLeftTree As Boolean)
            For Each node As MyTreeNode In tree.Nodes

                '// get the image HTML string
                Dim src As String = node.BasicInfo.ImageSrc
                Dim hashKey As String = ""
                If src IsNot Nothing Then
                    '// extract the Base64 data
                    If node.BasicInfo.Base64Data <> "" Then
                        hashKey = CalculateMD5Hash(src)
                        '// if the image doesn't exist - then add it.
                        If Not imageList.Images.ContainsKey(hashKey) Then
                            Dim img As System.Drawing.Image = CreateImageFromBase64(node.BasicInfo.Base64Data)
                            imageList.Images.Add(hashKey, img)
                            '// add the hash and name to the Dictionary if we're processing the widgets
                            If Not isLeftTree Then
                                WidgetListByHash.Add(hashKey, node.BasicInfo)
                            End If
                        End If
                        If Not isLeftTree Then
                            If node.BasicInfo.Level = 0 Then
                                WidgetListByName.Add(node.BasicInfo.Name & " (" & node.BasicInfo.Type & ")", node.BasicInfo)
                            Else
                                '// only add if the widget name is not already present!
                                If Not WidgetListByName.ContainsKey(node.BasicInfo.Name) Then
                                    WidgetListByName.Add(node.BasicInfo.Name, node.BasicInfo)
                                End If
                            End If
                        End If
                        node.ImageKey = hashKey
                    End If
                End If

                '// special processing for the left tree
                If isLeftTree Then
                    '// if the current widget type (derived from parsing the node) can't be found in the master widgetlist then
                    '// try and find it based on the icon hashkey.
                    '// note - some widgets share the same icon (i.e. Horizontal Slider and Vertical Slider and Button)
                    If Not WidgetListByName.ContainsKey(node.BasicInfo.Type) Then
                        '// now try and look it up in WidgetList by Image Hash
                        Dim Msg As String = String.Format("Not Found ({0}) With name ({1}) ", node.BasicInfo.Type, node.BasicInfo.Name)
                        If WidgetListByHash.ContainsKey(hashKey) Then
                            node.BasicInfo.Type = WidgetListByHash(hashKey).Name
                            If node.Data IsNot Nothing Then
                                'Dim dic As Dictionary(Of String, String) = node.Data
                                ' dic("itemType") = WidgetListByHash(hashKey).Name
                                node.Data("itemType") = WidgetListByHash(hashKey).Name
                            End If
                        End If
                        Debug.Print("{0} replaced With {1} that has Type {2}", Msg, node.BasicInfo.Type, WidgetListByHash.ContainsKey(hashKey))
                    End If
                End If
                'Process all the children iteratively!
                If node.GetNodeCount(False) > 0 Then
                    ProcessImageList(node, isLeftTree)
                End If
            Next
        End Sub

        Public Sub GoToElement(node As MyTreeNode)
            If _isConnected Then
                FindElement(leftTree, node.BasicInfo.GUID, True)
            End If

            'ChangeElementData(leftTree, node.BasicInfo.GUID)
        End Sub

        Public Sub WriteToElement(node As MyTreeNode)
            Dim myDic As Dictionary(Of String, String) = node.Data
            If myDic IsNot Nothing Then
                Dim current As String = GetSplitValueItem(myDic("name"), 1, True)
                myDic("name") = SetSplitValueItem(myDic("name"), 1, current & "XX", {"[", "]"})
                FindElement(leftTree, node.BasicInfo.GUID, True)
                UpdatePropertyTable(leftTree, node)
                node.Text = myDic("name")
            End If
        End Sub
        Public Sub Cleanup()

            If ie IsNot Nothing Then
                ie.Dispose()
            End If
        End Sub

        Public Sub WriteWidgetListsToFile(node As MyTreeNode)
            WriteiRuleTemplate(node, "G:\widgets.xml")
            'WriteDictionaryToFile("G:\WidgetListByName.xml", WidgetListByName)
            'WriteDictionaryToFile("G:\WidgetListByHash.xml", WidgetListByHash)
        End Sub

        Public Function ReadWidgetListsFromFile() As Boolean
            'WidgetListByName = ReadDictionaryFromFile("G:\WidgetListByName.xml")
            'WidgetListByHash = ReadDictionaryFromFile("G:\WidgetListByHash.xml")
            'WidgetTree = FillTree()
            WidgetTree = ReadiRuleTemplate("G:\widgets.xml")
            ProcessImageList(WidgetTree, False)
            Return True
        End Function

        Public Sub WriteToTemplate(node As MyTreeNode)
            WriteiRuleTemplate(node, "G:\template.xml")
        End Sub
        Public Sub ReadFromtemplate()
            PageTree = ReadiRuleTemplate("G:\template.xml")
            ProcessImageList(PageTree, True)
        End Sub
    End Class

End Namespace
