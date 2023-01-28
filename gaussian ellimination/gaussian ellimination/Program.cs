using PresiceMath;
using matrixMath;
using System;

Matrix mat;
Matrix coeficiant;
Matrix coeficiantInv;
Matrix answers;
while (true)
{
    bool hasInverse = false;
    Console.WriteLine("feed me coeficiant matrix A");
    if (!Matrix.UserInput(out coeficiant)) { break; }
    Console.WriteLine("feed me result vector B");
    if (!Matrix.UserInput(out answers)) { break; }
    
    Console.WriteLine("A =");
    Console.WriteLine(coeficiant);
    Console.WriteLine("B =");
    Console.WriteLine(answers);

    if (coeficiant.Reciprocal(out coeficiantInv))
    {
        hasInverse = true;
        Console.WriteLine("A^-1 =");
        Console.WriteLine(coeficiantInv);
    }


    mat = coeficiant.ConcatRow(answers);
    Console.WriteLine("the parameter matrix is");
    Console.WriteLine(mat);
    Console.WriteLine("this is the reduced echelon parameter matrix");
    mat = mat.ReducedEchelon();
    Console.WriteLine(mat);

    
    Console.WriteLine("the solution is");
    Fraction[] f = mat.BackSubtitution();

    if (f.Length == 0) { Console.WriteLine("no solutions"); }
    else { for (int i = 0; i < f.Length; i++) { Console.WriteLine($"X{i} = {f[i]}"); } }
    
    if(hasInverse)
    {
        Console.WriteLine("\nB * A^-1 =");
        Console.WriteLine(coeficiantInv * answers);
    }
    Console.WriteLine("\n\n\n");
}