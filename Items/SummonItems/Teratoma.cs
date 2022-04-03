using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.HiveMind;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class Teratoma : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teratoma");
            Tooltip.SetDefault("Summons the Hive Mind when used in the corruption\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneCorrupt && !NPC.AnyNPCs(ModContent.NPCType<HiveMind>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<HiveMind>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<HiveMind>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.RottenChunk, 13).AddIngredient(ModContent.ItemType<TrueShadowScale>(), 7).AddIngredient(ItemID.DemoniteBar, 3).AddTile(TileID.DemonAltar).Register();
        }
    }
}
