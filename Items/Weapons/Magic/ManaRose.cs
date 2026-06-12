using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ManaRose : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useAnimation = Item.useTime = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ManaBolt>();
            Item.shootSpeed = 10f;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NaturesGift).
                AddIngredient(ItemID.ManaCrystal).
                AddIngredient(ItemID.Moonglow, 5).
                AddTile(TileID.Anvils).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.JungleRose).
                AddIngredient(ItemID.ManaCrystal).
                AddIngredient(ItemID.Moonglow, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
