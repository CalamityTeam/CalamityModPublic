using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Calamitamini : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float Range = 1300f;
        public const float SeparationAnxietyMin = 1500f;
        public const float SeparationAnxietyMax = 3200f;
        public const float SafeDist = 200f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;
                    int brim = Dust.NewDust(source + dustVel, 0, 0, (int)CalamityDusts.Brimstone, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[brim].noGravity = true;
                    Main.dust[brim].velocity = dustVel;
                }
                Projectile.localAI[0] += 1f;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<Calamitamini>();
            player.AddBuff(ModContent.BuffType<EntropysVigilBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cEyes = false;
                }
                if (modPlayer.cEyes)
                {
                    Projectile.timeLeft = 2;
                }
            }

            Projectile.MinionAntiClump();

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
            }

            Vector2 targetVec = Projectile.position;
            float maxDistance = Range;
            bool foundTarget = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    bool canHit = true;
                    if (extraDist < maxDistance)
                        canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);
                    if (!foundTarget && targetDist < (maxDistance + extraDist) && canHit)
                    {
                        targetVec = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int index = 0; index < Main.maxNPCs; index++)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        bool canHit = true;
                        if (extraDist < maxDistance)
                            canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);
                        if (!foundTarget && targetDist < (maxDistance + extraDist) && canHit)
                        {
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            float sepAnxietyDist = SeparationAnxietyMin;
            if (foundTarget)
            {
                sepAnxietyDist = SeparationAnxietyMax;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > sepAnxietyDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 vecToTarget = targetVec - Projectile.Center;
                float targetDist = vecToTarget.Length();
                vecToTarget.Normalize();
                if (targetDist > 200f)
                {
                    float speedMult = 8f; //6
                    vecToTarget *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
                }
                else
                {
                    float speedMult = -4f;
                    vecToTarget *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
                }
            }
            else
            {
                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = Projectile.ai[0] == 1f;
                }
                float returnSpeed = 6f;
                if (returningToPlayer)
                {
                    returnSpeed = 18f;
                }
                Vector2 returnSpot = player.Center - Projectile.Center + new Vector2(0f, -60f);
                float playerDist = returnSpot.Length();
                if (playerDist > 200f && returnSpeed < 10f)
                {
                    returnSpeed = 10f;
                }
                if (playerDist < SafeDist && returningToPlayer && !Collision.SolidCollision(Projectile.Center, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDist > 70f)
                {
                    returnSpot.Normalize();
                    returnSpot *= returnSpeed;
                    Projectile.velocity = (Projectile.velocity * 40f + returnSpot) / 41f;
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }

            //Update rotation
            if (foundTarget)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetVec) + MathHelper.Pi, 0.1f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }

            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 110f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                float projSpeed = 8f;
                int projType = ModContent.ProjectileType<BrimstoneLaserSummon>();
                if (foundTarget && Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner && Collision.CanHitLine(Projectile.Center, Projectile.width, Projectile.height, targetVec, 0, 0))
                    {
                        Vector2 velocity = targetVec - Projectile.Center;
                        velocity.Normalize();
                        velocity *= projSpeed;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int yStart = frameHeight * Projectile.frame;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, yStart, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
