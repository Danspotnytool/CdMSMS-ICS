﻿Public Class BaseTextInput
    Inherits Control

    Private Label As New Transparent.Label
    Private TextBox As New Transparent.TextBox
    Private ReadOnly Underline As New Panel

    Overloads Property Name As String
        Get
            Return Me.Label.Text
        End Get
        Set(value As String)
            Me.Label.Text = value
        End Set
    End Property
    Overloads Property Text As String
        Get
            Return Me.TextBox.Text
        End Get
        Set(value As String)
            Me.TextBox.Text = value
        End Set
    End Property
    Overloads Property PasswordChar As Char
        Get
            Return Me.TextBox.PasswordChar
        End Get
        Set(value As Char)
            Me.TextBox.PasswordChar = value
        End Set
    End Property

    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        Me.BackColor = Color.Transparent

        Me.Controls.Add(Me.Label)
        Me.Controls.Add(Me.TextBox)
        Me.Controls.Add(Me.Underline)

        If Me.Size = New Size(0, 0) Then
            Me.Size = New Size(Globals.Unit(10), Globals.Unit(1))
        End If

        Me.Label.Location = New Point(Globals.Unit(0.25), Globals.Unit(0.25))
        Me.Label.Size = New Size(Me.Width - Globals.Unit(0.5), Globals.Unit(0.5))
        Me.Label.AutoSize = False
        Me.Label.Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold)
        Me.Label.ForeColor = Globals.Palette("Plain Dark")
        Me.Label.BackColor = Color.Transparent
        Me.Label.TextAlign = ContentAlignment.MiddleLeft

        AddHandler Me.Label.Click, AddressOf FocusTextBox

        Me.TextBox.Location = New Point(Globals.Unit(0.25), Globals.Unit(0.25))
        Me.TextBox.Size = New Size(Me.Width - Globals.Unit(0.5), Me.Height - Globals.Unit(0.25))
        Me.TextBox.AutoSize = False
        Me.TextBox.Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular)
        Me.TextBox.ForeColor = Globals.Palette("Plain Dark")
        Me.TextBox.BorderStyle = BorderStyle.None
        Me.TextBox.TextAlign = HorizontalAlignment.Left

        AddHandler Me.TextBox.GotFocus, AddressOf FocusTextBox
        AddHandler Me.TextBox.LostFocus, AddressOf UnfocusTextBox

        Me.Underline.Location = New Point(0, Me.Height - Globals.Unit(0.05))
        Me.Underline.Size = New Size(Me.Width, Globals.Unit(0.05))
        Me.Underline.BackColor = Globals.Palette("Plain Dark")
        Me.Underline.BringToFront()

        AddHandler Me.GotFocus, Sub()
                                    Me.TextBox.Focus()
                                End Sub
    End Sub

    Private Sub FocusTextBox(sender As Object, e As EventArgs)
        Me.Label.Location = New Point(Globals.Unit(0.125), 0)
        Me.Label.Size = New Size(Me.Width - Globals.Unit(0.25), Globals.Unit(0.25))
        Me.Label.Font = Globals.GetFont("Raleway", Globals.Unit(0.25), FontStyle.Bold)

        Me.BackColor = Globals.Palette("White")

        Me.Label.ForeColor = Globals.Palette("Secondary")
        Me.Underline.BackColor = Globals.Palette("Secondary")
        Me.TextBox.Focus()
    End Sub

    Private Sub UnfocusTextBox(sender As Object, e As EventArgs)
        If Me.TextBox.Text = "" Then
            Me.Label.Location = New Point(Globals.Unit(0.25), Globals.Unit(0.25))
            Me.Label.Size = New Size(Me.Width - Globals.Unit(0.5), Globals.Unit(0.5))
            Me.Label.Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold)

            Me.BackColor = Color.Transparent
        End If

        Me.Label.ForeColor = Globals.Palette("Plain Dark")
        Me.Underline.BackColor = Globals.Palette("Plain Dark")
    End Sub

    Protected Sub BaseTextInput_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Me.Label.Size = New Size(Me.Width - Globals.Unit(0.5), Globals.Unit(0.5))
        Me.TextBox.Size = New Size(Me.Width - Globals.Unit(0.5), Me.Height - Globals.Unit(0.25))
        Me.Underline.Size = New Size(Me.Width, Globals.Unit(0.05))
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
                                       Me.Underline.BackColor = Globals.Palette("Secondary")
                                       Alerted = False
                                   Else
                                       Me.Underline.BackColor = Globals.Palette("Primary Compliment")
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
                                         Me.Underline.BackColor = Globals.Palette("Plain Dark")
                                     End Sub
        TimerToStop.Start()
        Me.TextBox.Focus()
        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Asterisk)
    End Sub
End Class
