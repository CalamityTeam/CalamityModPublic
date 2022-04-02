using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SeekerSummonProj : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float CircleAngleRatio => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Seeker");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 84;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = projectile.MaxUpdates * 8;
            projectile.alpha = 255;
        }


        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                DoInitializationEffects();
                projectile.localAI[0] = 1f;
            }

            ProvidePlayerMinionBuffs();
            DetermineFrames();

            projectile.alpha = Utils.Clamp(projectile.alpha - 15, 0, 255);
            NPC potentialTarget = projectile.Center.MinionHoming(2050f, Owner);
            if (potentialTarget is null)
                FlyNearOwner();
            else
                AttackTarget(potentialTarget);
            Time++;
        }

        internal void DoInitializationEffects()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 40; i++)
            {
                Dust brimstoneFire = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(50f, 50f), DustID.Fireworks);
                brimstoneFire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 5f);
                brimstoneFire.scale = 1f + brimstoneFire.velocity.Length() * 0.1f;
                brimstoneFire.color = Color.Lerp(Color.White, Color.OrangeRed, Main.rand.NextFloat());
                brimstoneFire.noGravity = true;
            }
        }

        internal void ProvidePlayerMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<SoulSeekerBuff>(), 3600);

            // Verify player/minion state integrity. The minion cannot stay alive if the
            // owner is dead or if the caller of the AI is invalid.
            if (projectile.type != ModContent.ProjectileType<SeekerSummonProj>())
                return;

            if (Owner.dead)
                Owner.Calamity().soulSeeker = false;
            if (Owner.Calamity().soulSeeker)
                projectile.timeLeft = 2;
        }

        internal void DetermineFrames()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 5 == 4)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }

        internal void FlyNearOwner()
        {
            // Make an imp laugh sound every so often.
            if (Main.rand.NextBool(1600))
                Main.PlaySound(SoundID.DD2_KoboldFlyerHurt);

            Vector2 destination = Owner.Center + (MathHelper.TwoPi * CircleAngleRatio - MathHelper.PiOver2).ToRotationVector2() * 310f;
            projectile.Center = Vector2.Lerp(projectile.Center, destination, 0.03f);

            if (!projectile.WithinRange(destination, 20f))
                projectile.velocity = (projectile.velocity * 20f + projectile.SafeDirectionTo(destination) * 16f) / 21f;

            if (!projectile.WithinRange(Owner.Center, 1800f))
            {
                projectile.Center = Owner.Center;
                projectile.velocity = -Vector2.UnitY * 4f;
                projectile.netUpdate = true;
            }

            projectile.spriteDirection = (destination.X - projectile.Center.X > 0f).ToDirectionInt();
        }

        internal void AttackTarget(NPC target)
        {
            // Slow down and release a bunch of darts at the enemy for a short interval.
            if (Time % 90f > 45f)
            {
                // Make an imp scream sound every so often.
                if (Main.rand.NextBool(400))
                    Main.PlaySound(SoundID.DD2_KoboldFlyerChargeScream);

                projectile.velocity *= 0.9f;
                if (Time % 16f == 15f)
                {
                    float shootSpeed = 23f;
                    Vector2 eyePosition = projectile.Center + new Vector2(projectile.spriteDirection * 22f, -12f);
                    Vector2 aheadAim = (target.Center - eyePosition) / target.velocity.Length() / shootSpeed;
                    Vector2 shootVelocity = (target.Center + aheadAim - eyePosition).SafeNormalize(Vector2.UnitX * projectile.spriteDirection) * shootSpeed;
                    projectile.spriteDirection = (shootVelocity.X > 0f).ToDirectionInt();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(eyePosition, shootVelocity, ModContent.ProjectileType<BrimstoneDartSummon>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
                return;
            }

            projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
            if (!projectile.WithinRange(target.Center, 400f))
                projectile.velocity = (projectile.velocity * 10f + projectile.SafeDirectionTo(target.Center) * 22f) / 11f;
            else if (projectile.velocity.Length() < 28f)
                projectile.velocity = projectile.SafeDirectionTo(target.Center) * 29f;
        }
    }
}
