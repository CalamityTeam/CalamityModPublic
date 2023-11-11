using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class BabyPlaguebringer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Pets/PlaguebringerBab";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 46;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            float maxTargetDist = 800f;

            //lighting
            float lights = (float)Main.rand.Next(90, 111) * 0.01f;
            lights *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 0f * lights, 0.75f * lights, 0f * lights);

            bool decelerate = false;
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1] += 1f;
                Projectile.extraUpdates = 1;
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 5 || Projectile.frame < 4)
                {
                    Projectile.frame = 4;
                }
                if (Projectile.ai[1] > 30f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 0;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    decelerate = true;
                }
            }
            if (decelerate)
            {
                return;
            }
            Vector2 projPos = Projectile.position;
            bool canAttack = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!canAttack && targetDist < maxTargetDist)
                    {
                        maxTargetDist = targetDist;
                        projPos = npc.Center;
                        canAttack = true;
                    }
                }
            }
            if (!canAttack)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float targetDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!canAttack && targetDist < maxTargetDist)
                        {
                            maxTargetDist = targetDist;
                            projPos = nPC2.Center;
                            canAttack = true;
                        }
                    }
                }
            }
            float separationAnxietyRange = 1200f;
            if (canAttack)
            {
                separationAnxietyRange = 3000f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyRange)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (canAttack && Projectile.ai[0] == 0f)
            {
                Vector2 projDirection = projPos - Projectile.Center;
                float projLength = projDirection.Length();
                projDirection.Normalize();
                if (projLength > 200f)
                {
                    float scaleFactor2 = 8f;
                    projDirection *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + projDirection) / 41f;
                }
                else
                {
                    projDirection *= -4f;
                    Projectile.velocity = (Projectile.velocity * 40f + projDirection) / 41f;
                }
            }
            else
            {
                bool isReturning = false;
                if (!isReturning)
                {
                    isReturning = Projectile.ai[0] == 1f;
                }
                float returnSpeed = 6f;
                if (isReturning)
                {
                    returnSpeed = 15f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 playerDirection = player.Center - center2 + new Vector2(0f, -60f);
                float playerDistance = playerDirection.Length();
                if (playerDistance > 200f && returnSpeed < 8f)
                {
                    returnSpeed = 8f;
                }
                if (playerDistance < 500f && isReturning && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDistance > 2000f)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDistance > 70f)
                {
                    playerDirection.Normalize();
                    playerDirection *= returnSpeed;
                    Projectile.velocity = (Projectile.velocity * 40f + playerDirection) / 41f;
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 12)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 40f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.ai[1] == 0f && canAttack && maxTargetDist < 500f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.ai[0] = 2f;
                        Vector2 npcCenter = projPos - Projectile.Center;
                        npcCenter.Normalize();
                        Projectile.velocity = npcCenter * 8f;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int framing = texture.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Vector2 origin = new Vector2(texture.Width / 2f, framing / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture.Width, framing), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Plague>(), 120);
    }
}
