using System;


namespace KoikatsuVoiceManager
{
    public class VoiceUsage
    {
        public string Name { get; private set; }

        public string ArcPath { get; private set; }

        public Int64 PathID { get; private set; }

        public Int32 ReferenceCount { get; private set; }

        public Int32 CharacterID { get; private set; }

        public string Comment { get; set; }

        public VoiceUsage(string name, Int64 pathID, string arcPath, Int32 chaID = -1)
        {
            Name = name;
            PathID = pathID;
            ArcPath = arcPath;
            CharacterID = chaID;
        }

        /// <summary>
        /// Increment reference counter
        /// </summary>
        public void Ref()
        {
            ReferenceCount++;
        }

        /// <summary>
        /// Decrement reference counter
        /// </summary>
        public void Unref()
        {
            ReferenceCount--;
        }

        public static string VoiceKey(string name, string path)
        {
            string version = System.IO.Path.GetFileNameWithoutExtension(path);
            return name + "_" + version;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}", CharacterID, Name, ArcPath, Comment);
        }
    }
}
