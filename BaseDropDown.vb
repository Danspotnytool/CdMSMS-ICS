Public Class BaseDropDown
    Inherits Transparent.Panel

    Private WithEvents ComboBox As ComboBox
    Private WithEvents Label As Label
    Private WithEvents PlaceHolderElement As Label
    Private WithEvents DropDownArrow As Transparent.PictureBox

    Property Items As List(Of String)
        Get
            Dim toReturm As New List(Of String)
            For Each item In ComboBox.Items
                toReturm.Add(item)
            Next
            Return toReturm
        End Get
        Set
            ComboBox.Items.Clear()
            For Each item In Value
                ComboBox.Items.Add(item)
            Next
        End Set
    End Property

    Property SelectedIndex As Integer
        Get
            Return ComboBox.SelectedIndex
        End Get
        Set(value As Integer)
            ComboBox.SelectedIndex = value
        End Set
    End Property

    Property SelectedItem As String
        Get
            Return ComboBox.SelectedItem
        End Get
        Set(value As String)
            ComboBox.SelectedItem = value
            PlaceHodlder = value
            RaiseEvent SelectedIndexChanged(Me, Nothing)
        End Set
    End Property

    Property Name As String
        Get
            Return Label.Text
        End Get
        Set(value As String)
            Label.Text = value
        End Set
    End Property

    Property PlaceHodlder As String
        Get
            Return PlaceHolderElement.Text
        End Get
        Set(value As String)
            PlaceHolderElement.Text = value
        End Set
    End Property

    Sub New()
        Me.BorderStyle = BorderStyle.None
        Me.Padding = New Padding(0)

        Me.DropDownArrow = New Transparent.PictureBox With {
            .Image = Globals.LoadSvgFromResource("DropDown Arrow", New Size(Globals.Unit(1), Globals.Unit(1))).Draw(),
            .BackColor = Globals.Palette("Plain Light"),
            .Size = New Size(Globals.Unit(0.75), Globals.Unit(0.5)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Cursor = Cursors.Hand
        }
        Me.Controls.Add(Me.DropDownArrow)

        Me.Label = New Label With {
                .Location = New Point(0, 0),
                .Size = New Size(Me.Width, Globals.Unit(0.5)),
                .Text = "ComboBox",
                .TextAlign = ContentAlignment.MiddleLeft,
                .ForeColor = Globals.Palette("Plain Dark"),
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold)
            }
        Me.Controls.Add(Me.Label)

        Me.PlaceHolderElement = New Label With {
                .Location = New Point(0, Label.Height + Globals.Unit(0.25)),
                .Size = New Size(Me.Width, Globals.Unit(0.75)),
                .TextAlign = ContentAlignment.MiddleLeft,
                .BackColor = Globals.Palette("Plain Light"),
                .ForeColor = Globals.Palette("Plain Dark"),
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                .Padding = New Padding(Globals.Unit(0.25), 0, Globals.Unit(0.25), 0)
            }
        Me.Controls.Add(Me.PlaceHolderElement)

        Me.ComboBox = New ComboBox With {
                .Location = New Point(0, Label.Height + Globals.Unit(0.25)),
                .Size = New Size(Me.Width, Globals.Unit(0.75)),
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .FlatStyle = FlatStyle.Flat,
                .BackColor = Globals.Palette("Plain Light"),
                .ForeColor = Globals.Palette("Plain Dark"),
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular)
            }
        Me.Controls.Add(Me.ComboBox)

        AddHandler PlaceHolderElement.Click, Sub()
                                                 Me.ComboBox.DroppedDown = True
                                             End Sub
        Me.DropDownArrow.Location = New Point(
            Me.Width - Me.DropDownArrow.Width - Globals.Unit(0.25),
            Me.PlaceHolderElement.Top + (Me.PlaceHolderElement.Height / 2 - Me.DropDownArrow.Height / 2)
        )
    End Sub

    Protected Sub ComboBox_DropDownClosed(sender As Object, e As EventArgs) Handles ComboBox.DropDownClosed
        If Me.ComboBox.SelectedIndex = -1 Then
            Me.PlaceHolderElement.Text = Me.PlaceHodlder
            Me.SelectedItem = ""
        Else
            Me.PlaceHolderElement.Text = Me.ComboBox.SelectedItem
            Me.SelectedItem = Me.ComboBox.SelectedItem
        End If

        RaiseEvent SelectedIndexChanged(Me, e)
    End Sub

    Public Event SelectedIndexChanged(sender As Object, e As EventArgs)

    Protected Sub ComboBox_Resized(sender As Object, e As EventArgs) Handles Me.Resize
        Me.Label.Size = New Size(Me.Width, Me.Label.Height)
        Me.PlaceHolderElement.Size = New Size(Me.Width, Me.PlaceHolderElement.Height)
        Me.ComboBox.Size = New Size(Me.Width, Me.ComboBox.Height)
        Me.DropDownArrow.Location = New Point(
            Me.Width - Me.DropDownArrow.Width - Globals.Unit(0.25),
            Me.PlaceHolderElement.Top + (Me.PlaceHolderElement.Height / 2 - Me.DropDownArrow.Height / 2)
        )
    End Sub

    Private Timer As Timer
    Private TimerToStop As Timer
    Public Sub Alert()
        Dim Alerted As Boolean = False
        Me.Timer = New Timer With {
                    .Interval = Globals.Unit(3)
                }
        AddHandler Timer.Tick, Sub()
                                   If Alerted Then
                                       Me.PlaceHolderElement.BackColor = Globals.Palette("Plain Light")
                                       Alerted = False
                                   Else
                                       Me.PlaceHolderElement.BackColor = Globals.Palette("Primary Compliment")
                                       Alerted = True
                                   End If
                               End Sub
        Timer.Start()
        Me.TimerToStop = New Timer With {
            .Interval = Globals.Unit(50)
        }
        AddHandler TimerToStop.Tick, Sub()
                                         Timer.Stop()
                                         TimerToStop.Stop()
                                         Me.PlaceHolderElement.BackColor = Globals.Palette("Plain Light")
                                     End Sub
        TimerToStop.Start()
        Me.ComboBox.DroppedDown = True
        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Asterisk)
    End Sub
End Class
