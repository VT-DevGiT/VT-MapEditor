using Synapse.Api.CustomObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapEditor
{
    public class Cursor
    {
        /// <summary>
        /// layer 3
        /// </summary>
        public const int layer = 1 << (int)VT_Api.Core.Enum.LayerID.Hitbox;
        const string KeyCursor = "Cursor";
        const string OtherValue = "Other";
        const string MainValue = "Main";

        static List<Cursor> Cursors = new List<Cursor>();

        public SynapseObject AttachedObject { get; set; }
        public SynapseObject MainObject { get; set; }

        public Cursor(SynapseObject @object)
        {
            AttachedObject = @object;
            MainObject = SchematicHandler.Get.SpawnSchematic(0, @object.Position + Vector3.down * 2, @object.Rotation);
            MainObject.GameObject.transform.parent = @object.GameObject.transform;
            MainObject.ObjectData.Add(KeyCursor, MainValue);

            foreach(var children in MainObject.PrimitivesChildrens)
            {
                foreach (var atribute in children.CustomAttributes)
                {
                    switch (atribute)
                    {
                        case "Cursor:Center":
                            if (!@object.ObjectData.ContainsKey(KeyCursor))
                                @object.ObjectData.Add(KeyCursor, Axis.Center);
                            break;
                        case "Cursor:X+1":
                            if (!@object.ObjectData.ContainsKey(KeyCursor))
                                @object.ObjectData.Add(KeyCursor, Axis.Right);
                            break;
                        case "Cursor:Y+1":
                            if (!@object.ObjectData.ContainsKey(KeyCursor))
                                @object.ObjectData.Add(KeyCursor, Axis.Up);
                            break;
                        case "Cursor:Z+1":
                            if (!@object.ObjectData.ContainsKey(KeyCursor))
                                @object.ObjectData.Add(KeyCursor, Axis.Forward);
                            break;
                        case "Cursor:X-1":
                            if (!@object.ObjectData.ContainsKey(KeyCursor))
                                @object.ObjectData.Add(KeyCursor, Axis.Left);
                            break;
                        case "Cursor:Y-1":
                            if (!@object.ObjectData.ContainsKey(KeyCursor))
                                @object.ObjectData.Add(KeyCursor, Axis.Down);
                            break;
                        case "Cursor:Z-1":
                            if (!@object.ObjectData.ContainsKey(KeyCursor))
                                @object.ObjectData.Add(KeyCursor, Axis.Backward);
                            break;
                    }
                }
                if (!@object.ObjectData.ContainsKey(KeyCursor))
                    @object.ObjectData.Add(KeyCursor, OtherValue);
                @object.GameObject.layer = layer;
            }
        }

        public void Scale(Axis axis, int ammount)
        {
            Vector3 vectorToAdd;
            switch (axis)
            {
                case Axis.Up:
                    vectorToAdd = Vector3.up * ammount;
                    break;
                case Axis.Down:
                    vectorToAdd = Vector3.down * ammount;
                    break;
                case Axis.Left:
                    vectorToAdd = Vector3.left * ammount;
                    break;
                case Axis.Right:
                    vectorToAdd = Vector3.right * ammount;
                    break;
                case Axis.Forward:
                    vectorToAdd = Vector3.forward * ammount;
                    break;
                case Axis.Backward:
                    vectorToAdd = Vector3.back * ammount;
                    break;
                default:
                    return;
            }

            AttachedObject.Scale += vectorToAdd;
            MainObject.Scale -= vectorToAdd;
        }

        public void Move(Axis axis, int ammount)
        {
            Vector3 vectorToAdd;
            switch (axis)
            {
                case Axis.Up:
                    vectorToAdd = Vector3.up * ammount;
                    break;
                case Axis.Down:
                    vectorToAdd = Vector3.down * ammount;
                    break;
                case Axis.Left:
                    vectorToAdd = Vector3.left * ammount;
                    break;
                case Axis.Right:
                    vectorToAdd = Vector3.right * ammount;
                    break;
                case Axis.Forward:
                    vectorToAdd = Vector3.forward * ammount;
                    break;
                case Axis.Backward:
                    vectorToAdd = Vector3.back * ammount;
                    break;
                default:
                    return;
            }

            AttachedObject.Position += vectorToAdd;
        }

        public void Rotate(Axis axis, int ammount)
        {
            Quaternion rotationToAdd; 

            switch(axis)
            {
                case Axis.Up:
                    rotationToAdd = Quaternion.Euler(Vector3.up * ammount);
                    break;
                case Axis.Down:
                    rotationToAdd = Quaternion.Euler(Vector3.down * ammount);
                    break;
                case Axis.Left:
                    rotationToAdd = Quaternion.Euler(Vector3.left * ammount);
                    break;
                case Axis.Right:
                    rotationToAdd = Quaternion.Euler(Vector3.right * ammount);
                    break;
                case Axis.Forward:
                    rotationToAdd = Quaternion.Euler(Vector3.forward * ammount);
                    break;
                case Axis.Backward:
                    rotationToAdd = Quaternion.Euler(Vector3.back * ammount);
                    break;
                default:
                    return;
            }

            AttachedObject.Rotation *= rotationToAdd;
        }
        
        public static bool TryInteract(DefaultSynapseObject @object, InteractionType interaction, int amount, out string answer, out Cursor cursor)
        {
            if (!IsObjectIsCursor(@object))
            {
                answer = "You need to interact with a cursor";
                cursor = null;
                return false;
            }

            if (@object.ObjectData[KeyCursor] as string == OtherValue)
            {
                answer = "You can't interact with this";
                cursor = null;
                return false;
            }

            if (!TryGetCursor(@object, out cursor))
            {
                answer = "This cursor is not referenced ! (bug contact the Dev and explain)";
                return false;

            }

            if (@object.ObjectData[KeyCursor] as string == MainValue && interaction != InteractionType.Selector && interaction != InteractionType.Destroy)
            {
                answer = $"Cursor info | ID : {cursor.MainObject.ID}, Name {cursor.MainObject.ID}" +
                     $"\nPostion {cursor.MainObject.ID}, Rotation {cursor.MainObject.Rotation}, Scale {cursor.MainObject.Scale}";
                return true;
            }

            switch (interaction)
            {
                case InteractionType.Scale:
                    {
                        if (@object.ObjectData[KeyCursor] is Axis axis)
                        {
                            cursor.Scale(axis, amount);
                            answer = "Object Scaled";
                            return true;
                        }
                        else
                        {
                            answer = "is not an Axis ! (bug contact the Dev and explain)";
                            return false;
                        }
                    }
                case InteractionType.Rotation:
                    {
                        if (@object.ObjectData[KeyCursor] is Axis axis)
                        {
                            cursor.Rotate(axis, amount);
                            answer = "Object rotationed";
                            return true;
                        }
                        else
                        {
                            answer = "is not an Axis ! (bug contact the Dev and explain)";
                            return false;
                        }
                    }
                case InteractionType.Mover:
                    {
                        if (@object.ObjectData[KeyCursor] is Axis axis)
                        {
                            cursor.Move(axis, amount);
                            answer = "Object moved";
                            return true;
                        }
                        else
                        {
                            answer = "is not an Axis ! (bug contact the Dev and explain)";
                            return false;
                        }
                    }
                case InteractionType.Destroy:
                    cursor.MainObject.Destroy();
                    answer = "Object Destroyed";
                    return true;
                case InteractionType.Selector:
                    answer = "Object Selected";
                    return true;                    
                default:
                    answer = "Unknow Interaction";
                    return false;
            }
        }
        
        public static bool IsObjectIsCursor(ISynapseObject @object)
        {
            return @object.ObjectData.ContainsKey(KeyCursor);
        }

        public static bool TryGetCursor(DefaultSynapseObject @object, out Cursor cursor)
        {
            if (!IsObjectIsCursor(@object))
            {
                cursor = null;
                return false;
            }

            if (@object.ObjectData[KeyCursor] as string == MainValue)
            {
                cursor = Cursors.FirstOrDefault(c => c.MainObject == @object);
                return cursor != null;
            }
            else
            {
                cursor = Cursors.FirstOrDefault(c => c.MainObject == @object.Parent); 
                return cursor != null;
            }
        }
    }
}
