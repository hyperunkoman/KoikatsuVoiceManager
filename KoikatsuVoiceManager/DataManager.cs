using SB3Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using UnityPlugin;

namespace KoikatsuVoiceManager
{
    public class DataManager
    {
        public static string BasePath { get; set; }

        public static DataManager Instance { get; } = new DataManager();

        Dictionary<string, VoiceUsage> m_voiceUsages;
        Dictionary<string, string> m_voiceInfoDupCheck;
        List<VoiceInfo> m_VoiceInfos;

        public Dictionary<string, VoiceUsage> VoiceUsages => m_voiceUsages;

        private void LoadVoiceUsageOneArc(string filePath, int chaID)
        {
            //const string cutPath = @"assets/assetbundle/";
            //Int32 cutPathLen = cutPath.Length;

            using (var up = new UnityParser(filePath))
            {
                for (int i = 0; i < up.Cabinet.Bundle.m_Container.Count; i++)
                {
                    var entry = up.Cabinet.Bundle.m_Container[i];
                    var c = entry.Value.asset.asset;
                    if (c.classID() == UnityClassID.AudioClip)
                    {
                        string name = Path.GetFileNameWithoutExtension(entry.Key);
                        string key = VoiceUsage.VoiceKey(name, filePath);
                        if (!m_voiceUsages.ContainsKey(key))
                        {
                            var vu = new VoiceUsage(name, c.pathID, filePath, chaID);
                            m_voiceUsages.Add(key, vu);
                        }
                        else
                        {
                            Debug.WriteLine("duplicated: {0} {1} / {2}", name, filePath, m_voiceUsages[key].ArcPath);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("exclude: {0} {1}", entry.Key, filePath);
                    }
                }
            }
        }

        public void LoadVoiceUsage()
        {
            const string pcmPath = @"abdata\sound\data\pcm";

            m_voiceUsages = new Dictionary<string, VoiceUsage>();
            for (int i = 0; i <= 38; i++)
            {
                string path = Path.Combine(BasePath, pcmPath, string.Format("c{0:D2}", i), "h");
                foreach (var fname in Directory.GetFiles(path, "*.unity3d"))
                {
                    LoadVoiceUsageOneArc(fname, i);
                }
            }
        }



        private void ParseVoiceInfoOneLine(string name, string filePath, string texts, VoiceType vt)
        {
            int offset;
            int unitLen;

            if (vt == VoiceType.Voice)
            {
                offset = 1;
                unitLen = 23;
            }
            else if (vt == VoiceType.Breath)
            {
                offset = 2;
                unitLen = 9;
            }
            else if (vt == VoiceType.ShortBreath)
            {
                offset = 1;
                unitLen = 5;
            }
            else
            {
                return;
            }


            int ln = 0;
            foreach (var l in texts.Split('\n'))
            {
                ln++;
                var line = l.TrimEnd('\r', '\n', ' ');
                var tokens = line.Split('\t');
                if (tokens.Length < (unitLen * 4 + offset))
                {
                    if (tokens.Length > 1)
                    {
                        Debug.WriteLine("invalid line: {0}:{1:D} {2}", name, ln, filePath);
                    }
                    continue;
                }
                for (int i = 0; i < 4; i++)
                {
                    CharaVoiceInfoBase vib;
                    if(vt == VoiceType.Voice)
                    {
                        var vi = VoiceInfo.FromStringArray(name, i, tokens, unitLen * i + offset);
                        vib = vi;
                        m_VoiceInfos.Add(vi);
                    }
                    else if (vt == VoiceType.Breath)
                    {
                        var vi = BreathInfo.FromStringArray(name, i, tokens, unitLen * i + offset);
                        vib = vi;
                    }
                    else if (vt == VoiceType.ShortBreath)
                    {
                        var vi = BreathInfo.FromStringArrayForShort(name, i, tokens, unitLen * i + offset);
                        vib = vi;
                    }
                    else
                    {
                        throw new InvalidOperationException("Internal error");
                    }

                    var voicekey = VoiceUsage.VoiceKey(vib.VoiceName, vib.VoiceArcFileName);
                    if (m_voiceUsages.ContainsKey(voicekey))
                    {
                        m_voiceUsages[voicekey].Ref();
                        string text = vib.Text;
                        if (text.StartsWith("※") && text.Contains("と同じ"))
                        {
                            continue;
                        }
                        if (text.StartsWith("※") || text.StartsWith("☆")) text = text.Substring(1);
                        if (m_voiceUsages[voicekey].Comment == null)
                        {
                            m_voiceUsages[voicekey].Comment = text;
                        }
                        else if (m_voiceUsages[voicekey].Comment != text)
                        {
                            if (m_voiceUsages[voicekey].Comment.Length < 2)
                            {
                                m_voiceUsages[voicekey].Comment = text;
                            }
                            else if (text.Length < 2)
                            {
                                // 無効データ
                            }
                            else if (m_voiceUsages[voicekey].Comment.Length > 8 && text.Length > 8 &&
                                m_voiceUsages[voicekey].Comment.Substring(0, 8).CompareTo(text.Substring(0, 8)) == 0)
                            {
                                // 大体あってることにする
                            }
                            else
                            {
                                Debug.WriteLine("text not matched: {0}:{1:D} {2}", name, ln, filePath);
                                Debug.WriteLine("-" + m_voiceUsages[voicekey].Comment);
                                Debug.WriteLine("+" + vib.Text);
                            }
                        }
                    }
                }
            }
        }

        private void ParseBreathInfoOneLine(string name, string filePath, string texts)
        {
            int ln = 0;
            foreach (var l in texts.Split('\n'))
            {
                ln++;
                var line = l.TrimEnd('\r', '\n', ' ');
                var tokens = line.Split('\t');
                if (tokens.Length < (23 * 4 + 1))
                {
                    if (tokens.Length > 1)
                    {
                        Debug.WriteLine("invalid line: {0}:{1:D} {2}", name, ln, filePath);
                    }
                    continue;
                }
                for (int i = 0; i < 4; i++)
                {
                    var vi = VoiceInfo.FromStringArray(name, i, tokens, 23 * i + 1);
                    m_VoiceInfos.Add(vi);
                    var voicekey = VoiceUsage.VoiceKey(vi.VoiceName, vi.VoiceArcFileName);
                    if (m_voiceUsages.ContainsKey(voicekey))
                    {
                        m_voiceUsages[voicekey].Ref();
                        string text = vi.Text;
                        if (text.StartsWith("※") && text.Contains("と同じ"))
                        {
                            continue;
                        }
                        if (text.StartsWith("※") || text.StartsWith("☆")) text = text.Substring(1);
                        if (m_voiceUsages[voicekey].Comment == null)
                        {
                            m_voiceUsages[voicekey].Comment = text;
                        }
                        else if (m_voiceUsages[voicekey].Comment != text)
                        {
                            if (m_voiceUsages[voicekey].Comment.Length < 2)
                            {
                                m_voiceUsages[voicekey].Comment = text;
                            }
                            else if (text.Length < 2)
                            {
                                // 無効データ
                            }
                            else if (m_voiceUsages[voicekey].Comment.Length > 8 && text.Length > 8 &&
                                m_voiceUsages[voicekey].Comment.Substring(0, 8).CompareTo(text.Substring(0, 8)) == 0)
                            {
                                // 大体あってることにする
                            }
                            else
                            {
                                Debug.WriteLine("text not matched: {0}:{1:D} {2}", name, ln, filePath);
                                Debug.WriteLine("-" + m_voiceUsages[voicekey].Comment);
                                Debug.WriteLine("+" + vi.Text);
                            }
                        }
                    }
                }
            }
        }

        private void LoadVoiceInfoOneArc(string filePath)
        {
            using (var up = new UnityParser(filePath))
            {
                for (int i = 0; i < up.Cabinet.Bundle.m_Container.Count; i++)
                {
                    var entry = up.Cabinet.Bundle.m_Container[i];
                    var c = entry.Value.asset.asset;
                    if (c.classID() == UnityClassID.TextAsset)
                    {
                        string name = Path.GetFileNameWithoutExtension(entry.Key);

                        if (m_voiceInfoDupCheck.ContainsKey(name))
                        {
                            Debug.WriteLine("duplicated: {0} {1} / {2}", name, filePath, m_voiceInfoDupCheck[name]);
                            continue;
                        }
                        VoiceType voiceType = VoiceType.None;
                        if (name.StartsWith("personality_voice_")) voiceType = VoiceType.Voice;
                        else if (name.StartsWith("personality_breath_")) voiceType = VoiceType.Breath;
                        else if (name.StartsWith("personality_shortbreath_")) voiceType = VoiceType.ShortBreath;
                        else
                        {
                            //Debug.WriteLine("exclude(by name): {0} {1}", entry.Key, filePath);
                            continue;
                        }

                        TextAsset ta = up.Cabinet.LoadComponent(c.pathID);
                        ParseVoiceInfoOneLine(name, filePath, ta.m_Script, voiceType);
                        //up.Cabinet.UnloadSubfile(c);
                    }
                    else
                    {
                        //Debug.WriteLine("exclude(by type): {0} {1}", entry.Key, filePath);
                    }
                }
            }
        }

        public void LoadVoiceInfo()
        {
            const string listPath = @"abdata\h\list";
            string dirPath = Path.Combine(BasePath, listPath);

            m_voiceInfoDupCheck = new Dictionary<string, string>();
            m_VoiceInfos = new List<VoiceInfo>();

            var files = Directory.GetFiles(dirPath, "??_??.unity3d").OrderByDescending(n => n);

            foreach (var f in files)
            {
                LoadVoiceInfoOneArc(f);
            }
        }

        public void ReloadAll()
        {
            m_voiceUsages = null;
            m_voiceInfoDupCheck = null;
            m_VoiceInfos = null;
            GC.Collect();
            LoadVoiceUsage();
            LoadVoiceInfo();
        }

        private static Utility.SoundLib m_soundLib;

        public static void PlayAudioClip(string arcfile, Int64 pathid)
        {
            string name = "default";
            if (m_soundLib == null) m_soundLib = new Utility.SoundLib();
            m_soundLib.Stop(name);
            string arcPath = arcfile;// Path.Combine(BasePath, arcfile);
            byte[] audioData;
            using (var up = new UnityParser(arcPath))
            {
                AudioClip ta = up.Cabinet.LoadComponent(pathid);
                audioData = ta.m_AudioData;
                up.Cabinet.UnloadSubfile(ta);
            }
            m_soundLib.Play(name, audioData);
        }
    }
}
