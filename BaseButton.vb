Imports System.Drawing.Drawing2D
Imports System.IO

Public Class BaseButton
    Inherits Transparent.Button

    Property SubButton As Boolean

    Public Sub New()
        Me.Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold)
        Me.ForeColor = Globals.Palette("Plain Light")
        Me.BackColor = Globals.Palette("Secondary")

        Me.FlatStyle = FlatStyle.Flat
        Me.FlatAppearance.BorderSize = 0
        Me.FlatAppearance.BorderColor = Nothing
        Me.FlatAppearance.MouseOverBackColor = Globals.Palette("Plain Dark")
        Me.FlatAppearance.MouseDownBackColor = Globals.Palette("Plain Dark")

        Me.MinimumSize = New Size(0, Globals.Unit(1))
        Me.MaximumSize = New Size(0, Globals.Unit(1))
        Me.Padding = New Padding(Globals.Unit(0.5), 0, Globals.Unit(0.5), 0)
        Me.Size = New Size(Globals.Unit(3), Globals.Unit(1))
        Me.AutoSize = True
    End Sub


    Protected Sub BaseButton_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus
        Me.BackColor = Globals.Palette("Plain Dark")

        If SubButton Then
            Me.ForeColor = Globals.Palette("Plain Light")
            Me.BackColor = Globals.Palette("Plain Dark")
        End If
    End Sub
    Protected Sub BaseButton_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        Me.BackColor = Globals.Palette("Secondary")

        If SubButton Then
            Me.ForeColor = Globals.Palette("Plain Light")
            Me.BackColor = Globals.Palette("Plain Dark")
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim path As New GraphicsPath
        Dim radius As Integer = Me.Height * 0.5
        path.AddArc(0, 0, radius * 2, radius * 2, 90, 180)
        path.AddArc(Me.Width - radius * 2, 0, radius * 2, radius * 2, 270, 180)
        path.CloseAllFigures()

        If SubButton Then
            Me.ForeColor = Globals.Palette("Plain Light")
            Me.BackColor = Globals.Palette("Plain Dark")
            Me.FlatAppearance.MouseOverBackColor = Globals.Palette("Secondary")
            Me.FlatAppearance.MouseDownBackColor = Globals.Palette("Secondary")
        End If

        Me.Region = New Region(path)

        MyBase.OnPaint(e)
    End Sub
End Class
