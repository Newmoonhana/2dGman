using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInputModel
{
    [System.Serializable]
    public class inputItem
    {
        public enum TYPE
        {
            BUTTONDOWN,
            BUTTON,
            BUTTONUP
        }
        [SerializeField] public string input;
        [SerializeField] public TYPE type;
        [SerializeField] public bool isInput = false;

        public inputItem(string _input, TYPE _type)
        {
            input = _input;
            type = _type;
        }
    }
    [SerializeField] public List<inputItem> inputList = new List<inputItem>();

    public UserInputModel()
    {
        inputList.Add(new inputItem("Down", inputItem.TYPE.BUTTONDOWN));
        inputList.Add(new inputItem("Jump", inputItem.TYPE.BUTTONDOWN));
        inputList.Add(new inputItem("Jump", inputItem.TYPE.BUTTONUP));
        inputList.Add(new inputItem("Dash", inputItem.TYPE.BUTTON));
    }

    inputItem FindInputItem(string _input, inputItem.TYPE _type)
    {
        foreach (inputItem item in inputList)
        {
            if (item.input == _input)
                if (item.type == _type)
                    return item;
        }

        return null;
    }

    public bool IsButtonInput(string _input, inputItem.TYPE _type)
    {
        inputItem item = FindInputItem(_input, _type);
        if (item != null)
            if (item.isInput)
            {
                return true;
            }
        return false;
    }
}

public class UserInputManager : MonoBehaviour
{
    public static UserInputManager Instance;
    public UserInputModel model;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        foreach (UserInputModel.inputItem item in model.inputList)
        {
            if (Input.GetButtonDown(item.input))
            {
                if (item.type == UserInputModel.inputItem.TYPE.BUTTONDOWN)
                    item.isInput = true;
                else
                    item.isInput = false;
            }
            else if (Input.GetButton(item.input))
            {
                if (item.type == UserInputModel.inputItem.TYPE.BUTTON)
                    item.isInput = true;
                else
                    item.isInput = false;
            }
            else if (Input.GetButtonUp(item.input))
            {
                if (item.type == UserInputModel.inputItem.TYPE.BUTTONUP)
                    item.isInput = true;
                else
                    item.isInput = false;
            }
        }
    }
}
