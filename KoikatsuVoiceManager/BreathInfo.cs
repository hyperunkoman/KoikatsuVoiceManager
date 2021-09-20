using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KoikatsuVoiceManager
{

    public class BreathInfo : CharaVoiceInfoBase
    {
        /*
         * Breath 1行データ
         * 00: num
         * 01: group
         *  02: Asset path
         *  03: File name
         *  04: word
         *  05: absoluteOverWrite 1:true
         *  06: isWeakPointJudge 1:true
         *  07: faceWeak
         *  08: faceNormal[0]
         *  09: faceNormal[0]
         *  10: faceNormal[0]
         * 10-17: 02-10と同じ
         * 18-25: 02-10と同じ
         * 26-33: 02-10と同じ
         *
         */




        public Int32 Flag { get; private set; }

        public Int32[] Unknowns { get; private set; } = new Int32[17];

        public Int32 Kind { get; private set; }

        public Int32 Unknown2{ get; private set; }

        public BreathInfo(string infoFileName, string voiceArcName, string voiceName, string text, Int32 kind, Int32 level)
        {
            VoiceArcFileName = voiceArcName;
            VoiceName = voiceName;
            Text = text;
            Kind = kind;
            Level = level;
            InfoFileName = infoFileName;
            CharaID = CharaIDFromFileName(infoFileName);
        }

        public static BreathInfo FromStringArray(string filename, int level, string[] data, int offset = 0)
        {
            Int32 kind;
            Int32.TryParse(data[offset + 8], out kind);

            return new BreathInfo(filename,
                data[offset + 0], data[offset + 1], data[offset + 2], kind, level);
        }

        public static BreathInfo FromStringArrayForShort(string filename, Int32 level, string[] data, int offset = 0)
        {
            Int32 kind;
            Int32.TryParse(data[offset + 4], out kind);

            return new BreathInfo(filename,
                data[offset + 0], data[offset + 1], data[offset + 2], kind, level);
        }
    }
}
