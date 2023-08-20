using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class GhostFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool ableToHit = true;
        public NPC target;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 200;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool? CanDamage() => ableToHit ? (bool?)null : false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0] += 1f / (Projectile.extraUpdates + 1);

            if (Projectile.penetrate < 200) //If an enemy has already been hit by the projectile, prevents any further homing capabilities
            {
                if (Projectile.timeLeft > 60) { Projectile.timeLeft = 60; } //The projectile start shrinking and slowing down. it can still hit for a bit during this, to allow a bit of multi-target if the enemies are really close to eachother.
                Projectile.velocity *= 0.88f;
            }
            else if (Projectile.localAI[0] < 60f) //If the projectile hasn't been alive for 1 second yet, slow it down more every frame and prevent any homing capabilities.
            {
                Projectile.velocity *= 0.93f;
            }
            else FindTarget(player); //No barricades stand in our way. FIND AND KILL!

            if (Projectile.timeLeft <= 20) //removes projectile's ability to hit when it is super close to death because it will have been visually shrunk to almost nothing in PreDraw
            {
                ableToHit = false;
            }
        }
        public void FindTarget(Player player)
        {
            float maxDistance = 3000f;
            bool foundTarget = false;
            if (player.HasMinionAttackTargetNPC) //Targetted enemy takes priority for homing no matter what.
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (targetDist < maxDistance)
                    {
                        maxDistance = targetDist;
                        foundTarget = true;
                        target = npc;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (targetDist < maxDistance)
                        {
                            maxDistance = targetDist;
                            foundTarget = true;
                            target = npc;
                        }
                    }
                }
            }
            if (!foundTarget) //If the enemy that the projectile was heading towards died prematurely, slow down the projectile if there are no new targets.
            {
                Projectile.velocity *= 0.98f;
            }
            else KillTheThing(target); //If there is an npc found, seek and destroy.
        }
        public void KillTheThing(NPC npc)
        {
            Projectile.velocity = Projectile.SuperhomeTowardsTarget(npc, 50f/(Projectile.extraUpdates+1), 60f / (Projectile.extraUpdates + 1), 1f / (Projectile.extraUpdates + 1)); //predictionStrength is lessened to account for extra updates, otherwise the projectile tends to copy fast NPC movement 1:1. Not Cool.
        }
        public override bool PreDraw(ref Color lightColor) //photoviscerator ball drawcode, slightly edited
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.Cyan, Color.LightBlue, colorInterpolation) * 0.4f;
                color.A = 0;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2 (-28f, -28f); //Last vector is to offset the circle so that it is displayed where the hitbox actually is, instead of a bit down and to the right.
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                if (Projectile.timeLeft <= 60) //Shrinks to nothing when projectile is nearing death
                {
                    intensity *= Projectile.timeLeft / 60f;
                }
                // Become smaller the futher along the old positions we are.
                Vector2 outerScale = new Vector2(1f) * intensity;
                Vector2 innerScale = new Vector2(1f) * intensity * 0.7f;
                outerColor *= intensity;
                innerColor *= intensity;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
