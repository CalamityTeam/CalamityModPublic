using CalamityMod.NPCs.SlimeGod;
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
    public class SlimeGodBossBar : ModBossBar
    {
        // Used to determine the max health of a multi-segmented boss
        public NPC FalseNPCSegment;
        public List<int> SlimeGodSlimes = new List<int>
        {
            NPCType<CrimulanSlimeGod>(),
            NPCType<EbonianSlimeGod>(),
            NPCType<SplitCrimulanSlimeGod>(),
            NPCType<SplitEbonianSlimeGod>()
        };

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<SlimeGodCore>()]];

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC target = Main.npc[info.npcIndexToAimAt];
			if (!target.active)
				return false;

            life = 0f;
            lifeMax = 0f;

            // Add max health by feeding the data of false NPCs
            FalseNPCSegment = new NPC();
            FalseNPCSegment.SetDefaults(NPCType<CrimulanSlimeGod>(), target.GetMatchingSpawnParams());
            lifeMax += FalseNPCSegment.lifeMax;
            FalseNPCSegment.SetDefaults(NPCType<EbonianSlimeGod>(), target.GetMatchingSpawnParams());
            lifeMax += FalseNPCSegment.lifeMax;

            // Determine the current health of each slime
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC part = Main.npc[i];
				if (part.active && SlimeGodSlimes.Contains(part.type))
				{
					life += part.life;
				}
			}
            return true;
        }
    }
}