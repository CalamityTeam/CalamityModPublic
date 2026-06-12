using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("TheEmpyrean", "GodsBellows")]
    public class DeadSunsWind : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/DeadSunShot") { PitchVariance = 0.35f, Volume = 0.4f };
        public static readonly SoundStyle Ricochet = new("CalamityMod/Sounds/Item/DeadSunRicochet") { Volume = 0.35f };
        public static readonly SoundStyle Explosion = new("CalamityMod/Sounds/Item/DeadSunExplosion") { Volume = 0.5f };
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 24;
            Item.damage = 115;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = ShootSound;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmicFire>();
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldBlob>(18).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
