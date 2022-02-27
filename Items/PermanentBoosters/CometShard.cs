using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class CometShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet Shard");
            Tooltip.SetDefault("Permanently increases maximum mana by 50");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.rare = ItemRarityID.LightRed;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item29;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.cShard)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                if (Main.myPlayer == player.whoAmI)
                {
                    player.ManaEffect(50);
                }
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.cShard = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddIngredient(ItemID.FallenStar, 20);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 150);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
