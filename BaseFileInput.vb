Imports System.Drawing.Drawing2D

Public Class BaseFileInput
    Inherits Transparent.Panel

    Private color As Color = Globals.Palette("Secondary")

    Public Sub New()
        Me.BackColor = Globals.Palette("Plain Light")
        Me.BorderStyle = BorderStyle.None
        Me.MinimumSize = New Size(Me.Width, Globals.Unit(4))
        Me.MaximumSize = New Size(Me.Width, Globals.Unit(4))
        Me.AutoSize = True

        Dim Icon As New PictureBox With {
            .Size = New Size(Globals.Unit(1), Globals.Unit(1)),
            .Image = Globals.LoadSvgFromResource("File Input Icon", New Size(Globals.Unit(1), Globals.Unit(1))).Draw(),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Name = "Icon"
        }
        AddHandler Icon.MouseEnter, AddressOf BaseFileInput_MouseEnter
        AddHandler Icon.MouseLeave, AddressOf BaseFileInput_MouseLeave
        Icon.Location = New Point(Me.Width * 0.5 - Icon.Width * 0.5, Globals.Unit(1))
        Me.Controls.Add(Icon)

        Dim Label As New Transparent.Label With {
            .Text = "Click or Drag and Drop .csv file.",
            .AutoSize = True,
            .MinimumSize = New Size(Me.Width, Globals.Unit(1)),
            .MaximumSize = New Size(Me.Width, Globals.Unit(1)),
            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
            .ForeColor = color,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Name = "Label"
        }
        Label.Location = New Point(Me.Width * 0.5 - Label.Width * 0.5, Globals.Unit(2.5))
        AddHandler Label.MouseEnter, AddressOf BaseFileInput_MouseEnter
        AddHandler Label.MouseLeave, AddressOf BaseFileInput_MouseLeave
        Me.Controls.Add(Label)
    End Sub

    Protected Sub BaseFileInput_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        For Each Control As Control In Me.Controls
            If Control.Name = "Label" Then
                Control.MinimumSize = New Size(Me.Width, Globals.Unit(1))
                Control.MaximumSize = New Size(Me.Width, Globals.Unit(1))
            End If

            Control.Location = New Point(Me.Width * 0.5 - Control.Width * 0.5, Control.Location.Y)
        Next
    End Sub

    Protected Sub BaseFileInput_MouseEnter(sender As Object, e As EventArgs) Handles Me.MouseEnter
        Me.color = Globals.Palette("Plain Dark")
        Me.Controls("Label").ForeColor = Me.color
        Dim icon As PictureBox = Me.Controls("Icon")
        icon.Image = Globals.LoadSvgFromResource("File Input Icon Hovered", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
        Me.Refresh()
    End Sub
    Protected Sub BaseFileInput_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave
        Me.color = Globals.Palette("Secondary")
        Me.Controls("Label").ForeColor = Me.color
        Dim icon As PictureBox = Me.Controls("Icon")
        icon.Image = Globals.LoadSvgFromResource("File Input Icon", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
        Me.Refresh()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics
        Dim Pen As New Pen(color, Globals.Unit(0.1))

        g.DrawArc(
                    Pen,
                    0,
                    0,
                    CInt(Globals.Unit(1)),
                    CInt(Globals.Unit(1)),
                    180,
                    90
                )
        g.DrawLine(
                    Pen,
                    CInt(Globals.Unit(0.5)),
                    0,
                    CInt(Me.Width - Globals.Unit(0.5)),
                    0
                )
        g.DrawArc(
                    Pen,
                    CInt(Me.Width - Globals.Unit(1)),
                    0,
                    CInt(Globals.Unit(1)),
                    CInt(Globals.Unit(1)),
                    270,
                    90
                )
        g.DrawLine(
                    Pen,
                    Me.Width,
                    CInt(Globals.Unit(0.5)),
                    Me.Width,
                    CInt(Me.Height - Globals.Unit(0.5))
                )
        g.DrawArc(
                    Pen,
                    CInt(Me.Width - Globals.Unit(1)),
                    CInt(Me.Height - Globals.Unit(1)),
                    CInt(Globals.Unit(1)),
                    CInt(Globals.Unit(1)),
                    0,
                    90
                )
        g.DrawLine(
                    Pen,
                    CInt(Me.Width - Globals.Unit(0.5)),
                    Me.Height,
                    CInt(Globals.Unit(0.5)),
                    Me.Height
                )
        g.DrawArc(
                    Pen,
                    0,
                    CInt(Me.Height - Globals.Unit(1)),
                    CInt(Globals.Unit(1)),
                    CInt(Globals.Unit(1)),
                    90,
                    90
                )
        g.DrawLine(
                    Pen,
                    0,
                    CInt(Me.Height - Globals.Unit(0.5)),
                    0,
                    CInt(Globals.Unit(0.5))
                )
        g.Dispose()



        Dim path As New GraphicsPath
        Dim radius As Integer = Globals.Unit(0.5)
        path.AddArc(0, 0, radius * 2, radius * 2, 180, 90)
        path.AddArc(Me.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90)
        path.AddArc(Me.Width - radius * 2, Me.Height - radius * 2, radius * 2, radius * 2, 0, 90)
        path.AddArc(0, Me.Height - radius * 2, radius * 2, radius * 2, 90, 90)
        path.CloseAllFigures()
        Me.Region = New Region(path)
    End Sub
End Class
