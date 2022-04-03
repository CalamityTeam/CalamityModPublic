using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Melee
{
    public class PlagueBeeDust : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/PlagueDust";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 150;
            Projectile.alpha = 100;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[1]++;
            Lighting.AddLight(Projectile.Center, 0.05f, 0.4f, 0f);
            if (Projectile.ai[1] < 60f)
            {
                if (Projectile.ai[0] > 7f)
                {
                    float scalar = 1f;
                    if (Projectile.ai[0] == 8f)
                    {
                        scalar = 0.25f;
                    }
                    else if (Projectile.ai[0] == 9f)
                    {
                        scalar = 0.5f;
                    }
                    else if (Projectile.ai[0] == 10f)
                    {
                        scalar = 0.75f;
                    }
                    Projectile.ai[0] += 1f;
                    if (Projectile.ai[0] % 2f == 0f)
                    {
                        int spawnX = (int)(Projectile.width / 2);
                        int spawnY = (int)(Projectile.width / 2);
                        int bee = Projectile.NewProjectile(Projectile.Center.X + (float)Main.rand.Next(-spawnX, spawnX), Projectile.Center.Y + (float)Main.rand.Next(-spawnY, spawnY), Projectile.velocity.X, Projectile.velocity.Y, player.beeType(), player.beeDamage(Projectile.damage / 3), player.beeKB(0f), Projectile.owner);
                        if (bee.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[bee].Calamity().forceMelee = true;
                            Main.projectile[bee].penetrate = 1;
                        }
                    }
                    //Dust
                    int dustType = 89;
                    int plague = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                    Dust dust = Main.dust[plague];
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 1.8f;
                        dust.velocity.X *= 2f;
                        dust.velocity.Y *= 2f;
                    }
                    else
                    {
                        dust.scale *= 1.3f;
                    }
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= scalar;
                }
                else
                {
                    Projectile.ai[0] += 1f;
                }
            }
            else
            {
                Projectile.damage = (int)(Projectile.damage * 0.6);
                Projectile.velocity *= 0.85f;
                //Fade out
                if (Projectile.alpha < 255)
                    Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }

            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 8;
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
