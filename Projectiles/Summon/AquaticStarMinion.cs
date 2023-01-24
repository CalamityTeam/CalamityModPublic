using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AquaticStarMinion : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer moddedOwner => Owner.Calamity();

        public ref float CheckForSpawning => ref Projectile.localAI[0];
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Star");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 35;
        }

        public override void AI()
        {
            Owner.AddBuff(ModContent.BuffType<AquaticStar>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<AquaticStarMinion>())
            {
                if (Owner.dead)
                    moddedOwner.aquaticStar = false;
                if (moddedOwner.aquaticStar)
                    Projectile.timeLeft = 2;
            }
            // Checks if the minion can still exist.

            if (CheckForSpawning == 0f)
            {
                for (int i = 0; i < 45; i++)
                {
                    float angle = MathHelper.TwoPi / 45f * i;
                    Vector2 direction = angle.ToRotationVector2() * 10f;

                    Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 33, direction);
                    spawnDust.noGravity = true;
                }
                CheckForSpawning++;
            }
            // Does a dust effect where the minion spawns.

            Projectile.rotation += Projectile.velocity.X * 0.04f;
            // Make the minion spin depending on how fast it is.

            Projectile.ChargingMinionAI(1200f, 1500f, 2200f, 150f, 0, 24f, 15f, 4f, new Vector2(0f, -60f), 12f, 12f, false, false, 1);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
