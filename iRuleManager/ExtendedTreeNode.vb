Namespace Manager

    <Serializable()>
    Public Class MyTreeNode
        Inherits TreeNode

        Public Property ID As String
        Public Property parentID As String
        Public Property [Type] As eiRuleItemType
        Public Property XtraInfo As String
        Public Property Info As iRuleGenericItem
        Public Property Data As Object
        Public Property BasicInfo As IRuleBasicItemInfo
        Public Sub New(text As String)
            MyBase.New(text)
        End Sub

        Public Function GetParent() As MyTreeNode
            Return Me.Parent
        End Function
    End Class

End Namespace