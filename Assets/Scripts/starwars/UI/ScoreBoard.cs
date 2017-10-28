using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace StarWars.UI {
public class ScoreBoard : MonoBehaviour {
    [Serializable]
    public class ScorePanel {
        public Text nameText;
        public Text scoreText;
        public int Score {
            get {
                return _score;
            }
            set {
                _score = value;
                scoreText.text = _score.ToString();
            }
        }
        private int _score = 0;
    }

    public ScorePanel[] _scores;
    public int scoreToPause;

    private int index = 0;
    private readonly Dictionary<string, ScorePanel> scores =
        new Dictionary<string, ScorePanel>();

    protected void Awake() {
        index = 0;
        foreach (var score in _scores) {
            score.nameText.gameObject.SetActive(false);
        }
    }

    public void Add(string name, Color nameColor, Color scoreColor) {
        var score = _scores[index++];
        score.nameText.gameObject.SetActive(true);
        score.nameText.text = name;
        score.nameText.color = nameColor;
        score.scoreText.text = "0";
        score.scoreText.color = scoreColor;

        scores[name] = score;
    }

    public void AddScore(string name, int points) {
        scores[name].Score += points;
        if (scores[name].Score >= scoreToPause) {
            Debug.Break();
            scoreToPause += 5;
        }
    }
}
}
