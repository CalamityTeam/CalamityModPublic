using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class GraveGrimreaverProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GraveGrimreaver";

        public override void SetStaticDefaults()
        {
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
            Projectile.timeLeft = 210;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 22;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            //Spawn a skull every 30 frames
            if (Projectile.timeLeft % 30 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.2f, ModContent.ProjectileType<GrimreaverSkull>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f, 0f);
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 4f, 14f);

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 1).ToDirectionInt();
            Projectile.rotation += 0.1f * Projectile.spriteDirection;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Confused, 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Confused, 180);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            if (Projectile.Calamity().stealthStrike)
            {
                // Stealth strikes play a wraith death noise, summon a larger dust explosion, a swarm of 20 bats, and a rain of 10 skulls
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
                // Normal strikes play no extra sound, have a smaller dust explosion, and summon 4 bats 
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
                float random1 = (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                float random2 = (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                float random3 = (float)Main.rand.Next(minspeed, maxspeed);
                float randomAdjust = (float)Math.Sqrt((double)(random1 * random1 + random2 * random2));
                randomAdjust = random3 / randomAdjust;
                random1 *= randomAdjust;
                random2 *= randomAdjust;
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 75, 0, 0, 0, default, size);
                Dust dust = Main.dust[d];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                dust.position.Y += (float)Main.rand.Next(-spreadspeed, spreadspeed + 1);
                dust.velocity.X = random1;
                dust.velocity.Y = random2;
            }
        }
    }
}
