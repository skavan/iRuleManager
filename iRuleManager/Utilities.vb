Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml.Serialization

Namespace Manager


    Public Module Utilities
        '// serialise a Dictionary to XML
        Public Sub WriteToFile(file As String, dic As Dictionary(Of String, IRuleBasicItemInfo))
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
        Public Function ReadFromFile(file As String) As Dictionary(Of String, IRuleBasicItemInfo)
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
