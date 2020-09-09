using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Mishiro Usui/Amber Sienna
    public class ProfanedSoulCrystal : ModItem
    {

        /**
         * Notes: Drops from providence if the only damage source during the fight is from typeless damage or the profaned soul and the owners of those babs do not have profaned crystal.
         * All projectiles are in ProfanedSoulCrystalProjectiles.cs in the summon projectile directory
         * The projectiles being created/fired on click happens in CalamityGlobalItem (there's a region specially for it so ctrl + f is your friend)
         * the day/night buffs are in calamityplayermisceffects
         * the bab projectiles are the same, just refactored ai to be more adhering to DRY principle
         * bab spears being fired happens at the bottom of calplayer
         * Animation of legs is postupdate, animation of wings is frameeffects.
         * Projectiles transformed are ONLY affected by alldamage and summon damage bonuses, likewise the weapon's base damage/usetime is NOT taken into account.
         * You enrage below or at 50% hp.
         */
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Soul Crystal");
            Tooltip.SetDefault("Transforms you into an emissary of the profaned goddess\n" +
                "Requires 10 minion slots to use in order to grant the following effects\n" +
                "All non-summon weapons are converted into powerful summon variations\n" +
                "Falling below 50% life will empower these attacks\n" +
                "[c/f05a5a:Transforms Melee attacks into a barrage of spears]\n" +
                "[c/3a83e4:Transforms Magic attacks into a powerful splitting fireball]\n" +
                "[c/85e092:Transforms Ranged attacks into a flurry of fireballs and meteors]\n" +
                "[c/e97451:Transforms Rogue attacks into a deadly crystalline spiral]\n" +
                "Summons and empowers the profaned babs to fight alongside you\n" +
                "You are no longer affected by burn out when hit\n" +
                "Provides buffs depending on the time of day\n" +
                "Provides heat and cold protection in Death Mode\n" +
				"Thinking back, it was a boring life\n" +
				"[c/FFBF49:And so we burn it all in the name of purity]");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 4));
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.accessory = true;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            return !player.Calamity().pArtifact;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!CalamityWorld.downedSCal)
            {
                int index = 0;
                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name.StartsWith("Tooltip"))
                    {
                        if (line.Name == "Tooltip0")
                        {
                            index = tooltips.IndexOf(line);
                        } 
                        else
                        {
                            line.text = "";
                        }
                    }
                    else if (line.mod == "Terraria" && line.text.Contains("Sell price"))
                    {
                        line.text = "";
                    }
                        
                }
                
                tooltips.Insert(index+1, new TooltipLine(CalamityMod.Instance, "Tooltip1", "[c/f05a5a:The soul within this crystal has been defiled by the powerful magic of a supreme witch]\nMerchants will reject a defiled soul such as this."));
            }
            else if (Main.player[Main.myPlayer].Calamity().profanedCrystalBuffs)
            {
                int manaCost = (int)(100 * Main.player[Main.myPlayer].manaCost);
                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name == "Tooltip5")
                    {
                        line.text = "[c/3a83e4:Transforms Magic attacks into a powerful splitting fireball for " + manaCost + " mana per cast]";
                    }
                }
			}
			if (CalamityWorld.downedSCal)
			{
				if (!CalamityWorld.death)
				{
					foreach (TooltipLine line in tooltips)
					{
						if (line.mod == "Terraria" && line.Name == "Tooltip11")
						{
							line.text = "";
						}
					}
				}
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            modPlayer.pArtifact = true;
            modPlayer.profanedCrystal = true;

            if (hideVisual)
                modPlayer.profanedCrystalHide = true;
        }

        public override void AddRecipes()
        {
            PSCRecipe recipe = new PSCRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 5);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 25);
            recipe.AddIngredient(ModContent.ItemType<ProfanedSoulArtifact>());
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 50);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 100);
            recipe.AddIngredient(ItemID.ObsidianRose);
            recipe.AddTile(ModContent.TileType<ProfanedBasin>());
            recipe.needLava = true;
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class PSCRecipe : ModRecipe
    {

        public PSCRecipe(Mod mod) : base(mod) { }

        public override int ConsumeItem(int type, int numRequired)
        {
            int shadowSpec = ModContent.ItemType<ShadowspecBar>();
            int geode = ModContent.ItemType<DivineGeode>();
            int essence = ModContent.ItemType<UnholyEssence>();
            bool biomePower = Main.LocalPlayer.ZoneHoly || Main.LocalPlayer.ZoneUnderworldHeight;
            return biomePower && (type == (shadowSpec | geode | essence)) ? numRequired / 2 : numRequired; //cuts the above mats consumed by half if in the biomes instead of arbitrary biome locking
        }
    }

    public class ProfanedCrystalHead : EquipTexture
    {
        public override bool DrawHead()
        {
            return false;
        }
    }

    public class ProfanedCrystalBody : EquipTexture
    {
        public override bool DrawBody()
        {
            return false;
        }
    }

    public class ProfanedCrystalLegs : EquipTexture
    {
        public override bool DrawLegs()
        {
            return false;
        }
    }

    public class ProfanedCrystalWings : EquipTexture
    {
    }
}
