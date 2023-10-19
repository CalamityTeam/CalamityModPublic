using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoCooperBody : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private int AttackMode = 0;
        private int LimbID = 0;
        private int laserdirection = 1;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.minion = true;
            Projectile.minionSlots = 10f;
            Projectile.netImportant = true;
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.timeLeft = 18000;
            Projectile.timeLeft *= 5;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //dust
            if (Main.rand.NextBool(15))
            {
                int dusttype = Main.rand.NextBool() ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dusttype = 80;
                }
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dusttype , Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 50, default, 0.6f);
                Main.dust[dust].noGravity = true;

            }
            //Apply the buff
            bool isMinion = Projectile.type == ModContent.ProjectileType<EndoCooperBody>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<EndoCooperBuff>(), 3600);
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.endoCooper = false;
                }
                if (modPlayer.endoCooper)
                {
                    Projectile.timeLeft = 2;
                }
            }

            //Spawn effects
            if (Projectile.localAI[0] == 0f)
            {
                AttackMode = (int)Projectile.ai[0];
                LimbID = (int)Projectile.ai[1];
                Projectile.ai[0] = 0f;
                Projectile.ai[1] = 0f;
                Projectile.localAI[0] += 1f;
                for (int i = 0; i < 60; i++)
                {
                    int dusttype = Main.rand.NextBool() ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dusttype = 80;
                    }
                    Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, dusttype, dspeed.X, dspeed.Y, 50, default, 0.8f);
                    Main.dust[dust].noGravity = true;
                }
            }
            Projectile.localAI[1] = AttackMode;

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
                Projectile.Kill();

            Projectile.MinionAntiClump();
            bool accelerate = false;
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1] += 1f;
                Projectile.extraUpdates = 2;
                if (Projectile.ai[1] > 30f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 1;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    accelerate = true;
                }
            }
            if (accelerate)
            {
                return;
            }
            Vector2 objectivepos = Projectile.position;
            bool gotoenemy = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float disttoobjective = Vector2.Distance(npc.Center, Projectile.Center);
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
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float disttoobjective = Vector2.Distance(nPC2.Center, Projectile.Center);
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
            if (Vector2.Distance(player.Center, Projectile.Center) > maxdisttoenemy)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (gotoenemy && Projectile.ai[0] == 0f)
            {
                Vector2 speedtoenemy = objectivepos - Projectile.Center;
                float disttospeed = speedtoenemy.Length();
                speedtoenemy.Normalize();
                float stopdistance = AttackMode == 3 ? 120f : 200f;
                if (disttospeed > stopdistance)
                {
                    float scaleFactor2 = chasespeed1; //8
                    speedtoenemy *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + speedtoenemy) / 41f;
                }
                else
                {
                    float scalefactor3 = chasespeed2; //4
                    speedtoenemy *= -scalefactor3;
                    Projectile.velocity = (Projectile.velocity * 40f + speedtoenemy) / 41f; //41
                }
            }
            else
            {
                bool gotoplayer = false;
                if (!gotoplayer)
                {
                    gotoplayer = Projectile.ai[0] == 1f;
                }
                float speedtoplayer = 8f;
                if (gotoplayer)
                {
                    speedtoplayer = 16f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 playerpos = player.Center - center2 + new Vector2(0f, -60f);
                float LengthToPlayer = playerpos.Length();
                if (LengthToPlayer > 200f && speedtoplayer < 8f)
                {
                    speedtoplayer = 10f;
                }
                if (LengthToPlayer < idledistance && gotoplayer && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (LengthToPlayer > 2700f)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (LengthToPlayer > 70f)
                {
                    playerpos.Normalize();
                    playerpos *= speedtoplayer;
                    Projectile.velocity = (Projectile.velocity * 40f + playerpos) / 41f;
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }

            }

            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > firerate)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.ai[1] == 0f && gotoenemy && mindistance < 600f)
                {

                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        switch (AttackMode)
                        {
                            case 0:
                                    SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
                                    Vector2 aimlaser = objectivepos - Projectile.Center;
                                    aimlaser.Normalize();
                                    aimlaser = aimlaser.RotatedBy(MathHelper.ToRadians(30 * -laserdirection));
                                    float angularChange = (MathHelper.Pi / 180f) * 1.1f * laserdirection;
                                    //aimlaser *= 12f;
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, aimlaser, ModContent.ProjectileType<EndoBeam>(), Projectile.damage, 0f, Projectile.owner, angularChange, (float)Projectile.whoAmI);
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
                                    Projectile.netUpdate = true;
                                    break;

                            case 2: Projectile.ai[0] = 2f;
                                    Vector2 aimtoenemy = objectivepos - Projectile.Center;
                                    aimtoenemy.Normalize();
                                    Projectile.velocity = aimtoenemy * 18f;
                                    Projectile.netUpdate = true;
                                    break;
                            case 3: limbs.ai[0] = 4f;
                                    Projectile.netUpdate = true;
                                    break;
                            default:break;
                        }
                    }
                }
            }
            if (limbs.ai[0] == 2f && Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] += 1f;
                limbs.ai[0] = 3f;
                Projectile.netUpdate = true;
            }

            //Tilting and change directions
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X * 0.07f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/EndoCooperBody_Glow").Value;
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.LightSkyBlue, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override bool? CanDamage() => AttackMode == 2;
    }
}
