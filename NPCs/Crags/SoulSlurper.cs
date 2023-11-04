using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class SoulSlurper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.npcSlots = 1f;
            NPC.damage = 30;
            NPC.width = 60;
            NPC.height = 40;
            NPC.defense = 30;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 60;
            NPC.knockBackResist = 0.65f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.damage = 60;
                NPC.defense = 45;
                NPC.lifeMax = 2000;
            }
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SoulSlurperBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BrimstoneCragsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SoulSlurper")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void AI()
        {
            bool provy = DownedBossSystem.downedProvidence;
            Player target = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || target.dead)
            {
                NPC.TargetClosest(true);
            }
            float npcSpeed = 5f;
            float npcAcceleration = 0.07f;
            Vector2 npcCenter = NPC.Center;
            float targetX = target.Center.X;
            float targetY = target.Center.Y;
            targetX = (float)((int)(targetX / 8f) * 8);
            targetY = (float)((int)(targetY / 8f) * 8);
            npcCenter.X = (float)((int)(npcCenter.X / 8f) * 8);
            npcCenter.Y = (float)((int)(npcCenter.Y / 8f) * 8);
            targetX -= npcCenter.X;
            targetY -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetX * targetX + targetY * targetY));
            float accelerateDistance = targetDistance;
            bool tooFar = false;
            if (targetDistance > 600f)
            {
                tooFar = true;
            }
            if (targetDistance == 0f)
            {
                targetX = NPC.velocity.X;
                targetY = NPC.velocity.Y;
            }
            else
            {
                targetDistance = npcSpeed / targetDistance;
                targetX *= targetDistance;
                targetY *= targetDistance;
            }
            if (accelerateDistance > 100f)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] > 0f)
                {
                    NPC.velocity.Y += 0.023f;
                }
                else
                {
                    NPC.velocity.Y -= 0.023f;
                }
                if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                {
                    NPC.velocity.X += 0.023f;
                }
                else
                {
                    NPC.velocity.X -= 0.023f;
                }
                if (NPC.ai[0] > 200f)
                {
                    NPC.ai[0] = -200f;
                }
            }
            if (target.dead)
            {
                targetX = (float)NPC.direction * npcSpeed / 2f;
                targetY = -npcSpeed / 2f;
            }
            if (NPC.velocity.X < targetX)
            {
                NPC.velocity.X += npcAcceleration;
            }
            else if (NPC.velocity.X > targetX)
            {
                NPC.velocity.X -= npcAcceleration;
            }
            if (NPC.velocity.Y < targetY)
            {
                NPC.velocity.Y += npcAcceleration;
            }
            else if (NPC.velocity.Y > targetY)
            {
                NPC.velocity.Y -= npcAcceleration;
            }
            NPC.localAI[0] += 1f;
            if (NPC.justHit)
            {
                NPC.localAI[0] = 0f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= 120f)
            {
                NPC.localAI[0] = 0f;
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
                {
                    int dmg = 30;
                    if (Main.expertMode)
                    {
                        dmg = 22;
                    }
                    int projType = ModContent.ProjectileType<BrimstoneBarrage>();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), npcCenter.X, npcCenter.Y, targetX, targetY, projType, dmg + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
                }
            }
            int npcTileX = (int)NPC.Center.X;
            int npcTileY = (int)NPC.Center.Y;
            npcTileX /= 16;
            npcTileY /= 16;
            if (!WorldGen.SolidTile(npcTileX, npcTileY))
            {
                Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0.75f, 0f, 0f);
            }
            if (targetX > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2((double)targetY, (double)targetX);
            }
            if (targetX < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)targetY, (double)targetX) + MathHelper.Pi;
            }
            float recoilSpeed = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -recoilSpeed;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -recoilSpeed;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1.5f)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1.5f)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            if (tooFar)
            {
                if ((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f))
                {
                    if (Math.Abs(NPC.velocity.X) < 12f)
                    {
                        NPC.velocity.X *= 1.05f;
                    }
                }
                else
                {
                    NPC.velocity.X *= 0.9f;
                }
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2));
            int afterimageAmt = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crags/SoulSlurperGlow").Value;
            Color redGlow = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color glowmaskAfterimageColor = redGlow;
                    glowmaskAfterimageColor = Color.Lerp(glowmaskAfterimageColor, Color.White, 0.5f);
                    glowmaskAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 glowmaskAfterimagePos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    glowmaskAfterimagePos -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
                    glowmaskAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, glowmaskAfterimagePos, NPC.frame, glowmaskAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                }
            }

            spriteBatch.Draw(texture, drawLocation, NPC.frame, redGlow, NPC.rotation, halfSizeTexture, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule fishing = npcLoot.DefineConditionalDropSet(() => Main.hardMode);
            fishing.Add(ModContent.ItemType<SlurperPole>(), 30, hideLootReport: !Main.hardMode);
            fishing.AddFail(ModContent.ItemType<SlurperPole>(), 10, hideLootReport: Main.hardMode);

            LeadingConditionRule hardmode = npcLoot.DefineConditionalDropSet(DropHelper.Hardmode());
            LeadingConditionRule postProv = npcLoot.DefineConditionalDropSet(DropHelper.PostProv());
            hardmode.Add(ModContent.ItemType<EssenceofHavoc>(), 2);
            postProv.Add(ModContent.ItemType<Bloodstone>(), 4);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 60, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SoulSlurper").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SoulSlurper2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SoulSlurper3").Type, NPC.scale);
                }
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 10; i++)
                {
                    int brimDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[brimDust].scale = 0.5f;
                        Main.dust[brimDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    int brimDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[brimDust2].noGravity = true;
                    Main.dust[brimDust2].velocity *= 5f;
                    brimDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust2].velocity *= 2f;
                }
            }
        }
    }
}
