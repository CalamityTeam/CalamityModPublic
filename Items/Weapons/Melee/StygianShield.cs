using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [AutoloadEquip(EquipType.Shield)]
    public class StygianShield : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        
        // Held stats
        public const int HeldDefense = 12;
        public const int DisableDashDuration = 120;

        public int ThrownShieldID = ModContent.ProjectileType<StygianShieldThrown>();

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HeldDefense, DisableDashDuration / 60);

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 78;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.damage = 200;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useAnimation = Item.useTime = 40; // This is only for the "Very slow" tooltip. The real use time should be faster
            Item.shoot = ModContent.ProjectileType<StygianShieldAttack>();
            Item.shootSpeed = 10f;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true; // Donor: Cin2Win
            Item.UseSound = null;
        }

        // Can only throw a shield if none is active
        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[ThrownShieldID] <= 0;

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
            if (Player.ActiveItem().type == ModContent.ItemType<StygianShield>())
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
            if (Player.ActiveItem().type == ModContent.ItemType<StygianShield>())
            {
                Player.shield = EquipLoader.GetEquipSlot(Mod, "StygianShield", EquipType.Shield);
			    Player.cShield = 0;
            }
        }
    }
}
