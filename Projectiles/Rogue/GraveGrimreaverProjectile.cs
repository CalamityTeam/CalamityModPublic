using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
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
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 299;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            if (Projectile.timeLeft % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.2f, ModContent.ProjectileType<GrimreaverSkull>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f, 0f);
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 4f, 14f);

            Projectile.rotation += 0.07f;
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
                SpawnBats(20, 10, 30); //"At least 20"...
                for (int i = 0; i < 10; i++)
                CalamityUtils.ProjectileRain(Projectile.GetSource_FromThis(), Projectile.Center, 400f, 100f, 500f, 800f, 20, ModContent.ProjectileType<GrimreaverSkull>(), (int)(Projectile.damage * 0.75f), 3f, Projectile.owner);
            }
            else
            {
                SpawnBats(4, 30, 60);
            }
        }

        public void SpawnBats(int amount, int minspread, int maxspread)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < amount; i++)
                {
                    float rotation = MathHelper.ToRadians(Main.rand.Next(minspread, maxspread));
                    Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (amount - 1))) * 2;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<GrimreaverBat>(), (int)(Projectile.damage * 0.25f), 0f, Projectile.owner);
                }
            }
        }
    }
}
