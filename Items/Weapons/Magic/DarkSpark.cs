using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class DarkSpark : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useAnimation = Item.useTime = 10;
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<DarkSparkPrism>();
            Item.shootSpeed = 30f;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LastPrism).
                AddIngredient<DarkPlasma>(10).
                AddIngredient<RuinousSoul>(20).
                AddIngredient<DivineGeode>(30).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
