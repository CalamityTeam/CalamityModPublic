using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EyeOfNightSummon : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float HoverTime => ref projectile.ai[0];

        public const int ShootRate = 60;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Night");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 19;
        }

        public override void AI()
        {
            ProvidePlayerMinionBuffs();
            DynamicallyUpdateDamage();
            GenerateVisuals();
            NPC potentialTarget = projectile.Center.MinionHoming(750f, Owner);
            if (potentialTarget is null)
                FlyNearOwner();
            else
                AttackTarget(potentialTarget);
        }

        internal void ProvidePlayerMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<EyeOfNightBuff>(), 3600);

            // Verify player/minion state integrity. The minion cannot stay alive if the
            // owner is dead or if the caller of the AI is invalid.
            if (projectile.type != ModContent.ProjectileType<EyeOfNightSummon>())
                return;

            if (Owner.dead)
                Owner.Calamity().eyeOfNight = false;
            if (Owner.Calamity().eyeOfNight)
                projectile.timeLeft = 2;
        }

        internal void DynamicallyUpdateDamage()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if (Owner.MinionDamage() == projectile.Calamity().spawnedPlayerMinionDamageValue)
                return;
            int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue / projectile.Calamity().spawnedPlayerMinionDamageValue * Owner.MinionDamage());
            projectile.damage = trueDamage;
        }

        internal void GenerateVisuals()
        {
            // The code within this method is visual. There is no need to waste resources executing it on the server.
            if (Main.dedServ)
                return;

            projectile.rotation += projectile.velocity.X * 0.075f;
        }

        internal void FlyNearOwner()
        {
            Vector2 destination = Owner.Top - Vector2.UnitY * 45f + (projectile.identity * 0.9f).ToRotationVector2() * 16f;
            Vector2 idealVelocity = projectile.SafeDirectionTo(destination) * MathHelper.Lerp(2.3f, 8f, Utils.InverseLerp(16f, 160f, projectile.Distance(destination)));
            if (projectile.velocity.Length() < 0.4f)
                projectile.velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(0.5f, 1.1f) * Main.rand.NextBool(2).ToDirectionInt()) * -3.6f;
            else if (!projectile.WithinRange(destination, 20f))
                projectile.velocity = projectile.velocity * 0.9f + idealVelocity * 0.1f;

            if (!projectile.WithinRange(Owner.Center, 1800f))
            {
                projectile.Center = Owner.Center;
                projectile.velocity = -Vector2.UnitY * 4f;
                projectile.netUpdate = true;
            }

            // Slow down a bit over time.
            projectile.velocity *= 0.985f;
        }

        internal void AttackTarget(NPC target)
        {
            if (Main.myPlayer == projectile.owner && HoverTime % 70f == 69f)
            {
                Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(target.Center) * 8f, ModContent.ProjectileType<EyeOfNightCell>(), projectile.damage, projectile.knockBack, projectile.owner);
                HoverTime++;
            }

            Vector2 destination = target.Center;
            Vector2 destinationOffset = Vector2.Max(target.Size * 1.2f, Vector2.One * 90f).RotatedBy(projectile.identity * 0.96f + HoverTime / 15f);

            // Make the offset pulsate over time.
            destinationOffset *= MathHelper.Lerp(0.7f, 1.3f, (float)Math.Cos(projectile.identity * 1.11f + HoverTime / 14f) * 0.5f + 0.5f);

            destinationOffset.Y += (float)Math.Sin(projectile.identity * 1.16f + HoverTime / 15f + MathHelper.PiOver2) * MathHelper.Min(target.height * 0.8f, 70f);

            destination += destinationOffset;

            // Fly towards the destination faster the farther the eye already is to it.
            float flySpeed = MathHelper.Lerp(5f, 15f, Utils.InverseLerp(40f, 250f, projectile.Distance(destination), true));

            if (projectile.WithinRange(destination, 24f + target.velocity.Length() * 2f))
                HoverTime++;

            projectile.velocity = (destination - projectile.Center).SafeNormalize(Vector2.Zero) * flySpeed;
        }

        public override bool CanDamage() => false;
    }
}