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
    public class RavagerHead2 : ModNPC
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
            npc.lifeMax = 100;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.dontTakeDamage = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.dontTakeDamage = false;
                npc.life = 0;
                HitEffect(npc.direction, 9999);
                npc.netUpdate = true;
                return;
            }

			bool death = CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice;
			bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;

            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			// Rotation
			float num801 = npc.position.X + (npc.width / 2) - player.position.X - (player.width / 2);
			float num802 = npc.position.Y + npc.height - 59f - player.position.Y - (player.height / 2);
			float num803 = (float)Math.Atan2(num802, num801) + MathHelper.PiOver2;
			if (num803 < 0f)
				num803 += MathHelper.TwoPi;
			else if (num803 > MathHelper.TwoPi)
				num803 -= MathHelper.TwoPi;

			float num804 = 0.1f;
			if (npc.rotation < num803)
			{
				if ((num803 - npc.rotation) > MathHelper.Pi)
					npc.rotation -= num804;
				else
					npc.rotation += num804;
			}
			else if (npc.rotation > num803)
			{
				if ((npc.rotation - num803) > MathHelper.Pi)
					npc.rotation += num804;
				else
					npc.rotation -= num804;
			}

			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
				npc.rotation = num803;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			else if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
				npc.rotation = num803;

			npc.ai[1] += 1f;
			bool fireProjectiles = npc.ai[1] >= 480f;
			if (fireProjectiles && Vector2.Distance(npc.Center, player.Center) > 80f)
            {
				int type = ModContent.ProjectileType<ScavengerLaser>();
				int damage = npc.GetProjectileDamage(type);
				float projectileVelocity = death ? 8f : 6f;

				if (npc.ai[1] >= 600f)
				{
					npc.ai[0] += 1f;
					npc.ai[1] = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Main.PlaySound(SoundID.Item33, npc.position);
						Projectile.NewProjectile(npc.Center, Vector2.Normalize(player.Center - npc.Center) * projectileVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, -1f);
					}
				}
				else
				{
					if (npc.ai[1] % 40f == 0f)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Main.PlaySound(SoundID.Item33, npc.position);
							Projectile.NewProjectile(npc.Center, Vector2.Normalize(player.Center - npc.Center) * projectileVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, -1f);
						}
					}
				}
            }

			float num823 = 22f;
			float num824 = 0.3f;
			if (death)
			{
				num823 += 4f;
				num824 += 0.05f;
			}
			if (provy)
			{
				num823 *= 1.25f;
				num824 *= 1.25f;
			}
			if (BossRushEvent.BossRushActive)
			{
				num823 *= 1.5f;
				num824 *= 1.5f;
			}

			Vector2 vector82 = npc.Center;
			float distanceX = npc.ai[0] % 2f == 0f ? 480f : -480f;
			float distanceY = fireProjectiles ? -320f : 320f;
			float num825 = player.Center.X + (fireProjectiles ? distanceX : 0f) - vector82.X;
			float num826 = player.Center.Y + distanceY - vector82.Y;
			float num827 = (float)Math.Sqrt(num825 * num825 + num826 * num826);
			num827 = num823 / num827;
			num825 *= num827;
			num826 *= num827;

			if (npc.velocity.X < num825)
			{
				npc.velocity.X += num824;
				if (npc.velocity.X < 0f && num825 > 0f)
					npc.velocity.X += num824;
			}
			else if (npc.velocity.X > num825)
			{
				npc.velocity.X -= num824;
				if (npc.velocity.X > 0f && num825 < 0f)
					npc.velocity.X -= num824;
			}
			if (npc.velocity.Y < num826)
			{
				npc.velocity.Y += num824;
				if (npc.velocity.Y < 0f && num826 > 0f)
					npc.velocity.Y += num824;
			}
			else if (npc.velocity.Y > num826)
			{
				npc.velocity.Y -= num824;
				if (npc.velocity.Y > 0f && num826 < 0f)
					npc.velocity.Y -= num824;
			}
		}

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead3"), 1f);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                    Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
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
    }
}
