Option Explicit On
Option Strict On
Option Infer On

Imports System.Reactive.Linq
Imports Autofac
Imports OpcDaClient
Imports OpcDaClient.DeviceXPlorer
Imports OpcDaClient.DeviceXPlorer.Melsec
Imports Reactive.Bindings

Public Class MainWindowViewModel

    Private _compositeDisposable As New System.Reactive.Disposables.CompositeDisposable()

    Public Sub New()

        If GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic Then
            Return
        End If

        Dim opc = My.Application.Container.Resolve(Of DaClient)()

        _compositeDisposable.Add(opc.Watch(DaUtil.GetMonitoredItems(Me), TimeSpan.FromSeconds(3), 0, 0))
    End Sub

    Public ReadOnly Property LineProgresses As Progress() = EnumerableEx.Range(0, 3, 10).Select(Function(address) New Progress(address)).ToArray()

    Public Class Progress

        Public Sub New(startAddress As Integer)
            DaUtil.GetProperties(Of DxpItem)(Me).ToList().ForEach(Sub(item) item.Node.Address += startAddress)
        End Sub

        <Monitored> Public ReadOnly Property Plan As ItemInt16 = New ItemInt16(MelDevice.D, 0)
        <Monitored> Public ReadOnly Property Actual As ItemInt16 = New ItemInt16(MelDevice.D, 1)
        Public ReadOnly Property Diff As ReactiveProperty(Of Int16) = Observable.CombineLatest(Plan, Actual, Function(plan, actual) actual - plan).ToReactiveProperty()

    End Class
End Class