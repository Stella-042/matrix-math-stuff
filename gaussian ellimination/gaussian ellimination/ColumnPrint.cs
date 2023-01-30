namespace ColumnPrint
{
    public static class CP
    {
        public static void print(params string[] s)
        {
            string[][] noLF = new string[s.Length][];
            int[] cols = new int[s.Length];
            int[] rows = new int[s.Length];
            int maxRows = 0;
            for(int i = 0; i < s.Length; i++)
            {
                noLF[i] = s[i].Split('\n');
                maxRows = Math.Max(noLF[i].Length, maxRows);
                foreach(string line in noLF[i])
                {
                    cols[i] = Math.Max(line.Length, cols[i]);
                }
            }
            for (int i = 0; i < s.Length; i++)
            {
                rows[i] = (maxRows - noLF[i].Length) / 2;
            }
            for (int i = 0; i < maxRows; i++)
            {
                for (int j = 0; j < s.Length; j++)
                {
                    int ir = i - rows[j];
                    if(ir < noLF[j].Length & ir >= 0)
                    {
                        Console.Write(noLF[j][ir]);
                        Console.Write(new string(' ', cols[j] - noLF[j][ir].Length));
                    }
                    else
                    {
                        Console.Write(new string(' ', cols[j]));
                    }
                }
                Console.Write('\n');
            }
        }
    }
}
