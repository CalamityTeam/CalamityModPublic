using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.NPCs;
namespace CalamityMod.Items
{
    public class AncientMedallion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Medallion");
            Tooltip.SetDefault("A very old temple medallion\n" +
                "Summons the Ravager");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.rare = 8;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<RavagerBody>()) && player.ZoneOverworldHeight;
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)(player.position.X + (float)Main.rand.Next(-100, 101)), (int)(player.position.Y - 250f), ModContent.NPCType<RavagerBody>(), 0, 0f, 0f, 0f, 0f, 255);
                Main.PlaySound(SoundID.Roar, player.position, 0);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 5);
            recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
