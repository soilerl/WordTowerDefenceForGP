﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
public class MenShenTower : ShotTower {

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
    protected override void Attack(Monster monster)
    {
        base.Attack(monster);
    }

    protected override void SpawnBullet(string type, EventObject eventObject)
    {
        if (eventObject.name == "generateBullet")
        {
            Game.Instance.a_ObjectPool.ResourcesDir = "Prefabs/Bullets";
            GameObject go = Game.Instance.a_ObjectPool.Spawn("MenShenBullet");
            MenShenBullet bullet = go.GetComponent<MenShenBullet>();
            bullet.transform.position = m_ShotPoint.position;
            bullet.LoadBullet(this.UseBulletID, this.Level, this.MapRect, this);
        }
    }
}