﻿using Hints;
using MapEditor.ToolItem;
using MEC;
using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VT_Api.Extension;

namespace MapEditor
{
    public class MapEditUI : MonoBehaviour 
    {
        private const byte TextSize = 70;
        Player Player { get; set; }
        public string Info { get; set; }

        private CoroutineHandle UI;
        public bool UIRuning 
        { 
            get
            {
                return UI.IsRunning;
            } 
            set
            {
                if (value && !UIRuning)
                    UI = Timing.RunCoroutine(UICoroutine());
                else if (!value && UIRuning)
                    UI.IsRunning = false;
            }
        }

        protected void Start()
        {
            Player = gameObject.GetPlayer();
        }

        public IEnumerator<float> UICoroutine()
        {
            yield return Timing.WaitForSeconds(0.5f);
            while (Plugin.Instance.CurentEditedMap != null && Plugin.Instance.CurentEditedMap.Name != Plugin.MapNone && Player != null && Round.Get.RoundIsActive)
            {
                try
                {
                    Hint hint = new TextHint(
                        BuildRichText(),
                        new HintParameter[] { new StringHintParameter(string.Empty) },
                        null,
                        0.75f
                        );
                    Player.Hub.hints.Show(hint);
                }
                catch (Exception e)
                {
                    Synapse.Api.Logger.Get.Error(e);
                    break;
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        public string BuildRichText()
        { 
            StringBuilder builder = new StringBuilder();

            AddEnvironmentSection(builder);
            builder.Append("<voffset=14em>");
            builder.Append(" ");
            builder.Append("</voffset>");
            AddEditInformation(builder);


            return builder.ToString();
        }

        private void AddEditInformation(StringBuilder builder)
        {
            builder.Append($"<size=100%>");
            builder.Append($"<color=#F4D03F>");
            builder.Append($"<align=\"center\">");
            builder.AppendLine($"<b>MapEditing Info</b>");
            
            builder.AppendLine($"Map Info | ID : {Plugin.Instance.CurentEditedMap.Name}");

            if (Player.ItemInHand.IsDefined() && Player.ItemInHand.TryGetScript(out var item) && item is ITool tool)
                builder.AppendLine($"Tool | name : {item.Info.Name}, {tool.Info}");
            else
                builder.AppendLine($"No Tool in hand");

            builder.AppendLine(Info);
        }

        //https://github.com/AlmightyLks-SCP/HelperPlus/blob/main/HelperPlus/Services/RichTextBuilder.cs
        private void AddEnvironmentSection(StringBuilder builder)
        {
            builder.Append($"<size=100%>");
            builder.Append($"<color=#51d400>");
            builder.Append($"<align=\"left\">");
            builder.AppendLine($"<b>Environment Info</b>");

            bool didRaycastHit = Physics.Raycast(
                Player.CameraReference.transform.position,
                Player.CameraReference.transform.forward,
                out RaycastHit raycastHit,
                100f
            );
            Room targetRoom = Synapse.Api.Map.Get.Rooms.OrderBy(r => Vector3.Distance(raycastHit.point, r.Position)).FirstOrDefault();


            builder.Append($"<size={TextSize}%>");

            
            bool isValidPlayer = Player != null && Player.RoleType != RoleType.Spectator && Player.RoleType != RoleType.None;

            string targetPosition = didRaycastHit ? raycastHit.point.ToString() : "None";
            string targetMapPoint = didRaycastHit && isValidPlayer && targetRoom != null ? new MapPoint(targetRoom, raycastHit.point).ToString() : "None";

            builder.AppendLine($"Target Position: {targetPosition}");
            builder.AppendLine($"Target MapPoint: {targetMapPoint}");
           
            string targetName = didRaycastHit ? raycastHit.transform.gameObject.name : "None";

            builder.AppendLine($"Target Name: {targetName}");
            
            string playerPosition = isValidPlayer ? Player.Position.ToString() : "None";
            string playerRotation = isValidPlayer ? Player.Rotation.x.ToString() : "NONE";
            string playerMapPoint = isValidPlayer ? Player.MapPoint.ToString() : "None";

            builder.AppendLine($"Player Position: {playerPosition}");
            builder.AppendLine($"Player Rotation: {playerRotation}");
            builder.AppendLine($"Player MapPoint: {playerMapPoint}");
            
            Room room = Player?.Room;

            string currentRoomName = isValidPlayer && room != null ? Player.Room.RoomName : "None";
            string currentRoomType = isValidPlayer && room != null ? Player.Room.RoomType.ToString() : "None";

            builder.AppendLine($"Current Room Name: {currentRoomName}");
            builder.AppendLine($"Current Room Type: {currentRoomType}");
        }
    }
}
