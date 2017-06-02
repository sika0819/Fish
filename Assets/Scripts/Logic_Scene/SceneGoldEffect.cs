using UnityEngine;
using System.Collections.Generic;
using PathologicalGames;

public class MatchJudge
{
   public static bool IsMatch()
    {
        return PlayerRole.Instance.RoleInfo.RoleMe.GetMonthID() != 0; 
    }
}
//金币结束一刻放大效果
public class GoldEndLargenData
{
    public GameObject   m_GameObj;
    public Transform    m_TrnasGold;
    public float        m_LifeTime;
    private SpawnPool goldEffectPool;

    public void Init()
    {
        m_TrnasGold = m_GameObj.transform;
        m_LifeTime = 0.1f;
    }
    public bool Update(float delatime)
    {
        m_LifeTime -= delatime;
        if (m_LifeTime <= 0)
            return false;
        else
        {
            GoldTrnasScale(delatime);
            return true;
        }
    }
    void GoldTrnasScale(float delatime)
    {
        Vector3 TempScale = m_TrnasGold.localScale;
        TempScale += new Vector3(4.0f * delatime, 4.0f * delatime, 4.0f * delatime);
        m_TrnasGold.localScale = TempScale;
    }
    public void DestorySelf()
    {

        if (PoolManager.Pools.ContainsKey("GoldEffect"))
        {
            goldEffectPool = PoolManager.Pools["GoldEffect"];
            if(goldEffectPool.IsSpawned(m_TrnasGold))
                goldEffectPool.Despawn(m_TrnasGold);
        }
        //if (m_GameObj != null)
        //{
        //    GameObject.Destroy(m_GameObj);
        //    m_GameObj = null;
        //}
    }
}

//金币数字Label
public class GoldEffectLabelData
{
    public GameObject   GameObj;
    public Transform    m_BaseTrans;
    TweenPosition       m_TweenPosAnim;
    float               m_LifeTime = 1.01f;
    UILabel             GoldLLabel;
    bool                m_bScaleAnim = true;
    private SpawnPool goldEffectPool;

    public bool bScaleAnim
    {
        get { return m_bScaleAnim; }
        set { m_bScaleAnim = value; }
    }
    public float LifeTime
    {
        get { return m_LifeTime; }
        set { m_LifeTime = value; }
    }
    public void Init()
    {
        m_BaseTrans = GameObj.transform;
        m_BaseTrans.localScale = Vector3.one;
        GoldLLabel = GameObj.transform.GetChild(0).GetComponent<UILabel>();
        if (!bScaleAnim)
            m_TweenPosAnim = m_BaseTrans.GetComponent<TweenPosition>();
    }
    public void ResetScale()
    {
        m_BaseTrans.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
    public bool Update(float dTime)
    {
        LifeTime -= dTime;
        if (LifeTime <= 0)
            return false;
        else
        {
            if (m_bScaleAnim)
                TransFScale(dTime);
           return true;
        }
    }
    void TransFScale(float delatime)
    {
        if (LifeTime > 0.97f)
        {
            if (m_BaseTrans.localScale != Vector3.one)
                m_BaseTrans.localScale = Vector3.one;
        }
        else if (LifeTime < 0.97f && LifeTime > 0.85f)
        {
            Vector3 TempScale = m_BaseTrans.localScale;
            if (TempScale.x < 1.6f)
            {
                TempScale += new Vector3(4.5f * delatime, 4.5f * delatime, 4.5f * delatime);
                m_BaseTrans.localScale = TempScale;
            }
        }
        else
        {
            Vector3 TempScale = m_BaseTrans.localScale;
            if (TempScale.x > 1.0f)
            {
                TempScale -= new Vector3(4.5f * delatime, 4.5f * delatime, 4.5f * delatime);
                m_BaseTrans.localScale = TempScale;
            }
        }
       
    }
    public void SetText(string str)
    {
        GoldLLabel.text ="+"+ str;
    }
    public void SetLabelFontSize()
    {
        GoldLLabel.fontSize = 10;
    }
    public void SetPos(Vector3 pos)
    {
        pos.y += 0.1f;
        m_BaseTrans.position = pos;
    }
    public void SetTweenPos()
    {
        m_TweenPosAnim.from = m_BaseTrans.localPosition;
        Vector3 ToPos = m_BaseTrans.localPosition;
        ToPos.y += 40f;
        m_TweenPosAnim.to = ToPos;
    }
    public void DestorySelf()
    {

        //if (PoolManager.Pools.ContainsKey("GoldEffect"))
        //{
        //    goldEffectPool = PoolManager.Pools["GoldEffect"];
        //    if(goldEffectPool.IsSpawned(m_BaseTrans))
        //        goldEffectPool.Despawn(m_BaseTrans);
        //}
        if (GameObj != null)
        {
            GameObject.Destroy(GameObj);
            GameObj = null;
        }
    }

}
public class GoldEffectData
{   
    public GameObject       GameObj;
    public SimplePath       Path;
    public Vector3          m_vecpathend;
    public CatchedData      catchedData;
    public SceneGoldEffect.GoldTween m_Tween;
    public float            m_DelayTime = 1.0f;
    public bool             HaveChangeRoad = false;
    public uint GoldNum;
    public uint TotalNum;
    public byte ClientSeat;
    private SpawnPool goldEffectPool;
    public bool Update(float dTime)
    {
        m_DelayTime -= dTime;
        if (m_DelayTime > 0)
        {
            return true;
        }
        if (!HaveChangeRoad)
        {
            HaveChangeRoad = true;
            InitCRPath();
            PlayTween(false);           
        }
        GameObj.transform.position = Path.Update(dTime);
        return !Path.Finished;
        
    }
    public void PlayTween(bool bPlay)
    {
        if (bPlay)
        {
            if (!m_Tween.m_Pos.enabled) 
                m_Tween.m_Pos.enabled = true;
            if (!m_Tween.m_Sclae.enabled) 
                m_Tween.m_Sclae.enabled = true;
            m_Tween.m_Pos.ResetToBeginning();
            m_Tween.m_Sclae.ResetToBeginning();
            m_Tween.m_Pos.PlayForward();
            m_Tween.m_Sclae.PlayForward();
        }
        else
        {
            m_Tween.m_Pos.enabled = false;
            m_Tween.m_Sclae.enabled = false;
        }
    }
    public void InitCRPath()
    {

        Path = new SimplePath(GameObj.transform.position, m_vecpathend);
        Path.Speed = Utility.Range(2.0f, 3.0f);     

        //Vector3 PlayerPos = SceneRuntime.GetLauncherGoldIconPos(catchedData.ClientSeat);
        //Vector3 startControl = (((GameObj.transform.position + PlayerPos) * 0.5f) + GameObj.transform.position)*0.5f;
        //Vector3 endControl = (((GameObj.transform.position + PlayerPos) * 0.5f) + PlayerPos)*0.5f;
        //Path = new CRPath(GameObj.transform.position, startControl, PlayerPos, endControl);
        //Path.Speed = GetPathSpeed();
    }
    public void ScaleGoldTR(float fScalefrom,float fScaleto)
    {
        //uint GoldNum = this.catchedData.CatchType;
        this.GameObj.transform.localScale = new Vector3(fScalefrom, fScalefrom, fScalefrom);
        Vector3 Pos_ = GameObj.transform.localPosition;
        Pos_.z = 0;
        m_Tween.m_Pos.to = Pos_; 
        Pos_.y += 100;
        m_Tween.m_Pos.from = Pos_;
        m_Tween.m_Sclae.from = new Vector3(fScalefrom, fScalefrom, fScalefrom);// Vector3.one; ;
        m_Tween.m_Sclae.to = new Vector3(fScaleto, fScaleto, fScaleto);// new Vector3(fScale, fScale, fScale);
    }
    public void DestorySelf()
    {

        //if (PoolManager.Pools.ContainsKey("GoldEffect"))
        //{
        //    goldEffectPool = PoolManager.Pools["GoldEffect"];
        //    if(goldEffectPool.IsSpawned(GameObj.transform))
        //        goldEffectPool.Despawn(GameObj.transform);
        //}
        if (GameObj != null)
            GameObject.Destroy(GameObj);
    }
}
public class BigFishEffect
{
    public GameObject GameObj;
    public Vector3 Position {
        get {
            return GameObj.transform.position;
        }set {
            GameObj.transform.position = value;
        }
    }
    public CatchedData catchedData;
    public float m_DelayTime = 0;
    public float m_LifeTime = 1.0f;
    public Transform m_BaseTrans;
    TweenPosition m_TweenPosAnim;
    public byte ClientSeat;
    private UISprite FishSprite;
    bool m_bScaleAnim = true;
    private SpawnPool goldEffectPool;

    public bool bScaleAnim
    {
        get { return m_bScaleAnim; }
        set { m_bScaleAnim = value; }
    }
    public bool Update(float dTime)
    {
        m_DelayTime -= dTime;
        if (m_DelayTime > 0)
        {
            return true;
        }
        m_LifeTime -= dTime;
        if (m_LifeTime <= 0)
        {
            return false;
        }
        else {
            if (m_bScaleAnim)
                TransFScale(dTime);
            return true;
        }
    }
    public void Init(Vector3 pos)
    {
        Position = pos;
        FishSprite = GameObj.transform.FindChild("FishSprite").GetComponent<UISprite>();
        m_BaseTrans = GameObj.transform;
        m_BaseTrans.localScale = Vector3.one;
        if (!bScaleAnim)
            m_TweenPosAnim = GameObj.GetComponent<TweenPosition>();
    }
    void TransFScale(float delatime)
    {
        if (m_LifeTime > 0.97f)
        {
            if (m_BaseTrans.localScale != Vector3.one)
                m_BaseTrans.localScale = Vector3.one;
        }
        else if (m_LifeTime < 0.97f && m_LifeTime > 0.85f)
        {
            Vector3 TempScale = m_BaseTrans.localScale;
            if (TempScale.x < 1.6f)
            {
                TempScale += new Vector3(4.5f * delatime, 4.5f * delatime, 4.5f * delatime);
                m_BaseTrans.localScale = TempScale;
            }
        }
        else
        {
            Vector3 TempScale = m_BaseTrans.localScale;
            if (TempScale.x > 1.0f)
            {
                TempScale -= new Vector3(4.5f * delatime, 4.5f * delatime, 4.5f * delatime);
                m_BaseTrans.localScale = TempScale;
            }
        }

    }
    public void SetTweenPos(Vector3 pos)
    {
        m_BaseTrans.position = pos;
        m_TweenPosAnim = m_BaseTrans.GetComponent<TweenPosition>();
        if (m_BaseTrans)
        {
            m_TweenPosAnim.from = m_BaseTrans.transform.localPosition;
            Vector3 ToPos = m_BaseTrans.localPosition;
            ToPos.y += 40f;
            m_TweenPosAnim.to = ToPos;
        }
    }
    public void SetSprite(string spriteName)
    {
        if (GameObj != null)
        {
            if (FishSprite)
                FishSprite.spriteName = spriteName;
        }
    }
    public void DestorySelf()
    {

        //if (PoolManager.Pools.ContainsKey("GoldEffect"))
        //{
        //    goldEffectPool = PoolManager.Pools["GoldEffect"];
        //    goldEffectPool.Despawn(GameObj.transform);
        //}
        if (GameObj != null)
            GameObject.Destroy(GameObj);
    }
}
public class SceneGoldEffect
{
    List<GoldEffectData>                m_CatchedList = new List<GoldEffectData>();
    List<GoldEffectData>                m_UnlockRateList = new List<GoldEffectData>();//解锁奖励金币
    List<GoldEffectData>                m_diamondList = new List<GoldEffectData>();//钻石掉落
    List<GoldEffectLabelData>           m_CatchedLabelList = new List<GoldEffectLabelData>();
    List<GoldEffectLabelData>           m_GetGoldList = new List<GoldEffectLabelData>();
    List<BigFishEffect>                 m_bigEffectList = new List<BigFishEffect>(); 
    GoldEndLargenData                   m_EndLargenGold = new GoldEndLargenData();
    BigFishEffect                       m_BigFishEffect=new BigFishEffect();
    Object                              m_GoldObj = null;
    Object                              m_ScoreObj = null;
    Object                               m_DiamondObj = null;
    Object                              m_LabelObj = null;
    Object                              m_LargenGoldObj = null;
    Object                              m_GetGoldLabelObj = null;
    Object                              m_BossFishSpecailEftObj = null;
    Object                              m_BigFishEffectObj = null;//抓到大鱼的用户显示大鱼图标
    GameObject                          BigFishObj;
    #region 灰色金币声明
    Object                              m_GrayGoldObj = null;//灰色金币物体
    Object                              m_GetGrayGoldLabelObj = null;//灰色金币显示UI
    Object                              m_GrayLabelObj = null;//???
    #endregion
    SpawnPool                           GoldEffectPool;
    public void Init()
    {
        m_GoldObj = ResManager.Instance.LoadObject("SceneGoldEffect", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_ScoreObj = ResManager.Instance.LoadObject("SceneStarEffect", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_DiamondObj = ResManager.Instance.LoadObject("SceneDiamondEffect", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_LabelObj = ResManager.Instance.LoadObject("SceneGoldLabelEffect", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_LargenGoldObj = ResManager.Instance.LoadObject("SceneGoldLargen", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_GetGoldLabelObj = ResManager.Instance.LoadObject("SceneGetGoldLabel", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_BossFishSpecailEftObj = ResManager.Instance.LoadObject("Ef_Boss", "SceneRes/Prefab/Effect/Boss/", ResType.SceneRes);
        m_BigFishEffectObj = ResManager.Instance.LoadObject("GetFishEffect", "SceneRes/Prefab/Effect/CatchBigFish/", ResType.SceneRes);
        #region 灰色金币，其他用户的金币
        m_GrayGoldObj = ResManager.Instance.LoadObject("SceneGrayGoldEffect", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_GetGrayGoldLabelObj=ResManager.Instance.LoadObject("SceneGetGrayGoldLabel", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        m_GrayLabelObj= ResManager.Instance.LoadObject("SceneGrayGoldLabelEffect", "SceneRes/Prefab/UI/Gold/", ResType.SceneRes);
        #endregion
        if (!PoolManager.Pools.ContainsKey("GoldEffect"))
        {
            GoldEffectPool = PoolManager.Pools.Create("GoldEffect");
        }
        GoldEffectPool = PoolManager.Pools["GoldEffect"];
        GoldEffectPool.gameObject.AddComponent<UIPanel>();
        UIPanel effectPanel = GoldEffectPool.gameObject.GetComponent<UIPanel>();
        effectPanel.depth = 1;
        SetParent(GoldEffectPool.transform);
        #region 定义预制物体
        //PrefabPool GoldPrebPool = new PrefabPool(((GameObject)m_GoldObj).transform);
        //GoldPrebPool.preloadAmount = 1;
        //GoldPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //GoldPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //GoldPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //GoldPrebPool.cullDespawned = true;
        ////最终保留
        //GoldPrebPool.cullAbove = 1;
        ////多久清理一次
        //GoldPrebPool.cullDelay = 5;
        ////每次清理几个
        //GoldPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(GoldPrebPool);

        //PrefabPool GrayGoldPrebPool = new PrefabPool(((GameObject)m_GrayGoldObj).transform);
        //GrayGoldPrebPool.preloadAmount = 1;
        //GrayGoldPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //GrayGoldPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //GrayGoldPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //GrayGoldPrebPool.cullDespawned = true;
        ////最终保留
        //GrayGoldPrebPool.cullAbove = 1;
        ////多久清理一次
        //GrayGoldPrebPool.cullDelay = 5;
        ////每次清理几个
        //GrayGoldPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(GrayGoldPrebPool);

        //PrefabPool LabelPrebPool = new PrefabPool(((GameObject)m_LabelObj).transform);
        //LabelPrebPool.preloadAmount = 1;
        //LabelPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //LabelPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //LabelPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //LabelPrebPool.cullDespawned = true;
        ////最终保留
        //LabelPrebPool.cullAbove = 1;
        ////多久清理一次
        //LabelPrebPool.cullDelay = 5;
        ////每次清理几个
        //LabelPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(LabelPrebPool);

        //PrefabPool GrayLabelPrebPool = new PrefabPool(((GameObject)m_GrayLabelObj).transform);
        //GrayLabelPrebPool.preloadAmount = 1;
        //GrayLabelPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //GrayLabelPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //GrayLabelPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //GrayLabelPrebPool.cullDespawned = true;
        ////最终保留
        //GrayLabelPrebPool.cullAbove = 1;
        ////多久清理一次
        //GrayLabelPrebPool.cullDelay = 5;
        ////每次清理几个
        //GrayLabelPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(GrayLabelPrebPool);

        //PrefabPool LargenGoldPrebPool = new PrefabPool(((GameObject)m_LargenGoldObj).transform);
        //LargenGoldPrebPool.preloadAmount = 1;
        //LargenGoldPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //LargenGoldPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //LargenGoldPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //LargenGoldPrebPool.cullDespawned = true;
        ////最终保留
        //LargenGoldPrebPool.cullAbove = 1;
        ////多久清理一次
        //LargenGoldPrebPool.cullDelay = 2;
        ////每次清理几个
        //LargenGoldPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(LargenGoldPrebPool);

        //PrefabPool GetGoldPrebPool = new PrefabPool(((GameObject)m_GetGoldLabelObj).transform);
        //GetGoldPrebPool.preloadAmount = 1;
        //GetGoldPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //GetGoldPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //GetGoldPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //GetGoldPrebPool.cullDespawned = true;
        ////最终保留
        //GetGoldPrebPool.cullAbove = 1;
        ////多久清理一次
        //GetGoldPrebPool.cullDelay = 2;
        ////每次清理几个
        //GetGoldPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(GetGoldPrebPool);

        //PrefabPool GetGrayGoldPrebPool = new PrefabPool(((GameObject)m_GetGrayGoldLabelObj).transform);
        //GetGrayGoldPrebPool.preloadAmount = 1;
        //GetGrayGoldPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //GetGrayGoldPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //GetGrayGoldPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //GetGrayGoldPrebPool.cullDespawned = true;
        ////最终保留
        //GetGrayGoldPrebPool.cullAbove = 1;
        ////多久清理一次
        //GetGrayGoldPrebPool.cullDelay = 5;
        ////每次清理几个
        //GetGrayGoldPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(GetGrayGoldPrebPool);

        //PrefabPool bigFishPrebPool = new PrefabPool(((GameObject)m_BossFishSpecailEftObj).transform);
        //bigFishPrebPool.preloadAmount = 2;
        //bigFishPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //bigFishPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //bigFishPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //bigFishPrebPool.cullDespawned = true;
        ////最终保留
        //bigFishPrebPool.cullAbove = 1;
        ////多久清理一次
        //bigFishPrebPool.cullDelay = 2;
        ////每次清理几个
        //bigFishPrebPool.cullMaxPerPass = 5;

        //PrefabPool DemondPrebPool = new PrefabPool(((GameObject)m_DiamondObj).transform);
        //DemondPrebPool.preloadAmount = 2;
        //DemondPrebPool.limitInstances = true;
        ////关闭无限取Prefab
        //DemondPrebPool.limitFIFO = false;
        ////限制池子里最大的Prefab数量
        //DemondPrebPool.limitAmount = 5;
        ////开启自动清理池子
        //DemondPrebPool.cullDespawned = true;
        ////最终保留
        //DemondPrebPool.cullAbove = 1;
        ////多久清理一次
        //DemondPrebPool.cullDelay = 2;
        ////每次清理几个
        //DemondPrebPool.cullMaxPerPass = 5;
        ////初始化内存池
        //GoldEffectPool._perPrefabPoolOptions.Add(DemondPrebPool);
        #endregion
    }
    public void Clear()
    {
        GoldEffectPool.DespawnAll();
        //for (byte i = 0; i < m_CatchedList.Count; ++i)
        //    m_CatchedList[i].DestorySelf();
        //for (byte i = 0; i < m_CatchedLabelList.Count; ++i)
        //    m_CatchedLabelList[i].DestorySelf();
        //for (byte i = 0; i < m_GetGoldList.Count; ++i)
        //    m_GetGoldList[i].DestorySelf();

        if (m_bigEffectList.Count > 0)
        {
            for (int iLoop = m_bigEffectList.Count-1; iLoop >= 0; --iLoop)
            {
                //m_bigEffectList[iLoop].DestorySelf();
                Utility.ListRemoveAt(m_bigEffectList, iLoop);
            }
        }
        m_CatchedList.Clear();
        m_CatchedLabelList.Clear();
        m_EndLargenGold.DestorySelf();
        m_GetGoldList.Clear();
        m_bigEffectList.Clear();
        
        
    }
    public void ShowBossFishSpecailEft(CatchedData cd, Fish fish)
    {
        //GameObject go = GameObject.Instantiate(m_BossFishSpecailEftObj) as GameObject;
        GameObject go;
        if (GoldEffectPool.prefabPools.ContainsKey(m_BossFishSpecailEftObj.name))
        {
            //初始化内存池
            go = GoldEffectPool.Spawn(m_BossFishSpecailEftObj.name).gameObject;
        }
        else {
            go = GoldEffectPool.Spawn((GameObject)m_BossFishSpecailEftObj).gameObject;
        }
        //go.transform.SetParent(SceneObjMgr.Instance.UIPanelTransform, false);
        go.transform.position = SceneRuntime.WorldToNGUI(fish.Position);
        GlobalEffectData bossFishEft = new GlobalEffectData(go, 0, 5);
        GlobalEffectMgr.Instance.AddEffect(bossFishEft);
       // GlobalEffectMgr.SetEffectOnUI(go);

        SceneObjMgr.Instance.PlayBack(BackAnimType.BACK_ANIM_BOSS);

    }
    public void ShowDiamond(CatchedData cd, Fish fish)
    {
        GoldEffectData ged = new GoldEffectData();
        ged.GameObj = Initobj(m_DiamondObj);

        ged.GameObj.transform.position = SceneRuntime.WorldToNGUI(fish.Position) + (new Vector3(Utility.RandFloat(), Utility.RandFloat(), 0) * 0.45f);
        ged.m_Tween.m_Pos = ged.GameObj.transform.GetComponent<TweenPosition>();
        ged.m_Tween.m_Sclae = ged.GameObj.transform.GetComponent<TweenScale>();
        ged.ScaleGoldTR(1.0f,0.6f);
        ged.m_DelayTime = 2;
        ged.GoldNum = 1;
        ged.PlayTween(true);
        ged.m_vecpathend = SceneRuntime.GetLauncherGoldIconPos(cd.ClientSeat);        
        m_diamondList.Add(ged);
    }
    //单个金币
    public void ShowUnLockRateReward(Vector3 pos)
    {
        for (int i = 0; i < 3; i++)
        {
            GoldEffectData ged = new GoldEffectData();           
            ged.GameObj = Initobj(m_GoldObj);
            ged.GameObj.transform.position = pos+((new Vector3(Utility.RandFloat(), Utility.RandFloat(), 0)) * 0.15f);
            ged.m_Tween.m_Pos = ged.GameObj.transform.GetComponent<TweenPosition>();
            ged.m_Tween.m_Sclae = ged.GameObj.transform.GetComponent<TweenScale>();
            ged.ScaleGoldTR(0.6f,0.6f);
            ged.m_DelayTime = 0;
            ged.GoldNum = 1;
            ged.PlayTween(true);
            ged.m_vecpathend = SceneRuntime.GetLauncherGoldIconPos(SceneRuntime.SceneLogic.PlayerMgr.MySelf.ClientSeat);
            m_UnlockRateList.Add(ged);
        }
      //  if (SceneRuntime.PlayerMgr.GetPlayer(ged.catchedData.ClientSeat) == SceneRuntime.PlayerMgr.MySelf)
         //   GlobalAudioMgr.Instance.PlayOrdianryMusic(Audio.OrdianryMusic.m_GoldJump);
        
    }
    public void ShowGoldEffect(CatchedData cd, Fish fish)
    {
        if (MatchJudge.IsMatch()&&cd.ClientSeat!=SceneRuntime.MyClientSeat)//比赛只显示自己的效果
        {
            return;
        }
        Vector3 vecGoldEndpos = Vector3.one;
        if (MatchJudge.IsMatch())
        {
            if (cd.ClientSeat == SceneRuntime.MyClientSeat)
            {
                vecGoldEndpos = GlobalHallUIMgr.Instance.MatchScorePos();
            }
            else
            {
                vecGoldEndpos = SceneRuntime.GetLauncherGoldIconPos(cd.ClientSeat);
            }                               
        }
        else//比赛
        {
            vecGoldEndpos = SceneRuntime.GetLauncherGoldIconPos(cd.ClientSeat);
        }

        const int perGoldNum = 30;
        //这里的金币用的还是鱼的类型
        Vector3 FishPos = SceneRuntime.WorldToNGUI(fish.Position);
        //鱼的价值
        uint fishOrgGold = (uint)FishSetting.FishDataList[fish.FishType].Gold;
        uint fishGold = 0;
        if (cd.CatchType == (byte)CatchedType.CATCHED_SKILL)
            fishGold = fishOrgGold * SkillSetting.SkillDataList[cd.SubType].multiple;
        else
            fishGold = fishOrgGold * BulletSetting.BulletRate[cd.RateIndex];
        if (!MatchJudge.IsMatch()&&cd.ClientSeat==SceneRuntime.MyClientSeat)
        {
            GlobalHallUIMgr.Instance.GameShare.AddGlod((int)fishGold);
        }
        
        uint goldNum = fishOrgGold / perGoldNum;
        uint perNum = 0;
        uint lastNum = 0;
        if(goldNum == 0)
        {
            goldNum = 1;
            lastNum = fishGold;
        }
        else
        {
            perNum = fishGold / goldNum;
            lastNum = perNum + (fishGold - perNum * goldNum);
        }
        uint num = 0;
        for (byte i = 0; i < goldNum; ++i)
        {            
            GoldEffectData ged = new GoldEffectData();
            ged.catchedData = cd;        

            if (MatchJudge.IsMatch())
            {
                ged.GameObj = Initobj(m_ScoreObj);               
            }
            else
            {
                if (cd.ClientSeat==SceneRuntime.PlayerMgr.MyClientSeat)
                {
                    ged.GameObj = Initobj(m_GoldObj);
                }
                else {
                    ged.GameObj = Initobj(m_GrayGoldObj);
                }                           
            }
  
         
            ged.GameObj.transform.position = FishPos + (new Vector3(Utility.RandFloat(), Utility.RandFloat(), 0)) * (fish.IsBossFish()?0.45f:0.15f);
            ged.m_Tween.m_Pos = ged.GameObj.transform.GetComponent<TweenPosition>();
            ged.m_Tween.m_Sclae = ged.GameObj.transform.GetComponent<TweenScale>();
            ged.ScaleGoldTR(1.0f,fish.IsBossFish()?1.0f:0.6f);
            if (i > 0 && !fish.IsBossFish())
                ged.m_DelayTime += i * 0.1f;
            if (fish.IsBossFish())
            {
                ged.m_DelayTime = 1.5f;
            }
         
            if (i == goldNum - 1)
                ged.GoldNum = lastNum;
            else
                ged.GoldNum = perNum;
            num += ged.GoldNum;
            ged.ClientSeat = cd.ClientSeat;
            ged.TotalNum = (uint)cd.TotalNum;
            ged.PlayTween(true);
            ged.m_vecpathend = vecGoldEndpos;
            if (SceneRuntime.PlayerMgr.GetPlayer(ged.catchedData.ClientSeat) == SceneRuntime.PlayerMgr.MySelf)
                GlobalAudioMgr.Instance.PlayOrdianryMusic(Audio.OrdianryMusic.m_GoldJump);
            m_CatchedList.Add(ged);
        }

        // ShowGoldNumLabel(fishGold, FishPos, fish);
        ShowGoldNumLabel((uint)cd.GoldNum, FishPos,fish,cd.ClientSeat);
        
    }
    public void ShowBigFishGoldEffect(CatchedData cd, Fish fish)
    {
        if (fish.FishType!=null)
        {
            BigFishEffect bigFish = new BigFishEffect();
            m_bigEffectList.Add(bigFish);
            BigFishObj = Initobj(m_BigFishEffectObj);
            Vector3 pos = SceneRuntime.PlayerMgr.GetPlayer(cd.ClientSeat).Launcher.LauncherPos;
            bigFish.GameObj = BigFishObj;
            bigFish.m_LifeTime = 0.5f;
            if (m_bigEffectList.Count > 0)
                bigFish.m_DelayTime = 0.5f * m_bigEffectList.Count;
            bigFish.Init(pos);
            Vector3 tempPos = new Vector3(pos.x,0,pos.z);
            Vector3 newPos = pos-tempPos;
            newPos = new Vector3(pos.x,newPos.y*0.7f,pos.z);
            bigFish.SetTweenPos(newPos);
            bigFish.SetSprite(fish.FishType.ToString());
        }
    }
    public void PlayAudio(byte fishType,int randIndex) {

        switch (fishType)
        {
            case 1://boss蓝鲨
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectGoldBlueShark1);
                }
                else {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectGoldBlueShark2);
                }
                break;
            case 3://boss黑鲨
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectGoldShark1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectGoldShark2);
                }
                break;
            case 19://boss虎鲸
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectGoldJing1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectGoldJing2);
                }
                break;
            case 9://蓝鲨
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectBlueShark1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectBlueShark2);
                }
                break;
            case 10://双髻鲨
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectTiggerShark1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectTiggerShark2);
                }
                break;
            case 18://黑鲨
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectBlackShark1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectBlackShark2);
                }
                break;
            case 13://海龟
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectTurtoise1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectTurtoise2);
                }
                break;
            case 20://虎鲸
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectJing1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectJing2);
                }
                break;
            case 16:
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectSword1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectSword2);
                }
                break;
            case 23:
                if (randIndex == 0)
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectBian1);
                }
                else
                {
                    GlobalAudioMgr.Instance.PlayVoiceMusic(Audio.EffectVoiceType.EffectBian2);
                }
                break;
            default:
                break;
        }
    }
    public void ShowGetGoldNum(int  GoldNum, byte ClientSeat)
    {
        //显示一炮获得的总的金币数
        //剔除掉时间间隔短的
        if (m_GetGoldList.Count > 0)
        {
            GoldEffectLabelData gold = m_GetGoldList[m_GetGoldList.Count - 1];
            if (gold.LifeTime > 0.48f)
                return;
        }
        
        Vector3 pos = SceneRuntime.PlayerMgr.GetPlayer(ClientSeat).Launcher.LauncherPos;
        pos.x += 0.4f;
        pos.y += 0.125f;
        ShowGetGold(GoldNum, pos,ClientSeat);
    }
    void UpdateRoleGold(GoldEffectData ged)
    {
        if (ged != null)
        {
          
            if (PlayerRole.Instance.RoleInfo.RoleMe.GetMonthID() != 0)
            {
                GlobalHallUIMgr.Instance.UpdateMyMatchScoreInfo();
            }

            if (ged.ClientSeat == SceneRuntime.PlayerMgr.MyClientSeat)
            {
                //uint gold = PlayerRole.Instance.RoleInfo.RoleMe.GetGlobel() + ged.GoldNum;
                PlayerRole.Instance.OnAddUserGlobelByCatchedData(ged.ClientSeat,(int)ged.GoldNum);
            }
            else
            {
                ScenePlayer sp = SceneRuntime.PlayerMgr.GetPlayer(ged.ClientSeat);
                if (sp != null)
                {
                    uint gold = PlayerRole.Instance.TableManager.GetTableRole(sp.Player.playerData.ID).GetGlobel() + ged.GoldNum;
                    PlayerRole.Instance.TableManager.GetTableRole(sp.Player.playerData.ID).SetGlobel(gold);
                }
            }
            
        }
    }
    void ShowGoldNumLabel(uint _Score, Vector3 Pos, Fish fish, byte ClientSeat,bool bScale = true)
    {
        GoldEffectLabelData label = new GoldEffectLabelData();
        if (ClientSeat==SceneRuntime.PlayerMgr.MyClientSeat)
        {
            label.GameObj = Initobj(m_LabelObj);
        }
        else {
            label.GameObj = Initobj(m_GrayLabelObj);
        }
        if (label.GameObj != null)
        {
            label.bScaleAnim = bScale;
            label.Init();
            if (!bScale)
            {
                label.ResetScale();
                label.LifeTime = 2.0f;
            }
            label.SetPos(Pos);
            label.SetText(_Score.ToString());
            if (fish.IsBossFish())
            {
                label.LifeTime = 3.0f;
                label.SetLabelFontSize();
            }
            m_CatchedLabelList.Add(label);
        }
    }
   void ShowGetGold(int gold, Vector3 pos,byte ClientSeat)
    {
        GoldEffectLabelData getGold = new GoldEffectLabelData();
        if (ClientSeat==SceneRuntime.PlayerMgr.MyClientSeat)
        {
            getGold.GameObj = Initobj(m_GetGoldLabelObj);
        }
        else {
            getGold.GameObj = Initobj(m_GetGrayGoldLabelObj);
        }
        if (getGold.GameObj != null)
        {
            getGold.bScaleAnim = false;
            getGold.Init();
            //  getGold.ResetScale();
            getGold.LifeTime = 0.5f;
            getGold.SetPos(pos);
            getGold.SetTweenPos();
            getGold.SetText(gold.ToString());
            m_GetGoldList.Add(getGold);
        }
    }
    void ShowGoldEndLargen(byte clientSeat)
    {   
        if(MatchJudge.IsMatch())
        {
            return;
        }
        Vector3 pos = SceneRuntime.GetLauncherGoldIconPos(clientSeat);
        if (m_EndLargenGold.m_GameObj == null && m_EndLargenGold.m_LifeTime <= 0.03f) 
        {
            m_EndLargenGold.m_GameObj = Initobj(m_LargenGoldObj);
            m_EndLargenGold.Init();
            m_EndLargenGold.m_TrnasGold.position = pos;
        }
    }
    public void Update(float delta)
    {
        for (byte i = 0; i < m_CatchedList.Count; )
        {
            GoldEffectData ged = m_CatchedList[i];
            if (ged.Update(delta) == false)
            {                                
                    if (++ged.catchedData.GoldFinished == ged.catchedData.FishList.Count)
                    {

                    }// ShowGoldNumLabel(ged);
                    ShowGoldEndLargen(ged.catchedData.ClientSeat);      //金币结束动画后放大效果
                    UpdateRoleGold(ged);
                    if (SceneRuntime.PlayerMgr.GetPlayer(ged.catchedData.ClientSeat) == SceneRuntime.PlayerMgr.MySelf)
                        GlobalAudioMgr.Instance.PlayOrdianryMusic(Audio.OrdianryMusic.m_UserGetGold);
                
                ged.DestorySelf();
                Utility.ListRemoveAt(m_CatchedList, i);
            }
            else
            {
                ++i;
            }
               
        }

        for (int i = 0; i < m_UnlockRateList.Count; )
        {
            GoldEffectData ged = m_UnlockRateList[i];
             if (ged.Update(delta) == false)
             {
                 ged.DestorySelf();
                 Utility.ListRemoveAt(m_UnlockRateList, i);
                 GlobalAudioMgr.Instance.PlayOrdianryMusic(Audio.OrdianryMusic.m_Niuniu17);
             }
             else
             {
                 i++;
             }
        }
        for (int i = 0; i < m_diamondList.Count; )
        {
            GoldEffectData ged = m_diamondList[i];
            if (ged.Update(delta) == false)
            {
                ged.DestorySelf();
                Utility.ListRemoveAt(m_diamondList, i);
            }
            else
            {
                i++;
            }
        }

        for (byte i = 0; i < m_CatchedLabelList.Count; )
        {
            GoldEffectLabelData data = m_CatchedLabelList[i];
            if (data.Update(delta) == false)
            {
                data.DestorySelf();
                Utility.ListRemoveAt(m_CatchedLabelList, i);
            }
            else
                ++i;
        }
        for (byte j = 0; j < m_GetGoldList.Count; )
        {
            GoldEffectLabelData getGold = m_GetGoldList[j];
            if (getGold.Update(delta) == false)
            {
                getGold.DestorySelf();
                Utility.ListRemoveAt(m_GetGoldList, j);
            }
            else
                ++j;
        }
        if (m_bigEffectList.Count > 0)
        {
            for (int iLoop = m_bigEffectList.Count-1; iLoop >= 0; --iLoop)
            {
                BigFishEffect getBigEffect = m_bigEffectList[iLoop];
                if (getBigEffect.Update(delta) == false)
                {
                    getBigEffect.DestorySelf();
                    Utility.ListRemoveAt(m_bigEffectList, iLoop);
                }
            }
        }
        if (m_EndLargenGold.m_GameObj != null)
        {
            if (m_EndLargenGold.Update(delta) == false)
            {
                m_EndLargenGold.DestorySelf();
            }
        }
    }
    public void Shutdown()
    {
        //GoldEffectPool.DespawnAll();
        for (byte i = 0; i < m_CatchedList.Count;++i )
        {
            m_CatchedList[i].DestorySelf();
        }
            
        for (byte i = 0; i < m_UnlockRateList.Count; ++i)
        {
            m_UnlockRateList[i].DestorySelf();
        }

        for (byte i = 0; i < m_diamondList.Count; ++i)
        {
            m_diamondList[i].DestorySelf();
        }

        for (byte i = 0; i < m_CatchedLabelList.Count; ++i)
        {
            m_CatchedLabelList[i].DestorySelf();
        }
            
        for (byte j = 0; j < m_GetGoldList.Count; ++j )
        {
            m_GetGoldList[j].DestorySelf();
        }
        if (m_bigEffectList.Count > 0)
        {
            for (int iLoop = m_bigEffectList.Count - 1; iLoop >= 0; --iLoop)
            {
                m_bigEffectList[iLoop].DestorySelf();
            }
        }
        m_CatchedList.Clear();
        m_UnlockRateList.Clear();
        m_diamondList.Clear();
        m_CatchedLabelList.Clear();
        m_EndLargenGold.DestorySelf();
        m_bigEffectList.Clear();
        if(PoolManager.Pools.ContainsKey("GoldEffect"))
            PoolManager.Pools.Destroy("GoldEffect");
        ResManager.Instance.UnloadObject(m_GoldObj);
        ResManager.Instance.UnloadObject(m_ScoreObj);
        ResManager.Instance.UnloadObject(m_LabelObj);
        ResManager.Instance.UnloadObject(m_LargenGoldObj);
        ResManager.Instance.UnloadObject(m_GetGoldLabelObj);
        ResManager.Instance.UnloadObject(m_GrayGoldObj);
        ResManager.Instance.UnloadObject(m_GrayLabelObj);
        ResManager.Instance.UnloadObject(m_GetGrayGoldLabelObj);
        ResManager.Instance.UnloadObject(m_BigFishEffectObj);
    }

    GameObject Initobj( Object obj)
    {
        //return SetParent(((GameObject)GameObject.Instantiate(obj)).transform).gameObject;
        GameObject temp=null;
        if (GoldEffectPool.prefabPools.ContainsKey(obj.name))
        {
            temp = GoldEffectPool.Spawn(obj.name).gameObject;
            temp.transform.position = Vector3.zero;
            temp.transform.localScale = Vector3.one;
            temp.transform.rotation = Quaternion.identity;
            return temp;
        }
        temp = GoldEffectPool.Spawn(((GameObject)obj).transform).gameObject;
        temp.transform.position = Vector3.zero;
        temp.transform.localScale = Vector3.one;
        temp.transform.rotation = Quaternion.identity;
        return temp;
    }
    Transform SetParent(Transform objtr)
    {
        objtr.SetParent(SceneObjMgr.Instance.UIPanelTransform, false);
        if (!objtr.gameObject.activeSelf) objtr.gameObject.SetActive(true);
        return objtr;
    }
    public struct GoldTween
    {
        public TweenPosition m_Pos;
        public TweenScale m_Sclae;
    }
    public bool HaveFlyGold(byte byClientSeat)
    {
        foreach(GoldEffectData item in m_CatchedList)
        {
            if (item.ClientSeat == byClientSeat)
            {
                return true;
            }            
        }
        return false;
    }
}
