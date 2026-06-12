using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("DivineHatchet")]
    public class SeekingScorcher : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle ThrowSound = new("CalamityMod/Sounds/Item/SwingMid") { Volume = 0.5f, Pitch = -0.35f, PitchVariance = 0.1f };
        public static readonly SoundStyle HitSound =  new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastImpact") { Volume = 0.5f, Pitch = 0.2f, PitchVariance = 0.2f };
        public static readonly SoundStyle ShatterSound =  new("CalamityMod/Sounds/Item/BlazingCoreParry") { Volume = 0.4f, PitchVariance = 0.2f };
        public static readonly SoundStyle LightShatterSound = new("CalamityMod/Sounds/NPCKilled/CrownJewelShatter") { Pitch = 0.4f, PitchVariance = 0.3f };
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
        }

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 64;
            Item.damage = 1170;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useAnimation = Item.useTime = 55;
            Item.knockBack = 8.5f;
            Item.shoot = ModContent.ProjectileType<SeekingScorcherProj>();
            Item.shootSpeed = 12f;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PossessedHatchet).
                AddIngredient<DivineGeode>(5).
                AddIngredient<UnholyEssence>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
