using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ShiftingSands : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shifting Sands");
            Tooltip.SetDefault("Casts a sand shard that follows the mouse cursor");
        }

        public override void SetDefaults()
        {
            item.damage = 61;
            item.knockBack = 5f;
            item.useTime = item.useAnimation = 30;
            item.mana = 20;
            item.magic = true;
            item.channel = true;
            item.autoReuse = true;
            item.shootSpeed = 7f;
            item.shoot = ModContent.ProjectileType<ShiftingSandsProj>();

            item.width = item.height = 58;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item20;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
        }

        public override bool CanUseItem(Player player)
        {
            int projCount = 0;
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj.active && proj.owner == player.whoAmI && proj.type == item.shoot && proj.ai[0] <= 0f)
                {
                    projCount++;
                }
            }
            return projCount <= 0;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MagicMissile);
            recipe.AddIngredient(ModContent.ItemType<GrandScale>());
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.SpectreBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
