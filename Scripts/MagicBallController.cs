using UnityEngine;

public class MagicBallController : MonoBehaviour
{
    public GameObject MagicBall;
    public float MagicBallLifeMaxTime;
    float MagicBallLifeTimer;
    // Update is called once per frame
    void Update()
    {
        MagicBallLifeTimer += Time.deltaTime;
        if (MagicBallLifeTimer > MagicBallLifeMaxTime && MagicBall.name != "MagicBall") {
            Destroy(MagicBall);
        }
    }
}
