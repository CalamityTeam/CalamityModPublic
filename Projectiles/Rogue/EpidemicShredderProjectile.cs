using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class EpidemicShredderProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EpidemicShredder";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Epidemic Shredder");
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += Math.Sign(Projectile.velocity.X) * MathHelper.ToRadians(10f);
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
            }
            if (Projectile.timeLeft < 160f)
            {
                Projectile.velocity = (Projectile.velocity * 18f + Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 18f) / 19f;
                if (Main.player[Projectile.owner].Hitbox.Intersects(Projectile.Hitbox))
                    Projectile.Kill();
            }
            if (Projectile.timeLeft % 4 == 0 && Projectile.Calamity().stealthStrike)
            {
                int projIndex2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.velocity * -1f).RotatedByRandom(MathHelper.ToRadians(15f)), ModContent.ProjectileType<PlagueSeeker>(), (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.6f, Projectile.owner);
                if (projIndex2.WithinBounds(Main.maxProjectiles))
                    Main.projectile[projIndex2].DamageType = RogueDamageClass.Instance;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate > 1)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                SpawnSeeker();
                Projectile.penetrate--;
            }
            else
                Projectile.tileCollide = false;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SpawnSeeker();
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            SpawnSeeker();
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public void SpawnSeeker()
        {
            if (Projectile.ai[0] == 0f)
            {
                int projectileIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PlagueSeeker>(), (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.6f, Projectile.owner);
                if (projectileIndex.WithinBounds(Main.maxProjectiles))
                    Main.projectile[projectileIndex].DamageType = RogueDamageClass.Instance;
                Projectile.ai[0] = 12f; //0.2th of a second cooldown
            }
        }
    }
}
