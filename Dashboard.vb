Imports System.Resources
Imports System.Text.RegularExpressions
Imports Svg

Public Class Dashboard
    Inherits BaseForm

    Protected Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Name = "Dashboard"

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
    End Sub

    Protected Sub Dashboard_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Loaded Then
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