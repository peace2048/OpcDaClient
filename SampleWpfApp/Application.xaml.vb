Imports Autofac
Imports OpcDaClient

Class Application

    Public Sub New()
        Dim builder = New ContainerBuilder()
        builder.Register(Function(c) New ServerFactory()).As(Of IServerFactory)()
        builder.Register(Function(c) New DaClient(c.Resolve(Of IServerFactory)())).OnActivated(Sub(e) e.Instance.Connect("Takebishi.Dxp")).InstancePerLifetimeScope()
        Container = builder.Build()
    End Sub

    Public ReadOnly Property Container As IContainer

End Class
