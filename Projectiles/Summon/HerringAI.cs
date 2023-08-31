using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HerringAI : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public float EnemyDistanceDetection = 1200f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1f;
            Projectile.localNPCHitCooldown = 35;
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 24;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void AI()
        {
            CheckMinionExistance(); // Checks if the minion can still exist.

            Projectile.rotation = (Projectile.spriteDirection == -1) ? Projectile.velocity.ToRotation() + MathHelper.Pi : Projectile.velocity.ToRotation();
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            // Although the sprite is invisible, we'll keep track of it's direction and rotation so we can later apply it to the decorative herrings.

            Projectile.ChargingMinionAI(EnemyDistanceDetection, 1500f, 2200f, 150f, 0, 24f, 15f, 4f, new Vector2(0f, -60f), 12f, 12f, CalamityPlayer.areThereAnyDamnBosses, true, 1); // The AI of the minion.

            Projectile.netUpdate = true;
        }
        
        public void CheckMinionExistance()
        {
            Owner.AddBuff(ModContent.BuffType<Herring>(), 1);
            if (Projectile.type == ModContent.ProjectileType<HerringAI>())
            {
                if (Owner.dead)
                    ModdedOwner.herring = false;
                if (ModdedOwner.herring)
                    Projectile.timeLeft = 2;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Spawns the decorative fish.
            int herringAmount = 3;
            for (int herringIndex = 0; herringIndex < herringAmount; herringIndex++)
            {
                float angle = MathHelper.TwoPi / herringAmount * herringIndex;
                Vector2 velocity = angle.ToRotationVector2();

                // We'll keep track of the index of the invisible minion and the offset in which each herring spawns so we can later apply it to the decorative herrings.
                int herring = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<HerringMinion>(), 0, 0f, Projectile.owner, Projectile.whoAmI, angle);

                if (Main.projectile.IndexInRange(herring))
                    Main.projectile[herring].originalDamage = 0;
            }

            // Makes the dust spawn effect.
            int dustAmount = 100;
            for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
            {
                float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                Vector2 velocity = angle.ToRotationVector2() * 20f;
                Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, DustID.Water, velocity);
                spawnDust.customData = false;
                spawnDust.velocity *= 0.3f;
                spawnDust.scale = velocity.Length() * 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
