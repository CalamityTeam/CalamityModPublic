using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("TheEmpyrean", "GodsBellows")]
    public class DeadSunsWind : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static readonly SoundStyle Shoot = new("CalamityMod/Sounds/Item/DeadSunShot") { PitchVariance = 0.35f, Volume = 0.4f };
        public static readonly SoundStyle Ricochet = new("CalamityMod/Sounds/Item/DeadSunRicochet") { Volume = 0.35f };
        public static readonly SoundStyle Explosion = new("CalamityMod/Sounds/Item/DeadSunExplosion") { Volume = 0.5f };
        public override void SetDefaults()
        {
            Item.damage = 182;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 24;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = Shoot;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmicFire>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldConstruct>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
