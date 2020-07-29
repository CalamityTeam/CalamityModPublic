using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Boss;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.World;

namespace CalamityMod.NPCs.OldDuke
{
	public class OldDukeToothBall : ModNPC
	{
		bool spawnedProjectiles = false;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tooth Ball");
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 180;
			npc.width = 40;
			npc.height = 40;
			npc.defense = 0;
			npc.lifeMax = 5000;
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = 75000;
			}
			npc.alpha = 255;
            npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath11;
            npc.noGravity = true;
            npc.noTileCollide = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
			writer.Write(spawnedProjectiles);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
			spawnedProjectiles = reader.ReadBoolean();
		}

		public override void AI()
		{
            npc.rotation += npc.velocity.X * 0.05f;
            if (npc.alpha > 0)
            {
                npc.alpha -= 5;
            }
			npc.TargetClosest(false);
			Player player = Main.player[npc.target];
			if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                    return;
                }
            }
            else if (npc.timeLeft < 600)
            {
                npc.timeLeft = 600;
            }
            Vector2 vector = player.Center - npc.Center;
            if (vector.Length() < 40f || npc.ai[3] >= 900f)
            {
                npc.dontTakeDamage = false;
                npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
				npc.active = false;
				return;
            }

            npc.ai[3] += 1f;
            npc.dontTakeDamage = npc.ai[3] >= 600f ? false : true;
            if (npc.ai[3] >= 480f)
            {
                npc.velocity *= 0.985f;
                return;
            }

            float num1372 = 12f;
			if (Main.expertMode || CalamityWorld.bossRushActive)
				num1372 += Vector2.Distance(player.Center, npc.Center) * 0.01f;

			Vector2 vector167 = new Vector2(npc.Center.X + npc.direction * 20, npc.Center.Y + 6f);
            float num1373 = player.position.X + player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            npc.ai[0] -= Main.rand.Next(6);
            if (num1375 < 300f || npc.ai[0] > 0f)
            {
                if (num1375 < 300f)
                {
                    npc.ai[0] = 100f;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                return;
            }

            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
            }

			float num1247 = 0.5f;
			for (int num1248 = 0; num1248 < Main.maxNPCs; num1248++)
			{
				if (Main.npc[num1248].active)
				{
					if (num1248 != npc.whoAmI && Main.npc[num1248].type == npc.type)
					{
						if (Vector2.Distance(npc.Center, Main.npc[num1248].Center) < 48f)
						{
							if (npc.position.X < Main.npc[num1248].position.X)
								npc.velocity.X -= num1247;
							else
								npc.velocity.X += num1247;

							if (npc.position.Y < Main.npc[num1248].position.Y)
								npc.velocity.Y -= num1247;
							else
								npc.velocity.Y += num1247;
						}
					}
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return npc.alpha == 0;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
            player.AddBuff(BuffID.Venom, 180, true);
			player.AddBuff(BuffID.Poisoned, 180, true);
			player.AddBuff(ModContent.BuffType<Irradiated>(), 180);
		}

        public override bool CheckDead()
        {
            Main.PlaySound(SoundID.Item14, npc.position);

            npc.position.X = npc.position.X + (npc.width / 2);
            npc.position.Y = npc.position.Y + (npc.height / 2);
            npc.width = npc.height = 96;
            npc.position.X = npc.position.X - (npc.width / 2);
            npc.position.Y = npc.position.Y - (npc.height / 2);

            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }

            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && !spawnedProjectiles)
            {
				spawnedProjectiles = true;
				int totalProjectiles = 4;
				float radians = MathHelper.TwoPi / totalProjectiles;
				int damage = Main.expertMode ? 55 : 70;
				for (int k = 0; k < totalProjectiles; k++)
				{
					float velocity = Main.rand.Next(7, 11);
					Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * k);
					int proj = Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<SandTooth>(), damage, 0f, Main.myPlayer, 0f, 0f);
					Main.projectile[proj].timeLeft = 360;
				}
				Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SandPoisonCloud>(), damage, 0f, Main.myPlayer, 0f, 0f);
            }

            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}