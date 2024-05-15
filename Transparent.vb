Public Module Transparent
    Public Class Panel
        Inherits System.Windows.Forms.Panel
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class

    Public Class FlowLayoutPanel
        Inherits System.Windows.Forms.FlowLayoutPanel
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class

    Public Class Label
        Inherits System.Windows.Forms.Label
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class

    Public Class TextBox
        Inherits System.Windows.Forms.TextBox
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
            Me.BackColor = Color.Transparent
        End Sub
        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim CP As CreateParams = MyBase.CreateParams
                CP.ExStyle = CP.ExStyle Or &H20
                Return CP
            End Get
        End Property
    End Class

    Public Class Button
        Inherits System.Windows.Forms.Button
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class
End Module
