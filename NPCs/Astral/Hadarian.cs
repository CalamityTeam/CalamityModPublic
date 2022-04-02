using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Hadarian : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadarian");
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/HadarianGlow");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 40;
            npc.aiStyle = -1;
            npc.damage = 50;
            npc.defense = 8;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 330;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.knockBackResist = 0.75f;
            npc.value = Item.buyPrice(0, 0, 15, 0);
            banner = npc.type;
            bannerItem = ModContent.ItemType<HadarianBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 80;
                npc.defense = 18;
                npc.knockBackResist = 0.65f;
                npc.lifeMax = 490;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoVultureAI(npc, (CalamityWorld.death ? 0.225f : 0.15f), (CalamityWorld.death ? 5.25f : 3.5f), 32, 50, 150, 150);

            //usually done in framing but I put it here because it makes more sense to.
            npc.rotation = npc.velocity.X * 0.1f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y == 0f)
            {
                npc.spriteDirection = npc.direction;
            }
            else
            {
                if ((double)npc.velocity.X > 0.5)
                {
                    npc.spriteDirection = 1;
                }
                if ((double)npc.velocity.X < -0.5)
                {
                    npc.spriteDirection = -1;
                }
            }

            if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
            {
                npc.frame.Y = 0;
                npc.frameCounter = 0.0;
            }
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter > 5)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                }
                if (npc.frame.Y > frameHeight * 6 || npc.frame.Y == 0)
                {
                    npc.frame.Y = frameHeight;
                }
            }

            DoWingDust(frameHeight);
        }

        private void DoWingDust(int frameHeight)
        {
            int frame = npc.frame.Y / frameHeight;
            Dust d = null;
            switch (frame)
            {
                case 1:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(38, 16, 22, 20), Vector2.Zero, 0.35f);
                    break;
                case 2:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(38, 24, 30, 14), Vector2.Zero);
                    break;
                case 3:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(44, 28, 32, 20), Vector2.Zero);
                    break;
                case 4:
                    d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 82, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(42, 36, 18, 30), Vector2.Zero, 0.3f);
                    break;
            }

            if (d != null)
            {
                d.customData = 0.03f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[0] == 0f)
            {
                Vector2 position = npc.Bottom - new Vector2(19f, 42f);
                //20 34 38 42
                Rectangle src = new Rectangle(20, 34, 38, 42);
                spriteBatch.Draw(Main.npcTexture[npc.type], position - Main.screenPosition, src, drawColor, npc.rotation, default, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                //draw glowmask
                spriteBatch.Draw(glowmask, position - Main.screenPosition, src, Color.White * 0.6f, npc.rotation, default, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                return false;
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[0] != 0f)
            {
                Vector2 origin = new Vector2(41f, 39f);

                //draw glowmask
                spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition - new Vector2(0f, 12f), npc.frame, Color.White * 0.6f, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit"), npc.Center);
                        break;
                    case 1:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit2"), npc.Center);
                        break;
                    case 2:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit3"), npc.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 20);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/Hadarian/HadarianGore" + i));
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Tile tile = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY];
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(3) && spawnInfo.spawnTileType == ModContent.TileType<AstralSand>() && tile.wall == WallID.None)
            {
                return 0.25f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Stardust>(), 2, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Stardust>(), Main.expertMode);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<HadarianMembrane>(), CalamityWorld.downedAstrageldon, 2, 2, 3);
        }
    }
}
