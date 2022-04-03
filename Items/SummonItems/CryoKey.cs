using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Cryogen;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class CryoKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryo Key");
            Tooltip.SetDefault("Summons Cryogen when used in the tundra\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneSnow && !NPC.AnyNPCs(ModContent.NPCType<Cryogen>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Cryogen>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Cryogen>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddRecipeGroup("AnyIceBlock", 50).AddIngredient(ItemID.SoulofNight, 5).AddIngredient(ItemID.SoulofLight, 5).AddIngredient(ModContent.ItemType<EssenceofEleum>(), 8).AddTile(TileID.Anvils).Register();
        }
    }
}
