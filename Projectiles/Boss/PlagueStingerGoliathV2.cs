using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class PlagueStingerGoliathV2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/PlagueStingerGoliath";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exploding Plague Stinger");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1.5f;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (Projectile.position.Y > Projectile.ai[1])
                Projectile.tileCollide = true;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Value;
            Vector2 origin = new Vector2(glow.Width / 2, glow.Height / Main.projFrames[Projectile.type] / 2);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos -= new Vector2(glow.Width, glow.Height / Main.projFrames[Projectile.type]) * 1f / 2f;
            drawPos += origin * 1f + new Vector2(0f, 0f + 4f + Projectile.gfxOffY);
            Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);
            Main.spriteBatch.Draw(glow, drawPos, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            if (Projectile.owner == Main.myPlayer)
            {
                float scale = 1.5f + Projectile.ai[0] * 0.015f;
                int baseWidthAndHeight = 20;
                int proj = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlagueExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[proj].scale = scale;
                Main.projectile[proj].width = (int)(baseWidthAndHeight * scale);
                Main.projectile[proj].height = (int)(baseWidthAndHeight * scale);
                Main.projectile[proj].position.X = Projectile.Center.X - Main.projectile[proj].width * 0.5f;
                Main.projectile[proj].position.Y = Projectile.Center.Y - Main.projectile[proj].height * 0.5f;
            }
        }
    }
}
