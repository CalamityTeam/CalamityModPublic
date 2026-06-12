using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ShiftingSands : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = Item.height = 50;
            Item.damage = 81;
            Item.knockBack = 5f;
            Item.useAnimation = Item.useTime = 30;
            Item.mana = 20;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<ShiftingSandsProj>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item20;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override bool CanUseItem(Player player) => !player.channel;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ItemID.AncientCloth, 5).
                AddIngredient<GrandScale>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
