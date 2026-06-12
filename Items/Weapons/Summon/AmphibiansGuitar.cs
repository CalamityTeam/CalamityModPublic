using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AmphibiansGuitar : ModItem, ILocalizedModType
    {
        public override string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 64;
            Item.damage = 54;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<AmphibiansGuitarHoldout>();

            Item.useAnimation = Item.useTime = 36;
            Item.mana = 10;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TheAxe).
                AddIngredient(ItemID.Frog).
                AddIngredient<CoreofCalamity>(3).
                AddIngredient<LivingShard>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
