using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class OpalStriker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static readonly SoundStyle Charge = new("CalamityMod/Sounds/Item/OpalCharge") { Volume = 0.5f};
        public static readonly SoundStyle ChargeLoop = new("CalamityMod/Sounds/Item/OpalChargeLoop") { Volume = 0.5f};
        internal static readonly int ChargeLoopSoundFrames = 120;
        public static readonly SoundStyle Fire = new("CalamityMod/Sounds/Item/OpalFire") { PitchVariance = 0.4f, Volume = 0.3f };
        public static readonly SoundStyle ChargedFire = new("CalamityMod/Sounds/Item/OpalChargedFire") { PitchVariance = 0.3f, Volume = 0.6f };

        public static int AftershotCooldownFrames = 17;
        public static int FullChargeFrames = 88;

        public override void SetDefaults()
        {
            Item.damage = 37;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 30;
            Item.useTime = Item.useAnimation = AftershotCooldownFrames;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<OpalStrikerHoldout>();
            Item.shootSpeed = 12f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Marble, 25).
                AddIngredient(ItemID.MeteoriteBar, 10).
                AddIngredient(ItemID.Diamond, 3).
                AddIngredient(ItemID.Amber, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
