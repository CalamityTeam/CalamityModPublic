using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
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
            DisplayName.SetDefault("Prototype Andromecha Ring");
            Tooltip.SetDefault("Summons an collosal controllable mech\n" +
                               "Right click to display the mech's control panel\n" +
                               "The panel has 3 possible configurations, that you can choose using the brackets on the UI\n" +
                               "Each bracket powers 2 out of 3 functions, represented by icons on the UI.\n" +
                               "The bottom left icon shrinks the mech to the size of a player when powered, but weakens its attacks. \n" +
                               "The bottom right icon charges when powered. At full charge, you can click it to release bursts of branching lightning \n" +
                               "The top icon alternates between the 2 the primary attacks when clicked. \n" +
                               "The primary attack replaces your normal attacks. You can only use the primary attack if the top icon is powered\n" +
                               "The melee mode slashes enemies with Regicide, an enormous energy blade\n" +
                               "The ranged mode releases quickly 3 laser beams. Only one laser is shot if the mech is shrunk\n" +
                               "Exiting the mount while a boss is alive will temporarily hinder your movement\n" +
                               "The remains of what could have been, forged into a deadly exosuit\n" +
                                "[c/87ceeb:Now, make them pay.]");
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

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 100);
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
