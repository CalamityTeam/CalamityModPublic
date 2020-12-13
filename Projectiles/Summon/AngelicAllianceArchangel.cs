using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AngelicAllianceArchangel : ModProjectile
    {
		private int lifeSpan = 300;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archangel");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 68;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
            projectile.minionSlots = 0f;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(lifeSpan);

        public override void ReceiveExtraAI(BinaryReader reader) => lifeSpan = reader.ReadInt32();

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

			if (!modPlayer.divineBless)
			{
				projectile.Kill();
				return;
			}

			lifeSpan--;
			if (lifeSpan <= 0)
			{
				projectile.alpha += 30;
				if (projectile.alpha >= 255)
				{
					projectile.Kill();
					return;
				}
			}

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 30;
                for (int d = 0; d < dustAmt; d++)
                {
                    int idx = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, (int)CalamityDusts.ProfanedFire, 0f, 0f, 0, default, 1f);
                    Main.dust[idx].velocity *= 2f;
                    Main.dust[idx].scale *= 1.15f;
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

			//Adjust sprite direction so it faces correctly
            if (Math.Abs(projectile.velocity.X) > 0.2f)
            {
                projectile.spriteDirection = -projectile.direction;
            }

			projectile.ChargingMinionAI(1600f, 1800f, 2500f, 400f, 0, 30f, 24f, 12f, new Vector2(0f, -60f), 30f, 10f, true, true);

			//Give off some light
            float lightScalar = Main.rand.NextFloat(0.9f, 1.1f) * Main.essScale;
            Lighting.AddLight(projectile.Center, 0.2f * lightScalar, 0.17f * lightScalar, 0.1f * lightScalar);

			if (!projectile.FinalExtraUpdate())
				return;

			//Frames
            projectile.frameCounter++;
            if (projectile.frameCounter > 7)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);

		public override bool CanDamage() => projectile.alpha < 50;
    }
}
