using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    class PhantomicDagger : ModProjectile
    {
        private bool homing = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Dagger");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 38;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.alpha = 200;
            projectile.minion = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (homing)
                return null; //cannot hit until it is beginning to home.
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(homing);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            homing = reader.ReadBoolean();
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Avoid touching things that you probably aren't meant to damage
            if (target.defense > 999 || target.Calamity().DR >= 0.95f || target.Calamity().unbreakableDR)
                return;

            int cap = (int)(damage * 1.05f); // Capped at 5% dr ignoring
            damage = Math.Min((int)(damage * (1 / (1 - target.Calamity().DR))), cap);

            int projectileCount = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];
            int cap2 = 3;
            int oldDamage = damage;
            if (projectileCount > cap2)
            {
                damage -= (int)(oldDamage * ((projectileCount - cap2) * 0.05));
                if (damage < 1)
                    damage = 1;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int d = 0; d < 4; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[shadow].scale = 0.5f;
                    Main.dust[shadow].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 12; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 3f);
                Main.dust[shadow].noGravity = true;
                Main.dust[shadow].velocity *= 5f;
                shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 2f;
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (CalamityConfig.Instance.Afterimages)
            {
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            }
            return true;
        }

        public override void AI()
        {
            if (Main.dust.Length < Main.maxDust - 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 3f); //new Color(99, 54, 84)
                    Main.dust[dust].noGravity = true;
                }
            }
            if (projectile.alpha != 0)
            {
                projectile.rotation -= 0.25f;
                projectile.velocity.X *= 0.985f;
                projectile.velocity.Y *= 0.985f;
                if (projectile.alpha < 3)
                {
                    projectile.alpha = 0;
                    homing = true;
                }
                else
                    projectile.alpha -= 3;
            }
            else
            {
                NPC target = CalamityUtils.MinionHoming(projectile.Center, 1500f, Main.player[projectile.owner]);
                if (target != null)
                {
                    float num550 = 40f;
                    Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num551 = target.Center.X - vector43.X;
                    float num552 = target.Center.Y - vector43.Y;
                    float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                    if (num553 < 100f)
                    {
                        num550 = 28f; //14
                    }
                    num553 = num550 / num553;
                    num551 *= num553;
                    num552 *= num553;
                    projectile.velocity.X = (projectile.velocity.X * 25f + num551) / 26f;
                    projectile.velocity.Y = (projectile.velocity.Y * 25f + num552) / 26f;
                }
                else
                {
                    projectile.velocity *= 0.9f;
                }
                projectile.rotation = projectile.velocity.ToRotation() + (float)Math.Atan(90);
            }
        }
    }
}
