Imports System.Drawing.Drawing2D
Imports System.Resources
Imports System.Text.RegularExpressions
Imports Svg

Public Class Dashboard
    Inherits BaseForm

    Private MainPanel As FlowLayoutPanel

    Class DashboardItem
        Inherits Transparent.Panel

        Public VisitButton As New BaseButton With {
            .Text = "Visit",
            .Location = New Point(Globals.Unit(5), Globals.Unit(8))
        }

        Property ProgramHead As String
            Get
                Return Me.Controls("ProgramHead").Text
            End Get
            Set(value As String)
                Me.Controls("ProgramHead").Text = value
            End Set
        End Property
        Property Courses As String
            Get
                Return Me.Controls("Courses").Text
            End Get
            Set(value As String)
                Me.Controls("Courses").Text = value
            End Set
        End Property
        Property Faculties As String
            Get
                Return Me.Controls("Faculties").Text
            End Get
            Set(value As String)
                Me.Controls("Faculties").Text = value
            End Set
        End Property
        Property Facilities As String
            Get
                Return Me.Controls("Facilities").Text
            End Get
            Set(value As String)
                Me.Controls("Facilities").Text = value
            End Set
        End Property
        Property Students As String
            Get
                Return Me.Controls("Students").Text
            End Get
            Set(value As String)
                Me.Controls("Students").Text = value
            End Set
        End Property

        Property Header As String
            Get
                Return Me.Controls("Header").Text
            End Get
            Set(value As String)
                Me.Controls("Header").Text = value
            End Set
        End Property

        Public Sub New()
            Me.Width = Globals.Unit(13)
            Me.Height = Globals.Unit(10)
            Me.Margin = New Padding(0)
            Me.Padding = New Padding(0)
            Me.BorderStyle = BorderStyle.None

            Dim Header As New Label With {
                .Text = "Dashboard Item",
                .Font = Globals.GetFont("Raleway", Globals.Unit(1.5 / 2), FontStyle.Bold),
                .BackColor = Globals.Palette("Secondary"),
                .ForeColor = Globals.Palette("Plain Light"),
                .Height = Globals.Unit(1),
                .Width = Me.Width,
                .TextAlign = ContentAlignment.MiddleCenter,
                .Margin = New Padding(0),
                .Padding = New Padding(0),
                .Name = "Header"
            }
            Me.Controls.Add(Header)

            Me.Controls.Add(New Label With {
                            .Text = "Program Head",
                            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleLeft,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(1), Globals.Unit(2))
                        })
            Me.Controls.Add(New Label With {
                            .Text = "Courses",
                            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleLeft,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(1), Globals.Unit(3))
                        })
            Me.Controls.Add(New Label With {
                            .Text = "Faculties",
                            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleLeft,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(1), Globals.Unit(4))
                        })
            Me.Controls.Add(New Label With {
                            .Text = "Facilities",
                            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleLeft,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(1), Globals.Unit(5))
                        })
            Me.Controls.Add(New Label With {
                            .Text = "Students",
                            .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleLeft,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(1), Globals.Unit(6))
                        })

            Me.Controls.Add(New Label With {
                            .Text = "Dr. Juan Dela Cruz",
                            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleRight,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(6), Globals.Unit(2)),
                            .Name = "ProgramHead"
                        })
            Me.Controls.Add(New Label With {
                            .Text = "10",
                            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleRight,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(6), Globals.Unit(3)),
                            .Name = "Courses"
                        })
            Me.Controls.Add(New Label With {
                            .Text = "5",
                            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleRight,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(6), Globals.Unit(4)),
                            .Name = "Faculties"
                        })
            Me.Controls.Add(New Label With {
                            .Text = "3",
                            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleRight,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(6), Globals.Unit(5)),
                            .Name = "Facilities"
                        })
            Me.Controls.Add(New Label With {
                            .Text = "100",
                            .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                            .ForeColor = Globals.Palette("Plain Dark"),
                            .Height = Globals.Unit(1),
                            .Width = Globals.Unit(5),
                            .TextAlign = ContentAlignment.MiddleRight,
                            .Margin = New Padding(0),
                            .Padding = New Padding(0),
                            .Location = New Point(Globals.Unit(6), Globals.Unit(6)),
                            .Name = "Students"
                        })

            Me.Controls.Add(VisitButton)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Dim g As Graphics = e.Graphics
            Dim Pen As New Pen(Globals.Palette("Secondary"), Globals.Unit(0.1))

            g.DrawRectangle(Pen, 0, 0, Me.Width - Pen.Width, Me.Height - Pen.Width)
        End Sub
    End Class

    Private BSIT_DashboardItem As DashboardItem
    Private BSCpE_DashboardItem As DashboardItem

    Protected Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Name = "Dashboard"

        MainPanel = New FlowLayoutPanel With {
            .Height = Globals.Unit(10),
            .Width = Globals.Unit(28)
        }
        Me.Contents.Controls.Add(MainPanel)

        Dim BSIT_DashboardItem As New DashboardItem With {
            .Header = "BSIT Dashboard",
            .Margin = New Padding(0, 0, Globals.Unit(2), 0)
        }
        AddHandler BSIT_DashboardItem.VisitButton.Click, Sub()
                                                             Globals.PROGRAM = "bsit"
                                                             Me.GoToForm(New Dashboard_Calendar)
                                                         End Sub
        Invoke(Sub()
                   Try
                       Dim response = Globals.API("GET", "admin/dashboard/program/bsit", Nothing)
                       Dim data = Globals.JSONToDictionary(response)

                       BSIT_DashboardItem.ProgramHead = data("programHead")
                       BSIT_DashboardItem.Courses = data("courses")
                       BSIT_DashboardItem.Faculties = data("faculties")
                       BSIT_DashboardItem.Facilities = data("facilities")
                       BSIT_DashboardItem.Students = data("students")
                   Catch ex As Exception
                       Dim Modal As New BaseModal With {
                                  .Title = "Error",
                                  .Message = "An error occurred while fetching the data.",
                                  .Buttons = New Dictionary(Of String, DialogResult) From {
                                      {"OK", DialogResult.OK}
                                  }
                              }
                       Modal.ShowDialog()
                   End Try
               End Sub)
        MainPanel.Controls.Add(BSIT_DashboardItem)
        Dim BSCpE_DashboardItem As New DashboardItem With {
            .Header = "BSCpE Dashboard"
        }
        AddHandler BSCpE_DashboardItem.VisitButton.Click, Sub()
                                                              Globals.PROGRAM = "bscpe"
                                                              Me.GoToForm(New Dashboard_Calendar)
                                                          End Sub
        Invoke(Sub()
                   Try
                       Dim response = Globals.API("GET", "admin/dashboard/program/bscpe", Nothing)
                       Dim data = Globals.JSONToDictionary(response)

                       BSCpE_DashboardItem.ProgramHead = data("programHead")
                       BSCpE_DashboardItem.Courses = data("courses")
                       BSCpE_DashboardItem.Faculties = data("faculties")
                       BSCpE_DashboardItem.Facilities = data("facilities")
                       BSCpE_DashboardItem.Students = data("students")
                   Catch ex As Exception
                       Dim Modal As New BaseModal With {
               .Title = "Error",
               .Message = "An error occurred while fetching the data.",
               .Buttons = New Dictionary(Of String, DialogResult) From {
                   {"OK", DialogResult.OK}
               }
           }
                       Modal.ShowDialog()
                   End Try
               End Sub)
        MainPanel.Controls.Add(BSCpE_DashboardItem)

        Dim ResetButton As New BaseButton With {
            .Text = "Reset"
        }
        Me.Contents.Controls.Add(ResetButton)
        AddHandler Me.Resize, Sub()
                                  ResetButton.Location = New Point(
                                    Me.Contents.Width / 2 - ResetButton.Width / 2,
                                    Me.Contents.Height - ResetButton.Height - Globals.Unit(2)
                                  )
                              End Sub
        AddHandler ResetButton.Click, Sub()
                                          Dim Modal As New BaseModal With {
                                              .Title = "Reset System",
                                              .Message = "Are you sure you want to reset the system?",
                                              .Buttons = New Dictionary(Of String, DialogResult) From {
                                                  {"Yes", DialogResult.Yes},
                                                  {"No", DialogResult.No}
                                              }
                                          }
                                          If Modal.ShowDialog() = DialogResult.Yes Then
                                              Try
                                                  Dim response = Globals.API("POST", "reset", Nothing)
                                                  Dim data = Globals.JSONToDictionary(response)
                                                  Dim Modal2 As New BaseModal With {
                                                      .Title = "System Reset",
                                                      .Message = data("message"),
                                                      .Buttons = New Dictionary(Of String, DialogResult) From {
                                                          {"OK", DialogResult.OK}
                                                      }
                                                  }
                                                  Modal2.ShowDialog()
                                              Catch ex As Exception
                                                  Dim Modal2 As New BaseModal With {
                                                      .Title = "Error",
                                                      .Message = "An error occurred while resetting the system.",
                                                      .Buttons = New Dictionary(Of String, DialogResult) From {
                                                          {"OK", DialogResult.OK}
                                                      }
                                                  }
                                                  Modal2.ShowDialog()
                                              End Try
                                          End If
                                      End Sub

        'Check if the system is already set up
        Try
            Dim response = Globals.API("GET", "setup", Nothing)
        Catch ex As Exception
            Dim Modal As New BaseModal With {
                .Title = "System Setup",
                .Message = "The system is not set up. Please set up the system before logging in.",
                .Buttons = New Dictionary(Of String, DialogResult) From {
                    {"OK", DialogResult.OK},
                    {"Cancel", DialogResult.Cancel}
                }
            }
            If Modal.ShowDialog() = DialogResult.OK Then
                Me.GoToForm(New DeanSetup)
            Else
                Me.Close()
            End If
        End Try
        Loaded = True
        Me.Size = Globals.FormSize
    End Sub

    Protected Sub Dashboard_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Loaded Then
            Me.MainPanel.Location = New Point(
                (Me.Contents.Width - Me.MainPanel.Width) / 2,
                (Me.Contents.Height - Me.MainPanel.Height) / 2
            )

            Dim Background As New Bitmap(Me.Contents.Width, Me.Contents.Height)
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