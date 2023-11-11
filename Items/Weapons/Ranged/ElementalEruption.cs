using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ElementalEruption : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public int FlareCounter = 0;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 83;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 34;
            Item.useAnimation = Item.useTime = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElementalEruptionHoldout>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Gel;
            Item.channel = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
       
        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TerraFlameburster>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
