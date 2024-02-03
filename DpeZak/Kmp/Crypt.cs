using System.Text;

namespace DpeZak.Kmp
{
    public static class Crypt
    {
        /*  function EncryptPassw(const InString: AnsiString; Len: integer = 80): AnsiString;
            //Passwort als Hexstring der festen Länge <Len> verschlüsseln.
            //Len muss mindestens 2*Länge von Passw haben und gerade sein. Ansonsten erfolgt Exception.
            var
              S: AnsiString;
            begin
              if TrimA(InString) = '' then
              begin  //Leeres Passwort nicht verschlüsseln
                Result := '';
                Exit;
              end;
              if length(InString) * 2 > Len then
                EError('EncryptPassw: Password (%d) to long (%d)', [length(InString) * 2, Len]);
              S := InString;
              while Length(S) < (Len div 2) do   //08.01.12 bug bei len
                S := S + ' ';
              Result := StrToHexStr(EnCrypt(S));
            end;

          Result := '';
          Key := SysParam.StartKey;
          for I := 1 to Length(InString) do
          begin
            Result := Result + AnsiChar(Byte(InString[I]) xor (Key shr 8));
            Key := (Byte(Result[I]) + Key) * SysParam.MultKey + SysParam.AddKey;
          end;

         */
        public static string Encrypt(string pw, int len)
        {
            string result = "";
            if (string.IsNullOrEmpty(pw))
            {
                return result;
            }
            int len2 = len / 2;
            var pwlen = pw.PadRight(len2);
            byte[] bytes = Encoding.ASCII.GetBytes(pwlen);
            byte[] rbytes = new byte[bytes.Length];
            int StartKey = 983;
            int MultKey = 12689;
            int AddKey = 35897;
            int Key = StartKey;
            for (var i = 0; i < len2; i++)
            {
                rbytes[i] = (byte)(bytes[i] ^ (Key >> 8));
                Key = (rbytes[i] + Key) * MultKey + AddKey;
            }
            result = BitConverter.ToString(rbytes).Replace("-", "");
            return result;
        }
    }
}
