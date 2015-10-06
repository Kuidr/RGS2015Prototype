using UnityEngine;
using System.Collections;

public class BallCam : MonoBehaviour
{
    public Transform ball;
    public float map_height = 30f;
    public float cam_height = 10f;
    private float min_y, max_y;

    private int stage = 0; // 0 is mid, -1 is lower, 1 is upper
    private float stage_transition = 1; // 0 - begining transition, 1 - transitioned in


    private void Start()
    {
        max_y = (map_height / 2f) - (cam_height / 2f);
        min_y = -max_y; 
    }
    private void Update()
    {
        //float target_y = Mathf.Clamp(ball.position.y, min_y, max_y);
        float ball_y = ball.position.y;
        int target_stage = (int)Mathf.Abs(ball_y) / (int)(cam_height / 2f) * (int)Mathf.Sign(ball_y);
        target_stage = Mathf.Clamp(target_stage, -1, 1);

        if (target_stage != stage)
        {
            StopAllCoroutines();
            StartCoroutine(TransitionStage(target_stage));
        }
    } 

    private IEnumerator TransitionStage(int new_stage)
    {
        int old_stage = stage;
        stage = new_stage;

        stage_transition = 1 - stage_transition;
        while (stage_transition < 1)
        {
            stage_transition = Mathf.Min(stage_transition + Time.deltaTime / 2f, 1);
            float t = 1-Mathf.Pow(1-stage_transition, 2);

            float y = Mathf.Lerp(old_stage * cam_height, stage * cam_height, t);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);

            yield return null;
        }
    }
}
