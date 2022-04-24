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
using Data;

namespace Game
{
    public class  GamerController : MonoBehaviour
    {
        public Text wordIndicator;

        public Text scoreIndicator;
        public Text letterIndicator;
        
        public InputField Letter_input;
        
        private static string _inputOfPlayer;

       // private HangmanController hangman;
        private string word;
        private char[] revealed;
        private bool completed;

        private int nb_errors;

        [SerializeField] 
        private GameObject YouLost;
        
        [SerializeField] 
        private GameObject YouWon;
        
        [SerializeField] 
        private GameObject Buttons;
        
        [SerializeField] 
        private GameObject btn_exit;
        
        [SerializeField] 
        private GameObject Bonhomme;

        private int ManPhase = 0;
        
        
        [SerializeField] 
        private AudioSource YouLost_Sound;
        
        [SerializeField] 
        private AudioSource YouWon_Sound;

        public FirebaseManager Firebase;
        
        private int score;
        
        
        
        // Start is called before the first frame update
        void Start()
        {
            Firebase = FirebaseManager.Instance;
            
            score = Firebase.xpField;
            //hangman = GameObject.FindGameObjectsWithTag("Player").GetComponent<HangmanController>();
            reset();
            Debug.Log(word);
        }

        public void Update()
        {
            
            
            if(Letter_input.text != null)
                 _inputOfPlayer = Letter_input.text;

            if (nb_errors == 7)
            {
                score -= word.Length * 100;
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
        // Update is called once per frame
        public void OnValidateCLick()
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
                //if (!check(s).ToUpper)
                //hangman.punish();
                /*
                 * if (hangman.isDead)
                 * worldIndicator.text =word;
                 * completed=true;
                 */
            }

        }

        private bool check(char? c)
        {
            bool ret = false;
            int complete = 0; 
            score = Firebase.xpField;
                
            Debug.LogFormat(Firebase.xpField.ToString());
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
                else score -= 100;
                if (revealed[i] != 0)
                    complete++;
               
            }

            if (score != 0)
            {
                if (complete == revealed.Length)
                {
                    this.completed = true;
                    this.score += revealed.Length +100*complete;
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
            //setWorld("A-tester*!é");
        }
        public static string GetWord(string path = "Assets/Pendu/Script/word_bank.txt")
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
        }
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

        public void OnRestartClick()
        {
            for (int i = 1; i < 9; i++)
            {
                Bonhomme.transform.Find("Man" + i).gameObject.SetActive(false);
            }
            Debug.Log(word);
            reset();
        }

        public void OnExitCLick()
        {
            Firebase.xpField = score;
            Firebase.SaveDataButton();
            // DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
           // WriteNewScore(DBreference.Child("users").Child(User.UserId).GetValueAsync(), score);
            PhotonNetwork.LoadLevel(0);
            PhotonNetwork.LeaveRoom();
            
        }

    }
}