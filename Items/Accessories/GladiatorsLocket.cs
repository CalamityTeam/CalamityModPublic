using System;
using CalamityMod.CustomRecipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GladiatorsLocket : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 54;
            Item.value = Item.buyPrice(gold: 25); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundGladiatorsLocket)
            {
                RecipeUnlockHandler.HasFoundGladiatorsLocket = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float statPower = (float)Math.Round(0.2f * Utils.GetLerpValue(1, 0.5f, ((float)player.statLife / (float)player.statLifeMax2), true), 2);
            player.Calamity().gladiatorSword = true;
            player.GetDamage<GenericDamageClass>() += statPower;
            player.moveSpeed += statPower;
        }
    }
}
