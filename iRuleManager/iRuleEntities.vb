Namespace Manager



    <Serializable()>
    Public Class iRulePanel
        Property Device As String
        Property ID As String
        Property Name As String
        Property [Shared] As String
        Property Description As String
        Property Hidden As String
        Property HomePanel As String
        Property SwipeDisabled As String
        Property Locked As String
        Property Pages As New List(Of iRulePage)
    End Class

    <Serializable()>
    Public Class iRulePage
        Property Device As String
        Property ID As String
        Property ParentID As String
        Property Name As String
        Property Rows As Integer
        Property Columns As Integer
        Property Entrance As String
        Property EnableMotions As String
        Property EnableGestures As String
        Property HomePage As String
        Property Widgets As New List(Of iRuleWidget)
    End Class

    <Serializable()>
    Public Class iRuleDevice
        Property DeviceName As String
        Property ID As String
        Property Panels As New List(Of iRulePanel)
    End Class

    <Serializable()>
    Public Class iRuleWidget
        Property WidgetName As String
        Property WidgetType As String
        Property DisplayName As String = ""
        Property Row As String = ""
        Property Column As String = ""
        Property Width As String = ""
        Property Height As String = ""
        Property HideAfter As String = ""
        Property Font As String = ""
        Property FontSize As String = ""
        Property FontBold As String = ""
        Property TextColor As String = ""
        Property TextAlignment As String = ""
        Property ScrollingText As String = ""
    End Class

    <Serializable()>
    Public Class iRuleGenericItem
        Property ID As String = ""
        Property ParentID As String = ""
        Property Name As String = ""
        Property Type As eiRuleItemType = eiRuleItemType.Unknown
        Property TypeName As String = ""
        Property XtraInfo As String = ""
        Property ParentPanel As String = ""
    End Class
End Namespace
