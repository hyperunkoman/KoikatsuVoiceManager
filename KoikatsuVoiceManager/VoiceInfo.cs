using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KoikatsuVoiceManager
{
    public class VoiceInfo : CharaVoiceInfoBase
    {
        /*
         * 1行データ
         * 00: num(id)
         *  01: priority
         *  02: Asset path
         *  03: File name
         *  04: word(セリフテキスト)
         *  05-19(len=15): isPlayConditions(しゃべるかどうか判断)
         *  20: isOneShor
         *  21: notOverwrite
         *  22: face
         *  23: eyneck
         * 24-: 01-と同じ
         * 
         * 
         * 
         * 
         * 
         * 
         */



        public Int32 Flag { get; private set; }

        public Int32[] Unknowns { get; private set; } = new Int32[17];

        public Int32 Kind { get; private set; }

        public Int32 Unknown2{ get; private set; }

        public VoiceInfo(string infoFileName, Int32 flag, string voiceArcName, string voiceName, string text, Int32[] unknowns, Int32 kind, Int32 level, Int32 unknown2)
        {
            Flag = flag;
            VoiceArcFileName = voiceArcName;
            VoiceName = voiceName;
            Text = text;
            Kind = kind;
            Level = level;
            for (int i = 0; i < Unknowns.Length; i++)
            {
                Unknowns[i] = unknowns[i];
            }
            Unknown2 = unknown2;
            InfoFileName = infoFileName;
            CharaID = CharaIDFromFileName(infoFileName);
        }

        public static VoiceInfo FromStringArray(string filename, int level, string[] data, int offset = 0)
        {
            var unks = new Int32[17];
            for (int i = 0; i < unks.Length; i++)
            {
                unks[i] = 0;
                Int32.TryParse(data[offset + 4 + i], out unks[i]);
            }
            Int32 kind, unknown2;
            Int32.TryParse(data[offset + 21], out kind);
            Int32.TryParse(data[offset + 22], out unknown2);

            return new VoiceInfo(filename,
                Int32.Parse(data[offset + 0]), data[offset + 1], data[offset + 2], data[offset + 3],
                unks, kind, level, unknown2);
        }
    }
}
