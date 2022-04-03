using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class LeviathanTooth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviathan Tooth");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.Calamity().rogue = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
                if (Projectile.timeLeft < 575 && !Projectile.Calamity().stealthStrike)
                {
                    Projectile.velocity.Y += 0.5f;
                    if (Projectile.velocity.Y > 16f)
                    {
                        Projectile.velocity.Y = 16f;
                    }
                }
            }
            //Sticky Behaviour
            Projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Projectile.ModifyHitNPCSticky(6, false);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.Calamity().lineColor == 1)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/LeviathanTooth2");
            if (Projectile.Calamity().lineColor == 2)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/LeviathanTooth3");

            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            if (Projectile.Calamity().stealthStrike)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, tex);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 33, 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
