using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class RotomRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Triboluminescent Etomer");
            Tooltip.SetDefault("Summons an electric troublemaker\n" +
			"A little note is attached:\n" +
			"Thank you, Aloe! Very much appreciated from Ben");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.noMelee = true;
            item.width = 30;
            item.height = 34;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.UseSound = SoundID.Item113;
            item.shoot = ModContent.ProjectileType<RotomPet>();
            item.buffType = ModContent.BuffType<RotomBuff>();
            item.rare = 3;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PrismShard>(), 5);
            recipe.AddRecipeGroup("AnyGoldBar", 8);
            recipe.AddIngredient(ModContent.ItemType<DemonicBoneAsh>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
