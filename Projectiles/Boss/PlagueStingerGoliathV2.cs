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
    public class PlagueStingerGoliathV2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/Boss/PlagueStingerGoliath";

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
        }

        public override void AI()
        {
            if (Projectile.position.Y > Projectile.ai[1])
                Projectile.tileCollide = true;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Plague>(), 90);
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

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                float scale = 1.5f + Projectile.ai[0] * 0.015f;
                int baseWidthAndHeight = 20;
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlagueExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[proj].scale = scale;
                Main.projectile[proj].width = (int)(baseWidthAndHeight * scale);
                Main.projectile[proj].height = (int)(baseWidthAndHeight * scale);
                Main.projectile[proj].position.X = Projectile.Center.X - Main.projectile[proj].width * 0.5f;
                Main.projectile[proj].position.Y = Projectile.Center.Y - Main.projectile[proj].height * 0.5f;
            }
        }
    }
}
