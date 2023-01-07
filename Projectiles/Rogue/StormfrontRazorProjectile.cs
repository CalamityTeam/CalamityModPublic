using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.Utilities;

namespace CalamityMod.Projectiles.Rogue
{
    public class StormfrontRazorProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/StormfrontRazor";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Razor");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(Main.rand.Next(20, 100), 204, 250), 1f);
                Main.dust[d].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
            DrawOriginOffsetX = 50;
            DrawOriginOffsetY = 20;
            Projectile.ai[0]++;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            Projectile.rotation += Projectile.spriteDirection * MathHelper.ToRadians(45f);
           
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(Main.rand.Next(20, 100), 204, 250), 1f);
                Main.dust[d].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }
            int times = 1;
            if (Projectile.Calamity().stealthStrike)
            {
                times = 3;
            }
            // Create lightning, uses some of Dom's Heavenly Gale code
            for (int i = 0; i < times; i++)
            {
                int lightningDamage = (int)(Projectile.damage * StormfrontRazor.LightningDamageFactor);
                Vector2 lightningSpawnPosition = target.Center - Vector2.UnitY.RotatedByRandom(0.24f) * Main.rand.NextFloat(960f, 1020f);
                Vector2 lightningShootVelocity = (target.Center - lightningSpawnPosition + target.velocity * 7.5f).SafeNormalize(Vector2.UnitY) * 14f;
                int lightning = Projectile.NewProjectile(Projectile.GetSource_FromThis(), lightningSpawnPosition, lightningShootVelocity, ModContent.ProjectileType<StormfrontLightning>(), lightningDamage, 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(lightning))
                {
                    Main.projectile[lightning].CritChance = Projectile.CritChance;
                    Main.projectile[lightning].ai[0] = lightningShootVelocity.ToRotation();
                    Main.projectile[lightning].ai[1] = Main.rand.Next(100);
                    //I'll probably need some delay here
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(Main.rand.Next(20,100), 204, 250), 1f);
                Main.dust[d].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }
            int times = 1;
            if (Projectile.Calamity().stealthStrike)
            {
                times = 3;
            }
            // Create lightning, uses some of Dom's Heavenly Gale code
            for (int i = 0; i < times; i++)
            {
                int lightningDamage = (int)(Projectile.damage * StormfrontRazor.LightningDamageFactor);
                Vector2 lightningSpawnPosition = target.Center - Vector2.UnitY.RotatedByRandom(0.24f) * Main.rand.NextFloat(960f, 1020f);
                Vector2 lightningShootVelocity = (target.Center - lightningSpawnPosition + target.velocity * 7.5f).SafeNormalize(Vector2.UnitY) * 14f;
                int lightning = Projectile.NewProjectile(Projectile.GetSource_FromThis(), lightningSpawnPosition, lightningShootVelocity, ModContent.ProjectileType<StormfrontLightning>(), lightningDamage, 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(lightning))
                {
                    Main.projectile[lightning].CritChance = Projectile.CritChance;
                    Main.projectile[lightning].ai[0] = lightningShootVelocity.ToRotation();
                    Main.projectile[lightning].ai[1] = Main.rand.Next(100);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(Main.rand.Next(20,100), 204, 250), 1f);
                Main.dust[d].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            int times = 1;
            if (Projectile.Calamity().stealthStrike)
            {
                times = 3;
            }
            // Create lightning, uses some of Dom's Heavenly Gale code
            for (int i = 0; i < times; i++)
            {
                int lightningDamage = (int)(Projectile.damage * StormfrontRazor.LightningDamageFactor);
                Vector2 lightningSpawnPosition = Projectile.Center - Vector2.UnitY.RotatedByRandom(0.24f) * Main.rand.NextFloat(960f, 1020f);
                Vector2 lightningShootVelocity = (Projectile.Center - lightningSpawnPosition + Projectile.velocity * 7.5f).SafeNormalize(Vector2.UnitY) * 14f;
                int lightning = Projectile.NewProjectile(Projectile.GetSource_FromThis(), lightningSpawnPosition, lightningShootVelocity, ModContent.ProjectileType<StormfrontLightning>(), lightningDamage, 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(lightning))
                {
                    Main.projectile[lightning].CritChance = Projectile.CritChance;
                    Main.projectile[lightning].ai[0] = lightningShootVelocity.ToRotation();
                    Main.projectile[lightning].ai[1] = Main.rand.Next(100);
                }
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
