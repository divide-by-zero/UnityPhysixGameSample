using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TargetObject : MonoBehaviour
{
    private float? beginTime;

    [SerializeField]
    private Text infoLabel;

    [SerializeField]
    private Rigidbody2D rb;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("ClearObject")) return;
        beginTime = Time.realtimeSinceStartup;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("ClearObject")) return;
        if (!beginTime.HasValue) return;

        var elapsedTime = Time.realtimeSinceStartup - beginTime.Value;
        if (elapsedTime > 3.0f)
        {
            infoLabel.text = "成功！！\nおめでとう！！";
        }
        else
        {
            infoLabel.text = ((int)Mathf.Ceil(3.0f - elapsedTime)).ToString();
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("ClearObject")) return;
        beginTime = null;
        infoLabel.text = "";
    }

    public void Update()
    {
        //RigibodyがSleepするとOnCollisionStay2Dが走らなくなるので無理やり起こし続ける
        rb.WakeUp();

        //TODO ここに書くのはオカシイんだけど、Escapeでリトライ
        if(Input.GetKeyDown(KeyCode.Escape))SceneManager.LoadScene(0);
    }
}
