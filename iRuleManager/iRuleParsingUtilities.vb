Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml.Serialization
Imports WatiN.Core
Namespace Manager
    Public Enum eTreeSearchOptions
        None
        StopAtLevel
        RestrictToPanel
        RestrictToGroup
        RestrictToPage
        ExcludeGroup
    End Enum

    Public Module iRuleParsingUtilities
        Private _leftPanel As Div
        Private _tableProps As Table

        '// When we go deep, we need to set the Left Hand Panel
        Public Sub SetLeftPanel(lhp As Div)
            _leftPanel = lhp
        End Sub

#Region "Tree Scanning Functions"
        '// a structure for managing ScanFilters
        Public Structure iRuleScanFilter
            Public Type As eTreeSearchOptions
            Public Value As String
            Public Sub New(searchOption As eTreeSearchOptions, value As String)
                Me.Type = searchOption
                Me.Value = value
            End Sub
        End Structure

        '// parent overload to deal with a collection of divs rather than a div
        Public Function ParseTree(outerDivCol As ElementCollection, rootNode As MyTreeNode, bDoDeepScan As Boolean, filters As iRuleScanFilter()) As MyTreeNode
            For Each nodeDiv As Div In outerDivCol
                rootNode = ParseTree(nodeDiv, rootNode, 1, bDoDeepScan, filters)
            Next
            Return rootNode
        End Function

        '// the main (generic) function to parse an iRule Tree Structure
        Public Function ParseTree(outerDiv As Div, rootNode As MyTreeNode, level As Integer, bDoDeepScan As Boolean, filters As iRuleScanFilter()) As MyTreeNode
            For Each nodeDiv As Div In outerDiv.ChildrenOfType(Of Div)
                '// each nodeDiv (except, normally, the first) contain items in child elements.
                If nodeDiv.Exists Then
                    '// valid node (normal has a table with this element and a div with children)
                    '// or a div with table, if no children.
                    If nodeDiv.Children.Count = 2 Then      'we have children
                        rootNode = ParseTreeNodeWithChildren(nodeDiv, rootNode, level, bDoDeepScan, filters)
                    ElseIf nodeDiv.Tables.Count > 0
                        rootNode = ParseTreeNodeNoChildren(nodeDiv, rootNode, level, bDoDeepScan, filters)
                    End If
                End If
            Next
            Return rootNode
        End Function

        '// use this function when the node has children
        '// at some point to-do merge these two (with and without children)
        Private Function ParseTreeNodeWithChildren(nodeDiv As Div, rootNode As MyTreeNode, level As Integer, bDoDeepScan As Boolean, filters As iRuleScanFilter()) As MyTreeNode
            '// typically looks like:
            '// child 0 : <table>..<td><img expand/contract></td><td><table><td><img icon></td><td>Label</td></table></td></table>
            '// child 1 : <div>..<children div(s)...</div> --> fo rfurther processing
            Dim outerTable As Table = nodeDiv.Tables(0)

            '// then a subtable contains the item icon in first cell and the item name in second cell
            '// unless it's childless
            Dim itemTable As Table = outerTable.TableCells(1).Tables(0)
            Dim itemInfo As IRuleBasicItemInfo = GetHighLevelItemInfo(itemTable, level)

            If Not ProcessItemFurther(itemInfo, filters) Then Return rootNode

            '// first item is expand/contract image
            '----- to do ----- expand the item if required!
            Dim expandContractImage As Image = outerTable.TableCells(0).Children(0)
            expandContractImage.UIEvent("mousedown")

            '// lets put all this in a node
            Dim newNode As MyTreeNode = AddNodeWithGUID(nodeDiv, rootNode, itemInfo, True)
            '// now should we process the DeepScan?
            If bDoDeepScan Then
                If SelectPanelItem(itemTable) IsNot Nothing Then
                    newNode.Data = ReadItemProperties(itemInfo)
                End If
            End If



            '// now it's time to attack the children, if any.
            If nodeDiv.Divs.Count > 0 Then
                For Each childDiv In nodeDiv.ChildrenOfType(Of Div)
                    ParseTree(childDiv, newNode, level + 1, bDoDeepScan, filters)
                Next
            End If
            Return rootNode
        End Function

        '// use this function when the node has no children
        Private Function ParseTreeNodeNoChildren(nodeDiv As Div, rootNode As MyTreeNode, level As Integer, bDoDeepScan As Boolean, filters As iRuleScanFilter()) As MyTreeNode
            Dim itemTable As Table = nodeDiv.Tables(0)
            Dim itemInfo As IRuleBasicItemInfo = GetHighLevelItemInfo(itemTable, level)
            If Not ProcessItemFurther(itemInfo, filters) Then Return rootNode
            '// lets put all this in a node
            Dim newNode As MyTreeNode = AddNodeWithGUID(nodeDiv, rootNode, itemInfo, True)
            '// now should we process the DeepScan?
            If bDoDeepScan Then
                If SelectPanelItem(itemTable) IsNot Nothing Then
                    newNode.Data = ReadItemProperties(itemInfo)
                End If
            End If
            Return rootNode
        End Function

        '// using the filters, decide whether to process this item further
        Private Function ProcessItemFurther(itemInfo As IRuleBasicItemInfo, filters As iRuleScanFilter()) As Boolean
            '// if there are no filters, return true (i.e. process further)
            If filters Is Nothing Then Return True
            '// if there are filters
            For Each filter As iRuleScanFilter In filters
                Select Case filter.Type
                    Case eTreeSearchOptions.ExcludeGroup
                        If itemInfo.Name = "Actions" Then
                            Debug.Print("")
                        End If
                        If itemInfo.Level = 0 Then
                            For Each value In filter.Value.Split("|")
                                If itemInfo.Name = value Then Return False
                            Next
                            '// if not in the exclude group, Process it
                            Return True
                        End If
                    Case eTreeSearchOptions.RestrictToPanel
                        If itemInfo.Level = 0 Then
                            If itemInfo.Name = filter.Value Then Return True Else Return False
                        End If
                    Case eTreeSearchOptions.RestrictToGroup
                        If itemInfo.Level = 1 Then
                            If itemInfo.Name = filter.Value Then Return True Else Return False
                        End If
                    Case eTreeSearchOptions.RestrictToPage
                        If itemInfo.Level = 2 Then
                            If itemInfo.Name = filter.Value Then Return True Else Return False
                        End If
                    Case eTreeSearchOptions.StopAtLevel
                        If itemInfo.Level = filter.Value Then Return False Else Return True
                End Select
            Next
            '// if we didn't hit any filter, return True
            Return True
        End Function

        '// get the basic info about an item
        Private Function GetHighLevelItemInfo(itemTable As Table, level As Integer) As IRuleBasicItemInfo
            Dim itemInfo As New IRuleBasicItemInfo
            Dim img As Image = itemTable.TableCells(0).Children(0)
            itemInfo.ImageSrc = img.Src
            itemInfo.Name = itemTable.TableCells(1).Children(0).Text
            itemInfo.Type = GetItemType(itemTable)
            itemInfo.Level = level
            'Debug.Print(".Add ({0}{1}{0})", Chr(34), itemInfo.Name)
            Return itemInfo
        End Function

        '// Build the filters used to optimize Tree Scanning
        Public Function BuildScanFilters(panel As String, group As String, excludegroup As String, page As String, level As Integer) As iRuleScanFilter()
            Dim filters(-1) As iRuleScanFilter
            If panel <> "" Then     '// i.e. Main
                Dim filter As iRuleScanFilter = New iRuleScanFilter(eTreeSearchOptions.RestrictToPanel, panel)
                filters.Add(filter)
            End If
            If group <> "" Then     '// i.e. Landscape Pages
                Dim filter As iRuleScanFilter = New iRuleScanFilter(eTreeSearchOptions.RestrictToGroup, group)
                filters.Add(filter)
            End If
            If excludegroup <> "" Then      '// i.e. Home
                Dim filter As iRuleScanFilter = New iRuleScanFilter(eTreeSearchOptions.ExcludeGroup, excludegroup)
                filters.Add(filter)
            End If
            If page <> "" Then      '// i.e. Home
                Dim filter As iRuleScanFilter = New iRuleScanFilter(eTreeSearchOptions.RestrictToPage, page)
                filters.Add(filter)
            End If
            If level > 0 Then       '// i.e. 4
                Dim filter As iRuleScanFilter = New iRuleScanFilter(eTreeSearchOptions.StopAtLevel, level)
                filters.Add(filter)
            End If
            Return filters
        End Function

        '// convenience overload
        Public Function BuildScanFilters(excludegroup As String, level As Integer) As iRuleScanFilter()
            Return BuildScanFilters("", "", excludegroup, "", level)
        End Function

        '// Build the filters used to optimize Tree Scanning
        Public Function BuildScanFilters(panel As String, group As String, page As String, level As Integer) As iRuleScanFilter()
            Return BuildScanFilters(panel, group, "", page, level)
        End Function

#End Region

        '// Note the rootNode is by Ref and gets updated that way. The new node is returned.
        '// We also assign a GUID if a unique ID is not present. We also push the basicInfo into the MyTreeNode
        Private Function AddNodeWithGUID(ByRef nodeDiv As Div, ByRef rootNode As MyTreeNode, ByRef itemInfo As IRuleBasicItemInfo, bAssignGUID As Boolean) As MyTreeNode
            Dim newNode As New MyTreeNode(itemInfo.Name)
            If bAssignGUID Then
                If nodeDiv.Id = "" Then
                    nodeDiv.Id = Guid.NewGuid.ToString
                End If
                newNode.ID = nodeDiv.Id
                itemInfo.GUID = newNode.ID
                newNode.ToolTipText = "GUID:" & newNode.ID & "|" & itemInfo.Type & "|" & itemInfo.Level
                newNode.BasicInfo = itemInfo
            End If
            rootNode.Nodes.Add(newNode)
            Return newNode
        End Function

        '// We need to extract the item type out of the table that contains the item.
        Private Function GetItemType(table As Table) As String
            Dim info As String = table.Title
            Dim itemType As String = ""
            If info Is Nothing Then info = ""
            If info.Contains(":") Then
                itemType = info.Split(":")(0)
            Else
                '// this is designed to handle the right hand panel of widgets!
                If info = "" Then
                    itemType = table.TableCells(1).Children(0).Text
                End If
            End If
            Return itemType
        End Function

        '// Make a deep copy of an object
        Public Function DeepClone(src As IRuleBasicItemInfo) As IRuleBasicItemInfo
            Dim dest As New IRuleBasicItemInfo
            With dest
                .GUID = ""
                .ImageSrc = src.ImageSrc
                .Level = src.Level
                .Name = src.Name
                .Type = src.Type
            End With

            Return dest
        End Function

        '// Append a widget name to the specified dictionary and return the updated set
        Public Function AppendWidgetBasedOn(widgetListByName As Dictionary(Of String, IRuleBasicItemInfo), sourcename As String, targetname As String) As Dictionary(Of String, IRuleBasicItemInfo)
            '// get a deep clone of a button.
            Dim widget As IRuleBasicItemInfo = DeepClone(widgetListByName(sourcename))
            '// change name and type
            widget.GUID = "" : widget.Name = targetname : widget.Type = targetname
            '// add it anew
            widgetListByName.Add(widget.Name, widget)
            Return widgetListByName
        End Function

        '// given a classname, find all that match and find the one that is active.
        Function FindActiveDivByClassName(parentDiv As Div, className As String) As Div
            For Each potentialDiv As Div In parentDiv.Divs.Filter(Find.ByClass(className))
                If potentialDiv.Exists Then Return potentialDiv
            Next
            Return Nothing
        End Function

        Function FindTableByClassName(parentDiv As Div, className As String) As Table
            Dim foundTable As Table = parentDiv.Tables.Filter(Find.ByClass(className)).First
            Return foundTable
        End Function

        '// scan the relevant element and look for a non-hidden icon
        Public Function FindActiveImageByTitleName(parentDiv As Div, targetString As String) As Image
            For Each potentialImage As Image In parentDiv.Images.Filter(Find.ByTitle(targetString))
                If potentialImage.Exists Then Return potentialImage
            Next
            Return Nothing
        End Function

        '// track down a Div that contains a text string
        Public Function FindDivContainingText(source As Div, targetText As String) As Div
            For Each tabDiv As Div In source.ChildrenOfType(Of Div)
                If tabDiv.Text.Trim = targetText Then Return tabDiv
            Next
            Return Nothing
        End Function

        '// Perform the deepScan and read an items Properties from the Property Table
        Private Function ReadItemProperties(itemInfo As IRuleBasicItemInfo) As Dictionary(Of String, String)
            RefreshTableProps()
            Dim dicKeyValues As New Dictionary(Of String, String)
            dicKeyValues.Add("itemName", itemInfo.Name)
            dicKeyValues.Add("itemType", itemInfo.Type)
            'lstKVP.Add(New KeyValuePair(Of String, String)("itemMacro", itemInfo.XtraInfo))
            'lstKVP.Add(New KeyValuePair(Of String, String)("itemParentPanel", itemInfo.ParentPanel))
            dicKeyValues.Add("itemID", itemInfo.GUID)

            For Each tableRow As TableRow In _tableProps.TableRows
                Dim nameCell = tableRow.TableCell(Find.ByClass("nameCell"))
                Dim valueCell = tableRow.TableCell(Find.ByClass("valueCell"))

                '// read the element data
                Dim fldName As String = ReadPropertyName(nameCell)
                Dim fldValue As String = ReadPropertyValue(valueCell)

                dicKeyValues.Add(fldName, fldValue)
            Next
            Return dicKeyValues
        End Function

        '// read the name of the Property Element
        Private Function ReadPropertyName(nameCell As TableCell) As String
            If nameCell.Exists Then
                Dim nameDiv As Div = nameCell.Children(0)

                '// inspect the div, because the div we want might be nested, one level down
                If nameDiv.Children.Count > 0 Then
                    nameDiv = nameDiv.Children(0)
                End If
                '// now get the name of the field
                Return nameDiv.InnerHtml
            Else
                Return ""
            End If
        End Function

        '// read the value of different type of Property Element Items from the ValueCell
        Private Function ReadPropertyValue(valueCell As TableCell) As String
            Dim childEl As Element = valueCell.Children(0)
            Dim fldValue As String = ""
            Select Case childEl.TagName
                Case "INPUT"
                    fldValue = childEl.Text
                Case "SELECT"
                    fldValue = childEl.GetAttributeValue("value")
                Case "DIV" '(could be font structure or variable structure)
                    '//div1 class="gwt-HTML variable"
                    '//div2 class="gwt-Label"
                    Dim div0 As Div = childEl
                    Dim div1 As Div = Nothing
                    Dim div2 As Div = Nothing
                    If div0.Children.Count = 1 Then
                        '// this is probably the font class.
                        div1 = div0.Divs.Last
                        fldValue = div1.Text
                    Else
                        '// this is the variable+textfield case
                        div1 = div0.Children(0)
                        div2 = div0.Children(1)
                        fldValue = div1.Text & "|" & div2.Text
                    End If

            End Select
            Return fldValue
        End Function

        '// write a property element, based on type.
        Private Sub WritePropertyElement(valueCell As TableCell, NewValue As String)
            Dim childEl As Element = valueCell.Children(0)
            Select Case childEl.TagName
                Case "INPUT"
                    'fldValue = childEl.Text
                    Dim inp As TextField = childEl
                    inp.UIEvent("mousedown")
                    inp.TypeTextQuickly(NewValue.ToString)
                    inp.UIEvent("change")
                Case "SELECT"
                    Dim sel As WatiN.Core.SelectList = childEl
                    sel.UIEvent("mousedown")
                    sel.SetAttributeValue("value", NewValue)
                    sel.UIEvent("change")
                'fldValue = childEl.Text
                Case "DIV" '(could be font structure or variable structure)
                    '//div1 class="gwt-HTML variable"
                    '//div2 class="gwt-Label"
                    Dim div0 As Div = childEl
                    Dim div1 As Div = Nothing
                    Dim div2 As Div = Nothing
                    Dim div3 As Div = Nothing
                    If div0.Children.Count = 1 Then
                        '// this is probably the font class.
                        div1 = div0.Divs.Last
                        div1.UIEvent("mousedown")
                        div1.SetAttributeValue("value", NewValue)
                        div1.UIEvent("change")
                        'fldValue = div1.Text
                    Else
                        '// this is the variable+textfield case
                        div1 = div0.Children(0)     '//variable name
                        div2 = div0.Children(1)     '//Default Value
                        'div3 = valueCell.NextSibling     '//Popup menu

                        Dim popCell As TableCell = valueCell.NextSibling
                        'popCell.UIEvent("mousedown")   '//FirethePopup
                        popCell.Images(0).UIEvent("click")
                        'Dim dc As DomContainer = _leftPanel.DomContainer
                        Dim popupTable As Table = GetPopupMenu(_leftPanel.DomContainer)
                        '// get the table inside the table that contains the value set
                        Dim tblValues As Table = popupTable.OwnTableRows(0).OwnTableCells(1).Children(0)

                        'Dim tbody As TableBody = popupTable.Tables(1).TableBody(0)
                        '// get the input for the variable field and the value/default field
                        Dim inpVars As TextField = tblValues.TableRows(0).TextFields(0)
                        Dim inpDefault As TextField = tblValues.TableRows(1).TextFields(0)
                        inpDefault.UIEvent("mousedown")
                        Dim adjustedValue As String = GetSplitValueItem(NewValue, 1, True)

                        inpDefault.SetAttributeValue("value", adjustedValue)
                        inpDefault.UIEvent("change")
                        childEl.UIEvent("mousedown")
                        'fldValue = div1.Text & "|" & div2.Text
                    End If

            End Select
        End Sub

        Public Function GetSplitValueItem(splitValue As String, elementNumber As Integer, bCleanString As Boolean) As String
            If splitValue.Contains("|") Then
                Dim item As String = splitValue.Split("|")(elementNumber)
                If bCleanString Then
                    item = ((item.TrimStart("[")).TrimEnd("]")).Trim
                End If
                Return item
            Else
                '// Nothing to do
                Return splitValue
            End If
        End Function

        Public Function SetSplitValueItem(splitValue As String, elementNumber As Integer, value As String, wrap() As String) As String

            If splitValue.Contains("|") Then
                If wrap IsNot Nothing Then
                    value = wrap(0) & value & wrap(1)
                End If
                splitValue.Split("|")(elementNumber) = value
                Dim arrSplit() As String = splitValue.Split("|")
                arrSplit(elementNumber) = value
                Dim newValue As String = ""
                For Each el As String In arrSplit
                    newValue = newValue & el & "|"
                Next
                newValue = newValue.TrimEnd("|")
                Return newValue
            Else
                If splitValue <> "" Then
                    'If wrap IsNot Nothing Then
                    '    value = wrap(0) & value & wrap(1)
                    'End If
                    'splitValue = splitValue & "|" & value
                    splitValue = value
                Else
                    '// no wrap in this case.
                    splitValue = value
                End If
            End If
            Return splitValue
        End Function

        '// refresh the Table with the elemet properties -- if needed.
        Private Sub RefreshTableProps()
            '// long winded -- but only want to refresh tableProps if we have to.
            If _tableProps IsNot Nothing Then
                If _tableProps.Exists = False Then
                    _tableProps = GetTableProps(_leftPanel)
                End If
            Else
                _tableProps = GetTableProps(_leftPanel)
            End If
        End Sub

        '// Get the properties table
        Private Function GetTableProps(leftPanel As Div) As Table
            For Each subDiv As Div In leftPanel.Children
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
                    panName.UIEvent("mousedown")
                    Return panName
                End If
            End If
            Return Nothing
        End Function

        Public Function FindElement(source As Div, id As String, doClick As Boolean) As Div
            '// we need to find the element, 
            '// we need a function to walk up the tree And open the page
            If id Is Nothing Then Return Nothing
            Dim targetElement As Div = source.Div(id)
            Dim targetTable As Table = targetElement.Tables(0)

            If targetElement.Children.Count = 2 Then      'we have children so the actual table we wnat is further embedded.

                targetTable = targetTable.TableCells(1).Tables(0)
            Else
                'targetTable = targetElement.Tables(0)

            End If
            SelectPanelItem(targetTable)     '// the actual item should be in the first table
            'targetElement.UIEvent("mousedown")
            Return targetElement
        End Function

        Public Function UpdatePropertyTable(source As Div, node As MyTreeNode) As Div
            Dim thisDiv As Div = FindElement(source, node.BasicInfo.GUID, True)
            RefreshTableProps()
            '// for each property row...had to use index, because we need to refresh the table after a write and
            '// a ""for each" seems to lose its place."
            For i As Integer = 0 To _tableProps.TableRows.Count - 1
                Dim tableRow As TableRow = _tableProps.TableRows(i)
                Dim nameCell As TableCell = tableRow.TableCell(Find.ByClass("nameCell"))
                Dim valueCell As TableCell = tableRow.TableCell(Find.ByClass("valueCell"))
                Dim fldName As String = ReadPropertyName(nameCell)
                Dim fldValue As String = ReadPropertyValue(valueCell)
                'now find this element in the dictionary
                Dim mydic As Dictionary(Of String, String) = node.Data
                If mydic IsNot Nothing Then
                    Dim newValue = mydic(fldName)
                    If newValue <> fldValue Then
                        WritePropertyElement(valueCell, newValue)
                        RefreshTableProps() '//important after a write
                    End If
                End If
            Next
            Return Nothing
        End Function

        Public Function ChangeElementData(source As Div, id As String) As Div
            Dim thisDiv As Div = FindElement(source, id, True)
            RefreshTableProps()

            For Each tableRow As TableRow In _tableProps.TableRows
                Dim nameCell As TableCell = tableRow.TableCell(Find.ByClass("nameCell"))
                Dim valueCell As TableCell = tableRow.TableCell(Find.ByClass("valueCell"))

                If nameCell.Exists = False Then
                    Continue For
                End If

                Dim nameDiv As Div = nameCell.Children(0)
                '// inspect the div, because the div we want might be nested, one level down
                If nameDiv.Children.Count > 0 Then
                    nameDiv = nameDiv.Children(0)
                End If
                '// now get the name of the field
                Dim fldName As String = nameDiv.InnerHtml
                '// the fldValue might be the Title or the Value. .... "mmmh...need to review this"
                Dim fldValue As String = valueCell.Children(0).Title
                If fldValue = "" Then
                    fldValue = valueCell.Children(0).GetAttributeValue("value")
                End If

                '// is it a variable driven element?
                '// if so, call popup, and 
                '// enter data
                ''// valuecell is the td
                '// if its an "gwt-Textbox" INPUT child, then if type=text, its a regular TEXTFIELD
                '// if its a "gwt-Listbox" SELECT child, then its a dropdown box, but I think we can treat it as a text field
                '// if its a DIV child, then it's probably variable and popup driven
                '// there's a possibility that we could have 
                Dim childEl As Element = valueCell.Children(0)
                Select Case childEl.TagName
                    Case "INPUT"
                        fldValue = childEl.Text
                    Case "SELECT"
                        fldValue = childEl.Text
                    Case "DIV"
                        '//div1 class="gwt-HTML variable"
                        '//div2 class="gwt-Label"
                        Dim div1 As Div = DirectCast(childEl, Div).Children(0)
                        Dim div2 As Div = DirectCast(childEl, Div).Children(1)
                        fldValue = div1.Text & "|" & div2.Text
                End Select
                Debug.Print(fldName & ":" & fldValue)
            Next
            Return Nothing


            'Select Case fldName
            '        Case "column"
            '            Dim c As Integer = fldValue - 100
            '            Dim inp As TextField = valueCell.Children(0)            'input type="text" is the most common element type
            '            If inp IsNot Nothing Then

            '                'inp.PressEnter
            '                'SendKeys.SendWait("{TAB}")
            '                'SendKeys.SendWait("+{TAB}")
            '                'For Each ic As Char In c.ToString
            '                '    SendKeys.SendWait(ic)
            '                'Next
            '                'SendKeys.SendWait("{TAB}")

            '                inp.UIEvent("mousedown")
            '                inp.TypeTextQuickly(c.ToString)
            '                'inp.TypeTextAction.TypeText(c.ToString)
            '                inp.UIEvent("change")

            '            End If
            '        Case "name"
            '            If valueCell.Children(0).TagName = "INPUT" Then
            '                '// regular field
            '            ElseIf valueCell.Children(0).TagName = "DIV" Then
            '                Dim subDiv As Div = valueCell.Children(0)
            '                Dim subNameDiv As Div = subDiv.Children(1)

            '                Dim cval As String = subNameDiv.Text.TrimStart("[ ")
            '                cval = cval.TrimEnd("]").Trim
            '                cval = "[ " & cval & "01" & " ]"
            '                subNameDiv.UIEvent("mousedown")
            '                subNameDiv.SetAttributeValue("innerHTML", cval)
            '                subNameDiv.UIEvent("change")
            '                subDiv.UIEvent("change")
            '            Else
            '                '// something else

            '            End If

            '    End Select


        End Function

        Public Function GetPopupMenu(ie As DomContainer) As Table
            'The popupMenu is added as the final DIV of the body of the page
            Dim popupMenu As Div = ie.Body.Children.Last
            'popupMenu.UIEvent("contextmenu")

            '// no wgrab the Table containing the popup menu and find the target cell that we need to click.
            Return popupMenu.Tables.First

        End Function

        Public Function GetHandsetNode(leftPanel As Div, bDoDeepScan As Boolean) As MyTreeNode
            _leftPanel = leftPanel
            Dim handsetTable As Table = FindTableByClassName(_leftPanel, "irule-HandsetsManagerViewElementItem")

            If handsetTable.Exists = False Then
                Return Nothing
            End If
            handsetTable.UIEvent("mousedown")
            Dim node As New MyTreeNode(handsetTable.Title)
            Dim itemInfo As New IRuleBasicItemInfo
            itemInfo.Name = handsetTable.Title
            itemInfo.Type = "Handset"
            itemInfo.Level = 0
            node.BasicInfo = itemInfo
            If bDoDeepScan Then node.Data = ReadItemProperties(itemInfo)
            Return node
        End Function
    End Module

End Namespace