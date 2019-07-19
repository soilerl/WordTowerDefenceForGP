﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BloodBar : ResuableObject, IReusable
{

    private Transform followTarget;    //该血条跟随的目标
    private Vector2 bloodPos;      //血条在UI界面的坐标

    private Image bloodBar;

    private Text numberText;    //显示伤害的文本框

    private Monster relevantMonster;
    private float currentBloodRate;

    #region 属性
    public Monster RelevantMonster
    {
        get { return relevantMonster; }
        set { relevantMonster = value; }
    }

    public Transform FollowTarget
    {
        get { return followTarget; }
        set { followTarget = value; }
    }
    #endregion

    #region Unity回调
    void Start()
    {
        currentBloodRate = 1;
        bloodBar = transform.Find("Blood").GetComponent<Image>();
        bloodBar.fillAmount = 1;
    }
	
	void Update () 
    {
        if (followTarget != null)
        {
            FollowTargetMove();
        }

        if (relevantMonster != null)
        {
            float tempHpRate = (float)(relevantMonster.Hp) / relevantMonster.MaxHp;

            if (tempHpRate < currentBloodRate)   //如果血量发生变化
            {
                int hpChange = (int)((currentBloodRate - tempHpRate) * relevantMonster.MaxHp);
                if (hpChange > 0)
                {
                    Game.Instance.a_ObjectPool.ResourcesDir = "Prefabs/Items";
                    //Debug.Log("受到了" + hpChange + "伤害");
                    GameObject go = Game.Instance.a_ObjectPool.Spawn("DamageNumberText");
                    go.transform.SetParent(this.gameObject.transform);
                    go.transform.position = this.gameObject.transform.position + new Vector3(0, 10);
                    gameObject.transform.localScale = this.gameObject.transform.localScale;
                    this.ShowDamageNumber(hpChange, go);
                }
                currentBloodRate = tempHpRate;
            }
            SetBloodValue(currentBloodRate);
        }            
	}
    #endregion
    #region 方法
    private void ShowDamageNumber(int hpChange, GameObject go)
    {
        numberText = go.GetComponent<Text>();
        numberText.text = "-" + hpChange.ToString();
        Tweener tween = numberText.transform.DOLocalMove(new Vector3(1.2f, 23.3f, 0f), 1f);
        tween.OnComplete(() => ResetDamageNumber(go));
    }

    private void ResetDamageNumber(GameObject go)
    {
        go.transform.SetParent(null);
        go.transform.localPosition = new Vector3(0f, 10f);
        numberText.text = "";
        Game.Instance.a_ObjectPool.Unspawn(go);
    }


    /// <summary>
    /// 该血条跟随目标物体
    /// </summary>
    void FollowTargetMove()
    {
        //血条位置转换成屏幕坐标
         bloodPos = RectTransformUtility.WorldToScreenPoint(Camera.main, followTarget.position);
         transform.position = bloodPos + new Vector2(0, 30);
    }

    /// <summary>
    /// 设置血条显示
    /// </summary>
    public void SetBloodValue(float bloodValue)
    {
        //怪物血量为零
        if (relevantMonster.Hp == 0)
        {
            Game.Instance.a_ObjectPool.Unspawn(this.gameObject);
            //在血条消失位置下方生成金币特效
            Vector3 goldPostion = transform.position - new Vector3(0, 30, 0);
            //金币生成位置封装成事件
            ItemPositionArgs goldPositionArgs = new ItemPositionArgs() { ItemPostion = goldPostion };
            //由UIBoodParent脚本处理改金币特效事件
            MVC.SendEvent(Consts.E_GoldEffect, goldPositionArgs);
            currentBloodRate = 1;
            return;
        }
        bloodBar.fillAmount = bloodValue;
    }
    #endregion


    public override void OnSpawn()
    {
        
    }

    public override void OnUnspawn()
    {
        followTarget = null;
        relevantMonster = null;
    }
}
