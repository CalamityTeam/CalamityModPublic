using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class GhastlyChain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedChain";
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => 20 - Projectile.timeLeft;

        public float Gravity;

        public NPC NPCfrom
        {
            get => Main.npc[(int)Projectile.ai[0]];
            set => Projectile.ai[0] = value.whoAmI;
        }
        public NPC Target
        {
            get => Main.npc[(int)Projectile.ai[1]];
            set => Projectile.ai[1] = value.whoAmI;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Chain");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 8;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.timeLeft = 20;
        }

        public override bool? CanHitNPC(NPC target) => target == Target;

        public override void AI()
        {
            Projectile.Center = Target.Center;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.FlailBladeAttunement_GhostChainProc)
                sword.OnHitProc = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D chainTex = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedChain").Value;


            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            float opacity = Projectile.timeLeft > 10 ? 1 : Projectile.timeLeft / 10f;
            Vector2 Shake = Projectile.timeLeft < 15 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (15 - Projectile.timeLeft / 5f) * 0.5f;

            BezierCurve curve = new BezierCurve(new Vector2[] { Target.Center, Target.Center + Vector2.UnitY * Gravity, NPCfrom.Center + Vector2.UnitY * Gravity, NPCfrom.Center });
            int numPoints = 20;
            Vector2[] Nodes = curve.GetPoints(numPoints).ToArray();

            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = Nodes[i] + Shake * (float)Math.Sin(i / (float)numPoints * MathHelper.Pi);

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);

                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                Main.EntitySpriteDraw(chainTex, position - Main.screenPosition, null, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 0);
            }

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }
}
