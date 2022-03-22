using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using WebSocketSharp;
using Random = System.Random;

namespace Game
{
    public class GamerController : MonoBehaviourPun , IPunObservable
    {
        public Text wordIndicator;

        public Text scoreIndicator;
        public Text letterIndicator;
        
        public InputField Letter_input;
        
        private static string _inputOfPlayer;
        
        private string word;
        public char[] revealed;
        private int score;
        private bool completed;

        private int nb_errors = 0;

        [SerializeField] 
        private GameObject YouLost;
        
        [SerializeField] 
        private GameObject YouWon;
        
        [SerializeField] 
        private GameObject Buttons;
        
        [SerializeField] 
        private GameObject Bonhomme;

        private int ManPhase = 0;
        
        
        [SerializeField] 
        private AudioSource YouLost_Sound;
        
        [SerializeField] 
        private AudioSource YouWon_Sound;
        
        
        
        
        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                reset();
                Debug.LogError(word);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(wordIndicator.text);
                stream.SendNext(scoreIndicator.text);
                stream.SendNext(letterIndicator.text);
                stream.SendNext(word);
                //stream.SendNext(nb_errors);
            }

            if (stream.IsReading)
            {
                wordIndicator.text = (string) stream.ReceiveNext();
                scoreIndicator.text = (string) stream.ReceiveNext();
                letterIndicator.text = (string) stream.ReceiveNext();
                word = (string) stream.ReceiveNext();
                //nb_errors = (int) stream.ReceiveNext();
            }
        }
        
        [PunRPC]
        public void Update()
        {

            if(Letter_input.text != null)
                 _inputOfPlayer = Letter_input.text;

            if (nb_errors == 7)
            {
                this.gameObject.SetActive(false);
                YouLost.SetActive(true);
                Buttons.SetActive(true);
                for (int i = 0; i < revealed.Length; i++)
                {
                    revealed[i] = word[i];
                }
                UpdateWorldIndicator();
                
                YouLost_Sound.Play();
            }

            if (completed)
            {
                this.gameObject.SetActive(false);
                YouWon.SetActive(true);
                Buttons.SetActive(true);
                
                for (int i = 1; i < 9; i++)
                {
                    Bonhomme.transform.Find("Man" + i).gameObject.SetActive(false);
                }
                Bonhomme.transform.Find("Man8").gameObject.SetActive(true);
                
                YouWon_Sound.Play();
            }
        }


        public void OnButtonClick()
        {
            if (revealed.Length == 0)
            {
                revealed = new char[word.Length];
            }
            for (int i = 0; i < revealed.Length; i++)
            {
                revealed[i] = wordIndicator.text[i * 2];
            }

            OnValidateClick();

            /*if (photonView.IsMine)
            {
                OnValidateClick();
            }
            else
            {
                photonView.RPC("OnValidateClick",RpcTarget.MasterClient);
                Debug.Log(letterIndicator.text);
                Debug.Log(wordIndicator.text);
            }*/
        }
        
        [PunRPC]
        void OnValidateClick()
        {
            if (completed)
            {
                if (Input.anyKeyDown)
                    next();
            }
            char? c = Pendu_main.GetInput().ToString().ToUpper()[0];
            if (c != null && TextUtils.isAlpha((char) c))
            {
                check(c);
            }
        }

        [PunRPC]
        private bool check(char? c)
        {
            bool ret = false;
            int complete = 0;
            int score = 0;
            for (int i = 0; i < revealed.Length; i++)
            { 
                if (c == word[i])
                {
                    ret = true;
                    if (revealed[i] == 0)
                    {
                        revealed[i] = (char) c;
                        score += 100;
                    }
                }
                if (revealed[i] != 0)
                    complete++;
               
            }

            if (score != 0)
            {
                this.score += score;
                if (complete == revealed.Length)
                {
                    this.completed = true;
                    this.score += revealed.Length * 100;
                }
                UpdateWorldIndicator();
                updateIndicatorScore();
            }

            if (!ret && !letterIndicator.text.Contains((char) c))
            {
                letterIndicator.text += " ";
                letterIndicator.text += c;
                nb_errors++;
                ManPhase++;
                Bonhomme.transform.Find("Man" + ManPhase).gameObject.SetActive(true);
                if (ManPhase > 1)
                {
                    Bonhomme.transform.Find("Man" + (ManPhase - 1)).gameObject.SetActive(false);
                }
            }
            return ret;
        }

        [PunRPC]
        private void UpdateWorldIndicator()
        {
            string displayed = "";
            
            
            for (int i = 0; i < word.Length; i++)
            {
                char c = revealed[i];

                if (c == 0 )
                {
                    if (word[i] >= 'A' && word[i]<= 'Z' || word[i] >= 'a' && word[i] <= 'z')
                    {
                        c = '_';
                    }
                    else
                    {
                        c = word[i];
                    }
                }

                displayed += ' ';
                displayed += c;

            }

            wordIndicator.text = displayed;
        }

        [PunRPC]
        private void updateIndicatorScore()
        {
            scoreIndicator.text = "Score: " + score;
        }

        private void setWorld(string world)
        {
            world = world.ToUpper();
            this.word = world;
            revealed = new char[world.Length];
            letterIndicator.text =  " Letter: "+'\n';
            UpdateWorldIndicator();

        }
        public void next()
        {
            word = GetWord();
            setWorld(word);
        }

        public static string GetWord()
        {
            var index = new Random();
            int inter = index.Next(0, bank.Length);
            return bank[inter];
        }
        
        /*public static string GetWord(string path = "Assets/Pendu/Script/word_bank.txt")
        {
            if (!File.Exists(path))
                throw new ArgumentException("Loader: couldn't load word bank at " + path);
            
            try
            {
                var index = new Random().Next(File.ReadLines(path).Count());
                return File.ReadLines(path).Skip(index).Take(1).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }*/
        public void reset()
        {
            nb_errors = 0;
            ManPhase = 0;
            completed = false;
            YouLost.SetActive(false);
            YouWon.SetActive(false);
            Buttons.SetActive(false);
            this.gameObject.SetActive(true);
            updateIndicatorScore();
            next();
        }

        [PunRPC]
        public void OnRestartClick()
        {
            for (int i = 1; i < 9; i++)
            {
                Bonhomme.transform.Find("Man" + i).gameObject.SetActive(false);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                reset();
            }
            Debug.Log(word);
        }

        [PunRPC]
        public void OnExitCLick()
        {
            PhotonNetwork.LoadLevel(0);
            PhotonNetwork.LeaveRoom();
        }

        #region WordBank

        private static string[] bank =
        {
            "a",
            "ability",
            "able",
            "about",
            "above",
            "accept",
            "according",
            "account",
            "across",
            "act",
            "action",
            "activity",
            "actually",
            "add",
            "address",
            "administration",
            "admit",
            "adult",
            "affect",
            "after",
            "again",
            "against",
            "age",
            "agency",
            "agent",
            "ago",
            "agree",
            "agreement",
            "ahead",
            "air",
            "all",
            "allow",
            "almost",
            "alone",
            "along",
            "already",
            "also",
            "although",
            "always",
            "American",
            "among",
            "amount",
            "analysis",
            "and",
            "animal",
            "another",
            "answer",
            "any",
            "anyone",
            "anything",
            "appear",
            "apply",
            "approach",
            "area",
            "argue",
            "arm",
            "around",
            "arrive",
            "art",
            "article",
            "artist",
            "as",
            "ask",
            "assume",
            "at",
            "attack",
            "attention",
            "attorney",
            "audience",
            "author",
            "authority",
            "available",
            "avoid",
            "away",
            "baby",
            "back",
            "bad",
            "bag",
            "ball",
            "bank",
            "bar",
            "base",
            "be",
            "beat",
            "beautiful",
            "because",
            "become",
            "bed",
            "before",
            "begin",
            "behavior",
            "behind",
            "believe",
            "benefit",
            "best",
            "better",
            "between",
            "beyond",
            "big",
            "bill",
            "billion",
            "bit",
            "black",
            "blood",
            "blue",
            "board",
            "body",
            "book",
            "born",
            "both",
            "box",
            "boy",
            "break",
            "bring",
            "brother",
            "budget",
            "build",
            "building",
            "business",
            "but",
            "buy",
            "by",
            "call",
            "camera",
            "campaign",
            "can",
            "cancer",
            "candidate",
            "capital",
            "car",
            "card",
            "care",
            "career",
            "carry",
            "case",
            "catch",
            "cause",
            "cell",
            "center",
            "central",
            "century",
            "certain",
            "certainly",
            "chair",
            "challenge",
            "chance",
            "change",
            "character",
            "charge",
            "check",
            "child",
            "choice",
            "choose",
            "church",
            "citizen",
            "city",
            "civil",
            "claim",
            "class",
            "clear",
            "clearly",
            "close",
            "coach",
            "cold",
            "collection",
            "college",
            "color",
            "come",
            "commercial",
            "common",
            "community",
            "company",
            "compare",
            "computer",
            "concern",
            "condition",
            "conference",
            "Congress",
            "consider",
            "consumer",
            "contain",
            "continue",
            "control",
            "cost",
            "could",
            "country",
            "couple",
            "course",
            "court",
            "cover",
            "create",
            "crime",
            "cultural",
            "culture",
            "cup",
            "current",
            "customer",
            "cut",
            "dark",
            "data",
            "daughter",
            "day",
            "dead",
            "deal",
            "death",
            "debate",
            "decade",
            "decide",
            "decision",
            "deep",
            "defense",
            "degree",
            "Democrat",
            "democratic",
            "describe",
            "design",
            "despite",
            "detail",
            "determine",
            "develop",
            "development",
            "die",
            "difference",
            "different",
            "difficult",
            "dinner",
            "direction",
            "director",
            "discover",
            "discuss",
            "discussion",
            "disease",
            "do",
            "doctor",
            "dog",
            "door",
            "down",
            "draw",
            "dream",
            "drive",
            "drop",
            "drug",
            "during",
            "each",
            "early",
            "east",
            "easy",
            "eat",
            "economic",
            "economy",
            "edge",
            "education",
            "effect",
            "effort",
            "eight",
            "either",
            "election",
            "else",
            "employee",
            "end",
            "energy",
            "enjoy",
            "enough",
            "enter",
            "entire",
            "environment",
            "environmental",
            "especially",
            "establish",
            "even",
            "evening",
            "event",
            "ever",
            "every",
            "everybody",
            "everyone",
            "everything",
            "evidence",
            "exactly",
            "example",
            "executive",
            "exist",
            "expect",
            "experience",
            "expert",
            "explain",
            "eye",
            "face",
            "fact",
            "factor",
            "fail",
            "fall",
            "family",
            "far",
            "fast",
            "father",
            "fear",
            "federal",
            "feel",
            "feeling",
            "few",
            "field",
            "fight",
            "figure",
            "fill",
            "film",
            "final",
            "finally",
            "financial",
            "find",
            "fine",
            "finger",
            "finish",
            "fire",
            "firm",
            "first",
            "fish",
            "five",
            "floor",
            "fly",
            "focus",
            "follow",
            "food",
            "foot",
            "for",
            "force",
            "foreign",
            "forget",
            "form",
            "former",
            "forward",
            "four",
            "free",
            "friend",
            "from",
            "front",
            "full",
            "fund",
            "future",
            "game",
            "garden",
            "gas",
            "general",
            "generation",
            "get",
            "girl",
            "give",
            "glass",
            "go",
            "goal",
            "good",
            "government",
            "great",
            "green",
            "ground",
            "group",
            "grow",
            "growth",
            "guess",
            "gun",
            "guy",
            "hair",
            "half",
            "hand",
            "hang",
            "happen",
            "happy",
            "hard",
            "have",
            "he",
            "head",
            "health",
            "hear",
            "heart",
            "heat",
            "heavy",
            "help",
            "her",
            "here",
            "herself",
            "high",
            "him",
            "himself",
            "his",
            "history",
            "hit",
            "hold",
            "home",
            "hope",
            "hospital",
            "hot",
            "hotel",
            "hour",
            "house",
            "how",
            "however",
            "huge",
            "human",
            "hundred",
            "husband",
            "I",
            "idea",
            "identify",
            "if",
            "image",
            "imagine",
            "impact",
            "important",
            "improve",
            "in",
            "include",
            "including",
            "increase",
            "indeed",
            "indicate",
            "individual",
            "industry",
            "information",
            "inside",
            "instead",
            "institution",
            "interest",
            "interesting",
            "international",
            "interview",
            "into",
            "investment",
            "involve",
            "issue",
            "it",
            "item",
            "its",
            "itself",
            "job",
            "join",
            "just",
            "keep",
            "key",
            "kid",
            "kill",
            "kind",
            "kitchen",
            "know",
            "knowledge",
            "land",
            "language",
            "large",
            "last",
            "late",
            "later",
            "laugh",
            "law",
            "lawyer",
            "lay",
            "lead",
            "leader",
            "learn",
            "least",
            "leave",
            "left",
            "leg",
            "legal",
            "less",
            "let",
            "letter",
            "level",
            "lie",
            "life",
            "light",
            "like",
            "likely",
            "line",
            "list",
            "listen",
            "little",
            "live",
            "local",
            "long",
            "look",
            "lose",
            "loss",
            "lot",
            "love",
            "low",
            "machine",
            "magazine",
            "main",
            "maintain",
            "major",
            "majority",
            "make",
            "man",
            "manage",
            "management",
            "manager",
            "many",
            "market",
            "marriage",
            "material",
            "matter",
            "may",
            "maybe",
            "me",
            "mean",
            "measure",
            "media",
            "medical",
            "meet",
            "meeting",
            "member",
            "memory",
            "mention",
            "message",
            "method",
            "middle",
            "might",
            "military",
            "million",
            "mind",
            "minute",
            "miss",
            "mission",
            "model",
            "modern",
            "moment",
            "money",
            "month",
            "more",
            "morning",
            "most",
            "mother",
            "mouth",
            "move",
            "movement",
            "movie",
            "Mr",
            "Mrs",
            "much",
            "music",
            "must",
            "my",
            "myself",
            "name",
            "nation",
            "national",
            "natural",
            "nature",
            "near",
            "nearly",
            "necessary",
            "need",
            "network",
            "never",
            "new",
            "news",
            "newspaper",
            "next",
            "nice",
            "night",
            "no",
            "none",
            "nor",
            "north",
            "not",
            "note",
            "nothing",
            "notice",
            "now",
            "number",
            "occur",
            "of",
            "off",
            "offer",
            "office",
            "officer",
            "official",
            "often",
            "oh",
            "oil",
            "ok",
            "old",
            "on",
            "once",
            "one",
            "only",
            "onto",
            "open",
            "operation",
            "opportunity",
            "option",
            "or",
            "order",
            "organization",
            "other",
            "others",
            "our",
            "out",
            "outside",
            "over",
            "own",
            "owner",
            "page",
            "pain",
            "painting",
            "paper",
            "parent",
            "part",
            "participant",
            "particular",
            "particularly",
            "partner",
            "party",
            "pass",
            "past",
            "patient",
            "pattern",
            "pay",
            "peace",
            "people",
            "per",
            "perform",
            "performance",
            "perhaps",
            "period",
            "person",
            "personal",
            "phone",
            "physical",
            "pick",
            "picture",
            "piece",
            "place",
            "plan",
            "plant",
            "play",
            "player",
            "PM",
            "point",
            "police",
            "policy",
            "political",
            "politics",
            "poor",
            "popular",
            "population",
            "position",
            "positive",
            "possible",
            "power",
            "practice",
            "prepare",
            "present",
            "president",
            "pressure",
            "pretty",
            "prevent",
            "price",
            "private",
            "probably",
            "problem",
            "process",
            "produce",
            "product",
            "production",
            "professional",
            "professor",
            "program",
            "project",
            "property",
            "protect",
            "prove",
            "provide",
            "public",
            "pull",
            "purpose",
            "push",
            "put",
            "quality",
            "question",
            "quickly",
            "quite",
            "race",
            "radio",
            "raise",
            "range",
            "rate",
            "rather",
            "reach",
            "read",
            "ready",
            "real",
            "reality",
            "realize",
            "really",
            "reason",
            "receive",
            "recent",
            "recently",
            "recognize",
            "record",
            "red",
            "reduce",
            "reflect",
            "region",
            "relate",
            "relationship",
            "religious",
            "remain",
            "remember",
            "remove",
            "report",
            "represent",
            "Republican",
            "require",
            "research",
            "resource",
            "respond",
            "response",
            "responsibility",
            "rest",
            "result",
            "return",
            "reveal",
            "rich",
            "right",
            "rise",
            "risk",
            "road",
            "rock",
            "role",
            "room",
            "rule",
            "run",
            "safe",
            "same",
            "save",
            "say",
            "scene",
            "school",
            "science",
            "scientist",
            "score",
            "sea",
            "season",
            "seat",
            "second",
            "section",
            "security",
            "see",
            "seek",
            "seem",
            "sell",
            "send",
            "senior",
            "sense",
            "series",
            "serious",
            "serve",
            "service",
            "set",
            "seven",
            "several",
            "sex",
            "sexual",
            "shake",
            "share",
            "she",
            "shoot",
            "short",
            "shot",
            "should",
            "shoulder",
            "show",
            "side",
            "sign",
            "significant",
            "similar",
            "simple",
            "simply",
            "since",
            "sing",
            "single",
            "sister",
            "sit",
            "site",
            "situation",
            "six",
            "size",
            "skill",
            "skin",
            "small",
            "smile",
            "so",
            "social",
            "society",
            "soldier",
            "some",
            "somebody",
            "someone",
            "something",
            "sometimes",
            "son",
            "song",
            "soon",
            "sort",
            "sound",
            "source",
            "south",
            "southern",
            "space",
            "speak",
            "special",
            "specific",
            "speech",
            "spend",
            "sport",
            "spring",
            "staff",
            "stage",
            "stand",
            "standard",
            "star",
            "start",
            "state",
            "statement",
            "station",
            "stay",
            "step",
            "still",
            "stock",
            "stop",
            "store",
            "story",
            "strategy",
            "street",
            "strong",
            "structure",
            "student",
            "study",
            "stuff",
            "style",
            "subject",
            "success",
            "successful",
            "such",
            "suddenly",
            "suffer",
            "suggest",
            "summer",
            "support",
            "sure",
            "surface",
            "system",
            "table",
            "take",
            "talk",
            "task",
            "tax",
            "teach",
            "teacher",
            "team",
            "technology",
            "television",
            "tell",
            "ten",
            "tend",
            "term",
            "test",
            "than",
            "thank",
            "that",
            "the",
            "their",
            "them",
            "themselves",
            "then",
            "theory",
            "there",
            "these",
            "they",
            "thing",
            "think",
            "third",
            "this",
            "those",
            "though",
            "thought",
            "thousand",
            "threat",
            "three",
            "through",
            "throughout",
            "throw",
            "thus",
            "time",
            "to",
            "today",
            "together",
            "tonight",
            "too",
            "top",
            "total",
            "tough",
            "toward",
            "town",
            "trade",
            "traditional",
            "training",
            "travel",
            "treat",
            "treatment",
            "tree",
            "trial",
            "trip",
            "trouble",
            "true",
            "truth",
            "try",
            "turn",
            "TV",
            "two",
            "type",
            "under",
            "understand",
            "unit",
            "until",
            "up",
            "upon",
            "us",
            "use",
            "usually",
            "value",
            "various",
            "very",
            "victim",
            "view",
            "violence",
            "visit",
            "voice",
            "vote",
            "wait",
            "walk",
            "wall",
            "want",
            "war",
            "watch",
            "water",
            "way",
            "we",
            "weapon",
            "wear",
            "week",
            "weight",
            "well",
            "west",
            "western",
            "what",
            "whatever",
            "when",
            "where",
            "whether",
            "which",
            "while",
            "white",
            "who",
            "whole",
            "whom",
            "whose",
            "why",
            "wide",
            "wife",
            "will",
            "win",
            "wind",
            "window",
            "wish",
            "with",
            "within",
            "without",
            "woman",
            "wonder",
            "word",
            "work",
            "worker",
            "world",
            "worry",
            "would",
            "write",
            "writer",
            "wrong",
            "yard",
            "yeah",
            "year",
            "yes",
            "yet",
            "you",
            "young",
            "your",
            "yourself"
        };

        #endregion
        
    }
}