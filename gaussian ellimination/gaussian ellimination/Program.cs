using PresiceMath;
using matrixMath;
using ColumnPrint;

Matrix mat;
Matrix coeficiant;
Matrix coeficiantInv;
Matrix answers;
Matrix variables;
while (true)
{
    bool hasInverse = false;
    Console.WriteLine("feed me coeficiant matrix A");
    if (!Matrix.UserInput(out coeficiant)) { break; }
    Console.WriteLine("feed me result vector B");
    if (!Matrix.UserInput(out answers)) { break; }

    Fraction[][] var = new Fraction[answers.matrixArr.Length][];
    for (int i = 0; i < var.Length; i++) { var[i] = new Fraction[] {new Fraction($"X{i}")}; }
    variables = new Matrix(var);

    Console.WriteLine("A * X = B");
    Console.WriteLine(CP.build($"{coeficiant}","*", $"{variables}", " = ", $"{answers}"));
    Console.WriteLine();

    Fraction det;
    if(coeficiant.Determinant(out det))
    {
        Console.Write("Det(A) = ");
        Console.WriteLine(det);
        Console.WriteLine();
    }

    if (coeficiant.Reciprocal(out coeficiantInv))
    {
        hasInverse = true;
        Console.WriteLine("A^-1 =");
        Console.WriteLine(coeficiantInv);
        Console.WriteLine();

        Console.WriteLine(CP.build($"{coeficiant}", "*", $"{coeficiantInv}", " = ", $"{coeficiant * coeficiantInv}"));
        Console.WriteLine();
    }


    mat = coeficiant.ConcatRow(answers);
    Console.WriteLine("the parameter matrix is");
    Console.WriteLine(mat);
    Console.WriteLine();
    Console.WriteLine("this is the reduced echelon parameter matrix");
    mat = mat.ReducedEchelon();
    Console.WriteLine(mat);
    Console.WriteLine();


    Console.WriteLine("the solution is");
    Fraction[] f = mat.BackSubtitution();

    if (f.Length == 0) { Console.WriteLine("no solutions"); }
    else { for (int i = 0; i < f.Length; i++) { Console.WriteLine($"X{i} = {f[i]}"); } }
    
    if(hasInverse)
    {
        Console.WriteLine("\nA^-1 * B = x");
        Console.WriteLine(CP.build($"{coeficiantInv}", "*", $"{answers}", " = ", $"{coeficiantInv * answers}"));
    }
    Console.WriteLine("\n\n\n");
}