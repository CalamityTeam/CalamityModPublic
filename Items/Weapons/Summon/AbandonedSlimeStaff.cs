using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
	public class AbandonedSlimeStaff : ModItem
    {
		int slimeSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abandoned Slime Staff");
            Tooltip.SetDefault("Cast down from the heavens in disgust, this relic sings a song of quiet tragedy...\n" +
                               "Consumes all of the remaining minion slots on use\n" +
							   "Must be used from the hotbar\n" +
                               "Increased power and size based on the number of minion slots used\n" +
							   "Holding this weapon grants 20% increased jump speed");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.height = 62;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item44;

            item.summon = true;
            item.mana = 40;
            item.damage = 56;
            item.knockBack = 3f;
            item.useTime = item.useAnimation = 20;
            item.shoot = ModContent.ProjectileType<AstrageldonSummon>();
            item.shootSpeed = 10f;

            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.Calamity().customRarity = CalamityRarity.Dedicated; //rarity 21
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            bool autoJump = Main.player[Main.myPlayer].autoJump;
			string jumpAmt = autoJump ? "5" : "20";
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip4")
                {
                    line2.text = "Holding this weapon grants " + jumpAmt + "% increased jump speed";
                }
            }
        }

		public override void HoldItem(Player player)
        {
			//same boost as Aero Stone
			player.jumpSpeedBoost += player.autoJump ? 0.25f : 1f;

			double minionCount = 0;
			for (int j = 0; j < Main.projectile.Length; j++)
			{
                Projectile projectile = Main.projectile[j];
				if (projectile.active && projectile.owner == player.whoAmI && projectile.minion && projectile.type != ModContent.ProjectileType<AstrageldonSummon>())
				{
					minionCount += projectile.minionSlots;
				}
			}
			slimeSlots = (int)(player.maxMinions - minionCount);
		}

        public override bool CanUseItem(Player player)
		{
			return slimeSlots >= 1;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			CalamityUtils.KillShootProjectiles(true, type, player);
			float damageMult = ((float)Math.Log(slimeSlots, 8f)) + 1f;
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            int slime = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * damageMult), knockBack, player.whoAmI);
			Main.projectile[slime].minionSlots = slimeSlots;
            return false;
        }
    }
}
