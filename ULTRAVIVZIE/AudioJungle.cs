using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
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
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Networking.Match;
using TMPro;

namespace ULTRAVIVZIE
{
    //General Idea:
    // Audio Jungle will house the funny lookup dictionary
    // The Audio Jungle will manage the AudioPlants.
    // will attach audio plants onto the object with the audio I want to modify
    // These AudioPlants will contain funny periods according to the audio clips
    // Idea: constantly check audiosource to check clip. have a period dictionary for clips
    // Idk how subtitles will work
    // Could go with the president prime route but that's last resort.
    // could also patch the things and maybe do something with it
    public static class AudioJungle
    {
        public static AudioSource source = null;
        public static AudioClip main_clip = null;
        private static bool isMuted = false;
        public static Dictionary<string, List<AudioFunny>> funnyDic = new Dictionary<string, List<AudioFunny>>(); // Stores audfuns according to tags
        public static string recentPick;
        // These 2 aren't fully nessisary, just a bit more effecient than generating them every time (nvm)
        public static Dictionary<string, List<FunnyPeriod>> periodDic = new Dictionary<string, List<FunnyPeriod>>(); // Stores funnyperiods according to clip name
        public static Dictionary<string, List<SubtitledAudioSource.SubtitleDataLine>> subtitleDic = new Dictionary<string, List<SubtitledAudioSource.SubtitleDataLine>>(); // Stores subtitles according to clip name

        public static Dictionary<string, List<FunnyLine>> funnyLineDic = new Dictionary<string, List<FunnyLine>>(); // Stores funny lines according to clip name
        public static string[] loadedIDs = new string[0];

        public static bool Isgab_BeholdHavingAManicMeltdownAgain = false; // Cannot be bothers
        public static void SetSource(AudioSource main_source)
        {
            source = main_source;
            main_clip = main_source.clip;
        }
        public static void SortFunnies(AudioFunny[] audlist)
        {
            for (int i = 0; i < audlist.Length; i++)
            {
                for (int j = 0; j < audlist[i].tags.Count; j++)
                {
                    if (!funnyDic.ContainsKey(audlist[i].tags[j]))
                    {
                        funnyDic.Add(audlist[i].tags[j], new List<AudioFunny>());
                        funnyDic[audlist[i].tags[j]].Add(audlist[i]);
                    }
                    else
                    {
                        funnyDic[audlist[i].tags[j]].Add(audlist[i]);
                    }
                }
            }
        }
        public static AudioFunny Pick(string tag)
        {
            if (funnyDic == null) return null;
            List<AudioFunny> audlist;
            if (funnyDic.TryGetValue(tag, out audlist))
            {
                List<AudioFunny> selectList = new List<AudioFunny>(audlist);
                for (int i = 0; i < selectList.Count; i++)
                {
                    if (selectList[i].audMonkey.name == recentPick && selectList.Count > 1)
                    {
                        selectList.RemoveAt(i);
                        i--;
                    }
                }
                AudioFunny rafflewinnder = selectList[UnityEngine.Random.Range(0, selectList.Count)];
                recentPick = rafflewinnder.audMonkey.name;
                return rafflewinnder;
            }
            return null;
        }

        //Refunnies lines. Mainly used for non cutscene uniqueness
        public static void RefreshLinesById(string id)
        {
            if (!loadedIDs.Contains(id)) return;
            List<FunnyLine> funnyLineList;
            if (funnyLineDic.TryGetValue(id, out funnyLineList))
            {
                List<FunnyPeriod> theFunnies = new List<FunnyPeriod>();
                List<SubtitledAudioSource.SubtitleDataLine> theSubbies = new List<SubtitledAudioSource.SubtitleDataLine>();
                for(int i = 0; i < funnyLineList.Count(); i++)
                {
                    funnyLineList[i].RefunnyLines();
                    List<FunnyPeriod> theTempies = funnyLineList[i].GetFunnyPeriods();
                    for (int j = 0; j < theTempies.Count(); j++)
                    {
                        theFunnies.Add(theTempies[j]);
                    }
                    theSubbies.Add(funnyLineList[i].GetSubtitle());
                }
                funnyLineDic[id] = funnyLineList;
                periodDic[id] = theFunnies;
                subtitleDic[id] = theSubbies;
            } else
            {
                Debug.Log("Eroror: " + id + " funny lines were not found.");
            }
        }

        public static void AudifyTheDenseTropicalVegitation(string path) 
        {
            funnyLineDic = Cereal.BraisinRand(path);
            if (funnyLineDic == null)
            {
                Debug.Log("Good heavens! It appears that something went wrong when trying to deserialize the .lines file: " + path);
                return;
            }

            string[] jinglekeysinfrontofbabytypebeat = funnyLineDic.Keys.ToArray(); //iceimjustcold
            loadedIDs = jinglekeysinfrontofbabytypebeat;
            for (int i = 0; i < jinglekeysinfrontofbabytypebeat.Length; i++)
            {
                List<FunnyPeriod> listyFun = new List<FunnyPeriod>();
                List<SubtitledAudioSource.SubtitleDataLine> listySub = new List<SubtitledAudioSource.SubtitleDataLine>();
                for (int j = 0; j < funnyLineDic[jinglekeysinfrontofbabytypebeat[i]].Count(); j++)
                {
                    listySub.Add(funnyLineDic[jinglekeysinfrontofbabytypebeat[i]][j].GetSubtitle());
                    List<FunnyPeriod> misterTwister = funnyLineDic[jinglekeysinfrontofbabytypebeat[i]][j].GetFunnyPeriods();
                    for (int k = 0; k < misterTwister.Count; k++)
                    {
                        listyFun.Add(misterTwister[k]);
                    }
                }
                periodDic.Add(jinglekeysinfrontofbabytypebeat[i], listyFun);
                subtitleDic.Add(jinglekeysinfrontofbabytypebeat[i], listySub);
            }
            Debug.Log("We are quite literally gaming");
        }

        public static void RefreshLines()
        {
            periodDic.Clear();
            subtitleDic.Clear();
            string[] jinglekeysinfrontofbabytypebeat = funnyLineDic.Keys.ToArray();
            for (int i = 0; i < jinglekeysinfrontofbabytypebeat.Length; i++)
            {
                List<FunnyPeriod> listyFun = new List<FunnyPeriod>();
                List<SubtitledAudioSource.SubtitleDataLine> listySub = new List<SubtitledAudioSource.SubtitleDataLine>();
                for (int j = 0; j < funnyLineDic[jinglekeysinfrontofbabytypebeat[i]].Count(); j++)
                {
                    funnyLineDic[jinglekeysinfrontofbabytypebeat[i]][j].RefunnyLines();
                    List<FunnyPeriod> misterTwister = funnyLineDic[jinglekeysinfrontofbabytypebeat[i]][j].GetFunnyPeriods();
                    listySub.Add(funnyLineDic[jinglekeysinfrontofbabytypebeat[i]][j].GetSubtitle());
                    for (int k = 0; k < misterTwister.Count; k++)
                    {
                        listyFun.Add(misterTwister[k]);
                    }
                }
                periodDic.Add(jinglekeysinfrontofbabytypebeat[i], listyFun);
                subtitleDic.Add(jinglekeysinfrontofbabytypebeat[i], listySub);
            }
        }

        //I've been jokergiffing so hard over the weird cloning thing the intro do
        //My system for replacing voice lines is to attach a gameobject to the voiceline object
        //This enables me to have a mimic audiosource with is useful for things like gabe whos audio moves around
        //The issue is that (at least for intros) when loading from a checkpoint instead of using the existing, fully working "BossStuff" gameobject directly, it clones it.
        //For reasons beyond my comprehension, when it clones, fields with my custom class info are "refreshed" so to speak.
        //because of this, I have this less than ideal system where the AudioTrees have to ask the AudioJungle for their FunnyPeriods using voice clip names as id
        //This may be a blessing in disguise as it may be useful if I were to make a customizable
        public static string AddPeriod(string id, string tag, float start, float end, int stretchOptions = 0)
        {
            FunnyPeriod period = new FunnyPeriod();
            period.mutePeriod = new MutePeriod(start, end);
            period.audfun = AudioJungle.Pick(tag);
            period.tag = tag;

            if (period.audfun.audMonkey.clip != null && stretchOptions > 0)
            {
                float hypotheticalFactor = period.audfun.audMonkey.clip.length / (end - start);
                period.squishMode = stretchOptions;
            }

            if (periodDic.ContainsKey(id))
            {
                periodDic[id].Add(period);
            } else
            {
                periodDic.Add(id, new List<FunnyPeriod>());
                periodDic[id].Add(period);
            }

            if (period.audfun == null) return "    ";
            return period.audfun.text;
        }

        public static FunnyPeriod[] GetPeriods(string id)
        {
            List<FunnyPeriod> thing = null;
            if (periodDic.TryGetValue(id, out thing))
            {
                return thing.ToArray();
            }
            return null;
        }

        public static void ResetPeriods()
        {
            periodDic.Clear();
            subtitleDic.Clear();
        }
    }
    // FunnyLine expected format:
    //"["thing", 2.35f, 3.42f,2] at last..."(2.4f)
    // period.audfun.text + " at last..."
    // Can use line fragments to stitch the audfun text and lines together
    // Makes for easier and cheaper refreshing than leading brands

    // if it contains a fun frag, it will use that instead of line frag
    public class FunnyLineFragment
    {
        public FunnyPeriod funFrag;
        public string lineFrag;
        public bool silent = false; // if we want just funny periods with no text
    }
    public class FunnyLine
    {
        List<FunnyLineFragment> lineFrags = new List<FunnyLineFragment>();
        float time = 0f;
        public string GetLineText()
        {
            string line = "";
            int fragometer = 0;
            if (lineFrags == null) return line;
            for (int i = 0; i < lineFrags.Count; i++)
            {
                if (lineFrags[i].silent == true)
                {
                    fragometer++;
                    continue;
                }
                if (lineFrags[i].funFrag != null) // Takes priority over line frag
                {
                    if (lineFrags[i].funFrag.audfun != null)
                    {
                        line += lineFrags[i].funFrag.audfun.text;
                    }
                } else if (lineFrags[i].lineFrag != null)
                {
                    line += lineFrags[i].lineFrag;
                } else
                {
                    fragometer++;
                }
            }
            if (fragometer == lineFrags.Count)
            {
                return null;
            }
            return line;
        }
        public SubtitledAudioSource.SubtitleDataLine GetSubtitle()
        {
            SubtitledAudioSource.SubtitleDataLine subtitle = new SubtitledAudioSource.SubtitleDataLine();
            subtitle.subtitle = GetLineText();
            if (subtitle.subtitle == null)
            {
                return null;
            }
            subtitle.time = time;
            return subtitle;
        }
        public List<FunnyPeriod> GetFunnyPeriods()
        {
            List<FunnyPeriod> periods = new List<FunnyPeriod>();
            for (int i = 0; i < lineFrags.Count; i++)
            {
                if (lineFrags[i].funFrag != null)
                {
                    periods.Add(lineFrags[i].funFrag);
                }
            }
            return periods;
        }

        public void RefunnyLines()
        {
            for (int i = 0; i < lineFrags.Count; i++)
            {
                if (lineFrags[i].funFrag != null)
                {
                    lineFrags[i].funFrag.Refunny();
                }
            }
        }
        //(2.4f)"Hello. I'm menos prime. I am soooo gay lololol, also ["thing", 2.35f, 3.42f,2] at last..."
        // This should give us 3 line frags

        // very ghetto but it's not as complicated as it looks
        public bool ParseFunnyLine(string funnyLine)
        {
            int cursor = 0;

            int parentheses = 0;
            int brackets = 0;
            int quotations = 0;

            string readFeed = "";
            string beanFeed = "";

            List<FunnyLineFragment> temp_frags = new List<FunnyLineFragment>();

            while (cursor < funnyLine.Length)
            {
                //Debug.Log(cursor + " vs " + funnyLine.Length);
                if (funnyLine[cursor] == '\"' && brackets != 1)
                {
                    quotations = (quotations + 1) % 2;
                    if (quotations == 0)
                    {
                        FunnyLineFragment frag = new FunnyLineFragment();
                        frag.lineFrag = readFeed;
                        temp_frags.Add(frag);
                        readFeed = "";
                        cursor++;
                        continue;
                    }
                    cursor++;
                    continue;
                }

                if (funnyLine[cursor] == '[')
                {
                    // Anything we've read will be turned into a line fragment
                    brackets++;
                    if (quotations == 1)
                    {
                        FunnyLineFragment frag = new FunnyLineFragment();
                        frag.lineFrag = readFeed;
                        temp_frags.Add(frag);
                    }
                    readFeed = "";
                    cursor++;
                    continue;
                }

                if (quotations == 1)
                {
                    if (brackets == 0)
                    {
                        readFeed += funnyLine[cursor];
                    }
                }

                if (brackets == 1)
                {
                    if (funnyLine[cursor] == ']')
                    {
                        // Anything we've read will be turned into a funny fragment
                        brackets--;
                        FunnyLineFragment frag = new FunnyLineFragment();
                        FunnyPeriod funnyfrag = new FunnyPeriod();
                        funnyfrag.mutePeriod = new MutePeriod(0, 0);
                        string[] goodies = readFeed.Split(',');
                        // 0 - tag
                        // 1 - start
                        // 2 - end
                        // 3? - stretch (if specified) 

                        if (goodies.Length < 3)
                        {
                            Debug.Log("Not enough values to properly parse funny period: " + readFeed);
                        }
                        else if (goodies.Length < 4)
                        {
                            funnyfrag.squishMode = 0;
                        }

                        switch (goodies.Length)
                        {
                            // Sins must be committed for the sake of progress.
                            case 4:
                                //Debug.Log("case 4 in progress");
                                if (!int.TryParse(goodies[3], out funnyfrag.squishMode))
                                {
                                    Debug.Log("Oh nose! Something went wrong when trying to parse squish mode: " + goodies[3]);
                                }
                                //Debug.Log("case 4 complete");
                                goto case 3;
                            case 3:
                                //Debug.Log("case 3 in progress");
                                if (!float.TryParse(goodies[2], out funnyfrag.mutePeriod.end))
                                {
                                    Debug.Log("Oh nose! Something went wrong when trying to parse period end time: " + goodies[2]);
                                }
                                //Debug.Log("case 3 complete");
                                goto case 2;
                            case 2:
                                //Debug.Log("case 2 in progress");
                                if (!float.TryParse(goodies[1], out funnyfrag.mutePeriod.start))
                                {
                                    Debug.Log("Oh nose! Something went wrong when trying to parse period start time: " + goodies[1]);
                                }
                                //Debug.Log("case 2 complete");
                                goto case 1;
                            case 1:
                                //Debug.Log("case 1 in progress");
                                if (goodies[0].Contains('\"')) // This is a mousekatool that'll help us later (or not idk)
                                {
                                    string[] taggies = goodies[0].Split('\"');
                                    funnyfrag.tag = taggies[1];
                                    /*for (int i = 0; i < taggies.Length; i++)
                                    {
                                        if (taggies[i].Length > 1)
                                        {
                                            funnyfrag.tag = taggies[i];
                                            break;
                                        }
                                    }*/
                                    //Debug.Log(funnyfrag.tag);
                                }
                                else
                                {
                                    funnyfrag.tag = goodies[0];
                                }
                                funnyfrag.audfun = AudioJungle.Pick(funnyfrag.tag);

                                //Debug.Log("case 1 complete");
                                break;
                            default:
                                Debug.Log("Oh nose! Something went wrong when trying to parse: " + readFeed);
                                break;
                        }
                        frag.funFrag = funnyfrag;
                        if (quotations == 0)
                        {
                            frag.silent = true;
                        }
                        temp_frags.Add(frag);
                        readFeed = "";
                        cursor++;
                        continue;
                    }
                    readFeed += funnyLine[cursor];
                }

                if (funnyLine[cursor] == '(')
                {
                    parentheses++;
                    cursor++;
                    continue;
                }

                if (parentheses == 1)
                {
                    if (funnyLine[cursor] == ')')
                    {
                        parentheses--;
                        // parse readfeed for subtitle time
                        if (!float.TryParse(readFeed, out time))
                        {
                            Debug.Log("Oh nose! Subtitle start time couldn't be parsed: " + readFeed);
                        }
                        readFeed = "";
                        cursor++;
                        continue;
                    }
                    if (quotations == 0)
                    {
                        readFeed += funnyLine[cursor];
                    }
                    cursor++;
                    continue;
                }

                cursor++;
            }
            if (temp_frags.Count > 0)
            {
                lineFrags = temp_frags;
                return true;
            }
            return false;
        }

        public static FunnyLine CreateFunnyLine(string funnyLine)
        {
            FunnyLine funyly = new FunnyLine();

            if (funyly.ParseFunnyLine(funnyLine))
            {
                return funyly;
            }
            return null;
        }
    }

    public class AudioTree : MonoBehaviour
    {
        public AudioSource mainAud;
        public AudioSource funnyAud;
        private List<FunnyPeriod> periods;
        private bool isMainAudMuted = false;
        private FunnyPeriod currentPeriod = null;
        public string hahafunnyunitylolhahahehe;
        public string periodID = "";
        public void Awake()
        {
            if (funnyAud == null)
            {
                funnyAud = gameObject.AddComponent<AudioSource>();
            }
            //Debug.Log("luigi scream");
        }
        // TODO: There is an error here with the mute stuff. probably in audioflower two idk or really even c ath is point
        // I suspect it's inside the currentPeriod != period if statement cause it only pops up once
        public void Update()
        {
            if (periods == null)
            {
                AudioJungle.periodDic.TryGetValue(periodID, out periods);
            }

            if (mainAud != null && mainAud.isPlaying && periods != null)
            {
                //Debug.Log("shie ntd ipyu l;pve my i temy pre rpmnny pl;p[ary l; io pmnyl love my beion oand muy maoma ioin somry");
                funnyAud.pitch = mainAud.pitch;
                foreach (FunnyPeriod period in periods) // TODO: Cheaper maybe
                {
                    isMainAudMuted = period.mutePeriod.WithinRange(mainAud.time);
                    if (isMainAudMuted)
                    {
                        if (currentPeriod != period)
                        {
                            currentPeriod = period;
                            //Debug.Log("switched to -> " + currentPeriod.audfun.audMonkey.name);
                            if (period.audfun.audMonkey.clip != null)
                            {
                                funnyAud.clip = period.audfun.audMonkey.clip;
                            }
                            funnyAud.time = 0;

                            //Not mad, not seething, not babyraging or anything like that. Just can't be bothered.
                            if (periodID == "gab_Behold")
                            {
                                if (AudioJungle.Isgab_BeholdHavingAManicMeltdownAgain || (SceneHelper.CurrentScene != "Level 3-2"))
                                {
                                    continue;
                                }
                                else
                                {
                                    AudioJungle.Isgab_BeholdHavingAManicMeltdownAgain = true;
                                }
                            }
                            funnyAud.Play();
                        }

                        break;
                    }
                }
                
                if (isMainAudMuted)
                {
                    mainAud.volume = 0;
                    funnyAud.volume = 1.5f * AudioMixerController.instance.sfxVolume;
                    float stretchFactor = 1f;
                    switch (currentPeriod.squishMode) // not the cheapest but whatever
                    {
                        case 1: // Stretch to fit
                            stretchFactor = currentPeriod.audfun.audMonkey.clip.length / (currentPeriod.mutePeriod.end - currentPeriod.mutePeriod.start);
                            funnyAud.time = Mathf.Min(currentPeriod.audfun.audMonkey.clip.length, Mathf.Max(0, (mainAud.time - currentPeriod.mutePeriod.start) * stretchFactor));
                            break;
                        case 2: // Won't slow donw / Can only speed up
                            stretchFactor = Mathf.Max(1, currentPeriod.audfun.audMonkey.clip.length / (currentPeriod.mutePeriod.end - currentPeriod.mutePeriod.start));
                            funnyAud.time = Mathf.Min(currentPeriod.audfun.audMonkey.clip.length, Mathf.Max(0,(mainAud.time - currentPeriod.mutePeriod.start) * stretchFactor));
                            break;
                        default:
                            break;
                    }
                    //funnyAud.pitch = stretchFactor * mainAud.pitch;
                    /*if (currentPeriod != null && currentPeriod.stretchFactor != 1f)
                    {
                        funnyAud.time = Mathf.Max((mainAud.time - currentPeriod.mutePeriod.start) * currentPeriod.stretchFactor, -0.01f);
                        //funnyAud.pitch = currentPeriod.stretchFactor * mainAud.pitch;
                    }*/
                }
                else
                {
                    mainAud.volume = 1.0f;
                    funnyAud.volume = 0f;
                }
                if (GameStateManager.Instance.PlayerInputLocked)
                {
                    mainAud.volume = 0;
                    funnyAud.volume = 0;
                }
            }
        }

        public void SlowUpdate()
        {

        }
        public void PeriodSwitchCheck()
        {

        }
        //Unsused because soyence
        public string AddPeriod(string tag, float start, float end, int stretchOptions = 0)
        {
            FunnyPeriod period = new FunnyPeriod();
            period.mutePeriod = new MutePeriod(start, end);
            period.audfun = AudioJungle.Pick(tag);
            period.tag = tag;

            if (period.audfun.audMonkey.clip != null && stretchOptions > 0)
            {
                float hypotheticalFactor = period.audfun.audMonkey.clip.length / (end - start);
                period.squishMode = stretchOptions;
            }

            periods.Add(period);

            if (period.audfun == null) return "";
            return period.audfun.text;
        }

        public void SetAudioSource(AudioSource audSource)
        {
            mainAud = audSource;
        }
    }

    // basically the same as an audio tree but for enimies not cutscenes
    public class AudioFlower : MonoBehaviour
    {
        public AudioSource mainAud;
        public AudioSource funnyAud;
        private List<FunnyPeriod> periods;
        private bool isMainAudMuted = false;
        private FunnyPeriod currentPeriod = null;
        public string periodID = "";
        public List<string> idsOfIntrest = new List<string>();
        public string currentId = "";
        public void Awake()
        {
            if (funnyAud == null)
            {
                funnyAud = gameObject.AddComponent<AudioSource>();
            }
        }
        public void Update()
        {
            if (mainAud != null)
            {
                if (mainAud.clip != null && mainAud.clip.name != periodID)
                {
                    //Debug.Log("Doh mdoh udodoh mmdododoh");
                    periodID = mainAud.clip.name;
                    bool gaming = false;
                    for (int i = 0; i < idsOfIntrest.Count; i++)
                    {
                        if (periodID == idsOfIntrest[i] && AudioJungle.loadedIDs.Contains(periodID))
                        {
                            AudioJungle.RefreshLinesById(periodID);
                            gaming = AudioJungle.periodDic.TryGetValue(periodID, out periods);
                            //Debug.Log(periodID + " also " + (periods != null));
                            List<SubtitledAudioSource.SubtitleDataLine> gamersubs = new List<SubtitledAudioSource.SubtitleDataLine>();
                            if (AudioJungle.subtitleDic.TryGetValue(periodID, out gamersubs))
                            {
                                // For the time being, makes the assumption that there is and will always be one subtitle
                                SubtitledAudioSource.SubtitleDataLine theRealSubShady = null;
                                for (int j = 0; j < gamersubs.Count(); j++)
                                {
                                    if (gamersubs[j] != null)
                                    {
                                        theRealSubShady = gamersubs[j];
                                        break;
                                    }
                                }
                                if (theRealSubShady != null)
                                {
                                    SubtitleController.instance.previousSubtitle.GetComponentInChildren<TMP_Text>().text = theRealSubShady.subtitle;
                                    SubtitleController.instance.previousSubtitle.Fit();
                                }
                            }
                            
                            //SubtitleController.instance.previousSubtitle.GetComponentInChildren<TMP_Text>().SetText("bruh");
                            break;
                        }
                    }
                    if (!gaming)
                    {
                        periods = new List<FunnyPeriod>();
                    }
                    /*for (int i = 0; i < periods.Count; i++)
                    {
                        Debug.Log(periodID + ": " + periods[i].audfun.text);
                    }*/
                }

                if (mainAud.isPlaying && periods != null)
                {
                    //Debug.Log("shie ntd ipyu l;pve my i temy pre rpmnny pl;p[ary l; io pmnyl love my beion oand muy maoma ioin somry");
                    funnyAud.pitch = mainAud.pitch;
                    foreach (FunnyPeriod period in periods) // TODO: Cheaper maybe
                    {
                        isMainAudMuted = period.mutePeriod.WithinRange(mainAud.time);
                        if (isMainAudMuted)
                        {
                            if (currentPeriod != period)
                            {
                                currentPeriod = period;
                                //Debug.Log("switched to -> " + currentPeriod.audfun.audMonkey.name);
                                if (period.audfun.audMonkey.clip != null)
                                {
                                    funnyAud.clip = period.audfun.audMonkey.clip;
                                }
                                funnyAud.time = 0;
                                funnyAud.Play();
                                //Debug.Log(funnyAud.clip);
                            }
                            break;
                        }
                    }

                    if (isMainAudMuted && currentPeriod != null)
                    {
                        mainAud.volume = 0;
                        funnyAud.volume = 1.5f * AudioMixerController.instance.sfxVolume;
                        float stretchFactor = 1f;
                        switch (currentPeriod.squishMode)
                        {
                            case 1: // Stretch to fit
                                stretchFactor = currentPeriod.audfun.audMonkey.clip.length / (currentPeriod.mutePeriod.end - currentPeriod.mutePeriod.start);
                                funnyAud.time = Mathf.Min(currentPeriod.audfun.audMonkey.clip.length, Mathf.Max(0, (mainAud.time - currentPeriod.mutePeriod.start) * stretchFactor));
                                break;
                            case 2: // Won't slow donw / Can only speed up
                                stretchFactor = Mathf.Max(1, currentPeriod.audfun.audMonkey.clip.length / (currentPeriod.mutePeriod.end - currentPeriod.mutePeriod.start));
                                funnyAud.time = Mathf.Min(currentPeriod.audfun.audMonkey.clip.length, Mathf.Max(0, (mainAud.time - currentPeriod.mutePeriod.start) * stretchFactor));
                                break;
                            default:
                                break;
                        }
                        //funnyAud.pitch = stretchFactor * mainAud.pitch;//
                    }
                    else
                    {
                        mainAud.volume = 1.0f;
                        mainAud.pitch = 1;
                        funnyAud.volume = 0f;
                    }
                    if (GameStateManager.Instance.PlayerInputLocked)
                    {
                        mainAud.volume = 0;
                        funnyAud.volume = 0;
                    }
                } else if (mainAud.isPlaying == false)
                {
                    // unlike the cutscenes, we can access periods multiple times. This is a danger to our democracy because the replacement system kinda sorta depends on the next period being different. when this contract is violating, it just wont play it. As a temp fix, we'll set the currentPeriod to null but we (you and me) should look into redesigning this to be not this
                    currentPeriod = null;
                }
            }
        }

        public void SetAudioSource(AudioSource audSource)
        {
            mainAud = audSource;
            if (funnyAud != null)
            {
                funnyAud.SetCustomCurve(AudioSourceCurveType.CustomRolloff, mainAud.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
            }
        }
    }

    public class MutePeriod
    {
        public float start;
        public float end;
        public MutePeriod(float start, float end)
        {
            this.start = start;
            this.end = end;
        }
        public bool WithinRange(float num)
        {
            return (num >= start && num <= end);
        }
        
    }
    public class FunnyPeriod
    {
        public MutePeriod mutePeriod;
        public AudioFunny audfun;
        public string tag; // Used for repicking AudioFunnies if needed.
        public int squishMode;
        public float TimeToFunnyTime(float time)
        {
            return Mathf.Clamp(0, mutePeriod.end - mutePeriod.start, time - mutePeriod.start);
        }

        public override string ToString()
        {
            return "(" + tag + ", " + mutePeriod.start + ", " + mutePeriod.end + ", " + squishMode + ")";
        }

        public void Refunny()
        {
            if (tag == null) return;
            audfun = AudioJungle.Pick(tag);
        }
    }
    public class AudioFunny
    {
        public AudioMonkey audMonkey = new AudioMonkey();
        public string text;
        public List<string> tags = new List<string>();
    }
    //Should I call this something else? yeah. Am i gonna call this something else? no.
    public class AudioMonkey
    {
        public AudioClip clip { get; private set; }
        public string name { get; private set; }
        public void SetName(string name)
        {
            this.name = name;
            if (this.clip != null)
            {
                this.clip.name = name;
            }
        }
        public void GetAudioClipURL(string url, AudioType type = AudioType.WAV)
        {
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, type);
            request.SendWebRequest().completed += delegate
            {
                try
                {
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        clip = DownloadHandlerAudioClip.GetContent(request);
                        clip.name = name;
                        //Debug.Log("Audio Clip Loaded: " + name);
                    }
                    else
                    {
                        Debug.Log("Error when trying to load clip: " + url);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                finally
                {
                    request.Dispose();
                }

            };
        }
    }
}
