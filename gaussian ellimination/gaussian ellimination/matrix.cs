using PresiceMath;
using System.Text;
using ColumnPrint;

namespace matrixMath
{
    public readonly struct Matrix
    {
        public readonly Fraction[][] matrixArr;

        public Matrix(Fraction[][] m)
        {
            matrixArr = m;
        }
        public (int, int) Size()
        {
            return ( matrixArr.Length, matrixArr[0].Length );
        }
        public bool RowEquivilent(Matrix b)
        {
            return b.ReducedEchelon() == this.ReducedEchelon();
        }
        public Matrix ConcatRow(Matrix b)
        {
            Fraction[][] m = new Fraction[matrixArr.Length][];
            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new Fraction[matrixArr[i].Length + b.matrixArr[i].Length];
                for (int j = 0; j < Math.Max(matrixArr[i].Length, b.matrixArr[i].Length); j++)
                {
                    if(j < matrixArr[i].Length)
                    {
                        m[i][j] = matrixArr[i][j];
                    }
                    if(j < b.matrixArr[i].Length)
                    {
                        m[i][j + matrixArr[i].Length] = b.matrixArr[i][j];
                    }
                }
            }
            return new Matrix(m);
        }
        public Matrix ConcatCol(Matrix b)
        {
            Fraction[][] m = new Fraction[matrixArr.Length + b.matrixArr.Length][];
            for (int i = 0; i < Math.Max(matrixArr.Length, b.matrixArr.Length); i++)
            {
                if(i < matrixArr.Length)
                {
                    m[i] = matrixArr[i];
                }
                if(i < b.matrixArr.Length)
                {
                    m[i + matrixArr.Length] = b.matrixArr[i];
                }
            }
            return new Matrix(m);
        }
        public Matrix[] SplitRow(int n)
        {
            Fraction[][] m1 = new Fraction[matrixArr.Length][];
            Fraction[][] m2 = new Fraction[matrixArr.Length][];
            for (int i = 0; i < matrixArr.Length; i++)
            {
                m1[i] = new Fraction[n];
                m2[i] = new Fraction[matrixArr.Length - n];
                for (int j = 0; j < Math.Max(n, m2.Length); j++)
                {
                    if(j < n)
                    {
                        m1[i][j] = matrixArr[i][j];
                    }
                    if(j < m2[i].Length)
                    {
                        m2[i][j] = matrixArr[i][j + matrixArr.Length];
                    }
                }
            }
            return new Matrix[]{ new Matrix(m1), new Matrix(m2)};
        }
        public Matrix[] SplitCol(int n)
        {
            Fraction[][] m1 = new Fraction[n][];
            Fraction[][] m2 = new Fraction[matrixArr.Length - n][];
            for (int i = 0; i < Math.Max(n, m2.Length); i++)
            {
                if(i < n)
                {
                    m1[i] = matrixArr[i];
                }
                if(i < m2.Length)
                {
                    m2[i] = matrixArr[i + matrixArr.Length];
                }
            }
            return new Matrix[]{ new Matrix(m1), new Matrix(m2)};
        }

        public static bool operator !=(Matrix a, Matrix b) => !(a == b);
        public static bool operator == (Matrix a, Matrix b)
        {
            if(!(a.Size() == b.Size())) { return false; }
            for(int i = 0; i < a.matrixArr.Length; i++)
            {
                for (int j = 0; j < a.matrixArr[i].Length; j++)
                {
                    if(a.matrixArr[i][j] != b.matrixArr[i][j]) { return false; }
                }
            }
            return true;
        }

        public static Matrix operator *(Matrix a, int b) => a * new Fraction(b);
        public static Matrix operator *(Matrix a, Fraction b)
        {
            Matrix m = a;
            for (int i = 0; i < m.matrixArr.Length; i++)
            {
                for (int j = 0; j < m.matrixArr[i].Length; j++)
                {
                    m.matrixArr[i][j] *= b;
                }
            }
            return m;
        }
        public bool Reciprocal(out Matrix output)
        {
            output = this;
            if (matrixArr.Length != matrixArr[0].Length) return false;
            Fraction[][] mArr = new Fraction[matrixArr.Length][];
            for(int i = 0; i < mArr.Length; i++)
            {
                mArr[i] = new Fraction[matrixArr[i].Length * 2];
                for(int j = 0; j < matrixArr[i].Length; j++)
                {
                    mArr[i][j] = matrixArr[i][j];
                    mArr[i][j + matrixArr[i].Length] = new Fraction((j == i) ? 1 : 0);
                }
            }
            Matrix m = new Matrix(mArr).ReducedEchelon();

            mArr = new Fraction[matrixArr.Length][];
            for (int i = 0; i < mArr.Length; i++)
            {
                mArr[i] = new Fraction[matrixArr[i].Length];
                for (int j = 0; j < matrixArr[i].Length; j++)
                {
                    mArr[i][j] = m.matrixArr[i][j + matrixArr[i].Length];
                    if (m.matrixArr[i][j] != new Fraction((j == i) ? 1 : 0)) return false;
                }
            }
            output = new Matrix(mArr);
            return true;
        }
        
        public bool Determinant(out Fraction f)
        {
            f = new Fraction(0);
            if (matrixArr.Length != matrixArr[0].Length) return false;
            else if(Size() == (2,2))
            {
                f = matrixArr[0][0] * matrixArr[1][1] - matrixArr[1][0] * matrixArr[0][1];
                return true;
            }
            for (int n = 0; n < matrixArr.Length; n++)
            {
                Fraction[][] m = new Fraction[matrixArr.Length - 1][];
                for (int i = 0; i < m.Length; i++)
                {
                    m[i] = new Fraction[matrixArr[i].Length - 1];
                    for (int j = 0; j < m[i].Length; j++)
                    {
                        m[i][j] = matrixArr[i + 1][(j < n) ? j : j + 1];
                    }
                }
                Fraction det;
                new Matrix(m).Determinant(out det);
                f += matrixArr[0][n] * (((n & 1) == 0) ? det : -det);
            }
            return true;
        }

        public static Matrix operator /(Matrix a, Matrix b)
        {
            Matrix m;
            if(!b.Reciprocal(out m)) return new Matrix(new Fraction[0][]);
            return a * m;
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.matrixArr[0].Length != b.matrixArr.Length) 
            {
                return new Matrix(new Fraction[0][]); 
            }
            Fraction[][] m = new Fraction[a.matrixArr.Length][];

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new Fraction[b.matrixArr[0].Length];
                for (int j = 0; j < m[i].Length; j++)
                {
                    m[i][j] = new Fraction(0);
                    for (int n = 0; n < b.matrixArr.Length; n++)
                    {
                        m[i][j] += a.matrixArr[i][n] * b.matrixArr[n][j];
                    }
                }
            }

            return new Matrix(m);
        }
        public Fraction get (int i, int j) => matrixArr[i][j];
        public static Matrix operator -(Matrix a) => a * -1;
        public static Matrix operator +(Matrix a) => a;
        public static Matrix operator -(Matrix a, Matrix b) => a + (-b);
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (!(a.Size == b.Size)) { return new Matrix(new Fraction[][]{ }); }
            Matrix m = a;
            for (int i = 0; i < m.matrixArr.Length; i++)
            {
                for (int j = 0; j < m.matrixArr[i].Length; j++)
                {
                    m.matrixArr[i][j] += b.matrixArr[i][j];
                }
            }
            return m;
        }

        public static bool UserInput(out Matrix mat)
        {
            bool sucess = true;
            do
            {
                if (!sucess) { Console.WriteLine("invalid matrix, try again\n"); }
                string c;
                string matStr = "";
                string lf = "";
                while (true)
                {
                    c = Console.ReadLine();
                    if (c == "") { break; }
                    matStr += lf + c;
                    lf = "\n";
                }

                sucess = Matrix.tryParse(matStr, out mat);
                if (matStr.Length == 0) { return false; }
            }
            while (!sucess);
            return true;
        }

        public static bool tryParse(string matStr, out Matrix mat)
        {
            string[] matRowsStr = matStr.Split('\n');

            Fraction[][] matrix = new Fraction[matRowsStr.Length][];

            string[] rowStr = matRowsStr[0].Replace(" ", "").Split(',');
            int columns = rowStr.Length;

            for (int i = 0; i < matRowsStr.Length; i++)
            {
                rowStr = matRowsStr[i].Replace(" ", "").Split(',');
                if (!(rowStr.Length == columns))
                {
                    mat = new Matrix(matrix);
                    return false;
                }

                matrix[i] = new Fraction[columns];
                for (int j = 0; j < columns; j++)
                {
                    if (!Fraction.TryParse(rowStr[j], out matrix[i][j]))
                    {
                        mat = new Matrix(matrix);
                        return false;
                    }
                }
            }
            mat = new Matrix(matrix);
            return true;
        }

        private Fraction[] copy(Fraction[] a)
        {
            Fraction[] result = new Fraction[a.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i];
            }
            return result;
        }

        public bool GauseElimination(out Matrix mat)
        {
            return Echelon().BackSubtitution(out mat);
        }

        // -------------- //
        //  Elimination  //

        public bool BackSubtitution(out Matrix mat)
        {
            mat = this;
            int answerLength = matrixArr[0].Length - 1;
            Fraction[][] answers = new Fraction[answerLength][];
            for (int i = 0; i < answers.Length; i++)
            {
                answers[i] = new Fraction[] { new Fraction(1, 0) };
            }

            for (int i = Math.Min(matrixArr.Length - 1, answerLength); i >= 0; i--)
            {
                int nonZero = 0;
                int firstNonZero = 0;
                int variableN = 0;
                Fraction rowAnswer = matrixArr[i][answerLength];

                for (int j = answerLength - 1; j >= 0 ; j--)
                {
                    if (matrixArr[i][j] != 0) { nonZero++; firstNonZero = j; }
                }
                if (nonZero == 0 & matrixArr[i][matrixArr[i].Length - 1] != 0) { return false; }
                else if (nonZero == 1)
                {
                    answers[firstNonZero][0] = rowAnswer;
                }
                else if (nonZero > 1)
                {
                    for (int j = answerLength - 1; j > firstNonZero; j--)
                    {
                        if (!answers[j][0].Finite())
                        {
                            answers[j][0] = new Fraction($"N{variableN++}");
                        }
                        if (matrixArr[i][j] != 0)
                        {
                            rowAnswer -= matrixArr[i][j] * answers[j][0];
                        }
                    }
                    answers[firstNonZero][0] = rowAnswer / matrixArr[i][firstNonZero];
                }
            }
            mat = new Matrix(answers);
            return true;
        }

        // -------------- //
        // Gause Function //

        public Matrix Echelon()
        {
            return new Matrix(GauseStart());
        }

        public Matrix ReducedEchelon()
        {
            Fraction[][] mat = GauseStart();

            for (int i = 0; i < mat.Length; i++)
            {
                Fraction s = new Fraction(1);
                bool firstFound = false;
                int firstJ = 0;
                for(int j = 0; j < mat[i].Length; j++)
                {
                    if(firstFound)
                    {
                        mat[i][j] /= s;
                    }
                    else if(mat[i][j] != 0)
                    {
                        s = mat[i][j];
                        mat[i][j] = new Fraction(1);
                        firstFound = true;
                        firstJ = j;
                    }
                }
                if (!firstFound) { break; }

                for (int iN = 0; iN < i; iN++)
                {
                    s = mat[iN][firstJ];
                    mat[iN][firstJ] = new Fraction(0);
                    for (int jN = firstJ + 1; jN < mat[i].Length; jN++)
                    {
                        mat[iN][jN] -= mat[i][jN] * s;
                    }
                }
            }

            return new Matrix(mat);
        }

        private Fraction[][] GauseStart()
        {
            Fraction[][] matrixTrans = new Fraction[matrixArr.Length][];

            for (int i = 0; i < matrixArr.Length; i++)
            {
                matrixTrans[i] = copy(matrixArr[i]);
            }
            GauseRecursive(0, 0, ref matrixTrans);
            return matrixTrans;
        }

        private void GauseRecursive(int starti, int startj, ref Fraction[][] matrixTrans)
        {
            bool stepDone = false;
            int newj = matrixTrans[0].Length;
            Fraction[] working = copy(matrixTrans[starti]);

            // step 1, finding first row with earliest non zero value and making it first row //
            for (int j = startj; j < matrixTrans[0].Length & !stepDone; j++)
            {
                for (int i = starti; i < matrixTrans.Length & !stepDone; i++)
                {
                    if (matrixTrans[i][j] != 0)
                    {
                        stepDone = true;
                        newj = j;
                        matrixTrans[starti] = copy(matrixTrans[i]);
                        matrixTrans[i] = copy(working);
                    }
                }
            }

            if (newj == matrixTrans[0].Length) { return; } // if no onz zero found, return

            // step 2, elliminating the other first row candidates //
            for(int i = starti + 1; i < matrixTrans.Length; i++)
            {
                Fraction f = matrixTrans[i][newj];
                if (f != 0)
                {
                    working = copy(matrixTrans[starti]);
                    f /= working[newj];
                    matrixTrans[i][newj] = new Fraction(0);
                    for (int n = newj + 1; n < working.Length; n++)
                    {
                        matrixTrans[i][n] -= working[n] * f;
                    }
                }
            }

            // step 3, repeating till done //
            if (starti + 1 == matrixTrans.Length) { return; }
            GauseRecursive(starti + 1, newj + 1, ref matrixTrans);
            return;
        }

        // -------------- //

        public override string ToString()
        {
            int maxLen = 0;
            foreach (Fraction[] F in matrixArr)
            {
                foreach (Fraction f in F)
                {
                    maxLen = Math.Max(maxLen, f.ToString().Length);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (Fraction[] row in matrixArr)
            {
                sb.Append("|");
                string separator = " ";
                foreach (Fraction F in row)
                {
                    string f = F.ToString();
                    sb.Append($"{separator}{new string(' ', maxLen - f.Length)}{f}");
                    separator = ", ";
                }
                sb.Append(" |\n");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}