Imports System.IO
Imports System.Net
Imports System.Resources
Imports System.Text.RegularExpressions
Imports Svg

Public Class Login
    Inherits BaseForm

    Private FormPanel As New Transparent.FlowLayoutPanel
    Private MainFormPanel As New Transparent.FlowLayoutPanel

    Protected Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Name = "Login"

        Me.FormPanel.MinimumSize = New Size(
            Me.Width * 0.375,
            0
        )
        Me.FormPanel.MaximumSize = New Size(
            Me.Width * 0.375,
            0
        )
        Me.FormPanel.AutoSize = True
        Me.FormPanel.Location = New Point(
            CInt(Me.Width * 0.5 - Me.FormPanel.Width * 0.5),
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

        Me.MainFormPanel = New Transparent.FlowLayoutPanel With {
            .AutoSize = True,
            .MinimumSize = New Size(Me.FormPanel.Width - Globals.Unit(2), 0),
            .MaximumSize = New Size(Me.FormPanel.Width - Globals.Unit(2), 0)
        }
        Me.FormPanel.Controls.Add(Me.MainFormPanel)

        Dim EmailInput As New BaseTextInput With {
            .Name = "Email",
            .Size = New Size(Me.MainFormPanel.Width, Globals.Unit(1))
        }
        Me.MainFormPanel.Controls.Add(EmailInput)
        Dim PasswordInput As New BaseTextInput With {
            .Name = "Password",
            .Size = New Size(Me.MainFormPanel.Width, Globals.Unit(1)),
            .PasswordChar = "*"
        }
        Me.MainFormPanel.Controls.Add(PasswordInput)
        Dim LoginButton As New BaseButton With {
            .Text = "Login",
            .Name = "Login"
        }
        Me.MainFormPanel.Controls.Add(LoginButton)

        AddHandler LoginButton.Click, Sub()
                                          Dim Email As String = EmailInput.Text
                                          Dim Password As String = PasswordInput.Text

                                          If Email = "" Or Password = "" Then
                                              Dim Modal As New BaseModal With {
                                                  .Title = "Login",
                                                  .Message = "Please fill in all fields.",
                                                  .Buttons = New Dictionary(Of String, DialogResult) From {
                                                      {"OK", DialogResult.OK}
                                                  }
                                              }
                                              Modal.ShowDialog()
                                              Exit Sub
                                          End If

                                          Dim Data As New Dictionary(Of String, String) From {
                                              {"email", Email},
                                              {"password", Password}
                                          }

                                          Try
                                              Dim response = Globals.API("POST", "user/login", Globals.DictionaryToJSON(Data))

                                              Dim user = Globals.JSONToDictionary(response)
                                              Globals.TOKEN = user("token")

                                              If user("role") = "dean" Then
                                                  Me.GoToForm(New Dashboard)
                                              ElseIf user("role") = "bsit" Then
                                                  Globals.PROGRAM = "bsit"
                                                  Me.GoToForm(New Dashboard_Calendar)
                                              ElseIf user("role") = "bscpe" Then
                                                  Globals.PROGRAM = "bscpe"
                                                  Me.GoToForm(New Dashboard_Calendar)
                                              End If
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

        Dim i As Integer = 0
        For Each Control As Control In Me.MainFormPanel.Controls
            Control.Margin = New Padding(
                (Me.MainFormPanel.Width - Control.Width) * 0.5,
                0,
                0,
                Globals.Unit(1)
            )

            i = i + 1
        Next
        Me.MainFormPanel.Controls(Me.MainFormPanel.Controls.Count - 1).Margin = New Padding(0)

        Dim ForgotPasswordLabel As New Transparent.Label With {
            .Text = "Forgot Password?",
            .AutoSize = True,
            .MinimumSize = New Size(Me.MainFormPanel.Width, 0),
            .MaximumSize = New Size(Me.MainFormPanel.Width, 0),
            .Name = "ForgotPassword",
            .TextAlign = ContentAlignment.MiddleCenter,
            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
            .ForeColor = Globals.Palette("Plain Dark")
        }
        Me.FormPanel.Controls.Add(ForgotPasswordLabel)

        Loaded = True
        Me.Size = Globals.FormSize

        'Check if the system is already set up
        Try
            Dim response = Globals.API("GET", "setup", Nothing)
        Catch ex As WebException
            Dim rep As HttpWebResponse = ex.Response
            Using rdr As New StreamReader(rep.GetResponseStream())
                Dim Modal As New BaseModal With {
                    .Title = "Error",
                    .Message = rep.StatusCode & ": " & rdr.ReadToEnd()
                }
                Modal.ShowDialog()
            End Using

            Me.GoToForm(New DeanSetup)
        End Try
    End Sub

    Protected Sub Login_Resize(sender As Object, e As EventArgs) Handles Me.Resize
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

                If i > 0 Then
                    Control.Margin = New Padding(
                        Control.Margin.Left,
                        Globals.Unit(2),
                        0,
                        0
                    )
                End If

                i = i + 1
            Next
            Dim j As Integer = 0
            For Each Control As Control In Me.MainFormPanel.Controls
                Control.Margin = New Padding(
                            (Me.MainFormPanel.Width - Control.Width) * 0.5,
                            0,
                            0,
                            Globals.Unit(1)
                        )

                Control.Margin = New Padding(
                    Control.Margin.Left,
                    0,
                    0,
                    Globals.Unit(1)
                )

                j = j + 1
            Next
            Me.MainFormPanel.Controls(Me.MainFormPanel.Controls.Count - 1).Margin = New Padding(
                Me.MainFormPanel.Controls(Me.MainFormPanel.Controls.Count - 1).Margin.Left,
                0,
                0,
                0
            )

            Me.FormPanel.Location = New Point(
                CInt(Me.Width * 0.5 - Me.FormPanel.Width * 0.5),
                CInt(Me.Height * 0.5 - Me.FormPanel.Height * 0.5)
            )

            Dim Background As New Bitmap(Me.Width, Me.Height)
            Dim HalfTrapezoid = Globals.LoadSvgFromResource("Half Trapezoid").Draw()
            Dim BarCompliment_Top = Globals.LoadSvgFromResource("Bar Complement").Draw()
            Dim Bar_Top = Globals.LoadSvgFromResource("Bar").Draw()
            Dim BarCompliment_Bottom = Globals.LoadSvgFromResource("Bar Complement Bottom").Draw()
            Dim Bar_Bottom = Globals.LoadSvgFromResource("Bar Bottom").Draw()

            Using g As Graphics = Graphics.FromImage(Background)
                g.DrawImage(
                    BarCompliment_Top,
                    -CInt(Me.Width * 0.25),
                    -CInt(Globals.Unit(12)),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                g.DrawImage(
                    Bar_Top,
                    CInt(Me.Width - Bar_Top.Width),
                    -CInt(Globals.Unit(12)),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                g.DrawImage(
                    BarCompliment_Bottom,
                    0,
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
            End Using

            Me.BackgroundBitmap = Background
        End If
    End Sub
End Class