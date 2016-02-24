Public Class EnumerableEx

    Public Shared Iterator Function [For](Of T)(seed As T, condition As Func(Of T, Boolean), [next] As Func(Of T, T)) As IEnumerable(Of T)
        While condition(seed)
            Yield seed
            seed = [next](seed)
        End While
    End Function

    Public Shared Iterator Function Range(seed As Integer, count As Integer, [step] As Integer) As IEnumerable(Of Integer)
        For i = 1 To count
            Yield seed
            seed += [step]
        Next
    End Function

End Class
