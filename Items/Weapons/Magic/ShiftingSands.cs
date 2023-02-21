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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 92;
            Item.knockBack = 5f;
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 20;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<ShiftingSandsProj>();

            Item.width = Item.height = 58;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item20;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MagicMissile).
                AddIngredient<GrandScale>().
                AddIngredient(ItemID.AncientBattleArmorMaterial, 2).
                AddIngredient(ItemID.SpectreBar, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
