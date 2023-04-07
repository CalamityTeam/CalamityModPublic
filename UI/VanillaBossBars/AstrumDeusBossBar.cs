using CalamityMod.NPCs.AstrumDeus;
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
    public class AstrumDeusBossBar : ModBossBar
    {
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<AstrumDeusHead>()]];

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)/* tModPorter Note: life and shield current and max values are now separate to allow for hp/shield number text draw */
        {
            NPC target = Main.npc[info.npcIndexToAimAt];
			if (!target.active && !FindMoreWorms(ref info))
				return false;

            int life = 0;
            int lifeMax = 0;

            // Determine the real health by finding more of itself
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC worm = Main.npc[i];
				if (worm.active && worm.type == target.type)
				{
                    // In Death Mode, every worm must be killed
                    if (CalamityWorld.death)
                    {
                        life += worm.life;
                        lifeMax += worm.lifeMax;
                    }
                    // Otherwise, find the minimum
                    else
                    {
                        if (life <= 0)
                            life = worm.life;
                        else
                            life = Math.Min(life, worm.life);

                        lifeMax = worm.lifeMax;
                    }
				}
			}
            lifePercent = Utils.Clamp(life / (float)lifeMax, 0f, 1f);
            return true;
        }

        public bool FindMoreWorms(ref BigProgressBarInfo info)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC target = Main.npc[i];
				if (target.active && target.type == NPCType<AstrumDeusHead>())
				{
					info.npcIndexToAimAt = i;
					return true;
				}
			}
			return false;
        }
    }
}