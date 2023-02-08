namespace PresiceMath
{
    public readonly struct Fraction
    {
        private readonly Number num;
        private readonly Number den;

        public Fraction(int numerator)
        {
            num = new Number(numerator);
            den = new Number(1);
        }
        public Fraction(string numerator)
        {
            num = new Number(1,numerator);
            den = new Number(1);
        }
        private Fraction(Number numerator, Number denominator)
        {
            Number[] ls = construct(numerator, denominator);
            num = ls[0];
            den = ls[1];
        }
        public Fraction(int numerator, int denominator)
        {
            Number[] ls = construct(new Number(numerator), new Number(denominator));
            num = ls[0];
            den = ls[1];
        }
        private static Number[] construct(Number numerator, Number denominator)
        {
            if (numerator == denominator) return new Number[] { new Number(1), new Number(1) };
            if (numerator == 0) return new Number[] { new Number(0), new Number(1) };
            if (denominator == 0) return new Number[] { new Number(1), new Number(0) };
            if (numerator.known() & denominator.known())
            {
                int N0 = numerator.constant;
                int N1 = denominator.constant;
                int sign = Math.Sign(N0) * Math.Sign(N1);
                N0 = Math.Abs(N0);
                N1 = Math.Abs(N1);

                bool greaterThan1 = (N0 > N1);
                if (greaterThan1 & N0 % N1 == 0)
                {
                    return new Number[] { new Number(sign * N0 / N1), new Number(1) };
                }
                else if (!greaterThan1 & N1 % N0 == 0)
                {
                    return new Number[] { new Number(sign), new Number(N1 / N0) };
                }
            }
            if (denominator.constant < 0) return new Number[] { -numerator, -denominator };
            return new Number[]{numerator, denominator};
        }

        public static bool TryParse(string value, out Fraction fraction)
        {
            string[] ab = value.Split('/');
            Number a, b;
            if (ab.Length == 1)
            {
                if (Number.TryParse(ab[0], out a))
                {
                    fraction = new Fraction(a, new Number(1));
                    return true;
                }
            }
            else if (ab.Length == 2)
            {
                if (Number.TryParse(ab[0], out a) & Number.TryParse(ab[1], out b))
                {
                    fraction = new Fraction(a, b);
                    return true;
                }
            }
            fraction = new Fraction(0, 1);
            return false;
        }
        public static bool operator ==(Fraction a, int b) => (a == (new Fraction(b)));
        public static bool operator !=(Fraction a, int b) => !(a == b);
        public static bool operator ==(Fraction a, Fraction b) => (a - b).num == 0;
        public static bool operator !=(Fraction a, Fraction b) => !(a == b);
        public static Fraction operator +(Fraction a) => a;
        public static Fraction operator -(Fraction a) => new Fraction(-a.num, a.den);

        public static Fraction operator +(Fraction a, Fraction b)
            => new Fraction(a.num * b.den + b.num * a.den, a.den * b.den);
        public static Fraction operator -(Fraction a, Fraction b)=> a + (-b);
        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a.num * b.num, a.den * b.den);
        public static Fraction operator /(Fraction a, Fraction b)
            => new Fraction(a.num * b.den, a.den * b.num);

        public static Fraction operator +(Fraction a, int b) => a + new Fraction(b);
        public static Fraction operator -(Fraction a, int b) => a + (-b);
        public static Fraction operator *(Fraction a, int b) => a * new Fraction(b);
        public static Fraction operator /(Fraction a, int b) => a / new Fraction(b);

        public static Fraction operator +(int a, Fraction b) => new Fraction(a) + b;
        public static Fraction operator -(int a, Fraction b) => a + (-b);
        public static Fraction operator *(int a, Fraction b) => new Fraction(a) * b;
        public static Fraction operator /(int a, Fraction b) => new Fraction(a) / b;

        public bool Finite() { return den != 0; }
        public override string ToString()
        {
            if(den == 1)
            {
                return $"{num}"; 
            }
            else if(den == 0)
            {
                return "INF";
            }
            else if (num == 0)
            {
                return "0";
            }
            else
            {
                return $"{(num.known()? $"{num}" : $"({num})")} / {(den.known() ? $"{den}" : $"({den})")}";
            }
        }

        private struct Number
        {
            private product[] products;
            public int constant;

            public Number(int num)
            {
                products = new product[0];
                constant = num;
            }
            public Number(int cof, string prod, int num = 0, int exp = 1)
            {
                products = new product[] { new product(new Unknown(prod,exp), cof) };
                constant = num;
            }
            private Number(product[] prod, int num = 0)
            {
                products = prod;
                constant = num;
            }

            public bool known() { return products.Length == 0; }

            public static bool TryParse(string value, out Number number)
            {
                number = new Number(0);
                string[] terms = value.Replace("+", "+-").Replace(" ", "").Split("+");
                List<product> prod = new List<product>();
                int con = 0;
                int num = 0;
                foreach (string p in terms)
                {
                    if(int.TryParse(p,out num)) { con += num; continue; }
                    string[] unknowns = p.Split("*");
                    List<Unknown> un = new List<Unknown>();
                    int cof = 1;
                    foreach (string u in unknowns)
                    {
                        if (int.TryParse(u, out num)) { cof *= num; continue; }
                        num = 1;
                        string[] parts = p.Split("^");
                        if (parts.Length > 2) { return false; }
                        if (parts.Length == 2) 
                        {
                            if (!int.TryParse(parts[1], out num)) { return false; }
                        }
                        un.Add(new Unknown(parts[0], num));
                    }
                    prod.Add(new product(un.ToArray(), cof));
                }
                number = new Number(prod.ToArray(),con);
                return true;
            }

            public Number copy()
            {
                product[] prod = new product[products.Length];
                for (int i = 0; i < prod.Length; i++) { prod[i] = products[i].copy(); }
                return new Number(prod, constant);
            }

            public static bool operator ==(Number a, int b)
            {
                return (a.products.Length == 0) & (a.constant == b);
            }

            public static bool operator !=(Number a, int b) => !(a == b);
            public static bool operator ==(Number a, Number b)
            {
                Number n = a - b;
                return (n.products.Length == 0) & (n.constant == 0);
            }
            public static bool operator !=(Number a, Number b) => !(a == b);
            public static Number operator -(Number a, Number b) => a + (-b);
            public static Number operator +(Number a) => a;
            public static Number operator -(Number a)
            {
                product[] prod = new product[a.products.Length];
                int num = -a.constant;

                for (int i = 0; i < prod.Length; i++)
                {
                    prod[i] = a.products[i].copy();
                    prod[i].coeficiant *= -1;
                }

                return new Number(prod, num);
            }

            public static Number operator *(Number a, Number b)
            {
                Number output = new Number(0);

                foreach (product bp in b.products)
                {
                    bool ZC = a.constant == 0;
                    product[] prod = new product[a.products.Length + (ZC? 0 : 1)];
                    for (int i = 0; i < a.products.Length; i++)
                    {
                        prod[i] = a.products[i] * bp;
                    }
                    if (!ZC)
                    {
                        prod[a.products.Length] = bp.copy();
                        prod[a.products.Length].coeficiant *= a.constant;
                    }
                    output += new Number(prod);
                }

                if (b.constant != 0)
                {
                    Number aCopy = a.copy();
                    aCopy.constant *= b.constant;
                    for (int i = 0; i < a.products.Length; i++)
                    {
                        aCopy.products[i].coeficiant *= b.constant;
                    }
                    output += aCopy;
                }

                return output;
            }

            public static Number operator +(Number a, Number b)
            {
                List<product> prod = new List<product>();
                bool[] aHandeled = new bool[a.products.Length];

                foreach (product bp in b.products)
                {
                    bool similar = false;
                    for (int i = 0; i < a.products.Length; i++)
                    {
                        if (a.products[i].isMultiple(bp))
                        {
                            int coef = a.products[i].coeficiant + bp.coeficiant;
                            aHandeled[i] = true;
                            similar = true;
                            if (coef != 0) { prod.Add(new product(bp.unknowns, coef)); }
                        }
                    }
                    if (!similar)
                    {
                        prod.Add(bp.copy());
                    }
                }
                for (int i = 0; i < a.products.Length; i++) { if (!aHandeled[i]) { prod.Add(a.products[i]); } }
                return new Number(prod.ToArray(), a.constant + b.constant);
            }

            public override string ToString()
            {
                string outStr = (constant == 0) ? "" : $"{constant}";
                foreach (product p in products)
                {
                    if (outStr.Length == 0) { if (p.coeficiant < 0) { outStr += "-"; } }
                    else { outStr += (p.coeficiant < 0) ? " - " : " + "; }
                    outStr += p;
                }
                return (outStr.Length == 0) ? "0" : outStr;
            }

            // extra strucs //

            private struct product
            {
                public Unknown[] unknowns;
                public int coeficiant;
                public product(Unknown symb, int coef)
                {
                    unknowns = new Unknown[] { symb };
                    coeficiant = coef;
                }
                public product(Unknown[] symb, int coef)
                {
                    unknowns = symb;
                    coeficiant = coef;
                }

                public product copy()
                {
                    Unknown[] un = new Unknown[unknowns.Length];
                    int coef = coeficiant;
                    for (int i = 0; i < un.Length; i++) { un[i] = unknowns[i]; }
                    return new product(un, coef);
                }

                public bool isMultiple(product a)
                {
                    foreach (Unknown n in unknowns)
                    {
                        bool exists = false;
                        foreach (Unknown m in a.unknowns)
                        {
                            if (n.symbol.Equals(m.symbol) & n.exponent == m.exponent)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists) { return false; }
                    }
                    foreach (Unknown n in a.unknowns)
                    {
                        bool exists = false;
                        foreach (Unknown m in unknowns)
                        {
                            if (n.symbol.Equals(m.symbol) & n.exponent == m.exponent)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists) { return false; }
                    }
                    return true;
                }

                public static bool operator ==(product a, product b)
                {
                    if (a.coeficiant != b.coeficiant) { return false; }
                    return a.isMultiple(b);
                }

                public static bool operator !=(product a, product b) => !(a == b);

                public static product operator *(product a, product b)
                {
                    List<Unknown> un = new List<Unknown>();
                    bool[] aHandeled = new bool[a.unknowns.Length];

                    foreach (Unknown bp in b.unknowns)
                    {
                        bool similar = false;
                        for (int i = 0; i < a.unknowns.Length; i++)
                        {
                            if (a.unknowns[i].symbol.Equals(bp.symbol))
                            {
                                int exp = a.unknowns[i].exponent + bp.exponent;
                                aHandeled[i] = true;
                                similar = true;
                                if (exp != 0) { un.Add(new Unknown(bp.symbol, exp)); }
                            }
                        }
                        if (!similar)
                        {
                            un.Add(bp);
                        }
                    }
                    for (int i = 0; i < a.unknowns.Length; i++) { if (!aHandeled[i]) { un.Add(a.unknowns[i]); } }
                    return new product(un.ToArray(), a.coeficiant * b.coeficiant);
                }

                public override string ToString()
                {
                    string start = (Math.Abs(coeficiant) == 1) ? "" : $"{Math.Abs(coeficiant)}*";
                    string output = "";
                    foreach (Unknown un in unknowns)
                    {
                        output += $"{start}{un}";
                        start = "*";
                    }
                    return output;
                }
            }

            private readonly struct Unknown
            {
                public readonly string symbol;
                public readonly int exponent;

                public Unknown(string s, int exp = 1)
                {
                    symbol = s;
                    exponent = exp;
                }

                public static bool operator ==(Unknown a, Unknown b)
                {
                    return a.symbol.Equals(b.symbol) & a.exponent == b.exponent;
                }
                public static bool operator !=(Unknown a, Unknown b) => !(a == b);
                public override string ToString()
                {
                    return (exponent == 1) ? $"{symbol}" : $"{symbol}^{exponent}";
                }
            }
        }
    }
}