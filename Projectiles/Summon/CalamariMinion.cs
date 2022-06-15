using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class CalamariMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamari");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[1] == 0f)
            {
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 109, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                Projectile.localAI[1] += 1f;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }
            bool flag64 = Projectile.type == ModContent.ProjectileType<CalamariMinion>();
            player.AddBuff(ModContent.BuffType<Calamari>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.calamari = false;
                }
                if (modPlayer.calamari)
                {
                    Projectile.timeLeft = 2;
                }
            }
            if (Main.rand.NextBool(600))
            {
                SoundEngine.PlaySound(SoundID.Zombie35, Projectile.position);
            }
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] > 3f)
                {
                    SoundEngine.PlaySound(SoundID.Zombie34, Projectile.position);
                    int num = Dust.NewDust(Projectile.Center, 0, 0, 109, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1.4f);
                    Main.dust[num].scale = 0.5f + (float)Main.rand.NextDouble() * 0.3f;
                    Main.dust[num].velocity /= 2.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].noLight = true;
                }
                if (Projectile.ai[1] != 0f)
                {
                    return;
                }
                Projectile.ai[1] = 15f;
                Projectile.ai[0] = 0f;
                Projectile.velocity /= 5f;
                Projectile.velocity.Y = 0f;
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = 0;
                Projectile.netUpdate = true;
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = 0;
            }
            if (Projectile.extraUpdates > 1)
            {
                Projectile.extraUpdates = 0;
            }
            if (Projectile.numUpdates > 1)
            {
                Projectile.numUpdates = 0;
            }
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }
            Projectile.MinionAntiClump();
            Vector2 shootPosition = Projectile.position;
            float num10 = 3000f;
            bool flag = false;
            Vector2 center = player.Center;
            Vector2 value = new Vector2(0.5f);
            value.Y = 0f;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    Vector2 vector2 = npc.position + npc.Size * value;
                    float num12 = Vector2.Distance(vector2, center);
                    if (!flag && num12 < num10 && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        shootPosition = vector2;
                        flag = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (!flag)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC nPC = Main.npc[k];
                    if (nPC.CanBeChasedBy(Projectile, false))
                    {
                        Vector2 vector3 = nPC.position + nPC.Size * value;
                        float num13 = Vector2.Distance(vector3, center);
                        if (!flag && num13 < num10 && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, nPC.position, nPC.width, nPC.height))
                        {
                            num10 = num13;
                            shootPosition = vector3;
                            flag = true;
                            targetIndex = k;
                        }
                    }
                }
            }
            int num16 = 3500;
            if (flag)
            {
                num16 = 4000;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > num16)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (flag && Projectile.ai[0] == 0f)
            {
                Vector2 vector4 = shootPosition - Projectile.Center;
                float num17 = vector4.Length();
                vector4.Normalize();
                vector4 = shootPosition - Vector2.UnitY * 80f;
                int num18 = (int)vector4.Y / 16;
                if (num18 < 0)
                {
                    num18 = 0;
                }
                Tile tile = Main.tile[(int)vector4.X / 16, num18];
                if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                {
                    vector4 += Vector2.UnitY * 16f;
                    tile = Main.tile[(int)vector4.X / 16, (int)vector4.Y / 16];
                    if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                    {
                        vector4 += Vector2.UnitY * 16f;
                    }
                }
                vector4 -= Projectile.Center;
                num17 = vector4.Length();
                vector4.Normalize();
                if (num17 > 300f && num17 <= 1600f && Projectile.localAI[0] == 0f)
                {
                    Projectile.ai[0] = 2f;
                    Projectile.ai[1] = (int)(num17 / 10f);
                    Projectile.extraUpdates = (int)(Projectile.ai[1] * 2f);
                    Projectile.velocity = vector4 * 10f;
                    Projectile.localAI[0] = 60f;
                    return;
                }
                if (num17 > 200f)
                {
                    float scaleFactor2 = 20f;
                    vector4 *= scaleFactor2;
                    Projectile.velocity.X = (Projectile.velocity.X * 40f + vector4.X) / 41f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 40f + vector4.Y) / 41f;
                }
                if (num17 > 70f && num17 < 130f)
                {
                    float scaleFactor3 = 18f;
                    if (num17 < 100f)
                    {
                        scaleFactor3 = -7.5f;
                    }
                    vector4 *= scaleFactor3;
                    Projectile.velocity = (Projectile.velocity * 20f + vector4) / 21f;
                    if (Math.Abs(vector4.X) > Math.Abs(vector4.Y))
                    {
                        Projectile.velocity.X = (Projectile.velocity.X * 10f + vector4.X) / 11f;
                    }
                }
                else
                {
                    Projectile.velocity *= 0.97f;
                }
            }
            else
            {
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, Main.player[Projectile.owner].Center, 1, 1))
                {
                    Projectile.ai[0] = 1f;
                }
                float num21 = 12f; //6
                if (Projectile.ai[0] == 1f)
                {
                    num21 = 18f; //15
                }
                Vector2 center2 = Projectile.Center;
                Vector2 vector6 = player.Center - center2 + new Vector2(0f, -60f);
                float num23 = vector6.Length();
                if (num23 > 200f && num21 < 13.5f)
                {
                    num21 = 13.5f; //9
                }
                if (num23 < 800f && Projectile.ai[0] == 1f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (num23 > 4000f)
                {
                    Projectile.position.X = player.Center.X - (Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (Projectile.width / 2);
                }
                if (num23 > 70f)
                {
                    vector6.Normalize();
                    vector6 *= num21;
                    Projectile.velocity = (Projectile.velocity * 20f + vector6) / 21f;
                }
                else
                {
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    Projectile.velocity *= 1.01f;
                }
            }
            Projectile.rotation = Projectile.velocity.X * 0.025f;
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += 1f;
                if (Main.rand.Next(2) != 0)
                {
                    Projectile.ai[1] += 1f;
                }
            }
            if (Projectile.ai[1] > 20f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                float inkShootSpeed = 12f;
                int projID = ModContent.ProjectileType<CalamariInk>();
                if (flag)
                {
                    if (Math.Abs((shootPosition - Projectile.Center).ToRotation() - MathHelper.PiOver2) > MathHelper.PiOver4)
                    {
                        Projectile.velocity += (shootPosition - Projectile.Center - Vector2.UnitY * 80f).SafeNormalize(Vector2.Zero);
                        return;
                    }
                    if ((shootPosition - Projectile.Center).Length() <= 600f && Projectile.ai[1] == 0f)
                    {
                        Projectile.ai[1] += 1f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            SoundEngine.PlaySound(SoundID.Item111, Projectile.position);
                            Vector2 inkShootVelocity = Projectile.SafeDirectionTo(shootPosition, Vector2.UnitY) * inkShootSpeed;
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitY * 20f, inkShootVelocity, projID, Projectile.damage, 0f, Main.myPlayer, targetIndex, 0f);
                            if (Main.projectile.IndexInRange(p))
                                Main.projectile[p].originalDamage = Projectile.originalDamage;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = texture2D13.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
