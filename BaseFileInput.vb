Imports System.Drawing.Drawing2D
Imports System.IO

Public Class BaseFileInput
    Inherits Transparent.Panel

    Private color As Color = Globals.Palette("Secondary")
    Public FilePath As String = ""

    Public Sub New()
        Me.BackColor = Globals.Palette("Plain Light")
        Me.BorderStyle = BorderStyle.None
        Me.AutoSize = True

        Me.MaximumSize = New Size(Globals.Unit(7), Globals.Unit(3))
        Me.MinimumSize = New Size(Globals.Unit(7), Globals.Unit(3))

        Dim Icon As New PictureBox With {
            .Size = New Size(Globals.Unit(1), Globals.Unit(1)),
            .Image = Globals.LoadSvgFromResource("File Input Icon", New Size(Globals.Unit(1), Globals.Unit(1))).Draw(),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Name = "Icon"
        }
        Icon.Location = New Point((Me.Width - Icon.Width) / 0.5, Globals.Unit(0.75))
        Me.Controls.Add(Icon)

        Dim Label As New Label With {
            .Text = "Click to upload a .csv file.",
            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
            .ForeColor = color,
            .AutoSize = True,
            .Name = "Label",
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Label.Location = New Point((Me.Width - Label.Width) / 2, Globals.Unit(2))
        Me.Controls.Add(Label)

        For Each Control As Control In Me.Controls
            AddHandler Control.MouseEnter, AddressOf BaseFileInput_MouseEnter
            AddHandler Control.MouseLeave, AddressOf BaseFileInput_MouseLeave
            AddHandler Control.Click, AddressOf BaseFileInput_Click
        Next
    End Sub

    Protected Sub BaseFileInput_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        For Each Control As Control In Me.Controls
            Control.Location = New Point((Me.Width - Control.Width) / 2, Control.Location.Y)
        Next
    End Sub

    Protected Sub BaseFileInput_MouseEnter(sender As Object, e As EventArgs) Handles Me.MouseEnter
        color = Globals.Palette("Plain Dark")
        Dim Icon As PictureBox = Me.Controls("Icon")
        Icon.Image = Globals.LoadSvgFromResource("File Input Icon Hovered", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
        For Each Control As Control In Me.Controls
            Control.ForeColor = color
        Next
        Me.Refresh()
    End Sub

    Protected Sub BaseFileInput_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave
        color = Globals.Palette("Secondary")
        Dim Icon As PictureBox = Me.Controls("Icon")
        Icon.Image = Globals.LoadSvgFromResource("File Input Icon", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
        For Each Control As Control In Me.Controls
            Control.ForeColor = color
        Next
        Me.Refresh()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        Dim Pen As New Pen(color, Globals.Unit(0.1))

        'TopLeft
        g.DrawArc(
            Pen,
            Pen.Width / 2,
            Pen.Width / 2,
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            180,
            90
        )
        'Left
        g.DrawLine(
            Pen,
            Pen.Width / 2,
            CInt(Pen.Width / 2 + Globals.Unit(0.25)),
            Pen.Width / 2,
            CInt(Me.Height - Pen.Width / 2 - Globals.Unit(0.25))
        )
        'BottomLeft
        g.DrawArc(
            Pen,
            Pen.Width / 2,
            CInt(Me.Height - Globals.Unit(0.5) - Pen.Width),
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            90,
            90
        )
        'Bottom
        g.DrawLine(
            Pen,
            CInt(Pen.Width / 2 + Globals.Unit(0.25)),
            CInt(Me.Height - Pen.Width / 2),
            CInt(Me.Width - Pen.Width / 2 - Globals.Unit(0.25)),
            CInt(Me.Height - Pen.Width / 2)
        )
        'BottomRight
        g.DrawArc(
            Pen,
            CInt(Me.Width - Globals.Unit(0.5) - Pen.Width),
            CInt(Me.Height - Globals.Unit(0.5) - Pen.Width),
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            0,
            90
        )
        'Right
        g.DrawLine(
            Pen,
            CInt(Me.Width - Pen.Width / 2),
            CInt(Me.Height - Globals.Unit(0.25) - Pen.Width / 2),
            CInt(Me.Width - Pen.Width / 2),
            CInt(Pen.Width / 2 + Globals.Unit(0.25))
        )
        'TopRight
        g.DrawArc(
            Pen,
            CInt(Me.Width - Globals.Unit(0.5) - Pen.Width),
            Pen.Width / 2,
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            270,
            90
        )
        'Top
        g.DrawLine(
            Pen,
            CInt(Me.Width - Globals.Unit(0.25)),
            Pen.Width / 2,
            CInt(Pen.Width / 2 + Globals.Unit(0.25)),
            Pen.Width / 2
        )

        Dim Path As New GraphicsPath
        Path.AddArc(
            0,
            0,
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            180,
            90
        )
        Path.AddLine(
            0,
            CInt(Globals.Unit(0.25)),
            0,
            CInt(Me.Height - Globals.Unit(0.25))
        )
        Path.AddArc(
            0,
            CInt(Me.Height - Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            90,
            90
        )
        Path.AddLine(
            CInt(Globals.Unit(0.25)),
            Me.Height,
            CInt(Me.Width - Globals.Unit(0.25)),
            Me.Height
        )
        Path.AddArc(
            CInt(Me.Width - Globals.Unit(0.5)),
            Me.Height - CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            0,
            90
        )
        Path.AddLine(
            Me.Width,
            CInt(Me.Height - Globals.Unit(0.25)),
            Me.Width,
            CInt(Globals.Unit(0.25))
        )
        Path.AddArc(
            CInt(Me.Width - Globals.Unit(0.5)),
            0,
            CInt(Globals.Unit(0.5)),
            CInt(Globals.Unit(0.5)),
            270,
            90
        )
        Path.AddLine(
            CInt(Me.Width - Globals.Unit(0.25)),
            0,
            CInt(Globals.Unit(0.25)),
            0
        )

        Me.Region = New Region(Path)
    End Sub

    Protected Sub BaseFileInput_Click(sender As Object, e As EventArgs) Handles Me.Click
        Dim Dialog As New OpenFileDialog With {
            .Filter = "CSV Files (*.csv)|*.csv",
            .Title = "Select a CSV File"
        }
        If Dialog.ShowDialog() = DialogResult.OK Then
            Dim Label As Label = Me.Controls("Label")
            Label.Text = Path.GetFileName(Dialog.FileName)
            Me.FilePath = Dialog.FileName
            RaiseEvent FileSelected(Me, New EventArgs)
            BaseFileInput_Resize(Me, New EventArgs)
        End If
    End Sub

    Public Event FileSelected(sender As Object, e As EventArgs)


    Private Timer As Timer
    Private TimerToStop As Timer
    Public Sub Alert()
        Dim Alerted As Boolean = False
        Me.Timer = New Timer With {
                    .Interval = Globals.Unit(3)
                }
        AddHandler Timer.Tick, Sub()
                                   If Alerted Then
                                       Me.color = Globals.Palette("Secondary")
                                       Dim Label As Label = Me.Controls("Label")
                                       Label.ForeColor = Me.color
                                       Dim Icon As PictureBox = Me.Controls("Icon")
                                       Icon.Image = Globals.LoadSvgFromResource("File Input Icon", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
                                       Alerted = False
                                   Else
                                       Me.color = Globals.Palette("Primary Compliment")
                                       Dim Label As Label = Me.Controls("Label")
                                       Label.ForeColor = Me.color
                                       Dim Icon As PictureBox = Me.Controls("Icon")
                                       Icon.Image = Globals.LoadSvgFromResource("File Input Icon Alerted", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
                                       Alerted = True
                                   End If
                                   Me.Refresh()
                               End Sub
        Timer.Start()
        Me.TimerToStop = New Timer With {
            .Interval = Globals.Unit(50)
        }
        AddHandler TimerToStop.Tick, Sub()
                                         Timer.Stop()
                                         TimerToStop.Stop()
                                         Me.color = Globals.Palette("Secondary")
                                         Dim Label As Label = Me.Controls("Label")
                                         Label.ForeColor = Me.color
                                         Dim Icon As PictureBox = Me.Controls("Icon")
                                         Icon.Image = Globals.LoadSvgFromResource("File Input Icon", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
                                         Me.Refresh()
                                     End Sub
        TimerToStop.Start()
        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Asterisk)
    End Sub
End Class
