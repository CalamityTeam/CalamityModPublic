using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class EndoCooperBody : ModProjectile
    {
        private int AttackMode = 0;
        private int LimbID = 0;
        private int laserdirection = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ascened Cooper");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.minion = true;
            projectile.minionSlots = 10f;
            projectile.netImportant = true;
            projectile.width = 90;
            projectile.height = 90;
            projectile.timeLeft = 18000;
            projectile.timeLeft *= 5;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.extraUpdates = 1;
			projectile.coldDamage = true;
        }

        public override void AI()
        {   
            //dust
            if (Main.rand.NextBool(15))
            {
                int dusttype = Main.rand.NextBool(2) ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dusttype = 80;
                }
                int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dusttype , projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 50, default, 0.6f);
                Main.dust[dust].noGravity = true;

            }
            //Apply the buff
            bool flag64 = projectile.type == ModContent.ProjectileType<EndoCooperBody>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<EndoCooperBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.endoCooper = false;
                }
                if (modPlayer.endoCooper)
                {
                    projectile.timeLeft = 2;
                }
            }

            //Spawn effects
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                AttackMode = (int)projectile.ai[0];
                LimbID = (int)projectile.ai[1];
                projectile.ai[0] = 0f;
                projectile.ai[1] = 0f;
                projectile.localAI[0] += 1f;
                for (int i = 0; i < 60; i++)
                {
                    int dusttype = Main.rand.NextBool(2) ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dusttype = 80;
                    }
                    Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                    int dust = Dust.NewDust(projectile.Center, 1, 1, dusttype, dspeed.X, dspeed.Y, 50, default, 0.8f);
                    Main.dust[dust].noGravity = true;
                }
            }
            projectile.localAI[1] = AttackMode;

            //Damage Update
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }
            //Variables
            float mindistance = 2000f;
            float longdistance = AttackMode != 2 ? 1300f : 1200f;
            float longestdistance = AttackMode != 2 ? 2600f : 2500f;
            float idledistance = AttackMode != 2 ? 600f : 400f;
            float chasespeed1 = 30f;
            float chasespeed2 = 18f;
            float firerate = 30f;

            Projectile limbs = Main.projectile[LimbID];
            
            switch (AttackMode)
            {
                case 0: chasespeed1 = 29f;
                        chasespeed2 = 16f;
                        firerate = 60f;
                        break;
                case 1: chasespeed1 = 24f;
                        chasespeed2 = 12f;
                        firerate = 200f;
                        
                        break;
                case 2: chasespeed1 = 32f;
                        chasespeed2 = 20f;
                        firerate = 30f;
                        break;
                case 3: chasespeed1 = 34f;
                        chasespeed2 = 21f;
                        firerate = 30f;
                    break;
            }

            if (limbs.type != ModContent.ProjectileType<EndoCooperLimbs>() || !limbs.active)
                projectile.Kill();

			projectile.MinionAntiClump();
            bool flag24 = false;
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1] += 1f;
                projectile.extraUpdates = 2;
                if (projectile.ai[1] > 30f)
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.extraUpdates = 1;
                    projectile.numUpdates = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    flag24 = true;
                }
            }
            if (flag24)
            {
                return;
            }
            Vector2 objectivepos = projectile.position;
            bool gotoenemy = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float disttoobjective = Vector2.Distance(npc.Center, projectile.Center);
                    if (!gotoenemy && disttoobjective < mindistance)
                    {
                        mindistance = disttoobjective;
                        objectivepos = npc.Center;
                        gotoenemy = true;
                    }
                }
            }
            if (!gotoenemy)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float disttoobjective = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!gotoenemy && disttoobjective < mindistance)
                        {
                            mindistance = disttoobjective;
                            objectivepos = nPC2.Center;
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
            if (Vector2.Distance(player.Center, projectile.Center) > maxdisttoenemy)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (gotoenemy && projectile.ai[0] == 0f)
            {
                Vector2 speedtoenemy = objectivepos - projectile.Center;
                float disttospeed = speedtoenemy.Length();
                speedtoenemy.Normalize();
                float stopdistance = AttackMode == 3 ? 120f : 200f;
                if (disttospeed > stopdistance)
                {
                    float scaleFactor2 = chasespeed1; //8
                    speedtoenemy *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + speedtoenemy) / 41f;
                }
                else
                {
                    float scalefactor3 = chasespeed2; //4
                    speedtoenemy *= -scalefactor3;
                    projectile.velocity = (projectile.velocity * 40f + speedtoenemy) / 41f; //41
                }
            }
            else
            {
                bool gotoplayer = false;
                if (!gotoplayer)
                {
                    gotoplayer = projectile.ai[0] == 1f;
                }
                float speedtoplayer = 8f;
                if (gotoplayer)
                {
                    speedtoplayer = 16f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 playerpos = player.Center - center2 + new Vector2(0f, -60f);
                float lenghttoplayer = playerpos.Length();
                if (lenghttoplayer > 200f && speedtoplayer < 8f)
                {
                    speedtoplayer = 10f;
                }
                if (lenghttoplayer < idledistance && gotoplayer && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (lenghttoplayer > 2700f)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (lenghttoplayer > 70f)
                {
                    playerpos.Normalize();
                    playerpos *= speedtoplayer;
                    projectile.velocity = (projectile.velocity * 40f + playerpos) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
                
            }

            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > firerate)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }            
            if (projectile.ai[0] == 0f)
            {               
                if (projectile.ai[1] == 0f && gotoenemy && mindistance < 600f)
                {
                    
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        switch (AttackMode)
                        {
                            case 0:
                                    Main.PlaySound(SoundID.Item15, (int)projectile.position.X, (int)projectile.position.Y);
                                    Vector2 aimlaser = objectivepos - projectile.Center;
                                    aimlaser.Normalize();
                                    aimlaser = aimlaser.RotatedBy(MathHelper.ToRadians(30 * -laserdirection));
                                    float angularChange = (MathHelper.Pi / 180f) * 1.1f * laserdirection;
                                    //aimlaser *= 12f;
                                    Projectile.NewProjectile(projectile.Center, aimlaser, ModContent.ProjectileType<EndoBeam>(), projectile.damage, 0f, projectile.owner, angularChange, (float)projectile.whoAmI);
                                    laserdirection *= -1;
                                    break;

                            case 1: //Kill limbs
                                    if (limbs.ai[0] == 0f)
                                    {
                                        limbs.ai[0] = 1f;
                                    }
                                    //Respawn limbs
                                    else if (limbs.ai[0] == 2f)
                                    {
                                        limbs.ai[0] = 3f;
                                    }
                                    projectile.netUpdate = true;
                                    break;

                            case 2: projectile.ai[0] = 2f;
                                    Vector2 aimtoenemy = objectivepos - projectile.Center;
                                    aimtoenemy.Normalize();
                                    projectile.velocity = aimtoenemy * 18f;
                                    projectile.netUpdate = true;
                                    break;
                            case 3: limbs.ai[0] = 4f;
                                    projectile.netUpdate = true;
                                    break;
                            default:break;
                        }
                    }
                }
            }
            if (limbs.ai[0] == 2f && projectile.ai[1] == 0f)
            {
                projectile.ai[1] += 1f;
                limbs.ai[0] = 3f;
                projectile.netUpdate = true;
            }

            //Tilting and change directions
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.X * 0.07f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Summon/EndoCooperBody_Glow"), projectile.Center - Main.screenPosition, frame, Color.LightSkyBlue, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override bool CanDamage()
        {
            return AttackMode == 2;
        }
    }
}
