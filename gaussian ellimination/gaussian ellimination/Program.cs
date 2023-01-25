using PresiceMath;
using matrixMath;
using System;

Matrix mat;
Matrix mat2;
while (true)
{
    Console.Write("feed me matrix, here is template\nx,x,x\nx,x,x\nx,x,x\n\n");
    if (!Matrix.UserInput(out mat)) { break; }

    Console.WriteLine(mat);

    if (mat.Reciprocal(out mat2))
    {
        Console.WriteLine("this is the recipocal matrix");
        Console.WriteLine(mat2);
    }
    else { Console.WriteLine("this dont have a recipocal"); }

    Console.WriteLine("this is the reduced echelon matrix");

    mat = mat.ReducedEchelon();
    Fraction[] f = mat.BackSubtitution();

    Console.WriteLine(mat);
    Console.WriteLine("the solution is");
    if (f.Length == 0) { Console.WriteLine("no solutions"); }
    else { for (int i = 0; i < f.Length; i++) { Console.WriteLine($"X{i} = {f[i]}"); } }
    Console.WriteLine("\n\n\n");
}