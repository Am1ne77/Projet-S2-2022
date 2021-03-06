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
    public class GamerController : MonoBehaviour
    {
        public Text wordIndicator;

        public Text scoreIndicator;
        public Text letterIndicator;

        public InputField Letter_input;
        
        public Slider slider;
        
        public GameObject OptionsUI;
        public GameObject BTNChangeGame;
        public GameObject Setting;

        private static string _inputOfPlayer;

        // private HangmanController hangman;
        private string word;
        private char[] revealed;
        private bool completed;

        private int nb_errors;

        [SerializeField] private GameObject YouLost;

        [SerializeField] private GameObject YouWon;

        [SerializeField] private GameObject Buttons;
        
        [SerializeField] private GameObject EndUI;

        [SerializeField] private GameObject btn_exit;

        [SerializeField] private GameObject Bonhomme;

        private int ManPhase = 0;


        [SerializeField] private AudioSource YouLost_Sound;

        [SerializeField] private AudioSource YouWon_Sound;

        public FirebaseManager Firebase;

        private int score;



        // Start is called before the first frame update
        void Start()
        {
            Firebase = FirebaseManager.Instance;

            score = Firebase.xpPendu;
            //hangman = GameObject.FindGameObjectsWithTag("Player").GetComponent<HangmanController>();
            reset();
            Debug.Log(word);
        }

        public void Update()
        {


            if (Letter_input.text != null)
                _inputOfPlayer = Letter_input.text;

            if (nb_errors == 7)
            {

                this.gameObject.SetActive(false);
                Setting.SetActive(false);
                EndUI.SetActive(true);
                YouLost.SetActive(true);
                //Buttons.SetActive(true);
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
                Setting.SetActive(false);
                EndUI.SetActive(true);
                YouWon.SetActive(true);
                //Buttons.SetActive(true);

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
            score = Firebase.xpPendu;

            Debug.LogFormat(Firebase.xpField.ToString());
            for (int i = 0; i < revealed.Length; i++)
            {
                if (c == word[i])
                {
                    ret = true;
                    if (revealed[i] == 0)
                    {

                        score += 100;
                        revealed[i] = (char) c;
                    }
                }

                if (revealed[i] != 0)
                {
                    complete++;
                }

            }

            if (score != 0)
            {
                if (complete == revealed.Length)
                {
                    this.completed = true;
                    this.score += 100 * complete;
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
                score -= 100;
                updateIndicatorScore();
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

                if (c == 0)
                {
                    if (word[i] >= 'A' && word[i] <= 'Z' || word[i] >= 'a' && word[i] <= 'z')
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

            Firebase.xpPendu = score;
            scoreIndicator.text = "Score: " + score;
        }

        private void setWorld(string world)
        {
            world = world.ToUpper();
            this.word = world;
            revealed = new char[world.Length];
            letterIndicator.text = " Letter(s): " + '\n';
            UpdateWorldIndicator();

        }

        public void next()
        {
            word = GetWord();
            setWorld(word);
            //setWorld("A-tester*!??");
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

            Firebase.xpPendu = score;
            Firebase.SaveDataButton("Pendu");
            nb_errors = 0;
            ManPhase = 0;
            completed = false;
            EndUI.SetActive(false);
            YouLost.SetActive(false);
            YouWon.SetActive(false);
            //Buttons.SetActive(false);
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
            Firebase.xpPendu = score;
            Firebase.SaveDataButton("Pendu");
            // DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
            // WriteNewScore(DBreference.Child("users").Child(User.UserId).GetValueAsync(), score);
            PhotonNetwork.LoadLevel(3);

        }
        
        public void SettingScren()
        {
            OptionsUI.SetActive(true); 
            if (PhotonNetwork.IsMasterClient) BTNChangeGame.SetActive(true);
            else BTNChangeGame.SetActive(true);
        }
    
        public void SettingExit()
        {
            OptionsUI.SetActive(false);
            BTNChangeGame.SetActive(false);
        }
        public void Back()
        {
            PhotonNetwork.LoadLevel(3);
        }
        
        public void MainMenu()
        {
            Firebase.SignOutButton();
            PhotonNetwork.DestroyAll();
            SceneManager.LoadScene(1);

        }
        public void SliderControll()
        {
            Firebase.Audio.volume = slider.value;
        }
    }
}