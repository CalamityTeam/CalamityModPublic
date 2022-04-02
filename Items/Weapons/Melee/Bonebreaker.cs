using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Bonebreaker : ModItem
    {
        public const int BaseDamage = 60;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonebreaker");
            Tooltip.SetDefault("Fires javelins that stick to enemies before bursting into shrapnel");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 15;
            item.useTime = 15;
            item.width = 32;
            item.height = 32;
            item.damage = BaseDamage;
            item.melee = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<BonebreakerProjectile>();
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BoneJavelin, 150);
            recipe.AddIngredient(ModContent.ItemType<CorrodedFossil>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
