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
    public class DeathstareEyeball : ModProjectile
    {
        // This does not need to be synced. It is used solely for drawcode/visuals and is intended to be local.
        public float PupilScale = 1f;
        public const int BeamFireRate = 60;
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[1];
        public ref float PupilAngle => ref projectile.localAI[0];
        public ref float PupilOutwardness => ref projectile.localAI[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eyeball");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
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
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        #region AI
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.Blue.ToVector3());

            bool isCorrectMinion = projectile.type == ModContent.ProjectileType<DeathstareEyeball>();
            CalamityPlayer modPlayer = Owner.Calamity();
            Owner.AddBuff(ModContent.BuffType<DeathstareBuff>(), 3600);

            // Ensure that the projectile executing this code is a valid one.
            if (isCorrectMinion)
            {
                if (Owner.dead)
                    modPlayer.deathstareEyeball = false;
                if (modPlayer.deathstareEyeball)
                    projectile.timeLeft = 2;
            }

            Vector2 destination = Owner.Center + Vector2.UnitY * (Owner.gfxOffY - 110f);
            if (Owner.gravDir == -1f)
                destination.Y += 220f;

            // Very quickly fly towards above (or below if gravity is reversed) the player.
            projectile.Center = Vector2.Lerp(projectile.Center, destination, 0.36f);

            projectile.position = projectile.position.Floor();
            projectile.rotation = (projectile.position.X - projectile.oldPosition.X) * 0.07f + Owner.velocity.X * 0.2f;
            projectile.rotation = Utils.Clamp(projectile.rotation, -0.4f, 0.4f);

            if (projectile.ai[0] == 0f)
            {
                Initialize(Owner);
                projectile.ai[0] = 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Owner.MinionDamage());
                projectile.damage = trueDamage;
            }

            NPC potentialTarget = projectile.Center.MinionHoming(720f, Owner);

            if (potentialTarget is null)
                DoHoveringAI();
            else
                DoAttackingAI(potentialTarget);

            projectile.frame = (int)(Time / 5) % 4;

            Time++;
        }

        public void DoHoveringAI()
        {
            // Roll the pupil around if there's little movement from the eye and player.
            if (Time % 180f > 120f && Math.Abs(projectile.rotation) < 0.03f)
            {
                float idealAngle = Utils.InverseLerp(120f, 180f, Time % 180f, true) * MathHelper.TwoPi;

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
                if (Main.myPlayer == projectile.owner)
                {
                    Vector2 velocity = (target.Center - projectile.Center) / 15f;
                    int beam = Projectile.NewProjectile(target.Center, velocity, ModContent.ProjectileType<DeathstareBeam>(), projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[beam].ai[0] = Projectile.GetByUUID(projectile.owner, projectile.whoAmI);
                    Main.projectile[beam].Damage();
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
                PupilAngle = PupilAngle.AngleTowards(projectile.AngleTo(target.Center), MathHelper.ToRadians(15f));
            }
        }

        public void Initialize(Player player)
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

            if (!Main.dedServ)
                return;

            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, 52, 52, DustID.Blood);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1.2f, 1.5f);
            }
        }

        #endregion

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D eyeTexture = Main.projectileTexture[projectile.type];
            Rectangle frame = eyeTexture.Frame(1, 4, 0, projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects spriteEffects = Owner.gravDir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            spriteBatch.Draw(eyeTexture, projectile.Center - Main.screenPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

            // Pupil drawing.
            Texture2D pupilTexture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/DeathstareEyePupil");
            Vector2 pupilDrawPosition = projectile.Center - Main.screenPosition + PupilAngle.ToRotationVector2() * PupilOutwardness;
            pupilDrawPosition -= Vector2.UnitY * 4f;
            spriteBatch.Draw(pupilTexture, pupilDrawPosition, null, projectile.GetAlpha(lightColor), PupilAngle, pupilTexture.Size() * 0.5f, PupilScale, spriteEffects, 0f);
            return false;
        }

        public override bool CanDamage() => false;
    }
}
