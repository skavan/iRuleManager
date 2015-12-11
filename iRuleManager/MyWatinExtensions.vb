
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports WatiN.Core

Public Module MyWatinExtensions
    <DllImport("user32.dll")>
    Private Function SetFocus(hWnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Function SetForegroundWindow(hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <Runtime.CompilerServices.Extension>
    Public Sub TypeTextQuickly(textField As TextField, text As String)
        textField.SetAttributeValue("value", text)
    End Sub

    <System.Runtime.CompilerServices.Extension>
    Public Sub UIEvent(imageField As TextField)
        SetForegroundWindow(imageField.DomContainer.hWnd)
        SetFocus(imageField.DomContainer.hWnd)
        imageField.Focus()
        System.Windows.Forms.SendKeys.SendWait("{ENTER}")
        Thread.Sleep(1000)
    End Sub

    <System.Runtime.CompilerServices.Extension>
    Public Sub UIEvent(elem As Element, eventName As String, Optional button As Integer = 0)
        'SetForegroundWindow(elem.DomContainer.hWnd)
        'SetFocus(elem.DomContainer.hWnd)
        DispatchMouseEvent(elem, eventName, button)
        elem.WaitForComplete()
    End Sub

    '// performs a mousedown event on an element. pass in the watin element and the eventname i.e. "mousedown"
    Private Function DispatchMouseEvent(elem As Element, eventname As String, button As Integer) As Boolean
        Dim script As New StringBuilder

        Dim x As Integer = elem.NativeElement.GetElementBounds.X + (elem.NativeElement.GetElementBounds.Width / 2)
        Dim y As Integer = elem.NativeElement.GetElementBounds.Y + (elem.NativeElement.GetElementBounds.Height / 2)
        '// use a dispatchevent rather than a fire event.
        '// is it a mousevent or an HTML event?
        Select Case eventname
            Case "contextmenu"
                script.Append("var sevt = document.createEvent('HTMLEvents');")
                script.Append("sevt.initEvent('" & eventname & "',true, false);")
            Case Else
                script.Append("var sevt = document.createEvent('MouseEvents');")
                script.Append("sevt.initMouseEvent('" & eventname & "',true, true, window, 1, " & x & ", " & y & "," & x & "," & y & ", false, false, false, false, " & button & ", null);")
        End Select


        '// if there is no id, make one.
        If elem.Id = "" Then elem.Id = "SK999"
        script.Append("document.getElementById('" & elem.Id & "').dispatchEvent(sevt);")
        '// fire the event
        elem.DomContainer.Eval(script.ToString)
        '// give it a sec
        Thread.Sleep(100)
        '// reset the Id if required
        If elem.Id = "SK999" Then elem.Id = ""

        Return True
    End Function

    <Extension()>
    Public Sub Add(Of T)(ByRef arr As T(), item As T)
        Array.Resize(arr, arr.Length + 1)
        arr(arr.Length - 1) = item
    End Sub

End Module

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
