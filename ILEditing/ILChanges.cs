using CalamityMod.Tiles.DraedonStructures;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public class ILChanges
    {
        // This list should contain all vanilla NPCs present in Boss Rush which ARE NOT bosses and whose health is boosted over 32,768.
        private static readonly List<int> NeedsFourLifeBytes = new List<int>()
        {
            NPCID.EaterofWorldsHead,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,
            NPCID.Creeper,
            NPCID.WallofFleshEye,
        };

        // Holds the vanilla game function which spawns town NPCs, wrapped in a delegate for reflection purposes.
        // This function is (optionally) invoked manually in an IL edit to enable NPCs to spawn at night.
        private static Action VanillaSpawnTownNPCs;

        private static int labDoorOpen = -1;
        private static int labDoorClosed = -1;
        private static int aLabDoorOpen = -1;
        private static int aLabDoorClosed = -1;

        /// <summary>
        /// Loads all IL Editing changes in the mod.
        /// </summary>
        internal static void Load()
        {
            // Wrap the vanilla town NPC spawning function in a delegate so that it can be tossed around and called at will.
            var updateTime = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.Static | BindingFlags.NonPublic);
            VanillaSpawnTownNPCs = Delegate.CreateDelegate(typeof(Action), updateTime) as Action;

            // Cache the four lab door tile types for efficiency.
            labDoorOpen = ModContent.TileType<LaboratoryDoorOpen>();
            labDoorClosed = ModContent.TileType<LaboratoryDoorClosed>();
            aLabDoorOpen = ModContent.TileType<AgedLaboratoryDoorOpen>();
            aLabDoorClosed = ModContent.TileType<AgedLaboratoryDoorClosed>();

            ApplyLifeBytesChanges();
            ApplyBossZenDuringSlimeRain();
            SlideDungeonOver();
            BlockLivingTreesNearOcean();
            LabDoorFixes();
            AlterTownNPCSpawnRate();
        }

        /// <summary>
        /// Currently mostly useless, but clears static variables.
        /// </summary>
        internal static void Unload()
        {
            VanillaSpawnTownNPCs = null;
            labDoorOpen = labDoorClosed = aLabDoorOpen = aLabDoorClosed = -1;
        }

        #region IL Editing Routines
        private static void ApplyLifeBytesChanges() => On.Terraria.Main.InitLifeBytes += BossRushLifeBytes;

        private static void ApplyBossZenDuringSlimeRain() => On.Terraria.NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;

        private static void SlideDungeonOver()
        {
            IL.Terraria.WorldGen.MakeDungeon += (il) =>
            {
                var cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(i => i.MatchStsfld("Terraria.WorldGen", "dMaxY")))
                {
                    CalamityMod.Instance.Logger.Warn("Dungeon movement editing code failed.");
                    return;
                }
                cursor.Index++;
                cursor.EmitDelegate<Action>(() =>
                {
                    WorldGen.dungeonX += (WorldGen.dungeonX < Main.maxTilesX / 2).ToDirectionInt() * 450;
                    WorldGen.dungeonX = Utils.Clamp(WorldGen.dungeonX, 200, Main.maxTilesX - 200);
                });
            };
        }

        private static void BlockLivingTreesNearOcean()
        {
            IL.Terraria.WorldGen.GrowLivingTree += (il) =>
            {
                var cursor = new ILCursor(il);
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Func<int, int>>(x => Utils.Clamp(x, 560, Main.maxTilesX - 560));
                cursor.Emit(OpCodes.Starg, 0);
            };
        }

        private static void LabDoorFixes()
        {
            On.Terraria.WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
        }

        private static void AlterTownNPCSpawnRate()
        {
            IL.Terraria.Main.UpdateTime += (il) =>
            {
                // Don't do town NPC spawning at the end (after a !Main.dayTime return).
                // Do it at the beginning in spite of time.

                var cursor = new ILCursor(il);
                cursor.EmitDelegate<Action>(() =>
                {
                    // A cached delegate is used here instead of direct reflection for performance reasons
                    // since UpdateTime is called every frame.
                    if (Main.dayTime || CalamityConfig.Instance.CanTownNPCsSpawnAtNight)
                        VanillaSpawnTownNPCs();
                });

                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>("UpdateTime_SpawnTownNPCs")))
                {
                    CalamityMod.Instance.Logger.Warn("Town NPC spawn editing code failed.");
                    return;
                }

                cursor.Emit(OpCodes.Ret);
            };

            On.Terraria.Main.UpdateTime_SpawnTownNPCs += (orig) =>
            {
                int oldWorldRate = Main.worldRate;
                Main.worldRate *= CalamityConfig.Instance.TownNPCSpawnRateMultiplier;
                orig();
                Main.worldRate = oldWorldRate;
            };
        }
        #endregion

        #region IL Editing Injected/Hooked Functions
        private static void BossRushLifeBytes(On.Terraria.Main.orig_InitLifeBytes orig)
        {
            orig();
            foreach (int npcType in NeedsFourLifeBytes)
                Main.npcLifeBytes[npcType] = 4;
        }

        private static void PreventBossSlimeRainSpawns(On.Terraria.NPC.orig_SlimeRainSpawns orig, int plr)
        {
            if (!Main.player[plr].Calamity().bossZen)
                orig(plr);
        }

        private static bool OpenDoor_LabDoorOverride(On.Terraria.WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            Tile tile = Main.tile[i, j];
            // If the tile is somehow null, that's vanilla's problem, we're outta here
            if (tile is null)
                return orig(i, j, direction);

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            else if (tile.type == labDoorClosed)
                return OpenLabDoor(tile, i, j, labDoorOpen);
            else if (tile.type == aLabDoorClosed)
                return OpenLabDoor(tile, i, j, aLabDoorOpen);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, direction);
        }

        private static bool CloseDoor_LabDoorOverride(On.Terraria.WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
        {
            Tile tile = Main.tile[i, j];
            // If the tile is somehow null, that's vanilla's problem, we're outta here
            if (tile is null)
                return orig(i, j, forced);

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            else if (tile.type == labDoorOpen)
                return CloseLabDoor(tile, i, j, labDoorClosed);
            else if (tile.type == aLabDoorOpen)
                return CloseLabDoor(tile, i, j, aLabDoorClosed);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, forced);
        }
        #endregion

        #region Helper Functions
        private static int FindTopOfDoor(int i, int j, Tile rootTile)
        {
            Tile t = Main.tile[i, j];
            int topY = j;
            while(t != null && t.active() && t.type == rootTile.type)
            {
                // Immediately stop at the top of the world, if you got there somehow.
                if (topY == 0)
                    return topY;
                // Go up one space and re-assign the current tile.
                --topY;
                t = Main.tile[i, topY];
            }
            
            // The above loop will have gone 1 past the top of the door. Correct for this.
            return ++topY;
        }

        private static bool OpenLabDoor(Tile tile, int i, int j, int openID)
        {
            int topY = FindTopOfDoor(i, j, tile);
            return DirectlyTransformLabDoor(i, topY, openID);
        }

        private static bool CloseLabDoor(Tile tile, int i, int j, int closedID)
        {
            int topY = FindTopOfDoor(i, j, tile);
            return DirectlyTransformLabDoor(i, topY, closedID);
        }

        private static bool DirectlyTransformLabDoor(int doorX, int doorY, int newDoorID, int wireHitY = -1)
        {
            // Transform the door one tile at a time.
            // If applicable, skip wiring for all door tiles except the one that was hit by this wire event.
            for (int y = doorY; y < doorY + 4; ++y)
            {
                Main.tile[doorX, y].type = (ushort)newDoorID;
                if (Main.netMode != NetmodeID.MultiplayerClient && Wiring.running && y != wireHitY)
                    Wiring.SkipWire(doorX, y);
            }

            // Second pass: TileFrame all those positions, which will sync in multiplayer if applicable
            for (int y = doorY; y < doorY + 4; ++y)
                WorldGen.TileFrame(doorX, y);

            // Play the door closing sound (lab doors do not use the door opening sound)
            Main.PlaySound(SoundID.DoorClosed, doorX * 16, doorY * 16);
            return true;
        }
        #endregion
    }
}
