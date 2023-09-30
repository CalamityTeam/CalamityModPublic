using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MagnaCannon : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle ChargeFull = new("CalamityMod/Sounds/Item/MagnaCannonChargeFull") { Volume = 0.5f };
        internal static readonly int ChargeFullSoundFrames = 42;
        public static readonly SoundStyle ChargeLoop = new("CalamityMod/Sounds/Item/MagnaCannonChargeLoop") { Volume = 0.5f };
        internal static readonly int ChargeLoopSoundFrames = 153;
        public static readonly SoundStyle ChargeStart = new("CalamityMod/Sounds/Item/MagnaCannonChargeStart") { Volume = 0.5f };
        public static readonly SoundStyle Fire = new("CalamityMod/Sounds/Item/MagnaCannonShot") { PitchVariance = 0.3f, Volume = 0.4f };

        public static int AftershotCooldownFrames = 30;
        public static int FullChargeFrames = 138; //126 frames is durration of charge sound

        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 34;
            Item.useTime = Item.useAnimation = AftershotCooldownFrames;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<MagnaCannonHoldout>();
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Granite, 25).
                AddIngredient(ItemID.MeteoriteBar, 12).
                AddIngredient(ItemID.Diamond, 3).
                AddIngredient(ItemID.Sapphire, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
