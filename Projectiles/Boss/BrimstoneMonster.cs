using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneMonster : ModProjectile
    {
        private float speedAdd = 0f;
        private float speedLimit = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Monster");
        }

        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 320;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 36000;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedAdd);
            writer.Write(projectile.localAI[0]);
            writer.Write(speedLimit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedAdd = reader.ReadSingle();
            projectile.localAI[0] = reader.ReadSingle();
            speedLimit = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                projectile.active = false;
                projectile.netUpdate = true;
                return;
            }

			int choice = (int)projectile.ai[1];
			if (projectile.soundDelay <= 0 && (choice == 0 || choice == 2))
			{
				projectile.soundDelay = 420;
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BrimstoneMonsterDrone"), (int)projectile.Center.X, (int)projectile.Center.Y);
			}

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] += 1f;
				switch (choice)
				{
					case 0:
						speedLimit = 10f;
						break;
					case 1:
						speedLimit = 20f;
						break;
					case 2:
						speedLimit = 30f;
						break;
					case 3:
						speedLimit = 40f;
						break;
					default:
						break;
				}
            }

            if (speedAdd < speedLimit)
                speedAdd += 0.04f;

            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;

            Lighting.AddLight(projectile.Center, 3f, 0f, 0f);

            float num953 = (revenge ? 5f : 4.5f) + speedAdd; //100
            float scaleFactor12 = (revenge ? 1.5f : 1.35f) + (speedAdd * 0.25f); //5
            float num954 = 160f;

			if (NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
			{
				num953 *= 0.5f;
				scaleFactor12 *= 0.5f;
			}

			if (projectile.timeLeft < 90)
				projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 90f, 0f, 1f);
			else
				projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 35910) / 90f), 0f, 1f);

			int num959 = (int)projectile.ai[0];
            if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead)
            {
                if (projectile.Distance(Main.player[num959].Center) > num954)
                {
                    Vector2 vector102 = projectile.DirectionTo(Main.player[num959].Center);
                    if (vector102.HasNaNs())
                        vector102 = Vector2.UnitY;

                    projectile.velocity = (projectile.velocity * (num953 - 1f) + vector102 * scaleFactor12) / num953;
                }
            }
            else
            {
                if (projectile.ai[0] != -1f)
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
            }

			// Fly away from other brimstone monsters
			float velocity = 0.05f;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active)
				{
					if (i != projectile.whoAmI && Main.projectile[i].type == projectile.type)
					{
						if (Vector2.Distance(projectile.Center, Main.projectile[i].Center) < 320f)
						{
							if (projectile.position.X < Main.projectile[i].position.X)
								projectile.velocity.X -= velocity;
							else
								projectile.velocity.X += velocity;

							if (projectile.position.Y < Main.projectile[i].position.Y)
								projectile.velocity.Y -= velocity;
							else
								projectile.velocity.Y += velocity;
						}
					}
				}
			}
		}

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 170f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
			lightColor.R = (byte)(255 * projectile.Opacity);
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

		public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.Opacity != 1f)
				return;

			target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 900);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 300, true);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
