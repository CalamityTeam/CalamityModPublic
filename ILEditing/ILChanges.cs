using CalamityMod.Tiles.DraedonStructures;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
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
        
        /// <summary>
        /// Loads all IL Editing changes in the mod.
        /// </summary>
        public static void Initialize()
        {
            ApplyLifeBytesChanges();
            SlideDungeonOver();
            LabDoorFixes();
        }

        #region IL Editing Routines
        private static void ApplyLifeBytesChanges() => On.Terraria.Main.InitLifeBytes += BossRushLifeBytes;

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

        private static void LabDoorFixes()
        {
            On.Terraria.Player.TileInteractionsUse += Player_TileInteractionsUse;
            On.Terraria.WorldGen.OpenDoor += LabDoorsOpen;
            On.Terraria.WorldGen.CloseDoor += LabDoorsClose;
        }
        #endregion

        #region IL Editing Injection Functions
        private static void BossRushLifeBytes(On.Terraria.Main.orig_InitLifeBytes orig)
        {
            orig();
            foreach (int npcType in NeedsFourLifeBytes)
                Main.npcLifeBytes[npcType] = 4;
        }

        private static bool LabDoorsOpen(On.Terraria.WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            Tile tile = Main.tile[i, j];
            if (tile.type == ModContent.TileType<AgedLaboratoryDoorOpen>() || tile.type == ModContent.TileType<AgedLaboratoryDoorClosed>() ||
            tile.type == ModContent.TileType<LaboratoryDoorOpen>() || tile.type == ModContent.TileType<LaboratoryDoorClosed>())
            {
                return false;
            }
            else
            {
                return orig(i, j, direction);
            }
        }

        private static bool LabDoorsClose(On.Terraria.WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
        {
            Tile tile = Main.tile[i, j];
            if (tile.type == ModContent.TileType<AgedLaboratoryDoorOpen>() || tile.type == ModContent.TileType<AgedLaboratoryDoorClosed>() ||
            tile.type == ModContent.TileType<LaboratoryDoorOpen>() || tile.type == ModContent.TileType<LaboratoryDoorClosed>())
            {
                return false;
            }
            else
            {
                return orig(i, j, forced);
            }
        }

        private static void Player_TileInteractionsUse(On.Terraria.Player.orig_TileInteractionsUse orig, Player player, int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile.type == ModContent.TileType<AgedLaboratoryDoorOpen>())
            {
                DoorSwap(ModContent.TileType<AgedLaboratoryDoorClosed>(), ModContent.TileType<AgedLaboratoryDoorOpen>(), i, j);
            }
            else if (tile.type == ModContent.TileType<AgedLaboratoryDoorClosed>())
            {
                DoorSwap(ModContent.TileType<AgedLaboratoryDoorOpen>(), ModContent.TileType<AgedLaboratoryDoorClosed>(), i, j);
            }
            else if (tile.type == ModContent.TileType<LaboratoryDoorOpen>())
            {
                DoorSwap(ModContent.TileType<LaboratoryDoorClosed>(), ModContent.TileType<LaboratoryDoorOpen>(), i, j);
            }
            else if (tile.type == ModContent.TileType<LaboratoryDoorClosed>())
            {
                DoorSwap(ModContent.TileType<LaboratoryDoorOpen>(), ModContent.TileType<LaboratoryDoorClosed>(), i, j);
            }
            else
            {
                orig(player, i, j);
            }
        }
        #endregion

        #region Helper Functions
        public static void DoorSwap(int type1, int type2, int i, int j, bool forced = false)
        {
            if (PlayerInput.Triggers.JustPressed.MouseRight || forced)
            {
                ushort type = (ushort)type1;
                short frameY = 0;
                for (int dy = -4; dy < 4; dy++)
                {
                    if (Main.tile[i, j + dy].frameY > 0 && frameY == 0)
                        continue;
                    if (Main.tile[i, j + dy].type == type2)
                    {
                        if (Main.tile[i, j + dy] is null)
                        {
                            Main.tile[i, j + dy] = new Tile();
                        }
                        Main.tile[i, j + dy].type = type;
                        Main.tile[i, j + dy].frameY = frameY;
                        frameY += 16;
                        if ((int)frameY / 16 >= 4)
                            break;
                    }
                }

                Main.PlaySound(SoundID.DoorClosed, i * 16, j * 16, 1, 1f, 0f);
            }
        }
        #endregion
    }
}
