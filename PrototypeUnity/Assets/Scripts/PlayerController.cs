using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    public Vector2 InputMove { get; protected set; }
    public string InputSpellCode { get; protected set; }
    public Action InputCast { get; set; }
    public Action<int> InputScrollGroups { get; set; }


    private void Start()
    {
        InputMove = Vector2.zero;
        InputSpellCode = "";
    }
    
}
