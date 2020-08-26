using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PrototypeAndromechaRing : ModItem
    {
        public const int CrippleTime = 360; // 6 seconds
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flamsteed Ring");
            Tooltip.SetDefault("Summons a colossal controllable mech\n" +
			"Right click to display the mech's control panel\n" +
			"The panel has 3 configurations, selected using the brackets on the edges of the UI\n" +
			"Each bracket powers 2 out of 3 possible functions, represented by the circular icons.\n" +
			"The bottom left icon miniaturizes the mech to the size of a player, but weakens its weapons.\n" +
			"The bottom right icon is a powerful jet booster which greatly enhances movement.\n" +
			"The top icon is the mech's weaponry. It must be powered in order to attack.\n" +
			"Click the top icon to switch between Regicide, an enormous energy blade, and a powerful Gauss rifle.\n" +
			"Exiting the mount while a boss is alive will temporarily hinder your movement\n" +
			"THIS LINE IS MODIFIED BELOW\n" +
			"This one too");
        }

        public override void SetDefaults()
        {
            item.mana = 200;
            item.damage = 9999;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.width = item.height = 28;
            item.useTime = item.useAnimation = 10;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
            item.rare = 10;
            item.UseSound = SoundID.Item117;
            item.shoot = ModContent.ProjectileType<GiantIbanRobotOfDoom>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			Mod crouchMod = ModLoader.GetMod("CrouchMod");
			bool crouch = crouchMod != null;
			string crouchWarn = "Big, oversized robots can't crouch. Please don't try.";
			string flavor = CalamityUtils.ColorMessage("Now, make them pay.", new Color(135, 206, 235));
			foreach (TooltipLine line2 in list)
			{
				if (line2.mod == "Terraria" && line2.Name == "Tooltip9")
				{
					line2.text = crouch ? crouchWarn : flavor;
				}
				if (line2.mod == "Terraria" && line2.Name == "Tooltip10")
				{
					line2.text = crouch ? flavor : "";
				}
			}
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Excelsus>(), 4);
            recipe.AddIngredient(ModContent.ItemType<CosmicViperEngine>());
            recipe.AddIngredient(ItemID.WingsVortex);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player) => !(player.Calamity().andromedaCripple > 0 && CalamityPlayer.areThereAnyDamnBosses);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // If the player has any robots, kill them all.
            if (player.ownedProjectileCounts[item.shoot] > 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && 
                        Main.projectile[i].type == item.shoot &&
                        Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                if (CalamityPlayer.areThereAnyDamnBosses)
                {
                    player.Calamity().andromedaCripple = CrippleTime;
                    player.AddBuff(ModContent.BuffType<AndromedaCripple>(), player.Calamity().andromedaCripple);
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AdrenalineBurnout1"), position);
                }
                return false;
            }
            // Otherwise create one.
            return true;
        }
    }
}
