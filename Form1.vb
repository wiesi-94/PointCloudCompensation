Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports IQOPENLib

Public Class Form1

    Dim columns As Integer
    Dim rows As Integer
    Dim Points As Integer

    Friend p As New Parameters

    Dim a As Integer

    Dim Pfade As String()
    Dim FileNumber As Integer

    Dim fertig As Boolean = False

    Dim Status As String = "Wait for start"

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
        Dim fp = New System.Globalization.CultureInfo("en-US") 'decimal point

        'Read File
        Status = "Reading file"
        Dim fs As New IO.FileStream(Pfad, IO.FileMode.Open)
        Dim sr As New IO.StreamReader(fs)

        columns = sr.ReadLine()
        rows = sr.ReadLine()

        Points = columns * rows

        Dim x(Points - 1) As Double
        Dim y(Points - 1) As Double
        Dim z(Points - 1) As Double
        Dim i(Points - 1) As String

        Dim TransMatrix(7) As String
        For a = 0 To 7
            TransMatrix(a) = sr.ReadLine
        Next


        Dim RmIncl(2, 2) As Double 'Rotation matrix inclinometer

        Dim line() As String = TransMatrix(4).Split(" ") 'Row 7
        RmIncl(0, 0) = Double.Parse(line(0), CultureInfo.InvariantCulture)
        RmIncl(1, 0) = Double.Parse(line(1), CultureInfo.InvariantCulture)
        RmIncl(2, 0) = Double.Parse(line(2), CultureInfo.InvariantCulture)

        line = TransMatrix(5).Split(" ") 'Row 8
        RmIncl(0, 1) = Double.Parse(line(0), CultureInfo.InvariantCulture)
        RmIncl(1, 1) = Double.Parse(line(1), CultureInfo.InvariantCulture)
        RmIncl(2, 1) = Double.Parse(line(2), CultureInfo.InvariantCulture)

        line = TransMatrix(6).Split(" ") 'Row 9
        RmIncl(0, 2) = Double.Parse(line(0), CultureInfo.InvariantCulture)
        RmIncl(1, 2) = Double.Parse(line(1), CultureInfo.InvariantCulture)
        RmIncl(2, 2) = Double.Parse(line(2), CultureInfo.InvariantCulture)


        Dim RmComp = CreateRotationMatrix(p.D4, {p.D1, p.D2, p.D3})

        Dim newRotMat = CombineRotationMatrices(RmIncl, RmComp)
        Dim vektorUNDRol = MatrixToAngleAxis(newRotMat)

        TransMatrix(4) = newRotMat(0, 0).ToString("N8", fp) & " " & newRotMat(1, 0).ToString("N8", fp) & " " & newRotMat(2, 0).ToString("N8", fp) & " 0.00000000" 'Row 7, rotation matrix
        TransMatrix(5) = newRotMat(0, 1).ToString("N8", fp) & " " & newRotMat(1, 1).ToString("N8", fp) & " " & newRotMat(2, 1).ToString("N8", fp) & " 0.00000000" 'Row 8, rotation matrix
        TransMatrix(6) = newRotMat(0, 2).ToString("N8", fp) & " " & newRotMat(1, 2).ToString("N8", fp) & " " & newRotMat(2, 2).ToString("N8", fp) & " 0.00000000" 'Row 9, rotation matrix


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

        Status = "Apply compensation"

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


        Status = "Writing file"

        'Write new File
        fs = New IO.FileStream(Pfad.Substring(0, Pfad.Length - 4) & Prosfix & ".ptx", IO.FileMode.CreateNew)
        Dim sw As New IO.StreamWriter(fs)

        sw.Write((columns & vbLf))
        sw.Write((rows & vbLf))
        For a = 0 To 7
            sw.Write(TransMatrix(a) & vbLf)
        Next

        For a = 0 To Points - 1
            sw.Write(x(a).ToString("N5", fp) & " " & y(a).ToString("N5", fp) & " " & z(a).ToString("N5", fp) & " " & i(a) & vbLf)
        Next

        sw.Close()
        fs.Close()
    End Sub

    Public Function CreateRotationMatrix(angle As Double, axis As Double()) As Double(,)
        Dim radians As Double = angle '* Math.PI / 180 ' Winkel in Bogenmaß umrechnen
        Dim x As Double = axis(0)
        Dim y As Double = axis(1)
        Dim z As Double = axis(2)

        ' Normiere die Achse
        Dim norm As Double = Math.Sqrt(x * x + y * y + z * z)
        x /= norm
        y /= norm
        z /= norm

        Dim cosTheta As Double = Math.Cos(radians)
        Dim sinTheta As Double = Math.Sin(radians)
        Dim oneMinusCosTheta As Double = 1 - cosTheta

        ' Rotationsmatrix nach Rodrigues' Formel
        Dim rotationMatrix(2, 2) As Double
        rotationMatrix(0, 0) = cosTheta + x * x * oneMinusCosTheta
        rotationMatrix(0, 1) = x * y * oneMinusCosTheta - z * sinTheta
        rotationMatrix(0, 2) = x * z * oneMinusCosTheta + y * sinTheta

        rotationMatrix(1, 0) = y * x * oneMinusCosTheta + z * sinTheta
        rotationMatrix(1, 1) = cosTheta + y * y * oneMinusCosTheta
        rotationMatrix(1, 2) = y * z * oneMinusCosTheta - x * sinTheta

        rotationMatrix(2, 0) = z * x * oneMinusCosTheta - y * sinTheta
        rotationMatrix(2, 1) = z * y * oneMinusCosTheta + x * sinTheta
        rotationMatrix(2, 2) = cosTheta + z * z * oneMinusCosTheta

        Return rotationMatrix
    End Function

    Public Function CombineRotationMatrices(matrix1 As Double(,), matrix2 As Double(,)) As Double(,)
        ' Überprüfe, ob die Matrizen die richtige Größe haben (3x3)
        If matrix1.GetLength(0) <> 3 OrElse matrix1.GetLength(1) <> 3 OrElse matrix2.GetLength(0) <> 3 OrElse matrix2.GetLength(1) <> 3 Then
            Throw New ArgumentException("Beide Matrizen müssen 3x3 Matrizen sein.")
        End If

        ' Initialisiere die Ergebnis-Matrix
        Dim result(2, 2) As Double

        ' Führe die Matrizenmultiplikation durch
        For i As Integer = 0 To 2
            For j As Integer = 0 To 2
                result(i, j) = 0
                For k As Integer = 0 To 2
                    result(i, j) += matrix1(i, k) * matrix2(k, j)
                Next
            Next
        Next

        Return result
    End Function

    Public Function TransposeMatrix(matrix As Double(,)) As Double(,)
        Dim result(2, 2) As Double

        For i As Integer = 0 To 2
            For j As Integer = 0 To 2
                result(i, j) = matrix(j, i)
            Next
        Next

        Return result
    End Function

    Public Function CalculateSecondRotationMatrix(firstMatrix As Double(,), endMatrix As Double(,)) As Double(,)
        ' Berechne die Inverse (Transponierte) der ersten Rotationsmatrix
        Dim inverseFirstMatrix As Double(,) = TransposeMatrix(firstMatrix)

        ' Berechne die zweite Rotationsmatrix
        Dim secondMatrix As Double(,) = CombineRotationMatrices(inverseFirstMatrix, endMatrix)

        Return secondMatrix
    End Function

    Public Function MatrixToAngleAxis(rotationMatrix As Double(,)) As Double()
        ' Überprüfe, ob die Matrix die richtige Größe hat (3x3)
        If rotationMatrix.GetLength(0) <> 3 OrElse rotationMatrix.GetLength(1) <> 3 Then
            Throw New ArgumentException("Die Matrix muss eine 3x3 Matrix sein.")
        End If

        ' Berechne den Drehwinkel
        Dim angle As Double = Math.Acos((rotationMatrix(0, 0) + rotationMatrix(1, 1) + rotationMatrix(2, 2) - 1) / 2)

        ' Berechne die Drehachse
        Dim sinTheta As Double = Math.Sin(angle)
        Dim axisX As Double = (rotationMatrix(2, 1) - rotationMatrix(1, 2)) / (2 * sinTheta)
        Dim axisY As Double = (rotationMatrix(0, 2) - rotationMatrix(2, 0)) / (2 * sinTheta)
        Dim axisZ As Double = (rotationMatrix(1, 0) - rotationMatrix(0, 1)) / (2 * sinTheta)

        ' Normiere die Achse
        Dim norm As Double = Math.Sqrt(axisX * axisX + axisY * axisY + axisZ * axisZ)
        If norm <> 0 Then
            axisX /= norm
            axisY /= norm
            axisZ /= norm
        End If

        Return {axisX, axisY, axisZ, angle}
    End Function

    Private Sub CalculateAndApllyFaro(ByVal Pfad As String, ByVal Prosfix As String)
        Dim fp = New System.Globalization.CultureInfo("en-US") 'decimal point
        Status = "Reading file"
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

        Dim RotAxis(2) As Double
        Dim RotAngle As Double

        libRef.getScanOrientation(0, RotAxis(0), RotAxis(1), RotAxis(2), RotAngle)
        Dim RmIncl = CreateRotationMatrix(RotAngle, RotAxis)
        Dim RmComp = CreateRotationMatrix(p.D4, {p.D1, p.D2, p.D3})

        Dim newRotMat = CombineRotationMatrices(RmIncl, RmComp)
        Dim vektorUNDRol = MatrixToAngleAxis(newRotMat)

        TransMatrix(0) = "0.00000000 0.00000000 0.00000000" 'Row 3, point of origin and primary axes
        TransMatrix(1) = "1.00000000 0.00000000 0.00000000" 'Row 4, point of origin and primary axes
        TransMatrix(2) = "0.00000000 1.00000000 0.00000000" 'Row 5, point of origin and primary axes
        TransMatrix(3) = "0.00000000 0.00000000 1.00000000" 'Row 6, point of origin and primary axes

        TransMatrix(4) = newRotMat(0, 0).ToString("N8", fp) & " " & newRotMat(1, 0).ToString("N8", fp) & " " & newRotMat(2, 0).ToString("N8", fp) & " 0.00000000" 'Row 7, rotation matrix
        TransMatrix(5) = newRotMat(0, 1).ToString("N8", fp) & " " & newRotMat(1, 1).ToString("N8", fp) & " " & newRotMat(2, 1).ToString("N8", fp) & " 0.00000000" 'Row 8, rotation matrix
        TransMatrix(6) = newRotMat(0, 2).ToString("N8", fp) & " " & newRotMat(1, 2).ToString("N8", fp) & " " & newRotMat(2, 2).ToString("N8", fp) & " 0.00000000" 'Row 9, rotation matrix
        TransMatrix(7) = " 0.00000000  0.00000000  0.00000000  1.00000000"  'Row 10, translation vector


        Dim c As Integer = 0
        Dim ar() As String

        Dim pos As Double()
        Dim int As Integer()



        For co = 0 To columns - 1
            libRef.getPolarScanPoints2(0, 0, co, rows, pos, int) ' Read full column Polar
            For ro = 0 To rows - 1
                a = co * rows + ro
                r(a) = pos(ro * 3)
                the(a) = pos(ro * 3 + 1)
                alp(a) = pos(ro * 3 + 2)

                i(a) = int(ro)
            Next
        Next



        libRef.unloadScan(0)

        Status = "Apply compensation"
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

            If a / rows > EndFrontSight And Not EndFrontSight = -1 Then 'Secound scanner half
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


        Status = "Writing file"
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

        LB_Status.Text = Status

        If fertig Then
            BT_AddPTX.Enabled = True
            BT_Read.Enabled = True
            Status = "Finished!"
            fertig = False
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim dia As New Form2
        dia.ShowDialog()
    End Sub

    Private Sub BT_Del_Click(sender As Object, e As EventArgs) Handles BT_Del.Click
        ListBox1.Items.Clear()
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


    <Category("D-Inclinometer"), Description("D1 Rotation-Axis x")>
    Public Property D1 As Double

    <Category("D-Inclinometer"), Description("D2 Rotation-Axis y")>
    Public Property D2 As Double

    <Category("D-Inclinometer"), Description("D3 Rotation-Axis z")>
    Public Property D3 As Double

    <Category("D-Inclinometer"), Description("D4 Rotation")>
    Public Property D4 As Double
End Class