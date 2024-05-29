Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Net
Imports System.Resources
Imports System.Text.RegularExpressions
Imports Svg

Public Class Dashboard_Create
    Inherits BaseForm

    Private SidePanel As FlowLayoutPanel
    Private MainPanel As Transparent.Panel

    Protected Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Name = "Dashboard"

        Me.SidePanel = New FlowLayoutPanel With {
            .Size = New Size(Globals.Unit(2), Me.Contents.Height - (Me.HeaderBar.Height + Me.resizerBarTop.Height)),
            .BackColor = Globals.Palette("Plain Light"),
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .Padding = New Padding(0, 0, 0, 0),
            .Location = New Point(
                Me.resizerBarRight.Width,
                Me.HeaderBar.Height + Me.resizerBarTop.Height
            )
        }
        Me.Contents.Controls.Add(Me.SidePanel)

        Dim CallendarIcon As New PictureBox With {
            .Image = Globals.LoadSvgFromResource("Calendar Icon", New Size(Globals.Unit(2), Globals.Unit(2))).Draw(),
            .Size = New Size(Globals.Unit(2), Globals.Unit(2)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Cursor = Cursors.Hand,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }
        AddHandler CallendarIcon.Click, Sub()
                                            Me.GoToForm(New Dashboard_Calendar)
                                        End Sub
        Me.SidePanel.Controls.Add(CallendarIcon)
        Dim CreateIcon As New PictureBox With {
            .Image = Globals.LoadSvgFromResource("Create Icon Active", New Size(Globals.Unit(2), Globals.Unit(2))).Draw(),
            .Size = New Size(Globals.Unit(2), Globals.Unit(2)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Cursor = Cursors.Hand,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }
        Me.SidePanel.Controls.Add(CreateIcon)
        Dim NotificationIcon As New PictureBox With {
            .Image = Globals.LoadSvgFromResource("Notification Icon", New Size(Globals.Unit(2), Globals.Unit(2))).Draw(),
            .Size = New Size(Globals.Unit(2), Globals.Unit(2)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Cursor = Cursors.Hand,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }
        AddHandler NotificationIcon.Click, Sub()
                                               Me.GoToForm(New Dashboard_Notification)
                                           End Sub
        Me.SidePanel.Controls.Add(NotificationIcon)

        Me.MainPanel = New Transparent.Panel With {
            .Size = New Size(
                Me.Contents.Width - Me.SidePanel.Width,
                Me.Contents.Height - (Me.HeaderBar.Height + Me.resizerBarTop.Height + Me.resizerBarBottom.Height)
            ),
            .AutoScroll = True,
            .Location = New Point(
                Me.SidePanel.Width + Me.resizerBarRight.Width,
                Me.HeaderBar.Height + Me.resizerBarTop.Height
            )
        }
        Me.Contents.Controls.Add(Me.MainPanel)



        Dim YearLevel As New BaseDropDown With {
            .Items = New List(Of String),
            .Name = "Year Level",
            .Location = New Point(Globals.Unit(1), Globals.Unit(1)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select Year Level"
        }
        Invoke(Sub()
                   Try
                       Dim tempItems = New List(Of String)
                       YearLevel.Items.Clear()
                       Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/yearlevels/", Nothing)
                       Dim data = Globals.JSONToDictionary(response)

                       For Each Values In data.Values
                           tempItems.Add(Values)
                       Next

                       YearLevel.Items = tempItems
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
               End Sub)
        Me.MainPanel.Controls.Add(YearLevel)

        Dim Course As New BaseDropDown With {
            .Items = New List(Of String),
            .Name = "Course",
            .Location = New Point(Globals.Unit(1), Globals.Unit(3)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select Course"
        }
        AddHandler YearLevel.SelectedIndexChanged, Sub()
                                                       Course.Items.Clear()
                                                       Course.PlaceHodlder = "Select Course"
                                                       If YearLevel.SelectedIndex = -1 Then
                                                           Exit Sub
                                                       End If
                                                       Try
                                                           Dim tempItems = New List(Of String)
                                                           Course.Items.Clear()
                                                           Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/courses/" & YearLevel.SelectedIndex + 1, Nothing)
                                                           Dim data = Globals.JSONToDictionary(response)

                                                           For i As Integer = 0 To data.Keys.Count - 1
                                                               tempItems.Add(data.Keys(i) & " - " & data.Values(i))
                                                           Next

                                                           Course.Items = tempItems
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
        Me.MainPanel.Controls.Add(Course)

        Dim Faculty As New BaseDropDown With {
            .Items = New List(Of String),
            .Name = "Faculty",
            .Location = New Point(Globals.Unit(1), Globals.Unit(5)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select Faculty"
        }
        AddHandler YearLevel.SelectedIndexChanged, Sub()
                                                       Faculty.Items.Clear()
                                                       Faculty.PlaceHodlder = "Select Faculty"
                                                       If YearLevel.SelectedIndex = -1 Then
                                                           Exit Sub
                                                       End If
                                                       Try
                                                           Dim tempItems = New List(Of String)
                                                           Faculty.Items.Clear()
                                                           Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/faculties", Nothing)
                                                           Dim data = Globals.JSONToDictionary(response)

                                                           For i As Integer = 0 To data.Keys.Count - 1
                                                               tempItems.Add(data.Keys(i) & " - " & data.Values(i))
                                                           Next

                                                           Faculty.Items = tempItems
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
        Me.MainPanel.Controls.Add(Faculty)

        Dim Facility As New BaseDropDown With {
            .Items = New List(Of String),
            .Name = "Facility",
            .Location = New Point(Globals.Unit(1), Globals.Unit(7)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select Facility"
        }
        AddHandler YearLevel.SelectedIndexChanged, Sub()
                                                       Facility.Items.Clear()
                                                       Facility.PlaceHodlder = "Select Facility"
                                                       If YearLevel.SelectedIndex = -1 Then
                                                           Exit Sub
                                                       End If
                                                       Try
                                                           Dim tempItems = New List(Of String)
                                                           Facility.Items.Clear()
                                                           Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/facilities", Nothing)
                                                           Dim data = Globals.JSONToDictionary(response)

                                                           For i As Integer = 0 To data.Keys.Count - 1
                                                               tempItems.Add(data.Keys(i) & " - " & data.Values(i))
                                                           Next

                                                           Facility.Items = tempItems
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
        Me.MainPanel.Controls.Add(Facility)

        Dim Section As New BaseDropDown With {
            .Items = New List(Of String),
            .Name = "Section",
            .Location = New Point(Globals.Unit(1), Globals.Unit(9)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select Section"
        }
        AddHandler YearLevel.SelectedIndexChanged, Sub()
                                                       Section.Items.Clear()
                                                       Section.PlaceHodlder = "Select Section"
                                                       If YearLevel.SelectedIndex = -1 Then
                                                           Exit Sub
                                                       End If
                                                       Try
                                                           Dim tempItems = New List(Of String)
                                                           Section.Items.Clear()
                                                           Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/sections/" & YearLevel.SelectedIndex + 1, Nothing)
                                                           Dim data = Globals.JSONToDictionary(response)

                                                           For Each Values In data.Values
                                                               tempItems.Add(Values)
                                                           Next

                                                           Section.Items = tempItems
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
        Me.MainPanel.Controls.Add(Section)

        Dim Day As New BaseDropDown With {
            .Items = New List(Of String) From {
                "Sunday",
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday"
            },
            .Name = "Day",
            .Location = New Point(Globals.Unit(1), Globals.Unit(11)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select Day"
        }
        Me.MainPanel.Controls.Add(Day)

        Dim StartTime As New BaseDropDown With {
            .Items = New List(Of String) From {
                "07:00 AM",
                "07:30 AM",
                "08:00 AM",
                "08:30 AM",
                "09:00 AM",
                "09:30 AM",
                "10:00 AM",
                "10:30 AM",
                "11:00 AM",
                "11:30 AM",
                "12:00 PM",
                "12:30 PM",
                "01:00 PM",
                "01:30 PM",
                "02:00 PM",
                "02:30 PM",
                "03:00 PM",
                "03:30 PM",
                "04:00 PM",
                "04:30 PM",
                "05:00 PM",
                "05:30 PM",
                "06:00 PM",
                "06:30 PM",
                "07:00 PM"
            },
            .Name = "Start Time",
            .Location = New Point(Globals.Unit(1), Globals.Unit(13)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select Start Time"
        }
        Me.MainPanel.Controls.Add(StartTime)

        Dim EndTime As New BaseDropDown With {
            .Items = New List(Of String),
            .Name = "End Time",
            .Location = New Point(Globals.Unit(1), Globals.Unit(15)),
            .Size = New Size(Globals.Unit(12), Globals.Unit(1.5)),
            .PlaceHodlder = "Select End Time"
        }
        AddHandler StartTime.SelectedIndexChanged, Sub()
                                                       If StartTime.SelectedIndex = -1 Then
                                                           Exit Sub
                                                       End If
                                                       EndTime.Items.Clear()
                                                       EndTime.PlaceHodlder = "Select End Time"
                                                       Dim TimeItems = New List(Of String)
                                                       EndTime.Items.Clear()
                                                       Dim StartTimeIndex As Integer = StartTime.SelectedIndex
                                                       For i As Integer = StartTimeIndex + 1 To StartTime.Items.Count - 1
                                                           TimeItems.Add(StartTime.Items(i))
                                                       Next
                                                       EndTime.Items = TimeItems
                                                   End Sub
        Me.MainPanel.Controls.Add(EndTime)

        Dim SubmitButton As New BaseButton With {
            .Text = "Submit",
            .Name = "Submit",
            .Location = New Point(Globals.Unit(1), Globals.Unit(17))
        }
        AddHandler SubmitButton.Click, Sub()
                                           If YearLevel.SelectedIndex = -1 Then
                                               YearLevel.Alert()
                                               Exit Sub
                                           ElseIf Course.SelectedIndex = -1 Then
                                               Course.Alert()
                                               Exit Sub
                                           ElseIf Faculty.SelectedIndex = -1 Then
                                               Faculty.Alert()
                                               Exit Sub
                                           ElseIf Facility.SelectedIndex = -1 Then
                                               Facility.Alert()
                                               Exit Sub
                                           ElseIf Section.SelectedIndex = -1 Then
                                               Section.Alert()
                                               Exit Sub
                                           ElseIf Day.SelectedIndex = -1 Then
                                               Day.Alert()
                                               Exit Sub
                                           ElseIf StartTime.SelectedIndex = -1 Then
                                               StartTime.Alert()
                                               Exit Sub
                                           ElseIf EndTime.SelectedIndex = -1 Then
                                               EndTime.Alert()
                                               Exit Sub
                                           End If
                                           Dim Data As New Dictionary(Of String, String) From {
                                               {"yearLevel", YearLevel.SelectedItem},
                                               {"course", Course.SelectedItem},
                                               {"faculty", Faculty.SelectedItem},
                                               {"facility", Facility.SelectedItem},
                                               {"section", Section.SelectedItem},
                                               {"day", Day.SelectedItem},
                                               {"startTime", StartTime.SelectedItem},
                                               {"endTime", EndTime.SelectedItem}
                                           }
                                           Try
                                               Dim response As String = Globals.API("POST", "admin/dashboard/program/" & Globals.PROGRAM & "/schedule/", Globals.DictionaryToJSON(Data))
                                               Dim Modal As New BaseModal With {
                                                   .Title = "Success",
                                                   .Message = "Schedule created successfully."
                                               }
                                               Modal.ShowDialog()
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
        Me.MainPanel.Controls.Add(SubmitButton)


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
            Me.SidePanel.Size = New Size(Globals.Unit(2), Me.Contents.Height - (Me.HeaderBar.Height + Me.resizerBarTop.Height))
            Me.MainPanel.Size = New Size(
                Me.Contents.Width - (Me.SidePanel.Width + Me.resizerBarLeft.Width + Me.resizerBarRight.Width),
                Me.Contents.Height - (Me.HeaderBar.Height + Me.resizerBarTop.Height)
            )
        End If
    End Sub
End Class