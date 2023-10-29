using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class VileFeederSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private bool spawnDust = true;
        private int eaterCooldown = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.Calamity().lineColor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.Calamity().lineColor = reader.ReadInt32();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (spawnDust)
            {
                Projectile.Calamity().lineColor = -1;
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;
                    int dusty = Dust.NewDust(source + dustVel, 0, 0, 7, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity = dustVel;
                }
                spawnDust = false;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<VileFeederSummon>();
            player.AddBuff(ModContent.BuffType<VileFeederBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.vileFeeder = false;
                }
                if (modPlayer.vileFeeder)
                {
                    Projectile.timeLeft = 2;
                }
            }

            if (eaterCooldown < 0)
                eaterCooldown = 0;

            if (Projectile.ai[0] != 3f)
            {
                if (eaterCooldown > 0)
                    eaterCooldown--;
                Projectile.ChargingMinionAI(640f, 1100f, 2400f, 150f, 0, 40f, 8f, 4f, new Vector2(0f, -60f), 40f, 8f, false, false);
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(270);
            }
            else
            {
                Projectile.frame = 0;
                Projectile.extraUpdates = 0;
                bool breakAway = false;
                bool spawnDust = false;
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] % 30f == 0f)
                {
                    spawnDust = true;
                }
                int npcIndex = Projectile.Calamity().lineColor;
                if (Projectile.localAI[0] >= 600000f) //tryna make it stay on there "forever" without glitching
                {
                    breakAway = true;
                }
                else if (!npcIndex.WithinBounds(Main.maxNPCs))
                {
                    breakAway = true;
                }
                else if (Main.npc[npcIndex].active && !Main.npc[npcIndex].dontTakeDamage && Main.npc[npcIndex].defense < 9999)
                {
                    Projectile.Center = Main.npc[npcIndex].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcIndex].gfxOffY;
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
                    Projectile.ai[0] = 0f;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
                if (Projectile.owner == Main.myPlayer)
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
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, xVector, yVector, ModContent.ProjectileType<VileFeederProjectile>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack, Projectile.owner);
                        }
                        eaterCooldown = 80;
                    }
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            Rectangle myRect = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);

            if (Projectile.owner == Main.myPlayer)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    //covers most edge cases like voodoo dolls
                    if (npc.active && !npc.dontTakeDamage && npc.defense < 9999 && npc.Calamity().DR < 0.99f &&
                        ((Projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && Projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && Projectile.owner < Main.maxPlayers && player.killClothier))) ||
                        (Projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (Projectile.owner < 0 || npc.immune[Projectile.owner] == 0 || Projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !Projectile.ownerHitCheck)
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
                                stickingToNPC = Projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                stickingToNPC = Projectile.Colliding(myRect, npc.getRect());
                            }
                            if (stickingToNPC)
                            {
                                //reflect projectile if the npc can reflect it (like Selenians)
                                if (npc.reflectsProjectiles && Projectile.CanBeReflected())
                                {
                                    npc.ReflectProjectile(Projectile);
                                    return;
                                }

                                //let the projectile know it is sticking and the npc it is sticking too
                                Projectile.ai[0] = 3f;
                                Projectile.Calamity().lineColor = npcIndex;

                                //follow the NPC
                                Projectile.velocity = (npc.Center - Projectile.Center) * 0.75f;

                                Projectile.netUpdate = true;

                                //Count how many projectiles are attached, delete as necessary
                                Point[] array2 = new Point[10];
                                int projCount = 0;
                                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                                {
                                    Projectile proj = Main.projectile[projIndex];
                                    if (projIndex != Projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == Projectile.type && proj.ai[0] == 3f && proj.Calamity().lineColor == npcIndex)
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
                                    projCount = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[projCount].Y)
                                        {
                                            projCount = m;
                                        }
                                    }
                                    Main.projectile[array2[projCount].X].Kill();
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int framing = texture.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
