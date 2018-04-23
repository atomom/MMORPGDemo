﻿using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;
using Application = UnityEngine.Application;

namespace GameMain
{
    public class LevelPortal : LevelElement
    {
        public int     DestMapID;
        public Vector3 DestPos;
        public bool    DisplayText = false;
        public string  PortalName = string.Empty;
        public ConditionRelationType Relation = ConditionRelationType.And;
        public int OpenLevel;
        public int OpenItemID;
        public int OpenVIP;

        private GameObject m_PortalObj;

        public LevelRegion Region
        {
            get; set;
        }

        public int RegionID
        {
            get { return Region == null ? 0 : Region.Id; }
        }

        public GameObject PortalObj
        {
            get { return m_PortalObj; }
            set { m_PortalObj = value; }
        }

        public override void Build()
        {
            if (m_PortalObj == null)
            {
                if (LevelComponent.IsEditorMode)
                {
                    m_PortalObj = LevelComponent.CreateLevelEditorObject(MapHolderType.Portal);
                    m_PortalObj.transform.parent = transform;
                }
                else
                {
                    LevelObject Portal = GameEntry.Level.CreateLevelObject(Id);
                    m_PortalObj = Portal.gameObject;
                    Portal.CachedTransform.position = transform.position;
                    Portal.CachedTransform.rotation = transform.rotation;
                    Portal.CachedTransform.localScale = transform.localScale;
                }
            }
        }

        public override void SetName()
        {
            gameObject.name = "Portal_" + Id.ToString();
        }

        public override XmlObject Export()
        {
            MapPortal data = new MapPortal
            {
                Id = Id,
                OpenItemID = OpenItemID,
                OpenLevel = OpenLevel,
                OpenVIP = OpenVIP,
                PortalName = PortalName,
                RegionID = RegionID,
                DestMapID = DestMapID,
                DestPos = DestPos,
                DisplayText = DisplayText,
                ConditionRelation = Relation,
                Center = Position,
                Euler = Euler
            };
            return data;
        }

        public override void Import(XmlObject pData,bool pBuild)
        {
           MapPortal data = pData as MapPortal;
            if (data == null)
            {
                return;
            }

            Id = data.Id;
            OpenItemID = data.OpenItemID;
            OpenLevel = data.OpenLevel;
            OpenVIP = data.OpenVIP;
            PortalName = data.PortalName;
            DestMapID = data.DestMapID;
            DestPos = data.DestPos;
            DisplayText = data.DisplayText;
            Relation = data.ConditionRelation;
            Position = data.Center;
            this.Build();
            this.SetName();
            HolderRegion pHolder = GameEntry.Level.GetHolder(MapHolderType.Region) as HolderRegion;

            if (pHolder != null)
                this.Region = pHolder.FindElement(data.RegionID);

            if (Region != null)
            {
                Position = data.Center;
                Euler = data.Euler;
                Region.onTriggerEnter = onTriggerEnter;
            }
        }

        void onTriggerEnter(Collider other)
        {
            if (Region == null|| DestMapID == 0)
            {
                return;
            }

            if (other.gameObject.layer != Constant.Layer.PlayerId &&
                other.gameObject.layer != Constant.Layer.MountId)
            {
                return;
            }

            if (DestMapID == GameEntry.Level.LevelID)
            {
                ActorPlayer actorPlayer = LevelData.Player.Actor as ActorPlayer;

                actorPlayer.Vehicle.TranslateTo(DestPos, true);
            }
            else
            {
                //TODO 进入切换场景流程
                //GameEntry.Scene.l
            }
        }
    }
}

