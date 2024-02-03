using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Reflection.Metadata;

namespace DpeZak.Kmp
{
    internal static class DposUtils
    {
        #region Feldnamen korrigieren (Groß/Klein)

        // Feldnamen korrigieren (Groß/Klein) anhand Entity Property Names und Relflection

        public static string AdjustFieldname(string Fieldname, IList<string> FieldList)
        {
            foreach (var field in FieldList)
            {
                string fieldname = field.ToString();
                if (fieldname.ToUpper() == Fieldname.ToUpper())
                    return fieldname;
            }
            return Fieldname;
        }

        //Feldnamen korrigieren: IDicionary Keys korrigieren
        //zB für FormatList, FLD1=Asw,Status -> fld1=Asw,Status
        public static void AdjustListKeys(IDictionary Fieldnames, IList<string> FieldList)
        {
            foreach (string oldname in Fieldnames.Keys)
            {
                string newname = AdjustFieldname(oldname, FieldList);
                if (newname != oldname)
                {
                    Fieldnames[newname] = Fieldnames[oldname];
                    Fieldnames.Remove(oldname);
                }
            }
        }

        //Fieldliste einer Entity. Mit FieldInfos. Mit Formatstrings von Formatlist (wenn vorhanden)
        public static IDictionary<string, FieldInfo> GetFieldlist(Type TEntity)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = TEntity.GetProperties();  // BindingFlags.Public);

            // Formatlist laden:
            // In Entity:
            // [NotMapped]
            // public IDictionary<string, string> Formatlist { get; private set; } = new Dictionary<string, string>() ...
            string constFormatlist = "Formatlist"; //in Entity:
            bool hasFormatlist = propertyInfos.Where(x => x.Name == constFormatlist).FirstOrDefault() != null;
            var formatlist = new Dictionary<string, string>();
            if (hasFormatlist)
            {
                var entity = Activator.CreateInstance(TEntity, true);
                var s1 = entity.GetType().GetProperty(constFormatlist).GetValue(entity, null);
                var helplist = (Dictionary<string, string>)s1;
                //Anlegen case insensitiv:
                foreach (var item in helplist)
                    formatlist.Add(item.Key.ToUpper(), item.Value);
            }

            // write fieldlist
            // nur Felder mit Getter und Setter (auch [NotMapped])
            var fieldlist = new Dictionary<string, FieldInfo>();
            foreach (PropertyInfo propertyInfo in propertyInfos.
                Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null))
            {
                string formatstring = "";
                //Vergleichen case insensitiv:
                if (hasFormatlist && formatlist != null)
                    formatlist.TryGetValue(propertyInfo.Name.ToUpper(), out formatstring);

                fieldlist.Add(propertyInfo.Name, new FieldInfo(propertyInfo.PropertyType, formatstring));
            }

            // Fieldinfo.Formatstring befüllen:

            return fieldlist;
        }

        public static bool HasFilterChar(string kmpstr)
        {
            return kmpstr.Contains('%') || kmpstr.Contains('_');
        }

        #endregion

    }

    [Flags]
    public enum FormatOptions
    {
        alNone = 0,
        alRight = 1,          // r,  RzTextAlign := taRightJustify
        alLeft = 2,           // l,  RzTextAlign := taLeftJustify
        alCenter = 4,         // c,  RzTextAlign := taCenter
        alIgnoreName = 8,     //IGN, Fehlenden Feldnamen ignorieren
        alAutoGenerate = 16,  //A,   vom Server verwalteter Vorgabewert
        alRequired = 32,      //N,   Mussfeld
        alReadOnly = 64,      //R,   Nur-Lesen Feld
        alInternalCalc = 128, //C,   Feld wird in der Datenbank berechnet (nicht in DB speichern)
        alInt = 256,          //INT, Werte in Sql als Integer formatieren (statt string)->sql: F=12->(int)F=12
        alTrimLeft0 = 512,    //TL0, 0en links weg
        alAsw = 1024,         //ASW,<a>  a=Auswahlname  
    }

    public class FieldInfo
    {
        private string _formatstring;
        public FormatOptions Options;
        public string AswName;
        public string Formatstring { 
            get => _formatstring;
            set
            {
                // bestimmt _formatstring, Options und AswName:
                Options = FormatOptions.alNone;
                AswName = "";
                _formatstring = "";
                if (!string.IsNullOrEmpty(value))
                {
                    var fl = value.Split(",");
                    for (int i = 0; i < fl.Length; i++)
                    {
                        switch (fl[i])
                        {
                            case "r":
                                Options |= FormatOptions.alRight;
                                break;
                            case "l":
                                Options |= FormatOptions.alLeft;
                                break;
                            case "c":
                                Options |= FormatOptions.alCenter;
                                break;
                            case "IGN":
                                Options |= FormatOptions.alIgnoreName;
                                break;
                            case "A":
                                Options |= FormatOptions.alAutoGenerate;
                                break;
                            case "N":
                                Options |= FormatOptions.alRequired;
                                break;
                            case "R":
                                Options |= FormatOptions.alReadOnly;
                                break;
                            case "C":
                                Options |= FormatOptions.alInternalCalc;
                                break;
                            case "INT":
                                Options |= FormatOptions.alInt;
                                break;
                            case "TL0":
                                Options |= FormatOptions.alTrimLeft0;
                                break;
                            case "ASW":
                                Options |= FormatOptions.alAsw;
                                AswName = fl[++i];
                                break;
                            default:
                                _formatstring = string.Join(",", fl[i..]);
                                i = fl.Length;  //Ende
                                break;
                        }
                    }
                    if (_formatstring != null)
                        if (!_formatstring.StartsWith("{"))  //0.00 -> {0:0.00} wg string.Format(FormatString, value,
                            _formatstring  = "{0:" + _formatstring + "}";
                }

            }
        }

        public FieldType fieldType { get; set; }
        private Type _propertyType;
        public Type PropertyType
        {
            get => _propertyType;
            set
            {
                _propertyType = value;

                if (value == typeof(int) || value == typeof(short) || value == typeof(byte) ||
                    value == typeof(sbyte) || value == typeof(uint) || value == typeof(long) ||
                    value == typeof(bool) || value == typeof(Int32) || value == typeof(Int64) ||
                    value == typeof(int?) || value == typeof(short?) || value == typeof(byte?) ||
                    value == typeof(sbyte?) || value == typeof(uint?) || value == typeof(long?) ||
                    value == typeof(bool?) || value == typeof(Int32?) || value == typeof(Int64?)
                    )
                        fieldType = FieldType.ftInt;
                    
                else if (value == typeof(double) || value == typeof(float) || value == typeof(decimal) ||
                         value == typeof(double?) || value == typeof(float?) || value == typeof(decimal?)
                    )
                    fieldType = FieldType.ftFloat;

                else if (value == typeof(string)   //kein nullable string??? || value == typeof(string?)
                    )
                    fieldType = FieldType.ftString;

                else if (value == typeof(DateTime) || value == typeof(DateTime?))
                    fieldType = FieldType.ftDateTime;

                else
                    throw new NotImplementedException($"propertyType={value}");

            }
        }

        public FieldInfo(Type propertyType, string formatstring)
        {
            PropertyType = propertyType;
            Formatstring = formatstring;
        }
    }

    public enum FieldType
    {
        ftString,
        ftInt,
        ftFloat,
        ftDateTime
    }

    


}