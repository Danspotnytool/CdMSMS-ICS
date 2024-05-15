Imports System.IO
Imports System.Net
Imports System.Resources
Imports System.Text.RegularExpressions
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

        Dim NamePanel As New Transparent.Panel With {
            .Size = New Size(Me.FormPanel.Width - Globals.Unit(2), Globals.Unit(1))
        }
        Me.FormPanel.Controls.Add(NamePanel)
        Dim FirstNameInput As New BaseTextInput With {
            .Name = "First Name",
            .Size = New Size((NamePanel.Width / 2) - Globals.Unit(0.25), Globals.Unit(1))
        }
        NamePanel.Controls.Add(FirstNameInput)
        Dim LastNameInput As New BaseTextInput With {
            .Name = "Last Name",
            .Size = New Size((NamePanel.Width / 2) - Globals.Unit(0.25), Globals.Unit(1))
        }
        LastNameInput.Location = New Point(NamePanel.Width - LastNameInput.Width, 0)
        NamePanel.Controls.Add(LastNameInput)
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
                                          If FirstNameInput.Text = "" And FirstNameInput.Text.Length < 3 Then
                                              FirstNameInput.Alert()
                                              Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "First Name must be at least 3 characters."
                                              }
                                              Modal.ShowDialog()
                                              Exit Sub
                                          End If
                                          If LastNameInput.Text = "" And LastNameInput.Text.Length < 2 Then
                                              LastNameInput.Alert()
                                              Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Last Name must be at least 2 characters."
                                              }
                                              Modal.ShowDialog()
                                              Exit Sub
                                          End If
                                          If EmailInput.Text = "" And Not New Regex("^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").IsMatch(EmailInput.Text) Then
                                              EmailInput.Alert()
                                              Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Invalid email address."
                                              }
                                              Modal.ShowDialog()
                                              Exit Sub
                                          End If
                                          If PasswordInput.Text = "" And PasswordInput.Text.Length < 8 Then
                                              PasswordInput.Alert()
                                              Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Password must be at least 8 characters."
                                              }
                                              Modal.ShowDialog()
                                              Exit Sub
                                          End If
                                          If PasswordInput.Text <> ConfirmPasswordInput.Text Then
                                              ConfirmPasswordInput.Alert()
                                              Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Passwords do not match."
                                              }
                                              Modal.ShowDialog()
                                              Exit Sub
                                          End If

                                          Dim data As New Dictionary(Of String, String) From {
                                              {"firstName", FirstNameInput.Text},
                                              {"lastName", LastNameInput.Text},
                                              {"email", EmailInput.Text},
                                              {"password", PasswordInput.Text},
                                              {"role", "dean"}
                                          }
                                          Try
                                              Dim response As String = Globals.API("POST", "setup/admin", Globals.DictionaryToJSON(data))
                                              Me.GoToForm(New BSIT_ProgramHeadSetup)
                                          Catch ex As WebException
                                              Dim rep As HttpWebResponse = ex.Response
                                              Using rdr As New StreamReader(rep.GetResponseStream())
                                                  Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = rep.StatusCode & ": " & rdr.ReadToEnd()
                                                  }
                                                  Modal.ShowDialog()
                                              End Using
                                          End Try
                                      End Sub
        Loaded = True
        Me.Size = Globals.FormSize

        'Check if the system is already set up
        Try
            Dim response = Globals.API("GET", "setup", Nothing)
            Dim Modal As New BaseModal With {
                .Title = "System Setup",
                .Message = "The system is already set up. Please log in.",
                .Buttons = New Dictionary(Of String, DialogResult) From {
                    {"OK", DialogResult.OK}
                }
            }
            If Modal.ShowDialog() Then
                Me.GoToForm(New Login)
            End If
        Catch ex As Exception
        End Try
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