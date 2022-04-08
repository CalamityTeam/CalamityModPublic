using Terraria.DataStructures;
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
            Item.width = 62;
            Item.height = 34;
            Item.damage = 857;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;

            // Reduce volume to 30% so it stops destroying people's ears.
            var sound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/LargeWeaponFire");
            Item.UseSound = sound?.WithVolume(0.3f);

            Item.shoot = ModContent.ProjectileType<HighExplosivePeanutShell>();
            Item.shootSpeed = 13f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Item.shoot;
            return true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-17, 5);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PearlGod>()).AddIngredient(ModContent.ItemType<RuinousSoul>(), 5).AddIngredient(ItemID.LunarBar, 15).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
