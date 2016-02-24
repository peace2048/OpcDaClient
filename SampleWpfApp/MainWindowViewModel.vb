Option Explicit On
Option Strict On
Option Infer On

Imports System.Reactive.Linq
Imports Autofac
Imports OpcDaClient
Imports OpcDaClient.DeviceXPlorer.Melsec
Imports Reactive.Bindings

Public Class MainWindowViewModel

    Private _compositeDisposable As New System.Reactive.Disposables.CompositeDisposable()

    Public Sub New()

        If GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic Then
            Return
        End If

        Dim opc = My.Application.Container.Resolve(Of DaClient)()

        _compositeDisposable.Add(opc.Watch(LineProgresses.SelectMany(Function(a) DaUtil.GetMonitoredDaItem(a)), TimeSpan.FromSeconds(3), 0, 0))
    End Sub

    Public ReadOnly Property LineProgresses As Progress() =
    {
        New Progress(0),
        New Progress(10),
        New Progress(20)
    }

    Public Class Progress

        Public Sub New(startAddress As Integer)
        End Sub

        Public ReadOnly Property Plan As ItemInt16 = New ItemInt16(MelDevice.D, 0)
        Public ReadOnly Property Actual As ItemInt16 = New ItemInt16(MelDevice.D, 1)
        Public ReadOnly Property Diff As ReactiveProperty(Of Int16) = Observable.CombineLatest(Plan, Actual, Function(plan, actual) actual - plan).ToReactiveProperty()

    End Class
End Class