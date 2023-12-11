using System;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianRock : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public override string Texture => "CalamityMod/NPCs/ProfanedGuardians/ProfanedRocks";


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true; //the sounds get grating otherwise
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 1;
        }
        

        public override void AI()
        {
            // Dynamically update stats here, originalDamage can be found in MiscEffects
            Projectile.damage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(Projectile.originalDamage);
            Projectile.damage = Owner.ApplyArmorAccDamageBonusesTo(Projectile.damage);

            // Despawn properly
            if (Owner.Calamity().pSoulGuardians && Projectile.ai[0] == 0f)
                Projectile.timeLeft = 4;
            if (!Owner.Calamity().pSoulArtifact || Owner.dead || !Owner.active || (Owner.Calamity().profanedCrystal && !Owner.Calamity().profanedCrystalBuffs))
            {
                Owner.Calamity().pSoulGuardians = false;
                Projectile.active = false;
                return;
            }
            
            if (Projectile.ai[0] == 0f) //regular expected behaviour of floaty rocks
            {
                // Rotation velocity
                float minRotationVelocity = 0.5f;
                float rotationVelocityIncrease = 0.2f;
                rotationVelocityIncrease += rotationVelocityIncrease * (Projectile.ai[1] * 0.5f);

                // Rotate around Player
                var psc = Owner.Calamity().pscState;
                float angle = MathHelper.TwoPi / (psc > 0 ? 10 : 5) * Projectile.ai[1];
                float distance = 50f + (psc > 0 ? 30f : 0f);
                Projectile.Center = Owner.Center + Projectile.ai[1].ToRotationVector2() * distance;
                Projectile.rotation = Projectile.ai[1] + (float)Math.Atan(90);
                Projectile.ai[1] += MathHelper.ToRadians(psc > 0 ? 2f : -2f);
            }
            else if (Projectile.ai[0] == 1f) //rock yeetage begins
            {
                NPC target = Projectile.Center.MinionHoming(2000f, Owner);
                if (Owner.HasBuff<ProfanedCrystalWhipBuff>() && target != null)
                {
                    Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, 32f);
                    Projectile.ai[0] = 3f; //prevent slowdown if yeeted due to the ai buff from whips
                }
                else
                {
                    Projectile.velocity = Projectile.Center - Owner.Center;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= Owner.Calamity().profanedCrystalBuffs ? 25f : 20f;
                    Projectile.ai[0] = 2f;
                }
                Projectile.timeLeft = 300;
            }
            else if (Projectile.ai[0] == 2f) //rocks have been yeeted, handle the aftermath
            {
                if (Projectile.timeLeft > 275) //slow them down a little
                    Projectile.velocity *= 0.9725f;

                for (int i = 0; i < 2; i++)
                {
                    if (i == 0 || Projectile.timeLeft > 285)
                        Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, (int)CalamityDusts.ProfanedFire, 0f, -1f, 0, default, 1f);
                }
                    
            }
        }
        
        public override bool? CanDamage() => Projectile.ai[0] >= 1f ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            bool psc = Owner.Calamity().profanedCrystalBuffs;
            int rockType = (int)MathHelper.Clamp(Projectile.ai[2], 1f, 6f);;
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedRocks" + rockType.ToString()).Value;
            
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos -= new Vector2(texture.Width, texture.Height) * Projectile.scale / 2f;
            drawPos += drawOrigin * Projectile.scale + new Vector2(0f, Projectile.gfxOffY);
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            if (CalamityConfig.Instance.Afterimages && Projectile.ai[0] >= 1f)  //handle afterimages manually since the utility broke it and didn't render correctly
            {
                for (int i = 0; i < Projectile.oldPos.Length; ++i)
                {
                    drawPos = Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, frame, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                Main.spriteBatch.Draw(texture, drawPos, frame, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire, 0f, -1f, 0, default, 1f);
        }
    }
}
