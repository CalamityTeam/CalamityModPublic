using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGDeath : ModProjectile
    {
        public float TelegraphDelay
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public Vector2 OldVelocity;
        public const float TelegraphTotalTime = 75f;
        public const float TelegraphFadeTime = 15f;
        public const float TelegraphWidth = 4200f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Beam");
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
            cooldownSlot = 1;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(OldVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            OldVelocity = reader.ReadVector2();
        }

        public override void AI()
        {
            // Determine the relative opacities for each player based on their distance.
            // This has a lower bound of 0.35 to prevent the laser from going completely invisible and players getting hit by cheap shots.
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                projectile.netUpdate = true;
            }
            // Fade in after telegraphs have faded.
            if (TelegraphDelay > TelegraphTotalTime)
            {
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 25;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                // If an old velocity is in reserve, set the true velocity to it and make it as "taken" by setting it to <0,0>
                if (OldVelocity != Vector2.Zero)
                {
                    projectile.velocity = OldVelocity * ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 1.25f : 1f);
                    OldVelocity = Vector2.Zero;
                    projectile.netUpdate = true;
                }
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            // Otherwise, be sure to save the velocity the projectile started with. It will be set again when the telegraph is over.
            else if (OldVelocity == Vector2.Zero)
            {
                OldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Zero;
                projectile.netUpdate = true;
                projectile.rotation = OldVelocity.ToRotation() + MathHelper.PiOver2;
            }
            TelegraphDelay++;
        }

        public override bool CanHitPlayer(Player target) => TelegraphDelay > TelegraphTotalTime;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (TelegraphDelay > TelegraphTotalTime)
				target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.alpha);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TelegraphDelay >= TelegraphTotalTime)
                return true;
            Texture2D laserTelegraph = ModContent.GetTexture("CalamityMod/ExtraTextures/LaserWallTelegraphBeam");
            float yScale = 2f;
            if (TelegraphDelay < TelegraphFadeTime)
            {
                yScale = MathHelper.Lerp(0f, 2f, TelegraphDelay / 15f);
            }
            if (TelegraphDelay > TelegraphTotalTime - TelegraphFadeTime)
            {
                yScale = MathHelper.Lerp(2f, 0f, (TelegraphDelay - (TelegraphTotalTime - TelegraphFadeTime)) / 15f);
            }
            Vector2 scaleInner = new Vector2(TelegraphWidth / laserTelegraph.Width, yScale);
            Vector2 origin = laserTelegraph.Size() * new Vector2(0f, 0.5f);
            Vector2 scaleOuter = scaleInner * new Vector2(1f, 1.6f);

            Color colorOuter = Color.Lerp(Color.Cyan, Color.Purple, TelegraphDelay / TelegraphTotalTime * 2f % 1f); // Iterate through purple and cyan once and then flash.
            Color colorInner = Color.Lerp(colorOuter, Color.White, 0.75f);

            colorOuter *= 0.7f;
            colorInner *= 0.7f;

            spriteBatch.Draw(laserTelegraph, projectile.Center - Main.screenPosition, null, colorInner, OldVelocity.ToRotation(), origin, scaleInner, SpriteEffects.None, 0f);
            spriteBatch.Draw(laserTelegraph, projectile.Center - Main.screenPosition, null, colorOuter, OldVelocity.ToRotation(), origin, scaleOuter, SpriteEffects.None, 0f);
            return false;
        }
    }
}
