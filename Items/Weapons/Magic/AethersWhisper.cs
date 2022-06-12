using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AethersWhisper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether's Whisper");
            Tooltip.SetDefault("Inflicts long-lasting shadowflame and splits on tile hits\n" +
                "Projectiles gain damage as they travel");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 504;
            Item.knockBack = 5.5f;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<AetherBeam>();
            Item.mana = 30;
            Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;

            Item.width = 134;
            Item.height = 44;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlasmaRod>().
                AddIngredient<Lazhar>().
                AddIngredient<TwistingNether>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
