Imports System.Reflection
Imports System.Resources
Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading

Public Class BaseForm
    Inherits Form

    Public Sub New()
        InitializeComponent()
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)

        Me.ShowInTaskbar = False
        Me.DoubleBuffered = True
    End Sub

    Public BackgroundBitmap As Bitmap
    Public Loaded As Boolean = False

    Public Overloads Property Name As String
        Get
            Return Me.Text
        End Get
        Set(value As String)
            Me.Text = value
            Me.Controls.Find("titleLabel", True)(0).Text = value
        End Set
    End Property

    Public Sub GoToForm(Form As BaseForm)
        Dim Location = MyBase.Location
        Globals.FormLocation = Location
        Dim Size = MyBase.Size
        Globals.FormSize = Size
        Me.Hide()
        Form.Show()
        Form.Location = Location
    End Sub

    Protected Sub BaseForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.BackColor = Globals.Palette("White")
        Me.ForeColor = Globals.Palette("Secondary")
        Me.MinimumSize = Globals.MinimumFormSize
        Me.Size = Globals.FormSize
        Me.Location = Globals.FormLocation
        Me.FormBorderStyle = FormBorderStyle.None
        Me.ControlBox = False

        Me.Icon = My.Resources.ResourceManager.GetObject("Icon")

        Me.HeaderBar = New Panel With {
            .Dock = DockStyle.Top,
            .Height = Globals.Unit(1),
            .Cursor = Cursors.SizeAll,
            .BackColor = Globals.Palette("Plain Light"),
            .Name = "HaderBar"
        }
        Me.Controls.Add(Me.HeaderBar)

        Dim Title As New Label With {
            .Name = "titleLabel",
            .Dock = DockStyle.Fill,
            .AutoSize = False,
            .Text = "BaseForm",
            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
            .ForeColor = Me.ForeColor,
            .BackColor = Me.HeaderBar.BackColor,
            .Padding = New Padding(0),
            .Margin = New Padding(0),
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.HeaderBar.Controls.Add(Title)

        Dim LogoBox As New FlowLayoutPanel With {
            .AutoSize = True,
            .Dock = DockStyle.Left,
            .BackColor = Me.ForeColor,
            .ForeColor = Me.BackColor,
            .Padding = New Padding(Globals.Unit(1.25), Globals.Unit(0.125), 0, Globals.Unit(0.125)),
            .Margin = New Padding(0)
        }
        Dim Logo As New PictureBox With {
            .Size = New Size(Globals.Unit(0.75), Globals.Unit(0.75)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Padding = New Padding(0),
            .Margin = New Padding(0)
        }
        Logo.Image = My.Resources.ResourceManager.GetObject("Logo")
        LogoBox.Controls.Add(Logo)
        Dim NameLabel As New Label With {
            .Dock = DockStyle.Left,
            .AutoSize = True,
            .ForeColor = Me.BackColor,
            .BackColor = Me.ForeColor,
            .Text = "CdMSMS-ICS",
            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
            .Padding = New Padding(
                Globals.Unit(0.5),
                Globals.Unit(0.25),
                Globals.Unit(1),
                Globals.Unit(0.25)
            ),
            .Margin = New Padding(0)
        }
        Dim NameTriangle As New PictureBox With {
            .Dock = DockStyle.Left,
            .Size = New Size(Globals.Unit(1), Globals.Unit(1)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Padding = New Padding(0),
            .Margin = New Padding(0),
            .Image = Globals.LoadSvgFromResource("Header Triangle TitleBox", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
        }

        Me.HeaderBar.Controls.Add(NameTriangle)
        Me.HeaderBar.Controls.Add(NameLabel)
        Me.HeaderBar.Controls.Add(LogoBox)

        Dim ControlBox As New FlowLayoutPanel With {
            .Dock = DockStyle.Right,
            .Width = Globals.Unit(7.5),
            .Padding = New Padding(Globals.Unit(1), 0, Globals.Unit(1), 0),
            .Margin = New Padding(0),
            .BackColor = Me.ForeColor
        }
        Dim HideButton As New PictureBox With {
            .Name = "MinimizeButton",
            .Size = New Size(Globals.Unit(1.5), Globals.Unit(1)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Padding = New Padding(0),
            .Margin = New Padding(0),
            .Image = Globals.LoadSvgFromResource("Hide Window", New Size(Globals.Unit(1.5), Globals.Unit(1))).Draw()
        }
        AddHandler HideButton.Click, Sub()
                                         Me.WindowState = FormWindowState.Minimized
                                     End Sub
        ControlBox.Controls.Add(HideButton)
        ControlBox.Controls.Add(New Panel With {
            .Width = Globals.Unit(0.1)
        })
        Dim SizeButton As New PictureBox With {
            .Name = "SizeButton",
            .Size = New Size(Globals.Unit(1.5), Globals.Unit(1)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Padding = New Padding(0),
            .Margin = New Padding(0),
            .Image = Globals.LoadSvgFromResource("Maximize Window", New Size(Globals.Unit(1.5), Globals.Unit(1))).Draw()
        }
        AddHandler SizeButton.Click, Sub()
                                         If Me.WindowState = FormWindowState.Maximized Then
                                             Me.WindowState = FormWindowState.Normal
                                             Me.Size = Globals.FormSize
                                         Else
                                             Me.WindowState = FormWindowState.Maximized
                                         End If
                                     End Sub
        ControlBox.Controls.Add(SizeButton)
        ControlBox.Controls.Add(New Panel With {
            .Width = Globals.Unit(0.1)
        })
        Dim CloseButton As New PictureBox With {
            .Name = "CloseButton",
            .Size = New Size(Globals.Unit(1.5), Globals.Unit(1)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Padding = New Padding(0),
            .Margin = New Padding(0),
            .Image = Globals.LoadSvgFromResource("Close Window", New Size(Globals.Unit(1.5), Globals.Unit(1))).Draw()
        }
        AddHandler CloseButton.Click, Sub()
                                          Me.Close()
                                      End Sub
        ControlBox.Controls.Add(CloseButton)
        For Each Control As PictureBox In New List(Of PictureBox) From {HideButton, SizeButton, CloseButton}
            AddHandler Control.MouseEnter, Sub()
                                               Control.BackColor = Color.FromArgb(
                                                    Me.ForeColor.R + Globals.Unit(1),
                                                    Me.ForeColor.G + Globals.Unit(1),
                                                    Me.ForeColor.B + Globals.Unit(1)
                                                )
                                           End Sub
            AddHandler Control.MouseLeave, Sub()
                                               Control.BackColor = Me.ForeColor
                                           End Sub
        Next
        Dim ControlBoxTriangle As New PictureBox With {
            .Dock = DockStyle.Right,
            .Size = New Size(Globals.Unit(1), Globals.Unit(1)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Padding = New Padding(0),
            .Margin = New Padding(0),
            .Image = Globals.LoadSvgFromResource("Header Triangle ControlBox", New Size(Globals.Unit(1), Globals.Unit(1))).Draw()
        }

        Me.HeaderBar.Controls.Add(ControlBoxTriangle)
        Me.HeaderBar.Controls.Add(ControlBox)


        AddHandler Me.HeaderBar.MouseDown, AddressOf HeaderBar_MouseDown
        AddHandler Me.HeaderBar.MouseMove, AddressOf HeaderBar_MouseMove
        AddHandler Me.HeaderBar.MouseUp, AddressOf HeaderBar_MouseUp


        Me.resizerBarTop = New Panel With {
            .Dock = DockStyle.Top,
            .Height = Globals.Unit(0.1),
            .Cursor = Cursors.SizeNS,
            .BackColor = Me.ForeColor,
            .Name = "resizerBarTop"
        }
        Me.Controls.Add(resizerBarTop)
        AddHandler Me.resizerBarTop.MouseDown, AddressOf ResizerBarTop_MouseDown
        AddHandler Me.resizerBarTop.MouseMove, AddressOf ResizerBarTop_MouseMove
        AddHandler Me.resizerBarTop.MouseUp, AddressOf ResizerBarTop_MouseUp
        Me.resizerBarBottom = New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = Globals.Unit(0.1),
            .Cursor = Cursors.SizeNS,
            .BackColor = Me.ForeColor,
            .Name = "resizerBarBottom"
        }
        Me.Controls.Add(resizerBarBottom)
        AddHandler Me.resizerBarBottom.MouseDown, AddressOf ResizerBarBottom_MouseDown
        AddHandler Me.resizerBarBottom.MouseMove, AddressOf ResizerBarBottom_MouseMove
        AddHandler Me.resizerBarBottom.MouseUp, AddressOf ResizerBarBottom_MouseUp

        Me.resizerBarLeft = New Panel With {
            .Dock = DockStyle.Left,
            .Width = Globals.Unit(0.1),
            .Cursor = Cursors.SizeWE,
            .BackColor = Me.ForeColor,
            .Name = "resizerBarLeft"
        }
        Me.Controls.Add(resizerBarLeft)
        AddHandler Me.resizerBarLeft.MouseDown, AddressOf ResizerBarLeft_MouseDown
        AddHandler Me.resizerBarLeft.MouseMove, AddressOf ResizerBarLeft_MouseMove
        AddHandler Me.resizerBarLeft.MouseUp, AddressOf ResizerBarLeft_MouseUp
        Me.resizerBarRight = New Panel With {
            .Dock = DockStyle.Right,
            .Width = Globals.Unit(0.1),
            .Cursor = Cursors.SizeWE,
            .BackColor = Me.ForeColor,
            .Name = "resizerBarRight"
        }
        Me.Controls.Add(resizerBarRight)
        AddHandler Me.resizerBarRight.MouseDown, AddressOf ResizerBarRight_MouseDown
        AddHandler Me.resizerBarRight.MouseMove, AddressOf ResizerBarRight_MouseMove
        AddHandler Me.resizerBarRight.MouseUp, AddressOf ResizerBarRight_MouseUp



        Me.Contents = New Panel With {
            .BackColor = Me.BackColor,
            .AutoScroll = True,
            .BackgroundImageLayout = ImageLayout.Zoom,
            .Dock = DockStyle.Fill
        }
        AddHandler Me.Resize, Sub()
                                  Me.Contents.BackgroundImage = BackgroundBitmap
                              End Sub
        AddHandler Me.Contents.Click, Sub()
                                          Me.Contents.Focus()
                                      End Sub
        Contents.SendToBack()
        Me.Controls.Add(Contents)

        LoopChildrenAddMouseHeaderEvents(HeaderBar)

        Me.ShowInTaskbar = True
    End Sub

    Public Contents As Panel

    Public HeaderBar As Panel
    Public resizerBarTop As Panel
    Public resizerBarBottom As Panel
    Public resizerBarLeft As Panel
    Public resizerBarRight As Panel

    Private Sub LoopChildrenAddMouseHeaderEvents(Control As Control)
        AddHandler Control.MouseDown, AddressOf HeaderBar_MouseDown
        AddHandler Control.MouseMove, AddressOf HeaderBar_MouseMove
        AddHandler Control.MouseUp, AddressOf HeaderBar_MouseUp
        For Each Child As Control In Control.Controls
            LoopChildrenAddMouseHeaderEvents(Child)
        Next
    End Sub


    Protected Sub BaseForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Application.Exit()
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



    Public Is_ResizerBarTop_MouseDown As Boolean = False
    Protected Sub ResizerBarTop_MouseDown(sender As Object, e As MouseEventArgs)
        Is_ResizerBarTop_MouseDown = True
    End Sub
    Protected Sub ResizerBarTop_MouseMove(sender As Object, e As MouseEventArgs)
        If Is_ResizerBarTop_MouseDown Then
            Dim DistanceBetweenCursorAndFormY = Cursor.Position.Y - Me.Location.Y

            If Me.Height - DistanceBetweenCursorAndFormY > Me.MinimumSize.Height Then
                Me.Height -= DistanceBetweenCursorAndFormY
                Me.Location = New Point(
                    Me.Location.X,
                    Me.Location.Y + DistanceBetweenCursorAndFormY
                )
            End If
        End If
    End Sub
    Protected Sub ResizerBarTop_MouseUp(sender As Object, e As MouseEventArgs)
        Is_ResizerBarTop_MouseDown = False
    End Sub

    Public Is_ResizerBarBottom_MouseDown As Boolean = False
    Protected Sub ResizerBarBottom_MouseDown(sender As Object, e As MouseEventArgs)
        Is_ResizerBarBottom_MouseDown = True
    End Sub
    Protected Sub ResizerBarBottom_MouseMove(sender As Object, e As MouseEventArgs)
        If Is_ResizerBarBottom_MouseDown Then
            Dim DistanceBetweenCursorAndFormY = Cursor.Position.Y - Me.Location.Y - Me.Size.Height

            If Me.Height + DistanceBetweenCursorAndFormY > Me.MinimumSize.Height Then
                Me.Height += DistanceBetweenCursorAndFormY
            End If
        End If
    End Sub
    Protected Sub ResizerBarBottom_MouseUp(sender As Object, e As MouseEventArgs)
        Is_ResizerBarBottom_MouseDown = False
    End Sub

    Public Is_ResizerBarLeft_Mousedown As Boolean = False
    Protected Sub ResizerBarLeft_MouseDown(sender As Object, e As MouseEventArgs)
        Is_ResizerBarLeft_Mousedown = True
    End Sub
    Protected Sub ResizerBarLeft_MouseMove(sender As Object, e As MouseEventArgs)
        If Is_ResizerBarLeft_Mousedown Then
            Dim DistanceBetweenCursorAndFormX = Cursor.Position.X - Me.Location.X

            If Me.Width - DistanceBetweenCursorAndFormX > Me.MinimumSize.Width Then
                Me.Width -= DistanceBetweenCursorAndFormX
                Me.Location = New Point(
                    Me.Location.X + DistanceBetweenCursorAndFormX,
                    Me.Location.Y
                )
            End If
        End If
    End Sub
    Protected Sub ResizerBarLeft_MouseUp(sender As Object, e As MouseEventArgs)
        Is_ResizerBarLeft_Mousedown = False
    End Sub
    Public Is_ResizerBarRight_Mousedown As Boolean = False
    Protected Sub ResizerBarRight_MouseDown(sender As Object, e As MouseEventArgs)
        Is_ResizerBarRight_Mousedown = True
    End Sub
    Protected Sub ResizerBarRight_MouseMove(sender As Object, e As MouseEventArgs)
        If Is_ResizerBarRight_Mousedown Then
            Dim DistanceBetweenCursorAndFormX = Cursor.Position.X - Me.Location.X - Me.Size.Width

            If Me.Width + DistanceBetweenCursorAndFormX > Me.MinimumSize.Width Then
                Me.Width += DistanceBetweenCursorAndFormX
            End If
        End If
    End Sub
    Protected Sub ResizerBarRight_MouseUp(sender As Object, e As MouseEventArgs)
        Is_ResizerBarRight_Mousedown = False
    End Sub



    Protected Sub BaseForm_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Try
            Dim SideButton = Me.Controls.Find("SizeButton", True)(0)

            If SideButton IsNot Nothing Then
                If Me.WindowState = FormWindowState.Maximized Then
                    SideButton.BackgroundImage = Globals.LoadSvgFromResource("Minimize Window", New Size(Globals.Unit(1.5), Globals.Unit(1))).Draw()
                Else
                    SideButton.BackgroundImage = Globals.LoadSvgFromResource("Maximize Window", New Size(Globals.Unit(1.5), Globals.Unit(1))).Draw()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class