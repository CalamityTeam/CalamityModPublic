using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.DevourerofGods
{
    public class CosmicGuardianTail : ModNPC
    {
        public int invinceTime = 180;
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.CosmicGuardianHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.width = 30;
            NPC.height = 50;
            NPC.defense = 70;
            NPC.lifeMax = 50000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (invinceTime > 0)
            {
                invinceTime--;
                NPC.damage = 0;
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.damage = NPC.defDamage;
                NPC.dontTakeDamage = false;
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CosmicGuardianHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            NPC.Opacity = Main.npc[(int)NPC.ai[2]].Opacity;

            Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float num191 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float num192 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    num191 = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2(num192, num191) + 1.57f;
                num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
                int num194 = NPC.width;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;
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

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/CosmicGuardianTailGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return NPC.Opacity >= 1f && invinceTime <= 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("DoT2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("DoT3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("DoT4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("DoT5").Type, 1f);
                }
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 90, true);
        }
    }
}
