using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlagueChargerLarge : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PlagueCharger.DisplayName");
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.damage = 45;
            NPC.width = 36;
            NPC.height = 30;
            NPC.defense = 25;
            NPC.scale *= 0.75f;
            NPC.lifeMax = 300;
            NPC.aiStyle = NPCAIStyleID.Flying;
            AIType = NPCID.Bee;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.Bee;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            Banner = ModContent.NPCType<PlagueCharger>();
            BannerItem = ModContent.ItemType<PlagueChargerBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PlagueCharger")
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
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
                    afterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlagueEnemies/PlagueChargerGlow").Value;
            Color redEyeColor = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color redEyeAfterimageColor = redEyeColor;
                    redEyeAfterimageColor = Color.Lerp(redEyeAfterimageColor, Color.White, 0.5f);
                    redEyeAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 redEyeAfterimagePos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    redEyeAfterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    redEyeAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, redEyeAfterimagePos, NPC.frame, redEyeAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, redEyeColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 2, 3);
            npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.Stinger, 4, 2));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedGolemBoss || spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.12f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Plague>(), 90, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.PlagueBoomSound, NPC.Center);
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("PlagueCharger").Type, 0.75f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("PlagueCharger2").Type, 0.75f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("PlagueCharger3").Type, 0.75f);
                }
            }
        }
    }
}
