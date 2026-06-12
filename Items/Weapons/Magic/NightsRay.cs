using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class NightsRay : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 54;
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item72;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NightsRayBeam>();
            Item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Vilethorn).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ItemID.WandofFrosting).
                AddIngredient(ItemID.AmberStaff).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                AddCondition(Condition.NotRemixWorld).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.CrimsonRod).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ItemID.WandofFrosting).
                AddIngredient(ItemID.AmberStaff).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                AddCondition(Condition.NotRemixWorld).
                Register();

            // CIT 16NOV2024: Due to Wand of Sparking (and Frosting) being swapped with Magic Dagger in Remix, Night's Ray uses Magic Dagger in its recipe there.
            CreateRecipe().
                AddIngredient(ItemID.Vilethorn).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ItemID.MagicDagger).
                AddIngredient(ItemID.AmberStaff).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                AddCondition(Condition.RemixWorld).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.CrimsonRod).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ItemID.MagicDagger).
                AddIngredient(ItemID.AmberStaff).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                AddCondition(Condition.RemixWorld).
                Register();

        }
    }
}
