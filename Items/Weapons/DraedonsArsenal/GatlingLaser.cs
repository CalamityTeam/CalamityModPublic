using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GatlingLaser : ModItem
    {
        // This is the amount of charge consumed every time the holdout projectile fires a laser.
        public const float HoldoutChargeUse = 0.0075f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gatling Laser");
            Tooltip.SetDefault("Large laser cannon used primarily by Yharim's fleet and base defense force");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 43;
            Item.height = 24;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 43;
            Item.knockBack = 1f;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.channel = true;
            Item.mana = 6;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GatlingLaserFireStart");
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            Item.shoot = ModContent.ProjectileType<GatlingLaserProj>();
            Item.shootSpeed = 24f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 135f;
            modItem.ChargePerUse = 0f; // This weapon is a holdout. Charge is consumed by the holdout projectile.
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<GatlingLaserProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15).AddIngredient(ModContent.ItemType<DubiousPlating>(), 15).AddIngredient(ModContent.ItemType<BarofLife>(), 5).AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
