Imports System.ComponentModel
Imports System.Reactive.Linq
Imports System.Reactive.Subjects
Imports System.Runtime.CompilerServices
Imports OpcDaClient
Imports OpcDaClient.DeviceXPlorer
Imports OpcDaClient.DeviceXPlorer.Melsec

Public Class ItemBase(Of T)
    Inherits DxpItem
    Implements INotifyPropertyChanged, IObservable(Of T)

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _Value As T
    Private _subject As Subject(Of T) = New Subject(Of T)

    Public Sub New()
        Result.Select(AddressOf GetValue).
            DistinctUntilChanged().
            Subscribe(
                Sub(v)
                    _Value = v
                    RaisePropertyChanged(NameOf(Value))
                End Sub)
    End Sub

    Public Property Value As T
        Get
            Return _Value
        End Get
        Set(value As T)
            SetValue(value)
        End Set
    End Property

    Protected Overridable Sub SetValue(value As T)
        Result.Value = value
    End Sub

    Protected Overridable Function GetValue(raw As Object) As T
        Return CType(raw, T)
    End Function

    Protected Sub RaisePropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Function Subscribe(observer As IObserver(Of T)) As IDisposable Implements IObservable(Of T).Subscribe
        Return _subject.Subscribe(observer)
    End Function
End Class

Public Class ItemInt16
    Inherits ItemBase(Of Short)

    Public Sub New(device As DxpDevice, address As Integer)
        Node = New DxpNode With {.Device = device, .Address = address, .Size = 1}
    End Sub
End Class
