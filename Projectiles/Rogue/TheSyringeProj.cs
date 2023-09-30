using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class TheSyringeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheSyringe";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.light = 0.5f;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(8))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.Next(2) == 1 ? 107 : 89, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);

            // TODO -- This will almost never work due to the base damage being too low, and will round down.
            Projectile.damage += Projectile.originalDamage / 200;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 240);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Plague>(), 240);

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(100);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item107, Projectile.position);
            for (int k = 0; k < 7; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 89, Projectile.oldVelocity.X, Projectile.oldVelocity.Y);
            }
            int fireAmt = Main.rand.Next(1, 3);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int f = 0; f < fireAmt; f++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<TheSyringeCinder>(), (int)(Projectile.damage * 0.5), 0f, Main.myPlayer);
                }
                for (int s = 0; s < 2; ++s)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.NextFloat(0.4f, 0.7f) + Main.rand.NextFloat(-8f, 8f);
                    float SpeedY = -Projectile.velocity.Y * Main.rand.NextFloat(0.4f, 0.7f) + Main.rand.NextFloat(-8f, 8f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + SpeedX, Projectile.Center.Y + SpeedY, SpeedX, SpeedY, ModContent.ProjectileType<TheSyringeS1>(), (int)(Projectile.damage * 0.25), 0f, Main.myPlayer, Main.rand.Next(3), 0f);
                }
            }
            if (Projectile.owner == Main.myPlayer && Projectile.ai[1] == 1)
            {
                for (int b = 0; b < 5; b++)
                {
                    float speedX = Main.rand.NextFloat(-0.7f, 0.7f);
                    float speedY = Main.rand.NextFloat(-0.7f, 0.7f);
                    int bee = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, speedX, speedY, ModContent.ProjectileType<PlaguenadeBee>(), (int)(Projectile.damage * 0.5), 0f, Main.myPlayer);
                    Main.projectile[bee].penetrate = 1;
                }
            }
        }
    }
}
