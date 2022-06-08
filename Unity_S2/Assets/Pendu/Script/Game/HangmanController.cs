using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Game
{
    public class HagmanController : MonoBehaviour
    {
        public GameObject head;
        public GameObject torso;
        public GameObject arms;
        public GameObject legs;

        private int tries;
        private GameObject[] parts;

        private void Start()
        {
            parts = new[] {head, torso, arms, legs};
            tries = parts.Length-1;
        }
    }

}