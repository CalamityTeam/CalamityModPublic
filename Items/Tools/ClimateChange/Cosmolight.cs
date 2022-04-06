using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class Cosmolight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmolight");
            Tooltip.SetDefault("Changes night to day and vice versa\n" +
                "Does not work while a boss is alive");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item60;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !CalamityPlayer.areThereAnyDamnBosses;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.time = 0.0;
                Main.dayTime = !Main.dayTime;
                if (Main.dayTime)
                {
                    if (++Main.moonPhase >= 8)
                    {
                        Main.moonPhase = 0;
                    }
                }
                CalamityNetcode.SyncWorld();
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FallenStar, 10).
                AddIngredient(ItemID.SoulofLight, 7).
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<EssenceofCinder>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
