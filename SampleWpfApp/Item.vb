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

    Public Sub New()
        Result.Select(AddressOf GetValue).
            DistinctUntilChanged().
            Subscribe(
                Sub(v)
                    _Value = v
                    RaisePropertyChanged(NameOf(Value))
                End Sub)
    End Sub

    Private _Value As T
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

    Private _subject As Subject(Of T) = New Subject(Of T)

    Public Function Subscribe(observer As IObserver(Of T)) As IDisposable Implements IObservable(Of T).Subscribe
        Return _subject.Subscribe(observer)
    End Function
End Class

Public Class ItemInt16
    Inherits ItemBase(Of Short)

    Public Sub New(device As MelDevice, address As Integer)
        Node = New MelNode With {.Device = device, .Address = address, .Size = 1}
    End Sub
End Class

Public Class ItemInt16Array
    Inherits DaItem
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(device As MelDevice, address As Integer, count As Integer)
        Result.OfType(Of Int16()).Subscribe(
            Sub(arr)
                For i = 0 To arr.Length - 1
                    Values(i).Value = arr(i)
                Next
            End Sub)
    End Sub

    Public ReadOnly Property Values As NotifyProperty(Of Int32)()
End Class

Public Class NotifyProperty(Of T)
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _Value As T
    Public Property Value() As T
        Get
            Return _Value
        End Get
        Set(ByVal value As T)
            If Not EqualityComparer(Of T).Default.Equals(_Value, value) Then
                _Value = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Me.Value)))
            End If
        End Set
    End Property
End Class