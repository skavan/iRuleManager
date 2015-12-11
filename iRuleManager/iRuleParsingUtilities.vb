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

        <Serializable>
        Public Class IRuleBasicItemInfo
            Private _base64Data1 As String
            Property Name As String
            Property Type As String
            Property ImageSrc As String
            Property GUID As String
            Property Level As Integer

            Public Function Base64Data() As String
                Dim leftPos As Integer = ImageSrc.IndexOf("base64,") + 7
                If leftPos > 0 Then
                    Return ImageSrc.Substring(leftPos)
                Else
                    Return ""
                End If
            End Function
        End Class

        Public Structure iRuleScanFilter
            Public Type As eTreeSearchOptions
            Public Value As String
            Public Sub New(searchOption As eTreeSearchOptions, value As String)
                Me.Type = searchOption
                Me.Value = value
            End Sub
        End Structure

        '// parent overload to deal with a collection of divs rather than a div
        Public Function ParseTree(outerDivCol As ElementCollection, rootNode As MyTreeNode, filters As iRuleScanFilter()) As MyTreeNode
            For Each nodeDiv As Div In outerDivCol
                rootNode = ParseTree(nodeDiv, rootNode, 1, filters)
            Next
            Return rootNode
        End Function

        '// generic function to parse an iRule Tree Structure
        Public Function ParseTree(outerDiv As Div, rootNode As MyTreeNode, level As Integer, filters As iRuleScanFilter()) As MyTreeNode
            For Each nodeDiv As Div In outerDiv.ChildrenOfType(Of Div)
                '// each nodeDiv (except, normally, the first) contain items in child elements.
                If nodeDiv.Exists Then
                    '// valid node (normal has a table with this element and a div with children)
                    '// or a div with table, if no children.
                    If nodeDiv.Children.Count = 2 Then      'we have children
                        rootNode = ParseTreeNodeWithChildren(nodeDiv, rootNode, level, filters)
                    ElseIf nodeDiv.Tables.Count > 0
                        rootNode = ParseTreeNodeNoChildren(nodeDiv, rootNode, level, filters)
                    End If
                End If
            Next
            Return rootNode
        End Function

        '// use this function when the node has children
        Private Function ParseTreeNodeWithChildren(nodeDiv As Div, rootNode As MyTreeNode, level As Integer, filters As iRuleScanFilter()) As MyTreeNode
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
            '// now it's time to attack the children, if any.
            If nodeDiv.Divs.Count > 0 Then
                For Each childDiv In nodeDiv.ChildrenOfType(Of Div)
                    ParseTree(childDiv, newNode, level + 1, filters)
                Next
            End If
            Return rootNode
        End Function

        '// use this function when the node has no children
        Private Function ParseTreeNodeNoChildren(nodeDiv As Div, rootNode As MyTreeNode, level As Integer, filters As iRuleScanFilter()) As MyTreeNode
            Dim itemTable As Table = nodeDiv.Tables(0)
            Dim itemInfo As IRuleBasicItemInfo = GetHighLevelItemInfo(itemTable, level)
            If Not ProcessItemFurther(itemInfo, filters) Then Return rootNode
            '// lets put all this in a node
            Dim newNode As MyTreeNode = AddNodeWithGUID(nodeDiv, rootNode, itemInfo, True)
            Return rootNode
        End Function

        Private Function ProcessItemFurther(itemInfo As IRuleBasicItemInfo, filters As iRuleScanFilter()) As Boolean
            '// if there are no filters, return true (i.e. process further)
            If filters Is Nothing Then Return True
            '// if there are filters
            For Each filter As iRuleScanFilter In filters
                Select Case filter.Type
                    Case eTreeSearchOptions.ExcludeGroup
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
        Public Function BuildScanFilters(excludegroup As String) As iRuleScanFilter()
            Return BuildScanFilters("", "", excludegroup, "", 0)
        End Function

        '// Build the filters used to optimize Tree Scanning
        Public Function BuildScanFilters(panel As String, group As String, page As String, level As Integer) As iRuleScanFilter()
            Return BuildScanFilters(panel, group, "", page, level)
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

        '// scan the relevant element and look for a non-hidden icon
        Public Function FindActiveImageByTitleName(parentDiv As Div, targetString As String) As Image
            For Each potentialImage As Image In parentDiv.Images.Filter(Find.ByTitle(targetString))
                If potentialImage.Exists Then Return potentialImage
            Next
            Return Nothing
        End Function

        Public Function FindDivContainingText(source As Div, targetText As String) As Div
            For Each tabDiv As Div In source.ChildrenOfType(Of Div)
                If tabDiv.Text.Trim = targetText Then Return tabDiv
            Next
            Return Nothing
        End Function
    End Module

End Namespace