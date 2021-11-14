using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class DemonicPitchfork : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonic Pitchfork");
            Tooltip.SetDefault("Fires a demonic pitchfork");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 82;
            item.magic = true;
            item.mana = 11;
            item.width = 52;
            item.height = 48;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DemonicPitchforkProj>();
            item.shootSpeed = 16f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>(), 15);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
