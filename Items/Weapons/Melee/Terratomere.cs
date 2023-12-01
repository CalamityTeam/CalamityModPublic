using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Terratomere : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const int SwingTime = 83;

        public const int SlashLifetime = 27;

        public const int SmallSlashCreationRate = 9;

        public const int TrueMeleeHitHeal = 4;

        public const int TrueMeleeGlacialStateTime = 30;

        public const float SmallSlashDamageFactor = 0.4f;

        public const float ExplosionExpandFactor = 1.013f;

        public const float TrailOffsetCompletionRatio = 0.2f;

        public static readonly Color TerraColor1 = new(141, 203, 50);

        public static readonly Color TerraColor2 = new(83, 163, 136);

        public static readonly SoundStyle SwingSound = new("CalamityMod/Sounds/Item/TerratomereSwing");

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 66;
            Item.damage = 370;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useTime = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<TerratomereHoldoutProj>();
            Item.shootSpeed = 60f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Floodtide>().
                AddIngredient<Hellkite>().
                AddIngredient(ItemID.TerraBlade).
                AddIngredient<UelibloomBar>(7).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
