using CalamityMod.NPCs.Ravager;
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
    public class RavagerBossBar : ModBossBar
    {
        // Used to determine the max health of a multi-segmented boss
        public NPC FalseNPCSegment;
        public List<int> RavagerParts = new List<int>
        {
            NPCType<RavagerClawLeft>(),
            NPCType<RavagerClawRight>(),
            NPCType<RavagerHead>(),
            NPCType<RavagerLegLeft>(),
            NPCType<RavagerLegRight>()
        };

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<RavagerBody>()]];

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)/* tModPorter Note: life and shield current and max values are now separate to allow for hp/shield number text draw */
        {
            NPC target = Main.npc[info.npcIndexToAimAt];
			if (!target.active && !FindRavagerBody(ref info))
				return false;

            int life = target.life;
            int lifeMax = target.lifeMax;

            // Add max health by feeding the data of false NPCs
            foreach (int type in RavagerParts)
            {
                FalseNPCSegment = new NPC();
                FalseNPCSegment.SetDefaults(type, target.GetMatchingSpawnParams());
                lifeMax += FalseNPCSegment.lifeMax;
            }

            // Determine the current health of the parts
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC part = Main.npc[i];
				if (part.active && RavagerParts.Contains(part.type))
				{
					life += part.life;
				}
			}
            lifePercent = Utils.Clamp(life / (float)lifeMax, 0f, 1f);
            return true;
        }

        public bool FindRavagerBody(ref BigProgressBarInfo info)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC target = Main.npc[i];
				if (target.active && target.type == NPCType<RavagerBody>())
				{
					info.npcIndexToAimAt = i;
					return true;
				}
			}
			return false;
        }
    }
}