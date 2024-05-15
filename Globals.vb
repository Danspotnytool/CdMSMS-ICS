Imports Svg
Imports System.Drawing.Text
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Net
Imports System.IO
Imports System.Text

Module Globals
    Public Function HexToColor(hex As String) As Color
        hex = hex.Replace("#", "")
        Dim withAlpha As Boolean = hex.Length = 8
        If withAlpha Then
            Return Color.FromArgb(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                Convert.ToByte(hex.Substring(6, 2), 16)
            )
        End If
        Return Color.FromArgb(
            Convert.ToByte(hex.Substring(0, 2), 16),
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16)
        )
    End Function

    Public ReadOnly Palette As New Dictionary(Of String, Color) From {
        {"Primary", HexToColor("106A2E")},
        {"Secondary", HexToColor("0D7856")},
        {"Primary Compliment", HexToColor("F4D35E")},
        {"Plain Dark", HexToColor("1F1F1F")},
        {"Plain Light", HexToColor("F1F1F1")},
        {"White", HexToColor("FFFFFF")},
        {"Black", HexToColor("000000")}
    }
    Friend ReadOnly Primary As Color = Palette("Primary")
    Friend ReadOnly Secondary As Color = Palette("Secondary")
    Friend ReadOnly PrimaryCompliment As Color = Palette("Primary Compliment")
    Friend ReadOnly PlainDark As Color = Palette("Plain Dark")
    Friend ReadOnly PlainLight As Color = Palette("Plain Light")
    Friend ReadOnly White As Color = Palette("White")
    Friend ReadOnly Black As Color = Palette("Black")

    Public Function Unit(Optional units As Double = 1) As Double
        Return 30 * units
    End Function

    Public MinimumFormSize As New Size(Unit(20), Unit(20))
    Public FormSize As New Size(Unit(30), Unit(20))
    Public FormLocation As New Point(
        (Screen.PrimaryScreen.WorkingArea.Width - FormSize.Width) / 2,
        (Screen.PrimaryScreen.WorkingArea.Height - FormSize.Height) / 2
    )

    Public Function LoadSvgFromResource(resourceName As String, size As Size) As SvgDocument
        Dim resourceByte = My.Resources.ResourceManager.GetObject(resourceName)
        If resourceByte Is Nothing Then
            Throw New Exception("Resource not found: " & resourceName)
        End If
        Dim resourceString As String = System.Text.Encoding.UTF8.GetString(resourceByte)

        Dim svg = SvgDocument.FromSvg(Of SvgDocument)(resourceString)
        svg.Width = size.Width
        svg.Height = size.Height
        Return svg
    End Function
    Friend Function LoadSvgFromResource(resourceName As String) As SvgDocument
        Dim resourceByte = My.Resources.ResourceManager.GetObject(resourceName)
        If resourceByte Is Nothing Then
            Throw New Exception("Resource not found: " & resourceName)
        End If
        Dim resourceString As String = System.Text.Encoding.UTF8.GetString(resourceByte)

        Dim svg = SvgDocument.FromSvg(Of SvgDocument)(resourceString)
        Return svg
    End Function

    Function GetFontFromResource(resourceName As String) As FontFamily
        Dim fontBytes As Byte() = My.Resources.ResourceManager.GetObject(resourceName)
        Dim pinnedArray As GCHandle = GCHandle.Alloc(fontBytes, GCHandleType.Pinned)
        Dim fontData As IntPtr = pinnedArray.AddrOfPinnedObject()
        Dim fontLength As Integer = My.Resources.ResourceManager.GetObject(resourceName).Length
        Dim privateFontCollection As PrivateFontCollection = New PrivateFontCollection()
        privateFontCollection.AddMemoryFont(fontData, fontLength)
        pinnedArray.Free()
        Return privateFontCollection.Families(0)
    End Function

    Private ReadOnly OpenSans_Regular As FontFamily = GetFontFromResource("OpenSans_Regular")
    Private ReadOnly OpenSans_Italic As FontFamily = GetFontFromResource("OpenSans_Italic")
    Private ReadOnly OpenSans_Bold As FontFamily = GetFontFromResource("OpenSans_Bold")
    Private ReadOnly OpenSans_BoldItalic As FontFamily = GetFontFromResource("OpenSans_BoldItalic")
    Private ReadOnly OpenSans_SeimiBold As FontFamily = GetFontFromResource("OpenSans_SemiBold")
    Private ReadOnly OpenSans_SemiBoldItalic As FontFamily = GetFontFromResource("OpenSans_SemiBoldItalic")

    Private ReadOnly Raleway_Regular As FontFamily = GetFontFromResource("Raleway_Regular")
    Private ReadOnly Raleway_Bold As FontFamily = GetFontFromResource("Raleway_Bold")

    Public Function GetFont(Optional FontName As String = "Open Sans", Optional size As Single = Nothing, Optional style As FontStyle = FontStyle.Regular) As Font
        Dim BaseFont As Font
        If size = Nothing Then
            size = Unit(0.5)
        End If
        Select Case FontName.ToLower()
            Case "open sans"
                Select Case style
                    Case FontStyle.Bold
                        BaseFont = New Font(OpenSans_Bold, size, FontStyle.Bold, GraphicsUnit.Pixel)
                    Case FontStyle.Italic
                        BaseFont = New Font(OpenSans_Italic, size, FontStyle.Italic, GraphicsUnit.Pixel)
                    Case FontStyle.Bold Or FontStyle.Italic
                        BaseFont = New Font(OpenSans_BoldItalic, size, FontStyle.Bold Or FontStyle.Italic, GraphicsUnit.Pixel)
                    Case FontStyle.Regular
                        BaseFont = New Font(OpenSans_Regular, size, FontStyle.Regular, GraphicsUnit.Pixel)
                    Case Else
                        BaseFont = New Font(OpenSans_Regular, size, FontStyle.Regular, GraphicsUnit.Pixel)
                End Select
                Return BaseFont
            Case "raleway"
                Select Case style
                    Case FontStyle.Bold
                        BaseFont = New Font(Raleway_Bold, size, FontStyle.Bold, GraphicsUnit.Pixel)
                    Case FontStyle.Regular
                        BaseFont = New Font(Raleway_Regular, size, FontStyle.Regular, GraphicsUnit.Pixel)
                    Case Else
                        BaseFont = New Font(Raleway_Regular, size, FontStyle.Regular, GraphicsUnit.Pixel)
                End Select
                Return BaseFont
            Case Else
                Return GetFont("Open Sans", size, style)
        End Select
    End Function

    Public Function Fetch(uri As String, method As String, Optional data As String = "") As String
        Dim request As HttpWebRequest = WebRequest.Create(uri)
        request.Method = method
        Select Case method
            Case "GET"
                Dim response As HttpWebResponse = request.GetResponse()
                Dim reader As New StreamReader(response.GetResponseStream())
                Return reader.ReadToEnd()
            Case "POST"
                request.ContentType = "application/json"
                Dim dataBytes As Byte() = Encoding.UTF8.GetBytes(data)
                request.ContentLength = dataBytes.Length
                Dim requestStream As Stream = request.GetRequestStream()
                requestStream.Write(dataBytes, 0, dataBytes.Length)
                requestStream.Close()
                Dim response As HttpWebResponse = request.GetResponse()
                Dim reader As New StreamReader(response.GetResponseStream())
                Return reader.ReadToEnd()
            Case "UPDATE"
                request.Method = "PUT"
                request.ContentType = "application/json"
                Dim dataBytes As Byte() = Encoding.UTF8.GetBytes(data)
                request.ContentLength = dataBytes.Length
                Dim requestStream As Stream = request.GetRequestStream()
                requestStream.Write(dataBytes, 0, dataBytes.Length)
                requestStream.Close()
                Dim response As HttpWebResponse = request.GetResponse()
                Dim reader As New StreamReader(response.GetResponseStream())
                Return reader.ReadToEnd()
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function API(method As String, path As String, Optional data As String = "") As String
        Return Fetch("http://localhost:3090/api/" & path, method, data)
    End Function

    Public Function DictionaryToJSON(dictionary As Dictionary(Of String, String)) As String
        Dim json As String = "{"
        For Each item In dictionary
            json &= $"{Chr(34)}{item.Key}{Chr(34)}: " & $"{Chr(34)}{ item.Value.Replace(Chr(34), "\" & Chr(34)).Replace(Environment.NewLine, "\n") }{Chr(34)},"
        Next
        json = json.TrimEnd(",")
        json &= "}"
        Return json
    End Function
End Module
