using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AstrumAureus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.Items.SummonItems
{
    public class AstralChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Astral Chunk");
            Tooltip.SetDefault("Summons Astrum Aureus when used in the astral infection during nighttime\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && player.Calamity().ZoneAstral && !NPC.AnyNPCs(ModContent.NPCType<AstrumAureus>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC(new EntitySource_BossSpawn(player), (int)(player.position.X + Main.rand.Next(-250, 251)), (int)(player.position.Y - 500f), ModContent.NPCType<AstrumAureus>(), 1);
                Main.npc[npc].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(npc);
            }
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<AstrumAureus>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(30).
                AddIngredient(ItemID.FallenStar, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
