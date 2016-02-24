Public Class MonitoredAttribute
    Inherits Attribute

    Public Sub New()
        MyClass.New(Nothing)
    End Sub

    Public Sub New(groupName As String)
        Me.GroupName = groupName
    End Sub

    Public ReadOnly Property GroupName As String

End Class
