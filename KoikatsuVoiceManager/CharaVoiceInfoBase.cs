using System;
using System.Text.RegularExpressions;

namespace KoikatsuVoiceManager
{
    public enum VoiceType
    {
        None = 0,
        Voice,
        Breath,
        ShortBreath,
    }

    public abstract class CharaVoiceInfoBase
    {
        public string VoiceName { get; protected set; }

        public string VoiceArcFileName { get; protected set; }

        public Int32 CharaID { get; protected set; }

        public string InfoFileName { get; protected set; }

        public string Text { get; protected set; }

        public Int32 Level { get; protected set; }

        protected static Int32 CharaIDFromFileName(string fileName)
        {
            var m = Regex.Match(fileName, "c[0-9][0-9]");
            if (m.Success)
            {
                return Int32.Parse(m.Value.Substring(1));
            }
            return -1;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}", CharaID, VoiceName, VoiceArcFileName, Text);
        }
    }
}
