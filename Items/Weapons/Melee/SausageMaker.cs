using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SausageMaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sausage Maker");
            Tooltip.SetDefault("Sprays homing blood on hit");
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.damage = 36;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 42;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<SausageMakerSpear>();
            Item.shootSpeed = 6f;
            Item.Calamity().trueMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BloodSample>(), 8).AddIngredient(ItemID.Vertebrae, 4).AddIngredient(ItemID.CrimtaneBar, 5).AddTile(TileID.DemonAltar).Register();
        }
    }
}
