Public Module Transparent
    Public Class Panel
        Inherits System.Windows.Forms.Panel
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class

    Public Class FlowLayoutPanel
        Inherits System.Windows.Forms.FlowLayoutPanel
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class

    Public Class Label
        Inherits System.Windows.Forms.Label
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class

    Public Class TextBox
        Inherits System.Windows.Forms.TextBox
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class

    Public Class Button
        Inherits System.Windows.Forms.Button
        Public Sub New()
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent
        End Sub
    End Class
End Module
