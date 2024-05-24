Imports System.Reflection
Imports System.Resources

Public Class BaseModal
    Inherits Form

    Property Title As String
    Property Message As String
    Property Buttons As Dictionary(Of String, DialogResult)

    Private Timer As Timer
    Private TimerToStop As Timer

    Public Sub New()
        If Buttons Is Nothing Then
            Buttons = New Dictionary(Of String, DialogResult) From {
                {"OK", DialogResult.OK}
            }
        End If
        If Buttons.Count > 2 Then
            Throw New Exception("Only 2 buttons are allowed")
        End If
    End Sub

    Protected Sub BaseModal_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.BackColor = Globals.Palette("White")
        Me.ForeColor = Globals.Palette("Secondary")
        Me.Size = New Size(Globals.Unit(15), Globals.Unit(10))
        Me.Location = New Point(
            (Screen.PrimaryScreen.WorkingArea.Width - Me.Width) / 2,
            (Screen.PrimaryScreen.WorkingArea.Height - Me.Height) / 2
        )
        Me.FormBorderStyle = FormBorderStyle.None
        Me.ControlBox = False

        Me.Icon = My.Resources.ResourceManager.GetObject("Icon")

        Me.ShowInTaskbar = True

        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Asterisk)



        Dim Highlighted As Boolean = False



        Dim HeaderBar As New Panel With {
            .Size = New Size(Me.Width, Globals.Unit(1)),
            .Dock = DockStyle.Top,
            .BackColor = Me.ForeColor
        }
        Me.Controls.Add(HeaderBar)

        Dim TitleLabel As New Label With {
            .Text = Me.Title,
            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
            .ForeColor = Me.BackColor,
            .Size = New Size(
                HeaderBar.Width - Globals.Unit(4),
                HeaderBar.Height
            ),
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        TitleLabel.Location = New Point(
            (HeaderBar.Width - TitleLabel.Width) / 2,
            (HeaderBar.Height - TitleLabel.Height) / 2
        )
        HeaderBar.Controls.Add(TitleLabel)

        Dim Logo As New PictureBox With {
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Name = "Logo",
            .Size = New Size(Globals.Unit(0.75), Globals.Unit(0.75)),
            .Location = New Point(Globals.Unit(1.125), Globals.Unit(0.125))
        }
        Dim resourcesManager As ResourceManager = My.Resources.ResourceManager
        Dim LogoImage As Image = resourcesManager.GetObject("Logo")
        Logo.Image = LogoImage
        HeaderBar.Controls.Add(Logo)

        Dim CloseButton As New PictureBox With {
            .Name = "CloseButton",
            .Size = New Size(Globals.Unit(1), Globals.Unit(1)),
            .SizeMode = PictureBoxSizeMode.CenterImage,
            .Padding = New Padding(0),
            .Margin = New Padding(0),
            .Image = Globals.LoadSvgFromResource("Close Window", New Size(Globals.Unit(1), Globals.Unit(1))).Draw(),
            .Location = New Point(HeaderBar.Width - Globals.Unit(2), 0)
        }
        AddHandler CloseButton.MouseEnter, Sub()
                                               Try
                                                   CloseButton.BackColor = Color.FromArgb(
                                                    Me.ForeColor.R + Globals.Unit(1),
                                                    Me.ForeColor.G + Globals.Unit(1),
                                                    Me.ForeColor.B + Globals.Unit(1)
                                                   )
                                               Catch ex As Exception

                                               End Try
                                           End Sub
        AddHandler CloseButton.MouseLeave, Sub()
                                               CloseButton.BackColor = Me.ForeColor
                                           End Sub
        AddHandler CloseButton.Click, Sub()
                                          Me.DialogResult = DialogResult.Cancel
                                      End Sub
        HeaderBar.Controls.Add(CloseButton)

        For Each Control As Control In New List(Of Control) From {HeaderBar, TitleLabel, Logo, CloseButton}
            Control.Cursor = Cursors.SizeAll
            AddHandler Control.MouseDown, AddressOf HeaderBar_MouseDown
            AddHandler Control.MouseMove, AddressOf HeaderBar_MouseMove
            AddHandler Control.MouseUp, AddressOf HeaderBar_MouseUp
        Next

        Dim MessageLabelPanel As New Panel With {
            .Size = New Size(
                Me.Width - Globals.Unit(2),
                Globals.Unit(3)
            ),
            .AutoScroll = True
        }
        MessageLabelPanel.Location = New Point(
            (Me.Width - MessageLabelPanel.Width) / 2,
            HeaderBar.Height + Globals.Unit(1)
        )
        Me.Controls.Add(MessageLabelPanel)

        Dim MessageLabel As New Label With {
            .Text = Me.Message,
            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
            .ForeColor = Globals.Palette("Plain Dark"),
            .MinimumSize = New Size(Me.Width - Globals.Unit(2) - (SystemInformation.VerticalScrollBarWidth * 2), 0),
            .MaximumSize = New Size(Me.Width - Globals.Unit(2) - (SystemInformation.VerticalScrollBarWidth * 2), 0),
            .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Location = New Point(SystemInformation.VerticalScrollBarWidth, 0)
        }
        MessageLabelPanel.Controls.Add(MessageLabel)

        Dim ButtonPanel As New Panel With {
            .Size = New Size(Me.Width - Globals.Unit(2), Globals.Unit(1))
        }
        ButtonPanel.Location = New Point(
            (Me.Width - ButtonPanel.Width) / 2,
            MessageLabelPanel.Bottom + Globals.Unit(1)
        )
        Me.Controls.Add(ButtonPanel)

        Dim Button1 As New BaseButton With {
            .Text = Buttons.Keys(0),
            .Name = Buttons.Keys(0),
            .Size = New Size(Globals.Unit(5), Globals.Unit(1)),
            .Location = New Point(
                (ButtonPanel.Width - Globals.Unit(5)) / 2,
                0
            )
        }
        AddHandler Button1.Click, Sub()
                                      Me.DialogResult = Buttons.Values(0)
                                  End Sub
        ButtonPanel.Controls.Add(Button1)

        Dim Button2 As BaseButton
        If Buttons.Count = 2 Then
            Button2 = New BaseButton With {
                .Text = Buttons.Keys(1),
                .Name = Buttons.Keys(1),
                .Size = New Size(Globals.Unit(5), Globals.Unit(1))
            }
            AddHandler Button2.Click, Sub()
                                          Me.DialogResult = Buttons.Values(1)
                                      End Sub
            ButtonPanel.Controls.Add(Button2)

            Button1.Location = New Point(0, 0)
            Button2.Location = New Point(ButtonPanel.Width - Button2.Width, 0)
        End If

        Dim BorderLeft As New Panel With {
            .Size = New Size(Globals.Unit(0.1), Me.Height),
            .Location = New Point(0, 0),
            .BackColor = Me.ForeColor
        }
        Me.Controls.Add(BorderLeft)
        Dim BorderRight As New Panel With {
            .Size = New Size(Globals.Unit(0.1), Me.Height),
            .Location = New Point(Me.Width - Globals.Unit(0.1), 0),
            .BackColor = Me.ForeColor
        }
        Me.Controls.Add(BorderRight)
        Dim BorderBottom As New Panel With {
            .Size = New Size(Me.Width, Globals.Unit(0.1)),
            .Location = New Point(0, Me.Height - Globals.Unit(0.1)),
            .BackColor = Me.ForeColor
        }
        Me.Controls.Add(BorderBottom)

        Me.Timer = New Timer With {
                    .Interval = Globals.Unit(3)
                }
        AddHandler Timer.Tick, Sub()
                                   If Highlighted Then
                                       Me.ForeColor = Globals.Palette("Primary Compliment")
                                       HeaderBar.BackColor = Globals.Palette("Primary Compliment")
                                       Button1.BackColor = Globals.Palette("Primary Compliment")
                                       If Buttons.Count = 2 Then
                                           Button2.BackColor = Globals.Palette("Primary Compliment")
                                       End If
                                       BorderLeft.BackColor = Globals.Palette("Primary Compliment")
                                       BorderRight.BackColor = Globals.Palette("Primary Compliment")
                                       BorderBottom.BackColor = Globals.Palette("Primary Compliment")
                                       Highlighted = False
                                   Else
                                       Me.ForeColor = Globals.Palette("Secondary")
                                       HeaderBar.BackColor = Globals.Palette("Secondary")
                                       Button1.BackColor = Globals.Palette("Secondary")
                                       If Buttons.Count = 2 Then
                                           Button2.BackColor = Globals.Palette("Secondary")
                                       End If
                                       BorderLeft.BackColor = Globals.Palette("Secondary")
                                       BorderRight.BackColor = Globals.Palette("Secondary")
                                       BorderBottom.BackColor = Globals.Palette("Secondary")
                                       Highlighted = True
                                   End If
                               End Sub
        Timer.Start()
        Me.TimerToStop = New Timer With {
            .Interval = Globals.Unit(50)
        }
        AddHandler TimerToStop.Tick, Sub()
                                         Timer.Stop()
                                         Timer.Dispose()
                                         TimerToStop.Stop()
                                         TimerToStop.Dispose()
                                         Me.ForeColor = Globals.Palette("Secondary")
                                         HeaderBar.BackColor = Globals.Palette("Secondary")
                                         BorderLeft.BackColor = Globals.Palette("Secondary")
                                         BorderRight.BackColor = Globals.Palette("Secondary")
                                         BorderBottom.BackColor = Globals.Palette("Secondary")
                                     End Sub
        TimerToStop.Start()
    End Sub



    Protected Is_HeaderBar_MouseDown As Boolean = False
    Protected MouseLocationInsideTheForm As Point
    Protected Sub HeaderBar_MouseDown(sender As Object, e As MouseEventArgs)
        Is_HeaderBar_MouseDown = True
        MouseLocationInsideTheForm = Me.PointToClient(Cursor.Position)
    End Sub
    Protected Sub HeaderBar_MouseMove(sender As Object, e As MouseEventArgs)
        If Is_HeaderBar_MouseDown Then
            If Me.WindowState = FormWindowState.Maximized Then
                Me.WindowState = FormWindowState.Normal
            End If

            Dim CursorLocation = Cursor.Position

            Dim NewLocation As New Point(
                CursorLocation.X - MouseLocationInsideTheForm.X,
                CursorLocation.Y - MouseLocationInsideTheForm.Y
            )

            If NewLocation.X < 0 Then
                NewLocation = New Point(
                    0,
                    NewLocation.Y
                )
            End If
            If NewLocation.Y < 0 Then
                NewLocation = New Point(
                    NewLocation.X,
                    0
                )
            End If

            Me.Location = NewLocation
        End If
    End Sub
    Protected Sub HeaderBar_MouseUp(sender As Object, e As MouseEventArgs)
        Is_HeaderBar_MouseDown = False

        If Me.Location.Y = 0 Then
            Me.WindowState = FormWindowState.Maximized
        End If
    End Sub

End Class
