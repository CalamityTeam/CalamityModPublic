using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Particles;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.DataStructures;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class ChainedMeatHook : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedHook";
        public Player Owner => Main.player[Projectile.owner];

        public float Timer => MaxTwirlTime - Projectile.timeLeft;
        public const float MaxTwirlTime = 30;
        public ref float Twirling => ref Projectile.ai[0];

        public bool MayIExist => Owner.active && (Owner.HeldItem.ModItem is OmegaBiomeBlade blade) && blade.secondaryAttunement != null && blade.secondaryAttunement.id == AttunementID.FlailBlade;

        Particle smear;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.timeLeft = 1;
        }

        public override bool? CanHitNPC(NPC target) => Twirling == 1f && !target.friendly;

        public override void AI()
        {
            if (!MayIExist)
            {
                Projectile.Kill();
                return;
            }

            if (Twirling == 0)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft = 2;
                Vector2 idealPosition = Owner.Center - Owner.direction * Vector2.UnitX * 40f + Vector2.One.RotatedBy(Main.GlobalTimeWrappedHourly) * 10f;
                if ((idealPosition - Projectile.Center).Length() > 1500f)
                    Projectile.Center = idealPosition;
                Projectile.Center = Vector2.Lerp(Projectile.Center, idealPosition, 0.05f);
                Projectile.Center = Projectile.Center.MoveTowards(idealPosition, 4f);

            }


            if (Twirling == 1f)
            {
                if (Projectile.timeLeft == MaxTwirlTime)
                {
                    Projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                    SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * Projectile.scale, Color.White, Main.rand.NextBool() ? Color.Cyan : Color.MediumTurquoise, 0.4f + Main.rand.NextFloat(0f, 0.5f), 20 + Main.rand.Next(30), 1, 3f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }

                    if (smear == null)
                    {
                        smear = new CircularSmearVFX(Owner.Center, Color.HotPink, Projectile.velocity.ToRotation(), Projectile.scale * 1.5f);
                        GeneralParticleHandler.SpawnParticle(smear);
                    }
                }

                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.TwoPi * -2f * (1 / MaxTwirlTime));
                Projectile.velocity.Normalize();
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.Center = Owner.Center + (Projectile.velocity * 140 * (1 + 0.3f * (float)Math.Sin(Projectile.timeLeft / MaxTwirlTime * MathHelper.Pi)));

                //Update the variables of the smear
                if (smear != null)
                {
                    smear.Rotation = Projectile.rotation + MathHelper.PiOver2;
                    smear.Time = 0;
                    smear.Position = Owner.Center;
                    smear.Scale = 3.4f;
                    smear.Color = Color.MediumTurquoise * 0.9f * (float)Math.Sin(Projectile.timeLeft / MaxTwirlTime * MathHelper.Pi);
                }


                if (Projectile.timeLeft == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * Projectile.scale, Color.White, Main.rand.NextBool() ? Color.Cyan : Color.MediumTurquoise, 0.4f + Main.rand.NextFloat(0f, 0.5f), 20 + Main.rand.Next(30), 1, 3f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                    Twirling = 0f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int debuffTime = 100;
            target.AddBuff(BuffType<GlacialState>(), debuffTime);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item105, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * Projectile.scale, Color.White, Main.rand.NextBool() ? Color.Cyan : Color.MediumTurquoise, 0.4f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D handle = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedHookBody").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedHook").Value;

            float drawAngle = (Projectile.Center - Owner.Center).ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new(0f, handle.Height);
            Vector2 drawOffset = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            Main.EntitySpriteDraw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            DrawChain();

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void DrawChain()
        {
            Texture2D chainTex = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedChain").Value;

            int dist = (int)Vector2.Distance(Owner.Center, Projectile.Center) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = Owner.Center;
            Nodes[dist] = Projectile.Center;

            for (int i = 1; i < dist + 1; i++)
            {
                Rectangle frame = new(0, 0 + 18 * (i % 2), 12, 18);
                Vector2 positionAlongLine = Vector2.Lerp(Owner.Center, Projectile.Center, i / (float)dist); //Get the position of the segment along the line, as if it were a flat line
                Nodes[i] = positionAlongLine;

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / frame.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new(1, yScale);

                Vector2 origin = new(frame.Width / 2, frame.Height); //Draw from center bottom of texture
                Main.EntitySpriteDraw(chainTex, Nodes[i] - Main.screenPosition, frame, Color.White * 0.5f, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

    }
}
