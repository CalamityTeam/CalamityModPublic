using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SHPC : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SHPC");
            Tooltip.SetDefault("Fires plasma orbs that linger and emit massive explosions\n" +
                "Right click to fire powerful energy beams");
        }

        public override void SetDefaults()
        {
            item.damage = 18;
            item.magic = true;
            item.mana = 20;
            item.width = 124;
            item.height = 52;
            item.useTime = item.useAnimation = 7;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.UseSound = SoundID.Item92;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SHPB>();
            item.shootSpeed = 20f;

            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-35, -10);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            }
            else
            {
                item.UseSound = SoundID.Item92;
            }
            return base.CanUseItem(player);
        }

		public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
		{
			if (player.altFunctionUse == 2)
				mult *= 0.3f;
		}

		public override float UseTimeMultiplier	(Player player)
		{
			if (player.altFunctionUse == 2)
				return 1f;
			return 0.14f;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                for (int shootAmt = 0; shootAmt < 3; shootAmt++)
                {
                    float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
                    float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SHPL>(), damage, knockBack * 0.5f, player.whoAmI, 0f, 0f);
                }
                return false;
            }
            else
            {
                for (int shootAmt = 0; shootAmt < 3; shootAmt++)
                {
                    float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
                    float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SHPB>(), (int)(damage * 1.1), knockBack, player.whoAmI, 0f, 0f);
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PlasmaDriveCore>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SuspiciousScrap>(), 4);
            recipe.AddRecipeGroup("AnyMythrilBar", 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
