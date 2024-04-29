Imports System.Resources
Imports Svg

Public Class DeanSetup
    Inherits BaseForm

    Private FormPanel As New Transparent.FlowLayoutPanel

    Protected Sub DeanSetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Name = "Dean Setup"

        Me.FormPanel.MinimumSize = New Size(
            Me.Width * 0.5,
            0
        )
        Me.FormPanel.MaximumSize = New Size(
            Me.Width * 0.5,
            0
        )
        Me.FormPanel.AutoSize = True
        Me.FormPanel.Location = New Point(
            CInt((Me.Width * 0.75 - Me.FormPanel.Width * 0.5) - Globals.Unit(2)),
            CInt(Me.Height * 0.5 - Me.FormPanel.Height * 0.5)
        )
        Me.Contents.Controls.Add(Me.FormPanel)

        Dim Logo As New PictureBox With {
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Name = "Logo"
        }
        Dim resourcesManager As ResourceManager = My.Resources.ResourceManager
        Dim LogoImage As Image = resourcesManager.GetObject("CdMSMS-ICS Logo")
        Logo.Image = LogoImage
        Logo.Size = New Size(Me.FormPanel.Width, CInt(LogoImage.Size.Height * (Me.FormPanel.Width / LogoImage.Size.Width)))
        Me.FormPanel.Controls.Add(Logo)

        Dim Intro As New Transparent.Label With {
            .Text = "Let’s begin by registering the Dean account.",
            .MaximumSize = New Size(Me.FormPanel.Width, 0),
            .MinimumSize = New Size(Me.FormPanel.Width, 0),
            .AutoSize = True,
            .Font = Globals.GetFont("Raleway", Globals.Unit(1), FontStyle.Bold),
            .ForeColor = Globals.Palette("Primary"),
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.FormPanel.Controls.Add(Intro)

        Dim NameInput As New BaseTextInput With {
            .Name = "Name",
            .Size = New Size(Me.FormPanel.Width - Globals.Unit(2), Globals.Unit(1))
        }
        Me.FormPanel.Controls.Add(NameInput)
        Dim EmailInput As New BaseTextInput With {
            .Name = "Email",
            .Size = New Size(Me.FormPanel.Width - Globals.Unit(2), Globals.Unit(1))
        }
        Me.FormPanel.Controls.Add(EmailInput)
        Dim PasswordInput As New BaseTextInput With {
            .Name = "Password",
            .Size = New Size(Me.FormPanel.Width - Globals.Unit(2), Globals.Unit(1)),
            .PasswordChar = "*"
        }
        Me.FormPanel.Controls.Add(PasswordInput)
        Dim ConfirmPasswordInput As New BaseTextInput With {
            .Name = "Confirm Password",
            .Size = New Size(Me.FormPanel.Width - Globals.Unit(2), Globals.Unit(1)),
            .PasswordChar = "*"
        }
        Me.FormPanel.Controls.Add(ConfirmPasswordInput)

        Dim SetupButton As New BaseButton With {
            .Text = "Setup",
            .Name = "Setup"
        }
        Me.FormPanel.Controls.Add(SetupButton)
        AddHandler SetupButton.Click, Sub()
                                          Me.GoToForm(New BSIT_ProgramHeadSetup)
                                      End Sub



        Loaded = True
        Me.Size = Globals.FormSize
    End Sub

    Protected Sub DeanSetup_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Loaded Then
            Dim i As Integer = 0
            For Each Control As Control In Me.FormPanel.Controls
                If Control.Name = "Logo" Then
                    Dim Logo As PictureBox = Control
                    Dim resourcesManager As ResourceManager = My.Resources.ResourceManager
                    Dim LogoImage As Image = resourcesManager.GetObject("CdMSMS-ICS Logo")
                    Logo.Image = LogoImage
                    Logo.Size = New Size(Me.FormPanel.Width, CInt(LogoImage.Height * (Me.FormPanel.Width / LogoImage.Width)))
                End If

                Control.Margin = New Padding(
                    (Me.FormPanel.Width - Control.Width) * 0.5,
                    0,
                    0,
                    0
                )

                If i > 1 Then
                    Control.Margin = New Padding(
                        Control.Margin.Left,
                        Globals.Unit(1),
                        0,
                        0
                    )
                End If

                i = i + 1
            Next

            Me.FormPanel.Location = New Point(
                CInt((Me.Width * 0.75 - Me.FormPanel.Width * 0.5) - Globals.Unit(2)),
                CInt(Me.Height * 0.5 - Me.FormPanel.Height * 0.5)
            )



            Dim Background As New Bitmap(Me.Contents.Width, Me.Contents.Height)
            Dim SetupGraphics = Globals.LoadSvgFromResource("Setup Graphics").Draw()
            Dim HalfTrapezoid = Globals.LoadSvgFromResource("Half Trapezoid").Draw()
            Dim BarCompliment_Top = Globals.LoadSvgFromResource("Bar Complement").Draw()
            Dim Bar_Top = Globals.LoadSvgFromResource("Bar").Draw()
            Dim BarCompliment_Bottom = Globals.LoadSvgFromResource("Bar Complement Bottom").Draw()
            Dim Bar_Bottom = Globals.LoadSvgFromResource("Bar Bottom").Draw()

            Using g As Graphics = Graphics.FromImage(Background)
                'HalfTrapezoid
                g.DrawImage(
                    HalfTrapezoid,
                    0,
                    0,
                    CInt(HalfTrapezoid.Width * (Me.Contents.Height / HalfTrapezoid.Height)),
                    Me.Contents.Height
                )
                g.DrawImage(
                    BarCompliment_Top,
                    0,
                    -CInt(Globals.Unit(12)),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                g.DrawImage(
                    Bar_Top,
                    CInt(Me.Width * 0.5),
                    -CInt(Globals.Unit(12)),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                g.DrawImage(
                    BarCompliment_Bottom,
                    CInt(Me.Width * 0.25),
                    CInt(Me.Height * 0.75),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                g.DrawImage(
                    Bar_Bottom,
                    CInt(Me.Width * 0.75),
                    CInt(Me.Height * 0.75),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                'SetupGraphics
                g.DrawImage(
                    SetupGraphics,
                    0,
                    CInt((Me.Contents.Height / 2) - ((Me.Width * 0.5) / 2)),
                    CInt(Me.Width * 0.5),
                    CInt(Globals.Unit(19) * (Me.Width * 0.5 / Globals.Unit(19)))
                )
            End Using

            Me.BackgroundBitmap = Background
        End If
    End Sub
End Class