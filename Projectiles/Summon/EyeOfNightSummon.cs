using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EyeOfNightSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        public NPC Target => Owner.Center.MinionHoming(750f, Owner, CalamityPlayer.areThereAnyDamnBosses);
        public ref float HoverTime => ref Projectile.ai[0];

        public const int ShootRate = 60;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            ProvidePlayerMinionBuffs();
            GenerateVisuals();
            if (Target is null)
                FlyNearOwner();
            else
                AttackTarget(Target);
        }

        internal void ProvidePlayerMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<EyeOfNightBuff>(), 3600);

            // Verify player/minion state integrity. The minion cannot stay alive if the
            // owner is dead or if the caller of the AI is invalid.
            if (Projectile.type != ModContent.ProjectileType<EyeOfNightSummon>())
                return;

            if (Owner.dead)
                Owner.Calamity().eyeOfNight = false;
            if (Owner.Calamity().eyeOfNight)
                Projectile.timeLeft = 2;
        }

        internal void GenerateVisuals()
        {
            // The code within this method is visual. There is no need to waste resources executing it on the server.
            if (Main.dedServ)
                return;

            Projectile.rotation += Projectile.velocity.X * 0.075f;
        }

        internal void FlyNearOwner()
        {
            Vector2 destination = Owner.Top - Vector2.UnitY * 45f + (Projectile.identity * 0.9f).ToRotationVector2() * 16f;
            Vector2 idealVelocity = Projectile.SafeDirectionTo(destination) * MathHelper.Lerp(2.3f, 8f, Utils.GetLerpValue(16f, 160f, Projectile.Distance(destination)));
            if (Projectile.velocity.Length() < 0.4f)
                Projectile.velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(0.5f, 1.1f) * Main.rand.NextBool().ToDirectionInt()) * -3.6f;
            else if (!Projectile.WithinRange(destination, 20f))
                Projectile.velocity = Projectile.velocity * 0.9f + idealVelocity * 0.1f;

            if (!Projectile.WithinRange(Owner.Center, 1800f))
            {
                Projectile.Center = Owner.Center;
                Projectile.velocity = -Vector2.UnitY * 4f;
                Projectile.netUpdate = true;
            }

            // Slow down a bit over time.
            Projectile.velocity *= 0.985f;
        }

        internal void AttackTarget(NPC target)
        {
            if (Main.myPlayer == Projectile.owner && HoverTime % 70f == 69f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 8f, ModContent.ProjectileType<EyeOfNightCell>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                HoverTime++;
            }

            Vector2 destination = target.Center;
            Vector2 destinationOffset = Vector2.Max(target.Size * 1.2f, Vector2.One * 90f).RotatedBy(Projectile.identity * 0.96f + HoverTime / 15f);

            // Make the offset pulsate over time.
            destinationOffset *= MathHelper.Lerp(0.7f, 1.3f, (float)Math.Cos(Projectile.identity * 1.11f + HoverTime / 14f) * 0.5f + 0.5f);

            destinationOffset.Y += (float)Math.Sin(Projectile.identity * 1.16f + HoverTime / 15f + MathHelper.PiOver2) * MathHelper.Min(target.height * 0.8f, 70f);

            destination += destinationOffset;

            // Fly towards the destination faster the farther the eye already is to it.
            float flySpeed = MathHelper.Lerp(5f, 15f, Utils.GetLerpValue(40f, 250f, Projectile.Distance(destination), true));

            if (Projectile.WithinRange(destination, 24f + target.velocity.Length() * 2f))
                HoverTime++;

            Projectile.velocity = (destination - Projectile.Center).SafeNormalize(Vector2.Zero) * flySpeed;
        }

        public override bool? CanDamage() => false;
    }
}
