using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SarosAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer moddedOwner => Owner.Calamity();

        public ref float GeneralTimer => ref Projectile.ai[0];

        public bool CheckForSpawning = false;

        public static readonly SoundStyle SarosDiskThrow = new SoundStyle("CalamityMod/Sounds/Item/SarosDiskThrow", 3) { Volume = 0.4f, PitchVariance = 1 };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 8f;
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 132;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void AI()
        {
            NPC potentialTarget = Projectile.Center.MinionHoming(5000f, Owner);

            CheckMinionExistince(); // Ensure that the projectile using this AI is the correct projectile and that the owner has the appropriate buffs.
            SpawnEffect(); // Makes a dust spawn effect where the minion spawns.

            // Attack nearby targets.
            if (potentialTarget != null && Main.myPlayer == Projectile.owner)
                AttackTarget(potentialTarget);

            // Stay near the target and spin around.
            Projectile.Center = Owner.Center - Vector2.UnitY * 16f;
            // The projectile spins right at a constant speed.
            Projectile.rotation += MathHelper.ToRadians(1.5f);

            // Emit some light.
            Lighting.AddLight(Projectile.Center, Vector3.One * 1.2f);

            // A timer for the AI.
            GeneralTimer++;
        }

        #region Methods

        public void CheckMinionExistince()
        {
            Owner.AddBuff(ModContent.BuffType<SarosPossessionBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<SarosAura>())
            {
                if (Owner.dead)
                    moddedOwner.saros = false;
                if (moddedOwner.saros)
                    Projectile.timeLeft = 2;
            }
        }

        public void SpawnEffect()
        {
            if (CheckForSpawning == false)
            {
                int dustAmount = 360;
                for (int d = 0; d < dustAmount; d++)
                {
                    float angle = MathHelper.TwoPi / dustAmount * d;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(20f, 30f);

                    Dust spawnDust = Dust.NewDustPerfect(Owner.Center - Vector2.UnitY * 60f, (int)CalamityDusts.ProfanedFire, velocity);
                    spawnDust.noGravity = true;
                    spawnDust.color = Color.Lerp(Color.White, Color.Yellow, 0.25f);
                    spawnDust.scale = velocity.Length() * 0.25f;
                    spawnDust.velocity *= 0.7f;
                }
            }
            CheckForSpawning = true;
        }

        public void AttackTarget(NPC target)
        {
            if (GeneralTimer % 50f == 49f)
            {
                for (int i = -1; i < 2; i++)
                {
                    float angle = (target.Center - Projectile.Center).ToRotation() + (MathHelper.PiOver2 * i) + Main.rand.NextFloat(MathHelper.PiOver4, -MathHelper.PiOver4);
                    Vector2 velocity = angle.ToRotationVector2() * 30f;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SarosSunfire>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    Projectile.netUpdate = true;
                }
            }

            if (GeneralTimer % 100f == 99f)
            {
                float angle = (target.Center - Projectile.Center).ToRotation() + Main.rand.NextFloat(MathHelper.PiOver2, -MathHelper.PiOver2);
                Vector2 velocity = angle.ToRotationVector2() * 25f;
                SoundEngine.PlaySound(SarosDiskThrow, Projectile.Center);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SarosMicrosun>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);

                Projectile.netUpdate = true;
            }
        }

        public override bool? CanDamage() => false;

        #endregion
    }
}
