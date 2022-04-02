using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LeviathanTooth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviathan Tooth");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                if (projectile.timeLeft < 575 && !projectile.Calamity().stealthStrike)
                {
                    projectile.velocity.Y += 0.5f;
                    if (projectile.velocity.Y > 16f)
                    {
                        projectile.velocity.Y = 16f;
                    }
                }
            }
            //Sticky Behaviour
            projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(6, false);
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
            Texture2D tex = Main.projectileTexture[projectile.type];
            if (projectile.Calamity().lineColor == 1)
                tex = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/LeviathanTooth2");
            if (projectile.Calamity().lineColor == 2)
                tex = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/LeviathanTooth3");

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            if (projectile.Calamity().stealthStrike)
            {
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1, tex);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(projectile.Center, 1, 1, 33, 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
