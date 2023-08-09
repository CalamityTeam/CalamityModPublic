using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.AstrumAureus
{
    public class AureusSpawn : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.width = 90;
            NPC.height = 60;
            NPC.Opacity = 0f;
            NPC.defense = 10;
            NPC.lifeMax = 5000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Force despawn if Astrum Aureus isn't active
            if (CalamityGlobalNPC.astrumAureus < 0 || !Main.npc[CalamityGlobalNPC.astrumAureus].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.6f * NPC.Opacity * NPC.scale, 0.25f * NPC.Opacity * NPC.scale, 0f);

            NPC.rotation = Math.Abs(NPC.velocity.X) * NPC.direction * 0.04f;

            NPC.spriteDirection = NPC.direction;

            if (NPC.Opacity < 1f)
            {
                NPC.Opacity += 0.01f;

                if (NPC.Opacity > 0.33f)
                    NPC.velocity *= 0.95f;

                if (NPC.Opacity >= 1f)
                {
                    NPC.Opacity = 1f;
                    NPC.dontTakeDamage = false;
                }

                for (int i = 0; i < 8; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), NPC.velocity.X, NPC.velocity.Y, 255, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                }

                return;
            }

            NPC.TargetClosest();

            // Push away from each other
            float pushVelocity = 0.5f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                {
                    if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                    {
                        if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < (160f + 30f * (NPC.scale - 1f)))
                        {
                            if (NPC.position.X < Main.npc[i].position.X)
                                NPC.velocity.X = NPC.velocity.X - pushVelocity;
                            else
                                NPC.velocity.X = NPC.velocity.X + pushVelocity;

                            if (NPC.position.Y < Main.npc[i].position.Y)
                                NPC.velocity.Y = NPC.velocity.Y - pushVelocity;
                            else
                                NPC.velocity.Y = NPC.velocity.Y + pushVelocity;
                        }
                    }
                }
            }

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phase 2 check
            bool phase2 = lifeRatio <= 0.5f || Main.dayTime;

            // Charge towards the target and explode after some time
            int inertia = 30;
            Vector2 vector = Main.player[NPC.target].Center - NPC.Center;
            Vector2 vector2 = Main.npc[CalamityGlobalNPC.astrumAureus].Center - NPC.Center;
            float distanceFromAureus = vector2.Length();
            float distanceFromTarget = vector.Length();
            float chargeVelocity = 8f;
            if (phase2)
            {
                inertia = 50;
                chargeVelocity = 16f;
                float engagePhase2GateValue = 60f;
                float enlargeGateValue = engagePhase2GateValue + 300f;

                // Pause when phase 2 is triggered
                NPC.ai[0] += 1f;
                if (NPC.ai[0] < engagePhase2GateValue)
                {
                    NPC.velocity *= 0.95f;
                    return;
                }

                // Emit dust and sound when phase 2 charging begins
                if (NPC.ai[0] == engagePhase2GateValue)
                {
                    SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound, NPC.Center);

                    for (int j = 0; j < 10; j++)
                    {
                        int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                        Main.dust[dust].velocity *= 1.66f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[dust].scale = 0.5f;
                            Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                        Main.dust[dust].noGravity = true;
                    }

                    for (int k = 0; k < 20; k++)
                    {
                        int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 173, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 2f;
                        dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                        Main.dust[dust].velocity *= 1.33f;
                        Main.dust[dust].noGravity = true;
                    }
                }

                // Grow in size
                float enlargeDuration = 180f;
                if (NPC.ai[0] >= enlargeGateValue)
                {
                    NPC.Calamity().newAI[0] += 1f;
                    NPC.scale = MathHelper.Lerp(1f, 2f, NPC.Calamity().newAI[0] / enlargeDuration);
                    NPC.width = (int)(90f * NPC.scale);
                    NPC.height = (int)(60f * NPC.scale);
                }

                // Check if it can explode
                Point point = NPC.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point);
                bool explodeOnCollision = tileSafely.HasUnactuatedTile && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType] && !TileID.Sets.Platforms[tileSafely.TileType];
                bool explodeOnAureus = distanceFromAureus < (180f + 30f * (NPC.scale - 1f)) && !Main.dayTime;
                if (vector.Length() < (60f + 30f * (NPC.scale - 1f)) || explodeOnCollision || explodeOnAureus || NPC.Calamity().newAI[0] >= enlargeDuration)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.checkDead();
                    return;
                }
            }

            // Move towards the target
            chargeVelocity += distanceFromTarget / 200f;
            if (distanceFromTarget > chargeVelocity)
            {
                vector.Normalize();
                vector *= chargeVelocity;
            }
            NPC.velocity = (NPC.velocity * (inertia - 1) + vector) / inertia;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * balance);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.Calamity().newAI[0] >= 180f)
                return false;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (NPC.ai[0] >= 60f)
                NPC.DrawBackglow(Color.Lerp(Color.Cyan, Color.Orange, (float)Math.Sin(Main.GlobalTimeWrappedHourly) / 2f + 0.5f) with { A = 0 }, 2f + 8f * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi) + 1f), spriteEffects, NPC.frame, screenPos);

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 10;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AureusSpawnGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Orange, 0.5f) * NPC.Opacity;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(8) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            // Split into a halo of projectiles in death mode
            if (CalamityWorld.death || BossRushEvent.BossRushActive)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int totalProjectiles = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 36 : (3 + (int)((NPC.scale - 1f) * 3));
                    double radians = MathHelper.TwoPi / totalProjectiles;
                    int type = ModContent.ProjectileType<AstralLaser>();
                    int damage2 = NPC.GetProjectileDamage(type);
                    float velocity = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 10f : 6f;
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vector255, type, damage2, 0f, Main.myPlayer);
                    }
                }
            }

            // Return if Astrum Aureus isn't active
            if (CalamityGlobalNPC.astrumAureus < 0 || !Main.npc[CalamityGlobalNPC.astrumAureus].active)
                return;

            // Damage Aureus for a percentage of its HP if the spawn explodes on or near it
            if (Main.netMode != NetmodeID.MultiplayerClient && (Main.npc[CalamityGlobalNPC.astrumAureus].Center - NPC.Center).Length() < (200f + 30f * (NPC.scale - 1f)) && !Main.dayTime)
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Main.npc[CalamityGlobalNPC.astrumAureus].Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), (int)(Main.npc[CalamityGlobalNPC.astrumAureus].lifeMax / 200 * NPC.scale), 0f, Main.myPlayer, Main.npc[CalamityGlobalNPC.astrumAureus].whoAmI);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 173, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                NPC.position.X = NPC.position.X + NPC.width / 2;
                NPC.position.Y = NPC.position.Y + NPC.height / 2;
                NPC.damage = (int)(NPC.defDamage * NPC.scale);
                NPC.width = NPC.height = (int)(216 * NPC.scale);
                NPC.position.X = NPC.position.X - NPC.width / 2;
                NPC.position.Y = NPC.position.Y - NPC.height / 2;

                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                    Main.dust[num622].noGravity = true;
                }

                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                    Main.dust[num624].velocity *= 2f;
                    Main.dust[num624].noGravity = true;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            int debuffType = Main.zenithWorld ? ModContent.BuffType<GodSlayerInferno>() : ModContent.BuffType<AstralInfectionDebuff>();
            target.AddBuff(debuffType, (int)(180 * NPC.scale), true);
        }
    }
}
