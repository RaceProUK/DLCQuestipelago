﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BepInEx.Logging;
using DLCLib.Physics;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace DLCQuestipelago.AntiCrashes
{
    [HarmonyPatch(typeof(PhysicsManager))]
    [HarmonyPatch("PerformStep")]
    public static class PhysicsManagerPerformStepPatch
    {
        private static readonly Vector2 MAX_VELOCITY = new Vector2(30f);

        private static ManualLogSource _log;

        public static void Initialize(ManualLogSource log)
        {
            _log = log;
        }

        // protected void PerformStep(float dt)
        private static bool Prefix(PhysicsManager __instance, float dt)
        {
            try
            {
                var dynamicPhysicalEntitiesField = typeof(PhysicsManager).GetField("dynamicPhysicalEntities", BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var dynamicPhysicalEntity in (List<IPhysical>)dynamicPhysicalEntitiesField.GetValue(__instance))
                {
                    var physicsObject1 = dynamicPhysicalEntity.GetPhysicsObject();
                    physicsObject1.Velocity += physicsObject1.Acceleration * dt;
                    if (!physicsObject1.IgnoreGravity)
                    {
                        physicsObject1.Velocity += (physicsObject1.UseSlowGravity ? PhysicsManager.SLOW_GRAVITY : PhysicsManager.GRAVITY) * dt;
                    }

                    if (physicsObject1.IsOnGround)
                    {
                        physicsObject1.Velocity.X *= (float)(1.0 - (double)physicsObject1.GroundFriction * (double)dt);
                    }
                    else
                    {
                        physicsObject1.Velocity.X *= (float)(1.0 - (double)physicsObject1.AirFriction * (double)dt);
                    }

                    physicsObject1.Velocity = Vector2.Clamp(physicsObject1.Velocity, -MAX_VELOCITY, MAX_VELOCITY);
                    var physicsObject2 = physicsObject1;
                    physicsObject2.Position += physicsObject1.Velocity * dt;
                    physicsObject1.Acceleration = Vector2.Zero;
                    physicsObject1.IsOnGround = false;
                    physicsObject1.IsAtCeiling = false;
                    physicsObject1.IsOnWall = false;
                }

                var doCollisionMethod = typeof(PhysicsManager).GetMethod("DoCollision", BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var dynamicPhysicalEntity in (List<IPhysical>)dynamicPhysicalEntitiesField.GetValue(__instance))
                {
                    var physicsObject = dynamicPhysicalEntity.GetPhysicsObject();
                    if (!physicsObject.IsActive) continue;
                    doCollisionMethod.Invoke(__instance, new[] {physicsObject});
                    physicsObject.OffGroundTime += dt;
                    physicsObject.OffWallTime += dt;
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _log.LogError($"Failed in {nameof(PhysicsManagerPerformStepPatch)}.{nameof(Prefix)}:\n\t{ex}");
                Debugger.Break();
                return false; // don't run original logic
            }
        }
    }
}
