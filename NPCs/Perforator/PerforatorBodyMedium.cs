using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    public class PerforatorBodyMedium : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PerforatorHeadMedium.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 6;
            NPC.LifeMaxNERB(150, 180, 7000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.25f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            NPC.Calamity().SplittingWorm = true;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            NPC.realLife = -1;

            // Target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    if (NPC.ai[2] > 0f)
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), NPC.type, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                    else
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), ModContent.NPCType<PerforatorTailMedium>(), NPC.whoAmI, 0f, 0f, 0f, 0f, 255);

                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                // Splitting effect
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPC.aiStyle)
                {
                    NPC.type = ModContent.NPCType<PerforatorHeadMedium>();
                    int whoAmI = NPC.whoAmI;
                    float num25 = NPC.life / (float)NPC.lifeMax;
                    float num26 = NPC.ai[0];
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * num25);
                    NPC.ai[0] = num26;
                    NPC.TargetClosest(true);
                    NPC.netUpdate = true;
                    NPC.whoAmI = whoAmI;
                }
                if (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]].aiStyle != NPC.aiStyle)
                {
                    int whoAmI2 = NPC.whoAmI;
                    float num27 = NPC.life / (float)NPC.lifeMax;
                    float num28 = NPC.ai[1];
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * num27);
                    NPC.ai[1] = num28;
                    NPC.TargetClosest(true);
                    NPC.netUpdate = true;
                    NPC.whoAmI = whoAmI2;
                }

                if (!NPC.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            }

            Vector2 vector2 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float num39 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float num40 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);

            num39 = (int)(num39 / 16f) * 16;
            num40 = (int)(num40 / 16f) * 16;
            vector2.X = (int)(vector2.X / 16f) * 16;
            vector2.Y = (int)(vector2.Y / 16f) * 16;
            num39 -= vector2.X;
            num40 -= vector2.Y;
            float num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);

            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector2 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    num39 = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - vector2.X;
                    num40 = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - vector2.Y;
                }
                catch
                {
                }

                NPC.rotation = (float)Math.Atan2(num40, num39) + MathHelper.PiOver2;
                num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);
                int num53 = NPC.width;
                num53 = (int)(num53 * NPC.scale);
                num52 = (num52 - num53) / num52;
                num39 *= num52;
                num40 *= num52;
                NPC.velocity = Vector2.Zero;
                NPC.position.X += num39;
                NPC.position.Y += num40;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Perforator/PerforatorBodyMediumGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
            
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld)
            {
                int type = ModContent.ProjectileType<IchorBlob>();
                int damage = NPC.GetProjectileDamage(type);
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY, type, damage, 0f, Main.myPlayer);

                for (int i = -1; i < 2; i++) //releases 3 Ichor Shots
                {
                    int type2 = ModContent.ProjectileType<IchorShot>();
                    Vector2 baseVelocity = Vector2.UnitY * Main.rand.NextFloat(-12.5f, -5f);
                    int spread = Main.rand.Next(16, 36);
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread * i)), type2, damage, 0f, Main.myPlayer);
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf3").Type, NPC.scale);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 120, true);
        }
    }
}
