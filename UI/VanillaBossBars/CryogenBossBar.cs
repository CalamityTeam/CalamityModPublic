using CalamityMod.NPCs.Cryogen;
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
    public class CryogenBossBar : ModBossBar
    {
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            // Pyrogen moment
            if (Main.zenithWorld)
                return ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Pyrogen_Head_Boss");
            return ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase1_Head_Boss");
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC target = Main.npc[info.npcIndexToAimAt];
			if (!target.active)
				return false;

            // Get the boss health, obviously
            life = target.life;
            lifeMax = target.lifeMax;
            
            // Reset the shield
            shield = 0f;
            shieldMax = 0f;

            // Determine the shield health
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC part = Main.npc[i];
				if (part.active && part.type == NPCType<CryogenShield>())
				{
					shield += part.life;
                    shieldMax += part.lifeMax;
				}
			}
            return true;
        }
    }
}