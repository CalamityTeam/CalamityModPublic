using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DeathstareEyeball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        // This does not need to be synced. It is used solely for drawcode/visuals and is intended to be local.
        public float PupilScale = 1f;
        public const int BeamFireRate = 60;
        public Player Owner => Main.player[Projectile.owner];
        public NPC Target => Owner.Center.MinionHoming(720f, Owner, CalamityPlayer.areThereAnyDamnBosses);
        public ref float Time => ref Projectile.ai[1];
        public ref float PupilAngle => ref Projectile.localAI[0];
        public ref float PupilOutwardness => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.NeedsUUID[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        #region AI
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3());

            bool isCorrectMinion = Projectile.type == ModContent.ProjectileType<DeathstareEyeball>();
            CalamityPlayer modPlayer = Owner.Calamity();
            Owner.AddBuff(ModContent.BuffType<MiniatureEyeofCthulhu>(), 3600);

            // Ensure that the projectile executing this code is a valid one.
            if (isCorrectMinion)
            {
                if (Owner.dead)
                    modPlayer.deathstareEyeball = false;
                if (modPlayer.deathstareEyeball)
                    Projectile.timeLeft = 2;
            }

            Vector2 destination = Owner.Center + Vector2.UnitY * (Owner.gfxOffY - 110f);
            if (Owner.gravDir == -1f)
                destination.Y += 220f;

            // Very quickly fly towards above (or below if gravity is reversed) the player.
            Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.36f);

            Projectile.position = Projectile.position.Floor();
            Projectile.rotation = (Projectile.position.X - Projectile.oldPosition.X) * 0.07f + Owner.velocity.X * 0.2f;
            Projectile.rotation = Utils.Clamp(Projectile.rotation, -0.4f, 0.4f);

            if (Projectile.ai[0] == 0f)
            {
                Initialize(Owner);
                Projectile.ai[0] = 1f;
            }
            
            if (Target is null)
                DoHoveringAI();
            else
                DoAttackingAI(Target);

            Projectile.frame = (int)(Time / 5) % 4;

            Time++;
        }

        public void DoHoveringAI()
        {
            // Roll the pupil around if there's little movement from the eye and player.
            if (Time % 180f > 120f && Math.Abs(Projectile.rotation) < 0.03f)
            {
                float idealAngle = Utils.GetLerpValue(120f, 180f, Time % 180f, true) * MathHelper.TwoPi;

                PupilAngle = PupilAngle.AngleTowards(idealAngle, MathHelper.ToRadians(12f));
                PupilOutwardness = MathHelper.Lerp(PupilOutwardness, 4f, 0.2f);
            }
            else
            {
                float idealOutwardness = MathHelper.Clamp(Owner.velocity.Length() * 0.5f, 0f, 5f);
                float idealAngle = Owner.velocity.ToRotation();

                PupilOutwardness = MathHelper.Lerp(PupilOutwardness, idealOutwardness, 0.2f);
                PupilAngle = PupilAngle.AngleTowards(idealAngle, MathHelper.ToRadians(12f));
            }
        }

        public void DoAttackingAI(NPC target)
        {
            PupilOutwardness = MathHelper.Lerp(PupilOutwardness, 4f, 0.25f);

            if (Time % BeamFireRate == BeamFireRate - 20)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = (target.Center - Projectile.Center) / 15f;
                    int beam = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<DeathstareBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (Main.projectile.IndexInRange(beam))
                    {
                        Main.projectile[beam].ai[0] = Projectile.identity;
                        Main.projectile[beam].Damage();
                    }
                }
                PupilScale = 1.5f;
            }

            // After firing, have the pupil retract a bit.
            if (Time % BeamFireRate > BeamFireRate - 20)
            {
                PupilScale = MathHelper.Lerp(PupilScale, 0.7f, 0.15f);
            }
            // Otherwise, have it behave like normal.
            else
            {
                PupilScale = MathHelper.Lerp(PupilScale, 1f, 0.175f);
                PupilAngle = PupilAngle.AngleTowards(Projectile.AngleTo(target.Center), MathHelper.ToRadians(15f));
            }
        }

        public void Initialize(Player player)
        {
            if (!Main.dedServ)
                return;

            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, 52, 52, DustID.Blood);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1.2f, 1.5f);
            }
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D eyeTexture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = eyeTexture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            SpriteEffects spriteEffects = Owner.gravDir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(eyeTexture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None);

            // Pupil drawing.
            Texture2D pupilTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/DeathstareEyePupil").Value;
            Vector2 pupilDrawPosition = Projectile.Center - Main.screenPosition + PupilAngle.ToRotationVector2() * PupilOutwardness;
            pupilDrawPosition -= Vector2.UnitY * 6f;
            Main.EntitySpriteDraw(pupilTexture, pupilDrawPosition, null, Projectile.GetAlpha(lightColor), PupilAngle, pupilTexture.Size() * 0.5f, PupilScale, SpriteEffects.None);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
