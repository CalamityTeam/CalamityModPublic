using Terraria.DataStructures;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LunarKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
                       Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 120;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(copper: 24);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<LunarKunaiProj>();
            Item.shootSpeed = 22f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float kunaiSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = (float)player.direction;
                mouseYDist = 0f;
                mouseDistance = kunaiSpeed;
            }
            else
            {
                mouseDistance = kunaiSpeed / mouseDistance;
            }
            mouseXDist *= mouseDistance;
            mouseYDist *= mouseDistance;
			int projAmt = player.Calamity().StealthStrikeAvailable() ? 8 : 3;
			for (int i = 0; i < projAmt; i++)
			{
				float randXOffset = mouseXDist;
				float randYOffset = mouseYDist;
				float randOffsetDampener = 0.05f * (float)i;
				randXOffset += (float)Main.rand.Next(-35, 36) * randOffsetDampener;
				randYOffset += (float)Main.rand.Next(-35, 36) * randOffsetDampener;
				mouseDistance = (float)Math.Sqrt((double)(randXOffset * randXOffset + randYOffset * randYOffset));
				mouseDistance = kunaiSpeed / mouseDistance;
				randXOffset *= mouseDistance;
				randYOffset *= mouseDistance;
				float x4 = realPlayerPos.X;
				float y4 = realPlayerPos.Y;
				int stealth = Projectile.NewProjectile(source, x4, y4, randXOffset, randYOffset, ModContent.ProjectileType<LunarKunaiProj>(), damage, knockback, player.whoAmI, 0f, 0f);
				if (stealth.WithinBounds(Main.maxProjectiles) && player.Calamity().StealthStrikeAvailable())
					Main.projectile[stealth].Calamity().stealthStrike = true;
			}
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
