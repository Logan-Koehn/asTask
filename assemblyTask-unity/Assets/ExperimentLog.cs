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

public class ExperimentLog : MonoBehaviour
{
    public string filePath;
    GameObject currGlobalRecordsGO;
    bool sceneChanged = false;
    List<string> independentCSVData = new List<string>();
    int participantNumber = 0;
    //public StringBuilder csvData;
    StreamWriter writer;
    float time_s = 0;
    #region Consts to modify
    private const int FlushAfter = 40;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        var rnd = new System.Random();
        filePath = Application.dataPath + "/Records";
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
       
        DontDestroyOnLoad(transform.gameObject);
        //csvData = new StringBuilder();
        SetParticipantNumber(rnd.Next(1000, 9999));
    }
    // Update is called once per frame
    void Update()
    {
        time_s += Time.deltaTime;
    }


    public void SetCurrGlobalRecordsGO(GameObject currObject)
    {
        currGlobalRecordsGO = currObject;
        //sceneChanged = false;
        foreach (var independentData in independentCSVData)
        {
            //csvData.AppendLine(participantNumber + ";" + DateTime.Now.ToString("yyyyMMdd_HHmmss_f") + ";" + time_s + ";" + independentData);
        }
        independentCSVData.Clear();
    }
    public void SetParticipantNumber(int pNum)
    {
        participantNumber = pNum;
        filePath = filePath + "/Participant" + participantNumber.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssf") + ".csv";
        using (writer = File.CreateText(filePath))
        {
            writer.WriteLine("Participant_Number;Scene;Timestamp;Time_s;Category;Action;Step");
        }

    }
    public void AddData(string category = "n/a", string action = "n/a", string step = "n/a")
    {
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;
        /*
         * status (0=n/a; 1=start; 2=end)
         */
        /* if (sceneChanged)
            independentCSVData.Add(category + "," + action);
        else */
        string newLine = participantNumber.ToString();
        newLine += ";" + sceneName;
        newLine += ";" + DateTime.Now.ToString("HHmmss_ff");
        newLine += ";" + time_s;
        newLine += ";" + category;
        newLine += ";" + action;
        newLine += ";" + step;
        Debug.Log(filePath);
        using (writer = File.AppendText(filePath))
        {
            writer.WriteLine(newLine);
        }
       // csvData.AppendLine(newLine);
        //csvData.AppendLine(participantNumber + ";" + sceneName + ";" + DateTime.Now.ToString("HHmmss_ff") + ";" + time_s + ";" + category + ";" + action + ";" + step);
       // if (csvData.Length >= FlushAfter)
        //{
        //    FlushData();
        //}
    }

    /* void FlushData()
    {
        using (var csvWriter = new StreamWriter(filePath, true))
        {
            csvWriter.Write(csvData.ToString());
        }
        csvData.Clear();
    }

    public void EndCSV()
    {
        if (csvData == null)
        {
            return;
        }
        using (var csvWriter = new StreamWriter(filePath, true))
        {
            csvWriter.Write(csvData.ToString());
        }
        csvData = null;
    }

    private void OnDestroy()
    {
        EndCSV();
    } */
}