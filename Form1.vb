Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports IQOPENLib

Public Class Form1

    Dim columns As Integer
    Dim rows As Integer
    Dim Points As Integer

    Dim p As New Parameters

    Dim a As Integer

    Dim Pfade As String()
    Dim FileNumber As Integer

    Dim fertig As Boolean = False

    Private Sub BT_AddPTX_Click(sender As Object, e As EventArgs) Handles BT_AddPTX.Click
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "ptx files (*.ptx)|*.ptx|fls files (*.fls)|*.fls|All files (*.*)|*.*"
        ofd.Multiselect = True
        If ofd.ShowDialog = DialogResult.OK Then
            For Each f As String In ofd.FileNames()
                If IO.File.Exists(f) Then
                    ListBox1.Items.Add(f)
                Else
                    MsgBox("File does not exist")
                End If
            Next
        End If
    End Sub

    Private Sub BT_Read_Click(sender As Object, e As EventArgs) Handles BT_Read.Click
        fertig = False
        BT_Read.Enabled = False
        BT_AddPTX.Enabled = False
        FileNumber = 0

        ReDim Pfade(ListBox1.Items.Count - 1)
        For File = 0 To ListBox1.Items.Count - 1
            Pfade(File) = ListBox1.Items(File)
        Next

        PB_Files.Maximum = ListBox1.Items.Count

        Dim t As New System.Threading.Thread(AddressOf Run)
        t.Start()

    End Sub

    Private Sub Run()
        For Each f In Pfade
            FileNumber += 1
            If f.EndsWith("fls") Then
                CalculateAndApllyFaro(f, "-" & Now.ToString("dd.MM.yyyy"))
            Else
                CalculateAndAplly(f, "-" & Now.ToString("dd.MM.yyyy"))
            End If

        Next
        fertig = True
    End Sub

    Private Sub CalculateAndAplly(ByVal Pfad As String, ByVal Prosfix As String)
        Dim TransMatrix(7) As String

        'Read File
        Dim fs As New IO.FileStream(Pfad, IO.FileMode.Open)
        Dim sr As New IO.StreamReader(fs)

        columns = sr.ReadLine()
        rows = sr.ReadLine()

        Points = columns * rows

        Dim x(Points - 1) As Double
        Dim y(Points - 1) As Double
        Dim z(Points - 1) As Double
        Dim i(Points - 1) As String


        For a = 0 To 7
            TransMatrix(a) = sr.ReadLine
        Next

        Dim c As Integer = 0
        Dim ar() As String
        a = 0
        Do Until sr.EndOfStream
            a += 1
            ar = sr.ReadLine.Split(" ")
            x(c) = Double.Parse(ar(0), CultureInfo.InvariantCulture)
            y(c) = Double.Parse(ar(1), CultureInfo.InvariantCulture)
            z(c) = Double.Parse(ar(2), CultureInfo.InvariantCulture)

            i(c) = ar(3) & " " & ar(4) & " " & ar(5) & " " & ar(6)

            c += 1
        Loop
        sr.Close()
        fs.Close()


        Dim Roh As Double
        Dim Alpha As Double
        Dim Theta As Double

        For a = 0 To Points - 1

            Roh = Math.Sqrt(x(a) ^ 2 + y(a) ^ 2 + z(a) ^ 2)
            Alpha = Math.Atan(z(a) / Math.Sqrt(x(a) ^ 2 + y(a) ^ 2))
            Theta = Math.Atan2(y(a) * -1, x(a))

            If a > Points / 2 Then 'Secound scanner half
                Alpha = Math.PI - Alpha
                Theta -= Math.PI
            End If


            Theta -= p.B1 * Theta + p.B2 * Math.Sin(Theta) + p.B3 * Math.Cos(Theta) + p.B4 * Math.Sin(2 * Theta) + p.B5 * Math.Cos(2 * Theta) _
                + p.B6 * Sec(Alpha) + p.B7 * Math.Tan(Alpha) + p.B8 * rows ^ -1 + p.B9 * Math.Sin(Alpha) + p.B10 * Math.Cos(Alpha) 'Delta Theta


            Alpha -= p.C0 + p.C1 * Alpha + p.C2 * Math.Sin(Alpha) + p.C3 * Math.Sin(2 * Alpha) + p.C4 * Math.Cos(2 * Alpha) + p.C5 * Roh ^ -1 _
                + p.C6 * Math.Sin(2 * Theta) + p.C7 * Math.Cos(2 * Theta) + p.C8 * Math.Sin(4 * Theta) 'Delta alpha

            Roh -= p.A0 + p.A1 * Roh + p.A2 * Math.Sin(Alpha) 'Delta roh
            If p.U1 > 0 Then
                Roh -= p.A3 * Math.Sin(((4 / Math.PI) / p.U1) * Roh) + p.A4 * Math.Cos(((4 / Math.PI) / p.U1) * Roh) 'Delta roh, only when u1 is not 0
            End If


            Alpha -= 1.570796326795

            x(a) = (Roh) * Math.Sin(Alpha) * Math.Cos(Theta) * -1
            y(a) = (Roh) * Math.Sin(Alpha) * Math.Sin(Theta)
            z(a) = (Roh) * Math.Cos(Alpha)

            If Double.IsNaN(x(a)) Then x(a) = 0
            If Double.IsNaN(y(a)) Then y(a) = 0
            If Double.IsNaN(z(a)) Then z(a) = 0


        Next



        'Write new File
        fs = New IO.FileStream(Pfad.Substring(0, Pfad.Length - 4) & Prosfix & ".ptx", IO.FileMode.CreateNew)
        Dim sw As New IO.StreamWriter(fs)

        sw.Write((columns & vbLf))
        sw.Write((rows & vbLf))
        For a = 0 To 7
            sw.Write(TransMatrix(a) & vbLf)
        Next

        Dim fp = New System.Globalization.CultureInfo("en-US")

        For a = 0 To Points - 1
            sw.Write(x(a).ToString("N5", fp) & " " & y(a).ToString("N5", fp) & " " & z(a).ToString("N5", fp) & " " & i(a) & vbLf)
        Next

        sw.Close()
        fs.Close()
    End Sub

    Private Sub CalculateAndApllyFaro(ByVal Pfad As String, ByVal Prosfix As String)
        Dim fp = New System.Globalization.CultureInfo("en-US") 'decimal point

        'Read File
        Dim licenseCode = "FARO Open Runtime License" & vbLf &
                "Key:xxxxxxxxxxxxxxxxxxxxxx" & vbLf &
                vbLf &
                "The software is the registered property " &
                "of FARO Scanner Production GmbH, " &
                "Stuttgart, Germany." & vbLf &
                "All rights reserved." & vbLf &
                "This software may only be used with " &
                "written permission of FARO Scanner " &
                "Production GmbH, Stuttgart, Germany."

        Dim licLibIf As IiQLicensedInterfaceIf = New iQLibIfClass()
        licLibIf.License = licenseCode
        Dim libRef As IiQLibIf = CType(licLibIf, IiQLibIf)
        libRef.load(Pfad)

        rows = libRef.getScanNumRows(0)
        columns = libRef.getScanNumCols(0)
        libRef.scanReflectionMode = 0


        Dim EndFrontSight = libRef.getScanAnglesIf(0).ColEndFrontSight
        'MsgBox(libRef.getRootObject.getSphereObjSpeci)

        Dim TransMatrix(7) As String

        Points = columns * rows

        Dim x(Points - 1) As Double
        Dim y(Points - 1) As Double
        Dim z(Points - 1) As Double
        Dim i(Points - 1) As Short

        Dim r(Points - 1) As Double
        Dim alp(Points - 1) As Double
        Dim the(Points - 1) As Double


        'Dim fs As New IO.FileStream(Pfad, IO.FileMode.Open)
        'Dim sr As New IO.StreamReader(fs)

        TransMatrix(0) = "0.00000000 0.00000000 0.00000000"
        TransMatrix(1) = "1.00000000 0.00000000 0.00000000"
        TransMatrix(2) = "0.00000000 1.00000000 0.00000000"
        TransMatrix(3) = "0.00000000 0.00000000 1.00000000"
        TransMatrix(4) = "1.00000000 0.00000000 0.00000000 0.00000000"
        TransMatrix(5) = "0.00000000 1.00000000 0.00000000 0.00000000"
        TransMatrix(6) = "0.00000000 0.00000000 1.00000000 0.00000000"
        TransMatrix(7) = "0.00000000 0.00000000 0.00000000 1.00000000"


        Dim c As Integer = 0
        Dim ar() As String

        Dim pos As Double()
        Dim int As Integer()


        For co = 0 To columns - 1

            'libRef.getXYZScanPoints2(0, 0, co, rows, pos, int) ' Read full column
            'For ro = 0 To rows - 1
            '    a = co * rows + ro
            '    x(a) = pos(ro * 3)
            '    y(a) = pos(ro * 3 + 1)
            '    z(a) = pos(ro * 3 + 2)

            '    i(a) = int(ro)
            'Next

            libRef.getPolarScanPoints2(0, 0, co, rows, pos, int) ' Read full column Polar
            For ro = 0 To rows - 1
                a = co * rows + ro
                r(a) = pos(ro * 3)
                the(a) = pos(ro * 3 + 1)
                alp(a) = pos(ro * 3 + 2)
            Next

        Next

        libRef.unloadScan(0)


        Dim Roh As Double
        Dim Alpha As Double
        Dim Theta As Double

        For a = 0 To Points - 1

            Roh = Math.Sqrt(x(a) ^ 2 + y(a) ^ 2 + z(a) ^ 2)
            Alpha = Math.Atan(z(a) / Math.Sqrt(x(a) ^ 2 + y(a) ^ 2))
            Theta = Math.Atan2(y(a) * -1, x(a))

            Roh = r(a)
            Alpha = alp(a)
            Theta = the(a)

            If a / rows > EndFrontSight Then 'Secound scanner half
                Alpha = Math.PI - Alpha
                Theta -= Math.PI
            End If



            Theta -= p.B1 * Theta + p.B2 * Math.Sin(Theta) + p.B3 * Math.Cos(Theta) + p.B4 * Math.Sin(2 * Theta) + p.B5 * Math.Cos(2 * Theta) _
                + p.B6 * Sec(Alpha) + p.B7 * Math.Tan(Alpha) + p.B8 * Roh ^ -1 + p.B9 * Math.Sin(Alpha) + p.B10 * Math.Cos(Alpha) 'Delta Theta



            Alpha -= p.C0 + p.C1 * Alpha + p.C2 * Math.Sin(Alpha) + p.C3 * Math.Sin(2 * Alpha) + p.C4 * Math.Cos(2 * Alpha) + p.C5 * Roh ^ -1 _
                + p.C6 * Math.Sin(2 * Theta) + p.C7 * Math.Cos(2 * Theta) + p.C8 * Math.Sin(4 * Theta) 'Delta alpha

            Roh -= p.A0 + p.A1 * Roh + p.A2 * Math.Sin(Alpha) 'Delta roh
            If p.U1 > 0 Then
                Roh -= p.A3 * Math.Sin(((4 / Math.PI) / p.U1) * Roh) + p.A4 * Math.Cos(((4 / Math.PI) / p.U1) * Roh) 'Delta roh, only when u1 is not 0
            End If


            Alpha -= 1.570796326795

            x(a) = (Roh) * Math.Sin(Alpha) * Math.Cos(Theta) * -1
            y(a) = (Roh) * Math.Sin(Alpha) * Math.Sin(Theta)
            z(a) = (Roh) * Math.Cos(Alpha)

            If Double.IsNaN(x(a)) Then x(a) = 0
            If Double.IsNaN(y(a)) Then y(a) = 0
            If Double.IsNaN(z(a)) Then z(a) = 0


        Next



        'Write new File
        Dim fs = New IO.FileStream(Pfad.Substring(0, Pfad.Length - 4) & Prosfix & ".ptx", IO.FileMode.CreateNew)
        Dim sw As New IO.StreamWriter(fs)

        sw.Write((columns & vbLf))
        sw.Write((rows & vbLf))
        For a = 0 To 7
            sw.Write(TransMatrix(a) & vbLf)
        Next


        Dim int255 As Short
        For a = 0 To Points - 1
            int255 = 255 * (i(a) / 2047)
            sw.Write(x(a).ToString("N5", fp) & " " & y(a).ToString("N5", fp) & " " & z(a).ToString("N5", fp) & " " &
                     (1 / 2047 * i(a)).ToString("N5", fp) & " " & int255 & " " & int255 & " " & int255 & vbCrLf)
        Next

        sw.Close()
        fs.Close()
    End Sub

    Private Function Sec(angle As Double) As Double
        ' Calculate the secant of angle, in radians.
        Return 1.0 / Math.Cos(angle)
    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        PB_Files.Value = FileNumber
        PB_File.Maximum = Points
        PB_File.Value = a

        If fertig Then
            BT_AddPTX.Enabled = True
            BT_Read.Enabled = True
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim x As New System.Xml.Serialization.XmlSerializer(p.GetType)
        Dim fs As New FileStream("Parameter.xml", FileMode.Open)
        p = x.Deserialize(fs)
        fs.Close()
        PropertyGrid1.SelectedObject = p
    End Sub

    Private Sub BT_Save_Click(sender As Object, e As EventArgs) Handles BT_Save.Click
        Dim x As New System.Xml.Serialization.XmlSerializer(p.GetType)
        Dim fs As New FileStream("Parameter.xml", FileMode.Create)
        x.Serialize(fs, p)
        fs.Close()
    End Sub

End Class


Public Class Parameters



    <Category("A-Rangefinder"), Description("A0 describes the rangefinder offset")>
    Public Property A0 As Double

    <Category("A-Rangefinder"), Description("A1 describes the scale factor error")>
    Public Property A1 As Double

    <Category("A-Rangefinder"), Description("A2 describes the laser axis vertical offset")>
    Public Property A2 As Double

    <Category("A-Rangefinder"), Description("A3 and A4 describe the cyclic errors")>
    Public Property A3 As Double

    <Category("A-Rangefinder"), Description("A3 and A4 describe the cyclic errors")>
    Public Property A4 As Double

    <Category("A-Rangefinder"), Description("U1 describes the period(m) of the cyclic error")>
    Public Property U1 As Double




    <Category("B-Theta (vertical axis)"), Description("B1 describes the scale factor error")>
    Public Property B1 As Double

    <Category("B-Theta (vertical axis)"), Description("B2 and B3 describe the horizontal circle eccentricity")>
    Public Property B2 As Double

    <Category("B-Theta (vertical axis)"), Description("B2 and B3 describe the horizontal circle eccentricity ")>
    Public Property B3 As Double

    <Category("B-Theta (vertical axis)"), Description("B4 and B5 describe the non-orthogonality of encoder and vertical axis")>
    Public Property B4 As Double

    <Category("B-Theta (vertical axis)"), Description("B4 and B5 describe the non-orthogonality of encoder and vertical axis")>
    Public Property B5 As Double

    <Category("B-Theta (vertical axis)"), Description("B6 describes the collimation axis error")>
    Public Property B6 As Double

    <Category("B-Theta (vertical axis)"), Description("B7 describes the trunnion axis error")>
    Public Property B7 As Double

    <Category("B-Theta (vertical axis)"), Description("B8 describes horizontal eccentricity of collimation axis")>
    Public Property B8 As Double

    <Category("B-Theta (vertical axis)"), Description("B9 and B10 describe the trunnion axis wobble")>
    Public Property B9 As Double

    <Category("B-Theta (vertical axis)"), Description("B9 and B10 describe the trunnion axis wobble")>
    Public Property B10 As Double




    <Category("C-Alpha (Horizontal axis)"), Description("C0 describes the vertical circle index error")>
    Public Property C0 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C1 describes the scale factor error ")>
    Public Property C1 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C2 describes the vertical circle eccentricity")>
    Public Property C2 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C3 and C4 describe the non-orthogonality of encoder and trunnion axis")>
    Public Property C3 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C3 and C4 describe the non-orthogonality of encoder and trunnion axis")>
    Public Property C4 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C5 describes the vertical eccentricity of collimation axis")>
    Public Property C5 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C6, C7, and C8 describe the vertical axis wobble")>
    Public Property C6 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C6, C7, and C8 describe the vertical axis wobble")>
    Public Property C7 As Double

    <Category("C-Alpha (Horizontal axis)"), Description("C6, C7, and C8 describe the vertical axis wobble")>
    Public Property C8 As Double

End Class