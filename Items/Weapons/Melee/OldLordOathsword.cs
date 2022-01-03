using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class OldLordOathsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Lord Oathsword");
            Tooltip.SetDefault("A relic of the ancient underworld\n" +
                "Holding right click rapidly absorbs energy into the blade until it is sufficiently charged\n" +
                "Left clicking will either swing the blade as usual or cause you to fly in the direction of the cursor, depending on if the blade was fully charged\n" +
                "After flying the amount of charge the blade has is reduced to zero again");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.width = 70;
            item.height = 70;
            item.melee = true;
            item.useAnimation = 34;
            item.useTime = 34;
            item.channel = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.shoot = ProjectileID.PurificationPowder;
            item.useTurn = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool? CanHitNPC(Player player, NPC target) => false;

        public override bool CanHitPvp(Player player, Player target) => false;

        public static void CheckIfBladeShouldBeHeld(Player player)
        {
            Item heldItem = player.ActiveItem();
            if (heldItem.type != ModContent.ItemType<OldLordOathsword>())
                return;

            player.Calamity().bladeArmEnchant = true;
            bool bladeIsPresent = false;
            int holdoutType = ModContent.ProjectileType<OldLordOathswordProj>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != holdoutType || Main.projectile[i].owner != player.whoAmI || !Main.projectile[i].active)
                    continue;

                bladeIsPresent = true;
                break;
            }

            if (Main.myPlayer == player.whoAmI && !bladeIsPresent)
            {
                int damage = (int)(heldItem.damage * player.MeleeDamage());
                float kb = player.GetWeaponKnockback(heldItem, heldItem.knockBack);
                Projectile.NewProjectile(player.Center, Vector2.Zero, holdoutType, damage, kb, player.whoAmI);
            }
        }
    }
}
