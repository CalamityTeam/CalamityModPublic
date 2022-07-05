using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Particles
{
    public class WulfrumBastionPartsParticle : Particle
    {
        public override bool SetLifetime => true;
        public override string Texture => "CalamityMod/Particles/WulfrumBastionParts";
        public override bool UseCustomDraw => true;

        internal Rectangle Frame;
        internal Vector2 DestinationOffset;
        public Vector2 Offset;
        public float RotationOffset;
        public Player Owner;
        public float TimeOffset;
        public int AnimationTime;
        public float LifetimeCompletionAdjusted => Math.Clamp((Time - Lifetime * TimeOffset) / (float)AnimationTime, 0, 1f);

        public WulfrumBastionPartsParticle(Player owner, int variant, int lifetime)
        {
            Offset = (-Vector2.UnitY * (40f + Main.rand.NextFloat(34f))).RotatedByRandom(MathHelper.PiOver4 * 0.04f);
            Owner = owner;
            Position = Owner.Center;
            Scale = 1f;
            Color = Color.White;
            Velocity = Vector2.Zero;
            Rotation = 0f;
            RotationOffset = Main.rand.NextFloat(MathHelper.PiOver4 * 0f) * (Main.rand.NextBool() ? -1f : 1f);
            Lifetime = lifetime;
            AnimationTime = (int)(lifetime * 0.44f);

            switch (variant)
            {
                case 0: //Back leg
                    Frame = new Rectangle(46, 4, 10, 14);
                    DestinationOffset = new Vector2(7f, 16f); //1f?
                    TimeOffset = 0;
                    break;
                case 1: //Front leg
                    Frame = new Rectangle(30, 4, 12, 14);
                    DestinationOffset = new Vector2(-4f, 16f);
                    TimeOffset = 0;
                    break;
                case 2: //Bottom torso
                    Frame = new Rectangle(4, 30, 22, 12);
                    DestinationOffset = new Vector2(1f, 5f);
                    TimeOffset = 0.15f;
                    break;
                case 3: //Torso
                    Frame = new Rectangle(30, 24, 30, 16);
                    DestinationOffset = new Vector2(1f, -3f);
                    TimeOffset = 0.35f;
                    break;
                case 4: //Helmet
                    Frame = new Rectangle(2, 2, 22, 24);
                    DestinationOffset = new Vector2(-1f, -15f);
                    TimeOffset = 0.43f;
                    break;
                case 5: //Fusion Cannon
                    Frame = new Rectangle(14, 46, 38, 18);
                    DestinationOffset = new Vector2(-2f, -2f);
                    TimeOffset = 0.6f;
                    break;
            }

            Origin = Frame.Size() / 2;

            //Fusion Cannon rotates around the shoulder
            if (variant == 5)
                Origin.X -= 12f;

        }

        public override void Update()
        {
            Rotation = MathHelper.Lerp(RotationOffset, 0f, (float)Math.Pow(LifetimeCompletionAdjusted, 0.8f));

            if (Owner.dead || !Owner.active)
                Kill();
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            if (Owner.dead || !Owner.active)
                return;

            Texture2D baseTex = GeneralParticleHandler.GetTexture(Type);
            Vector2 center = new Vector2((int)Owner.MountedCenter.X, (int)(Owner.MountedCenter.Y + Owner.gfxOffY));
            Vector2 currentOffset = Vector2.Lerp(Offset, Vector2.Zero, (float)Math.Pow(LifetimeCompletionAdjusted, 0.8f));
            //Vector2 currentOffset = Vector2.Lerp(Offset, Vector2.Zero, 1f);
            Color lightColor = Lighting.GetColor(Owner.Center.ToTileCoordinates());

            Vector2 realDestinationOffset = new Vector2(DestinationOffset.X * Owner.direction, DestinationOffset.Y * Owner.gravDir);
            Vector2 realCurrentOffset = new Vector2(currentOffset.X * Owner.direction, currentOffset.Y * Owner.gravDir);


            SpriteEffects spriteEffect = SpriteEffects.None;
            Vector2 origin = Origin;

            if (Owner.direction < 0)
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
                origin.X = Frame.Width - Origin.X;
            }
                

            float opacity = Math.Clamp(LifetimeCompletionAdjusted * 4f ,0f, 1f);

            spriteBatch.Draw(baseTex, center + realDestinationOffset + realCurrentOffset - Main.screenPosition, Frame, lightColor * opacity, Rotation, origin, Scale, spriteEffect, 0);
        }
    }
}




