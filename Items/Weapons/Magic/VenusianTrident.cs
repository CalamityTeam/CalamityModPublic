using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VenusianTrident : ModItem
    {
        public static int BaseDamage = 108;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Venusian Trident");
            Tooltip.SetDefault("Casts an infernal trident that erupts into a gigantic explosion of fire and magma shards");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.magic = true;
            item.mana = 20;
            item.width = 48;
            item.height = 48;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 9f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item45;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VenusianBolt>();
            item.shootSpeed = 19f;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.InfernoFork);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 2);
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
