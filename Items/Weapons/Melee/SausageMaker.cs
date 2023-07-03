using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SausageMaker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.damage = 36;
            Item.DamageType = TrueMeleeDamageClass.Instance;
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
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<SausageMakerSpear>();
            Item.shootSpeed = 6f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimtaneBar, 4).
                AddIngredient<BloodSample>(12).
                AddIngredient(ItemID.Vertebrae, 4).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
