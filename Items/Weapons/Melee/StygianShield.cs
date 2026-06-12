using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [AutoloadEquip(EquipType.Shield)]
    public class StygianShield : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static readonly SoundStyle DashChargeSound = new("CalamityMod/Sounds/Item/StygianDashCharge");
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Item/StygianDash");
        public static readonly SoundStyle DashHitSound = new("CalamityMod/Sounds/Item/StygianBonk", 3);
        public static readonly SoundStyle ShieldThrowSound = new("CalamityMod/Sounds/Item/StygianThrow");
        public static readonly SoundStyle ThrowLoopSound = new("CalamityMod/Sounds/Item/StygianThrowLoop");
        public static readonly SoundStyle ShieldThrowHitSound = CommonCalamitySounds.ExoHitSound;
        public static readonly SoundStyle ShieldCatchSound = new("CalamityMod/Sounds/Item/StygianCatch");

        // Held stats
        public const int HeldDefense = 8;
        public const int DisableDashDuration = 90;

        public int ThrownShieldID = ModContent.ProjectileType<StygianShieldThrown>();

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HeldDefense, (DisableDashDuration / 60D).ToString("N1"));

        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 78;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.damage = 180;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useAnimation = Item.useTime = 40; // This is only for the "Very slow" tooltip. The real use time should be faster
            Item.shoot = ModContent.ProjectileType<StygianShieldAttack>();
            Item.shootSpeed = 10f;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true; // Donor: Cin2Win
            Item.UseSound = null;
        }

        // Can only throw a shield if there's two or less
        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[ThrownShieldID] <= 1;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override float UseSpeedMultiplier(Player player) => 2.5f;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ThrownShieldID;
                knockback *= 0f;
            }
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<OrnateShield>().
                AddIngredient<ScoriaBar>(6).
                AddIngredient(ItemID.HellstoneBar, 6).
                AddIngredient<LivingShard>(6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class StygianShieldPlayer : ModPlayer
    {
        // This loading behavior functions similarly to the Sergeant United Shield, which also draws a shield when held
        public int disableDashTimer = 0;

        public override void UpdateEquips()
        {
            if (Player.HeldItem.type == ModContent.ItemType<StygianShield>())
            {
                Player.hasRaisableShield = true;
                Player.statDefense += StygianShield.HeldDefense;
                Player.noKnockback = true;
                disableDashTimer = StygianShield.DisableDashDuration;
            }

            if (disableDashTimer > 0)
            {
                Player.Calamity().blockAllDashes = true;
                disableDashTimer--;
            }
            else if (Player.dead || !Player.active)
                disableDashTimer = 0;
        }

        // Overrides every other shield in accessories
        public override void UpdateVisibleVanityAccessories()
        {
            if (Player.HeldItem.type == ModContent.ItemType<StygianShield>())
            {
                Player.shield = EquipLoader.GetEquipSlot(Mod, "StygianShield", EquipType.Shield);
                Player.cShield = 0;
            }
        }
    }
}
