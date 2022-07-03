using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class WulfrumScrewdriverProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/WulfrumScrewdriver";
        public float Timer => (MaxTime * AnimTimeMult) - Projectile.timeLeft;
        public float LifetimeCompletion => Timer / (MaxTime * AnimTimeMult);

        public static int MaxTime = 55;
        public ref float AnimTimeMult => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Screwdriver");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 14;
            Projectile.height = 50;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            MaxTime = 20;
        }

        public override bool ShouldUpdatePosition() => false;
        public CurveSegment ThrustSegment = new CurveSegment(LinearEasing, 0f, 0f, 1f, 3);
        public CurveSegment HoldSegment = new CurveSegment(SineBumpEasing, 0.2f, 1f, 0.2f);
        public CurveSegment RetractSegment = new CurveSegment(PolyOutEasing, 0.76f, 1f, -0.8f, 3);
        public CurveSegment BumpSegment = new CurveSegment(SineBumpEasing, 0.9f, 0.2f, 0.15f);
        internal float DistanceFromPlayer => PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { ThrustSegment, HoldSegment,  RetractSegment, BumpSegment });
        public Vector2 OffsetFromPlayer => Projectile.velocity * DistanceFromPlayer * 18f;


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 60f * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + OffsetFromPlayer, Owner.Center + OffsetFromPlayer + (Projectile.velocity * bladeLenght), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (AnimTimeMult == 0) //Initialization
            {
                AnimTimeMult = MaxTime / (float)Owner.ActiveItem().useTime;
                Projectile.timeLeft = (int)(AnimTimeMult * MaxTime);

                Projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            //Manage position and rotation
            Projectile.Center = Owner.Center + OffsetFromPlayer ;
            Projectile.scale = 1f + (float)Math.Sin(LifetimeCompletion * MathHelper.Pi) * 0.2f; //SWAGGER

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.direction = Math.Sign(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() * Owner.gravDir - MathHelper.PiOver2);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = new Vector2(tex.Width / 2f, tex.Height);
            Vector2 scale = new Vector2(Math.Abs((float)Math.Sin(LifetimeCompletion * MathHelper.TwoPi)), 1f);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, scale * Projectile.scale, 0f, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
