using System.Text.RegularExpressions;

namespace DpeZak.Kmp
{

    /// <summary>
    /// Die Klasse enthält Angaben zu den Filtern einer Abfrage (where)
    /// Sie dient zum Erzeugen von dynamic-linc.
    /// Sie kann Filter im KMP Stil (Feldname=Suchkriterien) verarbeiten.
    /// </summary>
    public partial class FltrList
    {
        public string SqlWhere { get; set; }
        public IDictionary<int, object> SqlParams { get; set; }
        private int paramCount;

        /// <summary>
        /// Parameter: Wertstring typisieren (int, float datetime usw)
        /// </summary>
        /// <param name="wert">Wert als string</param>
        /// <param name="fieldinfo">Info zum Feldtyp</param>
        /// <returns>den Index des Parameters (für {0})</returns>
        /// <exception cref="NotImplementedException">fehlender Typ</exception>
        public int AddParam(string wert, FieldInfo fieldinfo)
        {
            object param = fieldinfo.fieldType switch
            {
                FieldType.ftString => wert,
                FieldType.ftInt => int.Parse(wert),
                FieldType.ftFloat => double.Parse(wert),
                FieldType.ftDateTime => DateTime.Parse(wert),
                _ => throw new NotImplementedException()
            };

            SqlParams.Add(paramCount, param);
            return paramCount++;
        }

        /// <summary>
        /// SQL Generieren. Schreibt nach FltrList.SqlWhere
        /// entityFieldlist um Datentyp zu erhalten
        /// </summary>
        /// <param name="entityFieldlist">Liste der Feldtyp Infos</param>
        public void GenSqlWhere(IDictionary<string, FieldInfo> entityFieldlist)
        {
            SqlParams.Clear();
            paramCount = 0;
            foreach (var item in Fltrs)
            {
                item.GenKmpTokList();  //setzt SqlOrFlag, KmpTokList
            }

            //vergl gensqlterm
            string sql = "";
            string boolOp;
            bool oldSqlOrFlag = true;
            bool firstFlag = true;
            foreach (var item in Fltrs)
            {
                if (!entityFieldlist.ContainsKey(item.Fieldname))
                    throw new ArgumentException($"FltrList: ungültiges Feld ({item.Fieldname})");
                var fieldinfo = entityFieldlist[item.Fieldname];
                item.GenLinqSqlStr(fieldinfo, this);
                if (oldSqlOrFlag != item.SqlOrFlag)
                {
                    if (sql.Length > 0)
                        sql = "(" + sql + ")";
                    oldSqlOrFlag = item.SqlOrFlag;
                }
                boolOp = firstFlag ? "" : item.SqlOrFlag ? " or " : " and ";
                firstFlag = false;
                sql += $"{boolOp}({item.LinqSqlStr})";
            }
            SqlWhere = sql;
        }

    }


    /// <summary>
    /// Verarbeitung der Suchkriterien einer Zeile, z.B. &gt;5&lt;10
    /// </summary>
    public class KmpTokItem
    {
        public string KmpTokStr { get; set; }
        public bool OrFlag { get; set; }
        public string SqlToken { get; set; }
        public bool NotFlag { get; set; }
        public bool BlockFlag { get; set; }

        public KmpTokItem(string kmpTokstr, bool orFlag)
        {
            KmpTokStr = kmpTokstr;
            OrFlag = orFlag;
        }

        public KmpTokItem()
        {
        }



        /// <summary>
        /// Ergibt true bei Native Block (mit {} umgeben) idF &amp;{F1=5}
        /// </summary>
        /// <param name="kmpstr"></param>
        /// <returns></returns>
        public static bool IsNativBlock(string kmpstr)
        {
            var Expression = @"\{(.*?)\}";
            Match m = Regex.Match(kmpstr, Expression);
            return m.Success;

        }


        /// <summary>
        /// ersetzt in kmpstr einen Ausdruck idF [Ausdruck] nach bla Ausdruck blu
        /// ergibt neuen string oder String.Empty wenn kein [Ausdruck] vorliegt.
        /// Idee: Feldnamen Groß/Klein korrigieren -> erst zum Schluss in SqlWhere oder so
        /// </summary>
        /// <param name="kmpstr"></param>
        /// <returns></returns>
        public static string GetNativAusdruck(string kmpstr)
        {
            var Expression = @"\[(.*?)\]";
            Match m = Regex.Match(kmpstr, Expression);
            if (m.Success)
                return kmpstr.Replace("[", " ").Replace("]", " ");
            else
                return string.Empty;

        }

        /// <summary>
        /// Übersetzt KMP Metazeichen in originäre
        /// </summary>
        /// <param name="kmpstr">Inputstring</param>
        /// <returns>Umgesetzter String</returns>
        public static string TranslateSql(string kmpstr)
        {
            string result = kmpstr;
            string Expression;
            string Replacement;

            const int MAX_AUSDRUCK = 10;
            string[] Ausdruck = new string[MAX_AUSDRUCK];
            string[] AusdruckGuid = new string[MAX_AUSDRUCK]
            {
                "a7084c00-748a-41f7-8d24-8c8cc27a2e5d",
                "df42a913-c5fc-489f-afdd-cb31707e947c",
                "d40bf98e-cb51-41f5-aa8b-a00a5ca49af0",
                "4dbf6e4b-b724-4c5c-a481-600d2606635c",
                "86b9e5b4-7208-4f70-8982-110e61a6bde3",
                "4705c122-c2f7-44d4-840a-6df1847a0795",
                "187281c0-cb62-4f31-82af-32511111c3a0",
                "fce1b347-8aa7-4bd4-a942-8d93cfb65c55",
                "ded3d14c-58ac-44c5-946a-c1880dcce47c",
                "cde0ac98-fef5-4451-a32e-31970a85d07e",
            };

            //Block {} nie umformen. Bzw ' nach " umsetzen wg Linq
            if (IsNativBlock(kmpstr))
            {
                result = result.Replace("'", "\""); // {F1='aha'} --> {F1="aha"}
                return result;
            }

            // 'sysdate', 'heute', 'today' -> [DateTime.Now]
            if (!result.Contains("[sysdate", StringComparison.OrdinalIgnoreCase))
                result = result.Replace("sysdate", "[DateTime.Now.Date]", StringComparison.OrdinalIgnoreCase);
            result = result.Replace("heute", "[DateTime.Now.Date]", StringComparison.OrdinalIgnoreCase);
            result = result.Replace("today", "[DateTime.Now.Date]", StringComparison.OrdinalIgnoreCase);

            //es sind nur 10 [Ausdruck]e pro Block möglich. Siehe Ende.
            //Text innerhaln [] unverändert lassen
            Expression = @"\[(.*?)\]";
            for (int i = 0; i < MAX_AUSDRUCK; i++)
            {
                Match m = Regex.Match(result, Expression);
                if (m.Success)
                {
                    Ausdruck[i] = m.Value;  // mit []
                    Ausdruck[i] = Ausdruck[i].Replace("'", "\""); // [F1+'aha'] --> [F1+"aha"]
                    result = result.Replace(Ausdruck[i], AusdruckGuid[i]); // [F1]>5 --> [Guid]>5
                }
            }

            // fester Bereich: {fester Block} -> {F1 like "%abc%" or F1 is null} 
            // fester Bereich: [Fester Ausdruck] -> [TARA * 0.05]
            // ::abc -> ..abc
            // ' -> ?  # nach <'> nicht suchbar
            // ? -> _  # evtl erst später
            // _ -> %  # Linq kennt kein '_' ???
            // <>..a  ->  <>%a%   #not like
            // ..a.. -> %a%
            // ..ab -> %ab%
            // a..b -> a~b
            // ab.. -> ab%
            // *abc, ab*c, abc* -> %abc, ab%c, abc%
            // neu: a~b ->  >=a&<=b
            //      a~<b -> >=a&<b

            Expression = @"(\:\:)(\w+)";
            Replacement = @"..$2";
            result = Regex.Replace(result, Expression, Replacement); // ::abc -> ..abc

            Expression = @"\'";
            Replacement = @"?";
            result = Regex.Replace(result, Expression, Replacement); // ' -> ?  (nach ' nicht suchbar)
            Expression = @"\?";
            Replacement = @"_";
            result = Regex.Replace(result, Expression, Replacement); // ? -> _  (evtl erst später)
            Expression = @"_";
            Replacement = @"%";
            result = Regex.Replace(result, Expression, Replacement); // _-> %  # Linq kennt kein '_'

            Expression = @"(\<\>\.\.)(\w+)";
            Replacement = @"<>%$2%";
            result = Regex.Replace(result, Expression, Replacement); // <>..a ->  <>%a%   #not like

            Expression = @"(\.\.)(\w+)(\.\.)";
            Replacement = @"%$2%";
            result = Regex.Replace(result, Expression, Replacement); // ..a.. -> %a%

            Expression = @"(\.\.)(\w+)";
            Replacement = @"%$2%";
            result = Regex.Replace(result, Expression, Replacement); // ..ab -> %ab%

            Expression = @"(\w+)(\.\.)(\w+)";
            Replacement = @"$1~$3";
            result = Regex.Replace(result, Expression, Replacement); // a..b -> a~b

            Expression = @"(\w+)(\.\.)";
            Replacement = @"$1%";
            result = Regex.Replace(result, Expression, Replacement); // ab.. -> ab%

            Expression = @"(\*)(\w+)(\*)";
            Replacement = @"%$2%";
            result = Regex.Replace(result, Expression, Replacement); // *ab* -> %ab%  a*b*c
            Expression = @"(\*)(\w+)";
            Replacement = @"%$2";
            result = Regex.Replace(result, Expression, Replacement); // *abc -> %abc
            Expression = @"(\w+)(\*)(\w+)";
            Replacement = @"$1%$3";
            result = Regex.Replace(result, Expression, Replacement); // ab*c -> ab%c
            Expression = @"(\w+)(\*)";
            Replacement = @"$1%";
            result = Regex.Replace(result, Expression, Replacement); // abc* -> abc%

            for (int i = 0; i < MAX_AUSDRUCK; i++)
            {
                if (Ausdruck[i] != "")
                {
                    result = result.Replace(AusdruckGuid[i], Ausdruck[i]);
                }
            }

            return result;
        }

        //wandelt Feld+KmpToken (>5) in Linq Ausdruck (FLD>5) um
        //wird direkt hinter Fieldname gesetzt, also Blank vor Buchstaben:
        public void GenSqlToken(FieldInfo fieldInfo, FltrList owner)
        {
            BlockFlag = false;
            if (IsNativBlock(KmpTokStr))
            {
                var s1 = KmpTokStr.Replace("{", "").Replace("}", "");
                SqlToken = s1;  //ohne {} mit Feldname
                NotFlag = false;
                BlockFlag = true;
                return;
            }
            //bereits in Line string kmptoken = TranslateSql(KmpTokStr);  //..abs -> %abs% usw
            string kmptoken = KmpTokStr;
            string operand = "=";  //in dynamic Linq ok
            string operand2 = "";  //Klammer zu bei Contains
            string wert = kmptoken;
            string param = "";

            string[] ops1 = new string[] { "<", ">", "=" };
            string[] ops2 = new string[] { "<>", "<=", ">=" };
            if (kmptoken.Length >= 1 && ops1.Contains(kmptoken[0..1]))
            {
                if (kmptoken.Length >= 2 && ops2.Contains(kmptoken[0..2]))
                {
                    operand = kmptoken[0..2];
                    if (kmptoken.Length > 2)
                        wert = kmptoken[2..];
                    else
                    {
                        wert = "";
                        if (operand == ">=")
                        {
                            operand = "<>";
                            param = "null";
                        }
                    }
                }
                else  //<, >, = :
                {
                    operand = kmptoken[0..1];
                    if (kmptoken.Length > 1)
                        wert = kmptoken[1..];
                    else
                    {
                        wert = "";
                        if (operand == "=")
                            param = "null";
                    }
                }
            }
            if (DposUtils.HasFilterChar(wert))
            {
                NotFlag = operand == "<>";  //not Fld.contains..
                if (wert.StartsWith('%') && wert.EndsWith('%'))
                    operand = ".Contains(";
                else if (wert.StartsWith('%'))
                    operand = ".EndsWith(";
                else if (wert.EndsWith('%'))
                    operand = ".StartsWith(";
                else
                    operand = ".Contains(";
                wert = wert.Replace("%", "");
                operand2 = ")";
            }
            if (!String.IsNullOrEmpty(wert))
            {
                param = GetNativAusdruck(wert);  // [Ausdruck] -> Ausdruck oder ""
                if (param == String.Empty)
                {
                    int nr = owner.AddParam(wert, fieldInfo);
                    param = $"@{nr}";
                }
                else
                    BaseUtils.Debug0();
            }
            SqlToken = $"{operand}{param}{operand2}";
        }
    }

    public partial class FltrListItem
    {
        //von GenKmpTokList befüllt:
        public IList<KmpTokItem> KmpTokList { get; set; }
        public string LinqSqlStr { get; set; }
        public bool SqlOrFlag { get; set; }
        private bool OraPlus { get; set; }
        private bool NullOrNotFlag { get; set; }
        private bool NotFlag { get; set; }

        // Where Teil ein Feld bzw eine Zeile betreffend. Ergebnis in SqlWhere und SqlOrFlag. Quelle in KmpStr
        // F1=a;b => F1=a or F1=b => F1={n1} or F2={m2} und SqlParams [n1]='a'  [n2]=b
        // Erstellt Token Listen KmpAnd/OrList einer Zeile
        public void GenKmpTokList()
        {
            SqlOrFlag = false;
            NullOrNotFlag = false;
            NotFlag = false;
            OraPlus = false;

            var kmpstr = KmpStr;
            if (kmpstr.StartsWith(";"))
            {
                SqlOrFlag = true;
                kmpstr = kmpstr[1..];
            }

            if (kmpstr.Length >= 4 && kmpstr.StartsWith("=;!"))
            {
                NullOrNotFlag = true;
                kmpstr = kmpstr[2..];  //mit !
            }

            if (kmpstr.Length >= 2 && kmpstr.StartsWith("!"))
            {
                NotFlag = true;
                kmpstr = kmpstr[1..];
            }

            if (kmpstr.IndexOf("||") > 0)
            {
                OraPlus = true;
                kmpstr = kmpstr.Replace("||", "+");
            }

            //Blockaufteilung nach rawlist, Trenner ist '&' oder ';':
            //rawlist enthält Blocktrenner (&;) ab 2. Block
            KmpTokList = new List<KmpTokItem>();
            var rawList = new List<string>();
            string KmpTokExpr = @"(^[^(\&|\;)]+)|((\&|\;)[^(\&|\;)]*)";
            for (Match m = Regex.Match(kmpstr, KmpTokExpr); m.Success; m = m.NextMatch())
            {
                rawList.Add(m.Value);
            }
            int i = 0;
            while (i < rawList.Count)
            {
                string toki = rawList[i];
                if (KmpTokItem.IsNativBlock(toki))
                {
                    // {aaa} bleibt unverändert
                    // Standardbehandlung
                    toki = toki.Replace("'", "\""); // {F1='aha'} --> {F1="aha"}
                    var kmpTokItem = new KmpTokItem
                    {
                        OrFlag = !toki.StartsWith("&"),  //true bei erstem Token ohne &;
                        KmpTokStr = toki
                    };
                    if (toki.StartsWith("&") || toki.StartsWith(";"))
                        kmpTokItem.KmpTokStr = toki[1..];
                    KmpTokList.Add(kmpTokItem);
                    i++;
                    continue;
                }
                //& Sonderbehandlung: an vorherigen Token anhängen:  ('Gmbh & Co KG' zusammenhalten)
                int j = i + 1;
                while (j < rawList.Count)
                {
                    string tokj = rawList[j];
                    //& (ohne Wert)  &<Alphanum>  &<Blank>  &<%>
                    //<5&>60;%ab%c%d%e -> <5 >60 %ab%c%d%e
                    string SonderExpr = @"(^\&$)|(^\&\s)|(^\&\w)|(^\&\%)";
                    if (Regex.IsMatch(tokj, SonderExpr))
                    {
                        j++;
                        toki += tokj;
                    }
                    else
                        break;
                }
                i = j;

                toki = KmpTokItem.TranslateSql(toki);  // ..a-->%a%  a*b-->a%b und x..y-->x~y
                string logicPrefix = "";

                // % Sonderbehandlung idF ab%cd --> %ab, %cd und %ab%c%d%e --> %ab%, %c%, %d%, %e
                var sl1 = toki.Split('%', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);  // &a%b --> &%a%, &%b%
                // ~ Sonderbehandlung idF x~y --> >x und <y
                var sl2 = toki.Split('~', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);  // x~y --> x, y

                if (sl1.Length > 1)
                {
                    foreach (string toki1 in sl1)
                    {
                        string toki2;
                        if (toki.Contains("%" + toki1 + "%")) toki2 = logicPrefix + "%" + toki1 + "%";
                        else if (toki.Contains("%" + toki1)) toki2 = logicPrefix + "%" + toki1;
                        else if (toki.Contains(toki1 + "%")) toki2 = logicPrefix + toki1 + "%";
                        else toki2 = toki1;  //Error
                        logicPrefix = "&";
                        var kmpTokItem = new KmpTokItem
                        {
                            OrFlag = !toki2.StartsWith("&"),  //true bei erstem Token ohne &;
                            KmpTokStr = toki2
                        };
                        if (toki2.StartsWith("&") || toki2.StartsWith(";"))
                        {
                            kmpTokItem.KmpTokStr = toki2[1..];
                        }
                        KmpTokList.Add(kmpTokItem);
                    }
                }
                else
                if (sl2.Length > 1)
                {
                    string toki2 = sl2[0];  //x --> >=x, >x --> >x
                    bool orFlag = !toki2.StartsWith("&");  //true bei erstem Token ohne &;
                    if (toki2.StartsWith("&") || toki2.StartsWith(";"))
                        toki2 = toki2[1..];
                    if (!toki2.StartsWith(">"))
                        toki2 = ">=" + toki2;
                    var kmpTokItem = new KmpTokItem
                    {
                        OrFlag = orFlag,
                        KmpTokStr = toki2
                    };
                    KmpTokList.Add(kmpTokItem);

                    toki2 = sl2[1];  //y --> <=x, <y --> <y
                    if (!toki2.StartsWith("<"))
                        toki2 = "<=" + toki2;
                    kmpTokItem = new KmpTokItem
                    {
                        OrFlag = false,  //immer and
                        KmpTokStr = toki2
                    };
                    KmpTokList.Add(kmpTokItem);
                }
                else
                {
                    // Standardbehandlung
                    var kmpTokItem = new KmpTokItem
                    {
                        OrFlag = !toki.StartsWith("&"),  //true bei erstem Token ohne &;
                        KmpTokStr = toki
                    };
                    if (toki.StartsWith("&") || toki.StartsWith(";"))
                        kmpTokItem.KmpTokStr = toki[1..];
                    KmpTokList.Add(kmpTokItem);
                }
            }

            //wir haben jetzt einzelne Tokens, pro &; Block
            //Tokens übersetzen: Operator:like, Operant:Datetime, Number, todo:Auswahl als Parameter
            //           spezial: '=':is null, '>='is not null
            //in FltrList.GenSqlWhere
        }

        //baut sql einer Zeile zusammen. Anhand Tokens und Fieldinfo.  n => F1=@0;Param.AddString(n) # a;b => F1=@1 or F1=@2; Param
        //and's klammern
        //Aufruf von GetSqlToken
        public void GenLinqSqlStr(DpeZak.Kmp.FieldInfo fieldInfo, FltrList owner)
        {
            string s1 = "";
            string boolOp;
            string notOp;
            bool oldOrFlag = true;
            bool firstFlag = true;
            bool secondFlag = false;
            foreach (var kmptoken in KmpTokList)
            {
                kmptoken.GenSqlToken(fieldInfo, owner);  //setzt SqlToken und NotFlag
                string s2 = kmptoken.SqlToken;
                string sqlFieldname = kmptoken.BlockFlag ? "" : Fieldname;
                notOp = kmptoken.NotFlag ? " not " : ""; //für not contains()
                if (secondFlag)
                {
                    oldOrFlag = kmptoken.OrFlag;
                    secondFlag = false;
                }
                if (firstFlag)
                {
                    oldOrFlag = kmptoken.OrFlag;
                    boolOp = "";
                    firstFlag = false;
                    secondFlag = true;
                }
                else
                    boolOp = kmptoken.OrFlag ? " or " : " and ";
                if (oldOrFlag != kmptoken.OrFlag)
                {
                    if (s1.Length > 0)
                        s1 = "(" + s1 + ")";
                    oldOrFlag = kmptoken.OrFlag;
                }
                s1 += $"{boolOp}{notOp}{sqlFieldname}{s2}";
            }
            if (NotFlag)
            {
                if (NullOrNotFlag)
                    s1 = $"{Fieldname}=null or not ({s1})";
                else
                    s1 = $"not ({s1})";
            }
            if (OraPlus)
            {
                s1 = s1.Replace("||", "+");
            }
            LinqSqlStr = s1;
        }
    }
}
