using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.IO;
using UnityEngine.SceneManagement;

using UnityEngine.XR.Interaction.Toolkit;
using System.Runtime.Remoting.Activation;
using System.Security;
using System.Security.Policy;

public class invisiBuild : MonoBehaviour
{

    private List<InputDevice> leftHandDevices = new List<InputDevice>();
    private List<InputDevice> rightHandDevices = new List<InputDevice>();
    public bool correctPlacement = false;
    GameObject lastTouchedBar;
    //starts at 2 to when all transforms are listed, it starts at te correct one
    public Transform[] children;
    public GameObject instructions;
    bool crossSpawned = false;
    public GameObject cross;
    public GameObject check;
    GameObject tempCross;
    GameObject tempCheck;
    public bool isGrabbed = false;
    public bool checkSpawned = false;
    public int mistakes = 0;
    public GameObject manager;
    bool tookPos = false;
    Vector3 startPos;
    bool canBeBuilt = true;
    Quaternion originalRotation;
    invisInstructions inst;

    // Start is called before the first frame update
    void Start()
    {
        instructions = GameObject.FindWithTag("SceneInstructions");
        manager = GameObject.FindWithTag("Manager");
        inst = instructions.GetComponent<invisInstructions>();
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("instruction"))
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            lastTouchedBar = other.gameObject;
            if (distance <= 0.03f && CheckProperties(other))
            {
                Debug.Log("Correct Placement");
                correctPlacement = true;
            }

            else correctPlacement = false;

            Debug.Log("Distance between objects: " + distance);
        }
    }
    void OnTriggerExit()
    {
        correctPlacement = false;

    }
    bool CheckProperties(Collider other)
    {
        bool correct = true;
        if (other.GetComponent<propCheck>().barlength != 0)
        {
            if (this.gameObject.GetComponent<propCheck>().barlength != other.GetComponent<propCheck>().barlength)
            {
                correct = false;
            }
        }
        if (other.GetComponent<propCheck>().color != "")
        {
            if (this.gameObject.GetComponent<propCheck>().color != other.GetComponent<propCheck>().color)
            {
                correct = false;
            }
        }
        Debug.Log(correct);
        return correct;
    }

    void Update()
    {
        if (canBeBuilt)
        {
            if (isGrabbed)
            {
                if (!tookPos)
                {
                    startPos = this.transform.position;
                    originalRotation = transform.rotation;
                    tookPos = true;
                }
                // button presses
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHandDevices);
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, rightHandDevices);

                bool rightTrigger = false;
                bool leftTrigger = false;
                if (rightHandDevices[0] != null)
                {
                    if (rightHandDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rightTrigger) && rightTrigger)
                    {
                        // main build button (Right Hand)

                        if (rightTrigger && correctPlacement)
                        {
                            canBeBuilt = false;
                            StartCoroutine("rightBar");
                            build();

                        }
                        if (rightTrigger && !correctPlacement)
                        {
                            canBeBuilt = false;
                            StartCoroutine("WrongBar");
                        }
                    }
                }
                if (leftHandDevices[0] != null)
                {
                    if (leftHandDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out leftTrigger) && leftTrigger)
                    {
                        // main build button (Left Hand)

                        if (leftTrigger && correctPlacement)
                        {
                            canBeBuilt = false;
                            StartCoroutine("rightBar");
                            build();
                        }
                        if (leftTrigger && !correctPlacement)
                        {
                            canBeBuilt = false;
                            StartCoroutine("WrongBar");
                        }
                    }
                }

            }
        }
    }
    void build()
    {

        GameObject newBar = Instantiate(this.gameObject, lastTouchedBar.transform.position, lastTouchedBar.transform.rotation); //this is the bar that is being built
        //newBar.gameObject.GetComponent<Renderer>().material = this.gameObject.GetComponent<Renderer>().material;
        //newBar.gameObject.GetComponent<Renderer>().material = instructions.GetComponent<invisInstructions>().builtMat;
        //this.gameObject.transform.position = lastTouchedBar.transform.position;
        //this.gameObject.transform.rotation = lastTouchedBar.transform.rotation;
        newBar.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newBar.gameObject.GetComponent<MeshCollider>().enabled = false;
        newBar.gameObject.GetComponent<XROffsetGrabInteractable>().enabled = false;
        newBar.gameObject.GetComponent<invisiBuild>().enabled = false;
        manager.GetComponent<ExperimentLog>().AddData(this.gameObject.name, "Correct placement");
        instructions.GetComponent<invisInstructions>().nextStep();
        this.transform.position = startPos;
        this.transform.rotation = originalRotation;
        StartCoroutine("resetCanBeBuilt");
        //this.gameObject.SetActive(false);



        //need to change this to a new instructions script
    }
    IEnumerator WrongBar()
    {

        //instructions.GetComponent<invisInstructions>().dataLog("Bar", "Correct");
        this.gameObject.GetComponent<XROffsetGrabInteractable>().interactionLayerMask = 0;
        inst.toggleHands(false);
        inst.mistakes++;
        inst.SetCurrentStepText();
        inst.builtShape.SetActive(true);
        inst.stepPanel.SetActive(false);
        //instructions.GetComponent<invisInstructions>().dataLog(this.gameObject.name, "incorrect placement", instructions.GetComponent<invisInstructions>().currentStep.ToString());
        manager.GetComponent<ExperimentLog>().AddData(this.gameObject.name, "Incorrect placement", inst.currentStep.ToString());
        if (!crossSpawned)
        {
            tempCross = Instantiate(cross, this.transform.position, Quaternion.identity);
            crossSpawned = true;
        }
        yield return new WaitForSeconds(2f);
        //shouldNotify = true;
        this.gameObject.GetComponent<XROffsetGrabInteractable>().interactionLayerMask = 1;
        Destroy(tempCross);
        crossSpawned = false;
        StartCoroutine("resetCanBeBuilt");

    }
    public void SetIsGrabbed(bool value)
    {
        isGrabbed = value;
    }
    public void grabLog()
    {
        manager.GetComponent<ExperimentLog>().AddData(this.gameObject.name, "grabbed");
    }
    public void dataLog(string category, string action)
    {
        manager.GetComponent<ExperimentLog>().AddData(category, action);
    }

    IEnumerator rightBar()
    {
        this.gameObject.GetComponent<XROffsetGrabInteractable>().interactionLayerMask = 0;
        //instructions.GetComponent<invisInstructions>().dataLog("Bar", "Correct");
        if (!checkSpawned)
        {
            tempCheck = Instantiate(check, this.transform.position, Quaternion.identity);
            checkSpawned = true;
        }
        yield return new WaitForSeconds(2f);
        Destroy(tempCheck);
        checkSpawned = false;
        this.gameObject.GetComponent<XROffsetGrabInteractable>().interactionLayerMask = 1;
    }
    IEnumerator resetCanBeBuilt()
    {
        yield return new WaitForSeconds(1f);
        canBeBuilt = true;
    }
}

