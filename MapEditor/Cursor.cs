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
        public const int layer = 9;
        public const string KeyCursor = "Cursor";

        static List<Cursor> Cursors = new List<Cursor>();

        public SynapseObject AttachedObject { get; set; }
        public SynapseObject MainObject { get; set; }

        public Cursor(SynapseObject @object)
        {
            Cursors.Add(this);
            AttachedObject = @object;
            MainObject = SchematicHandler.Get.SpawnSchematic(0, @object.Position + Vector3.down * 2, @object.Rotation);
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

        public static Cursor GetCursor(DefaultSynapseObject @object)
        {
            if (@object.ObjectData[KeyCursor] is Axis axis && axis == Axis.Main)
                return Cursors.FirstOrDefault(c => c.MainObject == @object);
            else
                return Cursors.FirstOrDefault(c => c.MainObject == @object.Parent);
        }
    }
}
