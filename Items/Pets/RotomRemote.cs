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
            item.useTime = item.useAnimation = 20;
            item.shoot = ModContent.ProjectileType<RotomPet>();
            item.buffType = ModContent.BuffType<RotomBuff>();

            item.width = 30;
            item.height = 34;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item113;

            item.value = Item.buyPrice(gold: 4);
            item.rare = ItemRarityID.Orange;
            item.Calamity().devItem = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Pets/RotomRemoteGlow"));
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
