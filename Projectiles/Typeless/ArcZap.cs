using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Typeless
{
    public class ArcZap : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public bool ableToHit = false;
        public NPC selectedTarget;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 15;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 25;
        }

        public override bool? CanDamage() => ableToHit ? (bool?)null : false;

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            Vector2 targetVector = target.Center - Projectile.Center;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.velocity += targetVector / 90f;
                Projectile.localAI[0]++;
            }
            Projectile.velocity.Y += 2f / 45f;
            Projectile.position += (target.position - target.oldPosition) / 16;
            if (Projectile.timeLeft <= 15) Projectile.position = target.Center; //puts the projectile inside the target if it somehow missed it (*Cough* WORMS)
            if (Projectile.penetrate == 3) { CheckNearTarget(target, targetVector); }
            else
            {
                ableToHit = false;
                if (Projectile.ai[1] > 1f) { SpawnArc(); }
                Projectile.Kill();
            }
        }

        public void CheckNearTarget(NPC target, Vector2 targetVector)
        {
            float targetDist = targetVector.Length();
            if (targetDist < 20f && Projectile.position.X < target.position.X + target.width && Projectile.position.X + Projectile.width > target.position.X && Projectile.position.Y < target.position.Y + target.height && Projectile.position.Y + Projectile.height > target.position.Y)
            {
                ableToHit = true;
            }

        }

        public void SpawnArc()
        {
            float maxDistance = 300f;
            int target = -1;
            for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
            {
                NPC npc = Main.npc[npcIndex];
                float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                if (targetDist < maxDistance && npc.Calamity().arcZapCooldown == 0 && npc.CanBeChasedBy())
                {
                    maxDistance = targetDist;
                    target = npcIndex;
                }
            }
            
            if (target > 0) 
            {
                selectedTarget = Main.npc[target];
                selectedTarget.Calamity().arcZapCooldown = 18;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X, Projectile.position.Y - 10f), new Vector2(0f, -2f), ModContent.ProjectileType<ArcZap>(), Projectile.damage, 0f, Projectile.owner, target, Projectile.ai[1] - 1f); 
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ChainLightning", 4) { Volume = 0.15f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                //Photoviscerator drawcode, edited slightly
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 13f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.Cyan, Color.LightBlue, colorInterpolation) * 0.4f;
                color.A = 0;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-32.5f, -32.5f); //Last vector is to offset the circle so that it is displayed where the hitbox actually is, instead of a bit down and to the right.
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                // Become smaller the futher along the old positions we are.
                Vector2 outerScale = new Vector2(1f) * intensity;
                Vector2 innerScale = new Vector2(1f) * intensity * 0.7f;
                outerColor *= intensity;
                innerColor *= intensity;
                Vector2 orbitDrawPosition1 = drawPosition + (drawPosition.SafeNormalize(Vector2.UnitY) * 10f).RotatedBy(MathHelper.Pi);
                Vector2 orbitDrawPosition2 = drawPosition - (drawPosition.SafeNormalize(Vector2.UnitY) * 10f).RotatedBy(MathHelper.Pi);
                if (Projectile.timeLeft > 15)
                {
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.15f, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.15f, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(lightTexture, orbitDrawPosition1, null, outerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.12f, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(lightTexture, orbitDrawPosition2, null, outerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.12f, SpriteEffects.None, 0);
                }
            }
            return false;
        }
    }
}
