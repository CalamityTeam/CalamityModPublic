using CalamityMod.NPCs.Leviathan;
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
    public class LeviathanAnahitaBossBar : ModBossBar
    {
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            // Prevent icon seizures by prioritizing Leviathan first
            if (NPC.AnyNPCs(NPCType<Leviathan>()))
                return TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<Leviathan>()]];

            return TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<Anahita>()]];
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC target = Main.npc[info.npcIndexToAimAt];

			if (!target.active && !FindTheRightFish(ref info))
				return false;

            // Immediately grab the boss's health, whichever one it is. We will check later.
            life = target.life;
            lifeMax = target.lifeMax;

            // Find the partner in question
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC wife = Main.npc[i];
                int targetCouple = target.type == NPCType<Anahita>() ? NPCType<Leviathan>() : NPCType<Anahita>();
                if (wife.active && wife.type == targetCouple)
                {
                    life += wife.life;
                    lifeMax += wife.lifeMax;
                }
            }

            // Reset the shield
            shield = 0f;
            shieldMax = 0f;

            // Determine Anahita's shield health only if she's solo (she can't just block Leviathan out of existence)
            if (target.type == NPCType<Anahita>() && !NPC.AnyNPCs(NPCType<Leviathan>()))
            {       
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC part = Main.npc[i];
                    if (part.active && part.type == NPCType<AnahitasIceShield>())
                    {
                        shield += part.life;
                        shieldMax += part.lifeMax;
                    }
                }
            }
            return true;
        }

        public bool FindTheRightFish(ref BigProgressBarInfo info)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC target = Main.npc[i];
				if (target.active && target.type == NPCType<Anahita>())
				{
					info.npcIndexToAimAt = i;
					return true;
				}
                else if (target.active && target.type == NPCType<Leviathan>())
                {
                    info.npcIndexToAimAt = i;
                    return true;
                }
			}
			return false;
        }
    }
}
