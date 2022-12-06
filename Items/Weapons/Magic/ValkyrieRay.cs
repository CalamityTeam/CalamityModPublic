using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ValkyrieRay : ModItem
    {
        // The base use time of the weapon is 47: 28 charge frames + 19 cooldown frames.
        // The rate at which it progresses through its charge and discharge cycle is dynamically sped up by reforges.
        // This math is handled in its holdout projectile, ValkyrieRayStaff.
        public const int ChargeFrames = 28;
        public const int CooldownFrames = 19;
        public const float GemDistance = 18f;
        public static readonly Color LightColor = new Color(235, 40, 121);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valkyrie Ray");
            Tooltip.SetDefault("Casts a devastating ray of holy power");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 52;
            Item.damage = 53;
            Item.knockBack = 8.5f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 26;
            Item.useTime = ChargeFrames + CooldownFrames;
            Item.useAnimation = ChargeFrames + CooldownFrames;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.NPCDeath7;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<ValkyrieRayStaff>();
            Item.shootSpeed = 25f;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 12).
                AddIngredient<AerialiteBar>(6).
                AddIngredient(ItemID.Ruby).
                AddIngredient(ItemID.SoulofFright).
                AddIngredient(ItemID.SoulofMight).
                AddIngredient(ItemID.SoulofSight).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
