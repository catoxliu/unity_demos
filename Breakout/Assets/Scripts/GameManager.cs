using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    //TODO: Bricks layout and walls should be adjusted according to screen size

    public int m_iBrickNumPerRow;
    public int m_iBrickRow;

    public Paddle m_vPaddle;
    public Ball m_vBall;
    public Text m_vFinish;
    public Text m_vScoreBoard;
    public Text m_vTimeScale;
    public Transform m_vBricksRoot;

    private int m_iGoldScore = 0;
    private int m_iBrickRemains;
    private int m_iMaxBrickNumPerRow = 5;
    private float m_fTimeScale = 1.0f;
    
    private int GoldScore
    {
        set { m_iGoldScore = value; m_vScoreBoard.text = "Gold Score : " + m_iGoldScore; }
        get { return m_iGoldScore; }
    }

    public static GameManager Instance = null;

    private ResourcesManager m_vResourcesManager;

    WaitForSeconds m_vWFS = new WaitForSeconds(0.1f);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        m_vResourcesManager = GetComponent<ResourcesManager>();

        InitBricks();
    }

    public void InitBricks()
    {
        //Recycle old bricks first, then generate new one.
        var bricksNeedRecycle = m_vBricksRoot.GetComponentsInChildren<Brick>();
        foreach(var b in bricksNeedRecycle)
        {
            RecycleResource(b);
        }

        m_iBrickRemains = m_iBrickNumPerRow * m_iBrickRow;
        var bricks = m_vResourcesManager.GetBrick(m_iBrickRemains);
        var idx = 0;
        for (var row = 0; row < m_iBrickRow; row++)
        {
            var height = row * -2.0f;
            for (var i = 0; i < m_iBrickNumPerRow; i++, idx++)
            {
                bricks[idx].transform.parent = m_vBricksRoot;
                bricks[idx].transform.localPosition = new Vector3(
                    Mathf.Lerp(-6,6,(float)i / (float)(m_iBrickNumPerRow-1)), height, 0);
                bricks[idx].SetActive(true);
            }

        }
    }

    public void Loose()
    {
        m_vFinish.gameObject.SetActive(true);
        m_vFinish.text = "Finish with Score : " + m_iGoldScore;
        m_vBall.gameObject.SetActive(false);
        GoldScore = 0;
    }

    public void Reset()
    {
        m_vFinish.gameObject.SetActive(false);
        m_vPaddle.Reset();
        m_vBall.transform.parent = m_vPaddle.transform;
        m_vBall.Reset();
    }

    public void GoldCollection()
    {
        GoldScore++;
    }

    public void BrickBreak(Brick brick)
    {
        StartCoroutine(GenerateGolds(brick.m_iHitsToBreak, brick.transform.position));
        RecycleResource(brick);
        CheckFinish();
    }

    public void RecycleResource(Object resource)
    {
        m_vResourcesManager.RecycleResource(resource);
    }

    void CheckFinish()
    {
        m_iBrickRemains--;
        if (m_iBrickRemains == 0)
        {
            if (m_iBrickNumPerRow == m_iMaxBrickNumPerRow)
            {
                m_iBrickRow++;
                m_iBrickNumPerRow = m_iBrickNumPerRow / 2;
            }
            else
            {
                m_iBrickNumPerRow++;
            }
            //This delay if for last gold to drop and collection.
            Invoke("InitBricks", 3);
            Invoke("Reset", 3);
        }
    }

    public void OnTimeScaleClick()
    {
        m_fTimeScale %= 2.0f;
        m_fTimeScale += 0.5f;
        Time.timeScale = m_fTimeScale;
        m_vTimeScale.text = m_fTimeScale + "x";
    }

    IEnumerator GenerateGolds(int num, Vector3 pos)
    {
        var golds = m_vResourcesManager.GetGold(num);
        foreach(var gold in golds)
        {
            gold.transform.parent = null;
            gold.transform.position = pos;
            gold.SetActive(true);
            //The velocity should be set to zero, or it will fly to random direction.
            gold.GetComponent<Rigidbody>().velocity = Vector3.zero;
            yield return m_vWFS;
        }
    }
}
