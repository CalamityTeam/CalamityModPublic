using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
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
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.width = 60;
            npc.height = 60;
            npc.defense = 40;
			npc.DR_NERD(0.15f);
            npc.lifeMax = 22010;
            npc.knockBackResist = 0f;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.Frostburn] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
            npc.buffImmune[BuffID.BetsysCurse] = false;
            npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
            if (CalamityWorld.downedProvidence)
            {
                npc.defense = 135;
                npc.lifeMax = 200000;
            }
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 400000;
            }
            double HPBoost = (double)CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
        }

        public override void AI()
        {
            bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;
            Vector2 center = npc.Center;
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            if (npc.timeLeft < 1800)
            {
                npc.timeLeft = 1800;
            }
            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
                npc.ai[1] = 0f;
            }
            if (npc.ai[0] == 0f)
            {
                float num659 = 21f;
                Vector2 vector79 = new Vector2(center.X, center.Y);
                float num660 = Main.npc[CalamityGlobalNPC.scavenger].Center.X - vector79.X;
                float num661 = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - vector79.Y;
                num661 += 88f;
                num660 += 70f;
                float num662 = (float)Math.Sqrt((double)(num660 * num660 + num661 * num661));
                if (num662 < 12f + num659)
                {
                    npc.rotation = 0f;
                    npc.velocity.X = num660;
                    npc.velocity.Y = num661;
                }
                else
                {
                    num662 = num659 / num662;
                    npc.velocity.X = num660 * num662;
                    npc.velocity.Y = num661 * num662;
                }
            }
        }

		public override bool CheckActive()
		{
			return false;
		}

		public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerLegRight"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerLegRight2"), 1f);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
