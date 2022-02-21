using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;
using CalamityMod.Particles;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.DataStructures;

namespace CalamityMod.Projectiles.Melee
{
    public class ChainedMeatHook : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedHook";
        public Player Owner => Main.player[projectile.owner];

        public float Timer => MaxTwirlTime - projectile.timeLeft;
        public static float MaxTwirlTime = 30;
        public ref float Twirling => ref projectile.ai[0];

        public bool MayIExist => Owner.active && (Owner.HeldItem.modItem is OmegaBiomeBlade blade) && blade.secondaryAttunement != null && blade.secondaryAttunement.id == AttunementID.FlailBlade;

        Particle smear;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Meat Hook");
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 66;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 40;
            projectile.timeLeft = 1;
        }

        public override bool? CanHitNPC(NPC target) => Twirling == 1f && !target.friendly;

        public override void AI()
        {
            if (!MayIExist)
            {
                projectile.Kill();
                return;
            }

            if (Twirling == 0)
            {
                projectile.velocity = Vector2.Zero;
                projectile.timeLeft = 2;
                Vector2 idealPosition = Owner.Center - Owner.direction * Vector2.UnitX * 40f + Vector2.One.RotatedBy(Main.GlobalTime) * 10f;
                if ((idealPosition - projectile.Center).Length() > 1500f)
                    projectile.Center = idealPosition;
                projectile.Center = Vector2.Lerp(projectile.Center, idealPosition, 0.05f);
                projectile.Center = projectile.Center.MoveTowards(idealPosition, 4f);

            }


            if (Twirling == 1f)
            {
                if (projectile.timeLeft == MaxTwirlTime)
                {
                    projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                    Main.PlaySound(SoundID.Item71, projectile.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.Cyan : Color.MediumTurquoise, 0.4f + Main.rand.NextFloat(0f, 0.5f), 20 + Main.rand.Next(30), 1, 3f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }

                    if (smear == null)
                    {
                        smear = new CircularSmearVFX(Owner.Center, Color.HotPink, projectile.velocity.ToRotation(), projectile.scale * 1.5f);
                        GeneralParticleHandler.SpawnParticle(smear);
                    }
                }

                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.TwoPi * -2f * (1 / MaxTwirlTime));
                projectile.velocity.Normalize();
                projectile.rotation = projectile.velocity.ToRotation();
                projectile.Center = Owner.Center + (projectile.velocity * 140 * (1 + 0.3f * (float)Math.Sin(projectile.timeLeft / MaxTwirlTime * MathHelper.Pi)));

                //Update the variables of the smear
                if (smear != null)
                {
                    smear.Rotation = projectile.rotation + MathHelper.PiOver2;
                    smear.Time = 0;
                    smear.Position = Owner.Center;
                    smear.Scale = 3.4f;
                    smear.Color = Color.MediumTurquoise * 0.9f * (float)Math.Sin(projectile.timeLeft / MaxTwirlTime * MathHelper.Pi);
                }


                if (projectile.timeLeft == 1)
                {
                    Main.PlaySound(SoundID.Item101, projectile.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.Cyan : Color.MediumTurquoise, 0.4f + Main.rand.NextFloat(0f, 0.5f), 20 + Main.rand.Next(30), 1, 3f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                    Twirling = 0f;
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int debuffTime = 100;
            target.AddBuff(BuffType<GlacialState>(), debuffTime);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item105, projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.Cyan : Color.MediumTurquoise, 0.4f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedHookBody");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedHook");

            float drawAngle = (projectile.Center - Owner.Center).ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            drawChain(spriteBatch);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void drawChain(SpriteBatch spriteBatch)
        {
            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedChain");

            int dist = (int)Vector2.Distance(Owner.Center, projectile.Center) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = Owner.Center;
            Nodes[dist] = projectile.Center;

            for (int i = 1; i < dist + 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0 + 18 * (i % 2), 12, 18);
                Vector2 positionAlongLine = Vector2.Lerp(Owner.Center, projectile.Center, i / (float)dist); //Get the position of the segment along the line, as if it were a flat line
                Nodes[i] = positionAlongLine;

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / frame.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);

                Vector2 origin = new Vector2(frame.Width / 2, frame.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, Nodes[i] - Main.screenPosition, frame, Color.White * 0.5f, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

    }
}