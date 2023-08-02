using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.MirrorofKalandraMinions
{
    public class WindRipper : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(MirrorofKalandra.TargetDistanceDetection, Owner);
        public ref float Oscillation => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 84;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            CheckMinionExistence();

            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center + (-MathHelper.PiOver2 - MathHelper.PiOver4 * 1.5f).ToRotationVector2() * (MirrorofKalandra.IdleDistanceFromPlayer + MirrorofKalandra.IdleDistanceFromPlayer * (MathF.Sin(Oscillation) / MirrorofKalandra.OscillationRange)), .4f);
            Projectile.velocity = Vector2.Zero;
            Oscillation += MirrorofKalandra.OscillationSpeed;

            if (Target is not null)
            {
                DoAnimation();
                ShootTarget();

                Projectile.rotation = Projectile.rotation.AngleTowards(CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, Target, MirrorofKalandra.Wind_ArrowSpeed).ToRotation(), .2f);
            }
            else
            {
                Projectile.frame = 0;
                Projectile.rotation = Projectile.rotation.AngleTowards(-MathHelper.PiOver2 - MathHelper.PiOver4 * 1.5f, .2f);
            }
        }

        public void DoAnimation()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % MirrorofKalandra.Wind_BowChargeTime == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        public void ShootTarget()
        {
            // On the first frame where the projectile is on the firing frame animation, fire an arrow.
            if (Projectile.frame == 5 && Projectile.frameCounter % MirrorofKalandra.Wind_BowChargeTime == 0 && Main.myPlayer == Projectile.owner)
            {
                Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.width / 2);
                int arrow = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, Target, MirrorofKalandra.Wind_ArrowSpeed),
                    ModContent.ProjectileType<WindRipperArrow>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Owner.whoAmI);

                if (Main.projectile.IndexInRange(arrow))
                    Main.projectile[arrow].originalDamage = Projectile.originalDamage;

                SoundEngine.PlaySound(SoundID.Item5, Owner.Center);

                Projectile.netUpdate = true;
            }
        }

        public void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<KalandraMirrorBuff>(), 3600);
            if (Projectile.type != ModContent.ProjectileType<WindRipper>())
                return;

            if (Owner.dead)
                ModdedOwner.KalandraMirror = false;
            if (ModdedOwner.KalandraMirror)
                Projectile.timeLeft = 2;
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
