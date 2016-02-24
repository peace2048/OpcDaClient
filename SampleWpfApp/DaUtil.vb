Imports System.Reflection
Imports OpcDaClient

Public Class DaUtil

    Public Shared Function GetMonitoredItems(obj As Object, Optional groupName As String = Nothing) As IEnumerable(Of IDaItem)

        If obj Is Nothing Then Enumerable.Empty(Of IDaItem)()

        Return _EnumerateProperties(Of IDaItem)(obj, obj.GetType().Assembly, If(groupName, String.Empty))

    End Function

    Public Shared Function GetProperties(Of T)(obj As Object) As IEnumerable(Of T)

        If obj Is Nothing Then Return Enumerable.Empty(Of T)()

        Return _EnumerateProperties(Of T)(obj, obj.GetType().Assembly, Nothing)

    End Function

    Private Shared Iterator Function _EnumerateProperties(Of T)(obj As Object, refrectionAssembly As Assembly, groupName As String) As IEnumerable(Of T)

        Dim targetType = GetType(T)
        Dim enumerableType = GetType(IEnumerable)
        Dim objType = obj.GetType()

        Dim properties = objType.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
        For Each prop In properties
            If targetType.IsAssignableFrom(prop.PropertyType) Then
                If groupName Is Nothing OrElse prop.GetCustomAttribute(Of MonitoredAttribute)()?.GroupName = groupName Then
                    Yield prop.GetValue(obj)
                End If

            ElseIf enumerableType.IsAssignableFrom(prop.PropertyType) Then
                Dim isMonitored = groupName Is Nothing OrElse prop.GetCustomAttribute(Of MonitoredAttribute)()?.GroupName = groupName
                For Each itemValue In DirectCast(prop.GetValue(obj), IEnumerable)
                    If itemValue IsNot Nothing Then
                        Dim itemValueType = itemValue.GetType()
                        If targetType.IsAssignableFrom(itemValueType) Then
                            If isMonitored Then
                                Yield DirectCast(itemValue, T)
                            End If
                        ElseIf itemValueType.Assembly = refrectionAssembly Then
                            For Each subProp In _EnumerateProperties(Of T)(itemValue, refrectionAssembly, groupName)
                                Yield subProp
                            Next
                        End If
                    End If
                Next
            ElseIf prop.PropertyType.Assembly = refrectionAssembly Then
                Dim propValue = prop.GetValue(obj)
                If propValue IsNot Nothing Then
                    For Each subProp In _EnumerateProperties(Of T)(propValue, refrectionAssembly, groupName)
                        Yield subProp
                    Next
                End If
            End If
        Next
    End Function

End Class
