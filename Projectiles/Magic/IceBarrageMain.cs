using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class IceBarrageMain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int pwidth = 58;
        private const int pheight = 58;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = pwidth;
            Projectile.height = pheight;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 280;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            NPC closestTarget = Projectile.Center.ClosestNPCAt(5000f, true, true);
            if (closestTarget != null)
                Projectile.Center = closestTarget.Center;

            Timer++;
            for (int j = 0; j < 3; j++)
            {
                int dustType = Main.rand.NextBool() ? DustID.BlueCrystalShard : Main.rand.NextBool(4) ? DustID.Ice : DustID.IceRod;
                if (Timer < 140f)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, Main.rand.NextVector2Circular(-4f, 4f), 50, default, 1.3f);
                    dust.noGravity = true;
                }
                else
                {
                    int direct = Main.rand.NextBool() ? 1 : -1;
                    Vector2 dustSpawn = Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height));

                    Dust dust1 = Dust.NewDustPerfect(dustSpawn, dustType, Vector2.UnitY * 10f * direct, 50, default, 1.3f);
                    dust1.noGravity = true;
                    direct = Main.rand.NextBool() ? 1 : -1;
                    Dust dust2 = Dust.NewDustPerfect(dustSpawn, dustType, Vector2.UnitX * 10f * direct, 50, default, 1.3f);
                    dust2.noGravity = true;
                }
            }

            if (Timer < 55)
            {
                for (int i = 0; i < 9; i++)
                {
                    int auraDustType = Main.rand.NextBool() ? DustID.BlueCrystalShard : Main.rand.NextBool(4) ? DustID.Ice : DustID.IceRod;
                    Vector2 auraDustPos = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(250f, 270f);
                    Vector2 auraDustSpeed = Vector2.Normalize(Projectile.Center - auraDustPos) * 0.5f;

                    Dust auraDust = Dust.NewDustPerfect(auraDustPos, auraDustType, auraDustSpeed, Scale: Main.rand.NextFloat(1.5f, 2f));
                    auraDust.noGravity = true;
                }
            }
            else if (Timer == 55f)
            {
                for (int i = 0; i < 210; i++)
                {
                    int inwardDustType = Main.rand.NextBool() ? DustID.BlueCrystalShard : Main.rand.NextBool(4) ? DustID.Ice : DustID.IceRod;
                    Vector2 inwardDustPos = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(250f, 270f);
                    Vector2 inwardDustSpeed = Vector2.Normalize(Projectile.Center - inwardDustPos) * Main.rand.NextFloat(8f, 34f);

                    Dust inwardDust = Dust.NewDustPerfect(inwardDustPos, inwardDustType, inwardDustSpeed, Scale: Main.rand.NextFloat(1.5f, 2f));
                    inwardDust.noGravity = true;
                }

            }
            else if (Timer == 140f)
            {
                Vector2 projcenter = Projectile.Center;
                Projectile.width = 200;
                Projectile.height = 200;
                Projectile.Center = projcenter;
                Projectile.Damage();

                for (int i = 0; i < 150; i++)
                {
                    int outwardDustType = Main.rand.NextBool() ? DustID.BlueCrystalShard : Main.rand.NextBool(4) ? DustID.Ice : DustID.IceRod;
                    Dust outwardDust = Dust.NewDustPerfect(Projectile.Center, outwardDustType, Main.rand.NextVector2Circular(-18f, 18f), 50, default, 1.5f);
                    outwardDust.noGravity = true;
                }
                Projectile.width = pwidth;
                Projectile.height = pheight;
                Projectile.Center = projcenter;

                // Bottom
                Vector2 pos1 = new Vector2(Projectile.Center.X, Projectile.Center.Y + (Projectile.height * 0.5f) + 20f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos1, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 0f);
                // Left
                Vector2 pos2 = new Vector2(Projectile.Center.X - (Projectile.width * 0.5f) - 20f, Projectile.Center.Y);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos2, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 1f);
                // Top
                Vector2 pos3 = new Vector2(Projectile.Center.X, Projectile.Center.Y - (Projectile.height * 0.5f) - 20f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos3, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 2f);
                // Right
                Vector2 pos4 = new Vector2(Projectile.Center.X + (Projectile.width * 0.5f) + 20f, Projectile.Center.Y);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos4, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 3f);
            }

            if (Timer > 90)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= 5f)
                {
                    Vector2 spawnPos = Projectile.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), -400f);
                    int ice = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Vector2.UnitY * 6f, ProjectileID.NorthPoleSnowflake, (int)(Projectile.damage * 0.05f), 2f, Projectile.owner, 0f, Main.rand.Next(3));
                    if (ice.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[ice].tileCollide = false;
                        Main.projectile[ice].DamageType = DamageClass.Magic;
                    }
                    Projectile.ai[1] = 0f;
                }
            }
        }

        public override bool? CanDamage() => Timer == 140f ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frozen, 60);
    }
}
