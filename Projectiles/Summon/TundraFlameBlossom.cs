using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class TundraFlameBlossom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer moddedOwner => Owner.Calamity();

        public ref float FlowerShootTimer => ref Projectile.ai[0];

        public ref float RotationMovement => ref Projectile.ai[1];
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            NPC potentialTarget = Projectile.Center.MinionHoming(1200f, Owner);

            CheckMinionExistance(); // Checks if the minion can still exist.
            SpawnEffect(); // Makes a dust effect when spawning.
            TargetNPC(potentialTarget); // Targets the NPC.

            Lighting.AddLight(Projectile.Center, Color.Fuchsia.ToVector3()); // Makes a light with the same color as the flowers.
            Projectile.Center = Owner.Center + RotationMovement.ToRotationVector2() * 100f; // Spins around the player.
            Projectile.rotation += MathHelper.ToRadians(6.25f * Owner.direction); // Rotates around itself in the direction of the owner
            Projectile.scale = MathHelper.Lerp(1f, 1.005f, FlowerShootTimer % 100f); // Expands the closer it is to shooting, goes back to normal once shot; peridoically.
            RotationMovement += MathHelper.ToRadians(1.25f * Owner.direction); // The changing variable that moves the flower, changes directions depending on the player.
        }

        #region Methods

        public void CheckMinionExistance()
        {
            Owner.AddBuff(ModContent.BuffType<TundraFlameBlossomsBuff>(), 1);
            if (Projectile.type == ModContent.ProjectileType<TundraFlameBlossom>())
            {
                if (Owner.dead)
                    moddedOwner.tundraFlameBlossom = false;
                if (moddedOwner.tundraFlameBlossom)
                    Projectile.timeLeft = 2;
            }
        }

        public void SpawnEffect()
        {
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust spawnEffect = Dust.NewDustPerfect(Projectile.Center, 179);
                    spawnEffect.noGravity = true;
                    spawnEffect.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 7f);
                }
                Projectile.localAI[0] += 1f;
            }
        }

        public void ShootFlowers()
        {
            SoundEngine.PlaySound(SoundID.Item20, Owner.Center);
            Vector2 velocity = -Projectile.SafeDirectionTo(Owner.Center) * 10f + Projectile.velocity;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<TundraFlameBlossomsOrb>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.netUpdate = true;
        }

        public void TargetNPC(NPC target)
        {
            if (target != null)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    FlowerShootTimer++;
                    if (FlowerShootTimer % 100f == 0f)
                        ShootFlowers();
                    FlowerShootTimer = (FlowerShootTimer == 101f) ? 1f : FlowerShootTimer;
                }
            }
            else
                FlowerShootTimer--;

            FlowerShootTimer = MathHelper.Clamp(FlowerShootTimer, 0f, 101f);
            Projectile.netUpdate = true;
        }

        public override bool? CanDamage() => false;

        #endregion
    }
}
