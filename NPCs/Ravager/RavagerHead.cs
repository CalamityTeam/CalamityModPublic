using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Ravager
{
	public class RavagerHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 40;
			npc.DR_NERD(0.15f);
            npc.lifeMax = 32705;
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
			npc.buffImmune[BuffID.BetsysCurse] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
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
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.buffImmune[ModContent.BuffType<BanishingFire>()] = false;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
            npc.alpha = 255;
            npc.value = Item.buyPrice(0, 0, 0, 0);
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = null;
            if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
            {
                npc.defense *= 2;
                npc.lifeMax *= 7;
            }
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 450000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
        }

        public override void AI()
        {
            bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            float speed = 21f;
            float centerX = Main.npc[CalamityGlobalNPC.scavenger].Center.X - npc.Center.X;
            float centerY = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - npc.Center.Y;
            centerY -= 20f;
            centerX += 1f;
            float totalSpeed = (float)Math.Sqrt(centerX * centerX + centerY * centerY);
            if (totalSpeed < 20f)
            {
                npc.rotation = 0f;
                npc.velocity.X = centerX;
                npc.velocity.Y = centerY;
            }
            else
            {
                totalSpeed = speed / totalSpeed;
                npc.velocity.X = centerX * totalSpeed;
                npc.velocity.Y = centerY * totalSpeed;
                npc.rotation = npc.velocity.X * 0.1f;
            }

            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            npc.ai[1] += 1f;
            if (npc.ai[1] >= (death ? 420f : 480f))
            {
                Main.PlaySound(SoundID.Item62, npc.position);
                npc.TargetClosest(true);
                npc.ai[1] = 0f;
				int type = ModContent.ProjectileType<ScavengerNuke>();
				int damage = npc.GetProjectileDamage(type);
				if (Main.netMode != NetmodeID.MultiplayerClient)
                {
					Vector2 shootFromVector = new Vector2(npc.Center.X, npc.Center.Y - 20f);
					Vector2 velocity = new Vector2(0f, -15f);
                    int nuke = Projectile.NewProjectile(shootFromVector, velocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, npc.target, 0f);
                    Main.projectile[nuke].velocity.Y = -15f;
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
            if (npc.life > 0)
            {
                int num285 = 0;
                while ((double)num285 < damage / (double)npc.lifeMax * 100.0)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, (float)hitDirection, -1f, 0, default, 1f);
                    num285++;
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<RavagerHead2>(), npc.whoAmI);
            }
        }
    }
}
