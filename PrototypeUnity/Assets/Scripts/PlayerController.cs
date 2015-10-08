using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    private const float AutoCastTime = 0.5f;
    private float auto_cast_timer = 0;

    public Vector2 InputMove { get; protected set; }
    private string input_spell_code;
    public string InputSpellCode
    {
        get
        {
            return input_spell_code;
        }
        protected set
        {
            input_spell_code = value;
            if (value != "") auto_cast_timer = AutoCastTime; // start auto cast
            if (InputSpellCodeChange != null) InputSpellCodeChange();
        }
    }
    public Action InputSpellCodeChange { get; set; }
    public Action InputCast { get; set; }
    public Action<int> InputScrollGroups { get; set; }


    protected void Start()
    {
        InputMove = Vector2.zero;
        InputSpellCode = "";
    }
    protected void Update()
    {
        // Auto Cast
        if (auto_cast_timer > 0)
        {
            auto_cast_timer -= Time.deltaTime;
            if (auto_cast_timer <= 0 && InputCast != null)
            {
                // cast
                InputCast();
                InputSpellCode = "";
            }
        }
    }
}
