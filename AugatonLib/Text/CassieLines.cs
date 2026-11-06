using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace AugatonLib.Text
{
    public static class CassieLines
    {
        private static readonly char[] Separators = { ' ', '\t' };

        // Renvoie le premier mot que C.A.S.S.I.E ne connait pas, ou null si la phrase est prononcable.
        public static string FindUnknownWord(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            foreach (string word in line.Split(Separators, System.StringSplitOptions.RemoveEmptyEntries))
            {
                if (IsSpoken(word) && !Announcer.IsValid(word))
                    return word;
            }

            return null;
        }

        public static string FindUnknownWord(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                string unknown = FindUnknownWord(line);

                if (unknown is not null)
                    return unknown;
            }

            return null;
        }

        // Ponctuation, chiffres, reglages (pitch_0.8, jam_02) et bruitages (.g4) ne sont pas des mots.
        public static bool IsSpoken(string word)
        {
            return !string.IsNullOrEmpty(word)
                && word[0] != '.'
                && word[0] != ','
                && !char.IsDigit(word[0])
                && !word.Contains("_");
        }
    }
}
