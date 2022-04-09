using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;

namespace CalamityMod.NPCs.Astral
{
    public class StellarCulex : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Culex");
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/StellarCulexGlow", AssetRequestMode.ImmediateLoad).Value;
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 50;
            NPC.aiStyle = 14; //bats
            NPC.npcSlots = 0.5f; //needed?
            NPC.damage = 55;
            NPC.defense = 18;
            NPC.DR_NERD(0.15f);
            NPC.knockBackResist = 0.65f;
            NPC.lifeMax = 210;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.DeathSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCKilled/AstralEnemyDeath");
            AnimationType = NPCID.GiantFlyingFox;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<StellarCulexBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 90;
                NPC.defense = 28;
                NPC.knockBackResist = 0.55f;
                NPC.lifeMax = 320;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            int frame = NPC.frame.Y / frameHeight;
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 100, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(66, 10, (frame == 0 || frame == 3) ? 32 : 24, 16), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit"), NPC.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit2"), NPC.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit3"), NPC.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Gore.NewGore(NPC.Center, NPC.velocity * 0.3f, Mod.Find<ModGore>("StellarCulexGore" + i).Type);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 origin = new Vector2(50, 30f);
            spriteBatch.Draw(glowmask, NPC.Center - screenPos, NPC.frame, Color.White * 0.6f, NPC.rotation, origin, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(2))
            {
                return 0.16f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.AddIf(() => !Main.expertMode, ModContent.ItemType<Stardust>(), 2, 1, 2);
            npcLoot.AddIf(() => Main.expertMode, ModContent.ItemType<Stardust>(), 1, 1, 3);
            npcLoot.AddIf(() => DownedBossSystem.downedAstrumAureus, ModContent.ItemType<StarbusterCore>(), 7);
        }
    }
}
