using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    public class AstrumDeusBody : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.AstrumDeusHead.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 38;
            NPC.height = 44;
            NPC.defense = 35;
            NPC.DR_NERD(0.2f);
            NPC.LifeMaxNERB(200000, 240000, 650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.5f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.4f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.35f;
            else if (Main.expertMode)
                NPC.scale *= 1.2f;

            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = AstrumDeusHead.DeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.dontCountMe = true;
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

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            CalamityAI.AstrumDeusAI(NPC, Mod, false);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool altBodyTextures = NPC.localAI[3] == 1f;
            bool drawCyan = NPC.Calamity().newAI[3] >= (Main.getGoodWorld ? 300f : 600f);
            bool deathModeEnragePhase = Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] == 3f;
            bool doubleWormPhase = NPC.Calamity().newAI[0] != 0f && !deathModeEnragePhase;

            Texture2D texture = altBodyTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyAltSpectral").Value : TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyGlow2").Value;
            Vector2 vector = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);

            Vector2 vector2 = NPC.Center - screenPos;
            vector2 -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            vector2 += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, vector2, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            texture = altBodyTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyAltGlow").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyGlow").Value;
            Color phaseColor = drawCyan ? Color.Cyan : Color.Orange;
            if (doubleWormPhase)
            {
                texture = drawCyan ? texture : (altBodyTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyAltGlow2").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyGlow3").Value);
                texture2 = drawCyan ? ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyGlow4").Value : texture2;
            }
            Color color = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Cyan, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);
            Color color2 = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Orange, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);

            int timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 1 : 2;
            for (int i = 0; i < timesToDraw; i++)
                spriteBatch.Draw(texture, vector2, NPC.frame, color, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            if (!altBodyTextures)
            {
                timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 2 : 1;
                for (int i = 0; i < timesToDraw; i++)
                    spriteBatch.Draw(texture2, vector2, NPC.frame, color2, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);
            }

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !NPC.dontTakeDamage;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && !Main.zenithWorld) // I value people's computers
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 100, true);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }
    }
}
