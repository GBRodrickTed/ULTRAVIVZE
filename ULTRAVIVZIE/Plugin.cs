using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

namespace ULTRAVIVZIE
    //this mod sucks
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public Harmony harm;
        public static AudioSource mainSource;
        public static AudioClip mainClip;
        public static GameObject obj;
        public static SubtitledAudioSource subSource = null;
        public static AudioFunny[] audFuns;
        public static List<GameObject> gigglebunch = new List<GameObject>();
        public void Start()
        {
            harm = new Harmony(PluginInfo.GUID);
            harm.PatchAll(typeof(Patchistan));
            Debug.Log("we did it tumblr");
            audFuns = Cereal.UncerealFunnies(Path.Combine(ModAudioDir(), "_aud_config_.funi"));
            AudioJungle.SortFunnies(audFuns);
            AudioJungle.AudifyTheDenseTropicalVegitation(Path.Combine(ModDataDir(), "script.lines"));
        }

        void ClipCallback(AudioClip clip)
        {
            mainClip = clip;
        }
        
        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.U))
            {
                GameObject fun = new GameObject();
                AudioSource funny = fun.AddComponent<AudioSource>();
                funny.clip = AudioJungle.Pick("wontburstyoureardrums").audMonkey.clip;
                funny.Play();
                gigglebunch.Add(fun);
            }

            if (Input.GetKeyUp(KeyCode.I))
            {
                AudioJungle.funnyLineDic = new Dictionary<string, List<FunnyLine>>();
                AudioJungle.periodDic = new Dictionary<string, List<FunnyPeriod>>();
                AudioJungle.subtitleDic = new Dictionary<string, List<SubtitledAudioSource.SubtitleDataLine>>();
                AudioJungle.AudifyTheDenseTropicalVegitation(Path.Combine(ModDataDir(), "script.lines"));
            }

            for (int i = 0; i < gigglebunch.Count; i++)
            {
                if (!gigglebunch[i].GetComponent<AudioSource>().isPlaying)
                {
                    Destroy(gigglebunch[i]);
                    gigglebunch.RemoveAt(i);
                    i--;
                }
            }
            //
            //327.0731 -597 109.7837
            //0 89.601 0
        }
        public static string GetTransformPath(Transform transform)
        {
            List<string> names = new List<string>();
            Transform focusTrans = transform;
            while (focusTrans != null)
            {
                names.Add(focusTrans.name);
                focusTrans = focusTrans.parent;
            }
            string path = "";
            for (int i = names.Count()-1; i >= 0; i--)
            {
                path += names[i]; if (i != 0) path += "/";
            }
            return path;
        }

        public static string ModDir()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        public static string ModDataDir()
        {
            return Path.Combine(ModDir(), "Data");
        }
        public static string ModAudioDir()
        {
            return Path.Combine(ModDataDir(), "Audio");
        }
        
    }
    //"["thing", 2.35f, 3.42f] at last..."(2.4f)
    public static class Patchistan
    {//HarmonyPatch(typeof(StockMapInfo), "Awake")
        [HarmonyPatch(typeof(StockMapInfo), nameof(StockMapInfo.Awake))]
        [HarmonyPrefix]
        public static void Testino(LevelStatsEnabler __instance)
        {
            AudioJungle.RefreshLines();

            AudioJungle.Isgab_BeholdHavingAManicMeltdownAgain = false;

            //foreach has brainworms
            AudioSource[] sources = Resources.FindObjectsOfTypeAll<AudioSource>();
            for (int i = 0; i < sources.Length; i++) 
            {
                if (sources[i] != null && sources[i].clip != null)
                {
                    /*if (sources[i].clip.name.Contains("gab"))
                    {
                        Debug.Log(sources[i].clip.name);
                    }*/

                    //TODO: Have a list of stuff and just check if name's containted in it
                    if (sources[i].clip.name == "mp_intro2"  ||
                        sources[i].clip.name == "mp_outro" ||
                        sources[i].clip.name == "sp_intro" ||
                        sources[i].clip.name == "sp_outro" ||
                        sources[i].clip.name == "gab_Intro1d" ||
                        sources[i].clip.name == "gab_Intro2b" ||
                        sources[i].clip.name == "gab_Behold" ||
                        sources[i].clip.name == "gab_Insignificant2b" ||
                        sources[i].clip.name == "gab_Woes" ||
                        sources[i].clip.name == "gab2nd_6-1tease1" ||
                        sources[i].clip.name == "gab2nd_6-1tease2" ||
                        sources[i].clip.name == "gab2nd_intro1" ||
                        sources[i].clip.name == "gab2nd_intro1b" ||
                        sources[i].clip.name == "gab2nd_intro2" ||
                        sources[i].clip.name == "gab2nd_outro" ||
                        sources[i].clip.name == "gab2nd_Woes"
                        ) 
                    {
                        string id = sources[i].clip.name;

                        if (sources[i].transform.parent == null && sources[i].clip.name == "gab_Behold") continue; // I literally don't idgafing know
                        //Debugging Stuff

                        /*if (sources[i].clip.name == "gab_Behold")
                        {
                            Debug.Log(Plugin.GetTransformPath(sources[i].transform));
                        }*/

                        Debug.Log("[" + id + "]");
                        SubtitledAudioSource tempSAS;
                        Plugin.GetTransformPath(sources[i].transform.parent);
                        if (sources[i].TryGetComponent<SubtitledAudioSource>(out tempSAS))
                        {
                            SubtitledAudioSource.SubtitleDataLine[] tempLines = tempSAS.subtitles.lines;
                            for (int j = 0; j < tempLines.Length; j++)
                            {
                                Debug.Log("(" + tempLines[j].time + ") " + "\"" + tempLines[j].subtitle + "\"");
                            }
                        }

                        if (!AudioJungle.loadedIDs.Contains(id)) continue;

                        List<SubtitledAudioSource.SubtitleDataLine> subtitles = AudioJungle.subtitleDic[id];
                        for ( int j = 0; j < subtitles.Count; j++)
                        {
                            if (subtitles[j] == null)
                            {
                                subtitles.RemoveAt(j);
                                j--;
                            }
                        }
                        GameObject audTreeObj = GameObject.Instantiate(new GameObject(), sources[i].transform);
                        AudioTree audTree = audTreeObj.AddComponent<AudioTree>();
                        audTree.periodID = id;
                        audTree.SetAudioSource(sources[i]);

                        SubtitledAudioSource subsource;
                        if (sources[i].TryGetComponent<SubtitledAudioSource>(out subsource))
                        {
                            subsource.subtitles.lines = subtitles.ToArray();
                        } else
                        {
                            //Debug.Log(id + " lines coundn't be found");
                        }
                        
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MinosPrime), nameof(MinosPrime.Start))]
        [HarmonyPostfix]
        public static void MinosLineParasite(MinosPrime __instance)
        {
            if (__instance.aud != null)
            {
                GameObject audFlowerObj = GameObject.Instantiate(new GameObject(), __instance.aud.transform);
                AudioFlower audFlower = audFlowerObj.AddComponent<AudioFlower>();
                audFlower.SetAudioSource(__instance.aud);
                audFlower.idsOfIntrest = new List<string> { 
                    "mp_crush",
                    "mp_die",
                    "mp_die2",
                    "mp_judgement",
                    "mp_judgement2",
                    "mp_prepare",
                    "mp_prepare2",
                    "mp_thyend",
                    "mp_thyend2",
                    "mp_useless",
                    "mp_weak"
                };
            }
        }

        [HarmonyPatch(typeof(SisyphusPrime), nameof(SisyphusPrime.Start))]
        [HarmonyPostfix]
        public static void SisyphusLineParasite(SisyphusPrime __instance)
        {
            if (__instance.aud != null)
            {
                GameObject audFlowerObj = GameObject.Instantiate(new GameObject(), __instance.aud.transform);
                AudioFlower audFlower = audFlowerObj.AddComponent<AudioFlower>();
                audFlower.SetAudioSource(__instance.aud);
                audFlower.idsOfIntrest = new List<string> {
                    "sp_begone",
                    "sp_begone2",
                    "sp_destroy",
                    "sp_destroy2",
                    "sp_grunt",
                    "sp_keepthemcoming",
                    "sp_nicetry",
                    "sp_nicetry2",
                    "sp_thiswillhurt",
                    "sp_yesthatsit",
                    "sp_youcantescape",
                    "sp_youcantescape2"
                };
            }
        }

        [HarmonyPatch(typeof(GabrielVoice), nameof(GabrielVoice.Start))]
        [HarmonyPostfix]
        public static void GabLineParasite(GabrielVoice __instance)
        {
            if (__instance.aud != null)
            {
                GameObject audFlowerObj = GameObject.Instantiate(new GameObject(), __instance.aud.transform);
                AudioFlower audFlower = audFlowerObj.AddComponent<AudioFlower>();
                audFlower.SetAudioSource(__instance.aud);
                //Why he got so many bars dough?
                audFlower.idsOfIntrest = new List<string> {
                    "gab_BigHurt1",
                    "gab_Coward",
                    "gab_Coward2",
                    "gab_Enough",
                    "gab_Enough2",
                    "gab_Hologram",
                    "gab_HologramFiltered",
                    "gab_Hurt1",
                    "gab_Hurt2",
                    "gab_Hurt3",
                    "gab_Hurt4",
                    "gab_Insignificant",
                    "gab_Insignificant2",
                    "gab_Insignificant2b",
                    "gab_Jesus",
                    "gab_PlayerDeath",
                    "gab_Swing",
                    "gab_Swing2",
                    "gab_Taunt1",
                    "gab_Taunt2",
                    "gab_Taunt3",
                    "gab_Taunt4",
                    "gab_Taunt5",
                    "gab_Taunt6",
                    "gab_Taunt7",
                    "gab_Taunt8",
                    "gab_Taunt9",
                    "gab_Taunt10",
                    "gab_Taunt11",
                    "gab_Taunt12",
                    "gab_Woes",
                    "gab2nd_phaseChange2",
                    "gab2nd_Pingas", //Had no idea there was a pingas line! I'm soying out so hard!!!
                    "gab2nd_TauntAngry1",
                    "gab2nd_TauntAngry2",
                    "gab2nd_TauntAngry3",
                    "gab2nd_TauntAngry4",
                    "gab2nd_TauntAngry5",
                    "gab2nd_TauntAngry6",
                    "gab2nd_TauntAngry7",
                    "gab2nd_TauntEcstatic1",
                    "gab2nd_TauntEcstatic2",
                    "gab2nd_TauntEcstatic3",
                    "gab2nd_TauntEcstatic4",
                    "gab2nd_TauntEcstatic5",
                    "gab2nd_TauntEcstatic6",
                    "gab2nd_TauntEcstatic7",
                    "gab2nd_TauntEcstatic8",
                    "gab2nd_TauntEcstatic9",
                    "gab2nd_TauntEcstatic10"
                };
            }
        }

        public static SubtitledAudioSource.SubtitleDataLine MakeLine(string subtitle, float time)
        {
            SubtitledAudioSource.SubtitleDataLine lineData = new SubtitledAudioSource.SubtitleDataLine();
            lineData.subtitle = subtitle;
            lineData.time = time;
            return lineData;
        }
    }

    
}
