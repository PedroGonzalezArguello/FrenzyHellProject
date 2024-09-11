using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] public GameObject prefab;

    public static PopUpManager _current;
    void Awake()
    {
        _current = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
         {
             PopUp(Vector3.one, Random.Range(0,101).ToString(), Color.red);
        }
    }

    public void PopUp(Vector3 position, string text, Color color)
    {
        var popup = Instantiate(prefab, position, Quaternion.identity);
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        temp.text = text;
        temp.faceColor = color;

        Destroy(popup, 1f);
    }
}