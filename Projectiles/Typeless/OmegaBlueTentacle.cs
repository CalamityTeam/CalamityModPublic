using CalamityMod.NPCs;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    [PierceResistException]
    public class OmegaBlueTentacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public bool initSegments = false;
        public Vector2[] segment = new Vector2[6];
        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 8;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreAI()
        {
            if (!initSegments)
            {
                initSegments = true;
                for (int i = 0; i < 6; i++)
                {
                    segment[i] = Projectile.Center;
                }
            }
            return true;
        }

        public override void AI()
        {
            bool madness = Owner.Calamity().omegaBlueAbyssalMadness;
            if (Owner.active && Owner.Calamity().omegaBlueSet)
                Projectile.timeLeft = 8;

            // Tentacle head movement (homing)
            Vector2 playerVel = Owner.position - Owner.oldPosition;
            Projectile.position += playerVel;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 0f)
            {
                Vector2 home = Owner.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(60) * Projectile.ai[1]);
                Vector2 distance = home - Projectile.Center;
                float range = distance.Length();
                distance.Normalize();
                if (Projectile.ai[0] == 0f)
                {
                    if (range > 13f)
                    {
                        Projectile.ai[0] = -1f; // If in fast mode, stay fast until back in range
                        if (range > 1300f)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    else
                    {
                        if (madness)
                            Projectile.ai[0] = 120f;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 3f + Main.rand.NextFloat(3f);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    distance /= 8f;
                }

                if (range > 120f) //switch to fast return mode
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
                Projectile.velocity += distance;
                if (range > 30f)
                    Projectile.velocity *= 0.96f;

                if (Projectile.ai[0] > 120f) //attack nearby enemy
                {
                    Projectile.ai[0] = 10 + Main.rand.Next(10);
                    float maxDistance = madness ? 900f : 600f;
                    int target = -1;
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.CanBeChasedBy(Projectile))
                        {
                            float npcDistance = Projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                target = npc.whoAmI;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        Projectile.velocity = Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 13f + (Main.npc[target].velocity / 2f) - (playerVel / 2f);
                        Projectile.ai[0] *= -1f;
                    }
                    Projectile.netUpdate = true;
                }
            }

            //tentacle segment updates
            segment[0] = Owner.Center;
            for (int i = 1; i < 5; i++)
            {
                MoveSegment(segment[i - 1], ref segment[i], segment[i + 1]);
            }
            MoveSegment(segment[4], ref segment[5], Projectile.Center + Projectile.velocity);

            if (madness)
            {
                if (Projectile.ai[0] != -1f)
                    Projectile.ai[0]++;
                Projectile.position += Projectile.velocity;
                //SMOOTH ASS DUST
                Vector2 dustPos = Projectile.position + Projectile.velocity;
                Vector2 tickVel = dustPos - Projectile.oldPosition; //playerVel + projectile.velocity * 2f;
                dustPos += new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation);
                dustPos += new Vector2(Projectile.width / 2 - 4, Projectile.height / 2 - 4);
                const float factor = 3f;
                int limit = (int)(tickVel.Length() / factor);
                if (limit == 0)
                {
                    Dust d = Dust.NewDustPerfect(dustPos, DustID.PurificationPowder, Vector2.Zero, 100, Color.Transparent, 0.9f);
                    d.noGravity = true;
                    d.noLight = true;
                    d.fadeIn = 1f;
                }
                else
                {
                    tickVel.Normalize();
                    tickVel *= factor;
                    for (int i = 0; i <= limit; i++)
                    {
                        Dust d = Dust.NewDustPerfect(dustPos, DustID.PurificationPowder, Vector2.Zero, 100, Color.Transparent, 0.9f);
                        d.noGravity = true;
                        d.noLight = true;
                        d.fadeIn = 1f;
                        d.position -= tickVel * i;
                    }
                }
            }
        }

        private static void MoveSegment(Vector2 previous, ref Vector2 current, Vector2 next)
        {
            current = previous + next;
            current /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Owner.Calamity().omegaBlueAbyssalMadness)
                modifiers.ApplyScalingForcedCrit(Projectile);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Owner.DoLifestealDirect(target, 10 * hit.Damage / Projectile.damage, 0.33f);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            GameShaders.Armor.ApplySecondary(Owner.cBody, Owner, new DrawData?());
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D segmentSprite = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/OmegaBlueTentacleSegment1").Value;
            for (int i = 0; i < 5; i++)
            {
                Projectile.rotation = (Projectile.Center - segment[i]).ToRotation();
                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        segmentSprite = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/OmegaBlueTentacleSegment2").Value;
                        break;
                    case 2:
                        segmentSprite = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/OmegaBlueTentacleSegment3").Value;
                        break;
                    case 3:
                        segmentSprite = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/OmegaBlueTentacleSegment4").Value;
                        break;
                    case 4:
                        segmentSprite = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/OmegaBlueTentacleSegment5").Value;
                        break;
                    default:
                        break;

                }
                Main.spriteBatch.Draw(segmentSprite, segment[i] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), segmentSprite.Bounds, Projectile.GetAlpha(lightColor), Projectile.rotation, segmentSprite.Bounds.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            Projectile.rotation = (Projectile.Center - segment[5]).ToRotation();
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), texture2D13.Bounds, Projectile.GetAlpha(lightColor), Projectile.rotation, texture2D13.Bounds.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
