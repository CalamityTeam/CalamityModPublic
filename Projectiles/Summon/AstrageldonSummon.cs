using CalamityMod.Buffs.Summon;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AstrageldonSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public bool dust = false;
        private int attackCounter = 1;
        private int teleportCounter = 400;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 62;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.alpha = 75;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.BabySlime;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            // For platform collision.
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];;
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = Projectile.Calamity();

            //hitbox size scaling
            float scale = (float)Math.Log(Projectile.minionSlots, 10f) + 1f;
            if (Projectile.scale != scale)
                Projectile.scale = scale;
            Projectile.width = (int)(64f * Projectile.scale);
            Projectile.height = (int)(62f * Projectile.scale);

            //on spawn effects and flexible minions
            if (!dust)
            {
                int dustAmt = 16;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dusty = Dust.NewDust(rotate + faceDirection, 0, 0, ModContent.DustType<AstralOrange>(), faceDirection.X * 1f, faceDirection.Y * 1f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = faceDirection;
                }

                dust = true;
            }

            //Bool setup
            bool isSummon = Projectile.type == ModContent.ProjectileType<AstrageldonSummon>();
            player.AddBuff(ModContent.BuffType<AbandonedSlimeBuff>(), 3600);
            if (isSummon)
            {
                if (player.dead)
                {
                    modPlayer.aSlime = false;
                }
                if (modPlayer.aSlime)
                {
                    Projectile.timeLeft = 2;
                }
            }

            if (Projectile.frame == 0 || Projectile.frame == 1)
            {
                float mindistance = 1000f;
                float longdistance = 2000f;
                float longestdistance = 3000f;
                Vector2 objectivepos = Projectile.position;
                bool gotoenemy = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        bool lineOfSight = Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        float disttoobjective = Vector2.Distance(npc.Center, Projectile.Center);
                        if ((!gotoenemy && disttoobjective < mindistance) && lineOfSight)
                        {

                            mindistance = disttoobjective;
                            objectivepos = npc.Center;
                            gotoenemy = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            bool lineOfSight = Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                            float disttoobjective = Vector2.Distance(npc.Center, Projectile.Center);
                            if ((!gotoenemy && disttoobjective < mindistance) && lineOfSight)
                            {

                                mindistance = disttoobjective;
                                objectivepos = npc.Center;
                                gotoenemy = true;
                            }
                        }
                    }
                }
                float maxdisttoenemy = longdistance;
                if (gotoenemy)
                {
                    maxdisttoenemy = longestdistance;
                }
                if (gotoenemy)
                {
                    float teleportRange = objectivepos.Length();
                    float scaleAddition = Projectile.scale * 5f;
                    if (teleportCounter <= 0 && teleportRange >= 800f)
                    {
                        int counter = 0;
                        while ((float)counter < 50f)
                        {
                            int dustType = Utils.SelectRandom(Main.rand, new int[]
                            {
                                ModContent.DustType<AstralBlue>(),
                                ModContent.DustType<AstralOrange>()
                            });
                            float rand1 = Main.rand.Next(-10, 11);
                            float rand2 = Main.rand.Next(-10, 11);
                            float rand3 = Main.rand.Next(3, 9);
                            float randAdjust = (float)Math.Sqrt((double)(rand1 * rand1 + rand2 * rand2));
                            randAdjust = rand3 / randAdjust;
                            rand1 *= randAdjust;
                            rand2 *= randAdjust;
                            int astralDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                            Dust dust = Main.dust[astralDust];
                            dust.noGravity = true;
                            dust.position.X = Projectile.Center.X;
                            dust.position.Y = Projectile.Center.Y;
                            dust.position.X += Main.rand.Next(-10, 11);
                            dust.position.Y += Main.rand.Next(-10, 11);
                            dust.velocity.X = rand1;
                            dust.velocity.Y = rand2;
                            counter++;
                        }
                        Projectile.position.X = objectivepos.X - (float)(Projectile.width / 2) + Main.rand.NextFloat(-100f, 100f);
                        Projectile.position.Y = objectivepos.Y - (float)(Projectile.height / 2) - Main.rand.NextFloat(0f + scaleAddition, 200f + scaleAddition);
                        Projectile.netUpdate = true;
                        teleportCounter = 600;
                    }
                    if (teleportCounter > 0)
                        teleportCounter -= Main.rand.Next(1, 4);
                }

                if (attackCounter > 0)
                {
                    attackCounter += Main.rand.Next(1, 4);
                }
                if (attackCounter > 300)
                {
                    attackCounter = 0;
                    Projectile.netUpdate = true;
                }
                float laserSpeed = 6f;
                int projType = ModContent.ProjectileType<AstrageldonLaser>();
                if (gotoenemy && attackCounter == 0)
                {
                    attackCounter += 2;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 laserVel = Projectile.SafeDirectionTo(objectivepos) * laserSpeed;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, laserVel, projType, Projectile.damage, 0f, Projectile.owner);
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int y6 = height * Projectile.frame;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
