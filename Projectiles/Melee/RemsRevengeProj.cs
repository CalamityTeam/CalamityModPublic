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
            DisplayName.SetDefault("Rem's Revenge");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 1;
            Projectile.scale = 2f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 projVector = player.Center - Projectile.Center;
            Projectile.rotation = projVector.ToRotation() - MathHelper.PiOver2;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            player.itemAnimation = 10;
            player.itemTime = 10;
            if (projVector.X < 0f)
            {
                player.ChangeDir(1);
                Projectile.direction = 1;
            }
            else
            {
                player.ChangeDir(-1);
                Projectile.direction = -1;
            }
            player.itemRotation = (projVector * -1f * (float)Projectile.direction).ToRotation();
            Projectile.spriteDirection = (projVector.X > 0f) ? -1 : 1;
            if (Projectile.ai[0] == 0f && projVector.Length() > 850f)
            {
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[0] == 1f || Projectile.ai[0] == 2f)
            {
                Projectile.extraUpdates = 2;
                float playerDist = projVector.Length();
                if (playerDist > 1500f)
                {
                    Projectile.Kill();
                    return;
                }
                if (playerDist > 1000f)
                {
                    Projectile.ai[0] = 2f;
                }
                Projectile.tileCollide = false;
                float returnSpeed = 15f;
                if (Projectile.ai[0] == 2f)
                {
                    returnSpeed = 30f;
                }
                Projectile.velocity = Vector2.Normalize(projVector) * returnSpeed;
                if (projVector.Length() < returnSpeed)
                {
                    Projectile.Kill();
                    return;
                }
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 5f)
            {
                Projectile.alpha = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D chainTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Chains/RemsRevengeChain");
            Vector2 projCenter = Projectile.Center;
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

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<WitherDebuff>(), 240);
            hitCounter++;
            if (hitCounter < 6)
                Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodExplosion>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner);
            if (hitCounter > 3)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
        }
    }
}
