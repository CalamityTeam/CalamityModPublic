using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    class VoidConcentrationOrb : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Orb");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 50;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;

            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.scale = 0.01f;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.scale < 1f)
                return false;
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int d = 0; d < 6; d++)
            {
                int shadow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[shadow].scale = 0.5f;
                    Main.dust[shadow].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 10; d++)
            {
                int shadow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 3f);
                Main.dust[shadow].noGravity = true;
                Main.dust[shadow].velocity *= 5f;
                shadow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 2f;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            NPC target = CalamityUtils.MinionHoming(Projectile.Center, 1800f, owner);

            if (Projectile.scale >= 1f)
            {
                if (target != null)
                {
                    float num550 = 40f;
                    Vector2 vector43 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
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
                    Projectile.velocity.X = (Projectile.velocity.X * 25f + num551) / 26f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 25f + num552) / 26f;
                    if (Main.rand.NextBool(5))
                        Projectile.velocity *= 1.1f;
                }

            }
            else
            {
                Projectile.scale += 0.025f;
                Projectile.velocity *= 1.03f;
            }

            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type] - 1)
            {
                Projectile.frame = 0;
            }
            Projectile.frameCounter++;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 4;
            }
        }
    }
}
