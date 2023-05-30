using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.UI.VanillaBossBars
{
    public class CeaselessVoidBossBar : ModBossBar
    {
        // Used to determine the max health of a multi-segmented boss
        public NPC FalseNPCSegment;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<CeaselessVoid>()]];

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC target = Main.npc[info.npcIndexToAimAt];
			if (!target.active)
				return false;

            // Get the boss health, obviously
            life = target.life;
            lifeMax = target.lifeMax;

            // Normal boss health
            float lifePercent = Utils.Clamp(life / lifeMax, 0f, 1f);
            
            // Reset the shield
            shield = 0f;
            shieldMax = 0f;

            // Determine the shield health
            // Amount of Dark Energies expected from each phase/difficulty
            int ExpectedBallsCounter = ((lifePercent <= 0.1f ? 3 : lifePercent <= 0.4f ? 2 : lifePercent <= 0.7f ? 1 : 0) + (CalamityWorld.death ? 6 : CalamityWorld.revenge ? 5 : Main.expertMode ? 4 : 3)) * (Main.getGoodWorld ? 6 : 3) + 2;
            // The Dark Energies will instantly all die at a certain point of their total max health
            float RatioToCombust = 0.2f;
            if (NPC.AnyNPCs(NPCType<DarkEnergy>()))
            {
                // Add max shield by feeding the data of false NPCs
                FalseNPCSegment = new NPC();
                FalseNPCSegment.SetDefaults(NPCType<DarkEnergy>(), target.GetMatchingSpawnParams());
                shieldMax = (int)(FalseNPCSegment.lifeMax * ExpectedBallsCounter * (1 - RatioToCombust));
                shield -= (int)(FalseNPCSegment.lifeMax * ExpectedBallsCounter * RatioToCombust);
                
                for (int i = 0; i < Main.maxNPCs; i++)
    			{
			    	NPC part = Main.npc[i];
		    		if (part.active && part.type == NPCType<DarkEnergy>())
	    				shield += part.life;				
    			}
            }
            return true;
        }
    }
}