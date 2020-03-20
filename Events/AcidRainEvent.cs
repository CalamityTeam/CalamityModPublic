using CalamityMod.ILEditing;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;

namespace CalamityMod.Events
{
	public class AcidRainEvent
	{
        // Make sure this does not interfere with any vanilla values. An obscure and random number is used just in case
        // Any other mods are changing the invasion ID as well.
        public const int InvasionID = 57;

        // A partially bright pale-ish cyan with a hint of yellow.
        public static readonly Color TextColor = new Color(115, 194, 147);

        // Not readonly so that if anyone else wants to add stuff in here with their own mod, they can.
        // The first value is the NPC type, the second is the value they're worth in the event
        public static List<(int, int)> PossibleEnemiesPreHM = new List<(int, int)>()
        {
            ( ModContent.NPCType<Radiator>(), 0 ),
            ( ModContent.NPCType<NuclearToad>(), 1 ),
            ( ModContent.NPCType<AcidEel>(), 1 ),
            ( ModContent.NPCType<Skyfin>(), 1 ),
            ( ModContent.NPCType<WaterLeech>(), 1 )
        };

        // Not readonly so that if anyone else wants to add stuff in here with their own mod, they can.
        // The first value is the NPC type, the second is the value they're worth in the event
        public static List<(int, int)> PossibleEnemiesAS = new List<(int, int)>()
        {
            ( ModContent.NPCType<Radiator>(), 0 ),
            ( ModContent.NPCType<NuclearToad>(), 0 ),
            ( ModContent.NPCType<AcidEel>(), 0 ),
            ( ModContent.NPCType<Orthocera>(), 1 ),
            ( ModContent.NPCType<IrradiatedSlime>(), 1 ),
            ( ModContent.NPCType<WaterLeech>(), 1 ),
            ( ModContent.NPCType<Skyfin>(), 1 ),
            ( ModContent.NPCType<Trilobite>(), 1 ),
            ( ModContent.NPCType<FlakCrab>(), 1 ),
            ( ModContent.NPCType<SulfurousSkater>(), 1 )
        };

        /// <summary>
        /// Broadcasts some text from a given localization key.
        /// </summary>
        /// <param name="localizationKey">The key to write</param>
        public static void BroadcastEventText(string localizationKey)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(localizationKey), TextColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(localizationKey), TextColor);
            }
        }
        /// <summary>
        /// Checks if the player's screen is in the view of the invasion, based on <paramref name="rectangleCheckSize"/>.
        /// </summary>
        /// <param name="rectangleCheckSize">The area to check based on the screen.</param>
        /// <param name="iconID">The ID for the icon. For more info on how this works, check <see cref="ILChanges.Initialize"/>.</param>.
        /// <returns></returns>
        public static bool NearInvasionCheck(int rectangleCheckSize)
        {
            Rectangle screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active)
                {
                    int type = Main.npc[i].type;
                    List<(int, int)> PossibleEnemies = CalamityWorld.downedAquaticScourge ? PossibleEnemiesAS : PossibleEnemiesPreHM;
                    if (PossibleEnemies.Select(enemy => enemy.Item2).Contains(type))
                    {
                        Rectangle invasionCheckArea = new Rectangle((int)Main.npc[i].Center.X - rectangleCheckSize / 2, (int)Main.npc[i].Center.Y - rectangleCheckSize / 2,
                            rectangleCheckSize, rectangleCheckSize);
                        if (screen.Intersects(invasionCheckArea))
                        {
                            Main.invasionProgressDisplayLeft = 480;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Attempts to start the Acid Rain event. Will fail if there is another invasion going on or the EoC has not been killed yet (unless you're in hardmode).
        /// </summary>
        public static void TryStartEvent()
        {
            if (CalamityWorld.rainingAcid || (!NPC.downedBoss1 && !Main.hardMode))
                return;
            int playerCount = 0;
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].active)
                {
                    playerCount++;
                }
            }
            if (playerCount > 0)
            {
                // So that vanilla doesn't shit itself
                Main.invasionType = -1;

                CalamityWorld.rainingAcid = true;
                // The E - 1 part is to ensure that we start at 1 as a multiple instead of 0
                // At a maximum of 255 players, the max multiplier is 9.98, or 998 enemies that need to be killed.
                CalamityWorld.acidRainPoints = (int)(180 * Math.Log(playerCount + Math.E - 1));
                Main.invasionProgress = 0;
                Main.invasionProgressIcon = -1;
                Main.invasionProgressWave = 0;

                // Make it rain normally
                Main.raining = true;

                // If abyssSide is true, then the abyss was generated on the left side of the world.
                // If false, the right.
                Main.invasionX = CalamityWorld.abyssSide ? 0 : Main.maxTilesX;
            }
            UpdateInvasion();
            BroadcastEventText("Mods.CalamityMod.AcidRainStart"); // A toxic downpour falls over the wasteland seas!
        }
        /// <summary>
        /// Updates the invasion, checking to see if it has ended.
        /// </summary>
        public static void UpdateInvasion(bool win = true)
        {
            // If the custom invasion is up
            if (CalamityWorld.rainingAcid)
            {
                // End invasion if we've defeated all the enemies, including Old Duke
                if (CalamityWorld.acidRainPoints <= 0)
                {
                    CalamityWorld.rainingAcid = false;
                    BroadcastEventText("Mods.CalamityMod.AcidRainEnd"); // The sulphuric skies begin to clear...
                    Main.invasionType = 0;
                    Main.invasionDelay = 0;
                    Main.invasionProgressNearInvasion = false;
                    CalamityWorld.acidRainExtraDrawTime = 40;

                    // Turn off the rain from the event
                    Main.numCloudsTemp = Main.rand.Next(5, 20 + 1);
                    Main.numClouds = Main.numCloudsTemp;
                    Main.windSpeedTemp = Main.rand.NextFloat(0.04f, 0.25f);
                    Main.windSpeedSet = Main.windSpeedTemp;
                    Main.maxRaining = 0f;
                    if (win)
                    {
                        CalamityWorld.downedEoCAcidRain = true;
                        CalamityWorld.downedAquaticScourgeAcidRain = CalamityWorld.downedAquaticScourge;
                    }
                    CalamityMod.StopRain();
                }
                CalamityMod.UpdateServerBoolean();

                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.AcidRainSync);
                    netMessage.Write(CalamityWorld.rainingAcid);
                    netMessage.Write(CalamityWorld.acidRainPoints);
                    netMessage.Send();
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.AcidRainUIDrawFadeSync);
                    netMessage.Write(CalamityWorld.acidRainExtraDrawTime);
                    netMessage.Send();
                }
            }
        }
    }
}
