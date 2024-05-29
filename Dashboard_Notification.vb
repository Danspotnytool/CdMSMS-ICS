Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Net
Imports System.Resources
Imports System.Text.RegularExpressions
Imports Svg

Public Class Dashboard_Notification
    Inherits BaseForm

    Private SidePanel As FlowLayoutPanel
    Private MainPanel As Transparent.Panel

    Private Class RequestTable
        Inherits Transparent.Panel

        Sub New(
                    courseCode As Object,
                    facultyName As Object,
                    originalFacilityName As Object,
                    originalDay As Object,
                    originalStartTime As Object,
                    originalEndTime As Object,
                    requestFacilityName As Object,
                    requestDay As Object,
                    requestStartTime As Object,
                    requestEndTime As Object,
                    status As Object,
                    requestReason As Object,
                    rejectReason As Object,
                    requestDate As Object,
                    scheduleID As Object,
                    requestID As Object
               )
            Me.BorderStyle = BorderStyle.None
            Me.Margin = New Padding(0, 0, 0, Globals.Unit(2))

            Dim Header As New Label With {
                .Text = courseCode & " - " & facultyName,
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.75), FontStyle.Bold),
                .ForeColor = Globals.Palette("White"),
                .BackColor = Globals.Palette("Primary"),
                .AutoSize = False,
                .Size = New Size(Me.Width, Globals.Unit(1)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(Header)

            Dim OriginalLabel As New Label With {
                .Text = "Original",
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("White"),
                .BackColor = Globals.Palette("Secondary"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(0, Header.Bottom),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(OriginalLabel)

            Dim OriginalFacilityLabel As New Label With {
                .Text = "Facility: " & originalFacilityName,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("Plain Light"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(0, Globals.Unit(2)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(OriginalFacilityLabel)

            Dim OriginalDayLabel As New Label With {
                .Text = "Day: " & originalDay,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("White"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(0, Globals.Unit(3)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(OriginalDayLabel)

            Dim OriginalTimeLabel As New Label With {
                .Text = "Time: " & originalStartTime & " - " & originalEndTime,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("Plain Light"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(0, Globals.Unit(4)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(OriginalTimeLabel)

            Dim RequestLabel As New Label With {
                .Text = "Request",
                .Font = Globals.GetFont("Raleway", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("White"),
                .BackColor = Globals.Palette("Secondary"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(Me.Width / 2, Header.Bottom),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(RequestLabel)

            Dim RequestFacilityLabel As New Label With {
                .Text = "Facility: " & requestFacilityName,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("White"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(Me.Width / 2, Globals.Unit(2)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(RequestFacilityLabel)

            Dim RequestDayLabel As New Label With {
                .Text = "Day: " & requestDay,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("Plain Light"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(Me.Width / 2, Globals.Unit(3)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(RequestDayLabel)

            Dim RequestTimeLabel As New Label With {
                .Text = "Time: " & requestStartTime & " - " & requestEndTime,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("White"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(Me.Width / 2, Globals.Unit(4)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(RequestTimeLabel)

            Dim ReasonLabel As New Label With {
                .Text = "Request Reason: " & requestReason,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("White"),
                .AutoSize = False,
                .Size = New Size(Me.Width, Globals.Unit(1)),
                .Location = New Point(0, Globals.Unit(5)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(ReasonLabel)

            Dim StatusLabel As New Label With {
                .Text = "Status: " & status,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("Plain Light"),
                .AutoSize = False,
                .Size = New Size(Me.Width, Globals.Unit(1)),
                .Location = New Point(0, Globals.Unit(6)),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            Me.Controls.Add(StatusLabel)

            If rejectReason = Nothing Then
                rejectReason = ""
            End If

            Dim RejectReasonLabel As New Label With {
                .Text = "Rejection Rason: " & rejectReason,
                .Font = Globals.GetFont("Open Sans", Globals.Unit(0.5), FontStyle.Bold),
                .ForeColor = Globals.Palette("Plain Dark"),
                .BackColor = Globals.Palette("White"),
                .AutoSize = False,
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(Me.Width / 2, Globals.Unit(6)),
                .TextAlign = ContentAlignment.MiddleCenter
            }

            If status = "rejected" Then
                StatusLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                Me.Controls.Add(RejectReasonLabel)
            End If

            AddHandler Me.Resize, Sub()
                                      Header.Size = New Size(Me.Width, Globals.Unit(1))

                                      OriginalLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      OriginalLabel.Location = New Point(0, Header.Bottom)

                                      OriginalFacilityLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      OriginalFacilityLabel.Location = New Point(0, Globals.Unit(2))

                                      OriginalDayLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      OriginalDayLabel.Location = New Point(0, Globals.Unit(3))

                                      OriginalTimeLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      OriginalTimeLabel.Location = New Point(0, Globals.Unit(4))

                                      RequestLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      RequestLabel.Location = New Point(Me.Width / 2, Header.Bottom)

                                      RequestFacilityLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      RequestFacilityLabel.Location = New Point(Me.Width / 2, Globals.Unit(2))

                                      RequestDayLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      RequestDayLabel.Location = New Point(Me.Width / 2, Globals.Unit(3))

                                      RequestTimeLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      RequestTimeLabel.Location = New Point(Me.Width / 2, Globals.Unit(4))

                                      ReasonLabel.Size = New Size(Me.Width, Globals.Unit(1))

                                      StatusLabel.Size = New Size(Me.Width, Globals.Unit(1))

                                      If status = "rejected" Then
                                          StatusLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                          RejectReasonLabel.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                          RejectReasonLabel.Location = New Point(Me.Width / 2, Globals.Unit(6))
                                      End If
                                  End Sub

            If status = "rejected" Or status = "approved" Then
                Dim Footer As New Transparent.Panel With {
                    .Size = New Size(Me.Width, Globals.Unit(1)),
                    .Location = New Point(0, Globals.Unit(7)),
                    .BackColor = Globals.Palette("Primary")
                }
                Me.Controls.Add(Footer)

                AddHandler Me.Resize, Sub()
                                          Footer.Size = New Size(Me.Width, Globals.Unit(1))
                                          Footer.Location = New Point(0, Globals.Unit(7))
                                      End Sub
                Return
            End If

            Dim ApproveButton As New BaseButton With {
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(0, Globals.Unit(7)),
                .Text = "Approve"
            }
            AddHandler ApproveButton.Click, Sub()
                                                Try
                                                    Dim data As New Dictionary(Of String, String) From {
                                                               {"requestID", requestID}
                                                           }
                                                    Dim response = Globals.API("POST", "admin/dashboard/program/" & Globals.PROGRAM & "/requests/" & requestID & "/approve/", Globals.DictionaryToJSON(data))

                                                    Dim Modal As New BaseModal With {
                                                        .Title = "Success",
                                                        .Message = "Request approved successfully."
                                                    }
                                                    Modal.ShowDialog()

                                                    RaiseEvent Updated(Me, New EventArgs())
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
            Me.Controls.Add(ApproveButton)

            Dim RejectTextInput As New BaseTextInput With {
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(0, Globals.Unit(7)),
                .Name = "Reason for rejection",
                .Visible = False
            }
            Me.Controls.Add(RejectTextInput)

            Dim RejectButton As New BaseButton With {
                .Size = New Size(Me.Width / 2, Globals.Unit(1)),
                .Location = New Point(Me.Width / 2, Globals.Unit(7)),
                .Text = "Reject",
                .SubButton = True
            }
            AddHandler RejectButton.Click, Sub()
                                               If RejectTextInput.Visible Then
                                                   If RejectTextInput.Text = "" Then
                                                       RejectTextInput.Alert()
                                                   Else
                                                       Try
                                                           Dim data As New Dictionary(Of String, String) From {
                                                               {"rejectReason", RejectTextInput.Text}
                                                           }
                                                           Dim response = Globals.API("POST", "admin/dashboard/program/" & Globals.PROGRAM & "/requests/" & requestID & "/reject/", Globals.DictionaryToJSON(data))

                                                           Dim Modal As New BaseModal With {
                                                               .Title = "Success",
                                                               .Message = "Request rejected successfully."
                                                           }
                                                           Modal.ShowDialog()

                                                           RaiseEvent Updated(Me, New EventArgs())
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
                                                   End If
                                               Else
                                                   RejectTextInput.Visible = True
                                                   RejectButton.Text = "Confirm"

                                                   ApproveButton.Visible = False
                                               End If
                                           End Sub
            Me.Controls.Add(RejectButton)

            AddHandler Me.Resize, Sub()
                                      ApproveButton.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      ApproveButton.Location = New Point(0, Globals.Unit(7))

                                      RejectButton.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                      RejectButton.Location = New Point(Me.Width / 2, Globals.Unit(7))

                                      RejectTextInput.Size = New Size(Me.Width / 2, Globals.Unit(1))
                                  End Sub
        End Sub

        Public Event Updated(sender As Object, e As EventArgs)
    End Class

    Protected Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BackendSocket.Connect("notifications_admin")
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
                                            BackendSocket.Close()
                                            Me.GoToForm(New Dashboard_Calendar)
                                        End Sub
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
                                         BackendSocket.Close()
                                         Me.GoToForm(New Dashboard_Create)
                                     End Sub
        Me.SidePanel.Controls.Add(CreateIcon)
        Dim NotificationIcon As New PictureBox With {
            .Image = Globals.LoadSvgFromResource("Notification Icon Active", New Size(Globals.Unit(2), Globals.Unit(2))).Draw(),
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

        Dim RequestsPanel As New FlowLayoutPanel With {
                                             .MinimumSize = New Size(Me.MainPanel.Width - Globals.Unit(2), 0),
                                             .MaximumSize = New Size(Me.MainPanel.Width - Globals.Unit(2), 0),
                                             .Location = New Point(Globals.Unit(1), Globals.Unit(1)),
                                             .FlowDirection = FlowDirection.TopDown,
                                             .WrapContents = False,
                                             .AutoSize = True,
                                             .Padding = New Padding(0, 0, 0, 0)
                                        }
        AddHandler Me.Resize, Sub()
                                  RequestsPanel.MinimumSize = New Size(Me.MainPanel.Width - Globals.Unit(2), 0)
                                  RequestsPanel.MaximumSize = New Size(Me.MainPanel.Width - Globals.Unit(2), 0)
                              End Sub
        Me.MainPanel.Controls.Add(RequestsPanel)

        Me.DisplayRequests(RequestsPanel)

        AddHandler BackendSocket.OnMessage, Sub(data As String)
                                                Me.DisplayRequests(RequestsPanel)
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
                BackendSocket.Close()
                Me.GoToForm(New DeanSetup)
            Else
                Me.Close()
            End If
        End Try
        Loaded = True
        Me.Size = Globals.FormSize
    End Sub

    Protected Sub DisplayRequests(RequestsPanel As FlowLayoutPanel)
        RequestsPanel.Controls.Clear()
        Dim response = Globals.API("GET", "admin/dashboard/program/" & Globals.PROGRAM & "/requests/", Nothing)
        Dim data = Globals.JSONToDictionary(response, True)

        For Each request In data.Values
            Dim RequestTable As New RequestTable(
                request("courseCode"),
                request("facultyName"),
                request("original")("facilityName"),
                request("original")("day"),
                request("original")("startTime"),
                request("original")("endTime"),
                request("request")("facilityName"),
                request("request")("day"),
                request("request")("startTime"),
                request("request")("endTime"),
                request("status"),
                request("requestReason"),
                request("rejectReason"),
                request("requestDate"),
                request("scheduleID"),
                request("requestID")
            )
            AddHandler RequestsPanel.Resize, Sub()
                                                 RequestTable.Size = New Size(RequestsPanel.Width, Globals.Unit(8))
                                             End Sub
            RequestsPanel.Controls.Add(RequestTable)

            AddHandler RequestTable.Updated, Sub()
                                                 RequestsPanel.Size = New Size(RequestsPanel.Width + 1, RequestsPanel.Height + 1)
                                                 Me.DisplayRequests(RequestsPanel)
                                                 RequestsPanel.Size = New Size(RequestsPanel.Width - 1, RequestsPanel.Height - 1)
                                             End Sub
        Next
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