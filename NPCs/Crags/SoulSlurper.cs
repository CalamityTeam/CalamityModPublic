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
            float num = 5f;
            float num2 = 0.07f;
            Vector2 source = NPC.Center;
            float num4 = target.Center.X;
            float num5 = target.Center.Y;
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            source.X = (float)((int)(source.X / 8f) * 8);
            source.Y = (float)((int)(source.Y / 8f) * 8);
            num4 -= source.X;
            num5 -= source.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            float num7 = num6;
            bool flag = false;
            if (num6 > 600f)
            {
                flag = true;
            }
            if (num6 == 0f)
            {
                num4 = NPC.velocity.X;
                num5 = NPC.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (num7 > 100f)
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
                num4 = (float)NPC.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (NPC.velocity.X < num4)
            {
                NPC.velocity.X += num2;
            }
            else if (NPC.velocity.X > num4)
            {
                NPC.velocity.X -= num2;
            }
            if (NPC.velocity.Y < num5)
            {
                NPC.velocity.Y += num2;
            }
            else if (NPC.velocity.Y > num5)
            {
                NPC.velocity.Y -= num2;
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
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), source.X, source.Y, num4, num5, projType, dmg + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
                }
            }
            int num10 = (int)NPC.Center.X;
            int num11 = (int)NPC.Center.Y;
            num10 /= 16;
            num11 /= 16;
            if (!WorldGen.SolidTile(num10, num11))
            {
                Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0.75f, 0f, 0f);
            }
            if (num4 > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2((double)num5, (double)num4);
            }
            if (num4 < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)num5, (double)num4) + MathHelper.Pi;
            }
            float num12 = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -num12;
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
                NPC.velocity.Y = NPC.oldVelocity.Y * -num12;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1.5f)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1.5f)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            if (flag)
            {
                if ((NPC.velocity.X > 0f && num4 > 0f) || (NPC.velocity.X < 0f && num4 < 0f))
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
            Vector2 vector11 = new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crags/SoulSlurperGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (float)(num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                }
            }

            spriteBatch.Draw(texture, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

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
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
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
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
