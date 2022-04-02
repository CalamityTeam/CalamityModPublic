using CalamityMod.CalPlayer;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class VileFeederSummon : ModProjectile
    {
        private bool spawnDust = true;
        private int eaterCooldown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Eater");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.Calamity().lineColor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.Calamity().lineColor = reader.ReadInt32();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (spawnDust)
            {
                projectile.Calamity().lineColor = -1;
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;
                    int num228 = Dust.NewDust(source + dustVel, 0, 0, 7, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = dustVel;
                }
                spawnDust = false;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<VileFeederSummon>();
            player.AddBuff(ModContent.BuffType<VileFeederBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.vileFeeder = false;
                }
                if (modPlayer.vileFeeder)
                {
                    projectile.timeLeft = 2;
                }
            }

            if (eaterCooldown < 0)
                eaterCooldown = 0;

            if (projectile.ai[0] != 3f)
            {
                if (eaterCooldown > 0)
                    eaterCooldown--;
                projectile.ChargingMinionAI(640f, 1100f, 2400f, 150f, 0, 40f, 8f, 4f, new Vector2(0f, -60f), 40f, 8f, false, false);
                projectile.frameCounter++;
                if (projectile.frameCounter > 3)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 4)
                {
                    projectile.frame = 0;
                }
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(270);
            }
            else
            {
                projectile.frame = 0;
                projectile.extraUpdates = 0;
                bool breakAway = false;
                bool spawnDust = false;
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    spawnDust = true;
                }
                int npcIndex = projectile.Calamity().lineColor;
                if (projectile.localAI[0] >= 600000f) //tryna make it stay on there "forever" without glitching
                {
                    breakAway = true;
                }
                else if (!npcIndex.WithinBounds(Main.maxNPCs))
                {
                    breakAway = true;
                }
                else if (Main.npc[npcIndex].active && !Main.npc[npcIndex].dontTakeDamage && Main.npc[npcIndex].defense < 9999)
                {
                    projectile.Center = Main.npc[npcIndex].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[npcIndex].gfxOffY;
                    if (spawnDust)
                    {
                        Main.npc[npcIndex].HitEffect(0, 1.0);
                    }
                }
                else
                {
                    breakAway = true;
                }
                if (breakAway)
                {
                    projectile.ai[0] = 0f;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                }
                if (projectile.owner == Main.myPlayer)
                {
                    if (eaterCooldown > 0)
                        eaterCooldown -= Main.rand.Next(1,3);

                    if (eaterCooldown <= 0)
                    {
                        int projNumber = Main.rand.Next(1,3);
                        for (int index2 = 0; index2 < projNumber; index2++)
                        {
                            float xVector = (float)Main.rand.Next(-35, 36) * 0.02f;
                            float yVector = (float)Main.rand.Next(-35, 36) * 0.02f;
                            xVector *= 10f;
                            yVector *= 10f;
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, xVector, yVector, ModContent.ProjectileType<VileFeederProjectile>(), (int)(projectile.damage * 1.25f), projectile.knockBack, projectile.owner, 0f, 0f);
                        }
                        eaterCooldown = 80;
                    }
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);

            if (projectile.owner == Main.myPlayer)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    //covers most edge cases like voodoo dolls
                    if (npc.active && !npc.dontTakeDamage && npc.defense < 9999 && npc.Calamity().DR < 0.99f &&
                        ((projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && projectile.owner < Main.maxPlayers && player.killClothier))) ||
                        (projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(npc))
                        {
                            bool stickingToNPC;
                            //Solar Crawltipede tail has special collision
                            if (npc.type == NPCID.SolarCrawltipedeTail)
                            {
                                Rectangle rect = npc.getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                stickingToNPC = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                stickingToNPC = projectile.Colliding(myRect, npc.getRect());
                            }
                            if (stickingToNPC)
                            {
                                //reflect projectile if the npc can reflect it (like Selenians)
                                if (npc.reflectingProjectiles && projectile.CanReflect())
                                {
                                    npc.ReflectProjectile(projectile.whoAmI);
                                    return;
                                }

                                //let the projectile know it is sticking and the npc it is sticking too
                                projectile.ai[0] = 3f;
                                projectile.Calamity().lineColor = npcIndex;

                                //follow the NPC
                                projectile.velocity = (npc.Center - projectile.Center) * 0.75f;

                                projectile.netUpdate = true;

                                //Count how many projectiles are attached, delete as necessary
                                Point[] array2 = new Point[10];
                                int projCount = 0;
                                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                                {
                                    Projectile proj = Main.projectile[projIndex];
                                    if (projIndex != projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == projectile.type && proj.ai[0] == 3f && proj.Calamity().lineColor == npcIndex)
                                    {
                                        array2[projCount++] = new Point(projIndex, proj.timeLeft);
                                        if (projCount >= array2.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (projCount >= array2.Length)
                                {
                                    int num30 = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[num30].Y)
                                        {
                                            num30 = m;
                                        }
                                    }
                                    Main.projectile[array2[num30].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int num214 = texture.Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override bool CanDamage() => projectile.ai[0] != 3f;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
