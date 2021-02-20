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
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 52;
            item.damage = 73;
            item.knockBack = 8.5f;
            item.magic = true;
            item.mana = 26;
            item.useTime = ChargeFrames + CooldownFrames;
            item.useAnimation = ChargeFrames + CooldownFrames;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.NPCDeath7;
            item.useTurn = false;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.value = Item.buyPrice(gold: 36);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<ValkyrieRayStaff>();
            item.shootSpeed = 25f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 12);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 6);
            recipe.AddIngredient(ItemID.Ruby);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
