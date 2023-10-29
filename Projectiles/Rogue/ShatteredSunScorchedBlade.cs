using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShatteredSunScorchedBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        int counter = 0;
        bool stealthOrigin = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
            counter++;
            if (counter == 1)
            {
                stealthOrigin = Projectile.ai[0] == 1f;
                Projectile.alpha += (int) Projectile.ai[1];
                Projectile.ai[0] = 0f;
            }
            if (counter == 20 && !Projectile.Calamity().stealthStrike && !stealthOrigin)
            {
                Projectile.tileCollide = true;
            }
            if (counter % 5 == 0)
            {
                Projectile.velocity *= 1.15f;
            }
            if (counter % 10 == 0)
            {
                if (!stealthOrigin && Projectile.alpha < 200)
                    Projectile.alpha += 6;
            }
            if (counter % 9 == 0 || (counter % 5 == 0 && Projectile.Calamity().stealthStrike))
            {
                int timesToSpawnDust = Projectile.Calamity().stealthStrike  ? 2 : 1;
                for (int i = 0; i < timesToSpawnDust; i++)
                {
                    int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, Projectile.Calamity().stealthStrike ? 1.8f : 1.3f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity *= 5f;
                    dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, Projectile.Calamity().stealthStrike ? 1.8f : 1.3f);
                    Main.dust[dusty].velocity *= 2f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Lighting.AddLight(Projectile.Center, 0.7f, 0.3f, 0f);
            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
            float targetDistStore;
            Vector2 homingPos = Projectile.position;
            bool isHoming = false;
            for (int j = 0; j < Main.maxNPCs; j++)
            {
                NPC nPC2 = Main.npc[j];
                if (nPC2.CanBeChasedBy(Projectile, false))
                {
                    float targetDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                    if (!isHoming)
                    {
                        targetDistStore = targetDist;
                        homingPos = nPC2.Center;
                        isHoming = true;
                    }
                }
            }
            if (isHoming && Projectile.ai[0] == 0f)
            {
                Vector2 homingDirection = homingPos - Projectile.Center;
                float homingLength = homingDirection.Length();
                homingDirection.Normalize();
                if (homingLength > 200f)
                {
                    float scaleFactor2 = 8f;
                    homingDirection *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + homingDirection) / 41f;
                }
                else
                {
                    homingDirection *= -4f;
                    Projectile.velocity = (Projectile.velocity * 40f + homingDirection) / 41f;
                }
            }

            if (Projectile.Calamity().stealthStrike)
            {
                float projX = Projectile.Center.X;
                float projY = Projectile.Center.Y;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                        float npcCenterY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                        float targetDistance = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                        if (targetDistance < 600f)
                        {
                            if (Main.npc[i].position.X < projX)
                            {
                                Main.npc[i].velocity.X += 0.25f;
                            }
                            else
                            {
                                Main.npc[i].velocity.X -= 0.25f;
                            }
                            if (Main.npc[i].position.Y < projY)
                            {
                                Main.npc[i].velocity.Y += 0.25f;
                            }
                            else
                            {
                                Main.npc[i].velocity.Y -= 0.25f;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                int numProj = 2;
                if (Projectile.owner == Main.myPlayer)
                {
                    Player owner = Main.player[Projectile.owner];
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 10f;
                    int spread = 6;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        int projDamage = (int)(Projectile.damage * 0.6f);
                        float kb = 1f;
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center.X, owner.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, Projectile.type, projDamage, kb, Projectile.owner, 1f, Projectile.alpha);
                        spread -= Main.rand.Next(2, 6);
                        Main.projectile[proj].ai[0] = 1f;
                    }
                    Projectile.Kill();
                }
            }
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                int numProj = 2;
                if (Projectile.owner == Main.myPlayer)
                {
                    Player owner = Main.player[Projectile.owner];
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 10f;
                    int spread = 6;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        int projDamage = (int)(Projectile.damage * 0.6f);
                        float kb = 1f;
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center.X, owner.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, Projectile.type, projDamage, kb, Projectile.owner, 1f, Projectile.alpha);
                        spread -= Main.rand.Next(2, 6);
                        Main.projectile[proj].ai[0] = 1f;
                    }
                    Projectile.Kill();
                }
            }
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 12; j++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 5f;
                dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
        }
    }
}
