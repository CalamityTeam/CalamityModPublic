using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerLegRight : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 50;
            NPC.width = 60;
            NPC.height = 60;
            NPC.defense = 40;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 12788;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath14;
            if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
            {
                NPC.defense *= 2;
                NPC.lifeMax *= 4;
            }
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 40000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;

                NPC.ai[1] = 0f;
            }

            NPC.Center = Main.npc[CalamityGlobalNPC.scavenger].Center + new Vector2(70f, 88f);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerLegRight").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerLegRight2").Type, 1f);
                }
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
