Imports WatiN.Core

Namespace Manager

    Public Enum eScanLevel
        NoScan = 0
        PanelsOnly = 1      'Proxy for Level
        PanelsAndPages = 2     'Proxy for Level
        PanelsAndPagesAndWidgets = 3
        PanelsAndPagesAndWidgetsAndDetail = 4
    End Enum

    Public Enum eScanDetail
        NoDetail
        WidgetInfo
        WidgetDeepInfo
    End Enum

    Public Enum eiRuleItemType
        Device = 0              '// i.e.    Samsung Tablet
        Panel = 1               '// i.e.    Family Room or Outdoors or Main
        PanelGroup = 2          '// i.e     Landscape Pages, Portrait Pages or Entrances
        Page = 3                '// i.e.    TiVo
        Widget = 4              '// i.e.    Button
        WidgetDetail = 5        '// i.e.    Do Something [macro etc..]
        Unknown = 10
    End Enum

    Module iRuleParsingFunctions

        '// Get the Device Panel table
        Public Function GetChildDivByClassName(sourceDiv As Div, className As String) As Div
            For Each subDiv As Div In sourceDiv.Children
                If subDiv.Children.Count > 0 Then
                    Dim sectionDiv As Div = subDiv.Children(0)
                    If sectionDiv.ClassName = className Then
                        Return sectionDiv.Divs.Filter(Find.ByClass("gwt-Tree")).First
                    End If
                End If
            Next
            Return Nothing
        End Function

        '// Main Tree Parser. Should Handle all eventualities!
        Public Function ParsePanelTreeItems(leftHandPanels As Div, panelItems As ElementCollection, tableProps As Table, node As MyTreeNode, thisLevel As eiRuleItemType,
                                       targetLevel As eScanLevel, scanDetail As eScanDetail, targetPanel As String, targetPage As String) As MyTreeNode

            Static thisPanel As String : Static thisPage As String
            '// reset whenever we hit a device or panel.
            Select Case thisLevel
                Case eiRuleItemType.Device
                    thisPanel = "" : thisPage = ""
                Case eiRuleItemType.Panel
                    thisPage = ""
                    If node.Info IsNot Nothing Then thisPanel = node.Info.Name

                Case eiRuleItemType.Page

            End Select

            '// ensure we don't traverse deeper than we need
            If thisLevel > (targetLevel + 1) Then Return Nothing

            '// scan each outer DIV, get the item info, optionally grab the detail, and then recurse for children
            For Each wiItem As Div In panelItems
                '// Get the core info
                Dim itemInfo As New iRuleGenericItem : Dim bEndOfTheLine As Boolean = False
                Dim parentTable As Table = GetCurrentItemInfo(wiItem, itemInfo, bEndOfTheLine)

                If itemInfo.Type = eiRuleItemType.Panel Then thisPanel = itemInfo.Name '// persist the panel name
                If itemInfo.Type = eiRuleItemType.Page Then thisPage = itemInfo.Name '// persist the page name

                '// if we are targetting a panel and this isn't it (but is a panel) get out of here.
                If targetPanel <> "" And itemInfo.Type = eiRuleItemType.Panel And thisPanel <> targetPanel Then Continue For

                '// if we are targetting a page and this isn't it (but is a page) get out of here.
                If targetPage <> "" And itemInfo.Type = eiRuleItemType.Page And thisPage <> targetPage Then Continue For

                '// if its an entrance, motion, gesture or whatever [i.e. not portrair page or lanscape pages], get outta here.
                If targetPage <> "" And itemInfo.Type = eiRuleItemType.PanelGroup And Not (itemInfo.Name.Contains("Pages")) Then Continue For

                If itemInfo.ID <> "" Then            '// is it a valid element?
                    Debug.Print(">".PadLeft(thisLevel * 5, "-") & itemInfo.Name)
                    '// create a node and add it to the parent, pass it the itemInfo for the data required
                    Dim newNode As MyTreeNode = AddNewNode(node, itemInfo, thisLevel)
                    If newNode.Parent IsNot Nothing Then
                        If itemInfo.Type = eiRuleItemType.Page Then
                            Debug.Print("Inspection")
                        End If
                        Dim pNode As MyTreeNode = newNode.GetParent

                        If pNode.Info IsNot Nothing Then
                            newNode.parentID = pNode.Info.ID
                            itemInfo.ParentID = pNode.Info.ID
                        End If

                        If thisPanel = "" Then itemInfo.ParentPanel = thisPanel
                    End If
                    If RequiresFurtherProcessing(itemInfo, thisPanel, thisPage, targetPanel, targetPage, thisLevel, targetLevel, scanDetail) Then
                        ExpandContractNode(parentTable)
                        If SelectPanelItem(parentTable) IsNot Nothing And targetPage <> "" Then
                            newNode.Data = ReadProperties2(leftHandPanels, tableProps, itemInfo)
                        End If
                    End If

                    '// OK time to work on the children of this item, which should be contained in the first DIV under the 
                    '// let's try the child panels - should be only one. The panItem shuld have just a table (used above for panel name) and a DIV for all the children

                    If Not bEndOfTheLine Then
                        Dim childPanelsParent As Div = wiItem.ChildOfType(Of Div)(Find.Any)
                        If childPanelsParent IsNot Nothing And childPanelsParent.Exists Then
                            '// now process the children
                            Dim subItems As ElementCollection = childPanelsParent.Children
                            ParsePanelTreeItems(leftHandPanels, subItems, tableProps, newNode, thisLevel + 1, targetLevel, scanDetail, targetPanel, targetPage)
                        End If
                    End If
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
            Return node
        End Function

        '// Do we need further processing on this item?
        Private Function RequiresFurtherProcessing(itemInfo As iRuleGenericItem, thisPanel As String, thisPage As String, targetPanel As String, targetPage As String, thisLevel As eiRuleItemType, targetLevel As eScanLevel, scanDetail As eScanDetail) As Boolean
            '// if this is the target Panel and we are at a higher level than the target page, expand the treeItem
            '// if scanDetail is enabled do the deepest level possible
            If itemInfo.Name = "Receivers" Then
                Debug.Print("YEAH")
            End If
            If thisPanel = targetPanel Then     '// we're in a chosen panel, expand based on Type
                If thisLevel = eiRuleItemType.Panel Then
                    Return True
                ElseIf thisLevel = eiRuleItemType.PanelGroup And itemInfo.Name.Contains("Pages") Then
                    Return True
                ElseIf thisLevel = eiRuleItemType.Page And thisPage = targetPage Then
                    Return True
                ElseIf thisLevel = eiRuleItemType.Widget And thisPage = targetPage And scanDetail = eScanDetail.WidgetDeepInfo
                    '// we want the deepest level of data on a widget.
                    Return True
                End If
            Else
                '// if there are no specific target pages -- then we need to expand the tree and or get detailed data just for panels, panels + pages or panels+pages+widgets
                '// but only for panels, pages or widgets
                If itemInfo.Type = eiRuleItemType.Panel _
                    Or itemInfo.TypeName = "Landscape Pages" Or itemInfo.Type = eiRuleItemType.Page Then
                    Select Case targetLevel
                        Case eScanLevel.PanelsOnly
                            If thisLevel <= 0 Then Return True
                        Case eScanLevel.PanelsAndPages
                            If thisLevel <= 1 Then Return True
                        Case eScanLevel.PanelsAndPagesAndWidgets
                            If thisLevel <= 2 Then Return True
                    End Select
                End If
            End If
            Return False
        End Function

        '// to-do : build in check to look for collapsed/expanded state and act accordingly
        Private Sub ExpandContractNode(parentTable As Table)
            Dim img As Image = parentTable.Images.First             '// get the first image -- this should be the Tree Expander icon
            If img IsNot Nothing Then img.UIEvent("mousedown")      '// expand/close the tree
        End Sub

        '// wrapper around the ItemInfo parsing routine. This is the one that should be called!
        Private Function GetCurrentItemInfo(wiItem As Div, ByRef itemInfo As iRuleGenericItem, ByRef bEndOfTheLine As Boolean) As Table
            '// Process the elements core information (name, type, etc)
            bEndOfTheLine = False
            Dim parentTable As Table = GetCurrentItemInfo(wiItem, itemInfo)
            If parentTable Is Nothing Then
                '// this case may occur when there are no further children and this is the last element in this branch
                parentTable = GetCurrentItemInfo(wiItem.ChildOfType(Of Div)(Find.First), itemInfo)
                '// so we redo looking inside the first DIV of wiItem. The children processing below needs to be cut short.
                bEndOfTheLine = True
            End If
            Return parentTable
        End Function

        '// Get the name and type of the current item
        Private Function GetCurrentItemInfo(wiItem As Div, ByRef itemInfo As iRuleGenericItem) As Table
            Dim parentTable As Table = wiItem.ChildOfType(Of Table)(Find.First) '// Get the first table within this DIV
            If parentTable.Exists = False Then Return Nothing                   '// There is no set of tables!
            Dim itemTable As Table = parentTable.Tables.First                   '// Get the Table with the stuff we want from within this DIV (should be the first one!)

            If itemTable Is Nothing And parentTable.Title <> "" Then
                '// this is the end of the line type event.
                itemTable = parentTable
            End If

            '// try and find a uniqueID
            Dim divID As Element = itemTable.Parent
            itemInfo.ID = divID.Id

            '// now do the parsing to extract the Name and Type Information
            If itemTable.Exists Then
                Dim info As String = itemTable.Title
                If info <> "" Then

                    Dim firstParam As String = info.Split("*")(0) '// example: Button: BtnMain * Link: Outdoors --> OutdoorZones
                    If firstParam <> "" Then

                        If firstParam.Contains(":") Then
                            Dim sType As String = firstParam.Split(":")(0).Trim
                            itemInfo.TypeName = sType
                            Select Case sType
                                Case "Panel"
                                    itemInfo.Type = eiRuleItemType.Panel
                                Case "Landscape Pages", "Portrait Pages", "Entrances", "Motions", "Gestures"
                                    itemInfo.Type = eiRuleItemType.PanelGroup
                                Case "Page"
                                    itemInfo.Type = eiRuleItemType.Page
                                Case "Label", "Button", "Textual Feedback", "Backgrounds", "Link", "Launch App", "Launch Browser"
                                    itemInfo.Type = eiRuleItemType.Widget
                                Case Else
                                    itemInfo.Type = eiRuleItemType.Unknown
                                    If firstParam.StartsWith("activity") Then
                                        itemInfo.Type = eiRuleItemType.Widget
                                        itemInfo.TypeName = "activity"
                                    End If
                            End Select

                            itemInfo.Name = firstParam.Split(":")(1).Trim
                            itemInfo.XtraInfo = info.Remove(0, firstParam.Length).Trim '// remove the first param from the string
                            If itemInfo.XtraInfo.Length > 0 Then
                                itemInfo.XtraInfo = itemInfo.XtraInfo.TrimStart("*")
                            End If
                        Else
                            itemInfo.Name = itemTable.Divs.First.InnerHtml
                            itemInfo.Type = "MACRO"
                            itemInfo.XtraInfo = info
                        End If

                    End If
                End If
            End If
            Return parentTable
        End Function

        '// Add a node and its item info to myTreeNode
        Private Function AddNewNode(parentNode As MyTreeNode, itemInfo As iRuleGenericItem, level As Integer) As MyTreeNode
            Dim nodeName As String = String.Format("{0} ({1}) [{2}]", itemInfo.Name, itemInfo.Type, itemInfo.ID)
            '// Create a node 
            Dim newNode As New MyTreeNode(nodeName)
            '// shove the itemInfo into it
            newNode.Info = itemInfo
            parentNode.Nodes.Add(newNode)
            Return newNode
        End Function

        Private Function ReadProperties2(leftHandPanels As Div, tableProps As Table, itemInfo As iRuleGenericItem) As List(Of KeyValuePair(Of String, String))
            '// long winded -- but only want to refresh tableProps if we have to.
            If tableProps IsNot Nothing Then
                If tableProps.Exists = False Then
                    tableProps = GetTableProps(leftHandPanels)
                End If
            Else
                tableProps = GetTableProps(leftHandPanels)
            End If
            Dim lstKVP As New List(Of KeyValuePair(Of String, String))
            lstKVP.Add(New KeyValuePair(Of String, String)("itemName", itemInfo.Name))
            lstKVP.Add(New KeyValuePair(Of String, String)("itemType", itemInfo.Type))
            lstKVP.Add(New KeyValuePair(Of String, String)("itemMacro", itemInfo.XtraInfo))
            lstKVP.Add(New KeyValuePair(Of String, String)("itemParentPanel", itemInfo.ParentPanel))
            lstKVP.Add(New KeyValuePair(Of String, String)("itemID", itemInfo.ID))

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

        '// Get the properties table
        Private Function GetTableProps(lefthandPanels As Div) As Table
            For Each subDiv As Div In lefthandPanels.Children
                If subDiv.Children.Count > 0 Then
                    Dim sectionDiv As Div = subDiv.Children(0)
                    If sectionDiv.ClassName = "irule-PropertiesManagerView" Then
                        Return sectionDiv.Tables.Filter(Find.ByClass("irule-PropertiesItem")).First
                    End If
                End If
            Next
            Return Nothing
        End Function

        '// Click the DIV surrrounding the Tree Element
        Private Function SelectPanelItem(itemTable As Table) As Div
            If itemTable IsNot Nothing And itemTable.Exists Then
                Dim panName As Div = itemTable.Divs.Filter(Find.ByClass("irule-TreeItemWithImage label")).First
                If panName IsNot Nothing Then
                    ''// CLICK THE ELEMENT
                    'panName.UIEvent("mousedown")
                    Return panName
                End If
            End If
            Return Nothing
        End Function

        '// Get the LeftHandPanelPopUpMenu and Click an element
        Public Sub ExecutePopupMenu(ie As IE, leftHandPanels As Div, elementName As String)
            Dim panelBody As Div = leftHandPanels.Divs.Filter(Find.ByClass("irule-GwtPanelBody")).First
            '// just to ensure we get rid of any existing popup.
            panelBody.UIEvent("mousedown", 0)
            '// now right click to call up the popup
            panelBody.UIEvent("mousedown", 2)


            Dim popupTable As Table = GetPopupMenu(ie)
            Dim targetTD As TableCell = GetCellByInnerText(popupTable, elementName)
            SelectAndClickElement(targetTD)
            'If targetTD IsNot Nothing Then
            '    targetTD.UIEvent("mouseover")
            '    targetTD.WaitForComplete()

            '    targetTD.UIEvent("mousedown", 0)
            '    targetTD.UIEvent("mouseup", 0)
            '    targetTD.UIEvent("click", 0)
            '    targetTD.WaitForComplete()
            '    Debug.Print("PopupMenu Action Complete!")
            'End If
        End Sub

        Public Function GetPopupMenu(ie As IE) As Table
            'The popupMenu is added as the final DIV of the body of the page
            Dim popupMenu As Div = ie.Body.Children.Last
            'popupMenu.UIEvent("contextmenu")

            '// no wgrab the Table containing the popup menu and find the target cell that we need to click.
            Return popupMenu.Tables.First

        End Function

        '// given an element that requires selection (popup, tab list etc..), select and click.
        Public Sub SelectAndClickElement(element As Element)
            If element IsNot Nothing Then
                element.UIEvent("mouseover")
                element.WaitForComplete()

                element.UIEvent("mousedown", 0)
                element.UIEvent("mouseup", 0)
                element.UIEvent("click", 0)
                element.WaitForComplete()
            End If
        End Sub

        Public Function GetCellByInnerText(srcTable As Table, targetString As String) As TableCell
            For Each td As TableCell In srcTable.TableCells
                If td.InnerHtml = targetString Then
                    Return td
                End If
            Next
            Return Nothing
        End Function
    End Module
End Namespace