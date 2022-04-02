using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DodusHandcannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dodu's Handcannon");
            Tooltip.SetDefault("The power of the nut rests in your hands\n" +
                "Fires high explosive peanut shells, literally");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.height = 34;
            item.damage = 857;
            item.ranged = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;

            // Reduce volume to 30% so it stops destroying people's ears.
            var sound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire");
            item.UseSound = sound?.WithVolume(0.3f);

            item.shoot = ModContent.ProjectileType<HighExplosivePeanutShell>();
            item.shootSpeed = 13f;
            item.useAmmo = AmmoID.Bullet;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().donorItem = true;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = item.shoot;
            return true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-17, 5);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PearlGod>());
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
