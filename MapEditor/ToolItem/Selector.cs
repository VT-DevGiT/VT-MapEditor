using MapGeneration;
using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Enum;
using System.Linq;
using UnityEngine;
using VT_Api.Core.Enum;
using VT_Api.Core.Items;
using VT_Api.Extension;

namespace MapEditor.ToolItem
{
    [VtItemInformation(
       BasedItemType = ItemType.GunRevolver,
       ID = (int)ItemID.Selector,
       Name = "Selector"
       )]
    public class Selector : AbstractWeapon, ITool
    {
        public const int ModMax = 5;

        public override ushort MaxAmmos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;


        private int selected;
        public int Selected
        {
            get => selected;
            set
            {
                if (value + selected < 0)
                    selected = ModMax;
                else if (ModMax < value)
                    selected = 0;
                else
                    selected = value;
            }
        }

        string ITool.Info
        {
            get
            {
                var playerSlectedAnObject = Plugin.Instance.PlayerSlectedObject.TryGetValue(Holder, out var SlectedObject) && SlectedObject != null;

                switch (Selected)
                {
                    case 0 when !playerSlectedAnObject:
                        return $"Fire on a cursor for select the Object";
                    case 0:
                        return $"Selected object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 1 when playerSlectedAnObject:
                        return $"Move the slected Object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 2 when playerSlectedAnObject:
                        return $"Copy the seleted Object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 3 when playerSlectedAnObject:
                        return $"Change the Referent room of the Object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 4 when playerSlectedAnObject:
                        return $"Change the Attached Type room type of the Object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 5 when playerSlectedAnObject:
                        return $"Set to constant position the object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";

                    default:
                        return "You need to select an Object !";
                }
            }
        }

        public override bool Realod()
        {
            Item.Durabillity = MaxAmmos;
            return false;
        }

        public override void Init()
        {
            Item.Durabillity = MaxAmmos;
        }

        public void Select(MapEditUI handler)
        {
            if (!Physics.Raycast(Holder.CameraReference.transform.position, Holder.CameraReference.transform.forward, out RaycastHit hitInfo, 50f))
                handler.Info = "<color=#FF0000>nothing found</color>";
            else
            {
                var compnts = hitInfo.collider.GetComponentsInParent(typeof(Component));

                var primitiveObject = compnts.FirstOrDefault(c => c is SynapseObjectScript) as SynapseObjectScript;

                if (primitiveObject == null || primitiveObject.Object is not DefaultSynapseObject defaultSynapseObject)
                {
                    handler.Info = "<color=#FF0000>You need to interact with a cursor</color>";
                    return;
                }

                if (!Cursor.TryGetCursor(defaultSynapseObject, out var cursor))
                {
                    handler.Info = "<color=#FF0000>You need to interact with a cursor</color>";
                }
                else
                {
                    if (!Plugin.Instance.PlayerSlectedObject.ContainsKey(Holder))
                        Plugin.Instance.PlayerSlectedObject.Add(Holder, cursor);
                    else
                        Plugin.Instance.PlayerSlectedObject[Holder] = cursor;
                    handler.Info = "Object Selected";
                }
            }
            return;
        }

        public void Move(MapEditUI handler, Vector3 newPostion)
        {
            if (!Plugin.Instance.PlayerSlectedObject.ContainsKey(Holder) || Plugin.Instance.PlayerSlectedObject[Holder] == null)
            {
                handler.Info = "<color=#FF0000>You need to select an object</color>";
                return;
            }

            Plugin.Instance.PlayerSlectedObject[Holder].AttachedObject.Position = newPostion;
        }

        public void Copy(MapEditUI handler, Vector3 postion)
        {
            if (!Plugin.Instance.PlayerSlectedObject.ContainsKey(Holder) || Plugin.Instance.PlayerSlectedObject[Holder] == null)
            {
                handler.Info = "<color=#FF0000>You need to select an object</color>";
                return;
            }

            var selected = Plugin.Instance.PlayerSlectedObject[Holder].AttachedObject;
            var schematic = SchematicHandler.Get.SpawnSchematic(selected.ID, Holder.Position, selected.Rotation);
            new Cursor(schematic, Plugin.Instance.CurentEditedMap);

            schematic.ObjectData.Add(Plugin.ObjectKeyMap, Plugin.Instance.CurentEditedMap.Name);
            schematic.ObjectData.Add(Plugin.ObjectKeyRoom, selected.ObjectData[Plugin.ObjectKeyRoom]);

            Plugin.Instance.EditingObjects.Add(schematic);
        }

        public void ChangeRoom(MapEditUI handler, int roomSetType)
        {
            if (!Plugin.Instance.PlayerSlectedObject.ContainsKey(Holder) || Plugin.Instance.PlayerSlectedObject[Holder] == null)
            {
                handler.Info = "<color=#FF0000>You need to select an object</color>";
                return;
            }

            var data = Plugin.Instance.PlayerSlectedObject[Holder].AttachedObject.ObjectData;

            if (roomSetType == 2)
            {
                data[Plugin.ObjectKeyRoom] = Plugin.Fixed;
                handler.Info = "Fixed set";
                return;
            }

            if (!Physics.Raycast(Holder.CameraReference.transform.position, Holder.CameraReference.transform.forward, out RaycastHit hitInfo, 50f))
                handler.Info = "<color=#FF0000>nothing found</color>";
            else
            {
                var compnts = hitInfo.collider.GetComponentsInParent(typeof(Component));

                var room = compnts.FirstOrDefault(c => c.GetComponent<RoomIdentifier>() != null)?.GetComponent<RoomIdentifier>().GetSynapseRoom();

                if (room == null)
                {
                    handler.Info = "<color=#FF0000>You need to interact with a room</color>";
                    return;
                }
                if (roomSetType == 0)
                    data[Plugin.ObjectKeyRoom] = room;
                else
                    data[Plugin.ObjectKeyRoom] = room.RoomName;

                handler.Info = "Room set";
            }
        }

        public override bool Shoot(Vector3 targetPosition, Player target) => false;

        public override bool Shoot(Vector3 targetPosition)
        {
            MapEditUI handler;
            if (Plugin.Instance.CurentEditedMap == null || Plugin.Instance.CurentEditedMap.Name == Plugin.MapNone)
            {
                Holder.SendBroadcast(2, "<color=red>You are not in edit mod</color>", true);
                return false;
            }
            else
            {
                handler = Holder.GetOrAddComponent<MapEditUI>();
                handler.UIRuning = true;
            }
            switch (Selected)
            {
                case 0:
                    Select(handler);
                    break;
                case 1:
                    Move(handler, targetPosition);
                    break;
                case 2:
                    Copy(handler, targetPosition);
                    break;
                case 3:
                case 4:
                case 5:
                    ChangeRoom(handler, Selected - 3);
                    break;
                    //TODO
            }
            return false;
        }
    }
}
