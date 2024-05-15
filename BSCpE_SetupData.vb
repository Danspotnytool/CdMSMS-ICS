Imports System.Resources
Imports Svg
Imports System.IO
Imports System.Net

Public Class BSCpE_SetupData
    Inherits BaseForm

    Private FormPanel As New Transparent.FlowLayoutPanel

    Protected Sub BSCpE_SetupData_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Name = "Setup BSCpE Data"

        Dim Title As New Label With {
            .Text = "Import BSCpE Data",
            .AutoSize = True,
            .MinimumSize = New Size(Me.Width - Globals.Unit(4), Globals.Unit(2)),
            .MaximumSize = New Size(Me.Width - Globals.Unit(4), Globals.Unit(2)),
            .Font = Globals.GetFont("Raleway", Globals.Unit(1.5), FontStyle.Bold),
            .ForeColor = Globals.Palette("Primary"),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Name = "Title"
        }
        Title.Location = New Point(
            CInt(Me.Width * 0.5 - Title.Width * 0.5),
            Globals.Unit(2)
        )
        Me.Contents.Controls.Add(Title)

        Me.FormPanel.AutoScroll = True
        Me.FormPanel.Size = New Size(
                Me.Width - Globals.Unit(4) + SystemInformation.VerticalScrollBarWidth,
                Me.Height - Globals.Unit(9)
            )
        Me.FormPanel.Location = New Point(
            CInt(Me.Width * 0.5 - (Me.FormPanel.Width - SystemInformation.VerticalScrollBarWidth) * 0.5),
            Globals.Unit(5)
        )
        Me.Contents.Controls.Add(Me.FormPanel)

        Dim CoursesInput As New FileInputPanel With {
            .Label = "Courses",
            .Description = "Upload .csv file of courses.",
            .Format = "courseCode, description, units, yearLevel"
        }
        Me.FormPanel.Controls.Add(CoursesInput)

        Dim FacultiesInput As New FileInputPanel With {
            .Label = "Faculties",
            .Description = "Upload .csv file of faculties.",
            .Format = "facultiesID, firstName, lastName"
        }
        Me.FormPanel.Controls.Add(FacultiesInput)

        Dim FacilitiesInput As New FileInputPanel With {
            .Label = "Facilities",
            .Description = "Upload .csv file of facilities.",
            .Format = "facilityID, name, description"
        }
        Me.FormPanel.Controls.Add(FacilitiesInput)

        Dim StudentsInput As New FileInputPanel With {
            .Label = "Students",
            .Description = "Upload .csv file of students.",
            .Format = "studentID, firstName, lastName, section, courses, regularity"
        }
        Me.FormPanel.Controls.Add(StudentsInput)

        Dim i As Integer = 0
        For Each Control As Control In Me.FormPanel.Controls
            Control.MinimumSize = New Size((Me.FormPanel.Width - SystemInformation.VerticalScrollBarWidth) * 0.5 - Globals.Unit(1), 0)
            Control.MaximumSize = New Size((Me.FormPanel.Width - SystemInformation.VerticalScrollBarWidth) * 0.5 - Globals.Unit(1), 0)

            If i Mod 2 = 0 Then
                Control.Margin = New Padding(0, 0, 0, 0)
            Else
                Control.Margin = New Padding(Globals.Unit(2), 0, 0, 0)
            End If
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

        Dim SubmitButton As New BaseButton With {
            .Name = "Submit",
            .Text = "Submit"
        }
        SubmitButton.Location = New Point(
            Me.FormPanel.Right - SubmitButton.Width,
            Me.FormPanel.Bottom + Globals.Unit(1)
        )
        AddHandler SubmitButton.Click, Sub()
                                           Dim Data As New Dictionary(Of String, String)
                                           If CoursesInput.FilePath = "" Then
                                               Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Please upload a .csv file for courses."
                                                }
                                               Modal.ShowDialog()
                                               CoursesInput.Alert()
                                               Exit Sub
                                           Else
                                               Try
                                                   Data.Add("courses", File.ReadAllText(CoursesInput.FilePath))
                                               Catch ex As Exception
                                                   Dim Modal As New BaseModal With {
                                                        .Title = "Error",
                                                        .Message = ex.Message
                                                    }
                                                   Modal.ShowDialog()
                                                   CoursesInput.Alert()
                                                   Exit Sub
                                               End Try
                                           End If
                                           If FacultiesInput.FilePath = "" Then
                                               Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Please upload a .csv file for faculties."
                                                }
                                               Modal.ShowDialog()
                                               FacultiesInput.Alert()
                                               Exit Sub
                                           Else
                                               Try
                                                   Data.Add("faculties", File.ReadAllText(FacultiesInput.FilePath))
                                               Catch ex As Exception
                                                   Dim Modal As New BaseModal With {
                                                        .Title = "Error",
                                                        .Message = ex.Message
                                                    }
                                                   Modal.ShowDialog()
                                                   FacultiesInput.Alert()
                                                   Exit Sub
                                               End Try
                                           End If
                                           If FacilitiesInput.FilePath = "" Then
                                               Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Please upload a .csv file for facilities."
                                                }
                                               Modal.ShowDialog()
                                               FacilitiesInput.Alert()
                                               Exit Sub
                                           Else
                                               Try
                                                   Data.Add("facilities", File.ReadAllText(FacilitiesInput.FilePath))
                                               Catch ex As Exception
                                                   Dim Modal As New BaseModal With {
                                                        .Title = "Error",
                                                        .Message = ex.Message
                                                    }
                                                   Modal.ShowDialog()
                                                   FacilitiesInput.Alert()
                                                   Exit Sub
                                               End Try
                                           End If
                                           If StudentsInput.FilePath = "" Then
                                               Dim Modal As New BaseModal With {
                                                    .Title = "Error",
                                                    .Message = "Please upload a .csv file for students."
                                                }
                                               Modal.ShowDialog()
                                               StudentsInput.Alert()
                                               Exit Sub
                                           Else
                                               Try
                                                   Data.Add("students", File.ReadAllText(StudentsInput.FilePath))
                                               Catch ex As Exception
                                                   Dim Modal As New BaseModal With {
                                                           .Title = "Error",
                                                           .Message = ex.Message
                                                        }
                                                   Modal.ShowDialog()
                                                   StudentsInput.Alert()
                                                   Exit Sub
                                               End Try
                                           End If
                                           Data.Add("program", "bscpe")
                                           Try
                                               Dim response As String = Globals.API("POST", "setup/program", Globals.DictionaryToJSON(Data))
                                               Me.GoToForm(New Login)
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
        Me.Contents.Controls.Add(SubmitButton)

        Loaded = True
        Me.Size = Globals.FormSize
    End Sub

    Protected Sub BSCpE_SetupData_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Loaded Then
            Me.Contents.Controls("Title").Location = New Point(
                CInt(Me.Width * 0.5 - Me.Contents.Controls("Title").Width * 0.5),
                Globals.Unit(2)
            )
            Me.FormPanel.Size = New Size(
                Me.Width - Globals.Unit(4) + SystemInformation.VerticalScrollBarWidth,
                Me.Height - Globals.Unit(9)
            )
            Me.FormPanel.Location = New Point(
                CInt(Me.Width * 0.5 - (Me.FormPanel.Width - SystemInformation.VerticalScrollBarWidth) * 0.5),
                Globals.Unit(5)
            )
            For Each Control As Control In Me.FormPanel.Controls
                Control.MinimumSize = New Size(
                    (Me.FormPanel.Width - SystemInformation.VerticalScrollBarWidth) * 0.5 - Globals.Unit(1),
                    Control.MaximumSize.Height
                )
                Control.MaximumSize = New Size(
                    (Me.FormPanel.Width - SystemInformation.VerticalScrollBarWidth) * 0.5 - Globals.Unit(1),
                    Control.MaximumSize.Height
                )
            Next
            Me.Contents.Controls("Submit").Location = New Point(
                Me.FormPanel.Right - Me.Contents.Controls("Submit").Width,
                Me.FormPanel.Bottom + Globals.Unit(1)
            )

            Dim Background As New Bitmap(Me.Contents.Width, Me.Contents.Height)
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
                    CInt(Me.Width * 0.5),
                    -CInt(Globals.Unit(12)),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                g.DrawImage(
                    BarCompliment_Bottom,
                    -CInt(Me.Width * 0.25),
                    CInt(Me.Height * 0.75),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
                g.DrawImage(
                    Bar_Bottom,
                    CInt(Me.Width * 0.875),
                    CInt(Me.Height * 0.75),
                    CInt(Globals.Unit(13)),
                    CInt(Globals.Unit(15))
                )
            End Using

            Me.BackgroundBitmap = Background
        End If
    End Sub

    Private Class FileInputPanel
        Inherits Transparent.Panel

        Public Property Label As String
            Get
                Return Me.Controls("Label").Text
            End Get
            Set(value As String)
                Me.Controls("Label").Text = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return Me.Controls("Description").Text
            End Get
            Set(value As String)
                Me.Controls("Description").Text = value
            End Set
        End Property
        Public Property Format As String
            Get
                Return Me.Controls("Format").Text
            End Get
            Set(value As String)
                Me.Controls("Format").Text = value
            End Set
        End Property

        Public FilePath As String

        Public Sub New()
            Me.AutoSize = True

            Dim Label As New Transparent.Label With {
                .Text = "File",
                .AutoSize = True,
                .MinimumSize = New Size(Me.Width, Globals.Unit(1.25)),
                .MaximumSize = New Size(Me.Width, Globals.Unit(1.25)),
                .Location = New Point(0, 0),
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.75), FontStyle.Bold),
                .ForeColor = Globals.Palette("Secondary"),
                .TextAlign = ContentAlignment.MiddleLeft,
                .Name = "Label"
            }
            Me.Controls.Add(Label)

            Dim Description As New Transparent.Label With {
                .Text = "Upload .csv file of courses.",
                .AutoSize = True,
                .MinimumSize = New Size(Me.Width, Globals.Unit(0.75)),
                .MaximumSize = New Size(Me.Width, Globals.Unit(0.75)),
                .Location = New Point(0, Globals.Unit(1.5)),
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                .ForeColor = Globals.Palette("Plain Dark"),
                .TextAlign = ContentAlignment.MiddleLeft,
                .Name = "Description"
            }
            Me.Controls.Add(Description)

            Dim FormatLabel As New Transparent.Label With {
                .Text = "Format:",
                .AutoSize = True,
                .MinimumSize = New Size(Me.Width, Globals.Unit(0.75)),
                .MaximumSize = New Size(Me.Width, Globals.Unit(0.75)),
                .Location = New Point(0, Globals.Unit(2.5)),
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Secondary"),
                .TextAlign = ContentAlignment.MiddleLeft,
                .Name = "FormatLabel"
            }
            Me.Controls.Add(FormatLabel)
            Dim FormatDescription As New Transparent.Label With {
                .Text = "Course Code, Course Title, Course Description, Course Units",
                .AutoSize = True,
                .MinimumSize = New Size(Me.Width, Globals.Unit(0.75)),
                .MaximumSize = New Size(Me.Width, Globals.Unit(0.75)),
                .Location = New Point(0, Globals.Unit(3)),
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Regular),
                .ForeColor = Globals.Palette("Plain Dark"),
                .TextAlign = ContentAlignment.MiddleLeft,
                .Name = "Format"
            }
            Me.Controls.Add(FormatDescription)

            Dim FileInput As New BaseFileInput With {
                .AutoSize = True,
                .MaximumSize = New Size(Me.Width, Globals.Unit(3)),
                .MinimumSize = New Size(Me.Width, Globals.Unit(3)),
                .Location = New Point(0, Globals.Unit(4)),
                .Name = "FileInput"
            }
            Me.Controls.Add(FileInput)
            AddHandler FileInput.FileSelected, Sub()
                                                   Me.FilePath = FileInput.FilePath
                                               End Sub
        End Sub

        Protected Sub FileInputPanel_Resize(sender As Object, e As EventArgs) Handles Me.Resize
            For Each Control As Control In Me.Controls
                Control.MinimumSize = New Size(Me.Width, Control.MaximumSize.Height)
                Control.MaximumSize = New Size(Me.Width, Control.MaximumSize.Height)
            Next
        End Sub

        Public Sub Alert()
            Dim FileInput As BaseFileInput = Me.Controls("FileInput")
            FileInput.Alert()
        End Sub
    End Class
End Class