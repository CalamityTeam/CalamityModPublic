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

        public const int MinibossDeathValue = 5;

        // A partially bright pale-ish cyan with a hint of yellow.
        public static readonly Color TextColor = new Color(115, 194, 147);

        // Not readonly so that if anyone else wants to add stuff in here with their own mod, they can.
        // The first value is the NPC type, the second is the value they're worth in the event.
        // The third value is whether the enemy can only spawn in water
        public static List<(int, int, bool)> PossibleEnemiesPreHM = new List<(int, int, bool)>()
        {
            ( ModContent.NPCType<Radiator>(), 1, true ),
            ( ModContent.NPCType<NuclearToad>(), 1, false ),
            ( ModContent.NPCType<AcidEel>(), 1, true ),
            ( ModContent.NPCType<Skyfin>(), 1, true ),
            ( ModContent.NPCType<WaterLeech>(), 1, true )
        };

        public static List<(int, int, bool)> PossibleEnemiesAS = new List<(int, int, bool)>()
        {
            ( ModContent.NPCType<Radiator>(), 0, true ),
            ( ModContent.NPCType<NuclearToad>(), 0, false ),
            ( ModContent.NPCType<AcidEel>(), 0, true ),
            ( ModContent.NPCType<Orthocera>(), 1, true ),
            ( ModContent.NPCType<IrradiatedSlime>(), 1, false ),
            ( ModContent.NPCType<WaterLeech>(), 1, true ),
            ( ModContent.NPCType<Skyfin>(), 1, true ),
            ( ModContent.NPCType<Trilobite>(), 1, true ),
            ( ModContent.NPCType<FlakCrab>(), 1, false ),
            ( ModContent.NPCType<SulfurousSkater>(), 1, false )
        };

        public static List<(int, int, bool)> PossibleEnemiesPolter = new List<(int, int, bool)>()
        {
            ( ModContent.NPCType<Radiator>(), 0, true ),
            ( ModContent.NPCType<NuclearToad>(), 0, false ),
            ( ModContent.NPCType<AcidEel>(), 0, true ),
            ( ModContent.NPCType<Orthocera>(), 1, true ),
            ( ModContent.NPCType<GammaSlime>(), 1, false ),
            ( ModContent.NPCType<WaterLeech>(), 1, true ),
            ( ModContent.NPCType<Skyfin>(), 1, true ),
            ( ModContent.NPCType<Trilobite>(), 1, true ),
            ( ModContent.NPCType<FlakCrab>(), 1, false ),
            ( ModContent.NPCType<SulfurousSkater>(), 1, false )
        };

        public static List<int> PossibleMinibossesAS = new List<int>()
        {
            ModContent.NPCType<CragmawMire>()
        };

        public static List<int> PossibleMinibossesPolter = new List<int>()
        {
            ModContent.NPCType<CragmawMire>()
        };

        public static bool AnyRainMinibosses
        {
            get
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && (PossibleMinibossesAS.Contains(Main.npc[i].type)) || PossibleMinibossesPolter.Contains(Main.npc[i].type))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

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
                    List<(int, int, bool)> PossibleEnemies = PossibleEnemiesPreHM;
                    if (CalamityWorld.downedAquaticScourge)
                        PossibleEnemies = PossibleEnemiesAS;
                    if (CalamityWorld.downedPolterghast)
                        PossibleEnemies = PossibleEnemiesPolter;
                    if (PossibleEnemies.Select(enemy => enemy.Item2).Contains(type))
                    {
                        Rectangle invasionCheckArea = new Rectangle((int)Main.npc[i].Center.X - rectangleCheckSize / 2, (int)Main.npc[i].Center.Y - rectangleCheckSize / 2,
                            rectangleCheckSize, rectangleCheckSize);
                        if (screen.Intersects(invasionCheckArea))
                        {
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
            if (CalamityWorld.rainingAcid || (!NPC.downedBoss1 && !Main.hardMode) || CalamityWorld.bossRushActive)
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
                CalamityWorld.rainingAcid = true;
                // The E - 1 part is to ensure that we start at 1 as a multiple instead of 0
                // At a maximum of 255 players, the max multiplier is 9.98, or 998 enemies that need to be killed.
                CalamityWorld.acidRainPoints = (int)(180 * Math.Log(playerCount + Math.E - 1));

                // Make it rain normally
				if (CalamityWorld.startAcidicDownpour)
				{
					Main.raining = true;
				}
                CalamityWorld.triedToSummonOldDuke = false;
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
                    CalamityWorld.triedToSummonOldDuke = false;
                    CalamityMod.StopRain();
                }
                CalamityMod.UpdateServerBoolean();

                // You will be tempted to turn this into a single if conditional.
                // Don't do this. Doing so has caused so much misery, with various things being read instead
                // of the correct thing, look booleans being mixed up in the sending and receiving process.
                // In short, leave this alone.
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
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.AcidRainOldDukeSummonSync);
                    netMessage.Write(CalamityWorld.triedToSummonOldDuke);
                    netMessage.Send();
                }
            }
        }
    }
}
