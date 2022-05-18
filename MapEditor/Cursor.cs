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
        /// layer 9
        /// </summary>
        public const int layer = 9;
        public const string KeyCursor = "Cursor";

        static List<Cursor> Cursors { get; } = new List<Cursor>();
        static int HigherId { get; set; } = 0;

        public SynapseObject AttachedObject { get; set; }
        public SynapseObject MainObject { get; set; }

        public Map Map { get; set; }


        public int ID { get; private set; }

        public Cursor(SynapseObject @object, Map map)
        {
            Cursors.Add(this);
            AttachedObject = @object;
            MainObject = SchematicHandler.Get.SpawnSchematic(0, @object.Position + Vector3.down * 2, new Quaternion(0,0,0,0));
            MainObject.GameObject.transform.parent = @object.GameObject.transform;
            MainObject.ObjectData.Add(KeyCursor, Axis.Main);

            foreach(var children in MainObject.PrimitivesChildrens)
            {
                foreach (var atribute in children.CustomAttributes)
                {
                    switch (atribute.ToLower())
                    {
                        case "cursor:center":
                            if (!children.ObjectData.ContainsKey(KeyCursor))
                                children.ObjectData.Add(KeyCursor, Axis.Center);
                            break;
                        case "cursor:x+1":
                            if (!children.ObjectData.ContainsKey(KeyCursor))
                                children.ObjectData.Add(KeyCursor, Axis.Right);
                            break;
                        case "cursor:y+1":
                            if (!children.ObjectData.ContainsKey(KeyCursor))
                                children.ObjectData.Add(KeyCursor, Axis.Up);
                            break;
                        case "cursor:z+1":
                            if (!children.ObjectData.ContainsKey(KeyCursor))
                                children.ObjectData.Add(KeyCursor, Axis.Forward);
                            break;
                        case "cursor:x-1":
                            if (!children.ObjectData.ContainsKey(KeyCursor))
                                children.ObjectData.Add(KeyCursor, Axis.Left);
                            break;
                        case "cursor:y-1":
                            if (!children.ObjectData.ContainsKey(KeyCursor))
                                children.ObjectData.Add(KeyCursor, Axis.Down);
                            break;
                        case "cursor:z-1":
                            if (!children.ObjectData.ContainsKey(KeyCursor))
                                children.ObjectData.Add(KeyCursor, Axis.Backward);
                            break;
                    }
                }
                if (!children.ObjectData.ContainsKey(KeyCursor))
                    children.ObjectData.Add(KeyCursor, Axis.Other);
            }
            if (@object.Scale.y != 1)
                MainObject.Position += Vector3.down * @object.Scale.y / 4;

            HigherId++;
            ID = HigherId;

            Map = map;
        }

        public void Scale(Axis axis, float ammount)
        {
            ammount = ammount * (float)0.01;
            Vector3 vectorToAdd;
            switch (axis)
            {
                case Axis.Up:
                    {
                        var sizeVector = Vector3.down * ammount;
                        vectorToAdd = sizeVector;
                        MainObject.Position += sizeVector / 4;
                        break;
                    }
                case Axis.Down:
                    {
                        var sizeVector = Vector3.up * ammount;
                        vectorToAdd = sizeVector;
                        MainObject.Position += sizeVector / 4;
                        break;
                    }
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

            MainObject.GameObject.transform.parent = null;
            AttachedObject.Scale += vectorToAdd;
            MainObject.GameObject.transform.parent = AttachedObject.GameObject.transform;
        }

        public void Scale(Vector3 vector, bool add = true)
        {
            MainObject.GameObject.transform.parent = null;
            if (vector.y != 0)
                MainObject.Position += AttachedObject.Scale / vector.y;
            if (add)
                AttachedObject.Scale += vector;
            else
                AttachedObject.Scale = vector;
            MainObject.GameObject.transform.parent = AttachedObject.GameObject.transform;
        }

        public void Move(Axis axis, float ammount)
        {
            ammount = ammount * (float)0.01;
            Vector3 vectorToAdd;
            switch (axis)
            {
                case Axis.Up:
                    vectorToAdd = Vector3.up * ammount;
                    break;
                case Axis.Down:
                    vectorToAdd = Vector3.down * ammount;
                    break;
                case Axis.Forward:
                    vectorToAdd = Vector3.forward * ammount;
                    break;
                case Axis.Backward:
                    vectorToAdd = Vector3.back * ammount;
                    break;
                case Axis.Left:
                    vectorToAdd = Vector3.left * ammount;
                    break;
                case Axis.Right:
                    vectorToAdd = Vector3.right * ammount;
                    break;
                default:
                    return;
            }

            AttachedObject.Position += vectorToAdd;
        }

        public void Move(Vector3 vector, bool add = true)
        {
            if (add)
                AttachedObject.Position += vector;
            else
                AttachedObject.Position = vector;
        }

        public void Rotate(Axis axis, float ammount)
        {
            ammount = ammount * (float)0.1;
            Quaternion rotationToAdd; 

            switch(axis)
            {
                case Axis.Left:
                    rotationToAdd = Quaternion.Euler(Vector3.up * ammount);
                    break;
                case Axis.Right:
                    rotationToAdd = Quaternion.Euler(Vector3.down * ammount);
                    break;
                case Axis.Forward:
                    rotationToAdd = Quaternion.Euler(Vector3.left * ammount);
                    break;
                case Axis.Backward:
                    rotationToAdd = Quaternion.Euler(Vector3.right * ammount);
                    break;
                case Axis.Up:
                    rotationToAdd = Quaternion.Euler(Vector3.forward * ammount);
                    break;
                case Axis.Down:
                    rotationToAdd = Quaternion.Euler(Vector3.back * ammount);
                    break;
                default:
                    return;
            }

            AttachedObject.Rotation *= rotationToAdd;
        }

        public void Rotate(Vector3 vector, bool add = true)
        {
            if (add)
            {
                var addRotation = Quaternion.Euler(vector);
                AttachedObject.Rotation = new Quaternion(AttachedObject.Rotation.x + addRotation.x, AttachedObject.Rotation.y + addRotation.y,
                                                         AttachedObject.Rotation.z + addRotation.z, AttachedObject.Rotation.w + addRotation.w);
            }
            else
                AttachedObject.Rotation = Quaternion.Euler(vector);
        }

        public static bool CanInteract(DefaultSynapseObject @object, out string answer, bool needToBeAxis = false)
        {
            if (!IsObjectIsCursor(@object))
            {
                answer = "You need to interact with a cursor";
                return false;
            }

            if (@object.ObjectData[KeyCursor] is Axis axis && axis == Axis.Other && !needToBeAxis)
            {
                var cursor = GetCursor(@object);
                answer = $"Cursor info | ID : {cursor.MainObject.ID}, Name {cursor.MainObject.ID}, Postion {cursor.MainObject.ID}, Rotation {cursor.MainObject.Rotation}, Scale {cursor.MainObject.Scale}";
                return false;
            }

            answer = "Can interact";
            return true;
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

            if (@object.ObjectData[KeyCursor] is Axis axis && axis == Axis.Main)
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

        public static bool TryGetCursor(SynapseObject @object, out Cursor cursor)
        {
            cursor = Cursors.FirstOrDefault(c => c.AttachedObject == @object);
            return cursor != null;
        }

        public static bool TryGetCursor(int id, out Cursor cursor)
        {
            cursor = Cursors.FirstOrDefault(c => c.ID == id);
            return cursor != null;
        }

        public static Cursor GetCursor(DefaultSynapseObject @object)
        {
            if (@object.ObjectData[KeyCursor] is Axis axis && axis == Axis.Main)
                return Cursors.FirstOrDefault(c => c.MainObject == @object);
            else
                return Cursors.FirstOrDefault(c => c.MainObject == @object.Parent);
        }

        public static Cursor GetCursor(SynapseObject @object)
        {
            return Cursors.FirstOrDefault(c => c.MainObject == @object);
        }
        public static Cursor GetCursor(int id)
        {
            return Cursors.FirstOrDefault(c => c.ID == id);
        }

        public static void ResetID()
        {
            HigherId = 0;
        }
    }
}
