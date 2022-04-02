using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class MalachiteStealth : ModProjectile
    {
        private const int lifeSpan = 300;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Malachite";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malachite");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
            projectile.timeLeft = lifeSpan;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, 10f, 25f);
                projectile.localAI[1] += 1f;
                if (projectile.localAI[1] > 4f)
                {
                    for (int num468 = 0; num468 < 3; num468++)
                    {
                        int num469 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 0.75f);
                        Main.dust[num469].noGravity = true;
                        Main.dust[num469].velocity *= 0f;
                    }
                }
            }
            else
            {
                int id = (int)projectile.ai[1];
                if (id.WithinBounds(Main.maxNPCs) && Main.npc[id].active && !Main.npc[id].dontTakeDamage)
                {
                    projectile.Center = Main.npc[id].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[id].gfxOffY;
                }
                else
                {
                    projectile.Kill();
                }
            }

            projectile.alpha -= 3;
            if (projectile.alpha < 30)
            {
                projectile.alpha = 30;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(Main.DiscoR, 203, 103, projectile.alpha);

        public override void Kill(int timeLeft)
        {
            projectile.ai[0] = 2f;
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 112);
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int i = 0; i < 7; i++)
            {
                int dusty = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.2f);
                Main.dust[dusty].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[dusty].scale = 0.5f;
                    Main.dust[dusty].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 15; j++)
            {
                int green = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.7f);
                Main.dust[green].noGravity = true;
                Main.dust[green].velocity *= 5f;
                green = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[green].velocity *= 2f;
            }
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
            projectile.extraUpdates = 0;
            projectile.ai[0] = 1f;
            projectile.ai[1] = target.whoAmI;
            projectile.velocity = target.Center - projectile.Center;
            projectile.velocity *= 0.75f;
            projectile.netUpdate = true;

            const int maxKunai = 3;
            int kunaiFound = 0;
            int oldestKunai = -1;
            int oldestKunaiTimeLeft = lifeSpan;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == projectile.type && i != projectile.whoAmI && Main.projectile[i].ai[1] == target.whoAmI)
                {
                    kunaiFound++;
                    if (Main.projectile[i].timeLeft < oldestKunaiTimeLeft)
                    {
                        oldestKunaiTimeLeft = Main.projectile[i].timeLeft;
                        oldestKunai = Main.projectile[i].whoAmI;
                    }
                    if (kunaiFound >= maxKunai)
                        break;
                }
            }
            if (kunaiFound >= maxKunai && oldestKunai >= 0)
            {
                Main.projectile[oldestKunai].Kill();
            }
        }

        public override bool CanDamage() => projectile.ai[0] != 1f;
    }
}
