﻿using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class LevelBarrier : LevelElement
    {
        public  float Width=14;
        private Transform   m_Body;
        private BoxCollider m_Collider;
        private Vector3 m_Size;

        private LevelObject m_Barrier;
        private float m_DefaultWidth = 14;

        public override void Build()
        {
            if (Width < m_DefaultWidth)
            {
               Width = m_DefaultWidth;
            }
            int count = Mathf.CeilToInt(Width / m_DefaultWidth);
            m_Size.x = count * m_DefaultWidth;
            m_Size.y = 4;
            m_Size.z = 1.5f;

            m_Body = transform.Find("Body");
            if (m_Body == null)
            {
                m_Body = new GameObject("Body").transform;
                m_Body.parent = transform;
                m_Body.transform.localPosition = Vector3.zero;
                m_Body.localEulerAngles = Vector3.zero;
            }
            else
            {
                m_Body.DestroyChildren();
            }
            float halfCount = count * 0.5f;
            for (int i = 0; i < count; i++)
            {
                if (LevelComponent.IsEditorMode)
                {
                    GameObject barrier = LevelComponent.CreateLevelEditorObject(MapHolderType.Portal);

                    Vector3 localPosition = Vector3.right*(i - halfCount + 0.5f)*m_DefaultWidth;
                    localPosition.z = m_Size.z*0.5f;
                    barrier.transform.localPosition = localPosition;
                }
                else
                {
                    m_Barrier = GameEntry.Level.CreateLevelObject(Id);
                    if (m_Barrier == null)
                    {
                        Log.Error("Create barrier failure.");
                        return;
                    }

                    m_Barrier.CachedTransform.position   = transform.position;
                    m_Barrier.CachedTransform.rotation   = transform.rotation;
                    m_Barrier.CachedTransform.localScale = transform.localScale;
                }
            }

            m_Collider = gameObject.GetOrAddComponent<BoxCollider>();
            m_Collider.size = m_Size;
            m_Collider.center = new Vector3(0, m_Size.y * 0.5f, m_Size.z * 0.5f);
            GlobalTools.SetLayer(gameObject, Constant.Layer.BarrerId);
        }

        public override void SetName()
        {
            gameObject.name = "Barrier_" + Id.ToString();
        }

        public override XmlObject Export()
        {
            MapBarrier data = new MapBarrier();
            data.Id = Id;
            data.Width = Width;
            data.TransParam = new MapTransform();
            data.TransParam.Position = Position;
            data.TransParam.Scale = Scale;
            data.TransParam.EulerAngles = Euler;
            return data;
        }

        public override void Import(XmlObject pData,bool pBuild)
        {
            MapBarrier data = pData as MapBarrier;
            if (data != null)
            {
                Id = data.Id;
                Width = data.Width;
                Position = data.TransParam.Position;
                Scale = data.TransParam.Scale;
                Euler = data.TransParam.EulerAngles;
            }

            this.Build();
            this.SetName();
        }
    }
}