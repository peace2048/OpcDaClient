Imports System.Reflection
Imports OpcDaClient

Public Class DaUtil

    Public Shared Function GetMonitoredDaItem(obj As Object, Optional groupName As String = Nothing) As IList(Of DaItem)

        Dim itemType = GetType(DaItem)
        Dim objType = obj.GetType()

        Dim properties = objType.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic).
            Where(Function(p) itemType.IsAssignableFrom(p.PropertyType)).
            Where(Function(p) p.CustomAttributes.OfType(Of MonitoredAttribute)().Any(Function(a) a.GroupName = groupName)).
            Select(Function(p) DirectCast(p.GetValue(obj), DaItem))

        Return properties.ToList()

    End Function

End Class
