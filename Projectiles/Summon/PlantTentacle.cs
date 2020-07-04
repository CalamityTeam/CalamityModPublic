using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class PlantTentacle : ModProjectile
    {
		private bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tentacle");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.ai[1] < 0 || projectile.ai[1] >= Main.maxProjectiles)
            {
                projectile.Kill();
                return;
            }
            // If something has gone wrong with either the tentacle or the host plant, destroy the proj.
            Projectile hostPlant = Main.projectile[(int)projectile.ai[1]];
            if (projectile.type != ModContent.ProjectileType<PlantTentacle>() || !hostPlant.active || hostPlant.type != ModContent.ProjectileType<PlantSummon>())
            {
                projectile.Kill();
                return;
            }

            if (projectile.frameCounter++ % 6 == 0)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            if (!initialized)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				initialized = true;
			}
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }

			projectile.rotation = (projectile.Center - hostPlant.Center).ToRotation() + MathHelper.Pi;

			float radians = MathHelper.TwoPi / 6f;
			float xOffset = hostPlant.Center.X + 150f;
			Vector2 source = new Vector2(xOffset, hostPlant.Center.Y);
			Vector2 goal = (source - hostPlant.Center).RotatedBy(radians * projectile.ai[0]);

			//Vector2 hourVector = (radians * projectile.ai[0]).ToRotationVector2();

			Vector2 targetPos = goal - projectile.Center;
			float targetDist = targetPos.Length();
			targetPos.Normalize();
			if (targetDist > 150f)
			{
				float speedMult = 8f;
				targetPos *= speedMult;
				projectile.velocity = (projectile.velocity * 40f + targetPos) / 41f;
			}
			else
			{
				float reverseSpeedMult = 4f;
				targetPos *= -reverseSpeedMult;
				projectile.velocity = (projectile.velocity * 40f + targetPos) / 41f;
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 107);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[1] < 0 || projectile.ai[1] >= Main.maxProjectiles)
            {
                return false;
            }

            // If something has gone wrong with either the tentacle or the host plant, return.
            Projectile hostPlant = Main.projectile[(int)projectile.ai[1]];
            if (projectile.type != ModContent.ProjectileType<PlantTentacle>() || !hostPlant.active || hostPlant.type != ModContent.ProjectileType<PlantSummon>())
            {
                return false;
            }

            Vector2 source = hostPlant.Center;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D chain = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/PlantationChain");
            Vector2 goal = projectile.Center;
            Rectangle? sourceRectangle = null;
            float textureHeight = (float)chain.Height;
            Vector2 drawVector = source - goal;
            float rotation = (float)Math.Atan2((double)drawVector.Y, (double)drawVector.X) - MathHelper.PiOver2;
            bool shouldDraw = true;
            if (float.IsNaN(goal.X) && float.IsNaN(goal.Y))
            {
                shouldDraw = false;
            }
            if (float.IsNaN(drawVector.X) && float.IsNaN(drawVector.Y))
            {
                shouldDraw = false;
            }
            while (shouldDraw)
            {
                if (drawVector.Length() < textureHeight + 1f)
                {
                    shouldDraw = false;
                }
                else
                {
                    Vector2 value2 = drawVector;
                    value2.Normalize();
                    goal += value2 * textureHeight;
                    drawVector = source - goal;
                    Color color = Lighting.GetColor((int)goal.X / 16, (int)(goal.Y / 16f));
                    Main.spriteBatch.Draw(chain, goal - Main.screenPosition, sourceRectangle, color, rotation, chain.Size() / 2f, 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }
    }
}
