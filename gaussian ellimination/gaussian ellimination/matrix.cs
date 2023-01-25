using PresiceMath;
using System.Text;

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

        public static bool operator !=(Matrix a, Matrix b) => !(a == b);
        public static bool operator == (Matrix a, Matrix b)
        {
            if(!(a.Size == b.Size)) { return false; }
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
        public static Matrix operator /(Matrix a, Matrix b)
        {
            Matrix m;
            if(!b.Reciprocal(out m)) return a;
            return a * m;
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.matrixArr.Length != b.matrixArr[0].Length) 
            {
                return new Matrix(new Fraction[][] { }); 
            }
            Fraction[][] m = new Fraction[a.matrixArr[0].Length][];

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new Fraction[b.matrixArr.Length];
                for (int j = 0; j < m[i].Length; j++)
                {
                    m[i][j] = a.matrixArr[i][j] * b.matrixArr[j][i];
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

        public Fraction[] GauseElimination()
        {
            return ReducedEchelon().BackSubtitution();
        }

        // -------------- //
        //  Elimination  //

        public Fraction[] BackSubtitution()
        {
            int answerLength = matrixArr[0].Length - 1;
            Fraction[] answers = new Fraction[answerLength];
            for (int i = 0; i < answers.Length; i++)
            {
                answers[i] = new Fraction(1, 0);
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
                if (nonZero == 0 & matrixArr[i][matrixArr[i].Length - 1] != 0) { return new Fraction[0]; }
                else if (nonZero == 1)
                {
                    answers[firstNonZero] = rowAnswer;
                }
                else if (nonZero > 1)
                {
                    for (int j = answerLength - 1; j > firstNonZero; j--)
                    {
                        if (!answers[j].Finite())
                        {
                            answers[j] = new Fraction($"N{variableN++}");
                        }
                        if (matrixArr[i][j] != 0)
                        {
                            rowAnswer -= matrixArr[i][j] * answers[j];
                        }
                    }
                    answers[firstNonZero] = rowAnswer / matrixArr[i][firstNonZero];
                }
            }
            return answers;
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
                string front = "| ";
                foreach (Fraction f in row)
                {
                    sb.Append($"{front}{new string(' ', maxLen - f.ToString().Length)}{f}");
                    front = ", ";
                }
                sb.Append(" |\n");
            }
            return sb.ToString();
        }
    }
}