using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAVIVZIE
{
    //[audio.wav]
    //text = "text"
    //tags = {"tag1", "tag2", "tag3", etc...}

    //[mp_intro2]
    //"Ahh..."(0f)
    //"["thing", 2.35f, 3.42f] at last..."(2.4f)

    //if you wanted to mute everything
    //[mp_intro2]
    //[mute, 0f, 45f]
    //"Ahh..."(0f)
    //"["thing", 2.35f, 3.42f] at last..."(2.4f)

    public static class Cereal
    {
        //not really in the buisness of cerealizing so I'm not gonna bother.
        //this is really crusty and really lazy. Don't do this
        public static AudioFunny[] UncerealFunnies(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("Can't deserialize value because \"" + path + "\" does not exist.");
                return null;
            }
            if (!string.Equals(".funi", Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Can't deserialize value because \"" + path + "\" is not a funi file.");
                return null;
            }
            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            sr.Close();
            List<int> indices = new List<int>();

            List<string> lines = new List<string>();
            lines = file.Split('\n').ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IndexOf("[") == 0)
                {
                    indices.Add(i);
                }
            }
            List<AudioFunny> audFuns = new List<AudioFunny>();
            for (int i = 0; i < indices.Count; i++)
            {
                AudioFunny audFun = new AudioFunny();
                string name = lines[indices[i]].Split('[', ']')[1];
                AudioType audType = AudioType.WAV;
                if (name.Contains(".wav"))
                {
                    audType = AudioType.WAV;
                }
                else if (name.Contains(".mp3"))
                {
                    audType = AudioType.MPEG;
                }
                audFun.audMonkey.SetName(name);
                audFun.audMonkey.GetAudioClipURL(Path.Combine(Plugin.ModAudioDir(), name), audType);
                for (int j = indices[i] + 1; j < lines.Count; j++)
                {
                    if (lines[j].IndexOf("[") == 0) break;
                    if (lines[j].IndexOf("text") == 0)
                    {
                        audFun.text = lines[j].Split('\"')[1];
                    }
                    if (lines[j].IndexOf("tags") == 0)
                    {
                        string[] krustyTags = lines[j].Split('\"');
                        List<string> unkrustedTags = new List<string>();
                        for (int k = 0; k < krustyTags.Length; k++)
                        {
                            if (k % 2 == 1)
                            {
                                unkrustedTags.Add(krustyTags[k]);
                            }
                        }
                        audFun.tags = unkrustedTags;
                    }
                }
                audFuns.Add(audFun);
            }
            return audFuns.ToArray();
        }

        public static Dictionary<string, List<FunnyLine>> BraisinRand(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("Cannot find file: " + path);
                return null;
            }

            if (!string.Equals(".lines", Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("I aint deserializeing your shit cause it aint a lines file: " + path);
                return null;
            }

            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            sr.Close();

            List<int> indices = new List<int>();
            List<string> lines = new List<string>();
            lines = file.Split('\n').ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IndexOf("[[") == 0)
                {
                    indices.Add(i);
                }
            }
            Dictionary<string, List<FunnyLine>> funnyLineDic = new Dictionary<string, List<FunnyLine>>();
            for (int i = 0; i < indices.Count; i++)
            {
                string id = lines[indices[i]].Split('[', ']')[2]; // smert
                if (id == null) continue;
                List<FunnyLine> glangly = null;
                if (funnyLineDic.TryGetValue(id, out glangly))
                {
                    Debug.Log("Warning: duplicate lines for " + id + " found in file: " + path);
                    continue;
                }
                funnyLineDic.Add(id, new List<FunnyLine>());

                for (int j = indices[i] + 1; j < lines.Count; j++)
                {
                    if (lines[j].IndexOf("[[") == 0) break;
                    FunnyLine humourousQuip = FunnyLine.CreateFunnyLine(lines[j]);
                    if (humourousQuip != null)
                    {
                        //Debug.Log(id);
                        funnyLineDic[id].Add(humourousQuip);
                    }
                }
            }
            return funnyLineDic;
        }
    }
}
