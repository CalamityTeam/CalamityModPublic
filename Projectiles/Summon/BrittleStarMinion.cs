using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BrittleStarMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(EnemyDistanceDetection, Owner);
        public float EnemyDistanceDetection = 1200f;
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 28;
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
            Owner.AddBuff(ModContent.BuffType<BrittleStar>(), 1);
            if (Projectile.type == ModContent.ProjectileType<BrittleStarMinion>())
            {
                if (Owner.dead)
                {
                    ModdedOwner.brittleStar = false;
                }
                if (ModdedOwner.brittleStar)
                {
                    Projectile.timeLeft = 2;
                }
            }

            Projectile.rotation += Projectile.velocity.X * 0.04f; // Spins faster the faster it moves in the X-axis.

            Projectile.ChargingMinionAI(EnemyDistanceDetection, 1500f, 2200f, 150f, 0, 24f, 15f, 4f, new Vector2(0f, -60f), 12f, 12f, Target.IsABoss(), true, 1);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
