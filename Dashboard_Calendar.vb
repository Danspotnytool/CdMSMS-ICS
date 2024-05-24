Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.Resources
Imports System.Text.RegularExpressions
Imports System.Net
Imports Svg
Imports System.IO

Public Class Dashboard_Calendar
    Inherits BaseForm

    Private SidePanel As FlowLayoutPanel
    Private MainPanel As Transparent.Panel
    Private Calendar As Transparent.Panel

    Private Class BaseComboBox
        Inherits Transparent.Panel

        Private WithEvents ComboBoxElement As Transparent.ComboBox
        Private WithEvents PlaceHolderElement As Label

        Property PlaceHolder As String
            Get
                Return PlaceHolderElement.Text
            End Get
            Set(value As String)
                PlaceHolderElement.Text = value
            End Set
        End Property

        Property Items As List(Of String)
            Get
                Dim toReturm As New List(Of String)
                For Each item In ComboBoxElement.Items
                    toReturm.Add(item)
                Next
                Return toReturm
            End Get
            Set
                ComboBoxElement.Items.Clear()
                For Each item In Value
                    ComboBoxElement.Items.Add(item)
                Next
            End Set
        End Property

        Property SelectedIndex As Integer
            Get
                Return ComboBoxElement.SelectedIndex
            End Get
            Set(value As Integer)
                ComboBoxElement.SelectedIndex = value
                BaseComboBoxDropDownClosed(Me, Nothing)
            End Set
        End Property

        Property SelectedItem As String
            Get
                Return ComboBoxElement.SelectedItem
            End Get
            Set(value As String)
                ComboBoxElement.SelectedItem = value
            End Set
        End Property

        Public Sub New()
            Me.BackColor = Color.Yellow
            Me.PlaceHolderElement = New Transparent.Label With {
                .Size = Me.Size,
                .Location = New Point(0, 0),
                .ForeColor = Globals.Palette("Plain Light"),
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                .BackColor = Globals.Palette("Primary"),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(Me.PlaceHolderElement)

            Me.ComboBoxElement = New Transparent.ComboBox With {
                .Size = Me.Size,
                .Location = New Point(0, 0),
                .DropDownWidth = Me.Width,
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("Plain Light"),
                .FlatStyle = FlatStyle.Flat
            }
            Me.Controls.Add(Me.ComboBoxElement)

            AddHandler Me.PlaceHolderElement.Click, Sub()
                                                        Me.ComboBoxElement.DroppedDown = True
                                                    End Sub
        End Sub

        Protected Sub BaseComboBoxResize(sender As Object, e As EventArgs) Handles Me.Resize
            Me.PlaceHolderElement.Size = Me.Size
            Me.ComboBoxElement.Size = Me.Size
            Me.ComboBoxElement.DropDownWidth = Me.Width
        End Sub

        Protected Sub BaseComboBoxDropDownClosed(sender As Object, e As EventArgs) Handles ComboBoxElement.DropDownClosed
            Me.PlaceHolderElement.Text = Me.ComboBoxElement.SelectedItem
            If Me.ComboBoxElement.Items.Count = 0 Then
                Me.PlaceHolderElement.Text = Me.PlaceHolder
            End If
            RaiseEvent SelectedIndexChanged(Me, e)
        End Sub

        Public Event SelectedIndexChanged(sender As Object, e As EventArgs)
    End Class

    Shared TimestampLabelHeight As Integer = 0

    Private Class SchedulePanel
        Inherits Transparent.Panel

        Public Header As Label
        Public Description As Label
        Public Identifier As Label

        Property Color As Color
            Get
                Return Me.BackColor
            End Get
            Set(value As Color)
                Me.BorderTop.BackColor = value
                Me.BorderBottom.BackColor = value
                Me.BorderLeft.BackColor = value
                Me.BorderRight.BackColor = value

                Me.Header.BackColor = value
            End Set
        End Property

        Private BorderTop As Panel
        Private BorderBottom As Panel
        Private BorderLeft As Panel
        Private BorderRight As Panel

        Property HeaderText As String
            Get
                Return Me.Header.Text
            End Get
            Set(value As String)
                Me.Header.Text = value
            End Set
        End Property
        Property DescriptionText As String
            Get
                Return Me.Description.Text
            End Get
            Set(value As String)
                Me.Description.Text = value
            End Set
        End Property
        Property IdentifierText As String
            Get
                Return Me.Identifier.Text
            End Get
            Set(value As String)
                Me.Identifier.Text = value
            End Set
        End Property
        Property ID As String
        Property YearLevel As String
        Property Section As String
        Property FacultyID As String
        Property Facility As String
        Property Day As String
        Property StartTime As String
        Property EndTime As String

        Public Sub New(
                HeaderText As String,
                DescriptionText As String,
                IdentifierText As String,
                ID As String,
                YearLevel As String,
                Section As String,
                FacultyID As String,
                Facility As String,
                Day As String,
                StartTime As String,
                EndTime As String
            )
            Me.BackColor = Globals.Palette("Plain Light")

            Me.BorderTop = New Panel With {
                .Size = New Size(Me.Width, Globals.Unit(0.1)),
                .Location = New Point(0, 0),
                .BackColor = Me.Color
            }
            Me.Controls.Add(Me.BorderTop)
            Me.BorderBottom = New Panel With {
                .Size = New Size(Me.Width, Globals.Unit(0.1)),
                .Location = New Point(0, Me.Height - Globals.Unit(0.1)),
                .BackColor = Me.Color
            }
            Me.Controls.Add(Me.BorderBottom)
            Me.BorderLeft = New Panel With {
                .Size = New Size(Globals.Unit(0.1), Me.Height),
                .Location = New Point(0, 0),
                .BackColor = Me.Color
            }
            Me.Controls.Add(Me.BorderLeft)
            Me.BorderRight = New Panel With {
                .Size = New Size(Globals.Unit(0.1), Me.Height),
                .Location = New Point(Me.Width - Globals.Unit(0.1), 0),
                .BackColor = Me.Color
            }
            Me.Controls.Add(Me.BorderRight)

            Me.Header = New Label With {
                .Text = HeaderText,
                .Size = New Size(Me.Width, TimestampLabelHeight),
                .Location = New Point(0, 0),
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.25) + (Globals.Unit(0.25) / 2), FontStyle.Bold),
                .TextAlign = ContentAlignment.MiddleCenter,
                .ForeColor = Globals.Palette("Plain Light")
            }
            Me.Controls.Add(Me.Header)

            Me.Description = New Label With {
                .Text = DescriptionText,
                .Size = New Size(Me.Width, TimestampLabelHeight),
                .Location = New Point(0, TimestampLabelHeight),
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.25) + (Globals.Unit(0.25) / 2), FontStyle.Regular),
                .TextAlign = ContentAlignment.MiddleCenter,
                .ForeColor = Globals.Palette("Plain Dark")
            }
            Me.Controls.Add(Me.Description)

            Me.Identifier = New Label With {
                .Text = IdentifierText,
                .Size = New Size(Me.Width, TimestampLabelHeight),
                .Location = New Point(0, TimestampLabelHeight * 2),
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.25) + (Globals.Unit(0.25) / 2), FontStyle.Regular),
                .TextAlign = ContentAlignment.MiddleCenter,
                .ForeColor = Globals.Palette("Plain Dark")
            }
            Me.Controls.Add(Me.Identifier)

            AddHandler Me.Header.Click, AddressOf SchedulePanelClick
            AddHandler Me.Description.Click, AddressOf SchedulePanelClick
            AddHandler Me.Identifier.Click, AddressOf SchedulePanelClick
            AddHandler Me.BorderTop.Click, AddressOf SchedulePanelClick
            AddHandler Me.BorderBottom.Click, AddressOf SchedulePanelClick
            AddHandler Me.BorderLeft.Click, AddressOf SchedulePanelClick
            AddHandler Me.BorderRight.Click, AddressOf SchedulePanelClick

            Me.ID = ID
            Me.YearLevel = YearLevel
            Me.Section = Section
            Me.FacultyID = FacultyID
            Me.Facility = Facility
            Me.Day = Day
            Me.StartTime = StartTime
            Me.EndTime = EndTime
        End Sub

        Protected Sub SchedulePanelResize(sender As Object, e As EventArgs) Handles Me.Resize
            Me.BorderTop.Size = New Size(Me.Width, Globals.Unit(0.1))
            Me.BorderBottom.Size = New Size(Me.Width, Globals.Unit(0.5))
            Me.BorderLeft.Size = New Size(Globals.Unit(0.1), Me.Height)
            Me.BorderRight.Size = New Size(Globals.Unit(0.1), Me.Height)

            Me.BorderBottom.Location = New Point(0, Me.Height - Globals.Unit(0.1))
            Me.BorderRight.Location = New Point(Me.Width - Globals.Unit(0.1), 0)

            Me.Header.Size = New Size(Me.Width, TimestampLabelHeight)
            Me.Description.Size = New Size(Me.Width, TimestampLabelHeight)
            Me.Identifier.Size = New Size(Me.Width, TimestampLabelHeight)

            Me.Description.Location = New Point(0, TimestampLabelHeight)
            Me.Identifier.Location = New Point(0, TimestampLabelHeight * 2)
        End Sub

        Protected Sub SchedulePanelClick(sender As Object, e As EventArgs) Handles Me.Click
            Dim Parent As Panel = Me.Parent
            Dim PreviousPopupPanel As Panel = Parent.Controls.Find("PopUpPanel", True).FirstOrDefault()
            Dim SchedulesBehind As New List(Of SchedulePanel)
            If PreviousPopupPanel IsNot Nothing Then
                Parent.Controls.Remove(PreviousPopupPanel)
            End If
            Dim PopUpPanel As New Panel With {
                .Size = New Size(
                    Me.Width * 2,
                    (
                        (Globals.Unit(1.5) + Globals.Unit(0.25)) * 6
                    ) + (
                        (Globals.Unit(1) + Globals.Unit(0.25)) * 3
                    )
                ),
                .BackColor = Me.Color,
                .Name = "PopUpPanel",
                .TabIndex = TabIndex + 1
            }
            Dim PopUpX As Integer = Me.Right
            Dim PopUpY As Integer = Me.Top
            If PopUpPanel.Width > Parent.Width - Me.Right Then
                PopUpX = Me.Left - PopUpPanel.Width
            End If
            If PopUpPanel.Height + Me.Top > Parent.Height - Globals.Unit(1) Then
                PopUpY = Parent.Height - PopUpPanel.Height - Globals.Unit(1)
            End If
            PopUpPanel.Location = New Point(PopUpX, PopUpY)
            AddHandler Me.Resize, Sub()
                                      PopUpPanel.Size = New Size(
                                            Me.Width * 2,
                                            (
                                                (Globals.Unit(1.5) + Globals.Unit(0.25)) * 6
                                            ) + (
                                                (Globals.Unit(1) + Globals.Unit(0.25)) * 3
                                            ) + Globals.Unit(0.25)
                                        )

                                      PopUpX = Me.Right
                                      PopUpY = Me.Top
                                      If PopUpPanel.Width > Parent.Width - Me.Right Then
                                          PopUpY = Parent.Height - PopUpPanel.Height
                                      End If
                                      If PopUpPanel.Height + Me.Top > Parent.Height - Globals.Unit(1) Then
                                          PopUpY = Parent.Height - PopUpPanel.Height - Globals.Unit(1)
                                      End If
                                      PopUpPanel.Location = New Point(PopUpX, PopUpY)
                                  End Sub
            Parent.Controls.Add(PopUpPanel)
            Me.Parent.Controls.SetChildIndex(PopUpPanel, 0)

            Dim FacultyDropDown As New BaseDropDown With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5)),
                .Location = New Point(Globals.Unit(0.25), Globals.Unit(0.25)),
                .Name = "Faculty",
                .PlaceHodlder = "Select Faculty"
            }
            AddHandler Me.Resize, Sub()
                                      FacultyDropDown.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5))
                                  End Sub
            Invoke(Sub()
                       FacultyDropDown.Items.Clear()
                       FacultyDropDown.PlaceHodlder = "Select Faculty"
                       Try
                           Dim tempItems = New List(Of String)
                           FacultyDropDown.Items.Clear()
                           Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/faculties", Nothing)
                           Dim data = Globals.JSONToDictionary(response)

                           For i As Integer = 0 To data.Keys.Count - 1
                               tempItems.Add(data.Keys(i) & " - " & data.Values(i))
                           Next

                           FacultyDropDown.Items = tempItems
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
            PopUpPanel.Controls.Add(FacultyDropDown)

            Dim SectionDropDown As New BaseDropDown With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5)),
                .Location = New Point(Globals.Unit(0.25), FacultyDropDown.Bottom + Globals.Unit(0.25)),
                .Name = "Section",
                .PlaceHodlder = "Select Section"
            }
            AddHandler Me.Resize, Sub()
                                      SectionDropDown.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5))
                                  End Sub
            Invoke(Sub()
                       SectionDropDown.Items.Clear()
                       SectionDropDown.PlaceHodlder = "Select Section"
                       Try
                           Dim tempItems = New List(Of String)
                           Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/sections/" & Me.YearLevel, Nothing)
                           Dim data = Globals.JSONToDictionary(response)

                           For Each value In data.Values
                               tempItems.Add(value)
                           Next

                           SectionDropDown.Items = tempItems
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
            PopUpPanel.Controls.Add(SectionDropDown)

            Dim FacilityDropDown As New BaseDropDown With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5)),
                .Location = New Point(Globals.Unit(0.25), SectionDropDown.Bottom + Globals.Unit(0.25)),
                .Name = "Facility",
                .PlaceHodlder = "Select Facility"
            }
            AddHandler Me.Resize, Sub()
                                      FacilityDropDown.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5))
                                  End Sub
            Invoke(Sub()
                       FacilityDropDown.Items.Clear()
                       FacilityDropDown.PlaceHodlder = "Select Facility"
                       Try
                           Dim tempItems = New List(Of String)
                           FacilityDropDown.Items.Clear()
                           Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/facilities", Nothing)
                           Dim data = Globals.JSONToDictionary(response)

                           For i As Integer = 0 To data.Keys.Count - 1
                               tempItems.Add(data.Keys(i) & " - " & data.Values(i))
                           Next

                           FacilityDropDown.Items = tempItems
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
            PopUpPanel.Controls.Add(FacilityDropDown)

            Dim DayDropdown As New BaseDropDown With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5)),
                .Location = New Point(Globals.Unit(0.25), FacilityDropDown.Bottom + Globals.Unit(0.25)),
                .Name = "Day",
                .PlaceHodlder = "Select Day"
            }
            DayDropdown.Items = New List(Of String) From {
                "Sunday",
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday"
            }
            AddHandler Me.Resize, Sub()
                                      DayDropdown.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5))
                                  End Sub
            PopUpPanel.Controls.Add(DayDropdown)

            Dim StartTimeDropDown As New BaseDropDown With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5)),
                .Location = New Point(Globals.Unit(0.25), DayDropdown.Bottom + Globals.Unit(0.25)),
                .Name = "StartTime",
                .PlaceHodlder = "Select Start Time"
            }
            StartTimeDropDown.Items = New List(Of String) From {
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
            }
            AddHandler Me.Resize, Sub()
                                      StartTimeDropDown.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5))
                                  End Sub
            PopUpPanel.Controls.Add(StartTimeDropDown)

            Dim EndTimeDropDown As New BaseDropDown With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5)),
                .Location = New Point(Globals.Unit(0.25), StartTimeDropDown.Bottom + Globals.Unit(0.25)),
                .Name = "EndTime",
                .PlaceHodlder = "Select End Time"
            }
            AddHandler Me.Resize, Sub()
                                      EndTimeDropDown.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1.5))
                                  End Sub
            AddHandler StartTimeDropDown.SelectedIndexChanged, Sub()
                                                                   If StartTimeDropDown.SelectedIndex = -1 Then
                                                                       Exit Sub
                                                                   End If
                                                                   EndTimeDropDown.Items.Clear()
                                                                   EndTimeDropDown.PlaceHodlder = "Select End Time"
                                                                   Dim TimeItems = New List(Of String)
                                                                   Dim StartTimeIndex As Integer = StartTimeDropDown.SelectedIndex
                                                                   For i As Integer = StartTimeIndex + 1 To StartTimeDropDown.Items.Count - 1
                                                                       TimeItems.Add(StartTimeDropDown.Items(i))
                                                                   Next
                                                                   EndTimeDropDown.Items = TimeItems
                                                               End Sub
            PopUpPanel.Controls.Add(EndTimeDropDown)

            Dim SubmitButton As New BaseButton With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1)),
                .Location = New Point(Globals.Unit(0.25), EndTimeDropDown.Bottom + Globals.Unit(0.25)),
                .Text = "Submit"
            }
            AddHandler Me.Resize, Sub()
                                      SubmitButton.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1))
                                  End Sub
            PopUpPanel.Controls.Add(SubmitButton)

            Dim DeleteButton As New BaseButton With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1)),
                .Location = New Point(Globals.Unit(0.25), SubmitButton.Bottom + Globals.Unit(0.25)),
                .Text = "Delete",
                .SubButton = True
            }
            AddHandler Me.Resize, Sub()
                                      DeleteButton.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1))
                                  End Sub
            PopUpPanel.Controls.Add(DeleteButton)

            Dim CancelButton As New BaseButton With {
                .Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1)),
                .Location = New Point(Globals.Unit(0.25), DeleteButton.Bottom + Globals.Unit(0.25)),
                .Text = "Cancel",
                .SubButton = True
            }
            AddHandler Me.Resize, Sub()
                                      CancelButton.Size = New Size((Me.Width * 2) - Globals.Unit(0.5), Globals.Unit(1))
                                      CancelButton.PerformClick()
                                  End Sub
            AddHandler CancelButton.Click, Sub()
                                               Parent.Controls.Remove(PopUpPanel)
                                           End Sub
            PopUpPanel.Controls.Add(CancelButton)

            FacultyDropDown.SelectedItem = FacultyDropDown.Items.Find(Function(x) x.Contains(Me.FacultyID))
            SectionDropDown.SelectedItem = SectionDropDown.Items.Find(Function(x) x.Contains(Me.Section))
            FacilityDropDown.SelectedItem = FacilityDropDown.Items.Find(Function(x) x.Contains(Me.Facility))
            DayDropdown.SelectedItem = DayDropdown.Items.Find(Function(x) x.Contains(Me.Day))
            'Convert Military time to 12 hour time format
            ' 08:00:00 to 08:00 AM
            Dim StartTime As String = Me.StartTime
            Dim StartTimeHour As Integer = CInt(StartTime.Split(":")(0))
            Dim StartTimeMinute As Integer = CInt(StartTime.Split(":")(1))
            Dim StartTimePeriod As String = "AM"
            If StartTimeHour > 12 Then
                StartTimeHour -= 12
                StartTimePeriod = "PM"
            End If
            StartTime = $"{StartTimeHour.ToString("00")}:{StartTimeMinute.ToString("00")} {StartTimePeriod}"
            StartTimeDropDown.SelectedItem = StartTimeDropDown.Items.Find(Function(x) x.Contains(StartTime))
            Dim EndTime As String = Me.EndTime
            Dim EndTimeHour As Integer = CInt(EndTime.Split(":")(0))
            Dim EndTimeMinute As Integer = CInt(EndTime.Split(":")(1))
            Dim EndTimePeriod As String = "AM"
            If EndTimeHour > 12 Then
                EndTimeHour -= 12
                EndTimePeriod = "PM"
            End If
            EndTime = EndTimeHour.ToString("00") & ":" & EndTimeMinute.ToString("00") & " " & EndTimePeriod
            EndTimeDropDown.SelectedItem = EndTimeDropDown.Items.Find(Function(x) x.Contains(EndTime))
            For Each Control As Control In Parent.Controls
                Control.Refresh()
            Next

            AddHandler SubmitButton.Click, Sub()
                                               Try
                                                   Dim FacultyID As String = FacultyDropDown.SelectedItem.Split(" - ")(0)
                                                   Dim Section As String = SectionDropDown.SelectedItem
                                                   Dim Facility As String = FacilityDropDown.SelectedItem.Split(" - ")(0)
                                                   Dim Day As String = DayDropdown.SelectedItem
                                                   Dim StartTimeData As String = StartTimeDropDown.SelectedItem
                                                   Dim EndTimeData As String = EndTimeDropDown.SelectedItem
                                                   If EndTimeDropDown.SelectedIndex = -1 Then
                                                       Dim EndTimeModal As New BaseModal With {
                                                           .Title = "Error",
                                                           .Message = "Please select an end time."
                                                       }
                                                       EndTimeModal.ShowDialog()
                                                       Exit Sub
                                                   End If
                                                   Dim data As New Dictionary(Of String, String) From {
                                                       {"facultyID", FacultyID},
                                                       {"facilityID", Facility},
                                                       {"section", Section},
                                                       {"day", Day},
                                                       {"startTime", StartTimeData},
                                                       {"endTime", EndTimeData}
                                                   }
                                                   Dim response As String = Globals.API("POST", "admin/dashboard/program/" & Globals.PROGRAM & "/update/schedule/" & Me.ID, Globals.DictionaryToJSON(data))
                                                   Dim Modal As New BaseModal With {
                                                         .Title = "Success",
                                                         .Message = response
                                                    }
                                                   Modal.ShowDialog()
                                                   Parent.Controls.Remove(PopUpPanel)
                                                   Parent.Controls.Find("Schedule", True).ToList().ForEach(Sub(x) Parent.Controls.Remove(x))

                                                   Dim ScheduleSelection As BaseComboBox = Parent.Controls.Find("ScheduleSelection", True).FirstOrDefault()
                                                   Dim ScheduleSelectionIndex As Integer = ScheduleSelection.SelectedIndex
                                                   ScheduleSelection.SelectedIndex = ScheduleSelectionIndex - 1
                                                   ScheduleSelection.SelectedIndex = ScheduleSelectionIndex
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

            AddHandler DeleteButton.Click, Sub()
                                               Try
                                                   Dim data As New Dictionary(Of String, String) From {
                                                       {"facultyID", Me.FacultyID},
                                                       {"facilityID", Me.Facility},
                                                       {"section", Me.Section},
                                                       {"day", Me.Day},
                                                       {"startTime", Me.StartTime},
                                                       {"endTime", Me.EndTime}
                                                   }
                                                   Dim response As String = Globals.API("POST", "admin/dashboard/program/" & Globals.PROGRAM & "/delete/schedule/" & Me.ID, Globals.DictionaryToJSON(data))
                                                   Dim Modal As New BaseModal With {
                                                         .Title = "Success",
                                                         .Message = response
                                                    }
                                                   Modal.ShowDialog()
                                                   Parent.Controls.Remove(PopUpPanel)
                                                   Parent.Controls.Find("Schedule", True).ToList().ForEach(Sub(x) Parent.Controls.Remove(x))

                                                   Dim ScheduleSelection As BaseComboBox = Parent.Controls.Find("ScheduleSelection", True).FirstOrDefault()
                                                   Dim ScheduleSelectionIndex As Integer = ScheduleSelection.SelectedIndex
                                                   ScheduleSelection.SelectedIndex = ScheduleSelectionIndex - 1
                                                   ScheduleSelection.SelectedIndex = ScheduleSelectionIndex
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
        End Sub
    End Class

    Private Schedules As New List(Of SchedulePanel)

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
            .Image = Globals.LoadSvgFromResource("Calendar Icon Active", New Size(Globals.Unit(2), Globals.Unit(2))).Draw(),
            .Size = New Size(Globals.Unit(2), Globals.Unit(2)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Cursor = Cursors.Hand,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }
        Me.SidePanel.Controls.Add(CallendarIcon)
        Dim CreateIcon As New PictureBox With {
            .Image = Globals.LoadSvgFromResource("Create Icon", New Size(Globals.Unit(2), Globals.Unit(2))).Draw(),
            .Size = New Size(Globals.Unit(2), Globals.Unit(2)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Cursor = Cursors.Hand,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }
        AddHandler CreateIcon.Click, Sub()
                                         Me.GoToForm(New Dashboard_Create)
                                     End Sub
        Me.SidePanel.Controls.Add(CreateIcon)
        Dim NotificationIcon As New PictureBox With {
            .Image = Globals.LoadSvgFromResource("Notification Icon", New Size(Globals.Unit(2), Globals.Unit(2))).Draw(),
            .Size = New Size(Globals.Unit(2), Globals.Unit(2)),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Cursor = Cursors.Hand,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }
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

        Me.Calendar = New Transparent.Panel With {
            .Size = New Size(
                Me.MainPanel.Width - Globals.Unit(2),
                Me.MainPanel.Height - Globals.Unit(2)
            ),
            .Location = New Point(Globals.Unit(1), Globals.Unit(1))
        }
        Me.MainPanel.Controls.Add(Me.Calendar)

        Dim ScheduleSelection As New BaseComboBox With {
            .Size = New Size(Me.Calendar.Width, Globals.Unit(1)),
            .Location = New Point(0, 0),
            .PlaceHolder = "Select Calendar",
            .Name = "ScheduleSelection"
        }
        AddHandler Me.Calendar.Resize, Sub()
                                           ScheduleSelection.Size = New Size(Me.Calendar.Width, Globals.Unit(1))
                                       End Sub
        Me.Calendar.Controls.Add(ScheduleSelection)
        Dim PopupPanel As Panel = Me.Calendar.Controls.Find("PopUpPanel", True).FirstOrDefault()
        If PopupPanel IsNot Nothing Then
            Me.Calendar.Controls.Remove(PopupPanel)
        End If
        Dim HeaderPanel As New Transparent.FlowLayoutPanel With {
            .Size = New Size(
                    Me.MainPanel.Width - Globals.Unit(2),
                    Me.MainPanel.Height - Globals.Unit(2)
                ),
            .Location = New Point(0, Globals.Unit(1)),
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .BackColor = Globals.Palette("Secondary"),
            .Padding = New Padding(0, 0, 0, 0),
            .Margin = New Padding(0, 0, 0, 0),
            .Name = "HeaderPanel"
        }
        AddHandler Me.Calendar.Resize, Sub()
                                           HeaderPanel.Size = New Size(
                                                Me.Calendar.Width,
                                                Globals.Unit(1)
                                           )
                                       End Sub
        Me.Calendar.Controls.Add(HeaderPanel)

        Dim Days As New List(Of String) From {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"
        }
        For Each Day In Days
            Dim DayLabel As New Transparent.Label With {
                .Text = Day,
                .Name = Day,
                .Size = New Size(
                    (Me.Calendar.Width / Days.Count) - (Globals.Unit(0.5) * (Days.Count - 1)),
                    Globals.Unit(1)
                ),
                .AutoSize = False,
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                .TextAlign = ContentAlignment.MiddleCenter,
                .Margin = New Padding(0),
                .Padding = New Padding(0),
                .BackColor = Globals.Palette("Secondary"),
                .ForeColor = Globals.Palette("Plain Light")
            }
            AddHandler Me.Calendar.Resize, Sub()
                                               HeaderPanel.Padding = New Padding(CInt(Me.Calendar.Width / 6.4), 0, 0, 0)
                                               DayLabel.Size = New Size(
                                                   (Me.Calendar.Width - (Me.Calendar.Width / 6.4)) / Days.Count,
                                                   Globals.Unit(1)
                                               )
                                           End Sub
            HeaderPanel.Controls.Add(DayLabel)
        Next

        Dim TimestampPanel As New Transparent.FlowLayoutPanel With {
            .Size = New Size(
                Me.Calendar.Width / 6.4,
                Me.Calendar.Height - Globals.Unit(3)
            ),
            .Location = New Point(0, Globals.Unit(2)),
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, Globals.Unit(0.5), 0, Globals.Unit(0.5)),
            .Margin = New Padding(0, 0, 0, 0),
            .Name = "TimestampPanel"
        }
        Me.Calendar.Controls.Add(TimestampPanel)
        Dim Times As New List(Of String) From {
            "07:00 - 07:30 AM",
            "07:30 - 08:00 AM",
            "08:00 - 08:30 AM",
            "08:30 - 09:00 AM",
            "09:00 - 09:30 AM",
            "09:30 - 10:00 AM",
            "10:00 - 10:30 AM",
            "10:30 - 11:00 AM",
            "11:00 - 11:30 AM",
            "11:30 - 12:00 NN",
            "12:00 - 12:30 PM",
            "12:30 - 01:00 PM",
            "01:00 - 01:30 PM",
            "01:30 - 02:00 PM",
            "02:00 - 02:30 PM",
            "02:30 - 03:00 PM",
            "03:00 - 03:30 PM",
            "03:30 - 04:00 PM",
            "04:00 - 04:30 PM",
            "04:30 - 05:00 PM",
            "05:00 - 05:30 PM",
            "05:30 - 06:00 PM",
            "06:00 - 06:30 PM",
            "06:30 - 07:00 PM"
        }
        For Each Time In Times
            Dim TimeLabel As New Transparent.Label With {
                .Text = Time,
                .Name = Time,
                .Size = New Size(
                    TimestampPanel.Width,
                    ((TimestampPanel.Height - Globals.Unit(1)) / Times.Count) - Globals.Unit(0.25) / 2
                ),
                .AutoSize = False,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5) - (Globals.Unit(0.25) / 2), FontStyle.Regular),
                .TextAlign = ContentAlignment.MiddleCenter,
                .Margin = New Padding(0, 0, 0, Globals.Unit(0.25) / 2),
                .Padding = New Padding(0),
                .ForeColor = Globals.Palette("Plain Dark")
            }
            AddHandler Me.Calendar.Resize, Sub()
                                               TimestampPanel.Size = New Size(
                                                   Me.Calendar.Width / 6.4,
                                                   Me.Calendar.Height - Globals.Unit(3)
                                               )
                                               TimeLabel.Size = New Size(
                                                   TimestampPanel.Width,
                                                   ((TimestampPanel.Height - Globals.Unit(1)) / Times.Count) - Globals.Unit(0.25) / 2
                                               )
                                               TimestampLabelHeight = TimeLabel.Height
                                           End Sub
            TimestampPanel.Controls.Add(TimeLabel)
        Next
        Dim FooterPanel As New Panel With {
            .Size = New Size(Me.Calendar.Width, Globals.Unit(1)),
            .Location = New Point(0, Me.Calendar.Height - Globals.Unit(1)),
            .BackColor = Globals.Palette("Primary")
        }
        AddHandler Me.Calendar.Resize, Sub()
                                           FooterPanel.Size = New Size(Me.Calendar.Width, Globals.Unit(1))
                                           FooterPanel.Location = New Point(0, Me.Calendar.Height - Globals.Unit(1))
                                       End Sub
        Me.Calendar.Controls.Add(FooterPanel)

        Dim Identifiers = New Dictionary(Of String, Dictionary(Of String, String))

        Invoke(Sub()
                   Try
                       Dim response As String = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/calendars/", Nothing)
                       Dim data = Globals.JSONToDictionary(response, True)
                       Dim Selections As New List(Of String)
                       For Each item In data.Values
                           Selections.Add(item("name"))
                           Identifiers.Add(item("name"), New Dictionary(Of String, String) From {
                                           {"key", item("key")},
                                           {"identifier", item("identifier")},
                                           {"name", item("name")}
                           })

                       Next
                       ScheduleSelection.Items = Selections
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

        Dim DisplaySchedules = Sub()
                                   'Check if ScheduleSelection is empty
                                   If ScheduleSelection.Items.Count = 0 Then
                                       ScheduleSelection.PlaceHolder = "No Calendars Available"
                                       Exit Sub
                                   End If
                                   If ScheduleSelection.SelectedIndex = -1 Then
                                       ScheduleSelection.PlaceHolder = "No Calendars Selected"
                                       Exit Sub
                                   End If
                                   Me.Calendar.Controls.Find("Schedule", True).ToList().ForEach(Sub(x) Me.Calendar.Controls.Remove(x))
                                   Me.Calendar.Controls.Find("PopUpPanel", True).ToList().ForEach(Sub(x) Me.Calendar.Controls.Remove(x))

                                   Dim Key As String = Identifiers(ScheduleSelection.SelectedItem)("key")
                                   Dim Identifier As String = Identifiers(ScheduleSelection.SelectedItem)("identifier")
                                   Dim PossibleTimes As New List(Of String) From {
                                                                    "07:00:00",
                                                                    "07:30:00",
                                                                    "08:00:00",
                                                                    "08:30:00",
                                                                    "09:00:00",
                                                                    "09:30:00",
                                                                    "10:00:00",
                                                                    "10:30:00",
                                                                    "11:00:00",
                                                                    "11:30:00",
                                                                    "12:00:00",
                                                                    "12:30:00",
                                                                    "13:00:00",
                                                                    "13:30:00",
                                                                    "14:00:00",
                                                                    "14:30:00",
                                                                    "15:00:00",
                                                                    "15:30:00",
                                                                    "16:00:00",
                                                                    "16:30:00",
                                                                    "17:00:00",
                                                                    "17:30:00",
                                                                    "18:00:00",
                                                                    "18:30:00",
                                                                    "19:00:00"
                                                               }
                                   Dim response As String
                                   Try
                                       response = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/schedule/" & Identifier & "/" & Key)
                                       Dim data = Globals.JSONToDictionary(response, True)
                                       Dim SchedulesPerDay As New Dictionary(Of String, List(Of SchedulePanel))

                                       Dim itemIndex = 0
                                       For Each item In data.Values
                                           Dim ScheduleDay As String = item("day")
                                           Dim ScheduleStartTime As String = item("startTime")
                                           Dim ScheduleEndTime As String = item("endTime")
                                           Dim ScheduleCourse As String = item("course")
                                           Dim ScheduleDescription As String = item("description")
                                           Dim ScheduleIdentifier As String = item("identifier")
                                           Dim ScheduleID As String = data.Keys(itemIndex)
                                           Dim YearLevel As String = item("section").ToString().ToCharArray()(0)
                                           Dim Section As String = item("section")
                                           Dim FacultyID As String = item("faculty")
                                           Dim Facility As String = item("facility")

                                           Dim SchedulePanel As New SchedulePanel(
                                                    ScheduleCourse,
                                                    ScheduleDescription,
                                                    ScheduleIdentifier,
                                                    ScheduleID,
                                                    YearLevel,
                                                    Section,
                                                    FacultyID,
                                                    Facility,
                                                    ScheduleDay,
                                                    ScheduleStartTime,
                                                    ScheduleEndTime
                                                )
                                           SchedulePanel.Name = "Schedule"
                                           Dim Left As Integer = HeaderPanel.Controls(ScheduleDay).Left
                                           Dim Right As Integer = HeaderPanel.Controls(ScheduleDay).Right
                                           Dim Width As Integer = Right - Left

                                           Dim Top As Integer = TimestampPanel.Controls(PossibleTimes.IndexOf(ScheduleStartTime)).Top + TimestampPanel.Top
                                           Dim Bottom As Integer = TimestampPanel.Controls(PossibleTimes.IndexOf(ScheduleEndTime) - 1).Bottom + TimestampPanel.Top
                                           Dim Height As Integer = Bottom - Top

                                           SchedulePanel.Location = New Point(Left, Top)
                                           SchedulePanel.Size = New Size(Width, Height)
                                           AddHandler Me.Resize, Sub()
                                                                     Left = HeaderPanel.Controls(ScheduleDay).Left
                                                                     Right = HeaderPanel.Controls(ScheduleDay).Right
                                                                     Width = Right - Left

                                                                     Top = TimestampPanel.Controls(PossibleTimes.IndexOf(ScheduleStartTime)).Top + TimestampPanel.Top
                                                                     Bottom = TimestampPanel.Controls(PossibleTimes.IndexOf(ScheduleEndTime) - 1).Bottom + TimestampPanel.Top
                                                                     Height = Bottom - Top

                                                                     SchedulePanel.Location = New Point(Left, Top)
                                                                     SchedulePanel.Size = New Size(Width, Height)
                                                                 End Sub
                                           Me.Calendar.Controls.Add(SchedulePanel)
                                           Me.Schedules.Add(SchedulePanel)

                                           If Not SchedulesPerDay.ContainsKey(ScheduleDay) Then
                                               SchedulesPerDay.Add(ScheduleDay, New List(Of SchedulePanel))
                                           End If
                                           SchedulesPerDay(ScheduleDay).Add(SchedulePanel)
                                           itemIndex = itemIndex + 1
                                       Next

                                       'Loop through each day
                                       For Each Day In SchedulesPerDay.Values
                                           'Sort
                                           Day.Sort(Function(x, y)
                                                        Return x.Location.Y.CompareTo(y.Location.Y)
                                                    End Function)

                                           Dim ScheduleIndex As Integer = 2
                                           For Each Schedule In Day
                                               If ScheduleIndex Mod 2 = 0 Then
                                                   Schedule.Color = Globals.Palette("Secondary")
                                               Else
                                                   Schedule.Color = Globals.Palette("Plain Dark")
                                               End If
                                               ScheduleIndex = ScheduleIndex + 1
                                           Next
                                       Next
                                   Catch ex As Exception
                                   End Try
                               End Sub

        AddHandler ScheduleSelection.SelectedIndexChanged, Sub()
                                                               DisplaySchedules()
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
            Me.SidePanel.Size = New Size(Globals.Unit(2), Me.Contents.Height - (Me.HeaderBar.Height + Me.resizerBarTop.Height))
            Me.MainPanel.Size = New Size(
                Me.Contents.Width - (Me.SidePanel.Width + Me.resizerBarLeft.Width + Me.resizerBarRight.Width),
                Me.Contents.Height - (Me.HeaderBar.Height + Me.resizerBarTop.Height)
            )
            Me.Calendar.Size = New Size(
                Me.MainPanel.Width - Globals.Unit(2),
                Me.MainPanel.Height - Globals.Unit(2)
            )
        End If
    End Sub
End Class