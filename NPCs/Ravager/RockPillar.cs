using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class RockPillar : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rock Pillar");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.width = 60;
            npc.height = 300;
			npc.defense = 50;
			npc.DR_NERD(0.3f);
			npc.chaseable = false;
			npc.lifeMax = CalamityWorld.downedProvidence ? 35000 : 5000;
            npc.alpha = 255;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
			npc.HitSound = SoundID.NPCHit41;
			npc.DeathSound = SoundID.NPCDeath14;
		}

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.life = 0;
                HitEffect(npc.direction, 9999);
                npc.netUpdate = true;
                return;
            }

            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            if (npc.alpha > 0)
            {
				npc.damage = 0;

				npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }
            else
            {
                if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
                    npc.damage = npc.defDamage * 2;
                else
                    npc.damage = npc.defDamage;
            }                

            if (npc.ai[0] == 0f)
            {
                if (npc.velocity.Y == 0f)
                {
                    if (npc.ai[1] == -1f)
                    {
						Main.PlaySound(SoundID.Item62, npc.position);

						for (int num621 = 0; num621 < 10; num621++)
						{
							int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
							Main.dust[num622].velocity *= 3f;
							if (Main.rand.NextBool(2))
							{
								Main.dust[num622].scale = 0.5f;
								Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
							}
						}
						for (int num623 = 0; num623 < 10; num623++)
						{
							int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Stone, 0f, 0f, 100, default, 3f);
							Main.dust[num624].noGravity = true;
							Main.dust[num624].velocity *= 5f;
							num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
							Main.dust[num624].velocity *= 2f;
						}

						npc.noTileCollide = true;
						npc.velocity.X = 12 * npc.direction;
                        npc.velocity.Y = -28.5f;
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                    }
                }
            }
            else
            {
                if (npc.velocity.Y == 0f || Vector2.Distance(npc.Center, Main.npc[CalamityGlobalNPC.scavenger].Center) > 2800f)
                {
                    Main.PlaySound(SoundID.Item14, npc.position);
                    npc.ai[0] = 0f;
                    npc.life = 0;
                    HitEffect(npc.direction, 9999);
                    npc.netUpdate = true;
                    return;
                }
                else
                {
                    npc.velocity.Y += 0.2f;

					if (npc.velocity.Y >= 0f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
						npc.noTileCollide = false;
				}
            }
        }

		public override bool CheckActive()
		{
			return false;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180, true);
			Main.PlaySound(SoundID.Item14, npc.position);
			npc.ai[0] = 0f;
			npc.life = 0;
			HitEffect(npc.direction, 9999);
			npc.netUpdate = true;
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 80;
                npc.height = 360;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
			else
			{
				for (int num621 = 0; num621 < 2; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 2; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Stone, 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
        }
    }
}
