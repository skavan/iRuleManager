Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml.Serialization

Namespace Manager


    Public Module Utilities
        '// the basic info class
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

        <Serializable>
        Public Class iRuleTemplate
            Public DeviceData As New Dictionary(Of String, String)
            Public GroupData As New Dictionary(Of String, String)
            Public PageData As New Dictionary(Of String, String)
            Public Widgets As New List(Of WidgetData)
        End Class

        <Serializable>
        Public Class WidgetData
            Public BasicInfo As IRuleBasicItemInfo
            Public Data As List(Of NameValue)
        End Class

        Public Sub WriteiRuleTemplate(node As MyTreeNode, file As String)
            Dim lstWidgets As New List(Of WidgetData)
            lstWidgets = BuildTemplate(node, lstWidgets)
            Serialize(Of List(Of WidgetData))(lstWidgets, file)
        End Sub

        '// Read the template file and reassemble the node Tree
        Public Function ReadiRuleTemplate(file As String) As MyTreeNode
            Dim lstWidgets As List(Of WidgetData) = Deserialize(Of List(Of WidgetData))(file)
            If lstWidgets Is Nothing Then Return Nothing
            Dim rootNode As New MyTreeNode("root")

            '// Special Treatment for the root.
            rootNode = CopyListToNode(rootNode, lstWidgets(0))

            Dim thisLevel As Integer = -1
            Dim previousLevel As Integer = -1
            Dim currentNode As MyTreeNode = rootNode

            For i = 1 To lstWidgets.Count - 1
                Dim item As WidgetData = lstWidgets(i)
                Dim thisNode As New MyTreeNode(item.BasicInfo.Name)
                thisNode = CopyListToNode(thisNode, item)
                '// Each node is either a sibling, a child or a parent (maybe >1 level)
                '// Act accordingly
                Select Case item.BasicInfo.Level
                    Case previousLevel
                        currentNode = currentNode.Parent
                        currentNode.Nodes.Add(thisNode)
                        previousLevel = item.BasicInfo.Level
                        currentNode = thisNode
                    Case > previousLevel
                        currentNode.Nodes.Add(thisNode)
                        currentNode = thisNode
                        previousLevel = item.BasicInfo.Level
                    Case < previousLevel
                        For j As Integer = item.BasicInfo.Level To previousLevel
                            currentNode = currentNode.Parent
                        Next
                        currentNode.Nodes.Add(thisNode)
                        previousLevel = item.BasicInfo.Level
                        currentNode = thisNode
                End Select
                Dim hashkey As String = CalculateMD5Hash(thisNode.BasicInfo.ImageSrc)
                'If Not ImageList.Images.ContainsKey(hashkey) Then
                '    Dim img As System.Drawing.Image = CreateImageFromBase64(thisNode.BasicInfo.Base64Data)
                '    ImageList.Images.Add(hashkey, img)
                'End If
                thisNode.ImageKey = hashkey
            Next
            Return rootNode
        End Function

        Private Function CopyListToNode(node As MyTreeNode, widget As WidgetData) As MyTreeNode
            If widget.BasicInfo IsNot Nothing Then
                node.BasicInfo = widget.BasicInfo
                node.Data = ListToDictionary(widget.Data)
                node.Text = widget.BasicInfo.Name
            End If
            Return node
        End Function
        Private Function BuildTemplate(node As MyTreeNode, lst As List(Of WidgetData)) As List(Of WidgetData)
            Dim item As New WidgetData
            item.BasicInfo = node.BasicInfo
            item.Data = DictionarytoList(node.Data)
            lst.Add(item)
            For Each n As MyTreeNode In node.Nodes
                lst = BuildTemplate(n, lst)
            Next
            Return lst
        End Function

        ''' <summary>
        ''' Convert a class state into XML
        ''' </summary>
        ''' <typeparam name="T">The type of object</typeparam>
        ''' <param name="obj">The object to serilize</param>
        ''' <param name="sConfigFilePath">The path to the XML</param>
        Public Sub Serialize(Of T)(ByVal obj As T, sConfigFilePath As String)
            Dim XmlBuddy As New System.Xml.Serialization.XmlSerializer(GetType(T))
            Dim MySettings As New System.Xml.XmlWriterSettings()
            MySettings.Indent = True
            MySettings.CloseOutput = True
            Dim MyWriter As System.Xml.XmlWriter = System.Xml.XmlWriter.Create(sConfigFilePath, MySettings)
            XmlBuddy.Serialize(MyWriter, obj)
            MyWriter.Flush()
            MyWriter.Close()
        End Sub


        ''' <summary>
        ''' Restore a class state from XML
        ''' </summary>
        ''' <typeparam name="T">The type of object</typeparam>
        ''' <param name="xml">the path to the XML</param>
        ''' <returns>The object to return</returns>
        Public Function Deserialize(Of T)(ByVal xml As String) As T
            Dim XmlBuddy As New System.Xml.Serialization.XmlSerializer(GetType(T))
            Dim fs As New FileStream(xml, FileMode.Open)
            Dim reader As New Xml.XmlTextReader(fs)
            If XmlBuddy.CanDeserialize(reader) Then
                Dim tempObject As Object = DirectCast(XmlBuddy.Deserialize(reader), T)
                reader.Close()
                Return tempObject
            Else
                Return Nothing
            End If
        End Function

        '// Convert a list of NameValues to a Dictionary
        Private Function ListToDictionary(entries As List(Of NameValue)) As Dictionary(Of String, String)
            Dim dic As New Dictionary(Of String, String)
            For Each item In entries
                dic.Add(item.Key, item.Value)
            Next
            Return dic
        End Function

        '// Convert a Dictionary (of key, value) to a List of NameValues
        Public Function DictionarytoList(dic As Dictionary(Of String, String)) As List(Of NameValue)
            Dim entries As New List(Of NameValue)
            If dic Is Nothing Then Return Nothing
            For Each key As String In dic.Keys
                Dim dicen As New NameValue With {.Key = key, .Value = dic(key)}
                entries.Add(dicen)
            Next
            Return entries
        End Function

        '// serialise a Dictionary to XML
        Public Sub WriteDictionaryToFile(file As String, dic As Dictionary(Of String, IRuleBasicItemInfo))
            Dim entries As New List(Of DictionaryEntry)
            Dim ser As XmlSerializer = New XmlSerializer(GetType(List(Of DictionaryEntry)))
            For Each key As String In dic.Keys
                Dim dicen As New DictionaryEntry With {.Key = key, .Value = dic(key)}

                entries.Add(dicen)
            Next
            Dim writer As TextWriter = New StreamWriter(file)
            ser.Serialize(writer, entries)
        End Sub
        '// deserialize from xml
        Public Function ReadDictionaryFromFile(file As String) As Dictionary(Of String, IRuleBasicItemInfo)
            Dim ser As XmlSerializer = New XmlSerializer(GetType(List(Of DictionaryEntry)))
            Dim reader As TextReader = New StreamReader(file)
            Dim entries As List(Of DictionaryEntry) = ser.Deserialize(reader)
            '// linq version.
            'Dim dic As Dictionary(Of String, IRuleBasicItemInfo) = entries.ToDictionary(Function(entry) entry.Key, Function(entry) entry.Value)
            Dim dic As New Dictionary(Of String, IRuleBasicItemInfo)
            For Each entry As DictionaryEntry In entries
                dic.Add(entry.Key, entry.Value)
            Next
            Return dic
        End Function
        Public Class NameValue
            Property Key As String
            Property Value As String
        End Class
        '// helper class for serialization
        <Serializable>
        Public Class DictionaryEntry
            Property Key As String
            Property Value As IRuleBasicItemInfo
            'Public Sub Entry(key As String, value As IRuleBasicItemInfo)
            '    Me.Key = key
            '    Me.Value = value
            'End Sub
        End Class

        '// given a string, compute a MD5 Hash
        Public Function CalculateMD5Hash(input As String) As String
            ' step 1, calculate MD5 hash from input
            Dim md5 As MD5 = System.Security.Cryptography.MD5.Create()
            Dim inputBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(input)
            Dim hash As Byte() = md5.ComputeHash(inputBytes)

            ' step 2, convert byte array to hex string
            Dim sb As New StringBuilder()
            For i As Integer = 0 To hash.Length - 1
                sb.Append(hash(i).ToString("X2"))
            Next
            Return sb.ToString()
        End Function

        '// Given a base64 string, make an image and return it.
        Public Function CreateImageFromBase64(base64String As String) As System.Drawing.Image
            'get a temp image from bytes, instead of loading from disk
            'data:image/gif;base64,
            'this image is a single pixel (black)
            Dim bytes As Byte() = Convert.FromBase64String(base64String)

            Dim image__1 As System.Drawing.Image
            Using ms As New MemoryStream(bytes)
                image__1 = System.Drawing.Image.FromStream(ms)
            End Using

            Return image__1
        End Function
    End Module

End Namespace
