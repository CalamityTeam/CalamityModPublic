using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SonOfYharon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Son of Yharon");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
            projectile.extraUpdates = 1;
            projectile.minionSlots = 4f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 100;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 244, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            if (Math.Abs(projectile.velocity.X) > 0.2f)
            {
                projectile.spriteDirection = -projectile.direction;
            }
            float lightScalar = (float)Main.rand.Next(90, 111) * 0.01f;
            lightScalar *= Main.essScale;
            Lighting.AddLight(projectile.Center, 1.2f * lightScalar, 0.8f * lightScalar, 0f * lightScalar);
            bool correctMinion = projectile.type == ModContent.ProjectileType<SonOfYharon>();
            player.AddBuff(ModContent.BuffType<YharonKindleBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.aChicken = false;
                }
                if (modPlayer.aChicken)
                {
                    projectile.timeLeft = 2;
                }
            }
            if (projectile.ai[0] == 2f)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter > 3)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame > 2)
                {
                    projectile.frame = 0;
                }
			}
			else
			{
				projectile.frameCounter++;
				if (projectile.frameCounter > 12)
				{
					projectile.frame++;
					projectile.frameCounter = 0;
				}
				if (projectile.frame > 3)
				{
					projectile.frame = 0;
				}
			}

			projectile.ChargingMinionAI(1000f, 2000f, 3000f, 500f, 1, 30f, 8f, 4f, new Vector2(0f, -60f), 40f, 8f, true, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
