using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class GraveGrimreaverProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GraveGrimreaver";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grave Grimreaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 310;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 22;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            if (Projectile.timeLeft % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.2f, ModContent.ProjectileType<GrimreaverSkull>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f, 0f);
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 4f, 14f);

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation += 0.1f * Projectile.spriteDirection;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Confused, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Confused, 180);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            if (Projectile.Calamity().stealthStrike)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = SoundID.NPCDeath52.Volume * 0.75f }, Projectile.Center);
                SpawnBats(20, -12, 12); //"At least 20"...
                DustExplosion(15, 6, 12, 30, 2.4f);
                for (int i = 0; i < 10; i++)
                {
                    CalamityUtils.ProjectileRain(Projectile.GetSource_FromThis(), Projectile.Center, 600f, 100f, 700f, 1000f, 20, ModContent.ProjectileType<GrimreaverSkull>(), (int)(Projectile.damage * 0.75f), 3f, Projectile.owner);
                }
            }
            else
            {
                SpawnBats(4, -12, 12);
                DustExplosion(10, 3, 9, 20, 2.15f);
            }
        }

        public void SpawnBats(int amount, int minspread, int maxspread)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < amount; i++)
                {
                    Vector2 speed = new Vector2(Main.rand.NextFloat(minspread, maxspread), Main.rand.NextFloat(minspread, maxspread));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<GrimreaverBat>(), (int)(Projectile.damage * 0.25f), 0f, Projectile.owner, 4);
                }
            }
        }

        public void DustExplosion(int spreadspeed, int minspeed, int maxspeed, int amount, float size)
        {
            for (int i = 0; i < amount; i++)
            {
                float num463 = (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                float num464 = (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                float num465 = (float)Main.rand.Next(minspeed, maxspeed);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 75, 0, 0, 0, default, size);
                Dust dust = Main.dust[d];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                dust.position.Y += (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                dust.velocity.X = num463;
                dust.velocity.Y = num464;
            }
        }
    }
}
