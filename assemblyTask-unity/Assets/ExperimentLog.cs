using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Content;

public class ExperimentLog : MonoBehaviour
{
    public static ExperimentLog instance;
    public string filePath;
    public string filePathW;
    int participantNumber = 0;
    StreamWriter writer;
    StreamWriter writerW;
    SceneDirector manager;
    public float time_s = 0;
    float tempTime = 0f;
    int counter = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            //Debug.Log("Destroyed"+SceneManager.GetActiveScene().name);
        }
        else
        {
            instance = this;
        }

        manager = this.gameObject.GetComponent<SceneDirector>();
        var rnd = new System.Random();
        filePath = Application.dataPath + "/Records";
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        if (instance == this) DontDestroyOnLoad(transform.gameObject);

        /* if (SceneManager.GetActiveScene().name == "Tutorial Video")
            SetParticipantNumber(0); */
    }
    // Update is called once per frame
    void Update()
    {
        time_s += Time.deltaTime;

    }

    public void SetParticipantNumber(int pNum)
    {

        participantNumber = pNum;
        string temp = filePath;
        filePath = filePath + "/Participant" + participantNumber.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssf") + ".csv";
        filePathW = temp + "/WideParticipant" + participantNumber.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
        using (writer = File.CreateText(filePath))
        {
            writer.WriteLine("Participant_Number;Shape;Condition;Adaptivity;Trial;Timestamp;Time_in_trial;Category;Action;Errortype;Step;TimeSinceLastEvent");
        }
        using (writerW = File.CreateText(filePathW))
        {
            //Debug.Log("Creating Wide File");
            writerW.WriteLine("");
            writerW.WriteLine("");
        }

    }

    public void AddData(string category = "n/a", string action = "n/a", string step = "n/a", string errorType = "n/a")
    {
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;
        string[] splitSceneName = sceneName.Split('_');

        float miliS = time_s * 1000;
        int seconds = ((int)time_s % 60);
        int minutes = ((int)time_s / 60);
        string timeString = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, miliS);
        float timeSinceLastEvent = miliS - tempTime;

        string newLine = participantNumber.ToString();
        if (splitSceneName.Length == 3)
        {
            newLine += ";" + splitSceneName[0];
            newLine += ";" + splitSceneName[1];
            newLine += ";" + splitSceneName[2];
        }
        else
        {
            newLine += ";" + sceneName;
            newLine += ";" + "n/a";
            newLine += ";" + "n/a";
        }
        newLine += ";" + manager.trialNumber.ToString();
        newLine += ";" + DateTime.Now.ToString("HH:mm.ss");
        newLine += ";" + Mathf.Round(miliS).ToString();
        newLine += ";" + category;
        newLine += ";" + action;
        newLine += ";" + errorType;
        newLine += ";" + step;
        newLine += ";" + Mathf.Round(timeSinceLastEvent).ToString();
        tempTime = miliS;
        using (writer = File.AppendText(filePath))
        {
            writer.WriteLine(newLine);
        }

    }
    public void AddWideData(int trialNumber, int mistakesMade)
    {
//        Debug.Log("Adding Wide Data");
        //Participant_Number;Shape;Condition;Adaptivity;PositionInExp;Trial;TotalTime;MistakesMade";
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;
        string[] splitSceneName = sceneName.Split('_');
        string newLine = "";
        float miliSW = time_s * 1000;
        if (counter == 1)
        {
            newLine = participantNumber.ToString() + ";";
        }

        if (splitSceneName.Length == 3)
        {
            newLine += splitSceneName[0];
            newLine += ";" + splitSceneName[1];
            newLine += ";" + splitSceneName[2];
        }
        else
        {
            newLine += sceneName;
            newLine += ";" + "n/a";
            newLine += ";" + "n/a";
        }
        newLine += ";" + manager.shapeNumber.ToString();
        newLine += ";" + trialNumber.ToString();
        newLine += ";" + Mathf.Round(miliSW).ToString();
        newLine += ";" + mistakesMade.ToString() + ";";

        string[] lines = File.ReadAllLines(filePathW);

        if (counter == 1)
        {
            lines[0] += "Participant_Number;Shape" + counter + ";Condition" + counter + ";Adaptivity" + counter + ";PositionInExp" + counter + ";Trial" + counter + ";TotalTime" + counter + ";MistakesMade" + counter + ";";
            Debug.Log("Added Lines");
        }
        else
        {
            lines[0] += "Shape" + counter + ";Condition" + counter + ";Adaptivity" + counter + ";PositionInExp" + counter + ";Trial" + counter + ";TotalTime" + counter + ";MistakesMade" + counter + ";";
            Debug.Log("Added Lines 2");
        }
        lines[^1] += newLine;
        counter++;
        File.WriteAllLines(filePathW, lines);

    }

}
