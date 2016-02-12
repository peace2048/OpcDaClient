Imports System.Reactive.Linq
Imports OpcDaClient
Imports OpcDaClient.Melsec
Imports Reactive.Bindings

Public Class MainWindowViewModel

    Public Sub New()

        If GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic Then
            Return
        End If
        Dim ioc = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default
        ioc.Register(Of IServerFactory)(Function() New ServerFactory())
        ioc.Register(Of DaClient)()

        Dim opc = ioc.GetInstance(Of DaClient)()
        opc.Connect("TAKEBISHI.Dxp")
        Dim item = New DaItem With {.Node = New DaNode With {.ItemId = "Device1.D0"}}
        opc.Read({item})
        opc.Watch(LineProgresses.SelectMany(Function(a) a.WatchItems), TimeSpan.FromSeconds(3), 0, 0)
    End Sub

    Public ReadOnly Property LineProgresses As Progress() =
    {
        New Progress(0),
        New Progress(10),
        New Progress(20)
    }

    Public Class Progress

        Public Sub New(baseAddress As Integer)
            Dim gen = New MelNodeSequenceGenerator(MelDevice.D, baseAddress)
            Dim planItem = gen.CreateDaItem(Of Short)(1)
            Dim actualItem = gen.CreateDaItem(Of Short)(1)
            WatchItems = {planItem, actualItem}
            Plan = planItem.ToReactiveProperty()
            Actual = actualItem.ToReactiveProperty()
            Diff = Observable.CombineLatest(planItem, actualItem, Function(p, a) a - p).ToReactiveProperty()
        End Sub

        Public ReadOnly WatchItems As IEnumerable(Of DaItem)

        Public ReadOnly Property Plan As ReactiveProperty(Of Short)
        Public ReadOnly Property Actual As ReactiveProperty(Of Short)
        Public ReadOnly Property Diff As ReactiveProperty(Of Short)

    End Class
End Class