using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WildBumblefuck : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = Vector2.UnitX * 36f
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/BumbleFolly";

        public override void SetDefaults()
        {
            NPC.npcSlots = 1f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 90;
            NPC.width = 120;
            NPC.height = 80;
            NPC.defense = 20;
            NPC.LifeMaxNERB(9375, 11250, 5000); // Old HP - 12000, 15000
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit51;
            NPC.DeathSound = SoundID.NPCDeath46;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.WildBumblefuck")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedMoonlord || spawnInfo.Player.Calamity().ZoneSunkenSea || !spawnInfo.Player.ZoneJungle)
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;

            return SpawnCondition.SurfaceJungle.Chance * 0.14f;
        }

        public override void AI()
        {
            CalamityAI.Bumblebirb2AI(NPC, Mod, false);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<EffulgentFeather>(), 1, 5, 7);
        public override void OnKill()
        {
            if (Main.zenithWorld)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 fireFrom = new Vector2(NPC.Center.X + (40 * i) - 120, NPC.Center.Y - 900f);
                        Vector2 ai0 = NPC.Center - fireFrom;
                        float ai = Main.rand.Next(100);
                        Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), NPC.damage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += NPC.ai[0] == 2.1f ? 1.5 : 1D;
            if (Main.zenithWorld)
            {
                NPC.frameCounter += 2D;
            }
            if (NPC.frameCounter > 4D) //iban said the time between frames was 5 so using that as a base
            {
                NPC.frameCounter = 0D;
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            int afterimageAmt = NPC.ai[0] == 2.1f ? 7 : 0;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.Gold, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 244, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 244, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
