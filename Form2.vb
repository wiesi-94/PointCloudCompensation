Imports System.Globalization

Public Class Form2

    Dim vektorUNDRol(3) As Double
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim sp1 = TextBox2.Text.Split(" ")
        Dim sp2 = TextBox3.Text.Split(" ")
        Dim sp3 = TextBox4.Text.Split(" ")

        Dim points(,) As Double = {
           {sp1(0), sp1(1), sp1(2)},
           {sp2(0), sp2(1), sp2(2)},
           {sp3(0), sp3(1), sp3(2)}
         }

        ' Normalenvektor der Ebene berechnen
        Dim normal() As Double = CalculatePlaneNormal(points)

        ' Normalenvektor nach oben
        If normal(2) < 0 Then
            normal(0) *= -1
            normal(1) *= -1
            normal(2) *= -1
        End If


        ' Drehachse und Drehwinkel zur Horizontalen berechnen
        Dim axis(2) As Double
        Dim angle_rad As Double
        CalculateRotationAxisAndAngle(normal, axis, angle_rad)


        Dim RM_End = Form1.CreateRotationMatrix(angle_rad, axis) 'Soll Rotationsmatrix um die drei Punkte in waage zu bringen
        vektorUNDRol = Form1.MatrixToAngleAxis(RM_End)

        Dim spA = TextBox5.Text.Split(" ")
        Dim RmIncl = Form1.CreateRotationMatrix(Double.Parse(TextBox6.Text, CultureInfo.InvariantCulture),
            {Double.Parse(spA(0), CultureInfo.InvariantCulture), Double.Parse(spA(1), CultureInfo.InvariantCulture), Double.Parse(spA(2), CultureInfo.InvariantCulture)}) 'Rotationsmatrix des Scanners (Inclinometer)
        vektorUNDRol = Form1.MatrixToAngleAxis(RmIncl)

        Dim RmComp = Form1.CalculateSecondRotationMatrix(RmIncl, RM_End) 'Dirffernz Rotationsmatrix (Fehler)
        vektorUNDRol = Form1.MatrixToAngleAxis(RmComp)


        TextBox8.Text = vektorUNDRol(0)
        TextBox9.Text = vektorUNDRol(1)
        TextBox10.Text = vektorUNDRol(2)

        TextBox7.Text = vektorUNDRol(3)
    End Sub



    Function CalculatePlaneNormal(points(,) As Double) As Double()
        ' Vektoren zwischen den Punkten
        Dim v1() As Double = {points(1, 0) - points(0, 0), points(1, 1) - points(0, 1), points(1, 2) - points(0, 2)}
        Dim v2() As Double = {points(2, 0) - points(0, 0), points(2, 1) - points(0, 1), points(2, 2) - points(0, 2)}

        ' Normalenvektor der Ebene durch Kreuzprodukt berechnen
        Dim normal() As Double = {
            v1(1) * v2(2) - v1(2) * v2(1),
            v1(2) * v2(0) - v1(0) * v2(2),
            v1(0) * v2(1) - v1(1) * v2(0)
        }

        ' Normalenvektor normieren
        Dim length As Double = Math.Sqrt(normal(0) * normal(0) + normal(1) * normal(1) + normal(2) * normal(2))
        normal = {normal(0) / length, normal(1) / length, normal(2) / length}

        Return normal
    End Function

    Sub CalculateRotationAxisAndAngle(normal() As Double, ByRef axis() As Double, ByRef angle_rad As Double)
        ' Die Horizontalebene hat die Normalenvektor (0, 0, 1)
        Dim horizontal_normal() As Double = {0, 0, 1}

        'Dim a = NumpyDotNet.np.cross(normal, horizontal_normal)

        ' Berechnung der Drehachse durch Kreuzprodukt
        axis = {
            normal(1) * horizontal_normal(2) - normal(2) * horizontal_normal(1),
            normal(2) * horizontal_normal(0) - normal(0) * horizontal_normal(2),
            normal(0) * horizontal_normal(1) - normal(1) * horizontal_normal(0)
        }

        Dim norm As Double = Math.Sqrt(axis(0) * axis(0) + axis(1) * axis(1) + axis(2) * axis(2))
        If norm <> 0 Then
            axis(0) /= norm
            axis(1) /= norm
            axis(2) /= norm
        End If

        ' Drehwinkel berechnen
        Dim dot_product = NumpyDotNet.np.dot(normal, horizontal_normal)
        angle_rad = NumpyDotNet.np.arccos(dot_product)

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form1.p.D1 = vektorUNDRol(0)
        Form1.p.D2 = vektorUNDRol(1)
        Form1.p.D3 = vektorUNDRol(2)
        Form1.p.D4 = vektorUNDRol(3)
        Form1.PropertyGrid1.Refresh()
    End Sub
End Class