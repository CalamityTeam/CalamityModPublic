using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class RemsRevengeProj : ModProjectile
    {
		private int hitCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball O Fugu");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
			Player player = Main.player[projectile.owner];
            Vector2 projVector = player.Center - projectile.Center;
            projectile.rotation = projVector.ToRotation() - MathHelper.PiOver2;
            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            player.itemAnimation = 10;
            player.itemTime = 10;
            if (projVector.X < 0f)
            {
                player.ChangeDir(1);
                projectile.direction = 1;
            }
            else
            {
                player.ChangeDir(-1);
                projectile.direction = -1;
            }
            player.itemRotation = (projVector * -1f * (float)projectile.direction).ToRotation();
            projectile.spriteDirection = (projVector.X > 0f) ? -1 : 1;
            if (projectile.ai[0] == 0f && projVector.Length() > 600f)
            {
                projectile.ai[0] = 1f;
            }
            if (projectile.ai[0] == 1f || projectile.ai[0] == 2f)
            {
				projectile.extraUpdates = 1;
                float playerDist = projVector.Length();
                if (playerDist > 1500f)
                {
                    projectile.Kill();
                    return;
                }
                if (playerDist > 800f)
                {
                    projectile.ai[0] = 2f;
                }
                projectile.tileCollide = false;
                float returnSpeed = 15f;
                if (projectile.ai[0] == 2f)
                {
                    returnSpeed = 30f;
                }
                projectile.velocity = Vector2.Normalize(projVector) * returnSpeed;
                if (projVector.Length() < returnSpeed)
                {
                    projectile.Kill();
                    return;
                }
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 5f)
            {
                projectile.alpha = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            projectile.ai[0] = 1f;
            projectile.netUpdate = true;
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D chainTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/RemsRevengeChain");
            Vector2 projCenter = projectile.Center;
            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2((float)chainTexture.Width * 0.5f, (float)chainTexture.Height * 0.5f);
            float chainHeight = (float)chainTexture.Height;
            Vector2 realCenter = mountedCenter - projCenter;
            float rotation = (float)Math.Atan2((double)realCenter.Y, (double)realCenter.X) - MathHelper.PiOver2;
            bool canDraw = true;
            if (float.IsNaN(projCenter.X) && float.IsNaN(projCenter.Y))
            {
                canDraw = false;
            }
            if (float.IsNaN(realCenter.X) && float.IsNaN(realCenter.Y))
            {
                canDraw = false;
            }
            while (canDraw)
            {
                if (realCenter.Length() < chainHeight + 1f)
                {
                    canDraw = false;
                }
                else
                {
                    Vector2 centerDir = realCenter;
                    centerDir.Normalize();
                    projCenter += centerDir * chainHeight;
                    realCenter = mountedCenter - projCenter;
                    Color color = Lighting.GetColor((int)projCenter.X / 16, (int)(projCenter.Y / 16f));
					Main.spriteBatch.Draw(chainTexture, projCenter - Main.screenPosition, sourceRectangle, color, rotation, origin, 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<WitherDebuff>(), 240);
			hitCounter++;
			if (hitCounter > 3)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}
        }
    }
}
