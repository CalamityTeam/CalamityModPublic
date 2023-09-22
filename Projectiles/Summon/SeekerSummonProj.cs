using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class SeekerSummonProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        public ref float CircleAngleRatio => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 84;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 8;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                DoInitializationEffects();
                Projectile.localAI[0] = 1f;
            }

            ProvidePlayerMinionBuffs();
            DetermineFrames();

            Projectile.alpha = Utils.Clamp(Projectile.alpha - 15, 0, 255);
            NPC potentialTarget = Projectile.Center.MinionHoming(2050f, Owner);
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
                Dust brimstoneFire = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(50f, 50f), 219);
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
            if (Projectile.type != ModContent.ProjectileType<SeekerSummonProj>())
                return;

            if (Owner.dead)
                Owner.Calamity().soulSeeker = false;
            if (Owner.Calamity().soulSeeker)
                Projectile.timeLeft = 2;
        }

        internal void DetermineFrames()
        {
            if (Projectile.FinalExtraUpdate())
                Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 5)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        internal void FlyNearOwner()
        {
            // Make an imp laugh sound every so often if you're the first seeker in the projectile array.
            if (Main.rand.NextBool(1600))
			{
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile seeker = Main.projectile[i];
                    if (seeker.type == Projectile.type)
                    {
                        if (seeker == Projectile)
                            SoundEngine.PlaySound(SoundID.DD2_KoboldFlyerHurt);
                        break;
                    }
                }
			}

            Vector2 destination = Owner.Center + (MathHelper.TwoPi * CircleAngleRatio / Owner.ownedProjectileCounts[Type] - MathHelper.PiOver2).ToRotationVector2() * 310f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.03f);

            if (!Projectile.WithinRange(destination, 20f))
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(destination) * 16f) / 21f;

            if (!Projectile.WithinRange(Owner.Center, 1800f))
            {
                Projectile.Center = Owner.Center;
                Projectile.velocity = -Vector2.UnitY * 4f;
                Projectile.netUpdate = true;
            }

            Projectile.spriteDirection = (destination.X - Projectile.Center.X > 0f).ToDirectionInt();
        }

        internal void AttackTarget(NPC target)
        {
            // Slow down and release a bunch of darts at the enemy for a short interval.
            if (Time % 90f > 45f)
            {
                // Make an imp scream sound every so often.
                if (Main.rand.NextBool(400))
                    SoundEngine.PlaySound(SoundID.DD2_KoboldFlyerChargeScream);

                Projectile.velocity *= 0.9f;
                if (Time % 16f == 15f)
                {
                    float shootSpeed = 23f;
                    Vector2 eyePosition = Projectile.Center + new Vector2(Projectile.spriteDirection * 22f, -12f);
                    Vector2 aheadAim = (target.Center - eyePosition) / target.velocity.Length() / shootSpeed;
                    Vector2 shootVelocity = (target.Center + aheadAim - eyePosition).SafeNormalize(Vector2.UnitX * Projectile.spriteDirection) * shootSpeed;
                    Projectile.spriteDirection = (shootVelocity.X > 0f).ToDirectionInt();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), eyePosition, shootVelocity, ModContent.ProjectileType<BrimstoneDartSummon>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                return;
            }

            Projectile.spriteDirection = (Projectile.velocity.X > 0f).ToDirectionInt();
            if (!Projectile.WithinRange(target.Center, 400f))
                Projectile.velocity = (Projectile.velocity * 10f + Projectile.SafeDirectionTo(target.Center) * 22f) / 11f;
            else if (Projectile.velocity.Length() < 28f)
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 29f;
        }
    }
}
